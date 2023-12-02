namespace AdventOfCode2023.Day02;
internal class Day02 : DayBase
{
    public Day02()
    {
        Ready = true;
    }


    private Dictionary<int, (int blue, int red, int green)[]> _games;

    private async Task Init(int part, bool useTestData)
    {
        _games = new Dictionary<int, (int blue, int red, int green)[]>();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        foreach (var line in lines)
        {
            var temp = line.Split(':');
            var gameNumber = Convert.ToInt32(temp[0].Replace("Game ", ""));
            var rounds = temp[1].Split(';').Select(x => x.Trim()).ToArray();

            _games[gameNumber] = new (int blue, int red, int green)[rounds.Length];

            var c = 0;
            foreach (var round in rounds)
            {
                var totals = round.Split(',').Select(x => x.Trim());
                (int blue, int red, int green) colors = (0,0,0);

                foreach (var total in totals)
                {
                    var tempColor = total.Split(' ');
                    var count = Convert.ToInt32(tempColor[0]);
                    switch (tempColor[1])
                    {
                        case "red":
                            colors.red = count;
                            break;
                        case "blue":
                            colors.blue = count;
                            break;
                        case "green":
                            colors.green = count;
                            break;
                    }
                }

                _games[gameNumber][c++] = colors;

            }

        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        int maxRed = 12;
        int maxGreen = 13;
        int maxBlue = 14;

        List<int> possibleGames = new List<int>();

        foreach (var game in _games)
        {
            bool isPossible = true;
            foreach (var round in game.Value)
            {
                if (round.blue > maxBlue || round.green > maxGreen || round.red > maxRed)
                {
                    isPossible = false;
                }
                
            }
            if (isPossible)
                possibleGames.Add(game.Key);
        }

        AnsiConsole.MarkupLineInterpolated($"Possible games were: [yellow]{string.Join(',',possibleGames)}[/] which sums to: [green]{possibleGames.Sum()}[/]");

    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        List<int> gamePower = new List<int>();
        int c = 0;
        foreach (var game in _games.OrderBy(x => x.Key))
        {
            int minRed = game.Value.Where(x => x.red > 0).Max(x => x.red);
            int minGreen = game.Value.Where(x => x.green > 0).Max(x => x.green);
            int minBlue = game.Value.Where(x => x.blue > 0).Max(x => x.blue);

            AnsiConsole.MarkupLineInterpolated($"Game [yellow]{c++}[/]: [red]{minRed}[/], [green]{minGreen}[/], [blue]{minBlue}[/]");

            gamePower.Add(minRed*minGreen*minBlue);
        }

        AnsiConsole.MarkupLineInterpolated($"Game Powers Were: [yellow]{string.Join(',', gamePower)}[/] which sums to: [green]{gamePower.Sum()}[/]");
    }
}
