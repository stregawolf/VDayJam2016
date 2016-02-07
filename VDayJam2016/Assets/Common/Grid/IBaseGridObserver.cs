public interface IGridObserver<T> where T : BaseGridCell, new()
{
	void OnGridCellChanged(int x, int y, T cell);
}
