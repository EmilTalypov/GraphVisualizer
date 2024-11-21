using System.Numerics;

namespace GraphVisualizer
{
    public class Graph<K, T>
        where T : INumber<T>
        where K : notnull
    {
        private readonly Dictionary<K, int> _vertexIndex = [];
        private readonly List<List<(int, T)>> _graph = [];

        public List<List<(int, T)>> GetGraph()
        {
            return _graph;
        }

        public List<List<(int, T)>> GraphAsList => _graph;

        public int? GetVertexIndex(K vertex)
        {
            return _vertexIndex.TryGetValue(vertex, out var index) ? index : null;
        }

        public void AddVertex(K vertex)
        {
            if (!_vertexIndex.ContainsKey(vertex))
            {
                _vertexIndex.Add(vertex, _vertexIndex.Count);
                _graph.Add([]);
            }
        }

        public void AddDirectEdge(K vertex, K otherVertex, T weight)
        {
            if (!_vertexIndex.ContainsKey(vertex))
            {
                AddVertex(vertex);
            }

            if (!_vertexIndex.ContainsKey(otherVertex))
            {
                AddVertex(otherVertex);
            }

            _vertexIndex.TryGetValue(vertex, out int vertexIndex);
            _vertexIndex.TryGetValue(otherVertex, out int otherVertexIndex);

            _graph[vertexIndex].Add((otherVertexIndex, weight));
        }

        public void AddDirectEdge(K vertex, K otherVertex)
        {
            AddDirectEdge(vertex, otherVertex, T.One);
        }

        public void AddEdge(K vertex, K otherVertex)
        {
            AddDirectEdge(vertex, otherVertex);
            AddDirectEdge(otherVertex, vertex);
        }

        public void AddEdge(K vertex, K otherVertex, T weight)
        {
            AddDirectEdge(vertex, otherVertex, weight);
            AddDirectEdge(otherVertex, vertex, weight);
        }
    }
}
