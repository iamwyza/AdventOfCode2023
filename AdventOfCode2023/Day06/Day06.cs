namespace AdventOfCode2023.Day06;
internal class Day06 : DayBase
{
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
