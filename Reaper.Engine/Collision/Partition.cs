using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Core.Graphics;
using System.Linq;

namespace Core.Collision
{
    public sealed class Partition
    {
        // Each corner (x4) can exist in a different cell.
        public const int MAX_CELLS_PER_BOX = 4;

        private readonly List<Box>[] _cells;

        private readonly int _cellSize;
        private readonly int _width;
        private readonly int _height;
        private readonly int _length;

        internal Partition(int cellSize, int width, int height)
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

        internal void Add(Box box)
        {
            var bounds = box.CalculateBounds();
            var cells = box.PartitionCells;
            var length = GetOccupyingCells(bounds, cells);

            for (int i = 0; i < length; i++)
            {
                _cells[cells[i]].Add(box);
            }

            box.PartitionCellCount = length;
        }

        internal void Remove(Box box)
        {
            var cellCount = box.PartitionCellCount;
            var cells = box.PartitionCells;

            for (int i = 0; i < cellCount; i++)
            {
                _cells[cells[i]].Remove(box);
            }
        }

        internal void Update(Box box)
        {
            Remove(box);
            Add(box);
        }

        public IEnumerable<Box> QueryBounds(Box box)
        {
            var cells = box.PartitionCells;
            var cellCount = box.PartitionCellCount;

            return UnionResults(cells, cellCount);
        }

        public IEnumerable<Box> QueryBounds(RectangleF bounds)
        {
            Span<int> cells = stackalloc int[MAX_CELLS_PER_BOX];
            var cellCount = GetOccupyingCells(bounds, cells);

            return UnionResults(cells, cellCount);
        }

        private IEnumerable<Box> UnionResults(Span<int> cells, int cellCount) 
        {
            var results = Enumerable.Empty<Box>();

            for (int i = 0; i < cellCount; i++)
            {
                results = results.Union(_cells[cells[i]]);
            }

            return results;
        }

        private int GetOccupyingCells(RectangleF bounds, Span<int> cells)
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
            Span<int> cellIndexes = stackalloc int[MAX_CELLS_PER_BOX];

            cellIndexes[0] = (int)index.X;
            cellIndexes[1] = (int)index.Y;
            cellIndexes[2] = (int)index.Z;
            cellIndexes[3] = (int)index.W;

            // Output distinct cell indexes.
            for (int i = 0; i < 4; i++) 
            {
                int iindex = cellIndexes[i];
                int j = 0;

                for (; j < i; j++)
                {
                    if (cellIndexes[j] == iindex) 
                    {
                        break;
                    }
                }

                if (i == j && iindex >= 0 && iindex < _cells.Length) 
                {
                    cells[resultLength++] = iindex;
                }      
            }

            return resultLength;
        }

        internal void DebugDraw()
        {
            // Draw grid cells.
            for (int i = 0; i < _length; i++)
            {
                var row = i / _width;
                var col = i % _width;
                var x = col * _cellSize;
                var y = row * _cellSize;
                var opacity = _cells[i].Count > 0 ? 0.8f : 0.05f;

                Renderer.DrawRectangleOutline(new Rectangle(x, y, _cellSize - 1, _cellSize - 1), Color.DarkGray * opacity);
            }

            // Draw individual colliders.
            for (int i = 0; i < _length; i++)
            {
                foreach (var box in _cells[i])
                {
                    Color color;

                    if (box.IsSolid)
                    {
                        color = Color.White * 0.8f;
                    }
                    else
                    {
                        color = Color.White * 0.8f;
                    }

                    Renderer.DrawRectangleOutline(box.CalculateBounds().ToXnaRect(), color);
                }
            }
        }
    }
}
