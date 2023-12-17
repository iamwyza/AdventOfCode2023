using System.Numerics;

// This is a derivation of https://github.com/davecusatis/A-Star-Sharp/blob/master/Astar.cs but with using the Grid class I use instead of the List<List<Node>>
// Also updated to handle nullable reference types, converts from Vector2 to Coord, converts from float to int since we don't need floating point math for this implementation
// Also adds the ability to inject special logic when determining if the edge is open based on some arbitrary rules (like in Day 17 where you can't go more than 3 tiles in the same direction)
namespace AdventOfCode2023.GridUtilities;

internal class Node :IComparable
{
    public Node? Parent;
    public Coord Position;
    public int DistanceToTarget;
    public int Cost;
    public int Weight;
    public Direction Direction;
    public float F
    {
        get
        {
            if (DistanceToTarget != -1 && Cost != -1)
                return DistanceToTarget + Cost;

            return -1;
        }
    }
    public bool Walkable;

    public Node(Coord pos, bool walkable, int weight = 1)
    {
        Parent = null;
        Position = pos;
        DistanceToTarget = -1;
        Cost = 1;
        Weight = weight;
        Walkable = walkable;
    }

    public override string ToString()
    {
        return Position.ToString();
    }

    public int CompareTo(object? obj)
    {
        throw new NotImplementedException();
    }
}

internal class AstarBestPath
{
    Grid<Node> Grid;
   

    public AstarBestPath(Grid<Node> grid)
    {
        Grid = grid;
    }

    public Stack<Node>? FindPath(Coord startVector, Coord endVector, Func<Node, Node, bool>? parentValidationLogic = null, Action<Node>? debugAction = null, int bestScoreStart = 0)
    {
        Node start = new Node(new Coord(startVector.X, startVector.Y), true);
        Node end = new Node(new Coord(endVector.X , endVector.Y), true);

        Stack<Node> path = new Stack<Node>();
        PriorityQueue<Node, float> openList = new PriorityQueue<Node, float>();
        List<Node> closedList = new ();
        HashSet<int> closedCoords = new((1 << 17));
        HashSet<Node> queuedItems = new ();
        Node current = start;
        int bestScore = bestScoreStart > 0 ? bestScoreStart : int.MaxValue;
        
        // add start node to Open List
        openList.Enqueue(start, start.F);
        int permutations = 0;
        while (openList.Count != 0) // && !closedList.Exists(x => x.Position == end.Position))
        {


            permutations++;
            if (permutations % 100_000 == 0)
            {
                AnsiConsole.MarkupLineInterpolated($"Processed [yellow]{permutations}[/] steps. With [yellow]{queuedItems.Count}[/] (or [yellow]{openList.Count}[/]) in the queue. ");
            }
            current = openList.Dequeue();
            queuedItems.Remove(current);

            debugAction?.Invoke(current);

            closedList.Add(current);
            var lookup = ((1 + current.Position.X) << 8) + ((1+ current.Position.Y) << 16);
            closedCoords.Add(lookup);

            if (current.Position == end.Position)
            {
                if (current.Cost < bestScore)
                    bestScore = current.Cost;
            }

            if (current.Cost > bestScore)
                continue;

            var adjacentNodes = GetAdjacentNodes(current);

            foreach (Node n in adjacentNodes)
            {
                //var direction = DirectionExtensions.GetDirection(current.Position, n.Position);
                lookup = ((1 + n.Position.X) << 8) + ((1 +n.Position.Y) << 16);
                if (closedCoords.Contains(lookup) || !n.Walkable) continue;

                if (parentValidationLogic != null && !parentValidationLogic(current, n)) continue;


                bool isFound = queuedItems.Contains(n);

                if (!isFound)
                {
                    n.Parent = current;
                    //n.DistanceToTarget = Math.Abs(n.Position.X - end.Position.X) + Math.Abs(n.Position.Y - end.Position.Y);
                    n.DistanceToTarget = 0;
                    n.Cost = n.Weight + n.Parent.Cost;
                    openList.Enqueue(n, n.F);
                    queuedItems.Add(n);
                }
            }
        }

        // construct path, if end was not closed return null
        if (!closedList.Exists(x => x.Position == end.Position))
        {
            return null;
        }


        // if all good, return path
        Node? temp = closedList.Where(x => x.Position == end.Position).OrderBy(x => x.Cost).First();
        if (temp == null) return null;
        do
        {
            path.Push(temp);
            temp = temp.Parent;
        } while (temp != start && temp != null);
        return path;
    }

