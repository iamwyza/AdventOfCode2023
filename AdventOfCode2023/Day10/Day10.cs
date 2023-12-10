using System.Diagnostics;
using Spectre.Console.Rendering;

namespace AdventOfCode2023.Day10
;
internal class Day10 : DayBase
{
    public Day10()
    {
        Ready = true;
    }

    private Grid<(Direction direction, bool isConnected)> _map;
    private Coord _startCoord = new Coord();

    [MemberNotNull(nameof(_map))]
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
                    Direction.None => '.',
                    Direction.Special => 'S',
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
        (Coord nextATile, Direction lastADirection) = FindFirstStep(_startCoord, new Coord(-1, -1));
        (Coord nextBTile, Direction lastBDirection) = FindFirstStep(_startCoord, nextATile);

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

            (Coord tempA, Direction nextADirection) = FindNextStep(nextATile, lastADirection);
            (Coord tempB, Direction nextBDirection) = FindNextStep(nextBTile, lastBDirection);

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

        (Coord nextATile, Direction lastADirection) = FindFirstStep(_startCoord, new Coord(-1, -1));
        (Coord nextBTile, Direction lastBDirection) = FindFirstStep(_startCoord, nextATile);

        var tileA = _map[nextATile];
        tileA.isConnected = true;
        _map[nextATile] = tileA;

        var tileB = _map[nextBTile];
        tileB.isConnected = true;
        _map[nextBTile] = tileB;

        _map.DefaultPrintConfig = ((Direction Direction, bool isConnected) input, Coord coord) =>
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
        while (true)
        {
            steps++;

            (Coord tempA, Direction nextADirection) = FindNextStep(nextATile, lastADirection);
            (Coord tempB, Direction nextBDirection) = FindNextStep(nextBTile, lastBDirection);

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

        // overlayMap.PrintMap(null, true);
        List<Dictionary<Coord, bool>> thisIsDumb = new();

        var allVisitedNodesA = new Dictionary<Coord, bool>();

        InitVisitedNodes(ref allVisitedNodesA);

        for (int y = 0; y <= overlayMap.Bounds.maxY; y++)
        {
            for (int x = 0; x <= overlayMap.Bounds.maxX; x++)
            {
                EvaluateTouchingCoords(new Coord(x, y), allVisitedNodesA);
            }
        }

        thisIsDumb.Add(allVisitedNodesA);

        //var allVisitedNodesB = new Dictionary<Coord, bool>();
        //InitVisitedNodes(ref allVisitedNodesB);

        //for (int y = overlayMap.Bounds.maxY; y >= 0; y--)
        //{
        //    for (int x = overlayMap.Bounds.maxX; x >= 0; x--)
        //    {
        //        EvaluateTouchingCoords(new Coord(x, y), allVisitedNodesB);
        //    }
        //}

        //thisIsDumb.Add(allVisitedNodesB);

        //var allVisitedNodesC = new Dictionary<Coord, bool>();
        //InitVisitedNodes(ref allVisitedNodesC);


        //for (int y = 0; y <= overlayMap.Bounds.maxY; y++)
        //{
        //    for (int x = overlayMap.Bounds.maxX; x >= 0; x--)
        //    {
        //        EvaluateTouchingCoords(new Coord(x, y), allVisitedNodesC);
        //    }
        //}

        //thisIsDumb.Add(allVisitedNodesC);

        //var allVisitedNodesD = new Dictionary<Coord, bool>();

        //InitVisitedNodes(ref allVisitedNodesD);


        //for (int y = overlayMap.Bounds.maxY; y >= 0; y--)
        //{
        //    for (int x = 0; x <= overlayMap.Bounds.maxX; x++)
        //    {
        //        EvaluateTouchingCoords(new Coord(x, y), allVisitedNodesD);
        //    }
        //}

        //thisIsDumb.Add(allVisitedNodesD);


        int numberOfSpots = 0;
        for (int y = 0; y <= overlayMap.Bounds.maxY; y++)
        {
            for (int x = 0; x <= overlayMap.Bounds.maxX; x++)
            {
                if (thisIsDumb.All(z => z[new Coord(x, y)] == false))
                    if (!_map[x, y].isConnected)
                    {
                        _map[x, y] = (Direction.Special, false);
                        numberOfSpots++;
                    }
            }
        }



        overlayMap.PrintMap(null, true);

        _map.PrintMap(null, false);

        AnsiConsole.MarkupLineInterpolated($"Number of ground spots is [green]{numberOfSpots}[/]");

        (bool, Coord[]? adjacentCoords) EvaluateTouchingCoords(Coord input, Dictionary<Coord, bool> visitedNodes)
        {

            if (visitedNodes.ContainsKey(input))
                return (visitedNodes[input], null);
            //if (input.X == 3 && input.Y == 7) Debugger.Break();

            if (overlayMap!.AdjacentTest(isConnected => !isConnected, input, true,
                    out Coord[]? adjacentCoords))
            {
                bool reachedEdge = false;
                List<Coord[]> nestedAdjacentCoords = new List<Coord[]>();

                if (adjacentCoords is not null)
                {
                    nestedAdjacentCoords.Add(adjacentCoords);
                }

                visitedNodes.Add(input, false);
                foreach (var adjacentCoord in adjacentCoords)
                {
                    var temp = EvaluateTouchingCoords(adjacentCoord, visitedNodes);
                    reachedEdge |= temp.Item1;
                    if (temp.adjacentCoords is not null)
                        nestedAdjacentCoords.Add(temp.adjacentCoords);
                }

                var allAdjacentCoords = nestedAdjacentCoords.SelectMany(x => x).ToArray();
                foreach (var adjacentCoord in allAdjacentCoords)
                {
                    visitedNodes[adjacentCoord] = reachedEdge;
                }
                visitedNodes[input] = reachedEdge;
                return (reachedEdge, allAdjacentCoords);
            }


            visitedNodes.Add(input, false);
            return (false, null);

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



    (Coord, Direction) FindNextStep(Coord currentLocation, Direction lastDirection)
    {
        var currentDirection = _map[currentLocation].direction;

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


    (Coord, Direction) FindFirstStep(Coord currentLocation, Coord skipCoord)
    {
        var westCoord = new Coord(currentLocation.X - 1, currentLocation.Y);
        var eastCoord = new Coord(currentLocation.X + 1, currentLocation.Y);
        var northCoord = new Coord(currentLocation.X, currentLocation.Y - 1);
        var southCoord = new Coord(currentLocation.X, currentLocation.Y + 1);

        // West / Left
        if (!westCoord.Equals(skipCoord)
            && _map.InBounds(westCoord)
            && _map[westCoord].direction is Direction.EastWest or Direction.NorthEast or Direction.SouthEast)
        {
            return (westCoord, Direction.West);
        }

        if (!eastCoord.Equals(skipCoord)
            && _map.InBounds(eastCoord)
            && _map[eastCoord].direction is Direction.EastWest or Direction.NorthWest or Direction.SouthWest)
        {
            return (eastCoord, Direction.East);
        }

        if (!northCoord.Equals(skipCoord)
            && _map.InBounds(northCoord)
            && _map[northCoord].direction is Direction.NorthSouth or Direction.SouthEast or Direction.SouthWest)
        {
            return (northCoord, Direction.North);
        }

        if (!southCoord.Equals(skipCoord)
            && _map.InBounds(southCoord)
            && _map[southCoord].direction is Direction.NorthSouth or Direction.NorthEast or Direction.NorthWest)
        {
            return (southCoord, Direction.South);
        }

        throw new Exception("Couldn't find first tile");
    }
}
