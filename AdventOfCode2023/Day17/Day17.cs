using System.Numerics;
using System.Runtime.InteropServices;
using AdventOfCode2023.GridUtilities;

namespace AdventOfCode2023.Day17;
internal class Day17 : DayBase
{
    public Day17()
    {
        Ready = true;
    }


    private Grid<int> _map = null!;

    private async Task Init(int part, bool useTestData)
    {
        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        // "c -'0'" -> This only works if you're guaranteed to have 0-9 as the values in the char location, otherwise using char.GetNumericValue is probably what you want.
        _map = new Grid<int>(lines, c => c - '0');

        _map.PrintMap(null, true, true);
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, true);
        int bestHeat = int.MaxValue;

        var overlayGrid = _map.CopyGrid((_, input) => false);

        var pathFindingGrid = _map.CopyGrid((coord, input) =>
            new Node(coord, true, input )
        );

        pathFindingGrid[0, 0].Cost = _map[0, 0];

        pathFindingGrid.DefaultPrintConfig = (node, coord) => (node.Weight.ToString()[0], Color.Default, "");

        pathFindingGrid.PrintMap();

        Func<Direction, Coord, (char letter, Color color, string? extraText)> mapDebugPrintConfig =
            (value, coord) => (value == Direction.None ? _map[coord].ToString()[0] : value.DirectionString(), value != Direction.None ? Color.Green : Color.Default, "");
        var totalHeat = 0;

        //var graph = new Graph(pathFindingGrid);

        //var results = Algorithms<Node>.Dijkstra(graph, graph.Grid[0,0], 0, (currentNode, newNode) => EnsureNoMoreThanStraightLine(4, currentNode, newNode));
        //var result = results.First(x => x.Key.Position == new Coord(12, 12));

        //overlayGrid.PrintMap();
        //PrintDebugGrid(result.Key, (value, coord) => (_map[coord].ToString()[0], value != Direction.None ? Color.Green : Color.Default, ""));

        //AnsiConsole.MarkupLineInterpolated($"This can't have worked.... [green]{result.Value}[/]");

        var astarApprox = new Astar(pathFindingGrid);
        var approxPath = astarApprox.FindPath(new Coord(0, 0), new Coord(pathFindingGrid.Bounds.maxX, pathFindingGrid.Bounds.maxY),
            (currentNode, newNode) => EnsureNoMoreThanStraightLine(4, currentNode, newNode),
            (node) => PrintDebugGrid(node, mapDebugPrintConfig));

        var astar = new AstarBestPath(pathFindingGrid);
        var path = astar.FindPath(new Coord(0, 0), new Coord(pathFindingGrid.Bounds.maxX, pathFindingGrid.Bounds.maxY),
            (currentNode, newNode) => EnsureNoMoreThanStraightLine(4, currentNode, newNode),
            (node) => PrintDebugGrid(node, mapDebugPrintConfig), approxPath.Last().Cost);

        if (path is null)
        {
            AnsiConsole.WriteLine("No path found :(");
            return;
        }

        foreach (var step in path)
        {
            totalHeat += _map[step.Position];
            overlayGrid[step.Position] = true;
        }

        overlayGrid.PrintMap();
        PrintDebugGrid(path.Last(), (value, coord) => (_map[coord].ToString()[0], value != Direction.None ? Color.Green : Color.Default, ""));

        AnsiConsole.MarkupLineInterpolated($"This can't have worked.... [green]{totalHeat}[/]");

    
    }

    private void PrintDebugGrid(Node node, Func<Direction, Coord, (char letter, Color color, string? extraText)> mapDebugPrintConfig)
    {
        return;
        var debugGrid = _map.CopyGrid((_, input) => Direction.None);
        debugGrid.DefaultPrintConfig = mapDebugPrintConfig;
        var currentNode = node;
        debugGrid[node.Position] = DirectionExtensions.GetDirection(new Coord(0, 0), node.Position);

        while (currentNode is not null)
        {
            if (currentNode.Parent is not null)
            {
                debugGrid[currentNode.Position] = DirectionExtensions.GetDirection(currentNode.Position, currentNode.Parent.Position);
                currentNode = currentNode.Parent;
            }
            else
            {
                break;
            }
        }

        debugGrid.PrintMap();
    }

    private bool EnsureNoMoreThanStraightLine(int max, Node checkNode, Node newNode)
    {
        var parentDirections = new List<Direction>
        {
            GetDirection( newNode, checkNode)
        };

        var currentNode = checkNode;

        while (currentNode != null)
        {
            if (currentNode.Parent is not null)
            {
                parentDirections.Add(GetDirection(currentNode, currentNode.Parent));
                currentNode = currentNode.Parent;
            }
            else
            {
                break;
            }

            if (parentDirections.Count >= max)
            {
                break;
            }
        }

        if (parentDirections.Count < max) return true;

        var isStraight = true;

        for (var index = 0; index < parentDirections.Count-1; index++)
        {
            isStraight &= parentDirections[index] == parentDirections[index+1];
        }

        return !isStraight;

        // Assumes cardinal directions
        Direction GetDirection(Node node, Node previousNode)
        {
            return DirectionExtensions.GetDirection(previousNode.Position, node.Position);
        }
    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, true);
    }

    private class Graph : GridGraph
    {
        public Graph(Grid<Node> grid) : base(grid)
        {
        }

        public override int GetWeight(Node u, Node v)
        {
            return v.Weight;
        }
    }
}