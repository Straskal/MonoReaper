using System.Collections.Generic;
using Core.Graphics;
using Microsoft.Xna.Framework;

namespace Core.Collision
{
    public sealed class Partition
    {
        private readonly Dictionary<Point, HashSet<Box>> _cells = new();

        public Partition(int cellSize)
        {
            CellSize = cellSize;
        }

        public int CellSize { get; }

        internal void Add(Box box)
        {
            GetQueryCells(box.CalculateBounds(), box.Points);

            foreach (var point in box.Points)
            {
                GetCellAtPoint(point).Add(box);
            }
        }

        internal void Remove(Box box)
        {
            GetQueryCells(box.CalculateBounds(), box.Points);

            foreach (var point in box.Points)
            {
                GetCellAtPoint(point).Remove(box);
            }
        }

        internal void Update(Box box)
        {
            Remove(box);
            Add(box);
        }

        public IEnumerable<Box> QueryBounds(RectangleF bounds)
        {
            var cellPoints = new List<Point>();
            GetQueryCells(bounds, cellPoints);
            return UnionResults(cellPoints);
        }

        public void QueryBounds(RectangleF bounds, HashSet<Box> queryResults)
        {
            queryResults.Clear();
            var cellPoints = new List<Point>();
            GetQueryCells(bounds, cellPoints);
            GetQueryResults(cellPoints, queryResults);
        }

        private HashSet<Box> GetCellAtPoint(Point point)
        {
            if (!_cells.TryGetValue(point, out var cell))
            {
                cell = new HashSet<Box>();
                _cells[point] = cell;
            }

            return cell;
        }

        private void GetQueryResults(List<Point> cellPoints, HashSet<Box> results)
        {
            foreach (var point in cellPoints)
            {
                if (_cells.TryGetValue(point, out var cell))
                {
                    results.UnionWith(cell);
                }
            }
        }

        private IEnumerable<Box> UnionResults(List<Point> cellPoints)
        {
            var results = new HashSet<Box>();

            foreach (var point in cellPoints)
            {
                if (_cells.TryGetValue(point, out var cell))
                {
                    results.UnionWith(cell);
                }
            }

            return results;
        }

        private void GetQueryCells(RectangleF bounds, List<Point> cells)
        {
            var min = Vector2.Floor(bounds.TopLeft / CellSize).ToPoint();
            var max = Vector2.Floor(bounds.BottomRight / CellSize).ToPoint();
            var length = max - min;

            for (int i = 0; i <= length.Y; i++)
            {
                for (int j = 0; j <= length.X; j++)
                {
                    cells.Add(new Point(min.X + j, min.Y + i));
                }
            }
        }

        internal void DebugDraw()
        {
            // Draw grid cells
            foreach (var kvp in _cells)
            {
                var row = kvp.Key.Y;
                var col = kvp.Key.X;
                var y = row * CellSize;
                var x = col * CellSize;
                var opacity = kvp.Value.Count > 0 ? 0.8f : 0.05f;

                Renderer.DrawRectangleOutline(new Rectangle(x, y, CellSize - 1, CellSize - 1), Color.DarkGray * opacity);
            }

            // Draw individual colliders.
            foreach (var kvp in _cells)
            {
                foreach (var box in kvp.Value)
                {
                    Color color;
                    color = Color.White * 1f;
                    Renderer.DrawRectangleOutline(box.CalculateBounds().ToXnaRect(), color);
                }
            }
        }
    }
}
