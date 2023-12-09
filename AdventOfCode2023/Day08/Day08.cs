﻿using System.Collections.Frozen;

namespace AdventOfCode2023.Day08;
internal class Day08 : DayBase
{

    public Day08()
    {
        Ready = true;
    }

    private FrozenDictionary<string, (string left, string right, bool leftEndsWithZ, bool rightEndsWithZ )> _map;
    private bool[] _pattern;


    private async Task Init(int part, bool useTestData)
    {
        var tempMap = new Dictionary<string, (string left, string right, bool leftEndsWithZ, bool rightEndsWithZ)>();

        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        _pattern = lines[0].ToCharArray().Select(x => x == 'L').ToArray();

        foreach (var line in lines.Skip(2))
        {
            var temp = line.Split(" = ");
            var temp2 = temp[1].Replace("(", "").Replace(")", "").Replace(" ", "").Split(',');

            tempMap.Add(temp[0], (temp2[0], temp2[1], temp2[0].EndsWith('Z'), temp2[1].EndsWith('Z')));
        }

        _map = tempMap.ToFrozenDictionary();
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);
        int index = 0;
        string currentLocation = "AAA";
        bool nextDirection = _pattern[index];
        int stepCount = 0;

        while (currentLocation != "ZZZ")
        {
            stepCount++;
            AnsiConsole.MarkupInterpolated($"Moving from [yellow]{currentLocation}[/] to ");
            currentLocation = nextDirection ? _map[currentLocation].left : _map[currentLocation].right;
            AnsiConsole.MarkupLineInterpolated($"[yellow]{currentLocation}[/]");


            if (index == _pattern.Length -1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            nextDirection = _pattern[index];
        }

        AnsiConsole.MarkupLineInterpolated($"It took [green]{stepCount}[/] steps to reach ZZZ");

    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        Part2Sync2();
    }

    private void Part2Sync2()
    {
        string[] paths =
            _map.Where(x => x.Key.EndsWith('A'))
                .Select(x => x.Key).ToArray();
        int index = 0;

        var direction = new ReadOnlySpan<bool>(_pattern);
        int patternCount = direction.Length - 1;

        long lcm = 0;

        for (var i = 0; i < paths.Length; i++)
        {

            var atZ = false;
            long stepCount = 0;

            while (!atZ)
            {
                var lookup = _map[paths[i]];
                if (direction[index])
                {
                    paths[i] = lookup.left;
                    atZ = false;
                }
                else
                {
                    paths[i] = lookup.right;
                    atZ = lookup.rightEndsWithZ;
                }

                if (index == patternCount)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }
                stepCount++;

            }
            AnsiConsole.MarkupLineInterpolated($"It took [green]{stepCount}[/] steps to reach {paths[i]}");

            lcm = lcm == 0 
                ? stepCount 
                : LeastCommonMultiple(lcm, stepCount);
        }


        AnsiConsole.MarkupLineInterpolated($"It took [green]{lcm}[/] steps to reach the end for all races at the same time.");
    }

    // Pulled this calculation method from the internet, no, i'm not above asking how to calculate LCM easily.
    static long GreatestCommonFactor(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static long LeastCommonMultiple(long a, long b)
    {
        return (a / GreatestCommonFactor(a, b)) * b;
    }
}
