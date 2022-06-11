using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reaper.Engine
{
    /// <summary>
    /// Defines the spatial behavior of a world object.
    /// </summary>
    [Flags]
    public enum SpatialType
    {
        // Not returned from spatial queries.
        Pass = 0,

        // Returned from overlap queries.
        Overlap = 1 << 0,

        // Returned from overlap and solid queries.
        Solid = Overlap | (1 << 1)
    }

    /// <summary>
    /// A structure that contains information about the nature of an overlap.
    /// </summary>
    public struct Overlap
    {
        public Vector2 Depth;
        public WorldObject Other;
    }

    /// <summary>
    /// The grid is a data structure that organizes world objects by their position and allows for efficient spatial queries.
    /// </summary>
    public class WorldObjectGrid
    {
        // Used to temporarily store cells for calculations.
        private readonly HashSet<int> _tempCells = new HashSet<int>();

        // The grid cells (buckets) that track world object positions.
        private readonly Dictionary<int, List<WorldObject>> _buckets;

        // The size of a single spatial hash cell.
        private readonly int _cellSize;

        // The width of the grid in cells.
        private readonly int _width;

        // The height of the grid in cells.
        private readonly int _height;

        // The number of cells in the grid.
        private readonly int _length;

        internal WorldObjectGrid(int cellSize, int width, int height)
        {
            _cellSize = cellSize;
            _width = (int)Math.Ceiling((double)width / cellSize);
            _height = (int)Math.Ceiling((double)height / cellSize);
            _length = _width * _height;
            _buckets = new Dictionary<int, List<WorldObject>>(_length);

            for (int i = 0; i < _length; i++)
            {
                _buckets[i] = new List<WorldObject>();
            }
        }

        /// <summary>
        /// Adds the object to world space.
        /// </summary>
        /// <param name="worldObject"></param>
        internal void Add(WorldObject worldObject)
        {
            if (worldObject.SpatialType != SpatialType.Pass)
            {
                var buckets = GetOccupyingBuckets(worldObject.Bounds);

                for (int i = 0; i < buckets.Length; i++)
                {
                    _buckets[buckets[i]].Add(worldObject);
                }
            }
        }

        /// <summary>
        /// Removes the object from world space.
        /// </summary>
        /// <param name="worldObject"></param>
        internal void Remove(WorldObject worldObject)
        {
            if (worldObject.PreviousSpatialType != SpatialType.Pass)
            {
                var buckets = GetOccupyingBuckets(worldObject.PreviousBounds);

                for (int i = 0; i < buckets.Length; i++)
                {
                    _buckets[buckets[i]].Remove(worldObject);
                }
            }
        }

        /// <summary>
        /// Updates the objects position in world space.
        /// </summary>
        /// <param name="worldObject"></param>
        internal void Update(WorldObject worldObject)
        {
            // TODO: This needs to be improved.
            // The current implementation will do a bunch of unnecessary work if the object isn't moving cells.
            if (worldObject.Position != worldObject.PreviousPosition)
            {
                if (worldObject.PreviousSpatialType != SpatialType.Pass)
                {
                    Remove(worldObject);
                }

                if (worldObject.SpatialType != SpatialType.Pass)
                {
                    Add(worldObject);
                }
            }
        }

        /// <summary>
        /// Returns true if the object is overlapping with another object.
        /// If true, outputs information about the overlap.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        public bool IsOverlapping(WorldObject worldObject, out Overlap overlap)
        {
            overlap = new Overlap();

            var overlaps = QueryBounds(worldObject.Bounds);
            var overlappedObject = overlaps.FirstOrDefault(other => other != worldObject);

            if (overlappedObject != null)
            {
                overlap.Depth = worldObject.Bounds.GetIntersectionDepth(overlappedObject.Bounds);
                overlap.Other = overlappedObject;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the object is overlapping with another object that does not contain the given tags.
        /// If true, outputs information about the overlap.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <param name="ignoreTags"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        public bool IsOverlapping(WorldObject worldObject, string[] ignoreTags, out Overlap overlap)
        {
            overlap = new Overlap();

            var overlaps = QueryBounds(worldObject.Bounds, ignoreTags);
            var overlappedObject = overlaps.FirstOrDefault(other => other != worldObject);

            if (overlappedObject != null)
            {
                overlap.Depth = worldObject.Bounds.GetIntersectionDepth(overlappedObject.Bounds);
                overlap.Other = overlappedObject;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the object is overlapping with another object at the given offset.
        /// If true, outputs information about the overlap.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="overlap"></param>
        /// <returns></returns>
        public bool IsCollidingAtOffset(WorldObject worldObject, float xOffset, float yOffset, out Overlap overlap)
        {
            overlap = new Overlap();
            var offsetBounds = worldObject.Bounds;
            offsetBounds.Offset(xOffset, yOffset);

            var overlappedObject = QueryBounds(offsetBounds).FirstOrDefault(other => other != worldObject && other.IsSolid);

            if (overlappedObject != null)
            {
                overlap.Depth = offsetBounds.GetIntersectionDepth(overlappedObject.Bounds);
                overlap.Other = overlappedObject;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns all world objects that overlap with the given bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="ignoreTags"></param>
        /// <returns></returns>
        public IEnumerable<WorldObject> QueryBounds(WorldObjectBounds bounds, params string[] ignoreTags)
        {
            return QueryBounds(bounds).Where(other => !ignoreTags.Any(tag => other.Tags.Contains(tag)));
        }

        /// <summary>
        /// Returns all world objects that overlap with the given bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="ignoreTags"></param>
        /// <returns></returns>
        public IEnumerable<WorldObject> QueryBounds(WorldObjectBounds bounds)
        {
            // 'Narrow Phase' of collision detection.
            // We let QueryBuckets(...) rule out all impossible collisions, then take the (sometimes) smaller result set and test intersections.
            return QueryBuckets(bounds).Where(other => bounds.Intersects(other.Bounds));
        }

        /// <summary>
        /// Super useful method for debugging collision shit.
        /// </summary>
        /// <param name="renderer"></param>
        internal void DebugDraw(Renderer renderer)
        {
            const float opacity = 0.3f;

            // Draw grid cells.
            for (int i = 0; i < _length; i++)
            {
                var row = i / _width;
                var col = i % _width;
                var x = col * _cellSize;
                var y = row * _cellSize;
                var color = Color.LightBlue;

                renderer.DrawRectangle(new Rectangle(x, y, _cellSize - 1, _cellSize - 1), color);
            }

            // Draw individual colliders.
            for (int i = 0; i < _length; i++)
            {
                foreach (var obj in _buckets[i])
                {
                    Color color;

                    switch (obj.SpatialType)
                    {
                        case SpatialType.Pass:
                            color = Color.Pink * opacity;
                            break;
                        case SpatialType.Overlap:
                            color = Color.Blue * opacity;
                            break;
                        default:
                            color = Color.Red * opacity;
                            break;
                    }

                    renderer.DrawRectangle(obj.Bounds.ToRectangle(), color);
                }
            }
        }

        // This is the "broad phase" portion of the spatial queries.
        // This returns all world objects that could 'possibly' collide with the given bounds.
        private IEnumerable<WorldObject> QueryBuckets(WorldObjectBounds bounds)
        {
            var results = new HashSet<WorldObject>();
            var buckets = GetOccupyingBuckets(bounds);

            for (int i = 0; i < buckets.Length; i++)
            {
                results.UnionWith(_buckets[buckets[i]]);
            }

            return results;
        }

        // Returns the buckets that contain the given bounds.
        // Bounds are split into their 4 corners: top left, top right, bottom left, bottom right.
        // The containing bucket is checked for each point, so a world object could technically exist within 4 separate cells.
        private Span<int> GetOccupyingBuckets(WorldObjectBounds bounds)
        {
            var width = _width;

            var tlRow = Math.Floor(bounds.Top / _cellSize);
            var tlCol = Math.Floor(bounds.Left / _cellSize);
            var tlBucket = (int)(tlCol + tlRow * width);

            var trRow = Math.Floor(bounds.Top / _cellSize);
            var trCol = Math.Floor(bounds.Right / _cellSize);
            var trBucket = (int)(trCol + trRow * width);

            var blRow = Math.Floor(bounds.Bottom / _cellSize);
            var blCol = Math.Floor(bounds.Right / _cellSize);
            var blBucket = (int)(blCol + blRow * width);

            var brRow = Math.Floor(bounds.Bottom / _cellSize);
            var brCol = Math.Floor(bounds.Right / _cellSize);
            var brBucket = (int)(brCol + brRow * width);

            _tempCells.Clear();

            if (tlBucket >= 0 && tlBucket < _length)
            {
                _tempCells.Add(tlBucket);
            }

            if (trBucket >= 0 && trBucket < _length)
            {
                _tempCells.Add(trBucket);
            }

            if (blBucket >= 0 && blBucket < _length)
            {
                _tempCells.Add(blBucket);
            }

            if (brBucket >= 0 && brBucket < _length)
            {
                _tempCells.Add(brBucket);
            }

            return _tempCells.ToArray();
        }
    }
}
