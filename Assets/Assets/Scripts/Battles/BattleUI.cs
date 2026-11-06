using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Owns everything the player sees and clicks during a battle: the action menu,
/// the enemy target chooser and the soul attack options.
/// It knows nothing about battle rules. It reports clicks through events and
/// is told what to show by the BattleManager.
/// Visual flair is delegated to BattleUIEffects.
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

    [Header("Effects")]
    [SerializeField] private BattleUIEffects effects;

    /// <summary>Raised with the slot index of the enemy button that was clicked.</summary>
    public event Action<int> EnemyTargetSelected;

    /// <summary>Raised with the slot index of the soul attack button that was clicked.</summary>
    public event Action<int> SoulAttackSelected;

    private RectTransform actionMenuRect;
    private RectTransform enemyChooserRect;
    private RectTransform soulOptionsRect;

    // Every button inside the action menu, found automatically
    private readonly List<Button> actionMenuButtons = new List<Button>();

    // Only the buttons currently on screen, in order
    private readonly List<Button> shownEnemyButtons = new List<Button>();
    private readonly List<Button> shownSoulButtons = new List<Button>();

    private void Awake()
    {
        actionMenuRect = AsRect(actionMenuPanel);
        enemyChooserRect = AsRect(enemyChooserRoot);
        soulOptionsRect = AsRect(soulOptionsRoot);

        if (actionMenuPanel != null)
        {
            actionMenuPanel.GetComponentsInChildren(true, actionMenuButtons);
        }

        BindButtons(enemyChooserButtons, index => EnemyTargetSelected?.Invoke(index));
        BindButtons(soulAttackButtons, index => SoulAttackSelected?.Invoke(index));
    }

    // ---------------------------------------------------------------------
    // Action menu
    // ---------------------------------------------------------------------

    public void ShowActionMenu()
    {
        SetActive(actionMenuPanel, true);
        if (effects != null) effects.PlayShow(actionMenuRect, actionMenuButtons);
    }

    public void HideActionMenu()
    {
        if (effects != null) effects.Stop(actionMenuRect, actionMenuButtons);
        SetActive(actionMenuPanel, false);
    }

    // ---------------------------------------------------------------------
    // Enemy chooser
    // ---------------------------------------------------------------------

    /// <param name="isEnemyAlive">Returns true if the enemy in the given slot can be targeted.</param>
    public void ShowEnemyChooser(Func<int, bool> isEnemyAlive)
    {
        SetActive(enemyChooserRoot, true);
        RefreshButtons(enemyChooserButtons, isEnemyAlive, shownEnemyButtons);
        if (effects != null) effects.PlayShow(enemyChooserRect, shownEnemyButtons);
    }

    public void HideEnemyChooser()
    {
        if (effects != null) effects.Stop(enemyChooserRect, enemyChooserButtons);
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
        RefreshButtons(soulAttackButtons, isAttackAvailable, shownSoulButtons);
        if (effects != null) effects.PlayShow(soulOptionsRect, shownSoulButtons);
    }

    public void HideSoulOptions()
    {
        if (effects != null) effects.Stop(soulOptionsRect, soulAttackButtons);
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

    private static void RefreshButtons(Button[] buttons, Func<int, bool> isAvailable, List<Button> shown)
    {
        shown.Clear();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            bool available = isAvailable(i);
            buttons[i].gameObject.SetActive(available);
            buttons[i].interactable = available;

            if (available) shown.Add(buttons[i]);
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

    private static RectTransform AsRect(GameObject target)
    {
        return target != null ? target.transform as RectTransform : null;
    }
}