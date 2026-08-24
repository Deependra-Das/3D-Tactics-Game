using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using TacticsGame.TileGrid;

namespace TacticsGame.UI
{
    public class TileHoverUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Canvas _tileCanvas;
        [SerializeField] private GameObject _tileHoverContainer;
        [SerializeField] private TMP_Text _tilePositionText;

        [Header("Raycast")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask _tileLayer;

        [Header("Position")]
        [SerializeField] private float _heightOffset = 1.5f;

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            HideTileInformation();
        }

        private void Update()
        {
            UpdateTileInformation();
        }

        private void LateUpdate()
        {
            FaceCamera();
        }

        /// <summary>
        /// This function updates the information of tile mouse is hovering over and calls DisplayTileInformation
        /// </summary>
        private void UpdateTileInformation()
        {
            if (_mainCamera == null || _tileCanvas == null || _tilePositionText == null)
            {
                return;
            }

            if (Mouse.current == null)
            {
                return;
            }

            Vector2 mousePosition = Mouse.current.position.ReadValue();

            Ray mouseRay = _mainCamera.ScreenPointToRay(mousePosition);

            if (!Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, _tileLayer))
            {
                HideTileInformation();
                return;
            }

            Tile tile = hit.collider.GetComponentInParent<Tile>();

            if (tile == null)
            {
                HideTileInformation();
                return;
            }

            DisplayTileInformation(tile);
        }

        /// <summary>
        /// This function shows the Grid position of the Tile on which mouse is hovering
        /// </summary>
        private void DisplayTileInformation(Tile tile)
        {
            Vector2Int gridPosition =  tile.GridPosition;

            _tilePositionText.text = $"({gridPosition.x}, {gridPosition.y})";

            Vector3 worldPosition = tile.transform.position;

            worldPosition.y += _heightOffset;

            _tileHoverContainer.transform.position =  worldPosition;

            if (!_tileHoverContainer.activeSelf)
            {
                _tileHoverContainer.SetActive(true);
            }
        }

        private void HideTileInformation()
        {
            if (_tileHoverContainer != null)
            {
                _tileHoverContainer.SetActive(false);
            }
        }

        private void FaceCamera()
        {
            if (_tileHoverContainer == null || !_tileHoverContainer.activeSelf || _mainCamera == null)
            {
                return;
            }

            Vector3 direction = _tileHoverContainer.transform.position - _mainCamera.transform.position;

            if (direction.sqrMagnitude <= 0.001f)
            {
                return;
            }

            _tileHoverContainer.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}