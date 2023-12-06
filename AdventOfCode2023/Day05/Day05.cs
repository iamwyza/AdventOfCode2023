using System.Xml.XPath;

namespace AdventOfCode2023.Day05;
internal class Day05 : DayBase
{
    private class MapItem
    {
        public long FromStart;
        public long FromEnd;
        public long ToStart;
        public long ToEnd;
        public long Size;
    }

    public Day05()
    {
        Ready = true;
    }

    private List<(long start, long size)> _seeds;
    private Dictionary<string, List<MapItem>> _lookup;
    private Dictionary<string, string> _steps;

    private async Task Init(int part, bool useTestData)
    {
        _seeds = new();
        _lookup = new();
        _steps = new();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        Regex regex = new Regex(@"(\w+)-to-(\w+) map:");
        string currentFrom = string.Empty;
        string currentTo = string.Empty; ;
        foreach (var line in lines)
        {

            if (line.StartsWith("seeds:"))
            {
                var temp = line.Split(':');
                var tempseeds = temp[1].Trim().Split(' ').Select(x => Convert.ToInt64(x)).ToArray();
                for (int i = 0; i < tempseeds.Length; i++)
                {
                    if (part == 1)
                    {
                        _seeds.Add((tempseeds[i], 0));
                    }
                    else
                    {
                        if (i + 1 <= tempseeds.Length)
                        {
                            _seeds.Add((tempseeds[i], tempseeds[i+1]));
                            i++;
                        }
                    }
                }
            }else if (regex.IsMatch(line))
            {
                var matches = regex.Matches(line);
                currentFrom = matches[0].Groups[1].Value;
                currentTo = matches[0].Groups[2].Value;
                _lookup.Add(currentTo, new List<MapItem>());
                _steps.Add(currentFrom, currentTo);

            }else if (!string.IsNullOrWhiteSpace(line))
            {
                var temp = line.Split(' ').Select(x => Convert.ToInt64(x)).ToArray();
                _lookup[currentTo].Add(new MapItem()
                {
                     ToStart = temp[0],
                     ToEnd = temp[0] + temp[2],
                    FromStart = temp[1],
                    FromEnd = temp[1] + temp[2],
                    Size = temp[2]
                });
            }
        }
        _steps.Add(currentTo, string.Empty);

    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);


        long lowestLocation = long.MaxValue;
        foreach (var seed in _seeds.Select(x => x.start))
        {
            string nextKey = "soil";
            var nextId = seed;
            AnsiConsole.MarkupInterpolated($"seed [yellow]{nextId,10}[/], ");

            while (_steps.ContainsKey(nextKey))
            {
                AnsiConsole.MarkupInterpolated($"{nextKey} ");
                nextId = NextItem(nextId, nextKey);
                AnsiConsole.MarkupInterpolated($"[yellow]{nextId,10}[/], ");

                nextKey = _steps[nextKey];

            }

            AnsiConsole.WriteLine();
            if (nextId < lowestLocation)
            {
                lowestLocation = nextId;
            }

            //break;
        }

        AnsiConsole.MarkupLineInterpolated($"Lowest Location is [yellow]{lowestLocation}[/]");


        long NextItem(long id, string key)
        {
            var map = _lookup[key].SingleOrDefault(x => id >= x.FromStart && id < x.FromEnd);
            if (map == null)
            {
                return id;
            }
            //AnsiConsole.MarkupLineInterpolated($"[green]{map.ToStart} {map.FromStart} {map.Size}[/]");
            //AnsiConsole.MarkupLineInterpolated($"([red]{id}[/] - [red]{map.FromStart}[/]) + [red]{map.ToStart}[/] ");
            //AnsiConsole.MarkupLineInterpolated($"([red]{id - map.FromStart}[/]) + [red]{map.ToStart}[/] ");
            return (id - map.FromStart) + map.ToStart ;
        }
    }

    // This is so bad.  takes 20 minutes on a 7600X.  But i'm too tired to think about how to do it right. 
    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);
        long actualLowestLocation = long.MaxValue;
        List<long> lowestLocations = new List<long>();

        Parallel.ForEach(_seeds, new ParallelOptions(){ MaxDegreeOfParallelism = 5}, seed =>
        {
            long lowestLocation = long.MaxValue;
            AnsiConsole.MarkupLineInterpolated($"Starting at [yellow]{seed.start}[/] for [yellow]{seed.size}[/]");
            for (long i = seed.start; i < seed.start + seed.size; i++)
            {
                string nextKey = "soil";
                var nextId = i;
                // AnsiConsole.MarkupInterpolated($"seed [yellow]{nextId,10}[/], ");

                while (_steps.ContainsKey(nextKey))
                {
                    //AnsiConsole.MarkupInterpolated($"{nextKey} ");
                    nextId = NextItem(nextId, nextKey);
                    //AnsiConsole.MarkupInterpolated($"[yellow]{nextId,10}[/], ");

                    nextKey = _steps[nextKey];

                }

                //AnsiConsole.WriteLine();
                if (nextId < lowestLocation)
                {
                    lowestLocation = nextId;
                }
            }

            lowestLocations.Add(lowestLocation);

            //break;
        });

        actualLowestLocation = lowestLocations.Min();

        AnsiConsole.MarkupLineInterpolated($"Lowest Location is [yellow]{actualLowestLocation}[/]");


        long NextItem(long id, string key)
        {
            var map = _lookup[key].SingleOrDefault(x => id >= x.FromStart && id < x.FromEnd);
            if (map == null)
            {
                return id;
            }
            //AnsiConsole.MarkupLineInterpolated($"[green]{map.ToStart} {map.FromStart} {map.Size}[/]");
            //AnsiConsole.MarkupLineInterpolated($"([red]{id}[/] - [red]{map.FromStart}[/]) + [red]{map.ToStart}[/] ");
            //AnsiConsole.MarkupLineInterpolated($"([red]{id - map.FromStart}[/]) + [red]{map.ToStart}[/] ");
            return (id - map.FromStart) + map.ToStart;
        }
    }
}
