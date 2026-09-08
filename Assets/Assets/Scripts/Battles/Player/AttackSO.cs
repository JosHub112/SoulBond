using UnityEngine;

/// <summary>
/// A single attack/skill. Create one asset per move via
/// Assets > Create > Battle System > Attack.
/// </summary>
[CreateAssetMenu(fileName = "New Attack", menuName = "Battle System/Attack")]
public class AttackSO : ScriptableObject
{
    public string attackName;
    public int damage;
    [Range(0f, 100f)]
    public float accuracy = 100f;

    [Header("Visuals")]
    //the animation that plays
    public AnimationClip attackAnimation;

    public GameObject hitEffectPrefab;
}