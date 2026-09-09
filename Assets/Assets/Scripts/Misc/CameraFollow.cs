using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping = 0.15f; // Set between 0.1 and 0.2 in Inspector

    public Transform target;
    private Vector3 vel = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        // Smooth from transform.position (camera) instead of target.position (player)
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vel, damping);
    }
}