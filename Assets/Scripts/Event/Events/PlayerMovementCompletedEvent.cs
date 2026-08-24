using UnityEngine;

namespace TacticsGame.Event
{
    public class PlayerMovementCompletedEvent 
    {
        public Vector2Int FinalPosition { get; }

        public PlayerMovementCompletedEvent(Vector2Int finalPosition)
        {
            FinalPosition = finalPosition;
        }
    }
}