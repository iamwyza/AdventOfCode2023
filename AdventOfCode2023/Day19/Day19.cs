using System.Xml.XPath;
using AdventOfCode2023.GridUtilities;

namespace AdventOfCode2023.Day19;
internal class Day19 : DayBase
{

    public Day19()
    {
        Ready = true;
    }

    private class Part
    {
        public int X;
        public int M;
        public int A;
        public int S;
    }

    private List<Part> _parts = new List<Part>();

    public Dictionary<string, (char propertyName, bool gt, int value, string result)[]> _workflows = new();

    private async Task Init(int part, bool useTestData)
    {
        var lines = useTestData ? await GetTestLines(part) : await GetLines();
        _parts.Clear();
        _workflows.Clear();

        bool workingOnWorkflows = true;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                workingOnWorkflows = false;
                continue;
            }

            if (workingOnWorkflows)
            {
                var temp = line.Split('{');
                var workFlowId = temp[0];
                var tempActions = temp[1][0..^1].Split(',');
                (char, bool, int, string)[] actions = new (char, bool, int, string)[tempActions.Length];
                for (var index = 0; index < tempActions.Length - 1; index++)
                {
                    var t = tempActions[index];
                    var temp3 = t[2..t.Length].Split(':');
                    actions[index] = (t[0], t[1] == '>', Convert.ToInt32(temp3[0]), temp3[1]);
                }

                actions[tempActions.Length - 1] = ('F', true, 0, tempActions[tempActions.Length - 1]);
                _workflows.Add(workFlowId, actions);

            }
            else
            {
                var partToAdd = new Part();
                foreach (var p in line[1..line.Length].Split(','))
                {
                    var property = p[0];
                    var value = Convert.ToInt32(p[2..].Replace("}",""));
                    switch (property)
                    {
                        case 'x':
                            partToAdd.X = value; break;
                        case 'm':
                            partToAdd.M = value; break;
                        case 'a':
                            partToAdd.A = value; break;
                        case 's':
                            partToAdd.S = value; break;
                    }
                }
                _parts.Add(partToAdd);
            }
        }
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        List<Part> acceptedParts = new List<Part>();

        foreach (var part in _parts)
        {
            AnsiConsole.MarkupLineInterpolated($"[yellow]X:{part.X}[/],[green]M:{part.M}[/],[red]A:{part.A}[/],[khaki1]S:{part.S}[/]");
            if (ProcessStep(part, "in"))
            {
                AnsiConsole.WriteLine("");
                AnsiConsole.WriteLine("Part Accepted");
                acceptedParts.Add(part);
            }
        }

        var total = acceptedParts.Sum(x => x.A + x.S + x.M + x.X);

        AnsiConsole.MarkupLineInterpolated($"Total was [green]{total}[/]");

        bool ProcessStep(Part part, string nextWorkflow)
        {
            AnsiConsole.MarkupLineInterpolated($"-> {nextWorkflow}");
            if (nextWorkflow == "A") return true;
            if (nextWorkflow == "R") return false;

            var workflow = _workflows[nextWorkflow];

            for (var index = 0; index < workflow.Length - 1; index++)
            {
                var step = workflow[index];

                if (step.value == 0)
                    return ProcessStep(part, step.result);

                bool goToNext = step.propertyName switch
                {
                    'x' => Compare(part.X, step.value, step.gt),
                    'm' => Compare(part.M, step.value, step.gt),
                    'a' => Compare(part.A, step.value, step.gt),
                    's' => Compare(part.S, step.value, step.gt)
                };

                if (goToNext)
                {
                    return ProcessStep(part, step.result);
                }
            }

            switch (workflow.Last().result)
            {
                case "A": return true;
                case "R": return false;
                default: return ProcessStep(part, workflow.Last().result);
            }

            bool Compare(int a, int b, bool isGt)
            {
                AnsiConsole.MarkupInterpolated($"Comparing [yellow]{a}[/] [red]{(isGt ? '>' : '<' )}[/] [yellow]{b}[/]");
                return (isGt && a > b) || (!isGt && a < b);
            }
        }
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, true);
    }
}