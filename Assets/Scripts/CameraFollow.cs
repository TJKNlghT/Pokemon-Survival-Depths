using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10f);

    private Transform target;

    private void Start()
    {
        TryFindPlayer();

        // Snap once at start
        if (target != null)
            transform.position = target.position + offset;
    }

    private void LateUpdate()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            TryFindPlayer();
            if (target == null) return;
        }

        Vector3 desiredPosition = target.position + offset;
        // Smooth pan; smoothSpeed ~ 0.1�0.2 works nicely
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    private void TryFindPlayer()
    {
        var player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            SetTarget(player.transform, true);  // snap on initial find
        }
    }

    /// Set a new camera target.
    /// snapImmediately = true  -> teleports camera to target
    /// snapImmediately = false -> camera smoothly pans to target
    public void SetTarget(Transform newTarget, bool snapImmediately = true)
    {
        target = newTarget;
        if (snapImmediately && target != null)
        {
            transform.position = target.position + offset;
        }
    }
}