using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Owns everything the player sees and clicks during a battle: the action menu,
/// the enemy target chooser and the soul attack options.
/// It knows nothing about battle rules. It reports clicks through events and
/// is told what to show by the BattleManager.
/// </summary>
public class BattleUI : MonoBehaviour
{
    [Header("Action Menu")]
    [SerializeField] private GameObject actionMenuPanel;

    [Header("Enemy Chooser")]
    [SerializeField] private GameObject enemyChooserRoot;
    [SerializeField] private Button[] enemyChooserButtons = new Button[4];

    [Header("Soul Options")]
    [SerializeField] private GameObject soulOptionsRoot;
    [SerializeField] private Button[] soulAttackButtons = new Button[4];

    /// <summary>Raised with the slot index of the enemy button that was clicked.</summary>
    public event Action<int> EnemyTargetSelected;

    /// <summary>Raised with the slot index of the soul attack button that was clicked.</summary>
    public event Action<int> SoulAttackSelected;

    private void Awake()
    {
        BindButtons(enemyChooserButtons, index => EnemyTargetSelected?.Invoke(index));
        BindButtons(soulAttackButtons, index => SoulAttackSelected?.Invoke(index));
    }

    // ---------------------------------------------------------------------
    // Action menu
    // ---------------------------------------------------------------------

    public void ShowActionMenu() => SetActive(actionMenuPanel, true);

    public void HideActionMenu() => SetActive(actionMenuPanel, false);

    // ---------------------------------------------------------------------
    // Enemy chooser
    // ---------------------------------------------------------------------

    /// <param name="isEnemyAlive">Returns true if the enemy in the given slot can be targeted.</param>
    public void ShowEnemyChooser(Func<int, bool> isEnemyAlive)
    {
        SetActive(enemyChooserRoot, true);
        RefreshButtons(enemyChooserButtons, isEnemyAlive);
    }

    public void HideEnemyChooser()
    {
        SetActive(enemyChooserRoot, false);
        HideButtons(enemyChooserButtons);
    }

    // ---------------------------------------------------------------------
    // Soul options
    // ---------------------------------------------------------------------

    /// <param name="isAttackAvailable">Returns true if the soul attack in the given slot can be used.</param>
    public void ShowSoulOptions(Func<int, bool> isAttackAvailable)
    {
        SetActive(soulOptionsRoot, true);
        RefreshButtons(soulAttackButtons, isAttackAvailable);
    }

    public void HideSoulOptions()
    {
        SetActive(soulOptionsRoot, false);
        HideButtons(soulAttackButtons);
    }

    // ---------------------------------------------------------------------
    // Convenience
    // ---------------------------------------------------------------------

    public void HideAll()
    {
        HideActionMenu();
        HideEnemyChooser();
        HideSoulOptions();
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------

    private static void BindButtons(Button[] buttons, Action<int> onClicked)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            int capturedIndex = i;
            buttons[i].onClick.AddListener(() => onClicked(capturedIndex));
        }
    }

    private static void RefreshButtons(Button[] buttons, Func<int, bool> isAvailable)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            bool available = isAvailable(i);
            buttons[i].gameObject.SetActive(available);
            buttons[i].interactable = available;
        }
    }

    private static void HideButtons(Button[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            buttons[i].gameObject.SetActive(false);
        }
    }

    private static void SetActive(GameObject target, bool active)
    {
        if (target != null) target.SetActive(active);
    }
}