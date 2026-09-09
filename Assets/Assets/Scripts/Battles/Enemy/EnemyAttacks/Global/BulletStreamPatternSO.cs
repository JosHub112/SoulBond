using UnityEngine;

[CreateAssetMenu(fileName = "BulletStream", menuName = "Attacks/Bullet Stream")]
public class BulletStreamPatternSO : AttackSO
{
    [Header("Stream Settings")]
    public int bulletCount = 1;
    public float spawnWidth = 4f;       // Horizontal spread range (-4 to 4)
    public float spawnHeightOffset = 5f; // Distance above the arena center

    public override void Execute(Vector3 arenaCenter, Transform playerTransform, HazardSpawner spawner)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float randomX = Random.Range(-spawnWidth, spawnWidth);
            Vector3 spawnPos = arenaCenter + new Vector3(randomX, spawnHeightOffset, 0f);
            Vector2 dir = Vector2.down;

            spawner.CreateHazardObject(hazardPrefab, spawnPos, dir, damage, projectileSpeed);
        }
    }
}