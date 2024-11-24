using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer.GraphView
{
    internal class VertexViewer(string name, PointF center) : IDrawable
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
            canvas.DrawString(_name, _center.X, _center.Y + 4.5f, HorizontalAlignment.Center);
        }
    }
}
