namespace AdventOfCode2023.Day07;
internal class Day07 : DayBase
{

    public Day07()
    {
        Ready = true;
        _hands = new List<(char[] cards, int bid)>();
    }

    private enum HandType
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    private readonly Dictionary<char, byte> _cardStrength = new()
    {
        { 'A', 14 },
        { 'K', 13 },
        { 'Q', 12 },
        { 'T', 10 },
        { '9', 9 },
        { '8', 8 },
        { '7', 7 },
        { '6', 6 },
        { '5', 5 },
        { '4', 4 },
        { '3', 3 },
        { '2', 2 },
        { 'J', 1 },

    };

    private List<(char[] cards, int bid)> _hands;

    private async Task Init(int part, bool useTestData)
    {
        _hands = new List<(char[] hand, int bid)>();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        foreach (var line in lines)
        {
            var temp = line.Split(' ');

            var hand = temp[0];
            var bid = Convert.ToInt32(temp[1]);

            _hands.Add((hand.ToCharArray(), bid));
        }

    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        List<(HandType type, char[] cards, int bid)> typedHands = new();

        foreach (var hand in _hands)
        {
            var groups = hand.cards.GroupBy(x => x).ToArray();
            switch (groups.Count())
            {
                case 1:
                    typedHands.Add((HandType.FiveOfAKind, hand.cards, hand.bid));
                    break;
                case 2:
                    if (groups[0].Count() == 4 || groups[0].Count() == 1)
                        typedHands.Add((HandType.FourOfAKind, hand.cards, hand.bid));
                    else
                        typedHands.Add((HandType.FullHouse, hand.cards, hand.bid));
                    break;
                case 3:
                    if (groups.Any(x => x.Count() == 3))
                        typedHands.Add((HandType.ThreeOfAKind, hand.cards, hand.bid));
                    else 
                        typedHands.Add((HandType.TwoPair, hand.cards, hand.bid));
                    break;
                case 4:
                    typedHands.Add((HandType.OnePair, hand.cards, hand.bid));
                    break;
                case 5:
                    typedHands.Add((HandType.HighCard, hand.cards, hand.bid));
                    break;
            }
        }

        List<(HandType type, char[] cards, int bid)> rankedHands = new List<(HandType type, char[] cards, int bid)>();

        foreach (var group in typedHands.OrderByDescending(x => x.type).GroupBy(x => x.type))
        {
            rankedHands.AddRange(group
                .OrderByDescending(x => _cardStrength[x.cards[0]])
                .ThenByDescending(x => _cardStrength[x.cards[1]])
                .ThenByDescending(x => _cardStrength[x.cards[2]])
                .ThenByDescending(x => _cardStrength[x.cards[3]])
                .ThenByDescending(x => _cardStrength[x.cards[4]])
            );
        }

        var total = 0;

        rankedHands.Reverse();

        for (var i = 0; i < rankedHands.Count; i++)
        {
            var rank = i + 1;
            var (handType, cards, bid) = rankedHands[i];
            total += bid * rank;
            AnsiConsole.MarkupLineInterpolated($"Rank [red]{rank}[/]: [yellow]{new string(cards)}[/] had a bid of [yellow]{bid}[/] with a value of [green]{bid * rank}[/]");
        }

        AnsiConsole.MarkupLineInterpolated($"Total winnings: [green]{total}[/] ");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        List<(HandType type, char[] cards, int bid)> typedHands = new();

        foreach (var hand in _hands)
        {
            var groups = hand.cards.GroupBy(x => x).ToArray();
            switch (groups.Count())
            {
                case 1:
                    typedHands.Add((HandType.FiveOfAKind, hand.cards, hand.bid));
                    break;
                case 2:
                    if (groups[0].Count() == 4 || groups[0].Count() == 1)
                    {
                        if (groups[0].Key == 'J' || groups[1].Key == 'J') // AAAAJ = AAAAA 
                            typedHands.Add((HandType.FiveOfAKind, hand.cards, hand.bid));
                        else 
                            typedHands.Add((HandType.FourOfAKind, hand.cards, hand.bid)); // AAAAK
                    }
                    else
                    {
                        if (groups[0].Key == 'J' || groups[1].Key == 'J')
                            typedHands.Add((HandType.FiveOfAKind, hand.cards, hand.bid)); // AAAJJ = AAAAA
                        else
                            typedHands.Add((HandType.FullHouse, hand.cards, hand.bid)); // AAAKK
                    }
                    break;
                case 3:
                    if (groups.Any(x => x.Count() == 3)) // AAA09
                    {
                        if (groups.Any(x => x.Key == 'J')) //AAAJ9 or JJJA9 or AAA9J
                            typedHands.Add((HandType.FourOfAKind, hand.cards, hand.bid)); // AAAJ9 = AAAA9 or JJJA9 = AAAA9
                        else
                            typedHands.Add((HandType.ThreeOfAKind, hand.cards, hand.bid)); // AAAJ9
                    }
                    else
                    {
                        if (groups.Any(x => x.Key == 'J')) // AAKK9 or AAJJ9 or AAKKJ
                        {
                            if (groups.Single(x => x.Key == 'J').Count() == 1) 
                                typedHands.Add((HandType.FullHouse, hand.cards, hand.bid)); // AAKKJ = AAAKK
                            else
                                typedHands.Add((HandType.FourOfAKind, hand.cards, hand.bid)); // AAAAK
                        }
                        else
                            typedHands.Add((HandType.TwoPair, hand.cards, hand.bid));
                    }
                    break;
                case 4:
                    if (groups.Any(x => x.Key == 'J')) //AAQJ9 or JJAQ9
                        typedHands.Add((HandType.ThreeOfAKind, hand.cards, hand.bid)); //AAQJ9 = AAQA9
                    else
                        typedHands.Add((HandType.OnePair, hand.cards, hand.bid));

                    break;
                case 5:
                    if (groups.Any(x => x.Key == 'J')) //AAQJ9 or JJAQ9
                        typedHands.Add((HandType.OnePair, hand.cards, hand.bid));
                    else
                        typedHands.Add((HandType.HighCard, hand.cards, hand.bid));
                    break;
            }
        }

        List<(HandType type, char[] cards, int bid)> rankedHands = new List<(HandType type, char[] cards, int bid)>();

        foreach (var group in typedHands.OrderByDescending(x => x.type).GroupBy(x => x.type))
        {
            rankedHands.AddRange(group
                .OrderByDescending(x => _cardStrength[x.cards[0]])
                .ThenByDescending(x => _cardStrength[x.cards[1]])
                .ThenByDescending(x => _cardStrength[x.cards[2]])
                .ThenByDescending(x => _cardStrength[x.cards[3]])
                .ThenByDescending(x => _cardStrength[x.cards[4]])
            );
        }

        var total = 0;

        rankedHands.Reverse();

        for (var i = 0; i < rankedHands.Count; i++)
        {
            var rank = i + 1;
            var (handType, cards, bid) = rankedHands[i];
            total += bid * rank;
            AnsiConsole.MarkupLineInterpolated($"Rank [red]{rank}[/]: [yellow]{new string(cards)}[/] which is a [green]{handType}[/] had a bid of [yellow]{bid}[/] with a value of [green]{bid * rank}[/]");
        }

        AnsiConsole.MarkupLineInterpolated($"Total winnings: [green]{total}[/] ");
    }
}
