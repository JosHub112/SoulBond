using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Enemy template. Create one asset per enemy type via
/// Assets > Create > Battle System > Enemy.
/// </summary>
[CreateAssetMenu(fileName = "New Enemy", menuName = "Battle System/Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public int maxHP;
    public Sprite enemySprite;

    [Header("Animations & Actions")]
    public AnimationClip idleAnimation;

    //all the attacks that the enemy does 
    public List<AttackSO> attacks = new List<AttackSO>();
}