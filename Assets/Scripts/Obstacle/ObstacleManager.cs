using UnityEngine;
using TacticsGame.TileGrid;

namespace TacticsGame.Obstacle
{
    /// <summary>
    /// This class manages all runtime obstacles in the scene by reading obstacle positions 
    /// from the ObstacleData ScriptableObject, marks the corresponding Tiles as blocked,
    /// and creates the visual obstacle spheres.
    /// </summary>
    public class ObstacleManager : MonoBehaviour
    {
        [SerializeField] private ObstacleData_SO _obstacleData;
        [SerializeField] private GameObject _obstaclePrefab;
        public static ObstacleManager Instance { get; private set; }

        private TileGridService _tileGridServiceObj;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Initialize(TileGridService tileGridService)
        {
            _tileGridServiceObj = tileGridService;
        }

        /// <summary>
        /// This function reads the data from the obstacleData_SO, 
        /// sets the blocked state of the corresponding Tile and 
        /// calls SpawnObstacle to create the obstacle sphere.
        /// </summary>
        public void GenerateObstacles()
        {
            if (_obstacleData == null)
            {
                Debug.LogWarning("ObstacleData has not been assigned.");
                return;
            }

            if (_obstaclePrefab == null)
            {
                Debug.LogWarning("Obstacle prefab has not been assigned.");
                return;
            }

            foreach (Vector2Int position in _obstacleData.blockedTiles)
            {
                if (!_tileGridServiceObj.IsInsideGrid(position))
                {
                    continue;
                }

                Tile tile = _tileGridServiceObj.GetTile(position);

                if (tile == null)
                    continue;

                tile.SetBlocked(true);
                SpawnObstacle(position);
            }
        }

        /// <summary>
        /// This function creates the obstacle visual at the specified grid position.
        /// </summary>
        private void SpawnObstacle(Vector2Int position)
        {
            Vector3 worldPosition = _tileGridServiceObj.GetGridToWorldPosition(position);

            // Raise the sphere so it sits on top of the tile.
            worldPosition.y = 0.75f;

            GameObject obstacle = Instantiate(_obstaclePrefab, worldPosition, Quaternion.identity);
            obstacle.name = $"Obstacle_{position.x}_{position.y}";
        }
    }
}