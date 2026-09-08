using UnityEngine;


/// A single attack/skill. Create one asset per move via
/// Assets > Create > Battle System > Attack.
/// 
public enum AttackPatternType
{
    BulletStream,  // Direct shots aimed at player
    RadialBurst,   // 360-degree ring of bullets
    AoECircle      // Spawns expanding danger zones near player
}

[CreateAssetMenu(fileName = "New Attack", menuName = "Battle System/Attack")]
public class AttackSO : ScriptableObject
{
    public string attackName;
    public int damage = 5;

    [Header("Bullet Hell Config")]
    public AttackPatternType patternType;
    public GameObject hazardPrefab; // Bullet, Laser, or AoE Prefab
    public float attackDuration = 5f; // How long enemy turn lasts
    public float fireRate = 0.5f;     // Seconds between spawns
    public float projectileSpeed = 5f;

    [Header("Settings")]
    public int bulletCountPerBurst = 8; // Used for Radial bursts

    [Header("Visuals")]
    public AnimationClip attackAnimation;
    public GameObject hitEffectPrefab;
    internal float spawnInterval;
}



