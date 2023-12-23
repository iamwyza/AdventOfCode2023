namespace AdventOfCode2023.Day15;
internal class Day15 : DayBase
{

    public Day15()
    {
        Ready = true;
    }

    private List<string> _steps = new();
    private async Task Init(int part, bool useTestData)
    {
        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _steps = lines[0].Split(',').ToList();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        int total = _steps.Sum(x => HashSteps(x));

        AnsiConsole.MarkupLineInterpolated($"Result of Hash is: [green]{total}[/] ");
    }

    public byte HashSteps(string steps)
    {
        var currentValue = 0;

        foreach (var step in steps)
        {
            //AnsiConsole.MarkupLineInterpolated($"Letter: [red]{step}[/]");
            //AnsiConsole.MarkupLineInterpolated($"Current Value is [yellow]{currentValue}[/]");
            currentValue += step;
            //AnsiConsole.MarkupLineInterpolated($"Current Value is [yellow]{currentValue}[/]");
            currentValue *= 17;
            //AnsiConsole.MarkupLineInterpolated($"Current Value is [yellow]{currentValue}[/]");
            currentValue %= 256;
        }

        return (byte)currentValue;
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        Dictionary<byte, List<(string label, byte value)>> boxes = new();

        for (byte i = 0; i < 255; i++)
        {
            boxes.Add(i, new List<(string, byte)>());
        }
        boxes.Add(255, new List<(string label, byte value)>());

        foreach (var step in _steps)
        {
            var temp = step.Split('=');
            string label;
            byte boxId;

            if (temp.Length == 2)
            {
                label = temp[0];
                var focalLength = Convert.ToByte(temp[1]);

                boxId = HashSteps(label);

                var box = boxes[boxId];

                var existingLens = box.FindIndex(x => x.Item1 == label);

                if (existingLens > -1)
                {
                    box.RemoveAt(existingLens);
                    box.Insert(existingLens, (label, focalLength));
                }
                else
                {
                    box.Add((label, focalLength));
                }
            }
            else
            {
                label = temp[0].Substring(0, temp[0].Length - 1);
                boxId = HashSteps(label);

                var box = boxes[boxId];

                var existingLens = box.FindIndex(x => x.Item1 == label);
                if (existingLens > -1)
                {
                    box.RemoveAt(existingLens);
                }
            }

            //AnsiConsole.MarkupLineInterpolated($"After '[yellow]{step}[/]': ");
            //PrintBoxes();
            //AnsiConsole.WriteLine();
            
        }

        var total = 0;
        foreach (var box in boxes)
        {
            var boxFocusingPower = 0;
            for (int i = 0; i < box.Value.Count; i++)
            {
                var lensFocusingPower = 1 + box.Key;
                lensFocusingPower *= i+1;
                lensFocusingPower *= box.Value[i].value;
                AnsiConsole.MarkupLineInterpolated($"Box [green]{box.Key}[/] Lens [green]{box.Value[i].label}[/] has a focusing power of [green]{lensFocusingPower}[/] ");
                boxFocusingPower += lensFocusingPower;
            }
            total += boxFocusingPower;
        }

        AnsiConsole.MarkupLineInterpolated($"Total Focusing Power: [green]{total}[/]");

        //void PrintBoxes()
        //{
        //    foreach (var box in boxes.OrderBy(x => x.Key))
        //    {
        //        if (box.Value.Count > 0)
        //        {
        //            AnsiConsole.MarkupLineInterpolated($"Box [yellow]{box.Key}[/]: {string.Join(' ', box.Value.Select(y => "[" + y.label + " " + y.value + "]"))}");
        //        }
        //    }
        //}
    }
}