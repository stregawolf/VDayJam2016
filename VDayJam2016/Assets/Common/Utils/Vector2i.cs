[System.Serializable]
public class Vector2i
{
    public static Vector2i kUp { get { return new Vector2i(0, 1); } }
    public static Vector2i kDown { get { return new Vector2i(0, -1); } }
    public static Vector2i kLeft { get { return new Vector2i(-1, 0); } }
    public static Vector2i kRight { get { return new Vector2i(1, 0); } }

    public static readonly Vector2i[] kDirections = new Vector2i[] { kUp, kDown, kLeft, kRight };
    
    public int mX;
    public int mY;

    public Vector2i(int x, int y)
    {
        mX = x;
        mY = y;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector2i)
        {
            Vector2i other = (Vector2i)obj;
            return other.mX == mX && other.mY == mY;
        }

        return false;
    }

    public override string ToString()
    {
        return string.Format("{0}, {1}", mX, mY);
    }

    public override int GetHashCode()
    {
        return (mX + mY) * (mX + mY + 1) / 2 + mY;
    }

    public static Vector2i operator +(Vector2i a, Vector2i b)
    {
        return new Vector2i(a.mX + b.mX, a.mY + b.mY);
    }

    public static Vector2i operator *(Vector2i a, int b)
    {
        return new Vector2i(a.mX * b, a.mY * b);
    }
}