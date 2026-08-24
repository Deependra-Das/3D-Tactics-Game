using UnityEngine;
using TacticsGame.Event;
using TacticsGame.TileGrid;
using TacticsGame.Obstacle;
using TacticsGame.Gameplay;
using TacticsGame.PathFinding;
using TacticsGame.UI;

namespace TacticsGame.Main
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TileGrid_SO _tileGrid_SO;
        public static GameManager Instance { get; private set; }

        public ServiceLocator Services { get; private set; }
        private EventBusService _eventBusService;
        private TileGridService _tileGridService;
        private PathfindingService _pathfindingService;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeServices();
            RegisterServices();
            UIManager.Instance.Initialize(_eventBusService);
            ObstacleManager.Instance.Initialize(_tileGridService);
            GameplayManager.Instance.Initialize(_tileGridService, _pathfindingService, _eventBusService);
        }

        private void InitializeServices()
        {
            Services = new ServiceLocator();
            _eventBusService = new EventBusService();
            _tileGridService = new TileGridService(_tileGrid_SO);
            _pathfindingService = new PathfindingService(_tileGridService);
        }

        private void RegisterServices()
        {
            Services.Register(_eventBusService);
            Services.Register(_tileGridService);
            Services.Register(_pathfindingService);
        }
    }
}
