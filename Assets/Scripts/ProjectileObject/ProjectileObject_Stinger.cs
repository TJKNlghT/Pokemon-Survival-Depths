using UnityEngine;

public class ProjectileObject_Stinger : ProjectileObject_Base
{
    [SerializeField] private float lifeTime = 5f;

    private void OnEnable()
    {
        if (lifeTime > 0) Destroy(gameObject, lifeTime);
    }

    // override if we need custom velocity behavior
    public override void SetupProjectile(Entity_Stats shooterStats, Vector2 velocity, DamageScaleData scale)
        => base.SetupProjectile(shooterStats, velocity, scale);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsInMask(other.gameObject, obstacleMask))
        {
            Destroy(gameObject);
            return;
        }

        if (IsInMask(other.gameObject, whatIsEnemy))
        {
            // AoE damage + status + VFX from base helper
            DamageEnemiesInRadius(transform, checkRadius);
            Destroy(gameObject);
        }
    }
}