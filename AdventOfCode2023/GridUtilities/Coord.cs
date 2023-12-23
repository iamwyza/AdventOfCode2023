namespace AdventOfCode2023.GridUtilities;
// ReSharper disable once InconsistentNaming
internal readonly struct Coord : IEqualityComparer<Coord>, IEquatable<Coord>
{

    private readonly uint _value;

    public int X => (int)(_value & 0xffff);

    public int Y => (int)(_value >> 16);

    public uint RawValue => _value;

    public Coord()
    {
        _value = 0;
    }

    public Coord(uint value)
    {
        _value = value;
    }

    public Coord(int x, int y)
    {
        //ArgumentOutOfRangeException.ThrowIfNegative(x);
        //ArgumentOutOfRangeException.ThrowIfNegative(y);
        //ArgumentOutOfRangeException.ThrowIfGreaterThan(x, 1 << 16);
        //ArgumentOutOfRangeException.ThrowIfGreaterThan(y, 1 << 16);
        _value = (uint)y << 16 | (uint)x;
    }

    public static bool operator ==(Coord left, Coord right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(Coord left, Coord right)
    {
        return left.X != right.X || left.Y != right.Y;
    }

    public readonly override string ToString()
    {
        return string.Format($"{X},{Y}");
    }

    public readonly bool Equals(Coord x, Coord y)
    {
        return x.X == y.X && x.Y == y.Y;
    }

    public readonly int GetHashCode(Coord obj)
    {
        return HashCode.Combine(obj.X, obj.Y);
    }


    public bool Equals(Coord other)
    {
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Coord other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}

internal static class CoordExtensions
{
    public static Coord Move(this Coord coord, Direction direction)
    {
        return direction switch
        {
            Direction.North => new Coord(coord.X, coord.Y - 1),
            Direction.East => new Coord(coord.X + 1, coord.Y),
            Direction.South => new Coord(coord.X, coord.Y + 1),
            Direction.West => new Coord(coord.X - 1, coord.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}