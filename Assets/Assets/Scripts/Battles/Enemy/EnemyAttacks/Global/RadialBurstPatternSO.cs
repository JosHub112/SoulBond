using UnityEngine;

[CreateAssetMenu(fileName = "RadialBurst", menuName = "Attacks/Radial Burst")]
public class RadialBurstPatternSO : AttackSO
{
    public int bulletCount = 12;
    public Vector3 spawnOffset = new Vector3(0, 3f, 0);

    public override void Execute(Vector3 arenaCenter, Transform playerTransform, HazardSpawner spawner)
    {
        float angleStep = 360f / Mathf.Max(1, bulletCount);
        float randomOffset = Random.Range(0f, 360f);

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = randomOffset + (angleStep * i);
            Vector2 dir = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)).normalized;
            Vector3 spawnPos = arenaCenter + spawnOffset;

            spawner.CreateHazardObject(hazardPrefab, spawnPos, dir, damage, projectileSpeed);
        }
    }
}