    private List<Node> GetAdjacentNodes(Node n)
    {
        List<Node> temp = new List<Node>();

        int row = n.Position.Y;
        int col = n.Position.X;

        if (row + 1 < Grid.Rows + 1)
        {
            temp.Add(CloneNode(Grid[col, row + 1], Direction.South)!);
        }
        if (row - 1 >= 0)
        {
            temp.Add(CloneNode(Grid[col, row - 1], Direction.North)!);
        }
        if (col - 1 >= 0)
        {
            temp.Add(CloneNode(Grid[col - 1, row], Direction.West)!);
        }
        if (col + 1 < Grid.Columns + 1)
        {
            temp.Add(CloneNode(Grid[col + 1, row], Direction.East)!);
        }

        return temp;

        Node? CloneNode(Node? node, Direction? direction, int depth = 0)
        {
            if (node == null || depth > 4)
            {
                return null;
            }

            return new Node(node.Position, node.Walkable, node.Weight)
            {
                Cost = node.Cost,
                Parent = node.Parent,
                DistanceToTarget = node.DistanceToTarget,
                Direction = direction ?? node.Direction
            };
        }
    }
}

internal class Astar
{
    Grid<Node> Grid;
    public Astar(Grid<Node> grid)
    {
        Grid = grid;
    }

    
    public Stack<Node>? FindPath(Coord startVector, Coord endVector, Func<Node, Node, bool>? parentValidationLogic = null, Action<Node>? debugAction = null)
    {
        Node start = new Node(new Coord(startVector.X, startVector.Y), true);
        Node end = new Node(new Coord(endVector.X, endVector.Y), true);

        Stack<Node> path = new Stack<Node>();
        PriorityQueue<Node, float> openList = new PriorityQueue<Node, float>();
        List<Node> closedList = new();
        List<Node> adjacencies;
        Node current = start;

        // add start node to Open List
        openList.Enqueue(start, start.F);

        while (openList.Count != 0)
        {
            current = openList.Dequeue();
            closedList.Add(current);
            adjacencies = GetAdjacentNodes(current);

            foreach (Node n in adjacencies)
            {
                if (!closedList.Contains(n) && n.Walkable)
                {

                    if (parentValidationLogic != null && !parentValidationLogic(current, n)) continue;

                    bool isFound = false;
                    foreach (var oLNode in openList.UnorderedItems)
                    {
                        if (oLNode.Element == n)
                        {
                            isFound = true;
                        }
                    }
                    if (!isFound)
                    {
                        n.Parent = current;
                        n.DistanceToTarget = Math.Abs(n.Position.X - end.Position.X) + Math.Abs(n.Position.Y - end.Position.Y);
                        n.Cost = n.Weight + n.Parent.Cost;
                        openList.Enqueue(n, n.F);
                    }
                }
            }
        }

        // construct path, if end was not closed return null
        if (!closedList.Exists(x => x.Position == end.Position))
        {
            return null;
        }

        // if all good, return path
        Node temp = closedList[closedList.IndexOf(current)];
        if (temp == null) return null;
        do
        {
            path.Push(temp);
            temp = temp.Parent;
        } while (temp != start && temp != null);
        return path;
    }

    private List<Node> GetAdjacentNodes(Node n)
    {
        List<Node> temp = new List<Node>();

        int row = (int)n.Position.Y;
        int col = (int)n.Position.X;

        if (row + 1 < Grid.Rows + 1)
        {
            temp.Add(Grid[col, row + 1]);
        }
        if (row - 1 >= 0)
        {
            temp.Add(Grid[col, row - 1]);
        }
        if (col - 1 >= 0)
        {
            temp.Add(Grid[col - 1,row]);
        }
        if (col + 1 < Grid.Columns +1 )
        {
            temp.Add(Grid[col + 1,row]);
        }

        return temp;
    }
}