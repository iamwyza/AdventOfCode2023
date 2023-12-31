﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace AdventOfCode2023.Day12;
internal class Day12 : DayBase
{

    public Day12()
    {
        Ready = true;
    }

    private readonly List<(char[], int[])> _records = new();

    private async Task Init(int part, bool useTestData)
    {
        _records.Clear();
        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        foreach (var line in lines)
        {
            var temp = line.Split(' ');
            var groups = temp[1].Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            _records.Add((temp[0].ToCharArray(), groups));
        }


    }

    //public override async Task RunPart1()
    //{
    //    PrintStart(1);
    //    await Init(1, true);
    //    int totalArrangements = 0;
    //    int line = 0;

    //    //var regexClass = new Day11Data.TestData() as DataMatch;
    //    //var regexClass = new Day11Data.Data() as DataMatch;
        
    //    foreach (var (conditions, groups) in _records)
    //    {

    //        //var tstring = string.Join("", groups.Take(groups.Length-1).Select(x => "[#]{" + x + "}\\.+"));

    //        //var rstring = @"^\.*" + tstring + "[#]{" + groups.Last() + "}\\.*$";

    //        //var regex = new Regex(rstring, RegexOptions.Compiled);

    //        var numberOfPlaceholders = conditions.Count(x => x == '?');

    //        var permutations = (-1 * ((Int32.MaxValue << numberOfPlaceholders) | Int32.MinValue));

    //        //AnsiConsole.WriteLine($"Mask :{permutations}         {Convert.ToString(permutations, toBase: 2).PadLeft(numberOfPlaceholders, '0')}");
    //        //AnsiConsole.WriteLine($"[GeneratedRegex(@\"{rstring}\", RegexOptions.IgnoreCase, \"en-US\")] private static partial Regex Line{line++}();");

    //        int totalMatches = 0;
    //        for (int i = 0; i < permutations; i++)
    //        {
    //            var attempt = (char[])conditions.Clone();

    //            var t = Convert.ToString(i, toBase: 2).PadLeft(numberOfPlaceholders, '.');
    //            //AnsiConsole.MarkupLineInterpolated($"{i} = {t}");

    //            var replacements = new Stack<char>(t.ToCharArray());


    //            while (replacements.Count > 0)
    //            {
    //                var replacement = replacements.Pop();
    //                var index = Array.IndexOf(attempt, '?');
    //                attempt[index] = replacement == '1' ? '#' : '.';
    //            }




    //            //if (regexClass.IsMatch(line, new string(attempt)))
    //            //{
    //            //    totalMatches++;
    //            //    //AnsiConsole.MarkupLineInterpolated($"[yellow]{rstring}[/] against [red]{new string(attempt)}[/] has [green]{matches.Count}[/] matches.");

    //            //}


    //        }

    //        totalArrangements += totalMatches;
    //        //AnsiConsole.MarkupLineInterpolated($"Total permutations is [green]{totalMatches}[/]");
    //        //Console.WriteLine($"MaskedState:   {Convert.ToString((_state & mask) | (value << offset), toBase: 2).PadLeft(64, '0')}");

    //        line++;
    //    }

    //    //for (int i = 0; i < _records.Count; i++)
    //    //{
    //    //    AnsiConsole.WriteLine($"{i} => Line{i}().IsMatch(value),");
    //    //}

