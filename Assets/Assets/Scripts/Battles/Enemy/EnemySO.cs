using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Battle/Enemy Data")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public Sprite enemySprite;
    public int maxHealth = 20;

    
    public List<AttackSO> attacks = new List<AttackSO>();
}