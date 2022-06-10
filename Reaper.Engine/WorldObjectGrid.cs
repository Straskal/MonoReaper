using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using GridCell = System.Collections.Generic.HashSet<Reaper.Engine.WorldObject>;

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
    /// 
    /// TODO: This grid is really inefficient. Uses way too much Enumeration and LINQ, which both generate a lot of garbage.
    /// </summary>
    public class WorldObjectGrid
    {
        private readonly GridCell[,] _cells;

        public readonly int CellSize;
        public readonly int Width;
        public readonly int Height;

        internal WorldObjectGrid(int cellSize, int width, int height)
        {
            _cells = new GridCell[width, height];

            CellSize = cellSize;
            Width = (int)Math.Ceiling((double)width / cellSize);
            Height = (int)Math.Ceiling((double)height / cellSize);

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _cells[i, j] = new GridCell();
                }
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
                // Add the object all cells that contain in.
                foreach (var cellPos in GetOccupyingCells(worldObject.Bounds))
                {
                    _cells[cellPos.X, cellPos.Y].Add(worldObject);
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
                // Remove the object from all cells that contain it.
                foreach (var cellPos in GetOccupyingCells(worldObject.PreviousBounds))
                {
                    _cells[cellPos.X, cellPos.Y].Remove(worldObject);
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
            var overlappedObject = QueryBounds(worldObject.Bounds).FirstOrDefault(other => other != worldObject);

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
            var overlappedObject = QueryBounds(worldObject.Bounds, ignoreTags).FirstOrDefault(other => other != worldObject);

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

        public WorldObject[] QueryBounds(WorldObjectBounds bounds, params string[] ignoreTags)
        {
            // Removing ToRectangle() does not give us accurate overlaps.
            return QueryCells(bounds).Where(other => bounds.Intersects(other.Bounds) && !ignoreTags.Any(tag => other.Tags.Contains(tag))).ToArray();
        }

        public WorldObject[] QueryBounds(WorldObjectBounds bounds)
        {
            return QueryCells(bounds).Where(other => bounds.Intersects(other.Bounds)).ToArray();
        }

        internal void DebugDraw(Renderer renderer)
        {
            const float opacity = 0.3f;

            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    foreach (var wo in _cells[i, j])
                    {
                        Color color;

                        switch (wo.SpatialType)
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

                        renderer.DrawRectangle(wo.Bounds.ToRectangle(), color);
                    }
                }
            }
        }

        private IEnumerable<WorldObject> QueryCells(WorldObjectBounds bounds)
        {
            return GetOccupyingCells(bounds).SelectMany(cell => _cells[cell.X, cell.Y]).Distinct();
        }

        private IEnumerable<Point> GetOccupyingCells(WorldObjectBounds bounds)
        {
            return GetBoundPointCells(bounds).Distinct();
        }

        private IEnumerable<Point> GetBoundPointCells(WorldObjectBounds bounds)
        {
            if (TryGetCellPosition(new Vector2(bounds.X, bounds.Top), out int topLeftCol, out int topLeftRow))
                yield return new Point(topLeftCol, topLeftRow);

            if (TryGetCellPosition(new Vector2(bounds.Right, bounds.Top), out int topRightCol, out int topRightRow))
                yield return new Point(topRightCol, topRightRow);

            if (TryGetCellPosition(new Vector2(bounds.X, bounds.Bottom), out int bottomLeftCol, out int bottomLeftRow))
                yield return new Point(bottomLeftCol, bottomLeftRow);

            if (TryGetCellPosition(new Vector2(bounds.Right, bounds.Bottom), out int bottomRightCol, out int bottomRightRow))
                yield return new Point(bottomRightCol, bottomRightRow);
        }

        private bool TryGetCellPosition(Vector2 position, out int column, out int row)
        {
            column = (int)(position.X / CellSize);
            row = (int)(position.Y / CellSize);

            return column < _cells.GetLength(0) && row < _cells.GetLength(1);
        }
    }
}
