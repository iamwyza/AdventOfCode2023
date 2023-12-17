namespace AdventOfCode2023.GridUtilities;
// ReSharper disable once InconsistentNaming
internal struct Coord : IEqualityComparer<Coord>
{
    public int X;

    public int Y;

    public Coord()
    {
        X = 0; Y = 0;
    }

    public Coord(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(Coord left, Coord right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(Coord left, Coord right)
    {
        return left.X != right.X || left.Y != right.Y;
    }

    public override string ToString()
    {
        return string.Format($"{X},{Y}");
    }

    public bool Equals(Coord x, Coord y)
    {
        return x.X == y.X && x.Y == y.Y;
    }

    public int GetHashCode(Coord obj)
    {
        return HashCode.Combine(obj.X, obj.Y);
    }

   
}

internal static class CoordExtensions
{
    public static long ToLookup(this Coord coord)
    {
        return ((1 + coord.X) << 8) + ((1 + coord.Y) << 16);
    }

    public static (int x, int y) FromLookup(this long lookupValue)
    {
        const int xMask = 0b_11111111;
        const int yMask = 0b_11111111_00000000;

        return ((int)(1-(lookupValue & xMask)),(int)(1-((lookupValue & yMask) >> 8)));
    }
}