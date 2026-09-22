using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Battle/Enemy Data")]
public class EnemySO : ScriptableObject
{
    public string enemyName;

    // CHANGE THIS: From Sprite to GameObject
    [Tooltip("The Prefab GameObject for this enemy (must contain a SpriteRenderer and EnemyController)")]
    public GameObject enemyPrefab;

    public int maxHealth = 20;
    public List<AttackSO> attacks = new List<AttackSO>();
}