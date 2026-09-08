using UnityEngine;

public class BattleHazard : MonoBehaviour
{
    private int damage;
    private Vector3 moveDirection; 
    private float moveSpeed;
    private AttackPatternType attackType;
    private Transform myTransform; 

    public void Setup(int damage, Vector2 direction, float speed, AttackPatternType type)
    {
        this.damage = damage;
        this.moveDirection = direction;
        this.moveSpeed = speed;
        this.attackType = type;
        this.myTransform = transform; 

        // Autodestroy to prevent memory leaks
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
       
        switch (attackType)
        {
            case AttackPatternType.BulletStream:
            case AttackPatternType.RadialBurst:
                // Both are projectiles that need to move
                myTransform.Translate(moveDirection * (moveSpeed * Time.deltaTime), Space.World);
                break;

            case AttackPatternType.AoECircle:
                // Grow scale continuously over time
                myTransform.localScale += Vector3.one * (moveSpeed * Time.deltaTime);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
          
            if (other.TryGetComponent<PlayerHealth>(out var health))
            {
                health.TakeDamage(damage);
            }

           
            if (attackType == AttackPatternType.BulletStream || attackType == AttackPatternType.RadialBurst)
            {
                Destroy(gameObject);
            }
        }
    }
}