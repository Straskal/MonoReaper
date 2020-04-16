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

    /// <summary>
    /// A structure that contains information about the nature of an overlap.
    /// </summary>
    public struct Overlap 
    {
        public Vector2 Depth;
        public WorldObject Other;
    }

    /// <summary>
    /// A LayoutGrid entity.
    /// </summary>
    public class GridObject
    {
        public GridObject(WorldObject worldObject) 
        {
            WorldObject = worldObject;
            Bounds = worldObject.Bounds;
        }

        public GridObject(WorldObject worldObject, WorldObjectBounds bounds)
        {
            WorldObject = worldObject;
            Bounds = bounds;
        }

        public WorldObject WorldObject { get; }
        public WorldObjectBounds Bounds { get; }
    }

    /// <summary>
    /// The grid is a data structure that organizes world objects by their position and allows for efficient spatial queries.
    /// 
    /// The layout is broken out into a grid, representing world space. When querying for objects, we only query the cells that the world object is in.
    /// Every world object has bounds. Bounds have 4 points: top left, top right, bottom left, bottom right.
    /// The 4 points determine which cells the world object is in.
    /// </summary>
    public class WorldObjectGrid
    {
        struct Cell
        {
            public HashSet<GridObject> WorldObjects;
        }

        private readonly int _cellSize;
        private readonly int _width;
        private readonly int _height;
        private readonly Cell[,] _cells;
        private readonly Dictionary<WorldObject, List<GridObject>> _additionalGridObjects;

        internal WorldObjectGrid(int cellSize, int width, int height)
        {
            _cellSize = cellSize;
            _width = (int)Math.Ceiling((double)width / cellSize);
            _height = (int)Math.Ceiling((double)height / cellSize);
            _cells = new Cell[_width, _height];
            _additionalGridObjects = new Dictionary<WorldObject, List<GridObject>>();

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    _cells[i, j].WorldObjects = new HashSet<GridObject>();
                }
            }
        }

        internal void Add(WorldObject worldObject)
        {
            if (worldObject.SpatialType == SpatialType.Pass)
                return;

            foreach (var cellPos in GetOccupyingCells(worldObject.Bounds))
            {
                _cells[cellPos.X, cellPos.Y].WorldObjects.Add(new GridObject(worldObject));
            }
        }

        /// <summary>
        /// Adds additional colliders to a world object's grid representation.
        /// This currently has some limitations:
        /// - When the owning world object moves, the additional grid objects do not move or get updated.
        /// 
        /// This was added in order to give the tilemap behavior a way to add tile colliders without actually adding them to the wo list, which caused noise.
        /// </summary>
        /// <param name="worldObject"></param>
        /// <param name="bounds"></param>
        internal void Add(WorldObject worldObject, WorldObjectBounds bounds)
        {
            if (worldObject.SpatialType == SpatialType.Pass)
                return;

            foreach (var cellPos in GetOccupyingCells(bounds))
            {
                var go = new GridObject(worldObject, bounds);

                if (_additionalGridObjects.TryGetValue(worldObject, out var additionalGridObjects)) 
                {
                    additionalGridObjects.Add(go);
                }
                else 
                {
                    _additionalGridObjects.Add(worldObject, new List<GridObject> { go });
                }

                _cells[cellPos.X, cellPos.Y].WorldObjects.Add(go);
            }
        }

        internal void Remove(WorldObject worldObject)
        {
            if (worldObject.PreviousSpatialType == SpatialType.Pass)
                return;

            foreach (var cellPos in GetOccupyingCells(worldObject.PreviousBounds))
            {
                _cells[cellPos.X, cellPos.Y].WorldObjects.RemoveWhere(go => go.WorldObject == worldObject);
            }

            if (_additionalGridObjects.TryGetValue(worldObject, out var additionalGridObjects))
            {
                foreach (var gridObject in additionalGridObjects)
                {
                    foreach (var cellPos in GetOccupyingCells(gridObject.Bounds))
                    {
                        _cells[cellPos.X, cellPos.Y].WorldObjects.Remove(gridObject);
                    }
                }
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
            var overlappedObject = QueryBounds(worldObject.Bounds).FirstOrDefault(other => other.WorldObject != worldObject);

            if (overlappedObject != null)
            {
                overlap.Depth = worldObject.Bounds.GetIntersectionDepth(overlappedObject.Bounds);
                overlap.Other = overlappedObject.WorldObject;
                return true;
            }
            return false;
        }

        public bool IsOverlapping(WorldObject worldObject, string[] ignoreTags, out Overlap overlap)
        {
            overlap = new Overlap();
            var overlappedObject = QueryBounds(worldObject.Bounds)
                .FirstOrDefault(other => other.WorldObject != worldObject && !ignoreTags.Any(tag => other.WorldObject.Tags.Contains(tag)));

            if (overlappedObject != null)
            {
                overlap.Depth = worldObject.Bounds.GetIntersectionDepth(overlappedObject.Bounds);
                overlap.Other = overlappedObject.WorldObject;
                return true;
            }
            return false;
        }

        public bool IsCollidingAtOffset(WorldObject worldObject, float xOffset, float yOffset, out Overlap overlap)
        {
            overlap = new Overlap();
            var bounds = worldObject.Bounds;
            bounds.Offset(xOffset, yOffset);
            var overlappedObject = QueryBounds(bounds).FirstOrDefault(other => other.WorldObject != worldObject && other.WorldObject.IsSolid);

            if (overlappedObject != null)
            {
                overlap.Depth = bounds.GetIntersectionDepth(overlappedObject.Bounds);
                overlap.Other = overlappedObject.WorldObject;
                return true;
            }
            return false;
        }

        public GridObject[] QueryBounds(WorldObjectBounds bounds, params string[] ignoreTags)
        {
            // Removing ToRectangle() does not give us accurate overlaps.
            return QueryCells(bounds).Where(other => 
                bounds.ToRectangle().Intersects(other.Bounds.ToRectangle()) 
                    && !ignoreTags.Any(tag => other.WorldObject.Tags.Contains(tag))).ToArray();
        }

        public GridObject[] QueryBounds(WorldObjectBounds bounds)
        {
            // Removing ToRectangle() does not give us accurate overlaps.
            return QueryCells(bounds).Where(other => bounds.ToRectangle().Intersects(other.Bounds.ToRectangle())).ToArray();
        }

        internal void DebugDraw(Renderer renderer) 
        {
            for (int i = 0; i < _cells.GetLength(0); i++) 
            {
                for (int j = 0; j < _cells.GetLength(1); j++)
                {
                    foreach (var go in _cells[i, j].WorldObjects) 
                    {
                        const float opacity = 0.3f;
                        Color color;

                        switch (go.WorldObject.SpatialType)
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

                        renderer.DrawRectangle(go.Bounds.ToRectangle(), color);
                    }
                }
            }
        }

        private IEnumerable<GridObject> QueryCells(WorldObjectBounds bounds) 
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
