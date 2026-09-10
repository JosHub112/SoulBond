using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Player Attack")]
   
    [SerializeField] private PlayerAttackSO currentAttack;

    [SerializeField] private Transform playerVfxAnchor;

    [Header("Enemy Chooser UI")]

    [SerializeField] private Button[] enemyChooserButtons = new Button[4];
    [SerializeField] private GameObject enemyChooserRoot; // optional parent object for the whole chooser group

    private List<int> selectedTargetIndices = new List<int>();
    private bool choosingTargets = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Fetch the spawner component
        hazardSpawner = GetComponent<HazardSpawner>();

        // Wire up enemy chooser buttons once, capturing each index correctly.
        for (int i = 0; i < enemyChooserButtons.Length; i++)
        {
            if (enemyChooserButtons[i] == null) continue;
            int capturedIndex = i; // avoid closure bug - don't use loop var directly
            enemyChooserButtons[i].onClick.AddListener(() => OnEnemyTargetSelected(capturedIndex));
        }
    }

    private void Start()
    {
        if (battleUIPanel != null) battleUIPanel.SetActive(false);
        HideEnemyChooser();
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

            EnemyController ec = newEnemy.GetComponent<EnemyController>();
            if (ec != null) ec.Initialize(currentEnemies[i]); // pulls maxHealth (and enemyName, etc) from the EnemySO

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
        choosingTargets = false;
        selectedTargetIndices.Clear();
        HideEnemyChooser();

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

        if (currentAttack == null)
        {
            Debug.LogWarning("No PlayerAttackSO assigned to BattleManager (currentAttack).");
            return;
        }

        Debug.Log("Player Clicks Attack");
        BeginTargetSelection(currentAttack);
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

    // ---------- Target selection ----------

    private void BeginTargetSelection(PlayerAttackSO attack)
    {
        choosingTargets = true;
        selectedTargetIndices.Clear();

        // Hide the main action panel (Flee/Attack/Block/Soul) while picking targets
        if (battleUIPanel != null) battleUIPanel.SetActive(false);
        Debug.Log("Beginning TargetSelection...");
        ShowEnemyChooser();
    }

    private void ShowEnemyChooser()
    {
        if (enemyChooserRoot != null) enemyChooserRoot.SetActive(true);
        Debug.Log("Target Selection Begun");
        for (int i = 0; i < enemyChooserButtons.Length; i++)
        {
            if (enemyChooserButtons[i] == null) continue;
            Debug.Log("Choose your target");

            bool enemyAlive = i < spawnedEnemyObjects.Count && spawnedEnemyObjects[i] != null;
            enemyChooserButtons[i].gameObject.SetActive(enemyAlive);
            enemyChooserButtons[i].interactable = enemyAlive;
        }
    }

    private void HideEnemyChooser()
    {
        if (enemyChooserRoot != null) enemyChooserRoot.SetActive(false);
        Debug.Log("EnemyChooser Hidden");

        for (int i = 0; i < enemyChooserButtons.Length; i++)
        {
            if (enemyChooserButtons[i] == null) continue;
            enemyChooserButtons[i].gameObject.SetActive(false);
        }
    }

    private void OnEnemyTargetSelected(int index)
    {
        if (!choosingTargets) return;
        if (index >= spawnedEnemyObjects.Count || spawnedEnemyObjects[index] == null) return;
        if (selectedTargetIndices.Contains(index)) return; 

        selectedTargetIndices.Add(index);


        int needed = Mathf.Max(1, currentAttack.Enemycount);

        if (selectedTargetIndices.Count >= needed)
        {
            choosingTargets = false;
            HideEnemyChooser();
            StartCoroutine(ExecutePlayerAttack(currentAttack, new List<int>(selectedTargetIndices)));
        }
    }

    // ---------- Attack execution ----------

private IEnumerator ExecutePlayerAttack(PlayerAttackSO attack, List<int> targetIndices)
    {
        state = BattleState.BUSY;

        // --- Charge phase ---
        Vector3 chargeOrigin = playerVfxAnchor != null
            ? playerVfxAnchor.position
            : playerMovement.transform.position;

        if (attack.ChargeVFX != null)
        {
            Instantiate(attack.ChargeVFX, chargeOrigin, Quaternion.identity);
        }

        float chargeDuration = 0.5f;

        if (attack.ChargeAnim != null)
        {
            chargeDuration = attack.ChargeAnim.length;
            PlayClip(attack.ChargeAnim);
        }

        yield return new WaitForSeconds(chargeDuration);


        // --- Attack phase ---
        // Spawns the attack VFX on every enemy that was selected
        if (attack.AttackVFX != null)
        {
            foreach (int idx in targetIndices)
            {
                if (idx >= spawnedEnemyObjects.Count)
                    continue;

                GameObject enemyObj = spawnedEnemyObjects[idx];

                if (enemyObj == null)
                    continue;

                Instantiate(
                    attack.AttackVFX,
                    enemyObj.transform.position,
                    Quaternion.identity
                );
            }
        }

        float attackDuration = 0.5f;

        if (attack.AttackAnim != null)
        {
            attackDuration = attack.AttackAnim.length;
            PlayClip(attack.AttackAnim);
        }

        yield return new WaitForSeconds(attackDuration);


        // --- Apply damage to chosen targets ---
        foreach (int idx in targetIndices)
        {
            if (idx >= spawnedEnemyObjects.Count || spawnedEnemyObjects[idx] == null)
                continue;

            EnemyController ec = spawnedEnemyObjects[idx].GetComponent<EnemyController>();

            if (ec != null)
            {
                ec.TakeDamage(attack.damage);
            }
            else
            {
                Debug.LogWarning(
                    $"Enemy at index {idx} has no EnemyController - damage not applied."
                );
            }
        }

        yield return new WaitForSeconds(0.3f);

        if (CheckVictoryCondition())
            StartCoroutine(EndBattleSequence(true));
        else
            StartCoroutine(StartEnemyTurn());
    }



    /// Plays an AnimationClip on the player using the legacy Animation component.

    private void PlayClip(AnimationClip clip)
    {
        if (playerMovement == null || clip == null) return;

        Animation anim = playerMovement.GetComponent<Animation>();
        if (anim == null)
        {
            Debug.LogWarning("No legacy Animation component found on player - skipping clip playback.");
            return;
        }

        if (anim.GetClip(clip.name) == null)
        {
            anim.AddClip(clip, clip.name);
        }
        anim.Play(clip.name);
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
        // True once every spawned enemy has been destroyed/removed.
        for (int i = 0; i < spawnedEnemyObjects.Count; i++)
        {
            if (spawnedEnemyObjects[i] != null) return false;
        }
        return spawnedEnemyObjects.Count > 0 || currentEnemies.Count > 0;
    }

    private IEnumerator EndBattleSequence(bool won)
    {
        // Hide UI immediately when battle finishes
        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(false);
        }
        HideEnemyChooser();

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

    /// Called by an EnemyController when it dies, so the manager's list stays in sync
    /// even if the enemy destroys itself instead of BattleManager doing it directly.
    public void NotifyEnemyDefeated(GameObject enemyObj)
    {
        int idx = spawnedEnemyObjects.IndexOf(enemyObj);
        if (idx >= 0) spawnedEnemyObjects[idx] = null; // keep index alignment with enemySpawnPosition
    }
}