using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ProjectileObject_Base : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] protected LayerMask whatIsEnemy;   // damage these
    [SerializeField] protected LayerMask obstacleMask;  // break on these (no damage)

    [Header("Radius Check")]
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 1f;

    protected Rigidbody2D rb;
    protected Entity_Stats stats;
    protected DamageScaleData damageScaleData;
    protected Entity_VFX vfx;
    protected Animator anim;

    protected Vector2 lastAnimDir = Vector2.down;

    protected virtual void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!targetCheck) targetCheck = transform;
        if (!anim) anim = GetComponentInChildren<Animator>();
    }
    public void SetMasksAndVFX(LayerMask targetMask, LayerMask obstacle, Entity_VFX vfxRef)
    {
        whatIsEnemy = targetMask;
        obstacleMask = obstacle;
        vfx = vfxRef;
    }

    public virtual void SetupProjectile(Entity_Stats shooterStats, Vector2 velocity, DamageScaleData scale)
    {
        stats = shooterStats;
        damageScaleData = scale;
        if (!rb) rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = velocity;


        anim.SetFloat("xVelocity", velocity.x);
        anim.SetFloat("yVelocity", velocity.y);
    }

    protected static bool IsInMask(GameObject go, LayerMask mask)
        => ((1 << go.layer) & mask.value) != 0;

    // AoE helper (already made earlier): does Damage + Status + VFX
    protected void DamageEnemiesInRadius(Transform center, float radius)
    {
        var hits = Physics2D.OverlapCircleAll(center.position, radius, whatIsEnemy);
        foreach (var col in hits)
        {
            var damagable = col.GetComponent<IDamagable>();
            var statusHandler = col.GetComponent<Entity_StatusHandler>();
            if (damagable == null) continue;

            var atk = stats.GetAttackData(damageScaleData);

            damagable.TakeDamage(atk.physicalDamage, transform);

            var element = (stats != null) ? stats.Element : ElementType.Normal;
            if (element != ElementType.Normal)
                statusHandler?.ApplyStatusEffect(element, atk.effectData);

            vfx?.CreateOnHitVFX(col);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (!targetCheck) targetCheck = transform;
        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }
}
