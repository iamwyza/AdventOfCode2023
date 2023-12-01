namespace AdventOfCode2023.Day01;

internal class Day01 : DayBase
{
    public Day01()
    {
        Ready = true;
        _map = new Grid<sbyte>();
    }

    private Grid<sbyte> _map;

    [MemberNotNull(nameof(_map))]
    private async Task Init()
    {
        _map = new Grid<sbyte>();

        var lines = await GetLines();

        foreach (var line in lines)
        {

        }

        _map.InitMap();

        _map.PrintMap();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init();

    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init();
    }
}
