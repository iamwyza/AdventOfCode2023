using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.XPath;
using AdventOfCode2023.GridUtilities;
using Spectre.Console.Rendering;

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

    private class Bound
    {
        public int MaxX = 4000;
        public int MaxM = 4000;
        public int MaxA = 4000;
        public int MaxS = 4000;

        public int MinX = 0;
        public int MinM = 0;
        public int MinA = 0;
        public int MinS = 0;

        public int X => MaxX - MinX + (MinX > 0 ? 1 : 0);
        public int M => MaxM - MinM + (MinM > 0 ? 1 : 0);
        public int A => MaxA - MinA + (MinA > 0 ? 1 : 0);
        public int S => MaxS - MinS + (MinS > 0 ? 1 : 0);

        public static Bound Copy(Bound bound)
        {
            return new Bound()
            {
                MaxX = bound.MaxX,
                MaxM = bound.MaxM,
                MaxA = bound.MaxA,
                MaxS = bound.MaxS,

                MinX = bound.MinX,
                MinM = bound.MinM,
                MinA = bound.MinA,
                MinS = bound.MinS
            };
        }

        public bool IsValid() => MinX <= MaxX && MinM <= MaxM && MinA <= MaxA && MinS <= MaxS;

        public void WriteRange()
        {
            AnsiConsole.MarkupInterpolated($" : [yellow]X:{X}[/],[green]M:{M}[/],[red]A:{A}[/],[khaki1]S:{S}[/]");
        }

        public void WriteBounds()
        {
            AnsiConsole.MarkupInterpolated(GetBoundsString());
        }

        public FormattableString GetBoundsString() =>
            $" : [yellow]X:{MinX} - {MaxX}[/],[green]M:{MinM} - {MaxM}[/],[red]A:{MinA} - {MaxA}[/],[khaki1]S:{MinS} - {MaxS}[/]";

        public IRenderable[] GetBoundsRenderables() => new IRenderable[]
        {
            Markup.FromInterpolated($"[yellow]{MinX} - {MaxX}[/]"),
            Markup.FromInterpolated($"[yellow]{X}[/]"),
            Markup.FromInterpolated($"[green]{MinM} - {MaxM}[/]"),
            Markup.FromInterpolated($"[green]{M}[/]"),
            Markup.FromInterpolated($"[red]{MinA} - {MaxA}[/]"),
            Markup.FromInterpolated($"[red]{A}[/]"),
            Markup.FromInterpolated($"[khaki1]{MinS} - {MaxS}[/]"),
            Markup.FromInterpolated($"[khaki1]{S}[/]")
        };
    }

    private class WorkflowStep
    {
        public char PropertyName;
        public bool Gt;
        public int Value;
        public string Result;
        public string WorkflowKey;

        public WorkflowStep()
        {

        }

        public WorkflowStep(char propertyName, bool gt, int value, string result, string workflowKey)
        {
            PropertyName = propertyName;
            Gt = gt;
            Value = value;
            Result = result;
            WorkflowKey = workflowKey;
        }

        public void WriteStep()
        {
            AnsiConsole.MarkupInterpolated(GetStepString());
        }

        public FormattableString GetStepString()
        {
            if (Value == 0)
                return $" -> ([green]{WorkflowKey}[/]) [yellow]Fallback[/]";
            else
                return $" -> ([green]{WorkflowKey}[/]) [yellow]{PropertyName}[/] [red]{(Gt ? '>' : '<')}[/] [yellow]{Value}[/]";
        }

        public FormattableString GetStepString2()
        {
            if (Value == 0)
                return $"[blue]{Result}[/]";

            return $"[yellow]{PropertyName}[/] [red]{(Gt ? '>' : '<')}[/] [yellow]{Value}[/]:[blue]{Result}[/]";
        }
    }

    private List<Part> _parts = new List<Part>();

    private Dictionary<string, WorkflowStep[]> _workflows = new();

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
                WorkflowStep[] actions = new WorkflowStep[tempActions.Length];
                for (var index = 0; index < tempActions.Length - 1; index++)
                {
                    var t = tempActions[index];
                    var temp3 = t[2..t.Length].Split(':');
                    actions[index] = new(t[0], t[1] == '>', Convert.ToInt32(temp3[0]), temp3[1], workFlowId);
                }

                actions[tempActions.Length - 1] = new('F', true, 0, tempActions[tempActions.Length - 1], workFlowId);
                _workflows.Add(workFlowId, actions);

            }
            else
            {
                var partToAdd = new Part();
                foreach (var p in line[1..line.Length].Split(','))
                {
                    var property = p[0];
                    var value = Convert.ToInt32(p[2..].Replace("}", ""));
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

                if (step.Value == 0)
                    return ProcessStep(part, step.Result);

                bool goToNext = step.PropertyName switch
                {
                    'x' => Compare(part.X, step.Value, step.Gt),
                    'm' => Compare(part.M, step.Value, step.Gt),
                    'a' => Compare(part.A, step.Value, step.Gt),
                    's' => Compare(part.S, step.Value, step.Gt)
                };

                if (goToNext)
                {
                    return ProcessStep(part, step.Result);
                }
            }

            switch (workflow.Last().Result)
            {
                case "A": return true;
                case "R": return false;
                default: return ProcessStep(part, workflow.Last().Result);
            }

            bool Compare(int a, int b, bool isGt)
            {
                AnsiConsole.MarkupInterpolated($"Comparing [yellow]{a}[/] [red]{(isGt ? '>' : '<')}[/] [yellow]{b}[/]");
                return (isGt && a > b) || (!isGt && a < b);
            }
        }
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        List<(WorkflowStep[], Bound)> paths = new List<(WorkflowStep[], Bound)>();
        Dictionary<string, WorkflowStep[]> reducedSteps = _workflows.ToDictionary(x => x.Key,
            x => x.Value.Select(y => new WorkflowStep(y.PropertyName, y.Gt, y.Value, y.Result, y.WorkflowKey))
                .ToArray());


        List<string> replaced = new List<string>();
        bool found = false;
        do
        {
            found = false;
            foreach (var key in reducedSteps.Keys)
            {
                var steps = reducedSteps[key];
                foreach (var result in new[] { "A", "R" })
                {
                    if (steps.All(x => x.Result == result))
                    {
                        foreach (var flow in reducedSteps)
                        {
                            foreach (var subStep in flow.Value)
                            {
                                if (subStep.Result == key)
                                {
                                    subStep.Result = result;
                                }
                            }
                        }
                        replaced.Add(key);
                        found = true;
                    }
                }
                
            }

            foreach (var r in replaced)
                reducedSteps.Remove(r);

            replaced.Clear();
        } while (found);


        foreach (var (flow, steps) in reducedSteps)
        {
            AnsiConsole.MarkupInterpolated($"[green]{flow}[/]");
            AnsiConsole.Write('{');
            bool first = true;
            foreach (var step in steps)
            {
                if (!first)
                    AnsiConsole.Write(",");
                first = false;
                AnsiConsole.MarkupInterpolated(step.GetStepString2());
            }
            AnsiConsole.WriteLine('}');
        }


        WorkflowStep[] start = reducedSteps["in"];

        ProcessStep(start, new List<WorkflowStep>(), new Bound());

        long totalCombinations = 0;
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("==========================================================");
        AnsiConsole.WriteLine();

        var table = new Table();
        table.AddColumns("Steps", "Bounds X", "X", "Bounds M", "M", "Bounds A", "A", "Bounds S", "S", "Combinations");
        table.Expand = true;
        foreach ((WorkflowStep[] pathway, Bound bounds) in paths)
        {
            FormattableString stepStrings = $"";

            foreach (var step in pathway)
            {
                stepStrings = $"{stepStrings}{step.GetStepString()}";
            }

            var combos = (long)bounds.X * (long)bounds.M * (long)bounds.A * (long)bounds.S;

            table.AddRow(new IRenderable[] { Markup.FromInterpolated(stepStrings) }.Union(bounds.GetBoundsRenderables()).Append(Markup.FromInterpolated($"[khaki1]{combos}[/]")));

            totalCombinations += combos;
        }

        AnsiConsole.Write(table);

        AnsiConsole.MarkupLineInterpolated($"Total Possible combinations: {totalCombinations}");

        void ProcessStep(WorkflowStep[] steps, List<WorkflowStep> previousSteps, Bound bounds)
        {
            var newBound = Bound.Copy(bounds);
            var step = steps[0];

            //step.WriteStep();

            var next = new List<WorkflowStep>(previousSteps) { step };

            if (step.Result == "R")
            {
                //Console.WriteLine(" -> R");
                UpdateBounds(newBound, step, true);

                if (steps.Length > 1)
                    ProcessStep(steps.Skip(1).ToArray(), next, newBound);
                return;
            }

            if (step.Value == 0)
            {
                if (step.Result == "A")
                {
                    //var temp = Bound.Copy(newBound);
                    paths.Add((next.ToArray(), newBound));
                    //Console.WriteLine(" -> A");
                }
                else
                {
                    ProcessStep(reducedSteps[step.Result], next, newBound);
                }

                return;
            }

            if (IsValidStep(step, newBound))
            {
                var temp = Bound.Copy(newBound);
                UpdateBounds(temp, step, false);
                UpdateBounds(newBound, step, true);

                if (step.Result == "A")
                {
                    paths.Add((next.ToArray(), temp));
                    //Console.WriteLine(" -> A");
                }
                else
                {
                    ProcessStep(reducedSteps[step.Result], next, temp);
                }
            }
            else
            {
                UpdateBounds(newBound, step, true);
            }

            if (steps.Length > 1)
                ProcessStep(steps.Skip(1).ToArray(), next, newBound);
        }

        bool IsValidStep(WorkflowStep step, Bound bounds)
        {
            return step.PropertyName switch
            {
                'x' => Compare(step.Gt ? bounds.MaxX : bounds.MinX, step.Value, step.Gt, step.PropertyName),
                'm' => Compare(step.Gt ? bounds.MaxM : bounds.MinM, step.Value, step.Gt, step.PropertyName),
                'a' => Compare(step.Gt ? bounds.MaxA : bounds.MinA, step.Value, step.Gt, step.PropertyName),
                's' => Compare(step.Gt ? bounds.MaxS : bounds.MinS, step.Value, step.Gt, step.PropertyName)
            };
        }

        void UpdateBounds(Bound bounds, WorkflowStep step, bool inverted)
        {
            var value = step.Value;
            if (step.Value > 0)
            {
                var updateMin = step.Gt;
                if (inverted)
                    updateMin = !updateMin;

                if (updateMin)
                {
                    if (!inverted)
                        value++;
                    switch (step.PropertyName)
                    {
                        case 'x':
                            if (value > bounds.MinX)
                                bounds.MinX = value;
                            break;
                        case 'm':
                            if (value > bounds.MinM)
                                bounds.MinM = value;
                            break;
                        case 'a':
                            if (value > bounds.MinA)
                                bounds.MinA = value;
                            break;
                        case 's':
                            if (value > bounds.MinS)
                                bounds.MinS = value;
                            break;
                    }
                }
                else
                {
                    if (!inverted)
                        value--;
                    switch (step.PropertyName)
                    {
                        case 'x':
                            if (value < bounds.MaxX)
                                bounds.MaxX = value;
                            break;
                        case 'm':
                            if (value < bounds.MaxM)
                                bounds.MaxM = value;
                            break;
                        case 'a':
                            if (value < bounds.MaxA)
                                bounds.MaxA = value;
                            break;
                        case 's':
                            if (value < bounds.MaxS)
                                bounds.MaxS = value;
                            break;
                    }
                }
            }
        }

        bool Compare(int a, int b, bool isGt, char property)
        {
            //AnsiConsole.MarkupLineInterpolated($"Comparing [green]{property}[/] [yellow]{a}[/] [red]{(isGt ? '>' : '<')}[/] [yellow]{b}[/]");
            return (isGt && a > b) || (!isGt && a < b);
        }
    }
}