    //    AnsiConsole.MarkupLineInterpolated($"There was a total of [green]{totalArrangements}[/] combinations");
    //}

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, true);
        int totalArrangements = 0;
        int line = 0;

        //var regexClass = new Day11Data.TestData() as DataMatch;
        //var regexClass = new Day11Data.Data() as DataMatch;

        foreach (var (conditions, groups) in _records)
        {
            var totalMatches = GetCombinations(conditions, groups);

            totalArrangements += totalMatches;
            AnsiConsole.MarkupLineInterpolated($"Total permutations is [green]{totalMatches}[/]");

            line++;
        }

        //for (int i = 0; i < _records.Count; i++)
        //{
        //    AnsiConsole.WriteLine($"{i} => Line{i}().IsMatch(value),");
        //}

        AnsiConsole.MarkupLineInterpolated($"There was a total of [green]{totalArrangements}[/] combinations");
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);
        int totalArrangements = 0;
        //var regexClass = new Day11Data.TestData() as DataMatch;
        //var regexClass = new Day11Data.Data() as DataMatch;
        var line = 0;
        foreach (var (c, g) in _records)
        {
            //if (line == 1) break;

            int[] groups = g
                .Concat(g)
                    .Concat(g)
                    .Concat(g)
                    .Concat(g)
                    .ToArray();

            char[] conditions = c
                .Concat(new[] { '?' }).Concat(c)
                .Concat(new[] { '?' }).Concat(c)
                .Concat(new[] { '?' }).Concat(c)
                .Concat(new[] { '?' }).Concat(c)
                .ToArray();

            //var tstring = string.Join("", groups.Take(groups.Length - 1).Select(x => "[#]{" + x + "}\\.+"));

            //var rstring = @"^\.*" + tstring + "[#]{" + groups.Last() + "}\\.*$";

            //var regex = new Regex(rstring, RegexOptions.Compiled);

            var conditionsBeforeCollapsing = new string(conditions);
            while (conditionsBeforeCollapsing.Contains(".."))
                conditionsBeforeCollapsing = conditionsBeforeCollapsing.Replace("..", ".");

            conditions = conditionsBeforeCollapsing.ToCharArray();

            int numberOfPlaceholders = conditions.Count(x => x == '?');

            var z = -1 * ((long.MaxValue << numberOfPlaceholders) | long.MinValue);
            ulong permutations = Convert.ToUInt64(z);

            //AnsiConsole.WriteLine($"Mask :{permutations}         {Convert.ToString(permutations, toBase: 2).PadLeft(numberOfPlaceholders, '0')}");
            //AnsiConsole.MarkupLineInterpolated($"[yellow]{new string(conditions)}[/]");
            int totalMatches = 0;
            int[] replacementLocations = new int[numberOfPlaceholders];
            int temp = 0;

            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i] == '?')
                {
                    replacementLocations[temp++] = i;
                }
            }

            var maxAdditionalItems = groups.Sum() - conditions.Count(x => x == '#');

            AnsiConsole.MarkupLineInterpolated($"Attempting [yellow]{new string(conditions)}[/] with groups [yellow]{string.Join(',', groups)}[/] possible solutions [green]{permutations}[/].  A total of [red]{maxAdditionalItems}[/] extra # can be added.");
            var stopwatch = Stopwatch.StartNew();

            Parallel.ForEach(Range(0L, permutations), new ParallelOptions() { MaxDegreeOfParallelism = 10 }, (i) =>
            {
                if (BitOperations.PopCount(i) != maxAdditionalItems)
                    return;

                char[] attempt = new char[conditions.Length];

                for (int con = 0; con < conditions.Length; con++)
                {
                    attempt[con] = conditions[con];
                }


                //var attempt = (char[])conditions.Clone();
                //var y = (long)i;
                // var t = Convert.ToString(y, toBase: 2).PadLeft(numberOfPlaceholders, '.');

                //AnsiConsole.MarkupLineInterpolated($"{i} = {t}");

                var temp2 = 1;
                //Console.WriteLine(attempt);
                foreach (var index in replacementLocations)
                {
                    //Console.WriteLine(Convert.ToString(y,2));
                    var bit = ((long)i & (1 << temp2++ - 1)) != 0;
                    //Console.WriteLine(Convert.ToString((y & (1 << temp2 - 1)),2).PadLeft(numberOfPlaceholders, '0'));
                    attempt[index] = bit ? '#' : '.';
                }
                //Console.WriteLine(attempt);


                //if (regexClass.IsMatch(line, new string(attempt)))
                //{
                //    Interlocked.Increment(ref totalMatches);
                //    //AnsiConsole.MarkupLineInterpolated($"[yellow]{rstring}[/] against [red]{new string(attempt)}[/] matches.");
                //}
            });
            stopwatch.Stop();

            totalArrangements += totalMatches;
            AnsiConsole.MarkupLineInterpolated($"Total permutations for [yellow]{new string(conditions)}[/] is [green]{totalMatches}[/].  Took [red]{stopwatch.ElapsedMilliseconds / 1000F:N1}[/] seconds at a rate of {permutations / (stopwatch.ElapsedMilliseconds / 1000F):N1} per second.");
            //Console.WriteLine($"MaskedState:   {Convert.ToString((_state & mask) | (value << offset), toBase: 2).PadLeft(64, '0')}");

            line++;
        }

        AnsiConsole.MarkupLineInterpolated($"There was a total of [green]{totalArrangements}[/] combinations");




    }

    private static int GetCombinations(char[] input, int[] groups)
    {
        Queue<(char[],int, int)> queue = new Queue<(char[], int, int)>();
        // TODO: copy and mutate each branch as we go, discarding branches that aren't possible. should drastically lower the possible combinations....
        queue.Enqueue((input,0, 0));
        //var tstring = string.Join("", groups.Take(groups.Length-1).Select(x => "[#?]{" + x + "}\\.+"));

        //var rstring = @"^\.*" + tstring + "[#?]{" + groups.Last() + "}\\.*$";

        //var regex = new Regex(rstring, RegexOptions.Compiled);

        int totalPermutations = 0;

        while (queue.Any())
        {
            var path = queue.Dequeue();
            Branch(path.Item1, startOffset: path.Item2, groupOffset: path.Item3);

        }

        return totalPermutations;

        void Branch(char[] toVerify, int startOffset, int groupOffset)
        {
            AnsiConsole.MarkupLineInterpolated($"Looking at {new string(toVerify)} -> {groupOffset} -> startoffset {startOffset}");
            if (groupOffset >= groups.Length)
            {
                if (!toVerify.Contains('?'))
                {
                    totalPermutations++;
                }
                return;
            }

            var groupSize = groups[groupOffset];

            for (int i = startOffset; i <= toVerify.Length; i++)
            {
                AnsiConsole.MarkupInterpolated($"[yellow]{toVerify[i]}[/] -> ");
                if (toVerify[i] == '?' || toVerify[i] == '#')
                {
                    for (int g = 0; g <= groupSize; g++)
                    {
                        AnsiConsole.MarkupInterpolated($"[yellow]{toVerify[i + g]}[/] -> ");
                        if (toVerify[i + g] != '?' && toVerify[i+g] != '#')
                            return;
                    }

                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLineInterpolated($"{i + groupSize} > {toVerify.Length}");
                    if (i + groupSize > toVerify.Length) return;

                    var temp = Array.IndexOf(toVerify, '?');
                    char[] leftCopy;
                    char[] rightCopy;
                    int offset = i;
                    if (temp <= i + groupSize)
                    {
                        offset += groupSize;
                        leftCopy = ((char[])toVerify.Clone()).ToArray();
                        rightCopy = ((char[])toVerify.Clone()).ToArray();
                    }
                    else
                    {
                        leftCopy = ((char[])toVerify.Clone()).ToArray();
                        rightCopy = ((char[])toVerify.Clone()).ToArray();
                    }

                    if (temp >= 0)
                    {
                        leftCopy[temp] = '.';
                        rightCopy[temp] = '#';
                    }
                    

                    queue.Enqueue((leftCopy, offset, groupOffset+1));
                    queue.Enqueue((rightCopy, offset, groupOffset + 1));

                    return;
                }
            }
        }
    }


    public static IEnumerable<ulong> Range(ulong fromInclusive, ulong toExclusive)
    {
        for (var i = fromInclusive; i < toExclusive; i++) yield return i;
    }


    //public override async Task RunPart2()
    //{
    //    PrintStart(2);
    //    await Init(2, true);

    //    int totalArrangements = 0;

    //    foreach (var (c, groups) in _records)
    //    {

    //        int[] multipleGroups =
    //            groups
    //                .Concat(groups)
    //                .Concat(groups)
    //                .Concat(groups)
    //                .Concat(groups).ToArray();

    //        char[] conditions = c
    //            .Concat(new[] { '?' }).Concat(c)
    //            .Concat(new[] { '?' }).Concat(c)
    //            .Concat(new[] { '?' }).Concat(c)
    //            .Concat(new[] { '?' }).Concat(c)
    //            .ToArray();

    //        var tstring = string.Join("", multipleGroups.Take(multipleGroups.Length - 1).Select(x => "[#]{" + x + "}\\.+"));

    //        var rstring = @"^\.*" + tstring + "[#]{" + multipleGroups.Last() + "}\\.*$";
    //        //AnsiConsole.MarkupLineInterpolated($"[yellow]{string.Join(',',multipleGroups)}[/] ");
    //        //AnsiConsole.MarkupLineInterpolated($"[yellow]{rstring}[/] ");

    //        var regex = new Regex(rstring, RegexOptions.Compiled);

    //        var numberOfPlaceholders = conditions.Count(x => x == '?');

    //        var permutations = (-1 * ((Int128.MaxValue << numberOfPlaceholders) | Int128.MinValue));
    //        Int128 mask = 0;

    //        for (var i = conditions.Length-1; i >= 0; i--)
    //        {
    //            Int128 m = ((Int128.MaxValue) ^ (127 << i));
    //            //AnsiConsole.WriteLine($"m :         {Convert.ToString(mask, toBase: 2)}");
    //            //AnsiConsole.WriteLine($"i :         {Convert.ToString((1L << i), toBase: 2)}");
    //            if (conditions[i] == '?')
    //            {
    //                mask |= (mask & m) | (Int128.One << conditions.Length-i-1);
    //            }
    //            else
    //            {
    //                mask <<= 0;
    //            }
    //        }

    //        AnsiConsole.MarkupLineInterpolated($"[yellow]{new string(conditions)}[/]");
    //        //AnsiConsole.WriteLine($"{Convert.ToString(mask, toBase: 2).PadLeft(conditions.Length, '0')}");
    //        AnsiConsole.WriteLine(mask.ToString("B").PadLeft(conditions.Length, '0'));
    //        AnsiConsole.WriteLine(permutations.ToString("B").PadLeft(conditions.Length, '0'));

    //        int totalMatches = 0;
    //        for (Int128 i = 0; i < permutations; i++)
    //        {
    //            //var attempt = (char[])conditions.Clone();


    //            for (int d = conditions.Length - 1; d >= 0; d--)
    //            {
    //                var z = mask >> d;
    //            }
    //            while (replacements.Count > 0)
    //            {
    //                var replacement = replacements.Pop();
    //                var index = Array.IndexOf(attempt, '?');
    //                attempt[index] = replacement == '1' ? '#' : '.';
    //            }



    //            var matches = regex.Matches(new string(attempt));

    //            if (matches.Count > 0)
    //            {
    //                totalMatches++;
    //                //AnsiConsole.MarkupLineInterpolated($"[yellow]{rstring}[/] against [red]{new string(attempt)}[/] has [green]{matches.Count}[/] matches.");

    //            }


    //        }

    //        totalArrangements += totalMatches;
    //        AnsiConsole.MarkupLineInterpolated($"Total permutations is [green]{totalMatches}[/]");
    //        //Console.WriteLine($"MaskedState:   {Convert.ToString((_state & mask) | (value << offset), toBase: 2).PadLeft(64, '0')}");


    //    }

    //    AnsiConsole.MarkupLineInterpolated($"There was a total of [green]{totalArrangements}[/] combinations");
    //}
}
