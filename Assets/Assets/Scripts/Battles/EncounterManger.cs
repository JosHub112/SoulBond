using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for working with UI Image components

/// <summary>
/// Rolls a random encounter chance whenever the player steps onto a tile
/// that exists on the assigned "ground" Tilemap, featuring smooth screen fades using a UI Image panel.
/// </summary>
public class EncounterManager : MonoBehaviour
{
    public static EncounterManager Instance { get; private set; }

    [Header("Encounter Settings")]
    [Range(0f, 100f)]
    public float encounterChancePercent = 10f;
    public Transform battlePlayerSpawnPoint;

    [Header("Transition UI")]
    [Tooltip("Drag your 'Transition' Panel GameObject here (needs an Image component).")]
    [SerializeField] private Image transitionPanel;
    [SerializeField] private float fadeDuration = 0.5f;

    public Vector3 savedOverworldPosition;
    public bool inBattle = false;

    [SerializeField] private List<EnemySO> possibleEnemies = new List<EnemySO>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Ensure the transition starts transparent and raycast target is disabled so it doesn't block clicks
        if (transitionPanel != null)
        {
            Color c = transitionPanel.color;
            c.a = 0f;
            transitionPanel.color = c;
            transitionPanel.raycastTarget = false;
        }
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

        // 1. Fade Out to Black
        yield return StartCoroutine(Fade(0f, 1f));

        // 2. Teleport player and start battle while screen is completely black
        savedOverworldPosition = playerTransform.position;
        playerTransform.position = battlePlayerSpawnPoint.position;

        if (BattleManager.Instance != null && possibleEnemies.Count > 0)
        {
            List<EnemySO> encounterList = new List<EnemySO>();
            int enemyCountToSpawn = Random.Range(1, 5); // 1 to 4 enemies

            for (int i = 0; i < enemyCountToSpawn; i++)
            {
                int randomIndex = Random.Range(0, possibleEnemies.Count);
                encounterList.Add(possibleEnemies[randomIndex]);
            }

            BattleManager.Instance.StartBattle(encounterList);
        }

        // 3. Fade In from Black
        yield return StartCoroutine(Fade(1f, 0f));
    }

    public void EndBattle(Transform playerTransform)
    {
        StartCoroutine(EndBattleRoutine(playerTransform));
    }

    private IEnumerator EndBattleRoutine(Transform playerTransform)
    {
        // 1. Fade Out to Black
        yield return StartCoroutine(Fade(0f, 1f));

        // 2. Move player back to overworld position while screen is black
        playerTransform.position = savedOverworldPosition;
        inBattle = false;

        // 3. Fade In from Black
        yield return StartCoroutine(Fade(1f, 0f));

        Debug.Log("BattleEnded");
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (transitionPanel == null) yield break;

        transitionPanel.raycastTarget = true; // Block input clicks during the fade
        float elapsedTime = 0f;

        Color panelColor = transitionPanel.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panelColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            transitionPanel.color = panelColor;
            yield return null;
        }

        panelColor.a = endAlpha;
        transitionPanel.color = panelColor;

        // If fully faded back in, allow clicks to pass through the panel again
        if (endAlpha == 0f)
        {
            transitionPanel.raycastTarget = false;
        }
    }
}