using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour {
    public const int kMaxPlacementAttempts = 100;

    protected static GameManager sInstance;
    public static GameManager Instance { get { return sInstance; } }

    public GameObject mPlayerRosePrefab;
    public GameObject mPlayerVuPrefab;
    public GameObject mGoalPrefab;

    public GameObject mCollectablePrefab;

    public Dungeon mDungeon;
    public FollowCamera mFollowCamera;
    public Fader mFader;
    
    public bool mbUseTestGenerationData = true;
    public DungeonGenerationData mTestGenerationData;
    protected DungeonGenerationData mCurrentLevelData;

    public LevelData mShopLevelData;
    public LevelData mReviveZoneLevelData;
    public LevelData mFlowerBossLevelData;
    public LevelData mChocolateBossLevelData;
    public LevelData mImposterVuBossLevelData;
    public LevelData mImposterRoseBossLevelData;

    public LevelData mWinVuLevelData;
    public LevelData mWinRoseLevelData;

    public LevelData mTestData;

    protected Goal mGoal;
    protected Vector3 mStartPos;
    protected PlayerController mPlayer1;
    public PlayerController Player1 { get { return mPlayer1; } }

    protected List<GameObject> mCollectables = new List<GameObject>();
    protected List<EnemyController> mEnemies = new List<EnemyController>();
    protected List<GameObject> mSpecialObjects = new List<GameObject>();

    protected void Awake()
    {
        if(sInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        sInstance = this;

        if (mDungeon == null)
        {
            mDungeon = FindObjectOfType<Dungeon>();
        }

        if (mFollowCamera == null)
        {
            mFollowCamera = FindObjectOfType<FollowCamera>();
        }

        if(mFader == null)
        {
            mFader = FindObjectOfType<Fader>();
        }

        GameObject goalObj = SpawnPrefab(mGoalPrefab);
        mGoal = goalObj.GetComponent<Goal>();
        GameObject playerObj = SpawnPrefab(GlobalData.sSelectedCharacter == SelectedCharacter.Rose ? mPlayerRosePrefab : mPlayerVuPrefab);
        mPlayer1 = playerObj.GetComponent<PlayerController>();
        mFollowCamera.Init(mPlayer1.transform);
    }

	protected void Start () 
    {
        if(mTestData != null)
        {
            LoadLevel(mTestData);
        }
        else
        {
            GenerateLevel();
        }

        SoundManager.Instance.FadeIn(.25f);
        SoundManager.Instance.PlayBgm(SoundManager.Instance.bgm_main, true);
        mFader.FadeIn();

        Signal.Register(SignalType.LevelComplete, OnLevelComplete);
        Signal.Register(SignalType.StartNextLevel, OnStartNextLevel);
        Signal.Register(SignalType.RestartLevel, OnRestartLevel);
        Signal.Register(SignalType.PlayerDeath, OnPlayerDeath);
        Signal.Register(SignalType.BossDefeated, OnBossDefeated);
        Signal.Register(SignalType.FinalBossLevelCompleted, OnFinalBossLevelCompleted);
	}

    protected void OnDestroy()
    {
        if (sInstance == this)
        {
            Signal.Unregister(SignalType.LevelComplete, OnLevelComplete);
            Signal.Unregister(SignalType.StartNextLevel, OnStartNextLevel);
            Signal.Unregister(SignalType.RestartLevel, OnRestartLevel);
            Signal.Unregister(SignalType.PlayerDeath, OnPlayerDeath);
            Signal.Unregister(SignalType.BossDefeated, OnBossDefeated);
            Signal.Unregister(SignalType.FinalBossLevelCompleted, OnFinalBossLevelCompleted);
            sInstance = null;
        }
    }

    public GameObject SpawnPrefab(GameObject prefab)
    {
        return SpawnPrefab(prefab, Vector3.zero, Quaternion.identity);
    }

    public GameObject SpawnPrefab(GameObject prefab, Vector3 pos, Quaternion rotation)
    {
        GameObject obj = Instantiate(prefab, pos, rotation) as GameObject;
        obj.transform.SetParent(transform);
        return obj;
    }

    public Room GetRandomRoom()
    {
        int startIndex = mDungeon.Rooms.Count > 1 ? 1 : 0;
        return mDungeon.Rooms[Random.Range(startIndex, mDungeon.Rooms.Count)];
    }

    public Vector3 FindGoalSpawnPosition()
    {
        Room randRoom = GetRandomRoom();
        Vector2i randomPosInRoom = randRoom.GetRandomPos();
        return mDungeon.GetTilePosition(randomPosInRoom.mX, randomPosInRoom.mY);
    }

    public void LoadLevel(LevelData data)
    {
        LoadLevel(data.mData, data.mPlayerCell, data.mGoalCell, data.mGoalSignalType, data.mbGoalActiveAtStart, data.mSpawnDatas);
    }

    public void LoadLevel(string data, Vector2i playerStartCell, Vector2i goalCell, SignalType goalSignalType, bool goalActive, LevelData.SpawnData[] spawnDatas = null)
    {
        ClearProjectiles();
        ClearSpecialObjects();
        ClearCollectables();
        ClearEnemies();

        mDungeon.LoadDungeon(data);
        mStartPos = mDungeon.GetTilePosition(playerStartCell.mX, playerStartCell.mY);
        RespawnPlayer1();
        mGoal.mSignalType = goalSignalType;
        mGoal.transform.position = mDungeon.GetTilePosition(goalCell.mX, goalCell.mY);
        mGoal.gameObject.SetActive(goalActive);
        mDungeon.SetCell(goalCell.mX, goalCell.mY, DungeonCell.TileType.Goal);
        
        if(spawnDatas != null)
        {
            for (int i = 0, n = spawnDatas.Length; i < n; ++i)
            {
                LevelData.SpawnData spawnData = spawnDatas[i];
                GameObject obj = SpawnPrefab(spawnData.mPrefab, mDungeon.GetTilePosition(spawnData.mCell.mX, spawnData.mCell.mY), Quaternion.Euler(0.0f, spawnData.mYRotation, 0.0f));
                switch(spawnData.mSpawnType)
                {
                    case LevelData.SpawnData.SpawnType.Special:
                        mSpecialObjects.Add(obj);
                        break;
                    case LevelData.SpawnData.SpawnType.Collectable:
                        mCollectables.Add(obj);
                        break;
                    case LevelData.SpawnData.SpawnType.Enemy:
                        EnemyController enemy = obj.GetComponent<EnemyController>();
                        if(enemy != null)
                        {
                            enemy.Init();
                            mEnemies.Add(enemy);
                        }
                        else
                        {
                            mSpecialObjects.Add(obj);
                        }
                        break;
                }
            }
        }
    }

    public void GenerateLevel()
    {
        if(mbUseTestGenerationData)
        {
            mCurrentLevelData = mTestGenerationData;
        }

        if(GlobalData.NumHearts <= 0)
        {
            GlobalData.NumHearts = 1;
        }

        ClearProjectiles();
        ClearSpecialObjects();

        mDungeon.GenerateDungeon(mCurrentLevelData.mEnvironmentData);
        mStartPos = mDungeon.GetTilePosition(mDungeon.InitialRoomPosition.mX, mDungeon.InitialRoomPosition.mY);
        RespawnPlayer1();
        mGoal.mSignalType = SignalType.LevelComplete;
        mGoal.transform.position = FindGoalSpawnPosition();
        mDungeon.SetCell(mGoal.transform.position, DungeonCell.TileType.Goal);

        // Generate room contents
        GenerateCollectables();
        GenerateEnemies();

        Signal.Dispatch(SignalType.LevelStart);
    }

    protected void ClearProjectiles()
    {
        BaseProjectile[] projectiles = FindObjectsOfType<BaseProjectile>();
        for(int i = 0, n = projectiles.Length; i < n; ++i)
        {
            Destroy(projectiles[i].gameObject);
        }
    }

    protected void ClearSpecialObjects()
    {
        for (int i = 0, n = mSpecialObjects.Count; i < n; ++i)
        {
            Destroy(mSpecialObjects[i]);
        }
        mSpecialObjects.Clear();
    }

    protected void ClearEnemies()
    {
        for (int i = 0, n = mEnemies.Count; i < n; ++i)
        {
            Destroy(mEnemies[i].gameObject);
        }
        mEnemies.Clear();
    }

    protected void GenerateEnemies()
    {
        ClearEnemies();

        for (int i = 0; i < mCurrentLevelData.mNumEnemies; ++i)
        {
            int attempts = 0;
            do
            {
                Room room = GetRandomRoom();
                Vector2i randPos = room.GetRandomPos();
                DungeonCell cell = mDungeon.GetCell(randPos.mX, randPos.mY);
                if (cell.mTileType == DungeonCell.TileType.Ground)
                {
                    cell.mTileType = DungeonCell.TileType.Collectable;
                    GameObject prefab = mCurrentLevelData.mEnemyPrefabs[Random.Range(0, mCurrentLevelData.mEnemyPrefabs.Length)];
                    GameObject enemyObj = SpawnPrefab(prefab, mDungeon.GetTilePosition(randPos.mX, randPos.mY), Quaternion.identity);
                    EnemyController enemy = enemyObj.GetComponent<EnemyController>();
                    enemy.Init();
                    mEnemies.Add(enemy);
                    break;
                }
                attempts++;

            } while (attempts < kMaxPlacementAttempts);
        }
    }

    protected void ClearCollectables()
    {
        for (int i = 0, n = mCollectables.Count; i < n; ++i)
        {
            Destroy(mCollectables[i]);
        }
        mCollectables.Clear();
    }

    protected void GenerateCollectables()
    {
        ClearCollectables();

        for (int i = 0; i < mCurrentLevelData.mNumCollectables; ++i)
        {
            int attempts = 0;
            do
            {
                Room room = GetRandomRoom();
                Vector2i randPos = room.GetRandomPos();
                DungeonCell cell = mDungeon.GetCell(randPos.mX, randPos.mY);
                if (cell.mTileType == DungeonCell.TileType.Ground)
                {
                    cell.mTileType = DungeonCell.TileType.Collectable;
                    GameObject collectableObj = SpawnPrefab(mCollectablePrefab, mDungeon.GetTilePosition(randPos.mX, randPos.mY), Quaternion.identity);
                    mCollectables.Add(collectableObj);
                    break;
                }
                attempts++;

            } while (attempts < kMaxPlacementAttempts);
        }
    }

    public void RespawnPlayer1()
    {
        mPlayer1.mPlayer.TeleportTo(mStartPos);
        mPlayer1.mPlayer.Revive();
    }

    public void OnLevelComplete()
    {
        Time.timeScale = 0.0f;
        SoundManager.Instance.FadeOut(.5f);
        mFader.FadeOut(() =>
        {
            SoundManager.Instance.FadeIn();
            SoundManager.Instance.PlayBgm(SoundManager.Instance.bgm_shop);
            if(GlobalData.ActiveBoss != GlobalData.BossId.None)
            {
                GlobalData.SetBossDefeated(GlobalData.ActiveBoss);
            }
            LoadLevel(mShopLevelData);
            Time.timeScale = 1.0f;
            mFader.FadeIn();
        });
    }

    public void OnPlayerDeath()
    {
        SoundManager.Instance.FadeOut(.5f);
        mFader.FadeOut(() =>
        {
            SoundManager.Instance.FadeIn();
            SoundManager.Instance.RestartBgm();
            
            GlobalData.NumHearts = 1;
            GlobalData.NumAmmo = GlobalData.MaxAmmo;
            LoadLevel(mReviveZoneLevelData);
            Time.timeScale = 1.0f;
            mFader.FadeIn();
        });
    }

    public void OnBossDefeated()
    {
        SoundManager.Instance.FadeOut(.5f);
        SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_win_ditty);
        mGoal.gameObject.SetActive(true);
    }

    public void OnFinalBossLevelCompleted()
    {
        mFader.FadeOut(() =>
        {
            if (GlobalData.sSelectedCharacter == SelectedCharacter.Rose)
            {
                LoadLevel(mWinRoseLevelData);
            }
            else
            {
                LoadLevel(mWinVuLevelData);
            }
            Time.timeScale = 1.0f;
            mFader.FadeIn();
        });
    }

    public void OnStartNextLevel()
    {
        Time.timeScale = 0.0f;
        mFader.FadeOut(() =>
        {
            if(GlobalData.ActiveBoss == GlobalData.BossId.None)
            {
                SoundManager.Instance.PlayBgm(SoundManager.Instance.bgm_main);
                GlobalData.CurrentFloor++;
                GenerateLevel();
            }
            else
            {
                LoadBossLevel(GlobalData.ActiveBoss);
            }
            Time.timeScale = 1.0f;
            mFader.FadeIn();
        });
    }

    public void OnRestartLevel()
    {
        Time.timeScale = 0.0f;
        mFader.FadeOut(() =>
        {
            if (GlobalData.ActiveBoss == GlobalData.BossId.None)
            {
                GenerateLevel();
            }
            else
            {
                LoadBossLevel(GlobalData.ActiveBoss);
            }
            Time.timeScale = 1.0f;
            mFader.FadeIn();
        });
    }

    public void QuitToTitle()
    {
        mFader.FadeOut(() => SceneManager.LoadScene("Title"));
    }

    public void LoadBossLevel(GlobalData.BossId bossId)
    {
        SoundManager.Instance.PlayBgm(SoundManager.Instance.bgm_boss_flower);
        switch (bossId)
        {
            case GlobalData.BossId.Flower:
                LoadLevel(mFlowerBossLevelData);
                break;
            case GlobalData.BossId.Chocolate:
                LoadLevel(mChocolateBossLevelData);
                break;
            case GlobalData.BossId.Imposter:
                if(GlobalData.sSelectedCharacter == SelectedCharacter.Rose)
                {
                    LoadLevel(mImposterRoseBossLevelData);
                }
                else
                {
                    LoadLevel(mImposterVuBossLevelData);
                }
                break;
        }
    }

    protected void Update()
    {
        mPlayer1.UpdatePlayerControls();

        for(int i = 0, n = mEnemies.Count; i < n; ++i)
        {
            mEnemies[i].UpdateBehavior(mPlayer1.mPlayer);
        }
    }

    protected void LateUpdate()
    {
        mFollowCamera.UpdatePosition();
        if (Input.GetKeyDown(KeyCode.F1))
        {
            OnRestartLevel();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            OnLevelComplete();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            OnStartNextLevel();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            GlobalData.NumHearts += 25;
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            GlobalData.NumAmmo += 25;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitToTitle();
        }
    }
}
