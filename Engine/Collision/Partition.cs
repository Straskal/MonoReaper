using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Engine.Graphics;

namespace Engine.Collision
{
    /// <summary>
    /// This class is used to organize boxes by position for efficient spatial queries.
    /// </summary>
    public sealed class Partition
    {
        /// <summary>
        /// Store boxes in their respective cell.
        /// </summary>
        private readonly Dictionary<Point, HashSet<Collider>> _cells = new();

        public Partition(int cellSize)
        {
            CellSize = cellSize;
        }

        /// <summary>
        /// Gets the size of the partitions cells.
        /// </summary>
        public int CellSize { get; }

        public void Add(Collider box)
        {
            if (box.PartitionCellPoints.Count != 0)
            {
                throw new InvalidOperationException("Cannot add box that is already added to partition.");
            }

            box.PartitionCellPoints.AddRange(GetCellsForRectangle(box.Bounds));

            foreach (var point in box.PartitionCellPoints)
            {
                GetCellAtPoint(point).Add(box);
            }
        }

        public void Remove(Collider box)
        {
            if (box.PartitionCellPoints.Count == 0)
            {
                throw new InvalidOperationException("Cannot remove box that isn't contained in partition.");
            }

            foreach (var point in box.PartitionCellPoints)
            {
                GetCellAtPoint(point).Remove(box);
            }

            box.PartitionCellPoints.Clear();
        }

        public void Update(Collider box)
        {
            Remove(box);
            Add(box);
        }

        public IEnumerable<Collider> Query(Vector2 position)
        {
            return QueryCells(GetCellsForRectangle(new RectangleF(position.X, position.Y, 1, 1)));
        }

        public IEnumerable<Collider> Query(RectangleF bounds)
        {
            return QueryCells(GetCellsForRectangle(bounds));
        }

        private List<Point> GetCellsForRectangle(RectangleF bounds)
        {
            var result = new List<Point>();
            var min = Vector2.Floor(bounds.TopLeft / CellSize).ToPoint();
            var max = Vector2.Floor(bounds.BottomRight / CellSize).ToPoint();
            var length = max - min;

            for (int y = 0; y <= length.Y; y++)
            {
                for (int x = 0; x <= length.X; x++)
                {
                    result.Add(new Point(min.X + x, min.Y + y));
                }
            }

            return result;
        }

        private HashSet<Collider> GetCellAtPoint(Point point)
        {
            if (!_cells.TryGetValue(point, out var cell))
            {
                _cells[point] = cell = new HashSet<Collider>();
            }

            return cell;
        }

        private IEnumerable<Collider> QueryCells(List<Point> cellPoints)
        {
            var result = new HashSet<Collider>();

            foreach (var point in cellPoints)
            {
                if (_cells.TryGetValue(point, out var cell))
                {
                    result.UnionWith(cell);
                }
            }

            return result;
        }

        internal void DebugDraw(Renderer renderer)
        {
            foreach (var kvp in _cells)
            {
                var row = kvp.Key.Y;
                var col = kvp.Key.X;
                var y = row * CellSize;
                var x = col * CellSize;
                var opacity = kvp.Value.Count > 0 ? 0.8f : 0.05f;

                renderer.DrawRectangleOutline(new Rectangle(x, y, CellSize - 1, CellSize - 1), Color.DarkBlue * opacity);
            }
        }
    }
}
