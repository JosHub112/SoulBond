using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Drives one battle from start to finish: player picks an action,
/// it resolves, then enemies take their turns, repeat until someone wins,
/// loses, or the player flees.

public enum BattleState
{
    START,
    PLAYERTURN,
    BUSY,
    ENEMYTURN,
    WON,
    LOST,
    FLED,
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set;}

    public BattleState state;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject battleUIPanel;

    [SerializeField] private Transform playerBattlePosition;
    [SerializeField] private Transform[] enemySpawnPosition = new Transform[4];

    [SerializeField] private List<EnemySO> currentEnemies = new List<EnemySO>();
    private Vector3 initialBattlePosition;

    [SerializeField] private GameObject enemyPrefab; // Drag your EnemyPrefab here
    private List<GameObject> spawnedEnemyObjects = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Ensure the battle UI is hidden when starting in the dungeon
        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(false);
        }
    }

    //Battle sequence
    public void StartBattle(List<EnemySO> enemies)
    {
        currentEnemies = enemies;
        state = BattleState.START;
        StartCoroutine(SetupBattleSequence());
    }

    private IEnumerator SetupBattleSequence()
    {
        // Clear old spawned enemies from previous battles
        foreach (GameObject enemyObj in spawnedEnemyObjects)
        {
            if (enemyObj != null) Destroy(enemyObj);
        }
        spawnedEnemyObjects.Clear();

        // Spawn new enemies visually at spawn positions
        for (int i = 0; i < currentEnemies.Count && i < enemySpawnPosition.Length; i++)
        {
            if (currentEnemies[i] == null || enemySpawnPosition[i] == null) continue;

            // Instantiate prefab at slot position
            GameObject newEnemy = Instantiate(enemyPrefab, enemySpawnPosition[i].position, Quaternion.identity, enemySpawnPosition[i]);

            // Apply the sprite from EnemySO
            SpriteRenderer sr = newEnemy.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = currentEnemies[i].enemySprite;
            }

            spawnedEnemyObjects.Add(newEnemy);
        }

        if (playerMovement != null)
        {
            initialBattlePosition = playerMovement.transform.position;
            playerMovement.canMoveInBattle = false;
        }

        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        state = BattleState.PLAYERTURN;

        if (playerMovement != null)
        {
            playerMovement.canMoveInBattle = false;
            // Snap player back to where they started the battle
            playerMovement.transform.position = initialBattlePosition;
        }

        // Show UI options (Attack, Block, Flee)
        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(true);
        }

        Debug.Log("Player Turn: Select an action (Attack, Block, Flee).");
    }

    public void OnAttackButtonClicked()
    {
        if (state != BattleState.PLAYERTURN) return;
        StartCoroutine(ExecutePlayerAttack());
    }

    public void OnBlockButtonClicked()
    {
        if (state != BattleState.PLAYERTURN) return;
        StartCoroutine(ExecutePlayerBlock());
    }

    public void OnFleeButtonClicked()
    {
        if (state != BattleState.PLAYERTURN) return;
        StartCoroutine(ExecuteFleeAttempt());
    }

    private IEnumerator ExecutePlayerAttack()
    {
        state = BattleState.BUSY;
        battleUIPanel.SetActive(false);

        Debug.Log("Player performs physical attack!");
        // TODO: Play attack animation and calculate damage to target enemy

        yield return new WaitForSeconds(1.5f);

        // Check if all enemies are defeated
        if (CheckVictoryCondition())
        {
            StartCoroutine(EndBattleSequence(true));
        }
        else
        {
            StartCoroutine(StartEnemyTurn());
        }
    }

    private IEnumerator ExecutePlayerBlock()
    {
        state = BattleState.BUSY;
        battleUIPanel.SetActive(false);

        Debug.Log("Player prepares to Block/Reflect!");
        // TODO: Activate deflection window logic

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator ExecuteFleeAttempt()
    {
        state = BattleState.BUSY;
        battleUIPanel.SetActive(false);

        Debug.Log("Attempting to flee...");
        yield return new WaitForSeconds(1f);

        bool success = Random.value > 0.3f; // 70% success chance

        if (success)
        {
            Debug.Log("Fled successfully!");
            StartCoroutine(EndBattleSequence(false));
        }
        else
        {
            Debug.Log("Failed to flee!");
            StartCoroutine(StartEnemyTurn());
        }
    }

    private IEnumerator StartEnemyTurn()
    {
        state = BattleState.ENEMYTURN;

        if (battleUIPanel != null) battleUIPanel.SetActive(false);

        // Unlock player movement for dodging phase
        if (playerMovement != null) playerMovement.canMoveInBattle = true;

        // Pick a active enemy and a random attack from its list
        if (currentEnemies.Count > 0 && currentEnemies[0] != null && currentEnemies[0].attacks.Count > 0)
        {
            EnemySO activeEnemy = currentEnemies[0];
            AttackSO selectedAttack = activeEnemy.attacks[Random.Range(0, activeEnemy.attacks.Count)];

            Debug.Log($"Enemy uses: {selectedAttack.attackName}");

            float timer = 0f;
            float nextSpawnTime = 0f;

            while (timer < selectedAttack.attackDuration)
            {
                timer += Time.deltaTime;

                if (timer >= nextSpawnTime && selectedAttack.hazardPrefab != null)
                {
                    nextSpawnTime = timer + selectedAttack.spawnInterval;
                    SpawnHazard(selectedAttack);
                }

                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(3f); // Fallback timer
        }

        // Lock player movement and snap back to start
        if (playerMovement != null)
        {
            playerMovement.canMoveInBattle = false;
            playerMovement.transform.position = initialBattlePosition;
        }

        StartPlayerTurn();
    }

    private void SpawnHazard(AttackSO attack)
    {
        Vector3 spawnPos = initialBattlePosition;
        Vector2 dir = Vector2.down;

        switch (attack.patternType)
        {
            case AttackPatternType.BulletStream:
                // Spawn random drops from the top of the arena
                spawnPos = initialBattlePosition + new Vector3(Random.Range(-4f, 4f), 5f, 0f);
                dir = Vector2.down;
                break;

            case AttackPatternType.RadialBurst:
                // Spawn in the center and shoot outwards toward a random angle
                spawnPos = initialBattlePosition + new Vector3(0, 3f, 0);
                float randomAngle = Random.Range(0f, 360f);
                dir = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized;
                break;

            case AttackPatternType.AoECircle:
                // Spawn exactly where the player is currently standing, then grow
                spawnPos = playerMovement.transform.position;
                dir = Vector2.zero;
                break;
        }

        GameObject hazardObj = Instantiate(attack.hazardPrefab, spawnPos, Quaternion.identity);

        // Efficiently get or add the component
        if (!hazardObj.TryGetComponent<BattleHazard>(out var hazardScript))
        {
            hazardScript = hazardObj.AddComponent<BattleHazard>();
        }

        hazardScript.Setup(attack.damage, dir, attack.projectileSpeed, attack.patternType);
    }

    private bool CheckVictoryCondition()
    {
        // Simple placeholder victory check
        return false;
    }

    private IEnumerator EndBattleSequence(bool won)
    {

        // Hide UI immediately when battle finishes
        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(false);
        }

        if (won)
        {
            state = BattleState.WON;
            Debug.Log("Victory!");
        }
        else
        {
            state = BattleState.FLED;
        }

        yield return new WaitForSeconds(1f);

        // Clean up enemy GameObjects before returning to overworld
        ClearEnemies();

        if (EncounterManager.Instance != null && playerMovement != null)
        {
            EncounterManager.Instance.EndBattle(playerMovement.transform);
        }
    }

    public void ClearEnemies()
    {
        foreach (GameObject enemyObj in spawnedEnemyObjects)
        {
            if (enemyObj != null)
            {
                Destroy(enemyObj);
            }
        }
        spawnedEnemyObjects.Clear();
        currentEnemies.Clear();
    }


}