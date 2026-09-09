using UnityEngine;

[CreateAssetMenu(fileName = "AoECircle", menuName = "Attacks/AoE Circle")]
public class AoECirclePatternSO : AttackSO
{
    public int bulletCount = 3;
    public float radius = 1.5f;

    public override void Execute(Vector3 arenaCenter, Transform playerTransform, HazardSpawner spawner)
    {
        if (playerTransform == null) return;

        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 spawnPos = playerTransform.position + new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), 0f);
            spawner.CreateHazardObject(hazardPrefab, spawnPos, Vector2.zero, damage, projectileSpeed);
        }
    }
}