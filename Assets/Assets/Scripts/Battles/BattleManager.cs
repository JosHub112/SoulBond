using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

[RequireComponent(typeof(HazardSpawner))] //adds spawner script automatically
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public BattleState state;

    [Header("Dependencies")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject battleUIPanel;
    private HazardSpawner hazardSpawner; 

    [Header("Positions & Spawning")]
    [SerializeField] private Transform playerBattlePosition;
    [SerializeField] private Transform[] enemySpawnPosition = new Transform[4];
    private Vector3 initialBattlePosition;

    [Header("Enemy Data")]
    [SerializeField] private List<EnemySO> currentEnemies = new List<EnemySO>();
    [SerializeField] private GameObject enemyPrefab;
    private List<GameObject> spawnedEnemyObjects = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Fetch the spawner component
        hazardSpawner = GetComponent<HazardSpawner>();
    }

    private void Start()
    {
        if (battleUIPanel != null) battleUIPanel.SetActive(false);
    }

    public void StartBattle(List<EnemySO> enemies)
    {
        currentEnemies = enemies;
        state = BattleState.START;
        StartCoroutine(SetupBattleSequence());
    }

    private IEnumerator SetupBattleSequence()
    {
        ClearEnemies();

        for (int i = 0; i < currentEnemies.Count && i < enemySpawnPosition.Length; i++)
        {
            if (currentEnemies[i] == null || enemySpawnPosition[i] == null) continue;

            GameObject newEnemy = Instantiate(enemyPrefab, enemySpawnPosition[i].position, Quaternion.identity, enemySpawnPosition[i]);

            SpriteRenderer sr = newEnemy.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = currentEnemies[i].enemySprite;

            spawnedEnemyObjects.Add(newEnemy);
        }

        if (playerMovement != null)
        {
            initialBattlePosition = playerMovement.transform.position;
            playerMovement.canMoveInBattle = false;
        }

        if (battleUIPanel != null) battleUIPanel.SetActive(false);

        yield return new WaitForSeconds(1f);
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        state = BattleState.PLAYERTURN;

        if (playerMovement != null)
        {
            playerMovement.canMoveInBattle = false;
            playerMovement.transform.position = initialBattlePosition;
        }

        if (battleUIPanel != null) battleUIPanel.SetActive(true);

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

    private IEnumerator StartEnemyTurn()
    {
        state = BattleState.ENEMYTURN;

        if (battleUIPanel != null) battleUIPanel.SetActive(false);
        if (playerMovement != null) playerMovement.canMoveInBattle = true;

        if (currentEnemies.Count > 0 && currentEnemies[0] != null && currentEnemies[0].attacks.Count > 0)
        {
            EnemySO enemy = currentEnemies[0]; 
            AttackSO selectedAttack = enemy.attacks[Random.Range(0, enemy.attacks.Count)];

            Debug.Log($"Enemy uses: {selectedAttack.name}");

            float timer = 0f;
            float nextSpawnTime = 0f;

            while (timer < 5f)
            {
                timer += Time.deltaTime;

                if (timer >= nextSpawnTime)
                {
                    nextSpawnTime = timer + 0.5f;
                    hazardSpawner.Spawn(selectedAttack, initialBattlePosition, playerMovement.transform);
                }

                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        if (playerMovement != null)
        {
            playerMovement.canMoveInBattle = false;
            playerMovement.transform.position = initialBattlePosition;
        }

        StartPlayerTurn();
    }

    private IEnumerator ExecutePlayerAttack()
    {
        state = BattleState.BUSY;
        battleUIPanel.SetActive(false);

        Debug.Log("Player performs physical attack!");
        yield return new WaitForSeconds(1.5f);

        if (CheckVictoryCondition()) StartCoroutine(EndBattleSequence(true));
        else StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator ExecutePlayerBlock()
    {
        state = BattleState.BUSY;
        battleUIPanel.SetActive(false);

        Debug.Log("Player prepares to Block/Reflect!");
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator ExecuteFleeAttempt()
    {
        state = BattleState.BUSY;
        battleUIPanel.SetActive(false);

        Debug.Log("Attempting to flee...");
        yield return new WaitForSeconds(1f);

        bool success = Random.value > 0.3f;

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



    private bool CheckVictoryCondition()
    {
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

    private void ClearActiveHazards()
    {
        BattleHazard[] hazards = FindObjectsOfType<BattleHazard>();
        foreach (BattleHazard h in hazards)
        {
            Destroy(h.gameObject);
        }
    }

    public void ClearEnemies()
    {
        foreach (GameObject enemyObj in spawnedEnemyObjects)
        {
            if (enemyObj != null) Destroy(enemyObj);
        }
        spawnedEnemyObjects.Clear();
    }
}