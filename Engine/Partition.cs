using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Engine
{
    public sealed class Partition
    {
        private readonly Dictionary<Point, List<Collider>> cells = new();
        private readonly int cellSize;
        private readonly float inverseCellSize;

        public Partition(int cellSize)
        {
            this.cellSize = cellSize;
            inverseCellSize = 1f / cellSize;
        }

        public void Add(Collider collider)
        {
            if (collider.cells.Count == 0)
            {
                foreach (var point in GetIntersectingCells(collider.Bounds))
                {
                    collider.cells.Add(point);
                    GetOrCreateCellAtPoint(point).Add(collider);
                }
            }
        }

        public void Remove(Collider collider)
        {
            if (collider.cells.Count != 0)
            {
                foreach (var point in collider.cells)
                {
                    GetOrCreateCellAtPoint(point).Remove(collider);
                }

                collider.cells.Clear();
            }
        }

        public void Update(Collider box)
        {
            Remove(box);
            Add(box);
        }

        internal void Clear() 
        {
            foreach (var kvp in cells) 
            {
                foreach (var collider in kvp.Value) 
                {
                    collider.cells.Clear();
                }
            }

            cells.Clear();
        }

        public IEnumerable<Collider> Query(Vector2 point)
        {
            foreach (var collider in GetCellAtPoint(point))
            {
                if (collider.OverlapPoint(point))
                {
                    yield return collider;
                }
            }
        }

        public IEnumerable<Collider> Query(CircleF circle)
        {
            foreach (var collider in QueryCells(GetIntersectingCells(circle.GetBounds())))
            {
                if (collider.OverlapCircle(circle))
                {
                    yield return collider;
                }
            }
        }

        public IEnumerable<Collider> Query(RectangleF bounds)
        {
            foreach (var collider in QueryCells(GetIntersectingCells(bounds))) 
            {
                if (collider.OverlapRectangle(bounds)) 
                {
                    yield return collider;
                }
            }
        }

        private List<Point> GetIntersectingCells(RectangleF bounds)
        {
            var result = new List<Point>();
            var min = Vector2.Floor(bounds.TopLeft * inverseCellSize).ToPoint();
            var max = Vector2.Floor(bounds.BottomRight * inverseCellSize).ToPoint();
            var length = max - min;
            var current = new Point();

            for (int y = 0; y <= length.Y; y++)
            {
                for (int x = 0; x <= length.X; x++)
                {
                    current.X = min.X + x;
                    current.Y = min.Y + y;
                    result.Add(current);
                }
            }

            return result;
        }

        private List<Collider> GetCellAtPoint(Vector2 point)
        {
            point = Vector2.Floor(point * inverseCellSize);

            if (cells.TryGetValue(point.ToPoint(), out var cell))
            {
                return cell;
            }

            return new List<Collider>();
        }

        private List<Collider> GetOrCreateCellAtPoint(Point point)
        {
            if (!cells.TryGetValue(point, out var cell))
            {
                cells[point] = cell = new List<Collider>();
            }

            return cell;
        }

        private IEnumerable<Collider> QueryCells(List<Point> cellPoints)
        {
            var result = new HashSet<Collider>();

            foreach (var point in cellPoints)
            {
                if (cells.TryGetValue(point, out var cell))
                {
                    result.UnionWith(cell);
                }
            }

            return result;
        }

        internal void DebugDraw(Renderer renderer)
        {
            foreach (var kvp in cells)
            {
                var row = kvp.Key.Y;
                var col = kvp.Key.X;
                var y = row * cellSize;
                var x = col * cellSize;
                var opacity = kvp.Value.Count > 0 ? 0.8f : 0.05f;

                renderer.DrawRectangleOutline(new Rectangle(x, y, cellSize - 1, cellSize - 1), Color.DarkBlue * opacity);
            }
        }
    }
}
