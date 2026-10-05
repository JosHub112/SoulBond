using UnityEngine;

public class BattleHazard : MonoBehaviour
{
    private int damage = 1;
    private Vector2 moveDirection;
    private float moveSpeed;
    private bool isInitialized = false;

    // Standard Setup without any AttackPatternType enum parameter
    public void Setup(int damageAmount, Vector2 direction, float speed)
    {
        damage = damageAmount;
        moveDirection = direction;
        moveSpeed = speed;
        isInitialized = true;

        AudioManager.Instance?.PlayHazardSpawnSFX();

        // Auto-destroy after 6 seconds to prevent memory leaks
        Destroy(gameObject, 6f);
    }

    private void Update()
    {
        if (!isInitialized) return;

        // Translate hazard according to set direction and speed
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}