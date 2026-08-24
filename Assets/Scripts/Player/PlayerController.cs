using System.Collections;
using System.Collections.Generic;
using TacticsGame.Event;
using TacticsGame.Gameplay;
using TacticsGame.Main;
using TacticsGame.PathFinding;
using TacticsGame.TileGrid;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TacticsGame.Player
{
    /// <summary>
    /// This class controls the player unit movement & handles player input.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActionObj;

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 2f;

        private Vector2Int _gridPosition;
        public Vector2Int GridPosition => _gridPosition;

        private TileGridService _tileGridServiceObj;
        private PathfindingService _pathfindingServiceObj;
        private EventBusService _eventBusServiceObj;
        private InputAction m_interactAction;
        private Camera _mainCamera;

        private bool _isMoving;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_inputActionObj == null)
            {
                Debug.LogError("Input Action Asset has not been assigned.");
                return;
            }

            InputActionMap playerActionMap = _inputActionObj.FindActionMap("Player", true);
            m_interactAction =playerActionMap.FindAction("Interact", true);
        }

        public void Initialize(TileGridService tileGridService, PathfindingService pathfindingService, EventBusService eventBusService, Vector2Int startPosition)
        {
            _tileGridServiceObj = tileGridService;
            _pathfindingServiceObj = pathfindingService;
            _eventBusServiceObj = eventBusService;

            _gridPosition = startPosition;

            _isMoving = false;

            SubscribeToEvents();
            DisableInput();
        }

        private void SubscribeToEvents()
        {
            _eventBusServiceObj.Subscribe<GameplayTurnChangedEvent>(HandleGameplayTurnChanged);
        }

        private void UnsubscribeToEvents()
        {
            _eventBusServiceObj.Unsubscribe<GameplayTurnChangedEvent>(HandleGameplayTurnChanged);
        }

        private void EnableInput()
        {
            if (m_interactAction == null)
                return;

            m_interactAction.Enable();
        }

        private void DisableInput()
        {
            if (m_interactAction == null)
                return;

            m_interactAction.Disable();
        }

        private void Update()
        {
            HandlePlayerInput();
        }

        private void HandlePlayerInput()
        {
            if (!m_interactAction.WasPressedThisFrame())
                return;

            if (_isMoving)
                return;

            TrySelectTile();
        }

        private void TrySelectTile()
        {
            if (_mainCamera == null)
            {
                Debug.LogWarning("No Main Camera was found");
                return;
            }

            // Create a ray from the camera through the mouse cursor.
            Ray mouseRay = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            // Check whether the ray hits an object.
            if (!Physics.Raycast(mouseRay, out RaycastHit hit))
            {
                return;
            }

            // Check whether the hit object is a Tile.
            Tile selectedTile = hit.collider.GetComponent<Tile>();

            if (selectedTile == null)
                return;

            // A blocked tile cannot be selected.
            if (selectedTile.IsBlocked)
                return;

            MoveToTile(selectedTile.GridPosition);
        }

        /// <summary>
        /// This function requests a path from the current Player position
        /// to the selected destination.
        /// </summary>
        private void MoveToTile(Vector2Int targetPosition)
        {
            if (_pathfindingServiceObj == null)
            {
                Debug.LogError( "PathfindingService could not be found.");
                return;
            }

            List<Vector2Int> path = _pathfindingServiceObj.FindShortestPath(_gridPosition,targetPosition);

            // No valid path exists.
            if (path == null)
            {
                Debug.Log( $"No path exists from " +$"{_gridPosition} to " + $"{targetPosition}.");
                return;
            }

            // The Player is already on the selected tile.
            if (path.Count == 0)
                return;

            StartCoroutine(MoveAlongPath(path));
        }

        /// <summary>
        /// This function gets the world position of every tile 
        /// contained in the calculated path & calls MoveToPosition
        /// </summary>
        private IEnumerator MoveAlongPath(List<Vector2Int> path)
        {
            _isMoving = true;

            foreach (Vector2Int targetPosition in path)
            {
                // Convert the grid position into world space.
                Vector3 targetWorldPosition = _tileGridServiceObj.GetGridToWorldPosition(targetPosition);

                // Preserve the Player's height above the grid.
                targetWorldPosition.y = transform.position.y;

                yield return MoveToPosition(targetWorldPosition);

                _gridPosition = targetPosition;
            }

            // Movement is complete.
            _isMoving = false;
        }

        /// <summary>
        /// This function moves the Player toward the target position.
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

        /// <summary>
        /// Reacts to a change in the active gameplay turn.
        ///
        /// Player input is enabled only during the Player turn.
        /// </summary>
        private void HandleGameplayTurnChanged(GameplayTurnChangedEvent eventData)
        {
            Debug.Log(eventData.CurrentTurn.ToString());
            if (eventData.CurrentTurn == GameplayTurn.Player)
            {
                if (!_isMoving)
                {
                    EnableInput();
                }
            }
            else
            {
                DisableInput();
            }
        }

        private void OnDestroy()
        {
            if (_eventBusServiceObj == null)
                return;

            UnsubscribeToEvents();
        }

    }
}