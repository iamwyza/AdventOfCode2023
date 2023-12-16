namespace AdventOfCode2023;

[Flags]
internal enum Direction :int
{
    None = 0,
    Special = 1,
    North = 2,
    East = 4,
    South = 8,
    West = 16,
    NorthEast = 32,
    NorthWest = 64,
    NorthSouth =128,
    EastWest =256,
    SouthEast = 512,
    SouthWest = 1024
}
