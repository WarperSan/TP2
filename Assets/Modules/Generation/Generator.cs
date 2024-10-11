using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

namespace GenerationModule
{
    public class Generator : MonoBehaviour
    {
        private const float CELL_SIZE = 3f;

        public NavMeshSurface[] surfaces;

        private readonly System.Random random = new(10);

        private void Awake()
        {
            var maze = new MazeGenerator(10, 10, random).Generate();

            DrawMaze(maze);

            foreach (var surface in surfaces)
                surface.BuildNavMesh();
        }

        private void DrawMaze(Cell[] cells)
        {
            foreach (var item in cells)
            {
                DrawCell(item);
                SpawnPath(item);
            }

            DrawGraveStones(cells);
        }

        private void DrawCell(Cell cell)
        {
            // Left wall
            if (cell.X == 0)
                SpawnWall(cell, new Vector2Int(cell.X, cell.Y), new Vector3(0, 180, 0));

            // Bottom wall
            if (cell.Y == 0)
                SpawnWall(cell, new Vector2Int(cell.X, cell.Y), new Vector3(0, -90, 0));

            // Right wall
            if (cell.State.HasFlag(CellState.RIGHT))
                SpawnWall(cell, new Vector2Int(cell.X + 1, cell.Y), new Vector3(0, 180, 0));

            // Top wall
            if (cell.State.HasFlag(CellState.TOP))
                SpawnWall(cell, new Vector2Int(cell.X, cell.Y + 1), new Vector3(0, -90, 0));
        }

        #region Wall

        [Header("Wall")]
        [SerializeField]
        private GameObject wall;

        private void SpawnWall(Cell cell, Vector2Int position, Vector3 rotation)
        {
            var g = Instantiate(wall, transform);
            g.name = $"Wall ({cell.X}; {cell.Y}) ({cell.State})";
            g.transform.SetLocalPositionAndRotation(
                new Vector3(position.x, 0, position.y) * CELL_SIZE,
                Quaternion.Euler(rotation)
            );
        }

        #endregion

        #region Path

        [Header("Path")]
        [SerializeField]
        private GameObject path;

        private void SpawnPath(Cell cell)
        {
            var g = Instantiate(path, transform);
            g.name = $"Path ({cell.X}; {cell.Y})";
            g.transform.SetLocalPositionAndRotation(
                new Vector3(cell.X, 0, cell.Y) * CELL_SIZE + new Vector3(CELL_SIZE, 0, CELL_SIZE) / 2,
                Quaternion.Euler(Vector3.up * Random.Range(-6, 6))
            );
            g.transform.localScale = Vector3.one * Random.Range(80, 100);
        }

        #endregion

        #region Gravestone

        [Header("Gravestone")]
        [SerializeField]
        private GameObject gravestone;

        [SerializeField, Min(1)]
        private int gravestoneCount = 1;

        private void DrawGraveStones(Cell[] cells)
        {
            Cell[] copy = cells
                .Where(c => c.State != CellState.NONE)
                .Where(c => c.IsInCorner())
                .ToArray();

            for (int i = 0; i < Mathf.Min(copy.Length, gravestoneCount); i++)
            {
                int rdmIndex = random.Next(0, copy.Length - i);

                SpawnGravestones(copy[rdmIndex]);

                // Swap with end
                (copy[rdmIndex], copy[^(i + 1)]) = (copy[^(i + 1)], copy[rdmIndex]);
            }
        }

        private void SpawnGravestones(Cell cell)
        {
            float rotation;
            Vector3 offset = new(1f, 0, 1f);

            if (cell.State.HasFlag(CellState.TOP | CellState.RIGHT))
            {
                rotation = -135f;
            }
            else if (cell.State.HasFlag(CellState.TOP | CellState.LEFT))
            {
                rotation = 135f;
                offset.x *= -1f;
            }
            else if (cell.State.HasFlag(CellState.BOTTOM | CellState.RIGHT))
            {
                rotation = -45f;
                offset.z *= -1;
            }
            else
            {
                rotation = 45f;
                offset.x *= -1;
                offset.z *= -1;
            }

            var g = Instantiate(gravestone, transform);
            g.name = $"Gravestone ({cell.X}; {cell.Y})";
            g.transform.SetLocalPositionAndRotation(
                new Vector3(cell.X, 0, cell.Y) * CELL_SIZE + new Vector3(CELL_SIZE, 0, CELL_SIZE) / 2 + offset,
                Quaternion.Euler(0, rotation, 0)
            );
        }

        #endregion
    }
}