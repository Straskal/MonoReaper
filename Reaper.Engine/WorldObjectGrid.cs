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
        struct Cell
        {
            public HashSet<WorldObject> WorldObjects;
        }

        private readonly int _cellSize;
        private readonly int _width;
        private readonly int _height;
        private readonly Cell[,] _cells;

        internal WorldObjectGrid(int cellSize, int width, int height)
        {
            _cellSize = cellSize;
            _width = (int)Math.Ceiling((double)width / cellSize);
            _height = (int)Math.Ceiling((double)height / cellSize);
            _cells = new Cell[_width, _height];

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    _cells[i, j].WorldObjects = new HashSet<WorldObject>();
                }
            }
        }

        internal void Add(WorldObject worldObject)
        {
            if (worldObject.SpatialType == SpatialType.Pass)
                return;

            foreach (var cellPos in GetOccupyingCells(worldObject.Bounds))
            {
                _cells[cellPos.X, cellPos.Y].WorldObjects.Add(worldObject);
            }
        }

        internal void Remove(WorldObject worldObject)
        {
            if (worldObject.PreviousSpatialType == SpatialType.Pass)
                return;

            foreach (var cellPos in GetOccupyingCells(worldObject.PreviousBounds))
            {
                _cells[cellPos.X, cellPos.Y].WorldObjects.Remove(worldObject);
            }
        }

        internal void Update(WorldObject worldObject)
        {
            if (worldObject.Position == worldObject.PreviousPosition)
                return;

            if (worldObject.PreviousSpatialType != SpatialType.Pass)
                Remove(worldObject);

            if (worldObject.SpatialType != SpatialType.Pass)
                Add(worldObject);
        }

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

        public bool IsOverlapping(WorldObject worldObject, string[] ignoreTags, out Overlap overlap)
        {
            overlap = new Overlap();
            var overlappedObject = QueryBounds(worldObject.Bounds).FirstOrDefault(other => other != worldObject && !ignoreTags.Any(tag => other.Tags.Contains(tag)));

            if (overlappedObject != null)
            {
                overlap.Depth = worldObject.Bounds.GetIntersectionDepth(overlappedObject.Bounds);
                overlap.Other = overlappedObject;
                return true;
            }

            return false;
        }

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
            return QueryCells(bounds).Where(other => 
                bounds.ToRectangle().Intersects(other.Bounds.ToRectangle()) 
                    && !ignoreTags.Any(tag => other.Tags.Contains(tag))).ToArray();
        }

        public WorldObject[] QueryBounds(WorldObjectBounds bounds)
        {
            // Removing ToRectangle() does not give us accurate overlaps.
            return QueryCells(bounds).Where(other => bounds.ToRectangle().Intersects(other.Bounds.ToRectangle())).ToArray();
        }

        internal void DebugDraw(Renderer renderer)
        {
            const float opacity = 0.3f;

            for (int i = 0; i < _cells.GetLength(0); i++) 
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    foreach (var wo in _cells[i, j].WorldObjects) 
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
            return GetOccupyingCells(bounds).SelectMany(cell => _cells[cell.X, cell.Y].WorldObjects).Distinct();
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
            column = (int)(position.X / _cellSize);
            row = (int)(position.Y / _cellSize);
            return column < _cells.GetLength(0) && row < _cells.GetLength(1);
        }
    }
}
