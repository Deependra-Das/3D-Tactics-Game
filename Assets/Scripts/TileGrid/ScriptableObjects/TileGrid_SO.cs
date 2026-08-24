using UnityEngine;

namespace TacticsGame.TileGrid
{
    /// <summary>
    /// Stores configuration information for the tile grid.
    /// </summary>

    [CreateAssetMenu(fileName = "TileGrid_SO", menuName = "ScriptableObjects/TileGrid_SO")]
    public class TileGrid_SO : ScriptableObject
    {
        [Header("Tile Prefab")]

        public Tile tilePrefab;

        [Header("Tile Grid Size")]

        // Number of tiles along the X axis.
        public int gridWidth;

        // Number of tiles along the Z axis.
        public int gridHeight;

        [Header("Tile Settings")]

        // Distance between the centers of two adjacent tiles.
        public float tileSize;
    }
}