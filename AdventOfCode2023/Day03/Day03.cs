using System.Text;
using AdventOfCode2023.GridUtilities;
using static System.Char;

namespace AdventOfCode2023.Day03;
internal class Day03 : DayBase
{
    public Day03()
    {
        Ready = true;
    }

    private Grid<char> _map;

    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
    {
        _map = new Grid<char>();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            if (_map.Bounds.maxX == 0)
            {
                _map.CheckBounds(new Coord(line.Length-1, lines.Length-1));
                _map.InitMap();
            }

            for (var x = 0; x < line.Length; x++)
            {
                _map[x,y] = line[x];
            }
        }

    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        List<int> numbersThatWereTouching = new List<int>();
        string[] numbersOnLine = new string[_map.Bounds.maxY+1];

        var outputMap = new Grid<bool>();
        outputMap.CheckBounds(new Coord(_map.Bounds.maxX, _map.Bounds.maxY));
        outputMap.InitMap();
        
        for (int y = 0; y <= _map.Bounds.maxY; y++)
        {
            StringBuilder currentNumber = new();

            bool isTouchingSymbol = false;

            List<Coord> coordsForCurrentNumber = new List<Coord>();
            List<int> numbersOnThisLine = new();

            for (int x = 0; x <= _map.Bounds.maxX; x++)
            {
                if (IsNumber(_map[x, y]))
                {

                    currentNumber.Append(_map[x, y]);
                    if (!isTouchingSymbol) // If we already know it's touching a symbol, we don't need to check anymore for this number
                    {
                        if (_map.AdjacentTest((c) => !(IsNumber(c) || c == '.'), new Coord(x, y), false, out Coord[]? _))
                        {
                            isTouchingSymbol = true;
                        }
                    }

                    coordsForCurrentNumber.Add(new Coord(x, y));

                }
                else if (!IsNumber(_map[x,y]))
                {
                    if (isTouchingSymbol)
                    {
                        int number = Convert.ToInt32(currentNumber.ToString());
                        numbersThatWereTouching.Add(number);
                        foreach (var coord in coordsForCurrentNumber)
                        {
                            outputMap[coord] = true;
                        }

                        numbersOnThisLine.Add(number);
                    }

                    isTouchingSymbol = false;
                    currentNumber.Clear();
                    coordsForCurrentNumber.Clear();
                }
            }

            if (isTouchingSymbol)
            {
                int number = Convert.ToInt32(currentNumber.ToString());
                numbersThatWereTouching.Add(number);
                foreach (var coord in coordsForCurrentNumber)
                {
                    outputMap[coord] = true;
                }

                numbersOnThisLine.Add(number);
            }

            currentNumber.Clear();
            coordsForCurrentNumber.Clear();

            numbersOnLine[y] = string.Join(',', numbersOnThisLine) + " = " + numbersOnThisLine.Sum();
            numbersOnThisLine.Clear();

            Console.WriteLine();

        }

        _map.PrintMap((char x, Coord c) =>
        {
            if (!(IsNumber(x) || x == '.'))
                return (x, Color.Red, numbersOnLine[c.Y]);

            if (outputMap[c])
                return (x, Color.Green, numbersOnLine[c.Y]);

            if (IsNumber(x))
                return (x, Color.Blue, numbersOnLine[c.Y]);

            return (x, Color.Default, numbersOnLine[c.Y]);
        }, printRowLabel: true, printEmptyRow:false);


        AnsiConsole.MarkupLineInterpolated($"The numbers that were touching symbols were: [yellow]{string.Join(',',numbersThatWereTouching)}[/].  Their sum is: [green]{numbersThatWereTouching.Sum()}[/]");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        Dictionary<Coord, (int[] numbers, List<Coord>[] associatedNumberCoords)> numbersThatWereTouching = new Dictionary<Coord, (int[] numbers, List<Coord>[] associatedNumberCoords)>();
        string[] numbersOnLine = new string[_map.Bounds.maxY + 1];

        var outputMap = new Grid<bool>();
        outputMap.CheckBounds(new Coord(_map.Bounds.maxX, _map.Bounds.maxY));
        outputMap.InitMap();

