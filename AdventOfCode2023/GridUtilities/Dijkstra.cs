namespace AdventOfCode2023.GridUtilities;
internal class StateGraph : IGraph<StateGraph.State>
{
    public readonly struct State : IEquatable<State>, IComparable<long>
    {
        private readonly long _value;
        public Coord Position => new((uint)((_value >> 32) & 0xffffffff));
        public Direction Direction => (Direction)(_value >> 19 & 0x1fff);
        public int Distance => (int)(_value & 0xfffff);


        public State(Coord position, Direction direction, int distance)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(distance, 0xfffff);

            _value = ((long)position.RawValue << 32)
                     + ((long)((int)direction & 0x1fff) << 19)
                     + distance;
        }

        public bool Equals(State other)
        {
            return _value == other._value;
        }

        public override bool Equals(object? obj)
        {
            return obj is State other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(State left, State right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(State left, State right)
        {
            return left._value != right._value;
        }

        public static bool operator ==(State left, long right)
        {
            return left._value == right;
        }

        public static bool operator !=(State left, long right)
        {
            return left._value != right;
        }

        public int CompareTo(long other)
        {
            return _value.CompareTo(other);
        }
    }

    public readonly Grid<sbyte> Grid;

    public StateGraph(Grid<sbyte> grid)
    {
        Grid = grid;
    }

    public virtual IEnumerable<State> GetVertices()
    {
        return Grid.Select(x => new State(x.Coordinate, Direction.None, 0)).ToList();
    }

    public virtual IEnumerable<State> GetNeighbors(State vertex, Func<State, Direction, bool>? neighborFilter)
    {
        int row = vertex.Position.Y;
        int col = vertex.Position.X;

        if (row + 1 < Grid.Rows + 1 && vertex.Direction != Direction.North)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.South))
                yield return new State(vertex.Position.Move(Direction.South), Direction.South, vertex.Direction == Direction.South ? vertex.Distance + 1 : 1);
        }
        if (row - 1 >= 0 && vertex.Direction != Direction.South)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.North))
                yield return new State(vertex.Position.Move(Direction.North), Direction.North, vertex.Direction == Direction.North ? vertex.Distance + 1 : 1);
        }
        if (col - 1 >= 0 && vertex.Direction != Direction.East)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.West))
                yield return new State(vertex.Position.Move(Direction.West), Direction.West, vertex.Direction == Direction.West ? vertex.Distance + 1 : 1);
        }
        if (col + 1 < Grid.Columns + 1 && vertex.Direction != Direction.West)
        {
            if (neighborFilter is null || neighborFilter.Invoke(vertex, Direction.East))
                yield return new State(vertex.Position.Move(Direction.East), Direction.East, vertex.Direction == Direction.East ? vertex.Distance + 1 : 1);
        }
    }

    public int GetWeight(State u, State v)
    {
        return Grid[v.Position];
    }

}

public interface IGraph<TVertexType> where TVertexType : IEquatable<TVertexType>
{
    IEnumerable<TVertexType> GetVertices();
    IEnumerable<TVertexType> GetNeighbors(TVertexType vertex, Func<TVertexType, Direction, bool>? neighborFilter);
    int GetWeight(TVertexType u, TVertexType v);
}

internal static class Algorithms<TVertexType> where TVertexType : IEquatable<TVertexType>
{
    public static (TVertexType end, double distance, Dictionary<TVertexType, TVertexType?> Path)?
        Dijkstra(IGraph<TVertexType> graph, TVertexType source, Func<TVertexType, bool> isTarget, 
            Func<TVertexType, bool>? continuationLogic = null,
            Func<TVertexType, Direction, bool>? neighborFilter = null)
    {
        var distance = new Dictionary<TVertexType, double>();
        var remaining = new PriorityQueue<TVertexType, double>();
        HashSet<TVertexType> visited = new HashSet<TVertexType>();
        var path = new Dictionary<TVertexType, TVertexType?>();

        distance[source] = graph.GetWeight(source, source);

        foreach (var vertex in graph.GetVertices())
        {
            if (!vertex.Equals(source))
            {
                distance[vertex] = double.PositiveInfinity;
                path[vertex] = default;
            }
        }

        foreach (var vertex in graph.GetNeighbors(source, null))
        {
            var measure = graph.GetWeight(source, vertex);
            distance[vertex] = measure;
            path[vertex] = source;
            remaining.Enqueue(vertex, measure);

        }

        while (remaining.TryDequeue(out TVertexType? subject, out double cost))
        {
            if (isTarget.Invoke(subject))
                return (subject, cost, path);

            foreach (var vertex in graph.GetNeighbors(subject, neighborFilter))
            {

                if (!continuationLogic?.Invoke(vertex) ?? false)
                    continue;
                if (visited.Contains(vertex)) continue;
                visited.Add(vertex);

                var measure = cost + graph.GetWeight(subject, vertex);
                remaining.Enqueue(vertex, measure);

                if (!distance.ContainsKey(vertex))
                {
                    distance.Add(vertex, measure);
                    path[vertex] = subject;
                }


                if (measure < distance[vertex])
                {
                    distance[vertex] = measure;
                    path[vertex] = subject;
                }

            }
        }

        return null;
    }
}
