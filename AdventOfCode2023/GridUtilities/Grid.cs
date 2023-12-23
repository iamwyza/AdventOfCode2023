using System.Collections;
using System.Text;

namespace AdventOfCode2023.GridUtilities;

internal class Grid<T> : IEnumerable<(Coord Coordinate, T Value)> //where T : IComparable
{

    public T this[Coord index]
    {
        get => Map[GetXIndex(index.X), GetYIndex(index.Y)];
        set => Map[GetXIndex(index.X), GetYIndex(index.Y)] = value;
    }

    public T this[int x, int y]
    {
        get => Map[GetXIndex(x), GetYIndex(y)];
        set => Map[GetXIndex(x), GetYIndex(y)] = value;
    }

    //public T this[Coord index]
    //{
    //    get => Map[index.X + XOffset, index.Y + YOffset];
    //    set => Map[index.X + XOffset, index.Y + YOffset] = value;
    //}

    //public T this[int x, int y]
    //{
    //    get => Map[x + XOffset, y + YOffset];
    //    set => Map[x + XOffset, y + YOffset] = value;
    //}

    private int _xLength;
    private int _yLength;

    private int GetXIndex(int x)
    {
        if (!CanWrapX) return x + XOffset;

        if (x + XOffset < 0)
        {
            return _xLength + x + XOffset;
        }

        if (x + XOffset > _xLength)
        {
            return _xLength + x + XOffset;
        }

        return x + XOffset;
    }

    private int GetYIndex(int y)
    {
        //if (!CanWrapY) return y + YOffset;

        if (y + YOffset < 0)
        {
            //if (y == -16188) Debugger.Break();
            if (y % _yLength != 0)
                return _yLength + y % _yLength + YOffset;

            return YOffset; // Y = 0 + offset
        }

        if (y + YOffset >= _yLength)
        {
            if (y % _yLength != 0)
                return y % _yLength + YOffset;

            return YOffset;
        }

        return y + YOffset;
    }

    public Grid()
    {
        CheckBounds(new Coord(0, 0));
        InitMap();
    }

    public Grid(int minX, int minY, int maxX, int maxY)
    {
        Bounds = (minX, minY, maxX, maxY);
        InitMap();
    }

