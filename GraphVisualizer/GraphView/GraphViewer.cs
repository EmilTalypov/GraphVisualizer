using System.Diagnostics;
using System.Net;
using System.Numerics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;

namespace GraphVisualizer.GraphView
{
    public static class MathExtension
    {
        public static float AngleBetween(PointF a, PointF b)
        {
            return (float)Math.Atan2(b.Y - a.Y, b.X - a.X);
        }
    }

    public interface IGraphViewer : IDrawable
    {
        GraphViewer<T> Copy<T>()
            where T : INumber<T>
        {
            throw new NotImplementedException();
        }

        void AddVertex(string vertex, PointF center)
        {
            throw new NotImplementedException();
        }

        void AddDirectEdge(string vertex, string otherVertex, object weight)
        {
            throw new NotImplementedException();
        }

        void AddDirectEdge(string vertex, string otherVertex)
        {
            throw new NotImplementedException();
        }

        void AddEdge(string vertex, string otherVertex, object weight)
        {
            throw new NotImplementedException();
        }

        void AddEdge(string vertex, string otherVertex)
        {
            throw new NotImplementedException();
        }
    }

    public class GraphViewer<T> : IGraphViewer
        where T : INumber<T>
    {
        private readonly Graph<T> _graph;
        private readonly Dictionary<string, VertexViewer> _vertices;
        private readonly Dictionary<(string, string), EdgeViewer<T>> _edges;

        public GraphViewer()
        {
            _graph = new();
            _vertices = [];
            _edges = [];
        }

        public GraphViewer(
            Graph<T> graph,
            Dictionary<string, VertexViewer> vertices,
            Dictionary<(string, string), EdgeViewer<T>> edges
        )
        {
            _graph = graph;
            _vertices = vertices;
            _edges = edges;
        }

        public GraphViewer<K> Copy<K>()
            where K : INumber<K>
        {
            return new(
                _graph.Copy<K>(),
                _vertices,
                _edges.Select(edge => (edge.Key, edge.Value.Copy<K>())).ToDictionary()
            );
        }

        public void Clear()
        {
            _graph.Clear();
            _vertices.Clear();
            _edges.Clear();
        }

        public void AddVertex(string vertex, PointF center)
        {
            _graph.AddVertex(vertex);
            _vertices.Add(vertex, new VertexViewer(vertex, center));
        }

        public void AddDirectEdge(string vertex, string otherVertex, object weight)
        {
            if (weight.GetType() != typeof(T))
            {
                throw new ArgumentException(
                    $"Tried to create {weight.GetType()} edge between {vertex}, {otherVertex}"
                );
            }

            if (
                !_vertices.TryGetValue(vertex, out VertexViewer? vertexView)
                || !_vertices.TryGetValue(otherVertex, out VertexViewer? otherVertexView)
            )
            {
                throw new ArgumentException(
                    $"Trying to create edge between not existing vertexes {vertex} {otherVertex}"
                );
            }

            _graph.AddDirectEdge(vertex, otherVertex, (T)weight);

            if (!_edges.TryGetValue((vertex, otherVertex), out EdgeViewer<T>? edgeView))
            {
                edgeView = new(vertexView.Center, otherVertexView.Center);
                _edges[(vertex, otherVertex)] = edgeView;
            }

            edgeView.AddMultipleEdge((T)weight);
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.LightGray;
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 4;
            canvas.Font = new Microsoft.Maui.Graphics.Font("OpenSansRegular");
            canvas.FontSize = 18;
            canvas.FontColor = Colors.Blue;

            foreach (var (names, edge) in _edges)
            {
                edge.Draw(canvas, dirtyRect, _edges.ContainsKey((names.Item2, names.Item1)));
            }

            foreach (var (_, vertex) in _vertices)
            {
                vertex.Draw(canvas, dirtyRect);
            }
        }

        public void AddDirectEdge(string vertex, string otherVertex)
        {
            AddDirectEdge(vertex, otherVertex, T.One);
        }

        public void AddEdge(string vertex, string otherVertex, object weight)
        {
            AddDirectEdge(vertex, otherVertex, weight);
            AddDirectEdge(otherVertex, vertex, weight);
        }

        public void AddEdge(string vertex, string otherVertex)
        {
            AddEdge(vertex, otherVertex, T.One);
        }
    }
}
