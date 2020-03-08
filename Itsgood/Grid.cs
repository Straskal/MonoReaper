using System.Collections.Generic;
using System.Linq;

namespace ItsGood
{
    internal class Grid
    {
        struct Cell
        {
            public List<WorldObject> WorldObjects;
        }

        private readonly int _cellSize;
        private readonly int _width;
        private readonly int _height;
        private readonly Cell[,] _cells;

        public Grid(int cellSize, int width, int height) 
        {
            _cellSize = cellSize;
            _width = width;
            _height = height;
            _cells = new Cell[width, height];

            for (int i = 0; i < _width; i++) 
            {
                for (int j = 0; i < _height; i++)
                {
                    _cells[i, j].WorldObjects = new List<WorldObject>();
                }
            }
        }

        public void Add(WorldObject worldObject) 
        {
            int cellX = (int)(worldObject.Position.X / _cellSize);
            int cellY = (int)(worldObject.Position.Y / _cellSize);

            _cells[cellX, cellY].WorldObjects = _cells[cellX, cellY].WorldObjects ?? new List<WorldObject>();
            _cells[cellX, cellY].WorldObjects.Add(worldObject);
        }

        public void Remove(WorldObject worldObject)
        {
            int cellX = (int)(worldObject.Position.X / _cellSize);
            int cellY = (int)(worldObject.Position.Y / _cellSize);

            _cells[cellX, cellY].WorldObjects = _cells[cellX, cellY].WorldObjects ?? new List<WorldObject>();
            _cells[cellX, cellY].WorldObjects.Remove(worldObject);
        }

        public void Update(WorldObject worldObject) 
        {
            int previousCellX = (int)(worldObject.PreviousPosition.X / _cellSize);
            int previousCellY = (int)(worldObject.PreviousPosition.Y / _cellSize);
            int cellX = (int)(worldObject.Position.X / _cellSize);
            int cellY = (int)(worldObject.Position.Y / _cellSize);

            if (previousCellX == cellX && previousCellY == cellY)
                return;

            _cells[previousCellX, previousCellY].WorldObjects.Remove(worldObject);
            _cells[cellX, cellY].WorldObjects.Add(worldObject);
        }

        public IEnumerable<WorldObject> QueryCollisions(WorldObject worldObject)
        {
            int cellX = (int)(worldObject.Position.X / _cellSize);
            int cellY = (int)(worldObject.Position.Y / _cellSize);

            return _cells[cellX, cellY].WorldObjects.Where(other => other != worldObject && other.IsSolid && other.Bounds.Intersects(worldObject.Bounds));
        }

        public IEnumerable<WorldObject> QueryOverlaps(WorldObject worldObject)
        {
            int cellX = (int)(worldObject.Position.X / _cellSize);
            int cellY = (int)(worldObject.Position.Y / _cellSize);

            return _cells[cellX, cellY].WorldObjects.Where(other => other != worldObject && other.Bounds.Intersects(worldObject.Bounds));
        }
    }
}
