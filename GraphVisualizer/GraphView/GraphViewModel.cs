using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using GraphVisualizer.GraphView;

namespace GraphVisualizer.GraphView
{
    internal static class GraphViewConsts
    {
        public static int VertexRadius = 35;
        public static int IdealEdgeLength = 135;
        public static int TriangleSize = 10;
    }

    public class GraphViewModel : IDrawable
    {
        private IGraphViewer _graphViewer;
        private Type _graphType;

        public GraphViewModel()
        {
            _graphViewer = new GraphViewer<int>();
            _graphType = typeof(int);

            Graph<int>.EdgesSumOverflow += SwitchGraphType<long>;

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
                ":3",
                new PointF(
                    5 * GraphViewConsts.VertexRadius + 2 * GraphViewConsts.IdealEdgeLength,
                    3 * GraphViewConsts.VertexRadius + GraphViewConsts.IdealEdgeLength
                )
            );

            AddDirectEdge("lol", ":3");
            AddDirectEdge("a", "b");
            AddDirectEdge("b", "1");
            AddDirectEdge("1", "a");
            AddDirectEdge("a", "b");
            AddDirectEdge("a", "b", 1);
            AddDirectEdge("b", "1", -2);
            AddDirectEdge("1", "b", 0);
        }

        public void SwitchGraphType<T>()
            where T : INumber<T>
        {
            if (typeof(T) != typeof(int) && typeof(T) != typeof(long) && typeof(T) != typeof(float))
            {
                throw new ArgumentException($"Tried to switch to {typeof(T).Name} graph");
            }

            _graphViewer = _graphViewer.Copy<T>();
            _graphType = typeof(T);
        }

        public void AddVertex(string name, PointF center)
        {
            _graphViewer.AddVertex(name, center);
        }

        public void AddDirectEdge<T>(string vertex, string otherVertex, T weight)
            where T : INumber<T>
        {
            if (typeof(T) != _graphType)
            {
                throw new ArgumentException(
                    $"Tried to add {typeof(T).Name} edge while graph is {_graphType.Name}"
                );
            }

            _graphViewer.AddDirectEdge(vertex, otherVertex, weight);
        }

        public void AddEdge<T>(string vertex, string otherVertex, T weight)
            where T : INumber<T>
        {
            AddDirectEdge(vertex, otherVertex, weight);
            AddDirectEdge(otherVertex, vertex, weight);
        }

        public void AddDirectEdge(string vertex, string otherVertex)
        {
            if (_graphType == typeof(int))
            {
                AddDirectEdge(vertex, otherVertex, 1);
            }
            else if (_graphType == typeof(long))
            {
                AddDirectEdge(vertex, otherVertex, (long)1);
            }
            else
            {
                AddDirectEdge(vertex, otherVertex, 1.0f);
            }
        }

        public void AddEdge(string vertex, string otherVertex)
        {
            AddDirectEdge(vertex, otherVertex);
            AddDirectEdge(otherVertex, vertex);
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            _graphViewer.Draw(canvas, dirtyRect);
        }
    }
}
