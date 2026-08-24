using TacticsGame.Event;
using TacticsGame.PathFinding;
using TacticsGame.TileGrid;
using UnityEngine;

namespace TacticsGame.Enemy
{
    public class EnemyAIController : MonoBehaviour
    {
        private Vector2Int _gridPosition;
        private TileGridService _tileGridServiceObj;
        private PathfindingService _pathfindingServiceObj;
        private EventBusService _eventBusServiceObj;

        public void Initialize(TileGridService tileGridService,PathfindingService pathfindingService, EventBusService eventBusService, Vector2Int startPosition)
        {
            _tileGridServiceObj = tileGridService;
            _pathfindingServiceObj = pathfindingService;
            _eventBusServiceObj = eventBusService;
            _gridPosition = startPosition;
        }
    }
}
