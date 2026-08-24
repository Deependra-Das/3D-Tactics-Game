using UnityEngine;

namespace TacticsGame.Enemy
{
    /// <summary>
    /// Declares the behaviour that an EnemyAI unit must provide.
    /// </summary>
    public interface IAI
    {
        /// <summary>
        /// Executes the EnemyAI's behaviour for its current turn.
        /// </summary>
        void ExecuteTurn();

        /// <summary>
        /// Determines the grid position that the AI wants to reach.
        /// </summary>
        Vector2Int GetTargetPosition(Vector2Int currentPosition, Vector2Int playerPosition);
    }
}