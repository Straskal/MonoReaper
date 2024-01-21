using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine
{
    public sealed class Partition
    {
        private readonly Dictionary<Point, List<Collider>> cells = new();
        private readonly int cellSize;
        private readonly float inverseCellSize;

        private readonly List<Point> cellResults = new();
        private readonly HashSet<Collider> unionResults = new();
        private readonly List<Collider> queryResults = new();

        public Partition(int cellSize)
        {
            this.cellSize = cellSize;
            inverseCellSize = 1f / cellSize;
        }

        public void Add(Collider collider)
        {
            if (collider.cells.Count == 0)
            {
                foreach (var point in GetOverlappingCells(collider.Bounds))
                {
                    collider.cells.Add(point);
                    AddColliderToCell(collider, point);
                }
            }
        }

        public void Remove(Collider collider)
        {
            if (collider.cells.Count != 0)
            {
                foreach (var point in collider.cells)
                {
                    RemoveColliderFromCell(collider, point);
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
            queryResults.Clear();

            var cellPoint = point * inverseCellSize;
            cellPoint.Floor();

            if (cells.TryGetValue(cellPoint.ToPoint(), out var cell))
            {
                foreach (var collider in cell)
                {
                    if (collider.OverlapPoint(point))
                    {
                        queryResults.Add(collider);
                    }
                }
            }

            return queryResults;
        }

        public IEnumerable<Collider> Query(CircleF circle)
        {
            queryResults.Clear();

            foreach (var collider in QueryCells(GetOverlappingCells(circle.GetBounds())))
            {
                if (collider.OverlapCircle(circle))
                {
                    queryResults.Add(collider);
                }
            }

            return queryResults;
        }

        public IEnumerable<Collider> Query(RectangleF bounds)
        {
            queryResults.Clear();

            foreach (var collider in QueryCells(GetOverlappingCells(bounds))) 
            {
                if (collider.OverlapRectangle(bounds)) 
                {
                    queryResults.Add(collider);
                }
            }

            return queryResults;
        }

        private void AddColliderToCell(Collider collider, in Point point)
        {
            if (!cells.TryGetValue(point, out var cell))
            {
                cells[point] = cell = new List<Collider>();
            }

            cell.Add(collider);
        }

        private void RemoveColliderFromCell(Collider collider, in Point point)
        {
            if (cells.TryGetValue(point, out var cell))
            {
                if (cell.Remove(collider))
                {
                    if (cell.Count == 0)
                    {
                        cells.Remove(point);
                    }
                }
            }
        }

        private List<Point> GetOverlappingCells(RectangleF bounds)
        {
            cellResults.Clear();

            var minF = bounds.TopLeft * inverseCellSize;
            var maxF = bounds.BottomRight * inverseCellSize;
            minF.Floor();
            maxF.Floor();

            var min = minF.ToPoint();
            var max = maxF.ToPoint();
            var len = max - min;

            for (int y = 0; y <= len.Y; y++)
            {
                for (int x = 0; x <= len.X; x++)
                {
                    cellResults.Add(new Point(min.X + x, min.Y + y));
                }
            }

            return cellResults;
        }

        private IEnumerable<Collider> QueryCells(List<Point> cellPoints)
        {
            unionResults.Clear();

            foreach (var point in cellPoints)
            {
                if (cells.TryGetValue(point, out var cell))
                {
                    unionResults.UnionWith(cell);
                }
            }

            return unionResults;
        }

        internal void DebugDraw(Renderer renderer)
        {
            foreach (var kvp in cells)
            {
                var y = kvp.Key.Y * cellSize;
                var x = kvp.Key.X * cellSize;

                renderer.DrawRectangleOutline(new Rectangle(x, y, cellSize - 1, cellSize - 1), Color.DarkBlue);
            }

            foreach (var kvp in cells)
            {
                foreach (var collider in kvp.Value) 
                {
                    collider.Draw(renderer);
                }
            }
        }
    }
}
