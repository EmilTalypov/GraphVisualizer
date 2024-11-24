using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer.GraphView
{
    internal class EdgeViewer<T>(PointF center1, PointF center2)
        where T : INumber<T>
    {
        private readonly PointF _center1 = center1;
        private readonly PointF _center2 = center2;
        private readonly SortedSet<T> _weights = [];

        public void AddMultipleEdge(T weight)
        {
            _weights.Add(weight);
        }

        public static PathF GetTrianglePath(PointF edgeEnd, float angle)
        {
            PathF triangle = new();

            PointF baseMidpoint = new(
                edgeEnd.X - 2 * GraphViewConsts.TriangleSize * MathF.Cos(angle),
                edgeEnd.Y - 2 * GraphViewConsts.TriangleSize * MathF.Sin(angle)
            );

            PointF baseLeft = new(
                baseMidpoint.X + GraphViewConsts.TriangleSize * MathF.Cos(angle + MathF.PI / 2),
                baseMidpoint.Y + GraphViewConsts.TriangleSize * MathF.Sin(angle + MathF.PI / 2)
            );

            PointF baseRight = new(
                baseMidpoint.X + GraphViewConsts.TriangleSize * MathF.Cos(angle - MathF.PI / 2),
                baseMidpoint.Y + GraphViewConsts.TriangleSize * MathF.Sin(angle - MathF.PI / 2)
            );

            triangle.MoveTo(edgeEnd);
            triangle.LineTo(baseLeft);
            triangle.LineTo(baseRight);
            triangle.LineTo(edgeEnd);

            return triangle;
        }

        public void Draw(ICanvas canvas, RectF _, bool existsBackEdge)
        {
            PointF _center1 = this._center1;
            PointF _center2 = this._center2;

            float angle = MathExtension.AngleBetween(_center1, _center2);

            // drawing edge weights

            string weightsString = _weights.Count switch
            {
                1 => $"{_weights.First()}",
                2 => $"{_weights.Min()}, {_weights.Max()}",
                _ => $"{_weights.Min()},..., {_weights.Max()}",
            };

            PointF midpoint = new((_center1.X + _center2.X) / 2, (_center1.Y + _center2.Y) / 2);

            canvas.SaveState();

            canvas.Translate(
                midpoint.X - (angle > 0 ? 20 : 11) * MathF.Cos(angle + MathF.PI / 2),
                midpoint.Y - (angle > 0 ? 20 : 11) * MathF.Sin(angle + MathF.PI / 2)
            );

            canvas.Rotate((angle + (angle > 0 ? MathF.PI : 0)) * 180 / MathF.PI);

            canvas.FontColor = Colors.LightGray;
            canvas.DrawString(weightsString, 0, 0, HorizontalAlignment.Center);

            canvas.RestoreState();

            // drawing arrow

            float dx =
                (GraphViewConsts.VertexRadius + GraphViewConsts.TriangleSize) * MathF.Cos(angle);
            float dy =
                (GraphViewConsts.VertexRadius + GraphViewConsts.TriangleSize) * MathF.Sin(angle);

            if (existsBackEdge)
            {
                _center1.X += dx;
                _center1.Y += dy;
            }

            _center2.X -= dx;
            _center2.Y -= dy;

            canvas.DrawLine(_center1, _center2);

            _center2.X += GraphViewConsts.TriangleSize * MathF.Cos(angle);
            _center2.Y += GraphViewConsts.TriangleSize * MathF.Sin(angle);

            canvas.FillPath(EdgeViewer<T>.GetTrianglePath(_center2, angle));
        }
    }
}
