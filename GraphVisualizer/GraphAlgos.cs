using System.Numerics;

namespace GraphVisualizer
{
    public enum DijkstraType
    {
        MIN,
        MAX,
    }

    public static class GraphAlgos
    {
        public static List<T> Dijkstra<T>(
            Graph<T> sourceGraph,
            string start,
            DijkstraType type = DijkstraType.MIN
        )
            where T : INumber<T>
        {
            List<List<(int, T)>> graph = sourceGraph.GraphAsList;
            int? startIndex = sourceGraph.GetVertexIndex(start);

            if (!startIndex.HasValue)
            {
                throw new ArgumentException($"Can't find start vertex {start}");
            }

            T defaultDistance = T.Zero;

            if (type == DijkstraType.MIN)
            {
                if (graph.Any(edges => edges.Any(edge => edge.Item2 < T.Zero)))
                {
                    throw new InvalidOperationException("Dijkstra don't work with negative edges");
                }
                else
                {
                    defaultDistance = sourceGraph.EdgesSum;
                }
            }

            List<T> distance = Enumerable.Repeat(defaultDistance, graph.Count).ToList();

            SortedSet<(T, int)> queue = [(T.Zero, startIndex.Value)];
            distance[startIndex.Value] = T.Zero;

            while (queue.Count > 0)
            {
                var (dist, v) = type == DijkstraType.MIN ? queue.Min() : queue.Max();
                queue.Remove((dist, v));

                foreach (var (u, weight) in graph[v])
                {
                    if (
                        (distance[u] > dist + weight && type == DijkstraType.MIN)
                        || (distance[u] < dist + weight && type == DijkstraType.MAX)
                    )
                    {
                        queue.Remove((distance[u], u));

                        distance[u] = dist + weight;

                        queue.Add((distance[u], u));
                    }
                }
            }

            return distance;
        }

        private static void ColorComponent<T>(
            List<List<(int, T)>> graph,
            int start,
            ref List<int> visited,
            int marker
        )
            where T : INumber<T>
        {
            Queue<int> queue = new();

            queue.Enqueue(start);
            visited[start] = marker;

            while (queue.Count > 0)
            {
                int v = queue.Dequeue();

                foreach (var (u, _) in graph[v])
                {
                    if (visited[u] == 0)
                    {
                        queue.Enqueue(u);
                        visited[u] = marker;
                    }
                }
            }
        }

        public static List<int> BFS<T>(Graph<T> sourceGraph, string start)
            where T : INumber<T>
        {
            var graph = sourceGraph.GraphAsList;
            int? startIndex = sourceGraph.GetVertexIndex(start);

            if (!startIndex.HasValue)
            {
                throw new ArgumentException($"Can't find start vertex {start}");
            }

            Queue<int> queue = new();

            List<int> visitedAt = Enumerable.Repeat(-1, graph.Count).ToList();

            queue.Enqueue(startIndex.Value);
            visitedAt[startIndex.Value] = 0;

            while (queue.Count > 0)
            {
                int v = queue.Dequeue();

                foreach (var (u, _) in graph[v])
                {
                    if (visitedAt[u] == -1)
                    {
                        queue.Enqueue(u);
                        visitedAt[u] = visitedAt[v] + 1;
                    }
                }
            }

            return visitedAt;
        }

        public static List<int> GetComponents<T>(Graph<T> sourceGraph)
            where T : INumber<T>
        {
            var graph = sourceGraph.GraphAsList;

            List<int> colorOfComponent = Enumerable.Repeat(0, graph.Count).ToList();

            int color = 1;

            for (int v = 0; v < graph.Count; v++)
            {
                if (colorOfComponent[v] != 0)
                {
                    continue;
                }

                ColorComponent(graph, v, ref colorOfComponent, color);

                color += 1;
            }

            return colorOfComponent;
        }

        private static void TinToutDFS<T>(
            int vertex,
            ref List<List<(int, T)>> tree,
            ref List<int> tin,
            ref List<int> tout,
            ref int timer
        )
        {
            tin[vertex] = timer++;

            foreach (var (u, _) in tree[vertex])
            {
                if (tin[u] == -1)
                {
                    TinToutDFS(u, ref tree, ref tin, ref tout, ref timer);
                }
            }

            tout[vertex] = timer++;
        }

        public static (List<int>, List<int>) GetTinTout<T>(Graph<T> sourceTree, string root)
            where T : INumber<T>
        {
            var tree = sourceTree.GraphAsList;
            int? rootIndex = sourceTree.GetVertexIndex(root);

            if (!rootIndex.HasValue)
            {
                throw new ArgumentException($"Can't find {root} in vertices");
            }

            List<int> tin = Enumerable.Repeat(-1, tree.Count).ToList();
            List<int> tout = Enumerable.Repeat(-1, tree.Count).ToList();

            int timer = 0;
            TinToutDFS(rootIndex.Value, ref tree, ref tin, ref tout, ref timer);

            return (tin, tout);
        }
    }
}
