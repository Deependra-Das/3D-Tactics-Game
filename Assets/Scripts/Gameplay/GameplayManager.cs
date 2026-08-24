using TacticsGame.Obstacle;
using TacticsGame.Player;
using TacticsGame.TileGrid;
using UnityEngine;

namespace TacticsGame.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerController _playerPrefab;
        [SerializeField] private Vector2Int _playerStartPosition;

        public static GameplayManager Instance { get; private set; }

        private TileGridService _tileGridServiceObj;
        private PlayerController _player;

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

        public void Initialize(TileGridService tileGridService )
        {
            _tileGridServiceObj = tileGridService;
            _tileGridServiceObj.GenerateTileGrid();
            ObstacleManager.Instance.GenerateObstacles();
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            if (_playerPrefab == null)
            {
                Debug.LogError("Player prefab has not been assigned.");

                return;
            }

            if (!_tileGridServiceObj.IsInsideGrid(_playerStartPosition))
            {
                Debug.LogError("Player start position is outside the grid.");

                return;
            }

            Vector3 worldPosition = _tileGridServiceObj.GetGridToWorldPosition(_playerStartPosition);
            worldPosition.y = 1.25f;
            _player = Instantiate(_playerPrefab, worldPosition, Quaternion.identity);
            _player.name = "Player";
        }
    }
}