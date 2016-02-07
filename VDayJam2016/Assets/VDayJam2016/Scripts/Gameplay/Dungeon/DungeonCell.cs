public class DungeonCell : BaseGridCell {
    public enum TileType:int
    {
        Undefined = -1,
        Wall = 0,
        Ground = 1,
        Goal = 2,
        Monster = 3,
        Collectable = 4,
    }

    public TileType mTileType = TileType.Wall;
}
