using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using GraphVisualizer.GraphView;

namespace GraphVisualizer
{
    internal static class GraphViewConsts
    {
        public static int VertexRadius = 35;
        public static int IdealEdgeLength = 135;
        public static int TriangleSize = 10;
    }
}

namespace GraphVisualizer.ViewModels
{
    public class GraphViewModel : IDrawable
    {
        private IGraphViewer _graphViewer;

        public GraphViewModel()
        {
            _graphViewer = new GraphViewer<int>();
        }

        //public void UpdateGraphViewModelType<T>()
        //    where T : INumber<T>
        //{
        //    _graphViewer = new GraphViewer<T>(_graphViewer);
        //}

        void IDrawable.Draw(ICanvas canvas, RectF dirtyRect)
        {
            _graphViewer?.Draw(canvas, dirtyRect);
        }
    }
}
