using System;
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


    private Grid<sbyte> _map = null!;

    private async Task Init(int part, bool useTestData)
    {
        var lines = useTestData ? await GetTestLines(part) : await GetLines();

        // "c -'0'" -> This only works if you're guaranteed to have 0-9 as the values in the char location, otherwise using char.GetNumericValue is probably what you want.
        _map = new Grid<sbyte>(lines, c => (sbyte)(c - '0'));

        _map.PrintMap(null, true, true);
    }

    public override async Task RunPart1()
    {
        PrintStart(1);
        await Init(1, false);
        int bestHeat = int.MaxValue;

        var overlayGrid = _map.CopyGrid((_, input) => false);

        var pathFindingGrid = _map.CopyGrid((coord, input) =>
            input
        );

        overlayGrid.DefaultPrintConfig =
            (isPath, coord) => (isPath ? '#' : '.', isPath ? Color.Green : Color.Default, "");

        pathFindingGrid.DefaultPrintConfig = (node, coord) => (node.ToString()[0], Color.Default, "");

        pathFindingGrid.PrintMap();

        var graph = new StateGraph(pathFindingGrid);
        var endLocation = new Coord(overlayGrid.Bounds.maxX, overlayGrid.Bounds.maxY);

        var results = Algorithms<StateGraph.State>.Dijkstra(graph: graph, 
            source: new StateGraph.State(new Coord(0, 0), Direction.None, 0), 
            isTarget: node => node.Position == endLocation, 
            continuationLogic: currentNode => currentNode.Distance <= 3);
        
        if (results is null)
        {
            AnsiConsole.WriteLine("Couldn't Find Path");
            return;
        }

        overlayGrid[results.Value.end.Position] = true;

        var currentNode = results.Value.Path[results.Value.end];
        while (currentNode != 0 && results.Value.Path.ContainsKey(currentNode))
        {
            overlayGrid[currentNode.Position] = true;
            currentNode = results.Value.Path[currentNode];
        }

        overlayGrid.PrintMap();

        AnsiConsole.MarkupLineInterpolated($"This can't have worked.... [green]{results.Value.distance}[/]");


    }

    public override async Task RunPart2()
    {
        PrintStart(2);
        await Init(2, false);

        int bestHeat = int.MaxValue;

        var overlayGrid = _map.CopyGrid((_, input) => Direction.None);

        var pathFindingGrid = _map.CopyGrid((coord, input) =>
            input
        );

        overlayGrid.DefaultPrintConfig =
            (path, coord) => (path.DirectionString(), path == Direction.None ? Color.Default : Color.Green, "");

        pathFindingGrid.DefaultPrintConfig = (node, coord) => (node.ToString()[0], Color.Default, "");

        pathFindingGrid.PrintMap();

        var graph = new StateGraph(pathFindingGrid);
        var endLocation = new Coord(overlayGrid.Bounds.maxX, overlayGrid.Bounds.maxY);

        var results = Algorithms<StateGraph.State>.Dijkstra(graph: graph,
            source: new StateGraph.State(new Coord(0, 0), Direction.None, 0),
            isTarget: node => (node.Position == endLocation && node.Distance >= 4),
            continuationLogic: currentNode => currentNode.Distance <= 10,
            neighborFilter: (currentNode, direction) => currentNode.Direction == direction || currentNode.Distance >= 4);

    if (results is null)
        {
            AnsiConsole.WriteLine("Couldn't Find Path");
            return;
        }

        overlayGrid[results.Value.end.Position] = results.Value.end.Direction;

        var currentNode = results.Value.Path[results.Value.end];
        while (currentNode != 0 && results.Value.Path.ContainsKey(currentNode))
        {
            overlayGrid[currentNode.Position] = currentNode.Direction;
            currentNode = results.Value.Path[currentNode];
        }

        overlayGrid.PrintMap();

        AnsiConsole.MarkupLineInterpolated($"This can't have worked.... [green]{results.Value.distance}[/]");
    }
}