using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -5f);
    [SerializeField] private float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Fixed world-space offset (not affected by player rotation)
        Vector3 desiredPosition = target.position + offset;

        // Smooth follow movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Optional: make camera look at player's upper body or head
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
