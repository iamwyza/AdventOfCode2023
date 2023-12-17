using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2023.GridUtilities;

namespace AdventOfCode2023.Day11
;
internal class Day11 : DayBase
{

    public Day11()
    {
        Ready = true;
        _map = new();
    }

    private readonly List<int> _emptyCols = new();
    private readonly List<int> _emptyRows = new();
    private Grid<char> _map;
  
    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
    {
        _map = new Grid<char>();
        _map.DefaultPrintConfig = (input, _) => (input, Color.Default, string.Empty);

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _emptyCols.Clear();
        _emptyRows.Clear();

        var tempMap = new List<List<char>>();

        for (var y = 0; y < lines.Length; y++)
        {
            if (!lines[y].Contains('#'))
                _emptyRows.Add(y);
            tempMap.Add(lines[y].ToCharArray().ToList());
        }

        for (var x = 0; x < lines[0].Length; x++)
        {
            bool isEmpty = true;

            for (var y = 0; y < lines.Length; y++)
            {
                isEmpty &= lines[y][x] == '.';
            }

            if (isEmpty)
                _emptyCols.Add(x);
        }

        var width = tempMap[0].Count(x => x != ' ');
        _map.CheckBounds(new Coord(width, tempMap.Count));
        _map.InitMap();


        for (var y = 0; y < tempMap.Count; y++)
        {
            var line = tempMap[y];
            for (var x = 0; x < line.Count; x++)
            {
                if (line[x] != ' ')
                    _map[x,y] = line[x];
            }
        }

        

        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        CalculateDistances(1);  // should be 9214785 for the actual result
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        CalculateDistances(1_000_000);
    }

    void CalculateDistances(int expansionRate)
    {
        if (expansionRate > 1) expansionRate--;

        var galaxies = new List<Coord>();
        var galaxyPairs = new Dictionary<(Coord A, Coord B), bool>();

        for (int y = 0; y <= _map.Bounds.maxY; y++)
        {
            for (int x = 0; x <= _map.Bounds.maxX; x++)
            {
                if (_map[x, y] == '#')
                    galaxies.Add(new Coord(x, y));
            }
        }

        foreach (var a in galaxies)
        {
            foreach (var b in galaxies.Where(x => !x.Equals(a)))
            {
                if (!galaxyPairs.ContainsKey((a, b)) && !galaxyPairs.ContainsKey((b, a)))
                {
                    galaxyPairs.Add((a, b), true);
                }
            }
        }

        AnsiConsole.MarkupLineInterpolated($"There are [green]{galaxyPairs.Count}[/] pairs of galaxies");
        var distances = 0L;
        foreach (var pair in galaxyPairs)
        {
            var a = pair.Key.A;
            var b = pair.Key.B;

            var distance = Math.Abs(pair.Key.A.X - pair.Key.B.X) + Math.Abs(pair.Key.A.Y - pair.Key.B.Y);

            var minX = a.X < b.X ? a.X : b.X;
            var maxX = a.X > b.X ? a.X : b.X;

            var numberOfExpansionColumns = _emptyCols.Count(col => minX < col && maxX > col);


            var minY = a.Y < b.Y ? a.Y : b.Y;
            var maxY = a.Y > b.Y ? a.Y : b.Y;

            var numberOfExpansionRows = _emptyRows.Count(row => minY < row && maxY > row);

            distance += (numberOfExpansionColumns * expansionRate) + (numberOfExpansionRows * expansionRate);

            distances += distance;
            //AnsiConsole.MarkupLineInterpolated($"Distance between [yellow]{pair.Key.A}[/] and [yellow]{pair.Key.B}[/] is [green]{distance}[/]. Number of Expanded columns = {numberOfExpansionColumns}. Number of Expanded Rows = {numberOfExpansionRows}");
        }

        //distances -= expansionRate > 1 ? (galaxyPairs.Count * 2) + 10 : 0;
        AnsiConsole.MarkupLineInterpolated($"Total distance is [green]{distances}[/]");
    }

   
}
