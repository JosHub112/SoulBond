using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Rolls a random encounter chance whenever the player steps onto a tile
/// that exists on the assigned "ground" Tilemap.
/// </summary>
public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }

    [Range(0f, 100f)]
    public float encounterChancePercent = 10f;

    public Transform battlePlayerSpawnPoint;

    public Vector3 savedOverworldPosition;
    public bool inBattle = false;

    [SerializeField] private List<EnemySO> possibleEnemies = new List<EnemySO>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CheckForEncounters(Transform playerTransform)
    {
        if (inBattle) return;

        if (Random.Range(0f, 100f) <= encounterChancePercent)
        {
            TriggerBattle(playerTransform);
        }
    }

    public void TriggerBattle(Transform playerTransform)
    {
        inBattle = true;
        savedOverworldPosition = playerTransform.position;

        playerTransform.position = battlePlayerSpawnPoint.position;

        if (BattleManager.Instance != null && possibleEnemies.Count > 0)
        {
            List<EnemySO> encounterList = new List<EnemySO>();

            // Roll a random enemy count from 1 to 4
            int enemyCountToSpawn = Random.Range(1, 5); // Upper bound is exclusive, so max value is 4

            for (int i = 0; i < enemyCountToSpawn; i++)
            {
                int randomIndex = Random.Range(0, possibleEnemies.Count);
                encounterList.Add(possibleEnemies[randomIndex]);
            }

            BattleManager.Instance.StartBattle(encounterList);
        }
    }

    public void EndBattle(Transform playerTransform)
    {
        playerTransform.position = savedOverworldPosition;
        inBattle = false;

        Debug.Log("BattleEnded");
    }
}