public interface IInfiniteGridObserver<T> : IGridObserver<T> where T : BaseGridCell, new()
{
    void OnGridCycle(BaseInfiniteGrid<T> infiniteGrid);
}

public class BaseInfiniteGrid<T> : BaseGrid<T> where T: BaseGridCell, new()
{
    protected int mCycles = 0;
    public int Cycles { get { return mCycles; } }

    public override void Init(int width, int height)
    {
        base.Init(width, height);
    }

    public void Cycle()
    {
        mCycles++;

        // copy down
        for (int y = 1; y < mHeight; ++y)
        {
            mGridCells[y] = mGridCells[y - 1];
        }

        // set last row
        T[] row = new T[mWidth];
        for (int x = 0; x < mWidth; ++x)
        {
            row[x] = new T();
        }
        mGridCells[mHeight - 1] = row;

        foreach (var gridObserver in mGridObservers)
        {
            IInfiniteGridObserver<T> infiniteGridObserver = gridObserver as IInfiniteGridObserver<T>;
            if (infiniteGridObserver != null)
            {
                infiniteGridObserver.OnGridCycle(this);
            }
        }
    }
}