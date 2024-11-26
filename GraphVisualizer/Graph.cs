using System.Collections.Generic;
using System.Numerics;

namespace GraphVisualizer
{
    public class Graph<T>
        where T : INumber<T>
    {
        private readonly Dictionary<string, int> _vertexIndex;
        private readonly List<List<(int, T)>> _graph;
        private readonly List<string> _vertexName;

        private int _vertexCounter;
        private T _edgesSum;

        public List<List<(int, T)>> GraphAsList
        {
            get { return _graph; }
        }

        public T EdgesSum
        {
            get { return _edgesSum; }
        }

        public static event Action EdgesSumOverflow;

        public Graph()
        {
            _vertexIndex = [];
            _graph = [];
            _vertexName = [];
            _vertexCounter = 0;
            _edgesSum = T.Zero;
        }

        public Graph(
            Dictionary<string, int> vertexIndex,
            List<List<(int, T)>> graph,
            List<string> vertexName,
            int vertexCounter,
            T edgesSum
        )
        {
            _vertexIndex = vertexIndex;
            _graph = graph;
            _vertexName = vertexName;
            _vertexCounter = vertexCounter;
            _edgesSum = edgesSum;
        }

        public Graph<K> Copy<K>()
            where K : INumber<K>
        {
            return new(
                _vertexIndex,
                _graph
                    .Select(edges =>
                        edges.Select(edge => (edge.Item1, K.CreateChecked(edge.Item2))).ToList()
                    )
                    .ToList(),
                _vertexName,
                _vertexCounter,
                K.CreateChecked(_edgesSum)
            );
        }

        public void Clear()
        {
            _vertexIndex.Clear();
            _graph.Clear();
            _vertexName.Clear();
            _vertexCounter = 0;
            _edgesSum = T.Zero;
        }

        public int? GetVertexIndex(string vertex)
        {
            return _vertexIndex.TryGetValue(vertex, out var index) ? index : null;
        }

        public bool IsTree(string root)
        {
            List<int> componentColor = GraphAlgos.GetComponents(this);
            List<int> visitedAt = GraphAlgos.BFS(this, root);

            bool isConnected = componentColor.All(color => color == 1);
            bool visitedAll = visitedAt.All(visitedAt => visitedAt != -1);

            return isConnected && visitedAll;
        }

        public void AddVertex(string vertex)
        {
            if (_vertexIndex.ContainsKey(vertex))
            {
                return;
            }

            _vertexIndex.Add(vertex, _vertexCounter);
            _graph.Add([]);
            _vertexName.Add(vertex);

            _vertexCounter += 1;
        }

        public void AddDirectEdge(string vertex, string otherVertex, T weight)
        {
            int? vertexIndex = GetVertexIndex(vertex);
            int? otherVertexIndex = GetVertexIndex(otherVertex);

            if (!vertexIndex.HasValue || !otherVertexIndex.HasValue)
            {
                throw new Exception(
                    $"Tried to add edge between non existing vertexs {vertex}, {otherVertex}"
                );
            }

            _graph[vertexIndex.Value].Add((otherVertexIndex.Value, weight));

            try
            {
                checked
                {
                    _edgesSum += weight;
                }
            }
            catch (OverflowException ex)
            {
                EdgesSumOverflow.Invoke();
            }
        }

        public void DeleteVertex(string vertex)
        {
            int? vertexIndex = GetVertexIndex(vertex);

            if (!vertexIndex.HasValue)
            {
                throw new Exception($"Tried to delete non existing vertex {vertex}");
            }

            foreach (var (u, weight) in _graph[vertexIndex.Value])
            {
                List<(int, T)> backEdges = _graph[u]
                    .Where(edge => edge.Item1 == vertexIndex.Value)
                    .ToList();

                foreach (var (v, backWeight) in backEdges)
                {
                    DeleteDirectEdge(_vertexName[u], vertex, backWeight);
                }
            }

            _graph[vertexIndex.Value].Clear();
        }

        public void DeleteDirectEdge(string vertex, string otherVertex, T weight)
        {
            int? vertexIndex = GetVertexIndex(vertex);
            int? otherVertexIndex = GetVertexIndex(otherVertex);

            if (!vertexIndex.HasValue || !otherVertexIndex.HasValue)
            {
                throw new Exception(
                    $"Tried to delete edge between non existing vertexs {vertex} {otherVertex}"
                );
            }

            _graph[vertexIndex.Value].Remove((otherVertexIndex.Value, weight));
        }

        public void DeleteEdges(string vertex, string otherVertex)
        {
            int? vertexIndex = GetVertexIndex(vertex);
            int? otherVertexIndex = GetVertexIndex(otherVertex);

            if (!vertexIndex.HasValue || !otherVertexIndex.HasValue)
            {
                throw new Exception(
                    $"Tried to delete edges between non existing vertexs {vertex} {otherVertex}"
                );
            }

            _graph[vertexIndex.Value].RemoveAll(edge => edge.Item1 == otherVertexIndex.Value);
            _graph[otherVertexIndex.Value].RemoveAll(edge => edge.Item1 == vertexIndex.Value);
        }

        public void AddDirectEdge(string vertex, string otherVertex)
        {
            AddDirectEdge(vertex, otherVertex, T.One);
        }

        public void AddEdge(string vertex, string otherVertex)
        {
            AddDirectEdge(vertex, otherVertex, T.One);
            AddDirectEdge(otherVertex, vertex, T.One);
        }

        public void AddEdge(string vertex, string otherVertex, T weight)
        {
            AddDirectEdge(vertex, otherVertex, weight);
            AddDirectEdge(otherVertex, vertex, weight);
        }
    }
}
