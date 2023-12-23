using System.Diagnostics;
using AdventOfCode2023.GridUtilities;
using Spectre.Console.Rendering;

namespace AdventOfCode2023.Day10
;
internal class Day10 : DayBase
{
    public Day10()
    {
        Ready = true;
        _map = new Grid<(Direction direction, bool isConnected)>();
    }

    private Grid<(Direction direction, bool isConnected)> _map;
    private Coord _startCoord = new Coord();

    private async Task Init(int part, bool useTestData)
    {
        _map = new Grid<(Direction direction, bool isConnected)>();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _map.CheckBounds(new Coord(lines[0].Length - 1, lines.Length - 1));
        _map.InitMap();

        _map.DefaultPrintConfig = ((Direction Direction, bool isConnected) input, Coord coord) =>
        {
            return (
                input.Direction switch
                {
                    Direction.NorthSouth => '|',
                    Direction.EastWest => '-',
                    Direction.NorthEast => 'L',
                    Direction.NorthWest => 'J',
                    Direction.SouthWest => '7',
                    Direction.SouthEast => 'F',
                    Direction.None => ' ',
                    Direction.Special => 'S',
                    _ => throw new ArgumentOutOfRangeException()
                }
                , input.isConnected ? Color.Green : Color.Default
                , null);
        };

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                _map[x, y] = lines[y][x] switch
                {
                    '|' => (Direction.NorthSouth, false),
                    '-' => (Direction.EastWest, false),
                    'L' => (Direction.NorthEast, false),
                    'J' => (Direction.NorthWest, false),
                    '7' => (Direction.SouthWest, false),
                    'F' => (Direction.SouthEast, false),
                    '.' => (Direction.None, false),
                    'S' => (Direction.Special, true),
                    _ => throw new ArgumentOutOfRangeException()
                };
                if (lines[y][x] == 'S')
                {
                    _startCoord = new Coord(x, y);
                }
            }
        }


        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);
        (Coord nextATile, Direction lastADirection) = FindFirstStep(_map, _startCoord, new Coord(-1, -1));
        (Coord nextBTile, Direction lastBDirection) = FindFirstStep(_map, _startCoord, nextATile);

        var tileA = _map[nextATile];
        tileA.isConnected = true;
        _map[nextATile] = tileA;

        var tileB = _map[nextBTile];
        tileB.isConnected = true;
        _map[nextBTile] = tileB;


        int steps = 1;
        while (true)
        {
            steps++;

            (Coord tempA, Direction nextADirection) = FindNextStep(_map, nextATile, lastADirection);
            (Coord tempB, Direction nextBDirection) = FindNextStep(_map, nextBTile, lastBDirection);

            lastADirection = nextADirection;
            nextATile = tempA;
            tileA = _map[tempA];
            tileA.isConnected = true;
            _map[tempA] = tileA;

            lastBDirection = nextBDirection;
            nextBTile = tempB;
            tileB = _map[tempB];
            tileB.isConnected = true;
            _map[tempB] = tileB;

            if (tempA.Equals(tempB))
                break;
            //_map.PrintMap();

        }

        _map.PrintMap(null, false);

        AnsiConsole.MarkupLineInterpolated($"Furthest point is [green]{steps}[/] away at [green]{nextATile}[/].");


    }


    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        var tempMap = new Grid<(Direction direction, bool isConnected)>();
        tempMap.CheckBounds(new Coord(_map.Bounds.maxX*2 + 4, _map.Bounds.maxY*2 + 4));
        tempMap.InitMap();

        int yOffset = 2;
        for (int y = 0; y <= _map.Bounds.maxY; y++)
        {
            int xOffset = 2;
            for (int x = 0; x <= _map.Bounds.maxX; x++)
            {
                tempMap[x+xOffset,y+yOffset] = _map[x,y];

                if (_map[x, y].direction == Direction.Special)
                    _startCoord = new Coord(x + xOffset, y + yOffset);

                xOffset++;
                tempMap[x + xOffset, y + yOffset] = (Direction.None, false);

                
            }

            yOffset++;
        }

        tempMap.DefaultPrintConfig = _map.DefaultPrintConfig;
        tempMap.PrintMap();

        for (int y = 1; y < tempMap.Bounds.maxY; y++)
        {
            for (int x = 1; x < tempMap.Bounds.maxX-1; x++)
            {
                if (tempMap[x, y - 1].direction is Direction.SouthEast or Direction.NorthSouth or Direction.SouthWest or Direction.Special
                    && tempMap[x, y + 1].direction is Direction.NorthEast or Direction.NorthWest 
                        or Direction.NorthSouth or Direction.Special)
                    tempMap[x, y] = (Direction.NorthSouth, false);

            }
        }

        for (int x = 1; x < tempMap.Bounds.maxX - 1; x++)
        {
            for (int y = 1; y < tempMap.Bounds.maxY; y++)
            {
                if (tempMap[x-1, y].direction is Direction.NorthEast or Direction.SouthEast or Direction.EastWest or Direction.Special
                    && tempMap[x+1, y].direction is Direction.NorthWest or Direction.SouthWest
                        or Direction.EastWest or Direction.Special)
                    tempMap[x, y] = (Direction.EastWest, false);

            }
        }

        tempMap.PrintMap();
        

        (Coord nextATile, Direction lastADirection) = FindFirstStep(tempMap, _startCoord, new Coord(-1, -1));
        (Coord nextBTile, Direction lastBDirection) = FindFirstStep(tempMap, _startCoord, nextATile);

        var tileA = tempMap[nextATile];
        tileA.isConnected = true;
        tempMap[nextATile] = tileA;

        var tileB = tempMap[nextBTile];
        tileB.isConnected = true;
        tempMap[nextBTile] = tileB;

        

        tempMap.DefaultPrintConfig = ((Direction Direction, bool isConnected) input, Coord coord) =>
        {
            if (input.isConnected)
            {
                return (
                    input.Direction switch
                    {
                        Direction.NorthSouth => '|',
                        Direction.EastWest => '-',
                        Direction.NorthEast => 'L',
                        Direction.NorthWest => 'J',
                        Direction.SouthWest => '7',
                        Direction.SouthEast => 'F',
                        Direction.None => '.',
                        Direction.Special => 'S',
                        _ => throw new ArgumentOutOfRangeException()
                    }
                    , input.isConnected ? Color.Green : Color.Default
                    , null);
            }
            else
                return (
                    input.Direction == Direction.Special ? '0' : '.'
                    , input.Direction == Direction.Special ? Color.Blue : Color.Default
                    , null);
        };

        int steps = 1;
        while (true && steps < 1_000_000)
        {
            steps++;

            (Coord tempA, Direction nextADirection) = FindNextStep(tempMap, nextATile, lastADirection);
            (Coord tempB, Direction nextBDirection) = FindNextStep(tempMap, nextBTile, lastBDirection);

            lastADirection = nextADirection;
            nextATile = tempA;
            tileA = tempMap[tempA];
            tileA.isConnected = true;
            tempMap[tempA] = tileA;

            lastBDirection = nextBDirection;
            nextBTile = tempB;
            tileB = tempMap[tempB];
            tileB.isConnected = true;
            tempMap[tempB] = tileB;

            if (tempA.Equals(tempB))
                break;
            //tempMap.PrintMap();
        }

        var overlayMap = new Grid<bool>();
        overlayMap.CheckBounds(new Coord(tempMap.Bounds.maxX, tempMap.Bounds.maxY));
        overlayMap.InitMap();

        overlayMap.DefaultPrintConfig = (touchesEdge, location) =>
            (touchesEdge ? '.' : '0', touchesEdge ? Color.Blue : Color.Default, null);

        for (int y = 0; y <= overlayMap.Bounds.maxY; y++)
        {
            for (int x = 0; x <= overlayMap.Bounds.maxX; x++)
            {
                overlayMap[x, y] = tempMap[x, y].isConnected;
            }
        }

        // overlayMap.PrintMap(null, true);
        var allVisitedNodes = new Dictionary<Coord, bool>();

        InitVisitedNodes(ref allVisitedNodes);

        Queue<Coord> toEvaluate = new Queue<Coord>();
        toEvaluate.Enqueue(new Coord(1, 1));

        while (toEvaluate.Count > 0)
        {
            EvaluateTouchingCoords();

        }

        for (int y = 0; y <= overlayMap.Bounds.maxY; y++)
        {
            for (int x = 0; x <= overlayMap.Bounds.maxX; x++)
            {
                var coord = new Coord(x, y);
                if (allVisitedNodes.ContainsKey(coord) && allVisitedNodes[coord] == false)
                    if (!tempMap[coord].isConnected)
                    {
                        tempMap[coord] = (Direction.Special, false);
                    }
                if (!allVisitedNodes.ContainsKey(coord) && !tempMap[coord].isConnected)
                {
                    tempMap[coord] = (Direction.Special, false);
                }
            }
        }

        overlayMap.PrintMap();

        tempMap.PrintMap();

        var finalMap = new Grid<(Direction direction, bool isConnected)>();
        finalMap.CheckBounds(new Coord((tempMap.Bounds.maxX/2), (tempMap.Bounds.maxY/2)));
        finalMap.InitMap();
        finalMap.PrintMap();
        int numberOfSpots = 0;

        for (int y = 0; y <= tempMap.Bounds.maxY; y += 2)
        {
            for (int x = 0; x <= tempMap.Bounds.maxX; x += 2)
            {
                if (!tempMap[x, y].isConnected && tempMap[x, y].direction == Direction.Special)
                    numberOfSpots++;

                finalMap[x/2, y/2] = tempMap[x, y];
            }
        }

        finalMap.DefaultPrintConfig = _map.DefaultPrintConfig;
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



    (Coord, Direction) FindNextStep(Grid<(Direction direction, bool isConnected)> map, Coord currentLocation, Direction lastDirection)
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
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return (new Coord(tempX, tempY + 1), Direction.South);
        }

        if (currentDirection == Direction.EastWest && lastDirection == Direction.East)
            return (new Coord(tempX + 1, tempY), Direction.East);

        return (new Coord(tempX - 1, tempY), Direction.West);


    }


    (Coord, Direction) FindFirstStep(Grid<(Direction direction, bool isConnected)> map, Coord currentLocation, Coord skipCoord)
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
