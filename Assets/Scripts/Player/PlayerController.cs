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
        }
    }
}