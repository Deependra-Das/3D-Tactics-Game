using UnityEngine;
using UnityEngine.InputSystem;
using TacticsGame.Main;
using TacticsGame.TileGrid;

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

        private InputAction m_interactAction;
        private Camera _mainCamera;

        private bool _isMoving;

        private void OnEnable()
        {
            _inputActionObj.FindActionMap("Player").Enable();
        }

        private void OnDisable()
        {
            _inputActionObj.FindActionMap("Player").Disable();
        }

        private void Awake()
        {
            m_interactAction = InputSystem.actions.FindAction("Interact");
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            _tileGridServiceObj  = GameManager.Instance.Services.Get<TileGridService>();
            _gridPosition = _tileGridServiceObj.GetWorldToGridPosition(transform.position);
            _isMoving = false;
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

            Debug.Log(selectedTile.name);
        }
    }
}