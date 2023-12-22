using System.Numerics;
using AdventOfCode2023.GridUtilities;

namespace AdventOfCode2023.Day18;
internal class Day18 : DayBase
{
    private Grid<(Direction direction, bool isConnected)> _map = null!;

    public Day18()
    {
        Ready = true;
    }

    private Coord _startCoord = new Coord();

    private async Task Init(int part, bool useTestData)
    {
        _map = new Grid<(Direction direction, bool isConnected)>();
        _map.CheckBounds(new Coord(600,600));
        _map.InitMap();
        _map.DefaultPrintConfig = (val, coord) => (val.isConnected ? '#' : ' ', Color.Default, "");

        _map.ResetMap((Direction.None, false));

        var lines = useTestData ? await GetTestLines(part) : await GetLines();
        var currentCoord = new Coord(300, 300);

        var tempLines = new List<(Direction direction, ulong amount)>();

        foreach (var line in lines)
        {
            var temp = line.Split(' ');

            Direction direction = Direction.None;
            ulong amount = 0;

            if (part == 1)
            {
                 direction = temp[0] switch
                {
                    "R" => Direction.East,
                    "L" => Direction.West,
                    "U" => Direction.North,
                    "D" => Direction.South
                };
                 amount = Convert.ToUInt64(temp[1]);
            }
            else
            {
                amount = Convert.ToUInt64(temp[2][2..7], 16);
                direction = temp[2][7] switch
                {
                    '0' => Direction.East,
                    '1' => Direction.South,
                    '2' => Direction.West,
                    '3' => Direction.North
                };
            }
            
            tempLines.Add((direction, amount));
            
        }

        BigInteger lcm = tempLines[0].amount;
        foreach (var (_, amount) in tempLines.Skip(1))
        {
            AnsiConsole.MarkupInterpolated($"GCM of {lcm} and {amount} is ");
            lcm = BigInteger.GreatestCommonDivisor(lcm, amount);
            AnsiConsole.MarkupLineInterpolated($"{lcm}");
        }


        foreach (var ( direction, tAmount) in tempLines)
        {
            var amount = lcm / tAmount;
            var tempCoord = new Coord(currentCoord.X, currentCoord.Y);

            for (ulong i = 1; i <= amount; i++)
            {
                tempCoord = new Coord(currentCoord.X, currentCoord.Y);

                switch (direction)
                {
                    case Direction.North:
                        tempCoord.Y -= (int)i;
                        break;
                    case Direction.East:
                        tempCoord.X += (int)i;
                        break;
                    case Direction.South:
                        tempCoord.Y += (int)i;
                        break;
                    case Direction.West:
                        tempCoord.X -= (int)i;
                        break;
                }

                _map[tempCoord] = (direction, true);
            }

            currentCoord = tempCoord;
        }

        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);


        var overlayMap = new Grid<bool>();
        overlayMap.CheckBounds(new Coord(_map.Bounds.maxX, _map.Bounds.maxY));
        overlayMap.InitMap();

        overlayMap.DefaultPrintConfig = (touchesEdge, location) =>
            (touchesEdge ? '.' : '0', touchesEdge ? Color.Blue : Color.Default, null);

        for (int y = 0; y <= overlayMap.Bounds.maxY; y++)
        {
            for (int x = 0; x <= overlayMap.Bounds.maxX; x++)
            {
                overlayMap[x, y] = _map[x, y].isConnected;
            }
        }

        overlayMap.PrintMap(null, true);
        var allVisitedNodes = new Dictionary<Coord, bool>();

        InitVisitedNodes(ref allVisitedNodes);

        Queue<Coord> toEvaluate = new Queue<Coord>();
        toEvaluate.Enqueue(new Coord(1, 1));

        while (toEvaluate.Count > 0)
        {
            EvaluateTouchingCoords();
        }


        var tempMap = _map.CopyGrid<(Direction direction, bool isConnected, int value)>();

        for (int y = 0; y <= overlayMap.Bounds.maxY; y++)
        {
            for (int x = 0; x <= overlayMap.Bounds.maxX; x++)
            {
                var coord = new Coord(x, y);
                if (allVisitedNodes.ContainsKey(coord) && allVisitedNodes[coord] == false)
                    if (!tempMap[coord].isConnected)
                    {
                        tempMap[coord] = (Direction.Special, false, 0);
                    }
                if (!allVisitedNodes.ContainsKey(coord) && !tempMap[coord].isConnected)
                {
                    tempMap[coord] = (Direction.Special, false, 0);
                }
            }
        }

        overlayMap.PrintMap();

        _map.PrintMap();

        var finalMap = new Grid<(Direction direction, bool isConnected, int colorValue)>();
        finalMap.CheckBounds(new Coord((_map.Bounds.maxX), (_map.Bounds.maxY)));
        finalMap.DefaultPrintConfig = (val, coord) =>
        (
            val.direction == Direction.Special ? '0' : 
            (val.direction != Direction.None ? '#' : ' '), 
            Color.Default, ""
        );
        finalMap.InitMap();
        finalMap.ResetMap((Direction.None, false, 0));
        finalMap.PrintMap();
        int numberOfSpots = 0;

