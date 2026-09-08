using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;

    public Transform target;
    private Vector3 vel = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(target.position, targetPosition, ref 
        vel, damping);

    }
}
