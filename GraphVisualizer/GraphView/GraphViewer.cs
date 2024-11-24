using System.Diagnostics;
using System.Net;
using System.Numerics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;

namespace GraphVisualizer.GraphView
{
    internal static class MathExtension
    {
        public static float AngleBetween(PointF a, PointF b)
        {
            return (float)Math.Atan2(b.Y - a.Y, b.X - a.X);
        }
    }

    internal interface IGraphViewer
    {
        void AddVertex(string vertex, float x, float y)
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

        void Draw(ICanvas canvas, RectF dirtyRect)
        {
            throw new NotImplementedException();
        }
    }

    internal class GraphViewer<T> : IGraphViewer, IDrawable
        where T : INumber<T>
    {
        private readonly Graph<T> _graph = new();
        private readonly Dictionary<string, VertexViewer> _vertices = [];
        private readonly Dictionary<(string, string), EdgeViewer<T>> _edges = [];

        public GraphViewer()
        {
            AddVertex("a", new PointF(GraphViewConsts.VertexRadius, GraphViewConsts.VertexRadius));
            AddVertex(
                "b",
                new PointF(
                    GraphViewConsts.VertexRadius,
                    3 * GraphViewConsts.VertexRadius + GraphViewConsts.IdealEdgeLength
                )
            );
            AddVertex(
                "1",
                new PointF(
                    3 * GraphViewConsts.VertexRadius + GraphViewConsts.IdealEdgeLength,
                    GraphViewConsts.VertexRadius
                )
            );
            AddVertex(
                "lol",
                new PointF(
                    5 * GraphViewConsts.VertexRadius + 2 * GraphViewConsts.IdealEdgeLength,
                    GraphViewConsts.VertexRadius
                )
            );
            AddVertex(
                "shit",
                new PointF(
                    5 * GraphViewConsts.VertexRadius + 2 * GraphViewConsts.IdealEdgeLength,
                    3 * GraphViewConsts.VertexRadius + GraphViewConsts.IdealEdgeLength
                )
            );

            AddDirectEdge("lol", "shit");
            AddDirectEdge("a", "b");
            AddDirectEdge("b", "1");
            AddDirectEdge("1", "a");
            AddDirectEdge("a", "b");
            AddDirectEdge("a", "b", T.Zero);
            AddDirectEdge("b", "1", -T.One);
            AddDirectEdge("1", "b", T.Zero);
        }

        public void AddVertex(string vertex, PointF center)
        {
            _graph.AddVertex(vertex);
            _vertices.Add(vertex, new VertexViewer(vertex, center));
        }

        public void AddDirectEdge(string vertex, string otherVertex, T weight)
        {
            if (
                !_vertices.TryGetValue(vertex, out VertexViewer? vertexView)
                || !_vertices.TryGetValue(otherVertex, out VertexViewer? otherVertexView)
            )
            {
                throw new Exception(
                    $"Trying to create edge between not existing vertexes {vertex} {otherVertex}"
                );
            }

            _graph.AddDirectEdge(vertex, otherVertex, weight);

            if (!_edges.TryGetValue((vertex, otherVertex), out EdgeViewer<T>? edgeView))
            {
                edgeView = new(vertexView.Center, otherVertexView.Center);
                _edges[(vertex, otherVertex)] = edgeView;
            }

            edgeView.AddMultipleEdge(weight);
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

        public void AddEdge(string vertex, string otherVertex, T weight)
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
