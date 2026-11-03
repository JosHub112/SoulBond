using System.Collections;
using System.Collections.Generic;
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

    [Header("Transition")]
    [SerializeField] private CanvasGroup transitionPanel; // black panel with a CanvasGroup
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Start fully transparent and let clicks pass through
            if (transitionPanel != null)
            {
                transitionPanel.alpha = 0f;
                transitionPanel.blocksRaycasts = false;
            }
        }
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

        yield return Fade(0f, 1f); // to black

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

        yield return Fade(1f, 0f); // back to game
    }

    public void EndBattle(Transform playerTransform)
    {
        StartCoroutine(EndBattleRoutine(playerTransform));
    }

    private IEnumerator EndBattleRoutine(Transform playerTransform)
    {
        yield return Fade(0f, 1f); // to black

        playerTransform.position = savedOverworldPosition;

        yield return Fade(1f, 0f); // back to game

        inBattle = false;
        Debug.Log("BattleEnded");
    }

    private IEnumerator Fade(float from, float to)
    {
        if (transitionPanel == null) yield break;

        // Block clicks on UI underneath while fading
        transitionPanel.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            transitionPanel.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        transitionPanel.alpha = to;
        transitionPanel.blocksRaycasts = to >= 1f;
    }
}