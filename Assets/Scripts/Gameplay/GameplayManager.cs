using UnityEngine;
using TacticsGame.Event;
using TacticsGame.Obstacle;
using TacticsGame.PathFinding;
using TacticsGame.Player;
using TacticsGame.TileGrid;

namespace TacticsGame.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerController _playerPrefab;
        [SerializeField] private Vector2Int _playerStartPosition;

        public static GameplayManager Instance { get; private set; }

        private TileGridService _tileGridServiceObj;
        private EventBusService _eventBusServiceObj;
        private PathfindingService _pathfindingServiceObj;
        private PlayerController _player;

        private GameplayTurn _currentTurn = GameplayTurn.None;

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

        public void Initialize(TileGridService tileGridService, PathfindingService pathfindingService, EventBusService eventBusService)
        {
            _tileGridServiceObj = tileGridService;
            _pathfindingServiceObj = pathfindingService;
            _eventBusServiceObj = eventBusService;
            _tileGridServiceObj.GenerateTileGrid();
            ObstacleManager.Instance.GenerateObstacles();
            SpawnPlayer();
            ChangeTurn(GameplayTurn.Player);
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
            _player.Initialize(_tileGridServiceObj, _pathfindingServiceObj, _eventBusServiceObj,_playerStartPosition);
        }

        private void ChangeTurn(GameplayTurn newTurn)
        {
            _currentTurn = newTurn;
            _eventBusServiceObj.Publish(new GameplayTurnChangedEvent(_currentTurn));
        }
    }
}