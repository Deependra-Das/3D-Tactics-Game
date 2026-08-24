using System.Collections.Generic;
using UnityEngine;

namespace TacticsGame.Obstacle
{
    /// <summary>
    /// Stores the obstacle configuration for the grid.
    /// Each Vector2Int represents the grid coordinate of a blocked tile.
    /// </summary>
    [CreateAssetMenu(fileName = "ObstacleData_SO", menuName = "ScriptableObjects/ObstacleData_SO")]
    public class ObstacleData_SO : ScriptableObject
    {
        // List of Grid coordinates containing obstacles.
        public List<Vector2Int> blockedTiles = new();
    }
}
