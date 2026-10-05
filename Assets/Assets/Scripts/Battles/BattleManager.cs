using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

[RequireComponent(typeof(HazardSpawner))]
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
    private List<GameObject> spawnedEnemyObjects = new List<GameObject>();

    [Header("Player Attack")]
    [SerializeField] private PlayerAttackSO currentAttack;
    [SerializeField] private Transform playerVfxAnchor;

    [Header("Enemy Chooser UI")]
    [SerializeField] private Button[] enemyChooserButtons = new Button[4];
    [SerializeField] private GameObject enemyChooserRoot;

    [Header("Soul")]
    [SerializeField] private SoulSO currentSoul;
    [SerializeField] private Transform soulSpawnAnchor;
    private GameObject spawnedSoulObject;

    [Header("Soul Options UI")]
    [SerializeField] private GameObject soulOptionsRoot;
    [SerializeField] private Button[] soulAttackButtons = new Button[4];

    private AttackDataSO pendingAttack;

    private List<int> selectedTargetIndices = new List<int>();
    private bool choosingTargets = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        hazardSpawner = GetComponent<HazardSpawner>();

        for (int i = 0; i < enemyChooserButtons.Length; i++)
        {
            if (enemyChooserButtons[i] == null) continue;

            int capturedIndex = i;
            enemyChooserButtons[i].onClick.AddListener(() => OnEnemyTargetSelected(capturedIndex));
        }

        for (int i = 0; i < soulAttackButtons.Length; i++)
        {
            if (soulAttackButtons[i] == null) continue;

            int capturedIndex = i;
            soulAttackButtons[i].onClick.AddListener(() => OnSoulAttackChosen(capturedIndex));
        }
    }

    private void Start()
    {
        if (battleUIPanel != null) battleUIPanel.SetActive(false);
        HideEnemyChooser();
        HideSoulOptions();
    }

    public void StartBattle(List<EnemySO> enemies)
    {
        currentEnemies = enemies;
        state = BattleState.START;
        AudioManager.Instance?.PlayBattleMusic();
        StartCoroutine(SetupBattleSequence());
    }

    private IEnumerator SetupBattleSequence()
    {
        ClearEnemies();

        for (int i = 0; i < currentEnemies.Count && i < enemySpawnPosition.Length; i++)
        {
            if (currentEnemies[i] == null || currentEnemies[i].enemyPrefab == null || enemySpawnPosition[i] == null) continue;

            GameObject newEnemy = Instantiate(currentEnemies[i].enemyPrefab, enemySpawnPosition[i].position, Quaternion.identity, enemySpawnPosition[i]);

            EnemyController ec = newEnemy.GetComponentInChildren<EnemyController>();
            if (ec != null) ec.Initialize(currentEnemies[i]);

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
        pendingAttack = null;

        HideEnemyChooser();
        ClearActiveHazards();
        HideSoulOptions();
        DespawnSoul();

        if (playerMovement != null)
        {
            playerMovement.canMoveInBattle = false;
            playerMovement.transform.position = initialBattlePosition;
        }

        if (battleUIPanel != null) battleUIPanel.SetActive(true);
    }

    public void OnAttackButtonClicked()
    {
        if (state != BattleState.PLAYERTURN) return;
        if (currentAttack == null) return;

        pendingAttack = currentAttack;
        BeginTargetSelection();
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

    public void OnSoulButtonClicked()
    {
        if (state != BattleState.PLAYERTURN) return;
        if (currentSoul == null || currentSoul.attacks == null || currentSoul.attacks.Count == 0) return;

        SummonSoul();

        if (battleUIPanel != null) battleUIPanel.SetActive(false);
        ShowSoulOptions();
    }

    private void SummonSoul()
    {
        if (spawnedSoulObject != null) return;
        if (currentSoul.SoulSprite == null || soulSpawnAnchor == null) return;
        AudioManager.Instance?.PlaySoulSpawnSFX();

        spawnedSoulObject = Instantiate(currentSoul.SoulSprite, soulSpawnAnchor.position, Quaternion.identity, soulSpawnAnchor);
       

        SpriteRenderer sr = spawnedSoulObject.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Color initialColor = sr.color;
            initialColor.a = 0f;
            sr.color = initialColor;

            sr.DOFade(1f, 0.5f);

            spawnedSoulObject.transform.position += Vector3.down * 0.5f;
            spawnedSoulObject.transform.DOMoveY(soulSpawnAnchor.position.y, 0.5f).SetEase(Ease.OutBack);
        }
    }

    private void DespawnSoul()
    {
        if (spawnedSoulObject != null)
        {
            Destroy(spawnedSoulObject);
            spawnedSoulObject = null;
        }
    }

    private void ShowSoulOptions()
    {
        if (soulOptionsRoot != null) soulOptionsRoot.SetActive(true);

        for (int i = 0; i < soulAttackButtons.Length; i++)
        {
            if (soulAttackButtons[i] == null) continue;

            bool attackAvailable = currentSoul != null && i < currentSoul.attacks.Count && currentSoul.attacks[i] != null;
            soulAttackButtons[i].gameObject.SetActive(attackAvailable);
            soulAttackButtons[i].interactable = attackAvailable;
        }
    }

    private void HideSoulOptions()
    {
        if (soulOptionsRoot != null) soulOptionsRoot.SetActive(false);

        for (int i = 0; i < soulAttackButtons.Length; i++)
        {
            if (soulAttackButtons[i] == null) continue;
            soulAttackButtons[i].gameObject.SetActive(false);
        }
    }

    private void OnSoulAttackChosen(int index)
    {
        if (state != BattleState.PLAYERTURN) return;
        if (currentSoul == null || index >= currentSoul.attacks.Count || currentSoul.attacks[index] == null) return;

        SoulAttackSO chosen = currentSoul.attacks[index];

        HideSoulOptions();

        pendingAttack = chosen;
        BeginTargetSelection();
    }

    private void BeginTargetSelection()
    {
        choosingTargets = true;
        selectedTargetIndices.Clear();

        if (battleUIPanel != null) battleUIPanel.SetActive(false);
        ShowEnemyChooser();
    }

    private void ShowEnemyChooser()
    {
        if (enemyChooserRoot != null) enemyChooserRoot.SetActive(true);

        for (int i = 0; i < enemyChooserButtons.Length; i++)
        {
            if (enemyChooserButtons[i] == null) continue;

            bool enemyAlive = i < spawnedEnemyObjects.Count && spawnedEnemyObjects[i] != null;
            enemyChooserButtons[i].gameObject.SetActive(enemyAlive);
            enemyChooserButtons[i].interactable = enemyAlive;
        }
    }

    private void HideEnemyChooser()
    {
        if (enemyChooserRoot != null) enemyChooserRoot.SetActive(false);

        for (int i = 0; i < enemyChooserButtons.Length; i++)
        {
            if (enemyChooserButtons[i] == null) continue;
            enemyChooserButtons[i].gameObject.SetActive(false);
        }
    }

    private void OnEnemyTargetSelected(int index)
    {
        if (!choosingTargets) return;
        if (pendingAttack == null) return;
        if (index >= spawnedEnemyObjects.Count || spawnedEnemyObjects[index] == null) return;
        if (selectedTargetIndices.Contains(index)) return;

        selectedTargetIndices.Add(index);

        int needed = Mathf.Max(1, pendingAttack.Enemycount);

        if (selectedTargetIndices.Count >= needed)
        {
            choosingTargets = false;
            HideEnemyChooser();
            StartCoroutine(ExecuteAttack(pendingAttack, new List<int>(selectedTargetIndices)));
        }
    }

    private IEnumerator ExecuteAttack(AttackDataSO attack, List<int> targetIndices)
    {
        state = BattleState.BUSY;
        AudioManager.Instance?.PlaySFX(attack.attackSFX);

        bool isSoulAttacking = (attack is SoulAttackSO && spawnedSoulObject != null);
        Transform attacker = isSoulAttacking ? spawnedSoulObject.transform : playerMovement.transform;
        Vector3 startPos = attacker.position;

        

        GameObject mainTarget = null;
        foreach (int idx in targetIndices)
        {
            if (idx < spawnedEnemyObjects.Count && spawnedEnemyObjects[idx] != null)
            {
                mainTarget = spawnedEnemyObjects[idx];
                break;
            }
        }

        if (mainTarget != null)
        {
            if (attack.ChargeVFX != null)
            {
                Instantiate(attack.ChargeVFX, attacker.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(0.3f);

            Vector3 attackPos = mainTarget.transform.position + Vector3.left * 1.5f;
            yield return attacker.DOMove(attackPos, 0.15f).SetEase(Ease.InExpo).WaitForCompletion();

            foreach (int idx in targetIndices)
            {
                if (idx >= spawnedEnemyObjects.Count || spawnedEnemyObjects[idx] == null) continue;

                GameObject enemyObj = spawnedEnemyObjects[idx];

                if (attack.AttackVFX != null)
                {
                    Instantiate(attack.AttackVFX, enemyObj.transform.position, Quaternion.identity);
                }

                EnemyController ec = enemyObj.GetComponentInChildren<EnemyController>();
                if (ec != null) ec.TakeDamage(attack.damage);

                enemyObj.transform.DOShakePosition(duration: 0.3f, strength: 0.5f, vibrato: 20);

                SpriteRenderer enemySprite = enemyObj.GetComponentInChildren<SpriteRenderer>();
                if (enemySprite != null)
                {
                    enemySprite.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);
                }
            }

            yield return new WaitForSeconds(0.2f);

            yield return attacker.DOJump(startPos, jumpPower: 1f, numJumps: 1, duration: 0.4f).WaitForCompletion();
        }

        if (isSoulAttacking)
        {
            SpriteRenderer sr = spawnedSoulObject.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                yield return sr.DOFade(0f, 0.2f).WaitForCompletion();
            }

            DespawnSoul();
        }

        pendingAttack = null;
        yield return new WaitForSeconds(0.3f);

        if (CheckVictoryCondition()) StartCoroutine(EndBattleSequence(true));
        else StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator ExecutePlayerBlock()
    {
        state = BattleState.BUSY;
        battleUIPanel.SetActive(false);

        playerMovement.transform.DOPunchScale(new Vector3(0.2f, -0.2f, 0f), duration: 0.5f, vibrato: 5);

        yield return new WaitForSeconds(0.6f);
        StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator ExecuteFleeAttempt()
    {
        state = BattleState.BUSY;
        battleUIPanel.SetActive(false);

        bool success = Random.value > 0.8f;

        if (success)
        {
            StartCoroutine(EndBattleSequence(false));
        }
        else
        {
            yield return playerMovement.transform.DOPunchPosition(Vector3.left * 2f, 0.6f, vibrato: 3).WaitForCompletion();
            StartCoroutine(StartEnemyTurn());
        }
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

            AudioManager.Instance?.PlaySFX(selectedAttack.attackSFX);

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

    private bool CheckVictoryCondition()
    {
        for (int i = 0; i < spawnedEnemyObjects.Count; i++)
        {
            if (spawnedEnemyObjects[i] != null) return false;
        }
        return spawnedEnemyObjects.Count > 0 || currentEnemies.Count > 0;
    }

    private IEnumerator EndBattleSequence(bool won)
    {
        AudioManager.Instance?.PlayOverworldMusic();

        if (battleUIPanel != null)
        {
            battleUIPanel.SetActive(false);
        }

        HideEnemyChooser();
        HideSoulOptions();
        DespawnSoul();

        if (won)
        {
            state = BattleState.WON;
        }
        else
        {
            state = BattleState.FLED;
        }

        yield return new WaitForSeconds(1f);

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

    public void NotifyEnemyDefeated(GameObject enemyObj)
    {
        int idx = spawnedEnemyObjects.IndexOf(enemyObj);
        if (idx >= 0) spawnedEnemyObjects[idx] = null;
    }
}