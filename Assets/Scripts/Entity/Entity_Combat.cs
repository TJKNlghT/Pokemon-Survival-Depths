using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    protected Entity entity;
    protected Entity_VFX vfx;
    protected Entity_Stats stats;

    public DamageScaleData basicAttackScale;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab; // projectile prefab
    [SerializeField] private float projectileSpeed = 5f;  // default speed; can be overridden per projectile later

    [Header("Target Detection")]
    [SerializeField] public float targetCheckRadius = 0.5f;
    [SerializeField] private float forwardOffset = 1f;
    [SerializeField] public LayerMask whatIsTarget { get; protected set; }

    [Header("Attack timing")]
    [SerializeField] private float baseAttackCooldown = 1f; // fallback if stat missing
    private float nextAttackTime;

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();


        if (whatIsTarget == 0)
        {
            if (transform.CompareTag("Enemy"))
                whatIsTarget = LayerMask.GetMask("Player");
            else if (transform.CompareTag("Player"))
                whatIsTarget = LayerMask.GetMask("Enemy");
        }
    }

    public void EquipProjectile(GameObject prefab, float speed = -1f)
    {
        projectilePrefab = prefab;
        if (speed > 0f) projectileSpeed = speed;
    }

    public bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

    protected void ConsumeAttackCooldown()
    {
        float cd = baseAttackCooldown;

        if (stats != null && stats.offense.attackCooldown != null)
        {
            cd = stats.offense.attackCooldown.GetValue();
        }

        //scale cooldown by attackSpeed (can be removed if unbalanced cuz idk how this scales)
        float atkSpeed = stats.offense.attackSpeed.GetValue();
        cd /= Mathf.Max(0.1f, atkSpeed);

        nextAttackTime = Time.time + Mathf.Max(0.05f, cd);
    }
    public virtual void ShootProjectile()
    {
        if (!projectilePrefab) return;

        // Direction from lastDir
        Vector2 dir = (entity && entity.lastDir.sqrMagnitude > 0.001f)
            ? entity.lastDir.normalized
            : Vector2.down;

        Vector3 spawnPos = transform.position + (Vector3)(dir * forwardOffset);
        GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        var ownerCols = GetComponentsInChildren<Collider2D>();
        var projCols = go.GetComponentsInChildren<Collider2D>();
        foreach (var oc in ownerCols)
            foreach (var pc in projCols)
                if (oc && pc) Physics2D.IgnoreCollision(oc, pc, true);

        LayerMask targetMask = gameObject.CompareTag("Player")
            ? LayerMask.GetMask("Enemy")
            : LayerMask.GetMask("Player");

        LayerMask obstacleMask = LayerMask.GetMask("Obstacle");

        var proj = go.GetComponent<ProjectileObject_Base>();
        if (proj != null)
        {
            proj.SetMasksAndVFX(targetMask, obstacleMask, vfx);
            Vector2 velocity = dir * projectileSpeed;
            proj.SetupProjectile(stats, velocity, basicAttackScale);
        }

        ConsumeAttackCooldown();
    }

    public virtual void PerformAttack()
    {
        // guard by cooldown
        if (!CanAttack())
            return;

        var hits = GetDetectedColliders();

        foreach (var target in hits)
        {
            IDamagable damagable = target.GetComponent<IDamagable>();
            if (damagable == null)
                continue;

            AttackData attackData = stats.GetAttackData(basicAttackScale);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            float physicalDamage = attackData.physicalDamage;
            damagable.TakeDamage(physicalDamage, transform);

            ElementType element = stats != null ? stats.Element : ElementType.Normal;
            if (element != ElementType.Normal)
                statusHandler?.ApplyStatusEffect(element, attackData.effectData);

            HandlePostHitVfx(target);
        }

        // only consume cooldown
        ConsumeAttackCooldown();
    }

    protected virtual void HandlePostHitVfx(Collider2D target)
    {
        vfx?.CreateOnHitVFX(target);
    }

    protected Collider2D[] GetDetectedColliders()
    {
        Vector2 center = GetCheckCenter();
        return Physics2D.OverlapCircleAll(center, targetCheckRadius, whatIsTarget);
    }

    protected Vector2 GetCheckCenter()
    {
        // base position: rb if we have it, else transform
        Vector2 basePos = entity != null && entity.rb != null
            ? entity.rb.position
            : (Vector2)transform.position;

        // facing direction: use lastDir, fallback down if zero
        Vector2 dir = Vector2.down;
        if (entity != null && entity.lastDir.sqrMagnitude > 0.01f)
            dir = entity.lastDir.normalized;

        return basePos + dir * forwardOffset;
    }

    private void OnDrawGizmos()
    {

        Vector2 center = GetCheckCenter();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, targetCheckRadius);
    }
}
