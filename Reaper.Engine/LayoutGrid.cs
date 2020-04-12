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

        public bool IsCollidingAtOffset(WorldObject worldObject, float xOffset, float yOffset, out Overlap overlap)
        {
            overlap = new Overlap();
            var bounds = worldObject.Bounds;
            bounds.Offset(xOffset, yOffset);
            var overlappedObject = QueryBounds(bounds).FirstOrDefault(other => other != worldObject && other.IsSolid);

            if (overlappedObject != null)
            {
                overlap.Depth = bounds.GetIntersectionDepth(overlappedObject.Bounds);
                overlap.Other = overlappedObject;
                return true;
            }
            return false;
        }

        public IEnumerable<WorldObject> QueryBounds(WorldObjectBounds bounds)
        {
            return QueryCells(bounds).Where(other => bounds.ToRectangle().Intersects(other.Bounds.ToRectangle()));
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
