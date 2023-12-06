namespace AdventOfCode2023.Day06;
internal class Day06 : DayBase
{

    public Day06()
    {
        Ready = true;
    }

    private List<(int time, int distance)> _races;

    private async Task Init(int part, bool useTestData)
    {
        _races = new List<(int time, int distance)>();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        var times = lines[0].Split(':')[1].Split( " ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => Convert.ToInt32(x)).ToArray();
        var distances = lines[1].Split(':')[1].Split( " ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => Convert.ToInt32(x)).ToArray();
        for (var index = 0; index < times.Length; index++)
        {
            _races.Add((times[index], distances[index]));
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);
        List<int> possibleWinningTimes = new();

        foreach (var race in _races)
        {
            int numberOfWaysToWin = 0;

            for (var startTime = 1; startTime < race.time; startTime++)
            {
                if ((race.time - startTime) * startTime > race.distance)
                {
                    numberOfWaysToWin++; 
                }
            }

            possibleWinningTimes.Add(numberOfWaysToWin);
        }

        int marginOfError = 1;

        foreach (var time in possibleWinningTimes)
        {
            marginOfError *= time;
        }

        AnsiConsole.MarkupLineInterpolated($"The Margin of error is {marginOfError}");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        var raceTime = Convert.ToInt64(string.Join("", _races.Select(x => x.time)));
        var raceDistance = Convert.ToInt64(string.Join("", _races.Select(x => x.distance)));

        long numberOfWaysToWin = 0;

        for (var startTime = 1; startTime < raceTime; startTime++)
        {
            if ((raceTime - startTime) * startTime > raceDistance)
            {
                numberOfWaysToWin++;
            }
        }
        

        AnsiConsole.MarkupLineInterpolated($"The Number of ways to win is {numberOfWaysToWin}");
    }
}
