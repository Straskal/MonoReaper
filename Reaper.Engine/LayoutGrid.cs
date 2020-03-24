using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reaper.Engine
{
    public enum SpatialType
    {
        // Not returned from queries.
        Pass = 0,

        // Returned from general queries.
        Overlap = 1 << 0,

        // Returned from all queries.
        Solid = Overlap | (1 << 1)
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
            _width = width;
            _height = height;
            _cells = new Cell[width, height];

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
            if (worldObject.Type == SpatialType.Pass)
                return;

            foreach (var cellPos in GetOccupyingCells(worldObject.Bounds))
            {
                _cells[cellPos.X, cellPos.Y].WorldObjects.Add(worldObject);
            }
        }

        internal void Remove(WorldObject worldObject)
        {
            if (worldObject.Type == SpatialType.Pass)
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

        internal void UpdateType(WorldObject worldObject, SpatialType previous)
        {
            if (previous != SpatialType.Pass) 
                Remove(worldObject);

            if (worldObject.Type != SpatialType.Pass) 
                Add(worldObject);
        }

        public bool TestSolidOverlapOffset(WorldObject worldObject, float xOffset, float yOffset)
        {
            var bounds = GetOffsetBounds(worldObject, xOffset, yOffset);
            return QueryBounds(bounds).Any(other => other != worldObject && other.IsSolid);
        }

        public bool TestSolidOverlapOffset(WorldObject worldObject, float xOffset, float yOffset, out WorldObject overlappedWorldObject)
        {
            overlappedWorldObject = null;
            var bounds = GetOffsetBounds(worldObject, xOffset, yOffset);
            overlappedWorldObject = QueryBounds(bounds).FirstOrDefault(other => other != worldObject && other.IsSolid);
            return overlappedWorldObject != null;
        }

        public IEnumerable<WorldObject> QueryBounds(Rectangle bounds)
        {
            return QueryCells(bounds).Where(other => other.Bounds.Intersects(bounds));
        }

        private Rectangle GetOffsetBounds(WorldObject worldObject, float xOffset, float yOffset) 
        {
            var testedPosition = worldObject.Position + new Vector2(xOffset, yOffset);

            return new Rectangle(
                (int)Math.Round(testedPosition.X - worldObject.Origin.X),
                (int)Math.Round(testedPosition.Y - worldObject.Origin.Y),
                worldObject.Bounds.Width,
                worldObject.Bounds.Height);
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
