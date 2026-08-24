using UnityEngine;

namespace TacticsGame.Event
{
    public class EnemyMovementCompletedEvent 
    {
        public Vector2Int FinalPosition { get; }

        public EnemyMovementCompletedEvent(Vector2Int finalPosition)
        {
            FinalPosition = finalPosition;
        }
    }
}