using System.Numerics;

namespace GraphVisualizer {
    public enum DijkstraType {
        MIN,
        MAX,
    }

    public static class GraphAlgos {
        public static List<T> Dijkstra<K, T>(Graph<K, T> sourceGraph, K start, DijkstraType type = DijkstraType.MIN) where T : INumber<T> where K : notnull {
            var graph = sourceGraph.GetGraph();
            int? startIndex = sourceGraph.GetVertexIndex(start);

            if (!startIndex.HasValue) {
                throw new Exception($"Can't find start vertex {start}");
            }

            T defaultDistance = T.Zero;
            
            if (type == DijkstraType.MIN) {
                var (sum, existNegativeEdge) = GetEdgesSum(graph);

                if (existNegativeEdge) {
                    throw new Exception("Dijkstra don't work with negative edges");
                } else {
                    defaultDistance = sum;
                }
            }

            List<T> distance = Enumerable.Repeat(defaultDistance, graph.Count).ToList();

            SortedSet<(T, int)> queue = [(T.Zero, startIndex.Value)];
            distance[startIndex.Value] = T.Zero;

            while (queue.Count > 0) {
                var (dist, v) = type == DijkstraType.MIN ? queue.Min() : queue.Max();
                queue.Remove((dist, v));

                foreach (var (u, weight) in graph[v]) {
                    if ((distance[u] > dist + weight && type == DijkstraType.MIN) ||
                        (distance[u] < dist + weight && type == DijkstraType.MAX)) {
                        queue.Remove((distance[u], u));

                        distance[u] = dist + weight;

                        queue.Add((distance[u], u));
                    }
                }
            }

            return distance;
        }

        private static (T, bool) GetEdgesSum<T>(List<List<(int, T)>> graph) where T : INumber<T> {
            Queue<int> queue = new();
            List<bool> visited = Enumerable.Repeat(false, graph.Count).ToList();
   
            T sum = T.Zero;
            bool existNegativeEdge = false;

            visited[0] = true;
            queue.Enqueue(0);

            while (queue.Count > 0) {
                int v = queue.Dequeue();

                foreach (var (u, weight) in graph[v]) {
                    if (!visited[u]) {
                        if (weight < T.Zero) {
                            existNegativeEdge = true;
                        }

                        sum += weight;

                        visited[u] = true;
                        queue.Enqueue(u);
                    }
                }
            }

            return (sum, existNegativeEdge);
        }

        private static void ColorComponent<T>(List<List<(int, T)>> graph, int start, ref List<int> visited, int marker) where T : INumber<T> {
            Queue<int> queue = new();

            queue.Enqueue(start);
            visited[start] = marker;

            while (queue.Count > 0) {
                int v = queue.Dequeue();

                foreach (var (u, _) in graph[v]) {
                    if (visited[u] == 0) {
                        queue.Enqueue(u);
                        visited[u] = marker;
                    }
                }
            }
        }

        public static List<int> BFS<K, T>(Graph<K, T> sourceGraph, K start) where T : INumber<T> where K : notnull {
            var graph = sourceGraph.GetGraph();
            int? startIndex = sourceGraph.GetVertexIndex(start);

            if (!startIndex.HasValue) {
                throw new Exception($"Can't find start vertex {start}");
            }

            Queue<int> queue = new();

            List<int> visitedAt = Enumerable.Repeat(-1, graph.Count).ToList();

            queue.Enqueue(startIndex.Value);
            visitedAt[startIndex.Value] = 0;

            while (queue.Count > 0) {
                int v = queue.Dequeue();

                foreach (var (u, _) in graph[v]) {
                    if (visitedAt[u] == -1) {
                        queue.Enqueue(u);
                        visitedAt[u] = visitedAt[v] + 1;
                    }
                }
            }

            return visitedAt;
        }

        public static List<int> GetComponents<K, T>(Graph<K, T> sourceGraph) where T : INumber<T> where K : notnull {
            var graph = sourceGraph.GetGraph();

            List<int> colorOfComponent = Enumerable.Repeat(0, graph.Count).ToList();

            int color = 1;

            for (int v = 0; v < graph.Count; v++) {
                if (colorOfComponent[v] != 0) {
                    continue;
                }

                ColorComponent(graph, v, ref colorOfComponent, color);

                color += 1;
            }

            return colorOfComponent;
        }

        private static void TinToutDFS<T>(int vertex, ref List<List<(int, T)>> tree, ref List<int> tin, ref List<int> tout, ref int timer) {
            tin[vertex] = timer++;

            foreach (var (u, _) in tree[vertex]) {
                if (tin[u] == -1) {
                    TinToutDFS(u, ref tree, ref tin, ref tout, ref timer);
                }
            }

            tout[vertex] = timer++;
        }

        public static (List<int>, List<int>) GetTinTout<K, T>(Graph<K, T> sourceTree, K root) where T : INumber<T> where K : notnull {
            var tree = sourceTree.GetGraph();
            int? rootIndex = sourceTree.GetVertexIndex(root);

            if (!rootIndex.HasValue) {
                throw new Exception($"Can't find {root} in vertexes");
            }

            List<int> tin = Enumerable.Repeat(-1, tree.Count).ToList();
            List<int> tout = Enumerable.Repeat(-1, tree.Count).ToList();

            int timer = 0;
            TinToutDFS(rootIndex.Value, ref tree, ref tin, ref tout, ref timer);

            return (tin, tout);
        }
    }
}
