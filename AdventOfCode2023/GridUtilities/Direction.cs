using static System.Collections.Specialized.BitVector32;

namespace AdventOfCode2023.GridUtilities;

[Flags]
public enum Direction : int
{
    None = 0,
    Special = 1,
    North = 2,
    East = 4,
    South = 8,
    West = 16,
    NorthEast = 32,
    NorthWest = 64,
    NorthSouth = 128,
    EastWest = 256,
    SouthEast = 512,
    SouthWest = 1024
}

internal static class DirectionExtensions
{

    //Currently only supports cardinal directions.
    public static Direction GetDirection(Coord from, Coord to)
    {
       
            if (from.X > to.X)
            {
                return Direction.East;
            }

            if (from.X < to.X)
            {
                return Direction.West;
            }

            if (from.Y > to.Y)
            {
                return Direction.South;
            }

            if (from.Y < to.Y)
            {
                return Direction.North;
            }

            if (from == to)
            {
                return Direction.None;
            }

            throw new Exception($"Attempted to move from {from} to {to}");
    }


    public static Direction GetDirection(int fromX, int fromY, int toX, int toY)
    {

        if (fromX > toX)
        {
            return Direction.East;
        }

        if (fromX < toX)
        {
            return Direction.West;
        }

        if (fromY > toY)
        {
            return Direction.South;
        }

        if (fromY < toY)
        {
            return Direction.North;
        }

        if (fromX == toX && fromY == toY)
        {
            return Direction.None;
        }

        throw new Exception($"Attempted to move from {fromX},{fromY} to {toX},{toY}");
    }

    public static char DirectionString(this Direction direction)
    {
        return direction switch
        {
            Direction.None => '.',
            Direction.Special => 'S',
            Direction.North => '^',
            Direction.East => '>',
            Direction.South => 'v',
            Direction.West => '<',
            Direction.NorthEast => 'F',
            Direction.NorthWest => '7',
            Direction.NorthSouth => '|',
            Direction.EastWest => '-',
            Direction.SouthEast => 'J',
            Direction.SouthWest => 'L',
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}
