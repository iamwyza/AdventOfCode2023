﻿using AdventOfCode2023.GridUtilities;

namespace AdventOfCode2023.Day22;
internal class Day22 : DayBase
{
    private Grid<sbyte> _map;

    [MemberNotNull(nameof(_map))]
    private async Task Init(int part, bool useTestData)
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
        await Init(1, true);

    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, true);
    }
}