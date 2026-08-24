using UnityEngine;
using TacticsGame.Event;
using TacticsGame.TileGrid;

namespace TacticsGame.Main
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TileGrid_SO _tileGrid_SO;
        public static GameManager Instance { get; private set; }

        public ServiceLocator Services { get; private set; }
        private EventBusService _eventBusService;
        private TileGridService _tileGridService;

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
            _tileGridService.GenerateTileGrid();
        }

        private void InitializeServices()
        {
            Services = new ServiceLocator();
            _eventBusService = new EventBusService();
            _tileGridService = new TileGridService(_tileGrid_SO);
        }

        private void RegisterServices()
        {
            Services.Register(_eventBusService);
            Services.Register(_tileGridService);
        }
    }
}
