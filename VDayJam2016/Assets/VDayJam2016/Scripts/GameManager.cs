using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour {
    public const int kMaxPlacementAttempts = 100;

    protected static GameManager sInstance;
    public static GameManager Instance { get { return sInstance; } }

    public GameObject mPlayerPrefab;
    public GameObject mGoalPrefab;

    public GameObject mCollectablePrefab;

    public Dungeon mDungeon;
    public FollowCamera mFollowCamera;

    [System.Serializable]
    public class DungeonGenerationData
    {
        public int mNumCollectables = 0;
        public int mNumEnemies = 0;
        public GameObject[] mEnemyPrefabs;
    }
    public DungeonGenerationData mTestGenerationData;

    protected Goal mGoal;
    protected Vector3 mStartPos;
    protected PlayerController mPlayer1;

    protected List<GameObject> mCollectables = new List<GameObject>();

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
    }

	protected void Start () 
    {
        GameObject goalObj = Instantiate(mGoalPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        mGoal = goalObj.GetComponent<Goal>();
        GameObject playerObj = Instantiate(mPlayerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        mPlayer1 = playerObj.GetComponent<PlayerController>();
        mFollowCamera.Init(mPlayer1.transform);

        GenerateLevel();

        Signal.Register(SignalType.LevelComplete, OnLevelComplete);
	}

    protected void OnDestroy()
    {
        if (sInstance == this)
        {
            Signal.Unregister(SignalType.LevelComplete, OnLevelComplete);
            sInstance = null;
        }
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

    public void GenerateLevel()
    {
        mDungeon.GenerateDungeon();
        mStartPos = mDungeon.GetTilePosition(mDungeon.kInitialRoomPosition.mX, mDungeon.kInitialRoomPosition.mY);
        RespawnPlayer1();
        mGoal.transform.position = FindGoalSpawnPosition();
        mDungeon.SetCell(mGoal.transform.position, DungeonCell.TileType.Goal);

        // Generate room contents
        for (int i = 0, n = mCollectables.Count; i < n; ++i)
        {
            Destroy(mCollectables[i].gameObject);
        }
        mCollectables.Clear();

        for (int i = 0; i < mTestGenerationData.mNumCollectables; ++i)
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
                    GameObject collectableObj = Instantiate(mCollectablePrefab, mDungeon.GetTilePosition(randPos.mX, randPos.mY), Quaternion.identity) as GameObject;
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
    }

    public void OnLevelComplete()
    {
        GenerateLevel();
    }

    protected void LateUpdate()
    {
        mFollowCamera.UpdatePosition();
        if (Input.GetKeyDown(KeyCode.F1))
        {
            OnLevelComplete();
        }
    }
}
