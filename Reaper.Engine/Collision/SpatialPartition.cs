using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Core.Graphics;

namespace Core.Collision
{
    public sealed class SpatialPartition
    {
        // Each corner (x4) can exist in a different cell.
        private const int MAX_BOUNDS_BUCKETS = 4;

        private readonly List<Box>[] _cells;

        private readonly int _cellSize;
        private readonly int _width;
        private readonly int _height;
        private readonly int _length;

        internal SpatialPartition(int cellSize, int width, int height)
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

        internal void Add(Box worldObject)
        {
            if (worldObject.Layer != CollisionLayer.Pass)
            {
                Span<int> buckets = stackalloc int[MAX_BOUNDS_BUCKETS];

                Add(worldObject, buckets);
            }
        }

        internal void Remove(Box worldObject, Vector2 previousPosition)
        {
            Span<int> buckets = stackalloc int[MAX_BOUNDS_BUCKETS];

            Remove(worldObject, previousPosition, buckets);
        }

        internal void Update(Box worldObject, Vector2 previousPosition)
        {
            if (worldObject.Entity.Position == previousPosition)
                return;

            Span<int> buckets = stackalloc int[MAX_BOUNDS_BUCKETS];

            Remove(worldObject, previousPosition, buckets);

            Add(worldObject, buckets);
        }

        public IEnumerable<Box> QueryBounds(RectangleF bounds)
        {
            return QueryBuckets(bounds).Where(b => b.Layer.HasFlag(CollisionLayer.Overlap));
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

                    switch (obj.Layer)
                    {
                        case CollisionLayer.Pass:
                            color = Color.Pink * opacity;
                            break;
                        case CollisionLayer.Overlap:
                            color = Color.Blue * opacity;
                            break;
                        default:
                            color = Color.Red * opacity;
                            break;
                    }

                    Renderer.DrawRectangle(obj.CalculateBounds().ToXnaRect(), color);
                }
            }
        }

        private void Add(Box box, Span<int> buckets)
        {
            var length = GetOccupyingCells(box.CalculateBounds(), buckets);

            for (int i = 0; i < length; i++)
            {
                if (buckets[i] >= 0 && buckets[i] < this._cells.Length)
                {
                    _cells[buckets[i]].Add(box);
                }
            }
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

        private IEnumerable<Box> QueryBuckets(RectangleF bounds)
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
