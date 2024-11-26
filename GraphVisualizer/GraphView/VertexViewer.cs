using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics.Text;

namespace GraphVisualizer.GraphView
{
    public class VertexViewer(string name, PointF center) : IDrawable
    {
        private readonly string _name = name;
        private readonly PointF _center = center;

        public PointF Center
        {
            get { return _center; }
        }

        public void Draw(ICanvas canvas, RectF _)
        {
            canvas.FillCircle(_center, GraphViewConsts.VertexRadius);
            canvas.DrawString(
                _name,
                _center.X - GraphViewConsts.VertexRadius,
                _center.Y - GraphViewConsts.VertexRadius / 2.0f,
                GraphViewConsts.VertexRadius * 2,
                GraphViewConsts.VertexRadius,
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                TextFlow.OverflowBounds
            );
        }
    }
}