    public Grid(string[] lines, Func<char, T> converter)
    {
        CheckBounds(new Coord(lines[0].Length - 1, lines.Length - 1));
        InitMap();

        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (int x = 0; x < lines[y].Length; x++)
            {
                this[x, y] = converter(line[x]);
            }
        }
    }

    public bool CanWrapY = false;
    public bool CanWrapX = false;

    public T[,] Map { get; internal set; }

    public (int minX, int minY, int maxX, int maxY) Bounds;

    public int XOffset;
    public int YOffset;

    public int Rows => Bounds.maxY;
    public int Columns => Bounds.maxX;

    //public Coord MakeCord(int x, int y)
    //{
    //  return new Coord { X = x, Y = y, XOffset = XOffset, YOffset = YOffset};
    //}

    public Func<T, Coord, (char letter, Color color, string? extraText)> DefaultPrintConfig { get; set; } = (input, _) => (input is char c ? c : input.ToString()[0], Color.Default, string.Empty);

    [MemberNotNull(nameof(Map))]
    public void InitMap()
    {
        Map = new T[Bounds.maxX + 1, Bounds.maxY + 1];
        _xLength = Map.GetLength(0);
        _yLength = Map.GetLength(1);
    }

    public void ResetMap(T value)
    {
        for (int yRow = Bounds.minY; yRow <= Bounds.maxY; yRow++)
        {
            for (int xCol = Bounds.minX; xCol <= Bounds.maxX; xCol++)
            {
                Map[xCol, yRow] = value;
            }
        }
    }

    public void CalculateOffsets()
    {
        if (Bounds.minX < 0)
        {
            XOffset = Bounds.minX * -1;
            Bounds.minX = 0;
            Bounds.maxX += XOffset;
        }

        if (Bounds.minY < 0)
        {
            YOffset = Bounds.minY * -1;
            Bounds.minY = 0;
            Bounds.maxY += YOffset;
        }
    }

    public bool InBounds(Coord coord)
    {
        return coord.Y + YOffset >= 0
               && coord.X + XOffset >= 0
               && coord.Y + YOffset <= Bounds.maxY
               && coord.X + XOffset <= Bounds.maxX;
    }

    public void CheckBounds(Coord coord)
    {
        if (coord.X > Bounds.maxX)
            Bounds.maxX = coord.X;
        if (coord.Y > Bounds.maxY)
            Bounds.maxY = coord.Y;
        if (coord.X < Bounds.minX)
            Bounds.minX = coord.X;
        if (coord.Y < Bounds.minY)
            Bounds.minY = coord.Y;
    }

    public Grid<TNewGridType> CopyGrid<TNewGridType>(Func<Coord, T, TNewGridType>? converter = null)
        //where TNewGridType : IComparable
    {
        var newGrid = new Grid<TNewGridType>(Bounds.minX, Bounds.minY, Bounds.maxX, Bounds.maxY);

        if (converter != null)
        {
            foreach (var (coord, item) in this)
            {
                newGrid[coord] = converter(coord, item);
            }
        }

        return newGrid;
    }


    public bool AdjacentTest(Func<T, bool> test, Coord coord, bool cardinalOnly, [NotNullWhen(true)] out Coord[]? adjacentCoords)
    {
        Coord upperLeft = new Coord(coord.X - 1, coord.Y - 1);
        Coord upperCenter = new Coord(coord.X, coord.Y - 1);
        Coord upperRight = new Coord(coord.X + 1, coord.Y - 1);
        Coord left = new Coord(coord.X - 1, coord.Y);
        Coord right = new Coord(coord.X + 1, coord.Y);
        Coord lowerLeft = new Coord(coord.X - 1, coord.Y + 1);
        Coord lowerCenter = new Coord(coord.X, coord.Y + 1);
        Coord lowerRight = new Coord(coord.X + 1, coord.Y + 1);

        bool result = false;

        var temp = new List<Coord>();

        if (!cardinalOnly)
        {
            result |= CheckAndAdd(upperLeft);
            result |= CheckAndAdd(upperRight);
            result |= CheckAndAdd(lowerLeft);
            result |= CheckAndAdd(lowerRight);
        }

        result |= CheckAndAdd(upperCenter);
        result |= CheckAndAdd(left);
        result |= CheckAndAdd(right);
        result |= CheckAndAdd(lowerCenter);

        adjacentCoords = result ? temp.ToArray() : null;

        return result;

        bool CheckAndAdd(Coord coordToCheck)
        {
            if (InBounds(coordToCheck) && test(this[coordToCheck]))
            {
                temp.Add(coordToCheck);
                return true;
            }

            return false;
        }

    }


    public void PrintMap(Func<T, Coord, (char letter, Color color, string? extraText)>? config = null, bool printRowLabel = false, bool printEmptyRow = true, int yMin = 0, int yMax = 0)
    {
        config ??= DefaultPrintConfig;

        for (int yRow = Bounds.minY; yRow <= Bounds.maxY; yRow++)
        {


            bool hasData = false;

            if (!printEmptyRow)
            {
                for (int xCol = Bounds.minX; xCol <= Bounds.maxX; xCol++)
                {
                    if (!Map[xCol, yRow].Equals(default(T)))
                    {
                        hasData = true;
                        break;
                    }
                }
            }

            if (printEmptyRow || hasData && (yMin == 0 && yMax == 0 || yRow >= yMin && yRow <= yMax))
            {
                if (printRowLabel)
                    AnsiConsole.Markup($"{yRow:0000;-000} ");
                var stringBuilder = new StringBuilder();
                for (int xCol = Bounds.minX; xCol <= Bounds.maxX; xCol++)
                {
                    var setting = config(Map[xCol, yRow], new Coord(xCol, yRow));

                    stringBuilder.Append($"[{setting.color.ToMarkup()}]{setting.letter}[/]");
                }
                AnsiConsole.MarkupInterpolated($"{stringBuilder} [yellow]{config(Map[0, yRow], new Coord(0, yRow)).extraText}[/]");
                Console.WriteLine();
            }
        }

        Console.WriteLine();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<(Coord, T)> GetEnumerator()
    {
        for (int y = 0; y <= Bounds.maxY; y++)
        {
            for (int x = 0; x <= Bounds.maxX; x++)
            {
                yield return (new Coord(x, y), this[x, y]);
            }
        }
    }


}