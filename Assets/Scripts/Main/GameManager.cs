using UnityEngine;

namespace TacticsGame.Main
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public ServiceLocator Services { get; private set; }

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
        }

        private void InitializeServices()
        {
            Services = new ServiceLocator();
        }
    }
}
