using UnityEngine;

public abstract class AttackSO : ScriptableObject
{
    [Header("Base Properties")]
    public GameObject hazardPrefab;
    public int damage = 1;
    public float projectileSpeed = 5f;

    // Every unique attack pattern implements this method with its own math!
    public abstract void Execute(Vector3 arenaCenter, Transform playerTransform, HazardSpawner spawner);
}