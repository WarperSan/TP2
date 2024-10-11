using UnityEngine;

namespace GenerationModule
{
    [CreateAssetMenu(fileName = "MazeDrawer", menuName = "SO/MazeDrawer", order = 0)]
    public class MazeDrawer : ScriptableObject
    {
        [Header("Stats")]
        public float cellSize = 3;

        [Header("Wall")]
        public GameObject wallPrefab;

        [Header("Path")]
        public GameObject pathPrefab;

        [Header("Gravestone")]
        public GameObject gravestonePrefab;

        [Min(1)]
        public int gravestoneCount = 1;

        [Header("Props")]
        public GameObject[] propsPrefab;
    }
}