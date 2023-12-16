namespace AdventOfCode2023.Day14;
internal class Day14 : DayBase
{
    public Day14()
    {
        Ready = true;
        _map = new();
    }

    private Grid<sbyte> _map;
  
    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
    {
        _map = new Grid<sbyte>();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _map.CheckBounds(new Coord(lines[0].Length-1, lines.Length-1));
        _map.InitMap();
        _map.DefaultPrintConfig = (value, coord) => (value switch
        {
            1 => 'O',
            2 => '#',
            0 => '.'
        }, Color.Default, "");

        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                _map[x, y] = lines[y][x] switch
                {
                    'O' => 1,
                    '#' => 2,
                    '.' => 0
                };
            }
        }


        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        var total = 0;
        for (int y = 1; y <= _map.Bounds.maxY; y++)
        {
            for (int x = 0; x <= _map.Bounds.maxX; x++)
            {
                if (_map[x, y] == 1)
                {
                    var currentRow = y-1;
                    while (currentRow >= 0)
                    {
                        if (_map[x, currentRow] == 0)
                        {
                            currentRow--;
                            continue;
                        }

                        break;
                    }

                    if (currentRow != y - 1)
                    {
                        _map[x, currentRow+1] = 1;
                        _map[x, y] = 0;
                    }
                }
            }
        }

        for (int y = 0; y <= _map.Bounds.maxY; y++)
        {
            for (int x = 0; x <= _map.Bounds.maxX; x++)
            {
                if (_map[x, y] == 1)
                    total += _map.Bounds.maxY+1 - y;
            }
        }

        _map.PrintMap();
        AnsiConsole.MarkupLineInterpolated($"Total is [green]{total}[/]");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, true);
        bool debug = false;
        var total = 0;
        for (int i = 0; i < 1000000000; i++)
        {

            if (debug)
            {
                AnsiConsole.MarkupLineInterpolated($"Cycle [red]{i+1}[/] starting");
                AnsiConsole.MarkupLine("Moving [green]North[/]");

            }

            for (int y = 1; y <= _map.Bounds.maxY; y++)
            {
                for (int x = 0; x <= _map.Bounds.maxX; x++)
                {
                    if (_map[x, y] == 1)
                    {
                        var currentRow = y - 1;
                        while (currentRow >= 0)
                        {
                            if (_map[x, currentRow] == 0)
                            {
                                currentRow--;
                                continue;
                            }

                            break;
                        }

                        if (currentRow != y - 1)
                        {
                            _map[x, currentRow + 1] = 1;
                            _map[x, y] = 0;
                        }
                    }
                }
            }

            if (debug)
            {
                _map.PrintMap();
                AnsiConsole.MarkupLine("Moving [green]West[/]");
            }


            for (int x = 1; x <= _map.Bounds.maxX; x++)
            {
                for (int y = 0; y <= _map.Bounds.maxY; y++)
                {
                    if (_map[x, y] == 1)
                    {
                        var currentCol = x - 1;
                        while (currentCol >= 0)
                        {
                            if (_map[currentCol, y] == 0)
                            {
                                currentCol--;
                                continue;
                            }

                            break;
                        }

                        if (currentCol != x - 1)
                        {
                            _map[currentCol + 1, y] = 1;
                            _map[x, y] = 0;
                        }
                    }
                }
            }

            if (debug)
            {
                _map.PrintMap();
                AnsiConsole.MarkupLine("Moving [green]South[/]");
            }

            for (int y = _map.Bounds.maxY - 1; y >= 0; y--)
            {
                for (int x = 0; x <= _map.Bounds.maxX; x++)
                {
                    if (_map[x, y] == 1)
                    {
                        var currentRow = y + 1;
                        while (currentRow <= _map.Bounds.maxY)
                        {
                            if (_map[x, currentRow] == 0)
                            {
                                currentRow++;
                                continue;
                            }

                            break;
                        }

                        if (currentRow != y + 1)
                        {
                            _map[x, currentRow - 1] = 1;
                            _map[x, y] = 0;
                        }
                    }
                }
            }

            if (debug)
            {
                _map.PrintMap();
                AnsiConsole.MarkupLine("Moving [green]East[/]");
            }

            for (int x = _map.Bounds.maxX - 1; x >= 0; x--)
            {
                for (int y = 0; y <= _map.Bounds.maxY; y++)
                {
                    if (_map[x, y] == 1)
                    {
                        var currentCol = x + 1;
                        while (currentCol <= _map.Bounds.maxX)
                        {
                            if (_map[currentCol, y] == 0)
                            {
                                currentCol++;
                                continue;
                            }

                            break;
                        }

                        if (currentCol != x + 1)
                        {
                            _map[currentCol - 1, y] = 1;
                            _map[x, y] = 0;
                        }
                    }
                }
            }

        }

        for (int y = 0; y <= _map.Bounds.maxY; y++)
        {
            for (int x = 0; x <= _map.Bounds.maxX; x++)
            {
                if (_map[x, y] == 1)
                    total += _map.Bounds.maxY + 1 - y;
            }
        }

        _map.PrintMap();
        AnsiConsole.MarkupLineInterpolated($"Total is [green]{total}[/]");

    }
}