using System.Diagnostics;
using System.Linq;
using AdventOfCode2023.GridUtilities;

namespace AdventOfCode2023.Day20;

internal static class Day20Extensions
{
    public static string ToString(this Day20.ModuleType moduleType)
    {
        return moduleType switch
        {
            Day20.ModuleType.FlipFlop => "%",
            Day20.ModuleType.Conjunction => "&",
            Day20.ModuleType.Broadcast => "",
            Day20.ModuleType.Button => "",
            _ => throw new ArgumentOutOfRangeException(nameof(moduleType), moduleType, null)
        };
    }
}
internal class Day20 : DayBase
{
    public Day20()
    {
        Ready = true;
    }

    public enum ModuleType
    {
        FlipFlop,
        Conjunction,
        Broadcast,
        Button
    }

    private class Module
    {
        public string Name;
        public ModuleType Type;
        public bool State;
        public Module[] Inputs;
        public Module[] Outputs;
    }

    private List<Module> _modules = new();

    private async Task Init(int part, bool useTestData)
    {
        _modules.Clear();
        var temp = new Dictionary<Module, List<string>>();
        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        foreach (var line in lines)
        {
            var items = line.Split(" -> ");
            var outputs = items[1].Split(",").Select(x => x.Trim()).ToList();
            ModuleType type = items[0] switch
            {
                "broadcaster" => ModuleType.Broadcast,
                "button" => ModuleType.Button,
                _ => items[0][0] switch
                {
                    '%' => ModuleType.FlipFlop,
                    '&' => ModuleType.Conjunction,
                    _ => throw new ArgumentOutOfRangeException()
                }
            };


            var module = new Module()
            {
                Name = type is ModuleType.Broadcast or ModuleType.Button ? items[0] : items[0][1..],
                Type = type
            };
            temp.Add(module, outputs);
        }

        foreach (var (module, outputs) in temp)
        {
            foreach (var output in outputs)
            {
                AnsiConsole.WriteLine($"{module.Name} {output}");
            }
        }

        if (temp.All(x => x.Key.Type != ModuleType.Button))
            temp.Add(new Module()
            {
                Type = ModuleType.Button,
                Name = "button"
            }, new List<string>() { "broadcaster" });

        temp.Add(new Module()
        {
            Name = "output"
        }, new List<string>());


        var missingItems = temp.SelectMany(item => item.Value.Where(x => temp.All(m => m.Key.Name != x))).Distinct().ToArray();
        if (missingItems.Any())
            foreach (var missingItem in missingItems)
            {
                temp.Add(new Module()
                {
                    Name = missingItem
                }, new List<string>());
            }

        foreach (var item in temp)
        {
            item.Key.Outputs = item.Value.Select(x => temp.Single(m => m.Key.Name == x).Key).ToArray();
        }

        foreach (var item in temp)
        {
            item.Key.Inputs = temp.Where(x => x.Key.Outputs.Contains(item.Key)).Select(x => x.Key).ToArray();
        }



        _modules.AddRange(temp.Keys);
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);

        int highSignals = 0;
        int lowSignals = 0;

        for (int i = 0; i < 1000; i++)
        {
            if (i < 10)
                AnsiConsole.WriteLine();

            Queue<(bool Signal, Module OutputModule, Module inputModule)> queue = new Queue<(bool, Module, Module)>();

            queue.Enqueue((false, _modules.Single(x => x.Name == "broadcaster"), _modules.Single(x => x.Name == "button")));
            Module lastInputModule;

            while (queue.TryDequeue(out var e))
            {
                var module = e.OutputModule;
                var isHigh = e.Signal;
                

                if (i < 10)
                {
                    AnsiConsole.MarkupLineInterpolated($"[cyan]{Day20Extensions.ToString(e.inputModule.Type)}[/][yellow]{e.inputModule.Name}[/] -[red]{(isHigh ? "high" : "low")}[/]-> [cyan]{Day20Extensions.ToString(module.Type)}[/][green]{module.Name}[/]");
                }

                lastInputModule = e.inputModule;

                if (isHigh)
                    highSignals++;
                else
                    lowSignals++;

                switch (module.Type)
                {
                    case ModuleType.Broadcast:
                        foreach (var output in module.Outputs)
                        {
                            queue.Enqueue((false, output, module));
                        }
                        break;
                    case ModuleType.FlipFlop:
                        if (!isHigh)
                        {
                            module.State = !module.State;
                            foreach (var output in module.Outputs)
                                queue.Enqueue((module.State, output, module));
                        }
                        break;
                    case ModuleType.Conjunction:

                        var allHigh = module.Inputs.Where(x => x != lastInputModule).All(x => x.State) && isHigh;
                        foreach (var output in module.Outputs)
                            queue.Enqueue((!allHigh, output, module));
                        break;
                }
            }

        }


