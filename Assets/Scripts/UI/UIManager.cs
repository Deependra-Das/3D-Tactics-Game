using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TacticsGame.Event;
using TacticsGame.Gameplay;

namespace TacticsGame.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _turnText;
        [SerializeField] private Image _turnBackground;
        [Header("Player Turn")]
        [SerializeField] private string _playerTurnText = "PLAYER TURN";
        [SerializeField] private Color _playerTurnColor = Color.blue;

        [Header("Enemy Turn")]
        [SerializeField] private string _enemyTurnText = "ENEMY TURN";
        [SerializeField] private Color _enemyTurnColor = Color.yellow;

        [Header("None Turn")]
        [SerializeField] private string _noneTurnText = "";
        [SerializeField] private Color _noneTurnColor = Color.white;

        public static UIManager Instance { get; private set; }

        private EventBusService _eventBusServiceObj;

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

        public void Initialize(EventBusService eventBusService)
        {
            _eventBusServiceObj = eventBusService;

            if (_eventBusServiceObj == null)
            {
                Debug.LogError("EventBusService could not be found.");
                return;
            }

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _eventBusServiceObj.Subscribe<GameplayTurnChangedEvent>(HandleGameplayTurnChanged);
        }

        private void UnsubscribeFromEvents()
        {
            if (_eventBusServiceObj == null)
            {
                return;
            }

            _eventBusServiceObj.Unsubscribe<GameplayTurnChangedEvent>(HandleGameplayTurnChanged);
        }

        private void HandleGameplayTurnChanged(GameplayTurnChangedEvent eventData)
        {
            switch (eventData.CurrentTurn)
            {
                case GameplayTurn.Player:
                    UpdateTurnUI(_playerTurnText,_playerTurnColor);
                    break;

                case GameplayTurn.Enemy:
                    UpdateTurnUI( _enemyTurnText, _enemyTurnColor);
                    break;

                case GameplayTurn.None:
                default:
                    UpdateTurnUI(_noneTurnText, _noneTurnColor);
                    break;
            }
        }

        private void UpdateTurnUI(string turnText, Color backgroundColor)
        {
            if (_turnText != null)
            {
                _turnText.text = turnText;
            }

            if (_turnBackground != null)
            {
                _turnBackground.color = backgroundColor;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}
