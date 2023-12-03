DayBase.ScanForSolutions();

var choice = "";

string? last = null;


while (choice != "0")
{
    var prompt = new SelectionPrompt<string>()
        .Title(
            "Enter Day and Part to run.  Example: Day 1 - Part 1 would be 1-1.  Day 3 - Part 2 would be 3-2.  Enter 0 for exit;?")
        .PageSize(4)
        .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]");
    if (!string.IsNullOrEmpty(last))
    {
        prompt.AddChoice(last);
    }
    prompt.AddChoices("0")
    .AddChoices(DayBase.Days.Keys.OrderByDescending(x => x)
        .SelectMany(x => new[] { x + "-" + 1, x + "-" + 2 }).ToArray());


    choice = AnsiConsole.Prompt(
        prompt
    );

    if (choice == "0")
        break;

    AnsiConsole.Clear();

    var input = choice.Split('-', 2).Select(int.Parse).ToArray();

    if (input[1] == 1)
    {
        try
        {
            await DayBase.Days[input[0]].RunPart1().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }
    else
    {
        try
        {
            await DayBase.Days[input[0]].RunPart2().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    Console.WriteLine();

    last = choice;
}