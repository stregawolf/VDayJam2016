using System.Collections.Generic;

public class BaseGrid<T> where T: BaseGridCell, new()
{
	protected int mWidth;
    public int Width { get { return mWidth; } }
	protected int mHeight;
    public int Height { get { return mHeight; } }

	protected T[][] mGridCells;

    protected HashSet<IGridObserver<T>> mGridObservers;

	public virtual void Init(int width, int height)
	{
		mWidth = width;
		mHeight = height;

		mGridCells = new T[mHeight][];
		for(int y = 0; y < mHeight; ++y)
		{
			T[] row = new T[mWidth];
			for(int x = 0; x < mWidth; ++x)
			{
				row[x] = new T();
			}
			mGridCells[y] = row;
		}

        mGridObservers = new HashSet<IGridObserver<T>>();
	}

	public T GetGridCell(int x, int y)
	{
		if(mGridCells == null || x < 0 || x >= mWidth || y < 0 || y >= mHeight)
		{
			return null;
		}
		return mGridCells[y][x];
	}

	public void SetGridCell(int x, int y, T gridCell)
	{
		if(mGridCells == null || x < 0 || x >= mWidth || y < 0 || y >= mHeight)
		{
			return;
		}

		mGridCells[y][x] = gridCell;
		
		foreach(var gridObserver in mGridObservers)
		{
			gridObserver.OnGridCellChanged(x,y,gridCell);
		}
	}

	public bool RegisterGridObserver(IGridObserver<T> observer)
	{
		return mGridObservers.Add(observer);
	}

	public bool UnregisterGridObserver(IGridObserver<T> observer)
	{
		return mGridObservers.Remove(observer);
	}
}