using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Core.Graphics;

namespace Core.Collision
{
    public sealed class Partition
    {
        // Each corner (x4) can exist in a different cell.
        public const int MAX_BOUNDS_BUCKETS = 4;

        private readonly List<Box>[] _cells;

        private readonly int _cellSize;
        private readonly int _width;
        private readonly int _height;
        private readonly int _length;

        internal Partition(int cellSize, int width, int height)
        {
            _cellSize = cellSize;
            _width = (int)Math.Ceiling((double)width / cellSize);
            _height = (int)Math.Ceiling((double)height / cellSize);

            _length = _width * _height;
            _cells = new List<Box>[_length];

            for (int i = 0; i < _length; i++)
            {
                _cells[i] = new List<Box>();
            }
        }

        internal void Add(Box box)
        {
            var bounds = box.CalculateBounds();
            var cells = box.PartitionCells;
            var length = GetOccupyingCells(bounds, cells);

            for (int i = 0; i < length; i++)
            {
                if (cells[i] >= 0 && cells[i] < _cells.Length)
                {
                    _cells[cells[i]].Add(box);
                }
            }

            box.PartitionCellCount = length;
        }

        internal void Remove(Box box)
        {
            var cellCount = box.PartitionCellCount;
            var cells = box.PartitionCells;

            for (int i = 0; i < cellCount; i++)
            {
                if (cells[i] >= 0 && cells[i] < _cells.Length)
                {
                    _cells[cells[i]].Remove(box);
                }
            }
        }

        internal void Update(Box box)
        {
            Remove(box);
            Add(box);
        }

        public IEnumerable<Box> QueryBounds(RectangleF bounds)
        {
            return QueryCells(bounds);
        }

        internal void DebugDraw()
        {
            const float opacity = 0.3f;

            // Draw grid cells.
            for (int i = 0; i < _length; i++)
            {
                var row = i / _width;
                var col = i % _width;
                var x = col * _cellSize;
                var y = row * _cellSize;

                Renderer.DrawRectangle(new Rectangle(x, y, _cellSize - 1, _cellSize - 1), Color.Purple * opacity);
            }

            // Draw individual colliders.
            for (int i = 0; i < _length; i++)
            {
                foreach (var obj in _cells[i])
                {
                    Color color;

                    if (obj.IsSolid)
                    {
                        color = Color.Red * opacity;
                    }
                    else 
                    {
                        color = Color.Blue* opacity;
                    }

                    Renderer.DrawRectangle(obj.CalculateBounds().ToXnaRect(), color);
                }
            }
        }

        private int Add(Box box, Span<int> cells)
        {
            var length = GetOccupyingCells(box.CalculateBounds(), cells);

            for (int i = 0; i < length; i++)
            {
                if (cells[i] >= 0 && cells[i] < this._cells.Length)
                {
                    _cells[cells[i]].Add(box);
                }
            }

            return length;
        }

        private void Remove(Box box, Vector2 previousPosition, Span<int> buckets)
        {
            var bounds = Offset.GetRect(box.Entity.Origin, previousPosition.X, previousPosition.Y, box.Width, box.Height);
            var length = GetOccupyingCells(bounds, buckets);

            for (int i = 0; i < length; i++)
            {
                if (buckets[i] >= 0 && buckets[i] < this._cells.Length)
                {
                    _cells[buckets[i]].Remove(box);
                }
            }
        }

        private IEnumerable<Box> QueryCells(RectangleF bounds)
        {
            var results = new HashSet<Box>();
            Span<int> buckets = stackalloc int[MAX_BOUNDS_BUCKETS];
            var length = GetOccupyingCells(bounds, buckets);

            for (int i = 0; i < length; i++)
            {
                if (buckets[i] >= 0 && buckets[i] < this._cells.Length)
                {
                    results.UnionWith(_cells[buckets[i]]);
                }
            }

            return results;
        }

        private int GetOccupyingCells(RectangleF bounds, Span<int> buckets)
        {
            var resultLength = 0;

            // Calculate the bucket index for each corner of bounds.
            var y = new Vector4(bounds.Top, bounds.Top, bounds.Bottom, bounds.Bottom);
            var x = new Vector4(bounds.Left, bounds.Right, bounds.Left, bounds.Right);

            var row = y / _cellSize;
            var col = x / _cellSize;

            row.Floor();
            col.Floor();

            var index = row * _width + col;

            // Create temp collections for finding distinct bucket indexes.
            Span<int> bucketIndexes = stackalloc int[MAX_BOUNDS_BUCKETS];
            bucketIndexes[0] = (int)index.X;
            bucketIndexes[1] = (int)index.Y;
            bucketIndexes[2] = (int)index.Z;
            bucketIndexes[3] = (int)index.W;

            Span<int> visitedIndexes = stackalloc int[MAX_BOUNDS_BUCKETS];
            visitedIndexes.Fill(0);

            bool visited;
            int bi;

            // Only add distinct bucket indexes to result.
            for (int i = 0; i < bucketIndexes.Length; i++)
            {
                visited = false;
                bi = bucketIndexes[i];

                for (int j = 0; j < resultLength; j++)
                {
                    if (visitedIndexes[j] == bi)
                    {
                        visited = true;
                        break;
                    }
                }

                if (!visited)
                {
                    visitedIndexes[resultLength] = bi;
                    buckets[resultLength] = bi;
                    resultLength++;
                }
            }

            return resultLength;
        }
    }
}
