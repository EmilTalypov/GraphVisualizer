using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphVisualizer;
using Xunit;

#pragma warning disable CS8629
namespace UnitTests
{
    public class DijkstraTests
    {
        [Fact]
        public void Test1()
        {
            Graph<int, int> graph = new();

            graph.AddEdge(1, 2, 3);
            graph.AddEdge(2, 3, 4);
            graph.AddEdge(1, 3, 2);

            var distance = GraphAlgos.Dijkstra(graph, 1);

            Assert.Equal(0, distance[graph.GetVertexIndex(1).Value]);
            Assert.Equal(3, distance[graph.GetVertexIndex(2).Value]);
            Assert.Equal(2, distance[graph.GetVertexIndex(3).Value]);
        }

        [Fact]
        public void Test2()
        {
            Graph<string, int> graph = new();

            graph.AddEdge("a", "b", 3);
            graph.AddEdge("b", "c", 4);
            graph.AddEdge("a", "c", 2);

            var distance = GraphAlgos.Dijkstra(graph, "a");

            Assert.Equal(0, distance[graph.GetVertexIndex("a").Value]);
            Assert.Equal(3, distance[graph.GetVertexIndex("b").Value]);
            Assert.Equal(2, distance[graph.GetVertexIndex("c").Value]);
        }

        [Fact]
        public void Test3()
        {
            Graph<string, int> graph = new();

            graph.AddEdge("A", "B", 3);
            graph.AddEdge("B", "C", 4);
            graph.AddEdge("B", "C", 2);

            try
            {
                var distance = GraphAlgos.Dijkstra(graph, "abc");
            }
            catch (Exception ex)
            {
                Assert.Equal("Can't find start vertex abc", ex.Message);
            }
        }

        [Fact]
        public void Test4()
        {
            Graph<int, int> graph = new();

            graph.AddEdge(1, 3, 5);
            graph.AddEdge(1, 9, 6);
            graph.AddEdge(2, 4, 5);
            graph.AddEdge(2, 10, 3);
            graph.AddEdge(3, 4, 7);
            graph.AddEdge(3, 7, 4);
            graph.AddEdge(4, 10, 7);
            graph.AddEdge(5, 8, 7);
            graph.AddEdge(5, 9, 5);
            graph.AddEdge(6, 9, 3);
            graph.AddEdge(6, 10, 2);
            graph.AddEdge(8, 10, 6);

            var distance = GraphAlgos.Dijkstra(graph, 8);

            Assert.Equal(0, distance[graph.GetVertexIndex(8).Value]);
            Assert.Equal(6, distance[graph.GetVertexIndex(10).Value]);
            Assert.Equal(7, distance[graph.GetVertexIndex(5).Value]);
            Assert.Equal(8, distance[graph.GetVertexIndex(6).Value]);
            Assert.Equal(9, distance[graph.GetVertexIndex(2).Value]);
            Assert.Equal(13, distance[graph.GetVertexIndex(4).Value]);
            Assert.Equal(11, distance[graph.GetVertexIndex(9).Value]);
            Assert.Equal(17, distance[graph.GetVertexIndex(1).Value]);
            Assert.Equal(20, distance[graph.GetVertexIndex(3).Value]);
            Assert.Equal(24, distance[graph.GetVertexIndex(7).Value]);
        }

        [Fact]
        public void Test5()
        {
            Graph<int, int> graph = new();

            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(1, 4);
            graph.AddEdge(1, 5);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(3, 5);

            var distance = GraphAlgos.Dijkstra(graph, 2);

            Assert.Equal(0, distance[graph.GetVertexIndex(2).Value]);
            Assert.Equal(1, distance[graph.GetVertexIndex(1).Value]);
            Assert.Equal(1, distance[graph.GetVertexIndex(3).Value]);
            Assert.Equal(2, distance[graph.GetVertexIndex(4).Value]);
            Assert.Equal(2, distance[graph.GetVertexIndex(5).Value]);
        }

        [Fact]
        public void Test6()
        {
            Graph<int, int> graph = new();

            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3, -1);
            graph.AddEdge(1, 4);
            graph.AddEdge(1, 5);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(3, 5);

