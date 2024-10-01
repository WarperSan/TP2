using UnityEngine;
namespace GenerationModule
{
    public class Generator : MonoBehaviour
    {
        public GameObject wall;
        public GameObject path;

        private void Awake()
        {
            var maze = new MazeGenerator(10, 10, 10).Generate();

            DrawMaze(maze);
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
            // Right wall
            if (cell.State.HasFlag(CellState.RIGHT))
                SpawnWall(cell, new Vector2Int(cell.X + 1, cell.Y), new Vector3(0, 180, 0));

            // Left wall
            if (cell.X == 0)
                SpawnWall(cell, new Vector2Int(cell.X, cell.Y), new Vector3(0, 180, 0));

            // Top wall
            if (cell.State.HasFlag(CellState.TOP))
                SpawnWall(cell, new Vector2Int(cell.X, cell.Y + 1), new Vector3(0, -90, 0));

            // Bottom wall
            if (cell.Y == 0)
                SpawnWall(cell, new Vector2Int(cell.X, cell.Y), new Vector3(0, -90, 0));
        }

        private void SpawnWall(Cell cell, Vector2Int position, Vector3 rotation)
        {
            var g = Instantiate(wall, transform);
            g.name = $"Wall ({cell.X}; {cell.Y}) ({cell.State})";
            g.transform.SetLocalPositionAndRotation(
                new Vector3(position.x, 0, position.y) * 2,
                Quaternion.Euler(rotation)
            );
        }
        private void SpawnPath(Cell cell)
        {
            var g = Instantiate(path, transform);
            g.name = $"Path ({cell.X}; {cell.Y})";
            g.transform.SetLocalPositionAndRotation(
                new Vector3(cell.X, 0, cell.Y) * 2 + new Vector3(1, 0, 1),
                Quaternion.Euler(Vector3.up * Random.Range(-6, 6))
            );
            g.transform.localScale = Vector3.one * Random.Range(60, 80);
        }
    }
}