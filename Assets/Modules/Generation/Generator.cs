using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
namespace GenerationModule
{
    public class Generator : MonoBehaviour
    {
        public GameObject wall;
        public GameObject path;
        public NavMeshSurface[] surfaces;
        public GameObject gravestone;

        private System.Random random = new System.Random(10);

        private void Awake()
        {
            var maze = new MazeGenerator(10, 10, random).Generate();

            DrawMaze(maze);

            foreach (var surface in surfaces)
                surface.BuildNavMesh();

            for (int i = 0; i < 3; i++)
            {
                SpawnGravestones(maze[Random.Range(0, maze.Length)]);
            }
        }

        private void DrawMaze(Cell[] cells)
        {
            foreach (var item in cells)
            {
                DrawCell(item);
                SpawnPath(item);
            }
            
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

        private void SpawnWall(Cell cell, Vector2Int position, Vector3 rotation)
        {
            var g = Instantiate(wall, transform);
            g.name = $"Wall ({cell.X}; {cell.Y}) ({cell.State})";
            g.transform.SetLocalPositionAndRotation(
                new Vector3(position.x, 0, position.y) * 3,
                Quaternion.Euler(rotation)
            );
        }
        private void SpawnPath(Cell cell)
        {
            var g = Instantiate(path, transform);
            g.name = $"Path ({cell.X}; {cell.Y})";
            g.transform.SetLocalPositionAndRotation(
                new Vector3(cell.X, 0, cell.Y) * 3 + new Vector3(1.5f, 0, 1.5f),
                Quaternion.Euler(Vector3.up * Random.Range(-6, 6))
            );
            g.transform.localScale = Vector3.one * Random.Range(80, 100);
        }

        private void SpawnGravestones(Cell cell)
        {
            var g = Instantiate(gravestone, transform);
            g.name = $"Gravestone ({cell.X}; {cell.Y})";
            g.transform.SetLocalPositionAndRotation(
                new Vector3(cell.X, 0, cell.Y) * 3 + new Vector3(1.5f, 0, 1.5f),
                Quaternion.Euler(Vector3.up * Random.Range(-6, 6))
            );
            g.transform.localScale = Vector3.one * Random.Range(80, 100);
        }
    }
}