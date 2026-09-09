using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    public void Spawn(AttackSO attackPattern, Vector3 arenaCenter, Transform playerTransform)
    {
        if (attackPattern != null)
        {
            attackPattern.Execute(arenaCenter, playerTransform, this);
        }
    }

    // Must be PUBLIC so AttackPatternSO scripts can call it
    public void CreateHazardObject(GameObject prefab, Vector3 position, Vector2 direction, int damage, float speed)
    {
        if (prefab == null) return;

        GameObject hazardObj = Instantiate(prefab, position, Quaternion.identity);

        if (hazardObj.TryGetComponent<BattleHazard>(out var hazardScript))
        {
            hazardScript.Setup(damage, direction, speed);
        }
    }
}