        AnsiConsole.MarkupLineInterpolated($"Total Low: [yellow]{lowSignals}[/] High: [green]{highSignals}[/] = [red]{lowSignals * highSignals}[/]");
    }

    // I had to visualize this graph before I could realize that each of the outputs from the broadcaster each went into their own discrete and non-interacting branches.  
    // They only recombined 1 step before the end, so we can calculate the number of presses required to get them each into the correct state then find the LCM between those to determine how many it'd actually take.
    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        //var root = new Tree("root");
        //var currentLeaf = root.AddNode("rx");
        //HashSet<Module> visited = new HashSet<Module>();

        //AddNodes(currentLeaf, _modules.Single(x => x.Name == "rx"));


        //void AddNodes(TreeNode currentNode, Module currentModule)
        //{
        //    if (currentModule.Name == "broadcaster")
        //        return;

        //    if (visited.Contains(currentModule)) return;

        //    visited.Add(currentModule);

        //    foreach (var dependency in currentModule.Inputs)
        //    {
        //        var leaf = currentNode.AddNode((dependency.Type == ModuleType.FlipFlop ? "[green]" : "[blue]") + $"{dependency.Name}[/]");
        //        AddNodes(leaf, dependency);
        //    }
        //}

        //AnsiConsole.Write(root);


        //return;
        var rx = _modules.Single(x => x.Name == "rx");
        var broadcaster = _modules.Single(x => x.Name == "broadcaster");
        var button = _modules.Single(x => x.Name == "button");

        var broadcasterOutputs = broadcaster.Outputs;
        var numberOfPresses = new List<ulong>();

        foreach (var branch in broadcasterOutputs)
        {
            broadcaster.Outputs = new Module[1] { branch };

            Module? targetOutput = null;
            
            ulong count = 0;
            Queue<(bool Signal, Module OutputModule, Module inputModule)> queue = new Queue<(bool, Module, Module)>(1000);
            var found = false;
            while (!found)
            {

                count++;

                if (count % 100_000_000 == 0)
                {
                    AnsiConsole.MarkupLineInterpolated($"Up to [yellow]{count:N}[/] permutations.");
                }

                queue.Enqueue((false, broadcaster, button));

                while (queue.TryDequeue(out var e))
                {
                    var module = e.OutputModule;
                    var isHigh = e.Signal;
                    var lastInputModule = e.inputModule;

                    if (targetOutput is not null && module == targetOutput && isHigh)
                    {
                        found = true;
                        break;
                    }

                    if (targetOutput is null)
                    {
                        if (rx.Inputs.Contains(module))
                            targetOutput = module;
                    }

                    switch (module.Type)
                    {
                        case ModuleType.Broadcast:
                            foreach (var output in module.Outputs)
                            {
                                queue.Enqueue((false, output, module));
                            }
                            break;
                        case ModuleType.FlipFlop:
                            if (!isHigh)
                            {
                                module.State = !module.State;
                                foreach (var output in module.Outputs)
                                {
                                    queue.Enqueue((module.State, output, module));
                                }
                            }
                            break;
                        case ModuleType.Conjunction:

                            bool allHigh = isHigh;
                            if (allHigh)
                            {
                                foreach (var input in module.Inputs)
                                {
                                    if (input == lastInputModule) continue;
                                    if (!input.State)
                                    {
                                        allHigh = false;
                                        break;
                                    }
                                }
                            }


                            //var allHigh = module.Inputs.Where(x => x != lastInputModule).All(x => x.State) && isHigh;
                            foreach (var output in module.Outputs)
                            {
                                queue.Enqueue((!allHigh, output, module));
                            }
                            break;
                    }

                }

            }

            numberOfPresses.Add(count);
            AnsiConsole.MarkupLineInterpolated($"Branch {branch.Name} took {count} presses to get to the point where it's final output was low.");
        }

        var lcm = numberOfPresses[0];
        foreach (var num in numberOfPresses.Skip(1))
        {
            lcm = MathUtils.LeastCommonMultiple(lcm, num);
        }

        AnsiConsole.MarkupLineInterpolated($"Minimal [green]{lcm}[/] button presses");

    }
}