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

        // Returned from general spatial queries.
        Overlap = 1 << 0,

        // Returned from all spatial queries, including solid queries.
        Solid = Overlap | (1 << 1)
    }

    public struct Overlap 
    {
        public Vector2 Depth;
        public WorldObject Other;
    }

    /// <summary>
    /// The layout grid is a data structure that organizes world objects by their position and allows for efficient spatial queries.
    /// 
    /// The layout is broken out into a grid, representing world space. When querying for objects, we only query the cells that the world object is in.
    /// Every world object has bounds. Bounds have 4 points: top left, top right, bottom left, bottom right.
    /// The 4 points determine which cells the world object is in.
    /// </summary>
    public class LayoutGrid
    {
        struct Cell
        {
            public HashSet<WorldObject> WorldObjects;
        }

        private readonly int _cellSize;
        private readonly int _width;
        private readonly int _height;
        private readonly Cell[,] _cells;

        internal LayoutGrid(int cellSize, int width, int height)
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
            if (worldObject.SpatialType == SpatialType.Pass)
                return;

            foreach (var cellPos in GetOccupyingCells(worldObject.Bounds))
            {
                _cells[cellPos.X, cellPos.Y].WorldObjects.Remove(worldObject);
            }
        }

        internal void Update(WorldObject worldObject)
        {
            if (worldObject.Position == worldObject.PreviousPosition)
                return;

            Remove(worldObject);
            Add(worldObject);
        }

        internal void UpdateType(WorldObject worldObject)
        {
            if (worldObject.PreviousSpatialType != SpatialType.Pass) 
                Remove(worldObject);

            if (worldObject.SpatialType != SpatialType.Pass) 
                Add(worldObject);
        }

        public bool IsCollidingAtOffset(WorldObject worldObject, float xOffset, float yOffset)
        {
            var bounds = GetOffsetBounds(worldObject, xOffset, yOffset);
            return QueryBounds(bounds).Any(other => other != worldObject && other.IsSolid);
        }

        public bool IsColliding(WorldObject worldObject, out WorldObject overlappedWorldObject)
        {
            overlappedWorldObject = QueryBounds(worldObject.Bounds).FirstOrDefault(other => other != worldObject && other.IsSolid);
            return overlappedWorldObject != null;
        }

        public bool IsCollidingAtOffset(WorldObject worldObject, float xOffset, float yOffset, out WorldObject overlappedWorldObject)
        {
            var bounds = GetOffsetBounds(worldObject, xOffset, yOffset);
            overlappedWorldObject = QueryBounds(bounds).FirstOrDefault(other => other != worldObject && other.IsSolid);
            return overlappedWorldObject != null;
        }

        public bool IsCollidingAtOffsetOk(WorldObject worldObject, float xOffset, float yOffset, out Overlap overlap)
        {
            overlap = new Overlap();
            worldObject.Position += new Vector2(xOffset, yOffset);
            var overlappedObject = QueryBounds(worldObject).FirstOrDefault(other => other != worldObject && other.IsSolid);

            if (overlappedObject != null)
            {
                overlap.Depth = worldObject.GetIntersectionDepth(overlappedObject);
                overlap.Other = overlappedObject;
                worldObject.Position -= new Vector2(xOffset, yOffset);
                return true;
            }
            worldObject.Position -= new Vector2(xOffset, yOffset);
            return false;
        }

        public bool IsOverlappingAtOffset(WorldObject worldObject, float xOffset, float yOffset, out WorldObject overlappedWorldObject)
        {
            var bounds = GetOffsetBounds(worldObject, xOffset, yOffset);
            overlappedWorldObject = QueryBounds(bounds).FirstOrDefault(other => other != worldObject);
            return overlappedWorldObject != null;
        }

        public IEnumerable<WorldObject> QueryBounds(WorldObject bounds)
        {
            return QueryCells(bounds.Bounds).Where(other => bounds.GetIntersectionDepth(other) != Vector2.Zero);
        }

        public IEnumerable<WorldObject> QueryBounds(Rectangle bounds)
        {
            return QueryCells(bounds).Where(other => other.Bounds.Intersects(bounds));
        }

        private Rectangle GetOffsetBounds(WorldObject worldObject, float xOffset, float yOffset) 
        {
            var bounds = worldObject.Bounds;
            bounds.X += (int)Math.Round(xOffset);
            bounds.Y += (int)Math.Round(yOffset);
            return bounds;
        }

        private IEnumerable<WorldObject> QueryCells(Rectangle bounds) 
        {
            return GetOccupyingCells(bounds).SelectMany(cell => _cells[cell.X, cell.Y].WorldObjects).Distinct();
        }

        private IEnumerable<Point> GetOccupyingCells(Rectangle bounds) 
        {
            return GetBoundPointCells(bounds).Distinct();
        }

        private IEnumerable<Point> GetBoundPointCells(Rectangle bounds) 
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
