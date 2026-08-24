using UnityEngine;

namespace TacticsGame.PathFinding
{
    /// <summary>
    /// This class contains the standard movement directions used by
    /// grid-based systems.
    /// </summary>
    public static class GridDirections
    {
        /// <summary>
        /// The four directions available for grid movement.
        /// </summary>
        public static readonly Vector2Int[] MovementDirections =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
    }
}