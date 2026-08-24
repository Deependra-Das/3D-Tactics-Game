using TacticsGame.Gameplay;

namespace TacticsGame.Event
{
    public class GameplayTurnChangedEvent
    {
        public readonly GameplayTurn CurrentTurn;

        public GameplayTurnChangedEvent(GameplayTurn turn)
        {
            CurrentTurn = turn;
        }
    }
}
