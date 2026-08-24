using System.Collections.Generic;
using UnityEngine;

namespace TacticsGame.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData_SO", menuName = "ScriptableObjects/EnemyData_SO")]
    public class EnemyData_SO : ScriptableObject
    {
        public EnemyAIController enemyPrefab;
        public List<Vector2Int> enemySpawnPositionList = new List<Vector2Int>();
    }
}
