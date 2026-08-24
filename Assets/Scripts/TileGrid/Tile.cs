using UnityEngine;

namespace TacticsGame.TileGrid
{
    /// <summary>
    /// This class represents one tile in the generated grid & holds its data like position and state.
    /// </summary>

    public class Tile : MonoBehaviour
    {
        // Position of this tile in grid coordinates.
        private Vector2Int _gridPosition;

        // Indicates whether the tile is currently blocked.
        private bool _isBlocked;

        // Indicates whether the tile is occupied by a Player or Enemy
        private bool _isOccupied;

        /// <summary>
        /// This function is called to initialize the tile data like position & isBlocked.
        /// </summary>
        public void Initialize(Vector2Int position)
        {
            _gridPosition = position;
            _isBlocked = false;
            _isOccupied = false;
            gameObject.name = $"Tile_{position.x}_{position.y}";
        }

        /// <summary>
        /// This function updates the blocked state of this tile.
        /// </summary>
        public void SetBlocked(bool value)
        {
            _isBlocked = value;
        }

        public void SetOccupied(bool value)
        {
            _isOccupied = value;
        }

        public bool IsBlocked => _isBlocked;

        public bool IsOccupied => _isOccupied;

        public Vector2Int GridPosition => _gridPosition;
    }
}