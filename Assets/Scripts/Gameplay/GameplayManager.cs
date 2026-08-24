using UnityEngine;
using System.Collections.Generic;
using TacticsGame.Enemy;
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

        [Header("Enemy")]
        [SerializeField] private EnemyData_SO _enemyData;

        public static GameplayManager Instance { get; private set; }

        private TileGridService _tileGridServiceObj;
        private EventBusService _eventBusServiceObj;
        private PathfindingService _pathfindingServiceObj;
        private PlayerController _player;

        private GameplayTurn _currentTurn = GameplayTurn.None;

        // Stores references to all spawned Enemies.
        private readonly List<EnemyAIController> _spawnedEnemiesList = new List<EnemyAIController>();

        // Number of Enemies that have completed
        // their movement during the current Enemy turn.
        private int _movementCompletedEnemyCount;

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

        private void SubscribeToEvents()
        {
            _eventBusServiceObj.Subscribe<PlayerMovementCompletedEvent>(OnPlayerMovementCompleted);
        }

        private void UnsubscribeToEvents()
        {
            _eventBusServiceObj.Unsubscribe<PlayerMovementCompletedEvent>(OnPlayerMovementCompleted);
        }

        public void Initialize(TileGridService tileGridService, PathfindingService pathfindingService, EventBusService eventBusService)
        {
            _tileGridServiceObj = tileGridService;
            _pathfindingServiceObj = pathfindingService;
            _eventBusServiceObj = eventBusService;

            SubscribeToEvents();
            _tileGridServiceObj.GenerateTileGrid();
            ObstacleManager.Instance.GenerateObstacles();
            SpawnPlayer();
            SpawnEnemies();
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

        /// <summary>
        /// This function goes through the EnemyData and calls SpawnEnemy.
        /// </summary>
        private void SpawnEnemies()
        {
            if (_enemyData == null)
            {
                Debug.LogWarning("EnemyData has not been assigned.");

                return;
            }

            if (_enemyData.enemyPrefab == null)
            {
                Debug.LogError(
                    "Enemy prefab has not been assigned to EnemyData."
                );

                return;
            }

            if (_enemyData.enemySpawnPositionList == null)
            {
                Debug.LogWarning(
                    "Enemy spawn positions have not been configured."
                );

                return;
            }

            foreach (Vector2Int spawnPosition in _enemyData.enemySpawnPositionList)
            {
                // Prevent Enemies from spawning outside the generated grid.
                if (!_tileGridServiceObj.IsInsideGrid(spawnPosition))
                {
                    Debug.LogWarning($"Enemy spawn position " + $"{spawnPosition} is outside the grid.");
                    continue;
                }

                // Do not spawn an Enemy on an obstacle.
                if (_tileGridServiceObj.IsBlocked(spawnPosition))
                {
                    Debug.LogWarning($"Enemy spawn position " + $"{spawnPosition} is blocked.");
                    continue;
                }

                // Do not spawn an Enemy on the Player.
                if (spawnPosition == _playerStartPosition)
                {
                    Debug.LogWarning($"Enemy spawn position " + $"{spawnPosition} is occupied by the Player.");
                    continue;
                }

                SpawnEnemy(spawnPosition);
            }
        }

        /// <summary>
        /// This function creates and initializes Enemy.
        /// </summary>
        private void SpawnEnemy(Vector2Int spawnPosition)
        {
            Vector3 worldPosition = _tileGridServiceObj.GetGridToWorldPosition(spawnPosition);

            // Place the Enemy above the tile.
            worldPosition.y = 1.25f;

            EnemyAIController enemy = Instantiate(_enemyData.enemyPrefab, worldPosition, Quaternion.identity);

            enemy.name = $"Enemy_{_spawnedEnemiesList.Count}";

            // Initialize the Enemy with all required services.
            enemy.Initialize(_tileGridServiceObj, _pathfindingServiceObj, _eventBusServiceObj, spawnPosition);
            _spawnedEnemiesList.Add(enemy);
        }

        private void ChangeTurn(GameplayTurn newTurn)
        {
            _currentTurn = newTurn;
            RaiseGameplayTurnChangedEventEvent(_currentTurn);
        }

        private void OnPlayerMovementCompleted(PlayerMovementCompletedEvent eventData)
        {
            if (_currentTurn != GameplayTurn.Player)
                return;

            ChangeTurn(GameplayTurn.Enemy);
        }

        private void RaiseGameplayTurnChangedEventEvent(GameplayTurn currentTurn)
        {
            _eventBusServiceObj.Publish(new GameplayTurnChangedEvent(_currentTurn));
        }

        private void OnDestroy()
        {
            if (_eventBusServiceObj != null)
            {
                UnsubscribeToEvents();
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}