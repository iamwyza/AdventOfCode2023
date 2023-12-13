using System;
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

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);
        int totalArrangements = 0;
        int line = 0;

        //var regexClass = new Day11Data.TestData() as DataMatch;
        var regexClass = new Day11Data.Data() as DataMatch;

        foreach (var (conditions, groups) in _records)
        {
            
            //var tstring = string.Join("", groups.Take(groups.Length-1).Select(x => "[#]{" + x + "}\\.+"));

            //var rstring = @"^\.*" + tstring + "[#]{" + groups.Last() + "}\\.*$";

            //var regex = new Regex(rstring, RegexOptions.Compiled);

            var numberOfPlaceholders = conditions.Count(x => x == '?');

            var permutations = (-1*((Int32.MaxValue  << numberOfPlaceholders) | Int32.MinValue));

            //AnsiConsole.WriteLine($"Mask :{permutations}         {Convert.ToString(permutations, toBase: 2).PadLeft(numberOfPlaceholders, '0')}");
            //AnsiConsole.WriteLine($"[GeneratedRegex(@\"{rstring}\", RegexOptions.IgnoreCase, \"en-US\")] private static partial Regex Line{line++}();");

            int totalMatches = 0;
            for (int i = 0; i < permutations; i++)
            {
                var attempt = (char[])conditions.Clone();

                var t = Convert.ToString(i, toBase: 2).PadLeft(numberOfPlaceholders, '.');
                //AnsiConsole.MarkupLineInterpolated($"{i} = {t}");

                var replacements = new Stack<char>(t.ToCharArray());


                while (replacements.Count > 0)
                {
                    var replacement = replacements.Pop();
                    var index = Array.IndexOf(attempt, '?');
                    attempt[index] = replacement == '1' ? '#' : '.';
                }

             


                if (regexClass.IsMatch(line, new string(attempt)))
                {
                    totalMatches++;
                    //AnsiConsole.MarkupLineInterpolated($"[yellow]{rstring}[/] against [red]{new string(attempt)}[/] has [green]{matches.Count}[/] matches.");

                }


            }

            totalArrangements += totalMatches;
            //AnsiConsole.MarkupLineInterpolated($"Total permutations is [green]{totalMatches}[/]");
            //Console.WriteLine($"MaskedState:   {Convert.ToString((_state & mask) | (value << offset), toBase: 2).PadLeft(64, '0')}");

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
        await Init(2, true);
        int totalArrangements = 0;
        //var regexClass = new Day11Data.TestData() as DataMatch;
        var regexClass = new Day11Data.Data() as DataMatch;

        var line = 0;
        foreach (var (c, g) in _records)
        {
            int[] groups =
                g.Concat(g)
                    .Concat(g)
                    .Concat(g)
                    .Concat(g).ToArray();

            char[] conditions = c
                .Concat(new[] { '?' }).Concat(c)
                .Concat(new[] { '?' }).Concat(c)
                .Concat(new[] { '?' }).Concat(c)
                .Concat(new[] { '?' }).Concat(c)
                .ToArray();

            //var tstring = string.Join("", groups.Take(groups.Length - 1).Select(x => "[#]{" + x + "}\\.+"));

            //var rstring = @"^\.*" + tstring + "[#]{" + groups.Last() + "}\\.*$";

            //var regex = new Regex(rstring, RegexOptions.Compiled);

            var numberOfPlaceholders = conditions.Count(x => x == '?');

            var permutations = (-1 * ((long.MaxValue << numberOfPlaceholders) | long.MinValue));
            
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

            Parallel.ForEach(Range(0L, permutations), new ParallelOptions(){MaxDegreeOfParallelism = 10}, (i) =>
            {
                char[] attempt = new char[conditions.Length];

                 for (int con = 0; con < conditions.Length; con++)
                {
                    attempt[con] = conditions[con];
                }


                //var attempt = (char[])conditions.Clone();

                var t = Convert.ToString(i, toBase: 2).PadLeft(numberOfPlaceholders, '.');

                //AnsiConsole.MarkupLineInterpolated($"{i} = {t}");

                var temp2 = 0;
                foreach (var index in replacementLocations)
                {
                    attempt[index] = t[temp2++] == '1' ? '#' : '.';
                }



                if (regexClass.IsMatch(line, new string(attempt)))
                {
                    Interlocked.Increment(ref totalMatches);
                    //AnsiConsole.MarkupLineInterpolated($"[yellow]{rstring}[/] against [red]{new string(attempt)}[/] matches.");
                }
            });

            totalArrangements += totalMatches;
            AnsiConsole.MarkupLineInterpolated($"Total permutations for [yellow]{new string(conditions)}[/] is [green]{totalMatches}[/]");
            //Console.WriteLine($"MaskedState:   {Convert.ToString((_state & mask) | (value << offset), toBase: 2).PadLeft(64, '0')}");

            line++;
        }

        AnsiConsole.MarkupLineInterpolated($"There was a total of [green]{totalArrangements}[/] combinations");
    }

    public static IEnumerable<long> Range(long fromInclusive, long toExclusive)
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
