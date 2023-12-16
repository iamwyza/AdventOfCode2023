namespace AdventOfCode2023.Day16;
internal class Day16 : DayBase
{

    public Day16()
    {
        Ready = true;
        _map = new();
    }


    private Grid<char> _map;
    
    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
    {

        var lines = useTestData ? await GetTestLines(part) : await GetLines();
        
        _map = new Grid<char>(lines, (c) => c);

        _map.PrintMap(null, true, true);
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        var total = CalculateMax(new Coord(0, 0), Direction.East);

        AnsiConsole.MarkupLineInterpolated($"Total number of energized spaces is [green]{total}[/]");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        var max = 0;
        for (int y = 0; y <= _map.Bounds.maxY; y++)
        {
            if (y == 0 || y == _map.Bounds.maxY)
            {
                for (int x = 0; x <= _map.Bounds.maxX; x++)
                {
                    var temp = GetMax(x, y);
                    if (temp > max) 
                        max = temp;
                }
            }
            else
            {
                var temp = GetMax(0, y);
                if (temp > max)
                    max = temp;

                temp = GetMax(_map.Bounds.maxX, y);
                if (temp > max)
                    max = temp;
            }
        }

        int GetMax(int x, int y)
        {
            var temp = 0;
            var tMax = 0;
            var startCoord = new Coord(x, y);

            if (y == 0)
            {
                temp = CalculateMax(startCoord, Direction.South);
                if (temp > tMax)
                    tMax = temp;
            }

            if (x == 0)
            {
                temp = CalculateMax(startCoord, Direction.East);
                if (temp > tMax)
                    tMax = temp;
            }

            if (y == _map.Bounds.maxY)
            {
                temp = CalculateMax(startCoord, Direction.North);
                if (temp > tMax)
                    tMax = temp;
            }

            if (x == _map.Bounds.maxX)
            {
                temp = CalculateMax(startCoord, Direction.West);
                if (temp > tMax)
                    tMax = temp;
            }

            return tMax;

        }

        AnsiConsole.MarkupLineInterpolated($"Max number of energized spaces is [green]{max}[/]");
    }

    public int CalculateMax(Coord startCoord, Direction startDirection)
    {
        Queue<(Coord location, Direction direction)> beams = new Queue<(Coord, Direction)>();

        startCoord = startDirection switch
        {
            Direction.East => new Coord(startCoord.X - 1, startCoord.Y),
            Direction.West => new Coord(startCoord.X + 1, startCoord.Y),
            Direction.North => new Coord(startCoord.X, startCoord.Y + 1),
            Direction.South => new Coord(startCoord.X, startCoord.Y - 1)
        };


        var energizedMap = new Grid<int>
        {
            DefaultPrintConfig = (c, _) => (c > 0 ? '#' : '.', c > 0 ? Color.Yellow : Color.Default, null)
        };
        energizedMap.CheckBounds(new Coord(_map.Bounds.maxX, _map.Bounds.maxY));
        energizedMap.InitMap();
        Direction currentDirection = Direction.None;
        Coord currentCoord = startCoord;
        _map.DefaultPrintConfig = (input, coord) => (coord.Equals(currentCoord) ? currentDirection switch
        {
            Direction.North => '^',
            Direction.East => '>',
            Direction.South => 'v',
            Direction.West => '<'
        } : input, energizedMap[coord] > 0 ? Color.Yellow : Color.Default, null);
        beams.Enqueue((startCoord, startDirection));
        while (beams.Count != 0)
        {
            var beam = beams.Dequeue();
            var end = false;
            while (!end)
            {
                if (energizedMap.InBounds(beam.location) && ((Direction)energizedMap[beam.location]).HasFlag(beam.direction)) // We've already came through here before.
                {
                    break;
                }

                var newLocation = beam.direction switch
                {
                    Direction.East => new Coord(beam.location.X + 1, beam.location.Y),
                    Direction.West => new Coord(beam.location.X - 1, beam.location.Y),
                    Direction.North => new Coord(beam.location.X, beam.location.Y - 1),
                    Direction.South => new Coord(beam.location.X, beam.location.Y + 1)
                };


                //AnsiConsole.MarkupLineInterpolated($"Moving from [yellow]{beam.location}[/] to [yellow]{newLocation}[/] via [red]{beam.direction}[/]");

                if (energizedMap.InBounds(beam.location))
                    energizedMap[beam.location] = (int)((Direction)energizedMap[beam.location] | beam.direction);

                if (!_map.InBounds(newLocation))
                    break;

                currentCoord = newLocation;
                currentDirection = beam.direction;

                //energizedMap.PrintMap(null, true, true);
                // _map.PrintMap(null, true, true);
                beam.location = newLocation;

                switch (_map[newLocation])
                {
                    case '.':
                        continue;
                    case '-':
                        if (beam.direction is Direction.East or Direction.West)
                            continue;

                        beams.Enqueue((newLocation, Direction.East));
                        beams.Enqueue((newLocation, Direction.West));
                        end = true;
                        break;
                    case '|':
                        if (beam.direction is Direction.North or Direction.South)
                            continue;

                        beams.Enqueue((newLocation, Direction.North));
                        beams.Enqueue((newLocation, Direction.South));
                        end = true;
                        break;
                    case '/':
                        switch (beam.direction)
                        {
                            case Direction.North:
                                beam.direction = Direction.East;
                                break;
                            case Direction.East:
                                beam.direction = Direction.North;
                                break;
                            case Direction.South:
                                beam.direction = Direction.West;
                                break;
                            case Direction.West:
                                beam.direction = Direction.South;
                                break;
                        }
                        break;
                    case '\\':
                        switch (beam.direction)
                        {
                            case Direction.North:
                                beam.direction = Direction.West;
                                break;
                            case Direction.East:
                                beam.direction = Direction.South;
                                break;
                            case Direction.South:
                                beam.direction = Direction.East;
                                break;
                            case Direction.West:
                                beam.direction = Direction.North;
                                break;
                        }
                        break;
                }
            }
        }
        //energizedMap.PrintMap();

        var total = 0;
        foreach (var cell in energizedMap)
        {
            if (cell.Item2 > 0)
                total++;
        }

        return total;
    }
}