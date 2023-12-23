namespace AdventOfCode2023.Day01;

internal class Day01 : DayBase
{
    public Day01()
    {
        Ready = true;
    }

    private List<int> numbers = new();

    private async Task Init(int part, bool useTestData)
    {
        numbers.Clear();
        await Task.CompletedTask;
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, true);
        var lines = await GetLines();
        Regex regex = new Regex(@"(\d)");
        foreach (var line in lines)
        {
            var temp = line;

            var matches = regex.Matches(temp);


            int firstValue = Convert.ToInt32(matches.First().Captures.First().Value);
            int lastValue = Convert.ToInt32(matches.Last().Captures.Last().Value);

            AnsiConsole.MarkupLineInterpolated($"{line} == [green]{firstValue}[/][red]{lastValue}[/]");

            if (matches.Count == 1)
            {
                numbers.Add(Convert.ToInt32($"{firstValue}{firstValue}"));
            }
            else
            {
                numbers.Add(Convert.ToInt32($"{firstValue}{lastValue}"));
            }
        }

        AnsiConsole.MarkupLineInterpolated($"The Answer is: [green]{numbers.Sum()}[/]");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(1, true);

        var lines = await GetLines();
        Regex reverseRegex = new Regex(@"(\d|eno|owt|eerht|ruof|evif|xis|neves|thgie|enin)");
        Regex regex = new Regex(@"(\d|one|two|three|four|five|six|seven|eight|nine)");
        foreach (var line in lines)
        {

            var matches = regex.Matches(line);
            var reverseMatches = reverseRegex.Matches(new string(line.Reverse().ToArray()));


            int firstValue = ParseValue(matches.First().Captures.First().Value);
            int lastValue = ParseValue(new string(reverseMatches.First().Captures.First().Value.Reverse().ToArray()));

            AnsiConsole.MarkupLineInterpolated($"{line} == [green]{firstValue}[/][red]{lastValue}[/]");

            if (matches.Count == 1)
            {
                numbers.Add(Convert.ToInt32($"{firstValue}{firstValue}"));
            }
            else
            {
                numbers.Add(Convert.ToInt32($"{firstValue}{lastValue}"));
            }
        }

        int ParseValue(string value)
        {
            return Convert.ToInt32(value.Replace("eight", "8")
                .Replace("seven", "7")
                .Replace("three", "3")
                .Replace("four", "4")
                .Replace("five", "5")
                .Replace("nine", "9")
                .Replace("one", "1")
                .Replace("two", "2")
                .Replace("six", "6")
                );
        }

        AnsiConsole.MarkupLineInterpolated($"The Answer is: [green]{numbers.Sum()}[/]");
    }
}
