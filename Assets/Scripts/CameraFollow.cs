using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10f);

    private Transform target;

    private void Start()
    {
        // Try to grab a player at scene start
        TryFindPlayer();

        // Snap camera immediately if found
        if (target != null)
            transform.position = target.position + offset;
    }

    private void LateUpdate()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            // In case player was spawned later or respawned
            TryFindPlayer();
            if (target == null) return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    private void TryFindPlayer()
    {
        var player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            target = player.transform;
            // optional: snap immediately when found
            transform.position = target.position + offset;
        }
    }

    // Optional: lets other scripts explicitly set the target
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
            transform.position = target.position + offset;
    }
}