using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer.GraphView
{
    public class EdgeViewer<T>
        where T : INumber<T>
    {
        private readonly PointF _center1;
        private readonly PointF _center2;
        private readonly SortedSet<T> _weights;

        public EdgeViewer(PointF center1, PointF center2)
        {
            _center1 = center1;
            _center2 = center2;
            _weights = [];
        }

        public EdgeViewer(PointF center1, PointF center2, SortedSet<T> weights)
        {
            _center1 = center1;
            _center2 = center2;
            _weights = weights;
        }

        public EdgeViewer<K> Copy<K>()
            where K : INumber<K>
        {
            return new(
                _center1,
                _center2,
                new SortedSet<K>(_weights.Select(weight => K.CreateChecked(weight)))
            );
        }

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

            PointF center1 = _center1;
            PointF center2 = _center2;

            if (existsBackEdge)
            {
                center1.X += dx;
                center1.Y += dy;
            }

            center2.X -= dx;
            center2.Y -= dy;

            canvas.DrawLine(center1, center2);

            center2.X += GraphViewConsts.TriangleSize * MathF.Cos(angle);
            center2.Y += GraphViewConsts.TriangleSize * MathF.Sin(angle);

            canvas.FillPath(GetTrianglePath(center2, angle));
        }
    }
}
