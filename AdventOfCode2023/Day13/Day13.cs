using System.Diagnostics;
using System.Text;
using AdventOfCode2023.GridUtilities;

namespace AdventOfCode2023.Day13
;
internal class Day13 : DayBase
{

    public Day13()
    {
        Ready = true;
        _maps2 = new();
    }

    private readonly List<(int[] rows, int[] columns)> _maps2;

    private async Task Init(int part, bool useTestData)
    {
        _maps2.Clear();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();
        var buffer = new List<string>();

        Func<bool, Coord, (char letter, Color color, string? extraText)> defaultConfig = (input, _) => (input ? '#' : '.', Color.Default, string.Empty);

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
               AddMap();
            }
            else
            {
                buffer.Add(line);
            }
        }

        AddMap();

        void AddMap()
        {
            var rows = buffer.Select(x => Convert.ToInt32(x.Replace("#", "1").Replace(".", "0"), 2)).ToArray();
            int[] columns = new int[buffer[0].Length];

            for (int x = 0; x < buffer[0].Length; x++)
            {
                var stringBuilder = new StringBuilder();
                foreach (var bLine in buffer)
                {
                    stringBuilder.Append(bLine[x]);
                }

                columns[x] = Convert.ToInt32(stringBuilder.ToString().Replace("#", "1").Replace(".", "0"), 2);

            }

            _maps2.Add((rows, columns));
            buffer.Clear();
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);
        var total = 0;

        foreach (var set in _maps2)
        {
            AnsiConsole.WriteLine(string.Join(" ", set.columns));
            AnsiConsole.WriteLine(string.Join(" ", set.rows));

            var mirroredAtRow = CalculateMirrors(set.rows) * 100;
            var mirroredAtColumn = CalculateMirrors(set.columns);
            total += mirroredAtColumn + mirroredAtRow;
        }

        AnsiConsole.MarkupLineInterpolated($"Total is [green]{total}[/]");

        int CalculateMirrors(int[] rowOrCol)
        {
            var possibleMirrorStarts = new List<int>();
            for (var index = 0; index < rowOrCol.Length - 1; index++)
            {
                var row = rowOrCol[index];
                var nextRow = rowOrCol[index + 1];

                if (row == nextRow)
                    possibleMirrorStarts.Add(index);
            }

            foreach (var row in possibleMirrorStarts)
            {
                (int left, int right) = (row, row + 1);
                bool isMirror = true;
                while (true)
                {
                    left--;
                    right++;

                    if (left < 0 || right > rowOrCol.Length-1)
                    {
                        break;
                    }

                    if (rowOrCol[left] != rowOrCol[right])
                    {
                        isMirror = false;
                        break;
                    }
                }

                if (isMirror)
                {
                    AnsiConsole.MarkupLineInterpolated($"Mirrored between [yellow]{row+1}[/] and [yellow]{row + 2}[/]");
                    return row + 1;
                }
            }

            return 0;
        }
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        var total = 0;

        for (var index = 0; index < _maps2.Count; index++)
        {
            var set = _maps2[index];
            AnsiConsole.WriteLine("Columns: " + string.Join(" ", set.columns));
            AnsiConsole.WriteLine("Rows:    " + string.Join(" ", set.rows));
            Print(set, -1, -1);

            var mirroredAtRow = 0;
            var mirroredAtColumn = 0;
            var excludingRow = CalculateMirrors(set.rows, true, -1);
            var excludingColumn = CalculateMirrors(set.columns, false, -1);

            for (int i = 0; i < set.rows.Length; i++)
            {
                
                var numberOfBits = set.columns.Length;
                for (int b = 0; b < numberOfBits; b++)
                {
                    var rows = (int[])set.rows.Clone();
                    rows[i] ^= (1 << b);

                    mirroredAtRow = CalculateMirrors(rows, true, excludingRow);
                    if (mirroredAtRow >= 0)
                    {
                        mirroredAtRow++;
                        Print((rows, set.columns), i, b);
                        break;
                    }
                }

                if (mirroredAtRow >= 0)
                {
                    break;
                }
            }

            if (mirroredAtRow <= 0)
            {
                for (int i = 0; i < set.columns.Length; i++)
                {
                    var numberOfBits = set.rows.Length;

                    for (int b = 0; b < numberOfBits; b++)
                    {
                        var cols = (int[])set.columns.Clone();
                        cols[i] ^= (1 << b);

                        mirroredAtColumn = CalculateMirrors(cols, false, excludingColumn);

                        if (mirroredAtColumn >= 0)
                        {
                            mirroredAtColumn++;
                            Print((set.rows, cols), b, i);
                            break;
                        }
                    }

                    if (mirroredAtColumn >= 0)
                    {
                        break;
                    }
                }
            }

            if (mirroredAtColumn < 0 && mirroredAtRow < 0)
            {
                Debugger.Break();
            }

            if (mirroredAtColumn < 0)
                mirroredAtColumn = 0;

            if (mirroredAtRow < 0)
                mirroredAtRow = 0;

            total += mirroredAtColumn + (mirroredAtRow * 100);
        }

        AnsiConsole.MarkupLineInterpolated($"Total is [green]{total}[/]");

        void Print((int[] rows, int[] columns) set, int highlightRow, int highlightBit)
        {
            AnsiConsole.WriteLine();
            for (var index = 0; index < set.rows.Length; index++)
            {
                var row = set.rows[index];
                var temp = row.ToString("B").PadLeft(set.columns.Length, '0').Replace('0', '.').Replace('1', '#');

                if (index == highlightRow)
                {
                    var bit = temp.Length - highlightBit - 1;
                    temp = temp.Insert(bit + 1, "[/]");
                    temp = temp.Insert(bit, "[red]");
                }
                AnsiConsole.MarkupLine($"{index+1:D2} " + temp + " " 
                                       + row.ToString("B").PadLeft(set.columns.Length, '0') 
                                       + $" [yellow]{row}[/]"
                                       );
            }

            AnsiConsole.WriteLine();

            for (var index = 0; index < set.columns.Length; index++)
            {
                var column = set.columns[index];
                var temp = column.ToString("B").PadLeft(set.rows.Length, '0').Replace('0', '.').Replace('1', '#');
               
                AnsiConsole.MarkupLine($"{index + 1:D2} " + temp + " "
                                       + column.ToString("B").PadLeft(set.columns.Length, '0')
                                       + $" [yellow]{column}[/]"
                );
            }

            AnsiConsole.WriteLine();
        }

        int CalculateMirrors(int[] rowOrCol, bool isRow, int excludeRow)
        {
            var possibleMirrorStarts = new List<int>();
            for (var index = 0; index < rowOrCol.Length - 1; index++)
            {
                var row = rowOrCol[index];
                var nextRow = rowOrCol[index + 1];

                if (row == nextRow && index != excludeRow)
                    possibleMirrorStarts.Add(index);
            }

            possibleMirrorStarts.Reverse();

            foreach (var row in possibleMirrorStarts)
            {
                (int left, int right) = (row, row + 1);
                bool isMirror = true;

                while (true)
                {
                    isMirror &= rowOrCol[left] == rowOrCol[right];

                    if (!isMirror)
                        break;

                    left--;
                    right++;

                    if (left < 0 || right > rowOrCol.Length - 1)
                    {
                        break;
                    }
                }

                if (isMirror)
                {
                    if (excludeRow != -1)
                        AnsiConsole.MarkupLineInterpolated($"Mirrored [red]{(isRow ? "row" : "col")}[/] between [yellow]{row + 1}[/] and [yellow]{row + 2}[/]");

                    return row;
                }
            }

            return -1;
        }
    }
}