        for (int y = 0; y <= _map.Bounds.maxY; y++)
        {
            for (int x = 0; x <= _map.Bounds.maxX; x++)
            {
                if (!tempMap[x, y].isConnected && tempMap[x, y].direction == Direction.Special)
                    numberOfSpots++;

                finalMap[x, y] = tempMap[x, y];
            }
        }

        finalMap.PrintMap();




        AnsiConsole.MarkupLineInterpolated($"Number of ground spots is [green]{numberOfSpots}[/]");


        void EvaluateTouchingCoords()
        {
            var input = toEvaluate.Dequeue();
            if (allVisitedNodes.ContainsKey(input)) return;
            //if (input.X == 5 && input.Y == 3) Debugger.Break();

            if (overlayMap.AdjacentTest(isConnected => !isConnected, input, true,
                    out Coord[]? adjacentCoords))
            {
                bool reachedEdge = false;

                allVisitedNodes.Add(input, false);
                foreach (var adjacentCoord in adjacentCoords)
                {
                    toEvaluate.Enqueue(adjacentCoord);
                    if (allVisitedNodes.ContainsKey(adjacentCoord))
                        reachedEdge |= allVisitedNodes[adjacentCoord];
                }

                allVisitedNodes[input] = reachedEdge;
                return;
            }

            allVisitedNodes.Add(input, false);
        }

        void InitVisitedNodes(ref Dictionary<Coord, bool> visitedNodes)
        {
            for (int y = 0; y <= overlayMap.Bounds.maxY; y++)
            {
                visitedNodes.Add(new Coord(0, y), !overlayMap[0, y]);
                visitedNodes.Add(new Coord(overlayMap.Bounds.maxX, y), !overlayMap[overlayMap.Bounds.maxX, y]);
            }
            for (int x = 1; x <= overlayMap.Bounds.maxX - 1; x++)
            {
                visitedNodes.Add(new Coord(x, 0), !overlayMap[x, 0]);
                visitedNodes.Add(new Coord(x, overlayMap.Bounds.maxY), !overlayMap[x, overlayMap.Bounds.maxY]);
            }
        }

    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, true);
    }



    (Coord, Direction) FindNextStep(Grid<(Direction direction, bool isConnected, int colorValue)> map, Coord currentLocation, Direction lastDirection)
    {
        var currentDirection = map[currentLocation].direction;

        var tempX = currentLocation.X;
        var tempY = currentLocation.Y;

        if (currentDirection is Direction.NorthSouth)
        {
            if (lastDirection == Direction.North)
            {
                return (new Coord(tempX, tempY - 1), Direction.North);
            }
            return (new Coord(tempX, tempY + 1), Direction.South);

        }

        if (currentDirection is Direction.NorthWest)
        {
            if (lastDirection == Direction.East)
            {
                return (new Coord(tempX, tempY - 1), Direction.North);
            }
            return (new Coord(tempX - 1, tempY), Direction.West);

        }

        if (currentDirection is Direction.NorthEast)
        {
            if (lastDirection == Direction.West)
            {
                return (new Coord(tempX, tempY - 1), Direction.North);
            }
            return (new Coord(tempX + 1, tempY), Direction.East);
        }

        if (currentDirection is Direction.SouthEast or Direction.SouthWest)
        {

            if (lastDirection == Direction.North)
            {
                return currentDirection switch
                {
                    Direction.SouthEast => (new Coord(tempX + 1, tempY), Direction.East),
                    Direction.SouthWest => (new Coord(tempX - 1, tempY), Direction.West),
                };
            }

            return (new Coord(tempX, tempY + 1), Direction.South);
        }

        if (currentDirection == Direction.EastWest && lastDirection == Direction.East)
            return (new Coord(tempX + 1, tempY), Direction.East);

        return (new Coord(tempX - 1, tempY), Direction.West);


    }


    (Coord, Direction) FindFirstStep(Grid<(Direction direction, bool isConnected, int colorValue)> map, Coord currentLocation, Coord skipCoord)
    {
        var westCoord = new Coord(currentLocation.X - 1, currentLocation.Y);
        var eastCoord = new Coord(currentLocation.X + 1, currentLocation.Y);
        var northCoord = new Coord(currentLocation.X, currentLocation.Y - 1);
        var southCoord = new Coord(currentLocation.X, currentLocation.Y + 1);

        // West / Left
        if (!westCoord.Equals(skipCoord)
            && map.InBounds(westCoord)
            && map[westCoord].direction is Direction.EastWest or Direction.NorthEast or Direction.SouthEast)
        {
            return (westCoord, Direction.West);
        }

        if (!eastCoord.Equals(skipCoord)
            && map.InBounds(eastCoord)
            && map[eastCoord].direction is Direction.EastWest or Direction.NorthWest or Direction.SouthWest)
        {
            return (eastCoord, Direction.East);
        }

        if (!northCoord.Equals(skipCoord)
            && map.InBounds(northCoord)
            && map[northCoord].direction is Direction.NorthSouth or Direction.SouthEast or Direction.SouthWest)
        {
            return (northCoord, Direction.North);
        }

        if (!southCoord.Equals(skipCoord)
            && map.InBounds(southCoord)
            && map[southCoord].direction is Direction.NorthSouth or Direction.NorthEast or Direction.NorthWest)
        {
            return (southCoord, Direction.South);
        }

        throw new Exception("Couldn't find first tile");
    }
}