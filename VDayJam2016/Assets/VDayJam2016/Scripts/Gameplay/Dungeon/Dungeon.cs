using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Dungeon : MonoBehaviour {
    public static readonly Quaternion[] kTileRotations = { Quaternion.identity, Quaternion.Euler(0, 90, 0), Quaternion.Euler(0, 180, 0), Quaternion.Euler(0, 270, 0) };

    public int kSeed = 0;

    public int kWidth = 10;
    public int kHeight = 10;

    public int kMinRoomWidth = 3;
    public int kMaxRoomWidth = 3;
    public int kMinRoomHeight = 10;
    public int kMaxRoomHeight = 10;

    public int kMaxCorridorLength = 4;

    public int kMaxGenerationAttempts = 100;

    public Transform kTileParent = null;

    [System.Serializable]
    public class TileSet
    {
        public DungeonCell.TileType mType;
        public bool mAllowRandomOrientation = true;
        public GameObject[] mPrefabs;
    }
    public TileSet[] kTilePrefabs;
    protected Dictionary<DungeonCell.TileType, TileSet> mTileMapping = new Dictionary<DungeonCell.TileType, TileSet>();

    public bool kRandomizeInitialRoomPosition = true;
    public Vector2i kInitialRoomPosition;
    public Vector2i kInitialRoomSize;

    protected BaseGrid<DungeonCell> mGrid = null;
    protected List<GameObject> mTileObjects = new List<GameObject>();
    protected List<Room> mRooms = new List<Room>();
    public List<Room> Rooms { get { return mRooms; } }

    public bool mGenerateOnAwake = false;

    public Material mDungeonMaterial;
    public int kRenderGroupSize = 10;
    protected Dictionary<Vector2i, GameObject> mRenderGroups = new Dictionary<Vector2i, GameObject>();

    protected void Awake()
    {
        if (kTileParent == null)
        {
            kTileParent = transform;
        }

        if (kMaxRoomWidth > kWidth - 2) kMaxRoomWidth = kWidth - 2;
        if (kMaxRoomHeight > kHeight - 2) kMaxRoomHeight = kHeight - 2;

        mGrid = new BaseGrid<DungeonCell>();

        if (mGenerateOnAwake)
        {
            GenerateDungeon();
        }

        for(int i = 0, n = kTilePrefabs.Length; i < n; ++i)
        {
            mTileMapping.Add(kTilePrefabs[i].mType, kTilePrefabs[i]);
        }
    }
    /* TESTING
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateDungeon();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            mGrid.Init(kWidth, kHeight);
            GenerateRooms(kSeed);
            UpdateTileGraphics();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            IncrementGeneration(openWalls);
            UpdateTileGraphics();
        }
    }
     */

    public void GenerateDungeon()
    {
        mGrid.Init(kWidth, kHeight);
        GenerateRooms(Random.seed);
        UpdateTileGraphics();
    }

    Dictionary<Vector2i, Vector2i> openWalls = new Dictionary<Vector2i, Vector2i>();
    public void GenerateRooms(int seed)
    {
        mRooms.Clear();

        kSeed = seed;
        Random.seed = seed;
        openWalls.Clear();

        if (kRandomizeInitialRoomPosition)
        {
            int halfInitialRoomWidth = kInitialRoomSize.mX/2;
            int halfInitialRoomHeight = kInitialRoomSize.mY/2;
            kInitialRoomPosition.mX = Random.Range(halfInitialRoomWidth+1, kWidth - halfInitialRoomWidth-1);
            kInitialRoomPosition.mY = Random.Range(halfInitialRoomHeight+1, kHeight - halfInitialRoomHeight-1);
        }
        Room initialRoom = new Room();
        initialRoom.InitCenterSize(kInitialRoomPosition.mX, kInitialRoomPosition.mY, kInitialRoomSize.mX, kInitialRoomSize.mY);

        PlaceRoom(initialRoom);
        GetWalls(initialRoom, openWalls);

        int attempts = 0;
        while (attempts < kMaxGenerationAttempts && openWalls.Count > 0)
        {
            IncrementGeneration(openWalls);
            ++attempts;
        }
    }

    protected void IncrementGeneration(Dictionary<Vector2i, Vector2i> wallDirMapping = null)
    {
        Vector2i[] keys = wallDirMapping.Keys.ToArray();

        // make a corridor
        Vector2i corridorStart = keys[Random.Range(0, keys.Length - 1)];
        Vector2i dir = wallDirMapping[corridorStart];
        Vector2i corridorEnd = corridorStart + dir * Random.Range(0, kMaxCorridorLength);
        Room corridor = new Room();
        corridor.InitMinMax(corridorStart, corridorEnd);

        //Debug.DrawLine(GetTilePosition(corridorStart.mX, corridorStart.mY) + Vector3.up, GetTilePosition(corridorEnd.mX, corridorEnd.mY) + Vector3.up, Color.red, 10.0f);

        if (CanPlaceRoom(corridor))
        {
            PlaceRoom(corridor, true);
            GetWalls(corridor, wallDirMapping);

            // make a room to the end of it
            int roomWidth = Random.Range(kMinRoomWidth, kMaxRoomWidth);
            int halfRoomWidth = roomWidth / 2;
            int roomHeight = Random.Range(kMinRoomHeight, kMaxRoomHeight);
            int halfRoomHeight = roomHeight / 2;
            Room room = new Room();
            room.InitCenterSize(corridorEnd.mX, corridorEnd.mY, roomWidth, roomHeight);
            if (dir.mY != 0) // vertical
            {
                room.ShiftBy(dir * (halfRoomHeight + 1));
                room.ShiftBy(new Vector2i(Random.Range(-halfRoomWidth, halfRoomWidth), 0));
            }
            else // horizontal
            {
                room.ShiftBy(dir * (halfRoomWidth + 1));
                room.ShiftBy(new Vector2i(0, Random.Range(-halfRoomHeight, halfRoomHeight)));
            }

            if (CanPlaceRoom(room))
            {
                PlaceRoom(room);
                GetWalls(room, wallDirMapping);
            }

            wallDirMapping.Remove(corridorStart);
            if (dir.mY != 0) // vertical
            {
                wallDirMapping.Remove(corridorStart + Vector2i.kLeft);
                wallDirMapping.Remove(corridorStart + Vector2i.kRight);
            }
            else
            {
                wallDirMapping.Remove(corridorStart + Vector2i.kUp);
                wallDirMapping.Remove(corridorStart + Vector2i.kDown);
            }
        }
    }

    public bool CanPlaceRoom(Room room)
    {
        for (int y = room.mMin.mY; y <= room.mMax.mY; ++y)
        {
            for (int x = room.mMin.mX; x <= room.mMax.mX; ++x)
            {
                if (x == 0 || x == kWidth - 1 || y == 0 || y == kHeight - 1)
                {
                    return false;
                }

                DungeonCell cell = GetCell(x, y);
                if (cell == null || cell.mTileType != DungeonCell.TileType.Wall)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void PlaceRoom(Room room, bool isCorridor = false)
    {
        for (int y = room.mMin.mY; y <= room.mMax.mY; ++y)
        {
            for (int x = room.mMin.mX; x <= room.mMax.mX; ++x)
            {
                SetCell(x, y, DungeonCell.TileType.Ground);
            }
        }

        if (!isCorridor)
        {
            mRooms.Add(room);
        }
    }

    public Vector2i WorldToCellPos(Vector3 worldPos)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        return new Vector2i(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.z));
    }

    public DungeonCell GetCell(Vector3 worldPos)
    {
        Vector2i pos = WorldToCellPos(worldPos);
        return GetCell(pos.mX, pos.mY);
    }

    public DungeonCell GetCell(int x, int y)
    {
        return mGrid.GetGridCell(x, y);
    }

    public void SetCell(Vector3 worldPos, DungeonCell.TileType tileType)
    {
        Vector2i pos = WorldToCellPos(worldPos);
        SetCell(pos.mX, pos.mY, tileType);
    }

    public void SetCell(int x, int y, DungeonCell.TileType tileType)
    {
        DungeonCell cell = GetCell(x, y);
        if (cell != null)
        {
            cell.mTileType = tileType;
            mGrid.SetGridCell(x, y, cell);
        }
    }

    public Dictionary<Vector2i, Vector2i> GetWalls(Room room, Dictionary<Vector2i, Vector2i> wallDirMapping = null)
    {
        if (wallDirMapping == null)
        {
            wallDirMapping = new Dictionary<Vector2i, Vector2i>();
        }
        
        int top = room.mMax.mY + 1;
        int bottom = room.mMin.mY-1;
        int left = room.mMin.mX-1;
        int right = room.mMax.mX + 1;
        
        for (int x = room.mMin.mX; x <= room.mMax.mX; ++x)
        {
            Vector2i topPos = new Vector2i(x, top);
            Vector2i bottomPos = new Vector2i(x, bottom);
            if (mGrid.GetGridCell(x, top) != null && !wallDirMapping.ContainsKey(topPos))
            {
                wallDirMapping.Add(topPos, Vector2i.kUp);
            }
            if (mGrid.GetGridCell(x, bottom) != null && !wallDirMapping.ContainsKey(bottomPos))
            {
                wallDirMapping.Add(bottomPos, Vector2i.kDown);
            }
        }
        for (int y = room.mMin.mY; y <= room.mMax.mY; ++y)
        {
            Vector2i leftPos = new Vector2i(left, y);
            Vector2i rightPos = new Vector2i(right, y);
            if (mGrid.GetGridCell(left, y) != null && !wallDirMapping.ContainsKey(leftPos))
            {
                wallDirMapping.Add(leftPos, Vector2i.kLeft);
            }
            if (mGrid.GetGridCell(right, y) != null && !wallDirMapping.ContainsKey(rightPos))
            {
                wallDirMapping.Add(rightPos, Vector2i.kRight);
            }
        }
        return wallDirMapping;
    }

    public Vector3 GetTilePosition(int x, int y)
    {
        return transform.position + new Vector3(x, 0, y);
    }

    [ContextMenu("ClearGraphics")]
    public void ClearTileGraphics()
    {
        foreach (var pair in mRenderGroups)
        {
            Destroy(pair.Value);
        }
        mRenderGroups.Clear();
        mTileObjects.Clear();
    }

    protected GameObject GetRenderGroup(int tileX, int tileY)
    {
        Vector2i renderGroupKey = new Vector2i(tileX / kRenderGroupSize, tileY / kRenderGroupSize);
        GameObject renderGroup = null;
        if(!mRenderGroups.TryGetValue(renderGroupKey, out renderGroup) || renderGroup == null)
        {
            renderGroup = new GameObject(string.Format("RenderGroup-{0}", renderGroupKey));
            renderGroup.transform.position = transform.position;
            renderGroup.transform.SetParent(kTileParent);
            renderGroup.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = renderGroup.AddComponent<MeshRenderer>();
            meshRenderer.material = mDungeonMaterial;
            mRenderGroups.Add(renderGroupKey, renderGroup);
        }
        return renderGroup;
    }

    protected void UpdateTileGraphics()
    {
        ClearTileGraphics();

        for (int y = 0; y < kHeight; ++y)
        {
            for (int x = 0; x < kWidth; ++x)
            {
                DungeonCell cell = mGrid.GetGridCell(x,y);
                TileSet tileSet;
                if(mTileMapping.TryGetValue(cell.mTileType, out tileSet))
                {
                    GameObject tilePrefab = GetRandomTilePrefab(tileSet.mPrefabs);
                    Quaternion rotation = Quaternion.identity;
                    if(tileSet.mAllowRandomOrientation)
                    {
                        rotation = kTileRotations[Random.Range(0, kTileRotations.Length)];
                    }
                    GameObject tileObj = Instantiate(tilePrefab, GetTilePosition(x, y), rotation) as GameObject;
                    tileObj.transform.SetParent(GetRenderGroup(x, y).transform);
                    mTileObjects.Add(tileObj);
                }
            }
        }

        foreach(var pair in mRenderGroups)
        {
            CombineMesh(pair.Value);
        }

        for (int i = mTileObjects.Count - 1; i >= 0; --i)
        {
            Destroy(mTileObjects[i]);
        }
        mTileObjects.Clear();
    }

    protected void CombineMesh(GameObject obj)
    {
        //Zero transformation is needed because of localToWorldMatrix transform
        Vector3 position = obj.transform.position;
        obj.transform.position = Vector3.zero;

        //whatever man
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        MeshFilter mf = obj.GetComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine, true, true);

        //Reset position
        obj.transform.position = position;
        obj.SetActive(true);
    }

    protected GameObject GetRandomTilePrefab(GameObject[] prefabs)
    {
        return prefabs[Random.Range(0, prefabs.Length)];
    }
}



