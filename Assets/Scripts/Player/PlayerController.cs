using TacticsGame.Main;
using TacticsGame.TileGrid;
using UnityEngine;

namespace TacticsGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Vector2Int _gridPosition;
        public Vector2Int GridPosition => _gridPosition;

        private TileGridService _tileGridServiceObj;

        private void Start()
        {
            _tileGridServiceObj  = GameManager.Instance.Services.Get<TileGridService>();

            _gridPosition = _tileGridServiceObj.GetWorldToGridPosition(transform.position);
        }
    }
}