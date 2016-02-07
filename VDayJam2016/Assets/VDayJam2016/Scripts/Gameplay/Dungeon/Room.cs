using UnityEngine;
using System.Collections;

public struct Room
{
    public Vector2i mMin;
    public Vector2i mMax;

    public int Width { get { return mMax.mX - mMin.mX; } }
    public int Height { get { return mMax.mY - mMin.mY; } }
    public int Area { get { return Width * Height; } }

    public void InitCenterSize(int x, int y, int width, int height)
    {
        int halfWidth = width / 2;
        int halfHeight = height / 2;
        mMin = new Vector2i(x - halfWidth, y - halfHeight);
        mMax = new Vector2i(x + halfWidth, y + halfHeight);
    }

    public void InitMinSize(int x, int y, int width, int height)
    {
        mMin = new Vector2i(x, y);
        mMax = new Vector2i(x + width, y + height);
    }

    public void InitMinMax(Vector2i min, Vector2i max)
    {
        mMin = new Vector2i(Mathf.Min(min.mX, max.mX), Mathf.Min(min.mY, max.mY));
        mMax = new Vector2i(Mathf.Max(min.mX, max.mX), Mathf.Max(min.mY, max.mY)); ;
    }

    public void ShiftBy(Vector2i amount)
    {
        mMin = mMin + amount;
        mMax = mMax + amount;
    }

    public Vector2i GetRandomPos()
    {
        return new Vector2i(Random.Range(mMin.mX + 1, mMax.mX), Random.Range(mMin.mY + 1, mMax.mY));
    }
}