        for (int y = 0; y <= _map.Bounds.maxY; y++)
        {
            StringBuilder currentNumber = new();

            bool isTouchingSymbol = false;

            List<Coord> coordsForCurrentNumber = new List<Coord>();
            Coord? adjacentGearCoordinate = null;

            List<int> numbersOnThisLine = new();

            for (int x = 0; x <= _map.Bounds.maxX; x++)
            {
                if (IsNumber(_map[x, y]))
                {

                    currentNumber.Append(_map[x, y]);
                    if (!isTouchingSymbol) // If we already know it's touching a symbol, we don't need to check anymore for this number
                    {
                        if (_map.AdjacentTest((c) => c == '*', new Coord(x, y), false, out Coord[]? adjacentCoords))
                        {
                            isTouchingSymbol = true;
                            adjacentGearCoordinate = adjacentCoords[0];
                        }
                    }

                    coordsForCurrentNumber.Add(new Coord(x, y));

                }
                else if (!IsNumber(_map[x, y]))
                {
                    if (isTouchingSymbol && adjacentGearCoordinate != null)
                    {
                        int number = Convert.ToInt32(currentNumber.ToString());

                        if (!numbersThatWereTouching.ContainsKey(adjacentGearCoordinate.Value))
                        {
                            numbersThatWereTouching.Add(adjacentGearCoordinate.Value, (new[] { -1, -1 }, new List<Coord>[2]));
                        }

                        if (numbersThatWereTouching[adjacentGearCoordinate.Value].numbers[0] == -1)
                        {
                            numbersThatWereTouching[adjacentGearCoordinate.Value].numbers[0] = number;
                            numbersThatWereTouching[adjacentGearCoordinate.Value].associatedNumberCoords[0] = coordsForCurrentNumber.ToList();
                        }
                        else
                        {
                            numbersThatWereTouching[adjacentGearCoordinate.Value].numbers[1] = number;
                            numbersThatWereTouching[adjacentGearCoordinate.Value].associatedNumberCoords[1] = coordsForCurrentNumber.ToList();
                        }

                        

                        numbersOnThisLine.Add(number);
                    }

                    isTouchingSymbol = false;
                    currentNumber.Clear();
                    coordsForCurrentNumber.Clear();
                }
            }

            if (isTouchingSymbol && adjacentGearCoordinate != null)
            {
                int number = Convert.ToInt32(currentNumber.ToString());
                if (!numbersThatWereTouching.ContainsKey(adjacentGearCoordinate.Value))
                {
                    numbersThatWereTouching.Add(adjacentGearCoordinate.Value, (new[] { -1, -1 }, new List<Coord>[2]));
                }

                if (numbersThatWereTouching[adjacentGearCoordinate.Value].numbers[0] == -1)
                {
                    numbersThatWereTouching[adjacentGearCoordinate.Value].numbers[0] = number;
                    numbersThatWereTouching[adjacentGearCoordinate.Value].associatedNumberCoords[0] = coordsForCurrentNumber.ToList();
                }
                else
                {
                    numbersThatWereTouching[adjacentGearCoordinate.Value].numbers[1] = number;
                    numbersThatWereTouching[adjacentGearCoordinate.Value].associatedNumberCoords[1] = coordsForCurrentNumber.ToList();
                }

                numbersOnThisLine.Add(number);
            }

            currentNumber.Clear();
            coordsForCurrentNumber.Clear();

            numbersOnLine[y] = string.Join(',', numbersOnThisLine) + " = " + numbersOnThisLine.Sum();
            numbersOnThisLine.Clear();
        }

        foreach (var entry in numbersThatWereTouching)
        {
            if (entry.Value.numbers[1] == -1)
            {
                numbersThatWereTouching.Remove(entry.Key);
            }
        }

        foreach (var entry in numbersThatWereTouching)
        {
            foreach (var coords in entry.Value.associatedNumberCoords)
            {
                foreach (var coord in coords)
                {
                    outputMap[coord] = true;
                }
            }
        }


        _map.PrintMap((x, coord) =>
        {
            if (x == '*')
                return (x, Color.Red, null);

            if (outputMap[coord])
                return (x, Color.Green, null);

            if (IsNumber(x))
                return (x, Color.Blue, null);

            return (x, Color.Default, null);
        }, printRowLabel: true, printEmptyRow: false);


        AnsiConsole.MarkupLineInterpolated($"The numbers that were touching gears were: [yellow]{string.Join(',', numbersThatWereTouching.Select(x => x.Value.numbers[0] + " " + x.Value.numbers[1]))}[/].  Their sum is: [green]{numbersThatWereTouching.Select(x => x.Value.numbers[0] * x.Value.numbers[1]).Sum()}[/]");
    }
}