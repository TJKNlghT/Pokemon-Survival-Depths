using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public static event Action<Enemy> OnAnyEnemyDied;

    private bool hasReportedDeath = false;

    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_FaintState faintState;
    private Entity_VFX entityVfx;

    public float moveSpeed = 3f;

    [Header("Battle Details")]
    public Transform target;
    public float attackRange = 1.2f;  // stop to attack when within this

    [Header("Pathfinding")]
    public PathFindingGrid2D pathGrid;
    public float waypointTolerance = 0.05f;

    [HideInInspector] public List<Vector2> currentPath;
    [HideInInspector] public int pathIndex;

    public Entity_Combat combat { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        whatIsObstacle |= LayerMask.GetMask("Player");
        combat = GetComponent<Entity_Combat>();
        entityVfx = GetComponent<Entity_VFX>();
    }


    protected override void Start()
    {
        base.Start();

        if (target == null)
        {
            Player player = FindFirstObjectByType<Player>();
            if (player != null)
                target = player.transform;
        }

        if (pathGrid == null)
            pathGrid = FindFirstObjectByType<PathFindingGrid2D>();
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;
        float originalAnimSpeed = anim.speed;

        float speedMultiplier = 1 - slowMultiplier;

        moveSpeed = moveSpeed * speedMultiplier;
        anim.speed = anim.speed * speedMultiplier;
        
        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        anim.speed = originalAnimSpeed;
    }

    public override void EntityFaint()
    {
        //Prevent double-reporting and double-running faint logic
        if (hasReportedDeath)
            return;

        hasReportedDeath = true;

        base.EntityFaint();

        stateMachine.ChangeState(faintState);
        entityVfx?.PlayFaintVfxAndDisappear();

        OnAnyEnemyDied?.Invoke(this);
    }

    private void HandlePlayerFaint()
    {
        stateMachine.ChangeState(idleState);
    }

    public void RecalculatePath()
    {
        if (pathGrid == null || target == null) return;

        currentPath = pathGrid.FindPath(rb.position, target.position);
        pathIndex = 0;
    }

    public bool TryGetNextPathDir(out Vector2 dir)
    {
        dir = Vector2.zero;

        if (currentPath == null || pathIndex >= currentPath.Count)
            return false;

        Vector2 wp = currentPath[pathIndex];
        Vector2 pos = rb.position;
        Vector2 toWp = wp - pos;

        if (toWp.sqrMagnitude <= waypointTolerance * waypointTolerance)
        {
            pathIndex++;
            if (pathIndex >= currentPath.Count)
                return false;

            wp = currentPath[pathIndex];
            toWp = wp - pos;
        }

        if (toWp.sqrMagnitude < 0.0001f)
            return false;

        dir = toWp.normalized;
        return true;
    }

    public bool IsTargetInAttackRange()
    {
        Vector2 center = rb.position;
        Collider2D hit = Physics2D.OverlapCircle(center, attackRange, combat.whatIsTarget);
        return hit != null;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = rb != null ? (Vector3)rb.position : transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange);
    }

    private void OnEnable()
    {
        Player.OnPlayerFaint += HandlePlayerFaint;
    }

    private void OnDisable()
    {
        Player.OnPlayerFaint -= HandlePlayerFaint;
    }
}
