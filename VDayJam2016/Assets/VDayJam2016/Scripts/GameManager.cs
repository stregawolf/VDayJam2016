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
    
    public bool mbUseTestGenerationData = true;
    public DungeonGenerationData mTestGenerationData;
    protected DungeonGenerationData mCurrentLevelData;

    public LevelData mTestLevelData;

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

        GameObject goalObj = SpawnPrefab(mGoalPrefab);
        mGoal = goalObj.GetComponent<Goal>();
        GameObject playerObj = SpawnPrefab(GlobalData.sSelectedCharacter == SelectedCharacter.Rose ? mPlayerRosePrefab : mPlayerVuPrefab);
        mPlayer1 = playerObj.GetComponent<PlayerController>();
        mFollowCamera.Init(mPlayer1.transform);
    }

	protected void Start () 
    {
        GenerateLevel();

        Signal.Register(SignalType.LevelComplete, OnLevelComplete);
        Signal.Register(SignalType.StartNextLevel, OnStartNextLevel);
	}

    protected void OnDestroy()
    {
        if (sInstance == this)
        {
            Signal.Unregister(SignalType.LevelComplete, OnLevelComplete);
            Signal.Unregister(SignalType.StartNextLevel, OnStartNextLevel);
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
        LoadLevel(data.mData, data.mPlayerCell, data.mGoalCell, data.mGoalSignalType, data.mSpawnDatas);
    }

    public void LoadLevel(string data, Vector2i playerStartCell, Vector2i goalCell, SignalType goalSignalType, LevelData.SpawnData[] spawnDatas = null)
    {
        ClearSpecialObjects();
        ClearCollectables();
        ClearEnemies();

        mDungeon.LoadDungeon(data);
        mStartPos = mDungeon.GetTilePosition(playerStartCell.mX, playerStartCell.mY);
        RespawnPlayer1();
        mGoal.mSignalType = goalSignalType;
        mGoal.transform.position = mDungeon.GetTilePosition(goalCell.mX, goalCell.mY);
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
        LoadLevel(mTestLevelData);
    }

    public void OnStartNextLevel()
    {
        GlobalData.sCurrentFloor++;
        GenerateLevel();
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
            GenerateLevel();
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
            SceneManager.LoadScene("Title");
        }
    }
}
