using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private EnemySO enemyData;
    private int maxHealth;
    private int currentHealth;

    /// Called by BattleManager right after Instantiate, so this enemy's health
    /// (and anything else from the SO) comes from the EnemySO rather than a
    /// hardcoded value on the prefab.

    public void Initialize(EnemySO data)
    {
        enemyData = data;
        maxHealth = data != null ? data.maxHealth : 1;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage ({currentHealth}/{maxHealth} left).");

        // TODO: play hit VFX/anim here

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{name} defeated!");

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.NotifyEnemyDefeated(gameObject);
        }

        // TODO: play death VFX/anim here before destroying, if desired
        Destroy(gameObject);
    }
}