            try
            {
                var distance = GraphAlgos.Dijkstra(graph, 2);
            }
            catch (Exception ex)
            {
                Assert.Equal("Dijkstra don't work with negative edges", ex.Message);
            }
        }
    }

    public class BFSTests
    {
        [Fact]
        public void Test1()
        {
            Graph<int, int> graph = new();

            graph.AddEdge(1, 4);
            graph.AddEdge(1, 5);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 5);
            graph.AddEdge(4, 5);

            var visitedAt = GraphAlgos.BFS(graph, 4);

            Assert.Equal(0, visitedAt[graph.GetVertexIndex(4).Value]);
            Assert.Equal(1, visitedAt[graph.GetVertexIndex(5).Value]);
            Assert.Equal(1, visitedAt[graph.GetVertexIndex(1).Value]);
            Assert.Equal(2, visitedAt[graph.GetVertexIndex(3).Value]);
            Assert.Equal(3, visitedAt[graph.GetVertexIndex(2).Value]);
        }

        [Fact]
        public void Test2()
        {
            Graph<string, int> graph = new();

            graph.AddEdge("a", "b");
            graph.AddEdge("a", "c");
            graph.AddEdge("a", "d");
            graph.AddEdge("b", "c");
            graph.AddEdge("b", "d");
            graph.AddVertex("e");
            graph.AddVertex("f");

            var visitedAt = GraphAlgos.BFS(graph, "d");

            Assert.Equal(0, visitedAt[graph.GetVertexIndex("d").Value]);
            Assert.Equal(1, visitedAt[graph.GetVertexIndex("b").Value]);
            Assert.Equal(1, visitedAt[graph.GetVertexIndex("a").Value]);
            Assert.Equal(2, visitedAt[graph.GetVertexIndex("c").Value]);
            Assert.Equal(-1, visitedAt[graph.GetVertexIndex("e").Value]);
            Assert.Equal(-1, visitedAt[graph.GetVertexIndex("f").Value]);
        }
    }

    public class GetComponentsTests
    {
        [Fact]
        public void Test1()
        {
            Graph<int, int> graph = new();

            graph.AddEdge(1, 5);
            graph.AddEdge(2, 6);
            graph.AddEdge(3, 6);
            graph.AddEdge(3, 10);
            graph.AddEdge(4, 5);
            graph.AddEdge(6, 7);
            graph.AddEdge(6, 8);
            graph.AddVertex(9);

            var componentColor = GraphAlgos.GetComponents(graph);

            int first = componentColor[graph.GetVertexIndex(1).Value];
            int second = componentColor[graph.GetVertexIndex(2).Value];
            int third = componentColor[graph.GetVertexIndex(9).Value];

            Assert.Equal(first, componentColor[graph.GetVertexIndex(1).Value]);
            Assert.Equal(second, componentColor[graph.GetVertexIndex(2).Value]);
            Assert.Equal(second, componentColor[graph.GetVertexIndex(3).Value]);
            Assert.Equal(first, componentColor[graph.GetVertexIndex(4).Value]);
            Assert.Equal(first, componentColor[graph.GetVertexIndex(5).Value]);
            Assert.Equal(second, componentColor[graph.GetVertexIndex(6).Value]);
            Assert.Equal(second, componentColor[graph.GetVertexIndex(7).Value]);
            Assert.Equal(second, componentColor[graph.GetVertexIndex(8).Value]);
            Assert.Equal(third, componentColor[graph.GetVertexIndex(9).Value]);
            Assert.Equal(second, componentColor[graph.GetVertexIndex(10).Value]);
        }
    }

    public class GetTinToutTests
    {
        [Fact]
        public void Test1()
        {
            Graph<int, int> graph = new();

            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 5);
            graph.AddEdge(2, 6);
            graph.AddEdge(3, 7);
            graph.AddEdge(3, 8);

            var (tin, tout) = GraphAlgos.GetTinTout(graph, 1);

            Assert.Equal(0, tin[graph.GetVertexIndex(1).Value]);
            Assert.Equal(1, tin[graph.GetVertexIndex(2).Value]);
            Assert.Equal(2, tin[graph.GetVertexIndex(4).Value]);
            Assert.Equal(3, tout[graph.GetVertexIndex(4).Value]);
            Assert.Equal(4, tin[graph.GetVertexIndex(5).Value]);
            Assert.Equal(5, tout[graph.GetVertexIndex(5).Value]);
            Assert.Equal(6, tin[graph.GetVertexIndex(6).Value]);
            Assert.Equal(7, tout[graph.GetVertexIndex(6).Value]);
            Assert.Equal(8, tout[graph.GetVertexIndex(2).Value]);
            Assert.Equal(9, tin[graph.GetVertexIndex(3).Value]);
            Assert.Equal(10, tin[graph.GetVertexIndex(7).Value]);
            Assert.Equal(11, tout[graph.GetVertexIndex(7).Value]);
            Assert.Equal(12, tin[graph.GetVertexIndex(8).Value]);
            Assert.Equal(13, tout[graph.GetVertexIndex(8).Value]);
            Assert.Equal(14, tout[graph.GetVertexIndex(3).Value]);
            Assert.Equal(15, tout[graph.GetVertexIndex(1).Value]);
        }
    }
}
