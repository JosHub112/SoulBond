using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    private Vector2 movement;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private const string _horizontal = "Horizontal";
    private const string _vertical = "Vertical";
    private const string _lasthorizontal = "Lasthorizontal";
    private const string _lastvertical = "Lastvertical";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Read input from your InputManager
        movement.x = Input.Movement.x;
        movement.y = Input.Movement.y;

        if (movement != Vector2.zero)
        {
            
            Vector3 moveDelta = (Vector3)movement * moveSpeed * Time.deltaTime;
            Vector2 origin = (Vector2)transform.position + boxCollider.offset;

            
            RaycastHit2D hit = Physics2D.BoxCast(origin, boxCollider.size, 0f, movement, moveDelta.magnitude, obstacleLayer);

            
            if (hit.collider == null)
            {
                transform.position += moveDelta;
            }
        }

        animator.SetFloat(_horizontal, movement.x);
        animator.SetFloat(_vertical, movement.y);

        if (movement != Vector2.zero)
        {
            animator.SetFloat(_lasthorizontal, movement.x);
            animator.SetFloat(_lastvertical, movement.y);
        }
    }
}