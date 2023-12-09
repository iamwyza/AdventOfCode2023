namespace AdventOfCode2023.Day09;
internal class Day09 : DayBase
{

    public Day09()
    {
        Ready = true;
    }

    private List<int[]> _history = new();

    private async Task Init(int part, bool useTestData)
    {
        _history.Clear();
        
        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        foreach (var line in lines)
        {
            _history.Add(line.Split(' ').Select(x => Convert.ToInt32(x)).ToArray());
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);
        List<int> finalValues = new List<int>();

        foreach (var line in _history)
        {
            int[] temp = line;
            List<int[]> intermediates = new List<int[]>();
            while (true)
            {
                if (TryReduce(temp, out int[] intermediateRow))
                {
                    intermediates.Reverse();
                    int finalValue = line.Last();
                    foreach (var intermediate in intermediates)
                    {
                        finalValue += intermediate.Last();
                    }
                    finalValues.Add(finalValue);
                    AnsiConsole.MarkupLineInterpolated($"Final value of [yellow]{string.Join(' ', line)}[/] is [green]{finalValue}[/]");
                    break;
                }
                temp = intermediateRow;
                intermediates.Add(intermediateRow);
            }
        }

        AnsiConsole.MarkupLineInterpolated($"Final sum of all final values is [green]{finalValues.Sum()}[/].");



        bool TryReduce(int[] line, out int[] intermediate)
        {
            intermediate = new int[line.Length-1];
            bool allZeros = true;
            for (int i = 1; i < line.Length; i++)
            {
                intermediate[i - 1] = line[i]- line[i - 1];
                allZeros &= intermediate[i - 1] == 0;
            }

            return allZeros;
        }
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, false);

        List<int> finalValues = new List<int>();

        bool printDebug = false;

        foreach (var line in _history)
        {
            int[] temp = line;
            List<List<int>> intermediates = new List<List<int>>();
            if (printDebug)
            {
                Console.WriteLine();
                AnsiConsole.WriteLine("Row Boundary:");
                AnsiConsole.MarkupLineInterpolated($"{string.Join(' ', line)}");
            }
            
            while (true)
            {
                if (TryReduce(temp, out int[] intermediateRow))
                {
                    intermediates.Add(intermediateRow.ToList());

                    if (printDebug)
                    {
                        Console.WriteLine("Intermediates"); ;

                        var toPrint = new List<List<int>>
                        {
                            line.ToList(),
                        };
                        toPrint.AddRange(intermediates);
                        toPrint.Add(intermediateRow.ToList());

                        PrintTree(toPrint, false);
                    }

                    intermediates.Reverse();
                    for (int i = 1; i < intermediates.Count; i++)
                    {
                        var value = intermediates[i][0] - intermediates[i - 1][0];
                        intermediates[i].Insert(0, value);
                    }

                    intermediates.Add(line.ToList());
                    intermediates.Reverse();
                    intermediates[0].Insert(0, intermediates[0][0] - intermediates[1][0]);
                    if (printDebug)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Intermediates after."); ;
                        PrintTree(intermediates, false);

                    }

                    var finalValue = intermediates.First()[0];

                    AnsiConsole.MarkupLineInterpolated($"Final value of [yellow]{string.Join(' ', line)}[/] is [green]{finalValue}[/]");

                    finalValues.Add(finalValue);
                    break;
                }
                temp = intermediateRow;
                intermediates.Add(intermediateRow.ToList());
            }
        }

        AnsiConsole.MarkupLineInterpolated($"Final sum of all final values is [green]{finalValues.Sum()}[/].");



        bool TryReduce(int[] line, out int[] intermediate)
        {
            intermediate = new int[line.Length - 1];
            bool allZeros = true;
            for (int i = 1; i < line.Length; i++)
            {
                intermediate[i - 1] = line[i] - line[i - 1];
                allZeros &= intermediate[i - 1] == 0;
            }

            return allZeros;
        }

        void PrintTree(List<List<int>> values, bool invert)
        {

            int level = 0;
            if (invert)
            {
                values.Reverse();
            }
            foreach (var line in values)
            {
                AnsiConsole.MarkupLineInterpolated($"{"".PadLeft(level)}{string.Join(' ', line)}");
                level++;
            }
        }
    }
}
