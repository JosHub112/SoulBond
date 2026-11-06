using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }

    [Range(0f, 100f)]
    public float encounterChancePercent = 10f;

    public Transform battlePlayerSpawnPoint;

    public Vector3 savedOverworldPosition;
    public bool inBattle = false;

    [SerializeField] private List<EnemySO> possibleEnemies = new List<EnemySO>();

    [Header("Transition")]
    [SerializeField] private BattleUIEffects uiEffects; // owns the black transition panel

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
        if (inBattle) return;
        StartCoroutine(TriggerBattleRoutine(playerTransform));
    }

    private IEnumerator TriggerBattleRoutine(Transform playerTransform)
    {
        inBattle = true;
        savedOverworldPosition = playerTransform.position;

        yield return ScreenToBlack(flash: true); // blink, then black

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

        yield return ScreenFromBlack(); // back to game
    }

    public void EndBattle(Transform playerTransform)
    {
        StartCoroutine(EndBattleRoutine(playerTransform));
    }

    private IEnumerator EndBattleRoutine(Transform playerTransform)
    {
        yield return ScreenToBlack(flash: false); // choppy fade to black

        playerTransform.position = savedOverworldPosition;

        yield return ScreenFromBlack(); // back to game

        inBattle = false;
        Debug.Log("BattleEnded");
    }

    private IEnumerator ScreenToBlack(bool flash)
    {
        if (uiEffects != null) yield return uiEffects.FadeToBlack(flash);
    }

    private IEnumerator ScreenFromBlack()
    {
        if (uiEffects != null) yield return uiEffects.FadeFromBlack();
    }
}