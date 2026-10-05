using UnityEngine;

public abstract class AttackSO : ScriptableObject
{
    [Header("Base Properties")]
    public GameObject hazardPrefab;
    public int damage = 1;
    public float projectileSpeed = 5f;

    [Header("Audio")]
    public AudioClip attackSFX;

    //Causes a unique attack pattern
    public abstract void Execute(Vector3 arenaCenter, Transform playerTransform, HazardSpawner spawner);
}