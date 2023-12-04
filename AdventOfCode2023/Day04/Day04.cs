namespace AdventOfCode2023.Day04;
internal class Day04 : DayBase
{
    public Day04()
    {
        Ready = true;
    }

    public List<(int card, int[] winningNumbers, int[] yourNumbers)> _cards;

    private async Task Init(int part, bool useTestData)
    {
        _cards = new();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        // Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
        foreach (var line in lines)
        {
            var temp0 = line.Replace("  ", " ").Replace("  ", " ");
            var temp1 = temp0.Split(':'); // Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
            var temp2 = temp1[0].Split(' '); // Card 1
            int card = Convert.ToInt32(temp2[1]);

            var temp3 = temp1[1].Split('|'); // 41 48 83 86 17 | 83 86  6 31 17  9 48 53

            int[] winningNumbers = temp3[0].Trim().Split(' ').Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => Convert.ToInt32(x)).ToArray();

            int[] yourNumbers = temp3[1].Trim().Split(' ').Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => Convert.ToInt32(x)).ToArray();

            _cards.Add((card, winningNumbers, yourNumbers));
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        double total = 0;

        foreach (var card in _cards)
        {
            var matches = card.winningNumbers.Where(x => card.yourNumbers.Contains(x));
            double cardWorth = 0;

            if (matches.Any())
            {
                cardWorth = Math.Pow(2, matches.Count() - 1);
                total += cardWorth;
            }
            

            AnsiConsole.MarkupLineInterpolated($"Card [red]{card.card}[/] was worth [yellow]{cardWorth}[/] points.");
        }

        AnsiConsole.MarkupLineInterpolated($"Your total winnings are [green]{total}[/]");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        var bonusCopies = new int[_cards.Count];
        foreach (var card in _cards)
        {
            var matches = card.winningNumbers.Where(x => card.yourNumbers.Contains(x)).ToArray();

            if (matches.Any())
            {
                
                for (int i = 0; i <= bonusCopies[card.card-1]; i++)
                {
                    

                    for (int j = 0; j < matches.Count(); j++)
                    {
                        if (card.card + j > bonusCopies.Length -1)
                        {
                            continue;
                        }
                        bonusCopies[card.card + j]++;
                    }
                }

            }


            AnsiConsole.MarkupLineInterpolated($"Card [red]{card.card}[/] had [yellow]{matches.Count()}[/] matches and you had {bonusCopies[card.card - 1] + 1} scratchcards.");
        }

        AnsiConsole.MarkupLineInterpolated($"Your scratchcards are [green]{bonusCopies.Sum() + _cards.Count}[/]");
    }
}
