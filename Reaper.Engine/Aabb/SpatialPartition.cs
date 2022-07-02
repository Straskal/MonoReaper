using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Reaper.Engine.AABB;
using Reaper.Engine.Components;

namespace Reaper.Engine.Aabb
{
    public sealed class SpatialPartition
    {
        // Each corner (x4) can exist in a different cell.
        private const int MAX_BOUNDS_BUCKETS = 4;

        private readonly List<Box>[] cells;

        private readonly int cellSize;
        private readonly int width;
        private readonly int height;
        private readonly int length;

        internal SpatialPartition(int cellSize, int width, int height)
        {
            this.cellSize = cellSize;
            this.width = (int)Math.Ceiling((double)width / cellSize);
            this.height = (int)Math.Ceiling((double)height / cellSize);

            length = this.width * this.height;
            cells = new List<Box>[length];

            for (int i = 0; i < length; i++)
            {
                cells[i] = new List<Box>();
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
            return QueryBuckets(bounds)
                .Where(b => b.Layer.HasFlag(CollisionLayer.Overlap))
                .Where(o => bounds.Intersects(o.Bounds));
        }

        internal void DebugDraw()
        {
            const float opacity = 0.3f;

            // Draw grid cells.
            for (int i = 0; i < length; i++)
            {
                var row = i / width;
                var col = i % width;
                var x = col * cellSize;
                var y = row * cellSize;

                Renderer.DrawRectangle(new Rectangle(x, y, cellSize - 1, cellSize - 1), Color.Purple * opacity);
            }

            // Draw individual colliders.
            for (int i = 0; i < length; i++)
            {
                foreach (var obj in cells[i])
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

                    Renderer.DrawRectangle(obj.Bounds.AsRectangle, color);
                }
            }
        }

        private void Add(Box box, Span<int> buckets)
        {
            var length = GetOccupyingBuckets(box.Bounds, buckets);

            for (int i = 0; i < length; i++)
            {
                if (buckets[i] >= 0 && buckets[i] < this.cells.Length)
                {
                    this.cells[buckets[i]].Add(box);
                }
            }
        }

        private void Remove(Box box, Vector2 previousPosition, Span<int> buckets)
        {
            var bounds = OriginHelpers.GetOffsetRect(box.Entity.Origin, previousPosition.X, previousPosition.Y, box.Width, box.Height);
            var length = GetOccupyingBuckets(bounds, buckets);

            for (int i = 0; i < length; i++)
            {
                if (buckets[i] >= 0 && buckets[i] < this.cells.Length)
                {
                    this.cells[buckets[i]].Remove(box);
                }
            }
        }

        private IEnumerable<Box> QueryBuckets(RectangleF bounds)
        {
            var results = new HashSet<Box>();
            Span<int> buckets = stackalloc int[MAX_BOUNDS_BUCKETS];
            var length = GetOccupyingBuckets(bounds, buckets);

            for (int i = 0; i < length; i++)
            {
                if (buckets[i] >= 0 && buckets[i] < this.cells.Length)
                {
                    results.UnionWith(this.cells[buckets[i]]);
                }
            }

            return results;
        }

        private int GetOccupyingBuckets(RectangleF bounds, Span<int> buckets)
        {
            var resultLength = 0;

            // Calculate the bucket index for each corner of bounds.
            var y = new Vector4(bounds.Top, bounds.Top, bounds.Bottom, bounds.Bottom);
            var x = new Vector4(bounds.Left, bounds.Right, bounds.Left, bounds.Right);

            var row = y / cellSize;
            var col = x / cellSize;

            row.Floor();
            col.Floor();

            var index = row * width + col;

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
