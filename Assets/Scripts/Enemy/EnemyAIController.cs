using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TacticsGame.Event;
using TacticsGame.Gameplay;
using TacticsGame.PathFinding;
using TacticsGame.Player;
using TacticsGame.TileGrid;


namespace TacticsGame.Enemy
{
    public class EnemyAIController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 2f;

        private Vector2Int _gridPosition;
        private TileGridService _tileGridServiceObj;
        private PathfindingService _pathfindingServiceObj;
        private EventBusService _eventBusServiceObj;
        private bool _isMoving;

        public void Initialize(TileGridService tileGridService,PathfindingService pathfindingService, EventBusService eventBusService, Vector2Int startPosition)
        {
            _tileGridServiceObj = tileGridService;
            _pathfindingServiceObj = pathfindingService;
            _eventBusServiceObj = eventBusService;
            _gridPosition = startPosition;
            _isMoving = false;

            Tile currentTile = _tileGridServiceObj.GetTile(_gridPosition);

            if (currentTile != null)
            {
                currentTile.SetOccupied(true);
            }
        }

        public void ExecuteTurn()
        {
            if (_isMoving) return; 

            PlayerController player = GameplayManager.Instance.Player;

            if (player == null)
            {
                Debug.LogWarning("Enemy could not find the Player.");

                RaiseEnemyMovementCompletedEvent();
                return;
            }

            Vector2Int playerPosition = player.GridPosition;

            // determine which tile adjacent to the Player should be targeted.
            Vector2Int targetPosition = GetTargetPosition( _gridPosition, playerPosition);

            // If the Enemy is already in the best position, there is no movement required.
            if (targetPosition == _gridPosition)
            {
                RaiseEnemyMovementCompletedEvent();
                return;
            }

            // Calculate a path from the Enemy's current position to the selected target.
            List<Vector2Int> path = _pathfindingServiceObj.FindShortestPath(_gridPosition, targetPosition);

            // No valid path exists, return
            if (path == null)
            {
                Debug.Log($"Enemy at {_gridPosition} " + $"could not find a path to " + $"{targetPosition}."
                );

                RaiseEnemyMovementCompletedEvent();
                return;
            }

            // Start movement along the path.
            StartCoroutine(MoveAlongPath(path));
        }

        /// <summary>
        /// Determines which tile the Enemy should target.
        /// </summary>
        public Vector2Int GetTargetPosition(Vector2Int currentPosition, Vector2Int playerPosition)
        {
            Vector2Int[] adjacentPositions =
            {
                playerPosition + Vector2Int.up,
                playerPosition + Vector2Int.down,
                playerPosition + Vector2Int.left,
                playerPosition + Vector2Int.right
            };

            // If no valid adjacent position can be found, the Enemy remains where it is.
            Vector2Int bestPosition = currentPosition;

            // Stores the length of the shortest valid path found.
            int shortestPathLength = int.MaxValue;

            foreach (Vector2Int adjacentPosition in adjacentPositions)
            {
                // Ignore positions outside the grid.
                if (!_tileGridServiceObj.IsInsideGrid(adjacentPosition))
                {
                    continue;
                }

                // Ignore obstacle tiles.
                if (_tileGridServiceObj.IsBlocked(adjacentPosition))
                {
                    continue;
                }

                // Avoid moving onto the Player's current tile.
                if (adjacentPosition == playerPosition)
                {
                    continue;
                }

                // Determine whether the Enemy can reach the calculated adjacent position.
                List<Vector2Int> path = _pathfindingServiceObj.FindShortestPath(currentPosition, adjacentPosition);

                // Ignore unreachable positions.
                if (path == null)
                {
                    continue;
                }

                // Select the adjacent tile with the shortest path.
                if (path.Count < shortestPathLength)
                {
                    shortestPathLength = path.Count;
                    bestPosition = adjacentPosition;
                }
            }

            return bestPosition;
        }

        /// <summary>
        /// This function moves the Enemy through every tile/grid position
        /// contained in the calculated path.
        /// </summary>
        private IEnumerator MoveAlongPath(List<Vector2Int> path)
        {
            _isMoving = true;

            foreach (Vector2Int targetPosition in path)
            {
                // Convert the target grid position into its corresponding world position.
                Vector3 targetWorldPosition = _tileGridServiceObj.GetGridToWorldPosition(targetPosition);

                // Preserve the Enemy's current height above the grid.
                targetWorldPosition.y = transform.position.y;

                yield return MoveToPosition(targetWorldPosition);

                Tile previousTile = _tileGridServiceObj.GetTile(_gridPosition);

                Tile newTile = _tileGridServiceObj.GetTile(targetPosition);

                if (previousTile != null)
                {
                    previousTile.SetOccupied(false);
                }

                if (newTile != null)
                {
                    newTile.SetOccupied(true);
                }

                _gridPosition = targetPosition;
            }

            _isMoving = false;
            RaiseEnemyMovementCompletedEvent();
        }

        /// <summary>
        /// Smoothly moves the Enemy toward a target world-space position.
        /// </summary>
        private IEnumerator MoveToPosition(Vector3 targetPosition)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
        }

        private void RaiseEnemyMovementCompletedEvent()
        {
            if (_eventBusServiceObj == null)
                return;

            _eventBusServiceObj.Publish(new EnemyMovementCompletedEvent(_gridPosition));
        }
    }
}
