using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GenerationModule
{
    /// <summary>
    /// Class that generates a maze of the given size
    /// </summary>
    public class MazeGenerator
    {
        private readonly System.Random _random;
        private readonly Cell[] _cells;
        private readonly Vector2Int _size;

        public MazeGenerator(int width, int height, int? seed = null)
        {
            // Check for size
            if (width <= 0 || height <= 0)
                throw new ArgumentException("The maze must be at least 1x1.");

            _size = new Vector2Int(width, height);
            _cells = new Cell[width * height];

            // Generate random seed if not specified
            seed ??= UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            _random = new System.Random(seed.Value);
        }

        public Cell[] Generate()
        {
            // Resets all the cells
            for (var i = 0; i < _cells.Length; i++)
            {
                _cells[i] = new Cell
                {
                    X = i % _size.x,
                    Y = i / _size.x
                };
            }

            // Generate the maze
            var cellsToVisit = new Stack<Cell>();
            cellsToVisit.Push(_cells[0]);

            // While the stack is not empty
            while (cellsToVisit.Count > 0)
            {
                // Pop a cell from the stack and make it a current cell
                var currentCell = cellsToVisit.Pop();

                var neighbors = GetNeighbours(currentCell).Where(c => !c.WasVisited).ToArray();

                // If the current cell has any neighbours which have not been visited
                if (neighbors.Length == 0)
                    continue;

                // Push the current cell to the stack
                cellsToVisit.Push(currentCell);

                // Choose one of the unvisited neighbours
                var rdmNeighbor = neighbors[_random.Next(0, neighbors.Length)];

                // Remove the wall between the current cell and the chosen cell
                Cell.RemoveSharedWall(currentCell, rdmNeighbor);

                // Mark the chosen cell as visited and push it to the stack
                rdmNeighbor.WasVisited = true;
                cellsToVisit.Push(rdmNeighbor);
            }

            return _cells;
        }

        private IEnumerable<Cell> GetNeighbours(Cell cell)
        {
            var neighbours = new List<Vector2Int>();

            if (cell.X > 0)
                neighbours.Add(new Vector2Int(cell.X - 1, cell.Y));

            if (cell.X < _size.x - 1)
                neighbours.Add(new Vector2Int(cell.X + 1, cell.Y));

            if (cell.Y > 0)
                neighbours.Add(new Vector2Int(cell.X, cell.Y - 1));

            if (cell.Y < _size.y - 1)
                neighbours.Add(new Vector2Int(cell.X, cell.Y + 1));

            return neighbours.Select(n => _cells[n.x + n.y * _size.x]);
        }
    }

    public class Cell
    {
        public bool WasVisited;
        public CellState State = CellState.ALL;

        public int X;
        public int Y;

        public void RemoveWall(CellState wallToRemove) => State &= ~wallToRemove;

        public static void RemoveSharedWall(Cell cell1, Cell cell2)
        {
            if (cell1.X > cell2.X)
            {
                cell1.RemoveWall(CellState.LEFT);
                cell2.RemoveWall(CellState.RIGHT);
            }

            if (cell1.X < cell2.X)
            {
                cell1.RemoveWall(CellState.RIGHT);
                cell2.RemoveWall(CellState.LEFT);
            }

            if (cell1.Y > cell2.Y)
            {
                cell1.RemoveWall(CellState.BOTTOM);
                cell2.RemoveWall(CellState.TOP);

            }

            if (cell1.Y < cell2.Y)
            {
                cell1.RemoveWall(CellState.TOP);
                cell2.RemoveWall(CellState.BOTTOM);
            }
        }
    }

    [Flags]
    public enum CellState
    {
        /// <summary>
        /// Has no wall
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Has all four walls
        /// </summary>
        ALL = TOP | RIGHT | BOTTOM | LEFT,

        /// <summary>
        /// Has top wall
        /// </summary>
        TOP = 0b0001,

        /// <summary>
        /// Has right wall
        /// </summary>
        RIGHT = 0b0010,

        /// <summary>
        /// Has bottom wall
        /// </summary>
        BOTTOM = 0b0100,

        /// <summary>
        /// Has left wall
        /// </summary>
        LEFT = 0b1000,
    }
}