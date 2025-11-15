using UnityEngine;

public class Enemy_MoveState : EnemyState
{
    private Vector2 dir;
    private float repathCooldown = 0.3f;
    private float nextRepathTime;
    private bool isRetreating;
    private Vector2 retreatDir;
    private float retreatTimeLeft;
    private const float retreatDistanceFactor = 3f; // how many attackRanges to move back
    private const float retreatSpeedMultiplier = 3f;


    public Enemy_MoveState(StateMachine stateMachine, string animBoolName, Enemy enemy) : base(stateMachine, animBoolName, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        nextRepathTime = 0f;
        enemy.RecalculatePath();
        isRetreating = false;
    }

    public override void Update()
    {

            if (enemy.target != null)
        {

            Vector2 toPlayer = (Vector2)enemy.target.position - enemy.rb.position;

            if (toPlayer.sqrMagnitude > 0.01f)
            {
                dir = toPlayer.normalized;
                enemy.lastDir = dir;
            }
            else
            {
                dir = Vector2.zero;
            }
        }
        else
        {
            dir = Vector2.zero;
        }

        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isRetreating)
        {
            HandleRetreat();
            return;
        }

        if (enemy.target == null)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // 2) Handle collisions
        if (enemy.HandleCollisionDetection(dir, out Collider2D hit) &&
            hit.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (enemy.IsProjectileAttacker)
            {
                // RANGED ENEMY: back off when too close to player
                Vector2 backDir = -dir.normalized;

                if (TryStartRetreat(backDir))
                    return; // OK: ranged skips attack/path this frame
            }
            else
            {
                // MELEE: stop pushing into the player
                enemy.SetVelocity(0f, 0f);
                dir = Vector2.zero;

                // allow melee to attack, then stop processing movement
                if (enemy.IsTargetInAttackRange() && enemy.combat.CanAttack())
                    stateMachine.ChangeState(enemy.attackState);

                return; // <--- important: prevents path movement this frame
            }
        }

        if (enemy.IsTargetInAttackRange() && enemy.combat.CanAttack())
        {
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        if (enemy.IsProjectileAttacker && enemy.IsTargetInAttackRange())
        {
            enemy.SetVelocity(0f, 0f);
            return;
        }

        // Repath every X seconds (or if no path)
        if (Time.time >= nextRepathTime || enemy.currentPath == null)
        {
            enemy.RecalculatePath();
            nextRepathTime = Time.time + repathCooldown;
        }


        if (!enemy.TryGetNextPathDir(out dir))
        {
            enemy.SetVelocity(0f, 0f);
            return;
        }

        enemy.lastDir = dir;

        enemy.SetVelocity(dir.x * enemy.moveSpeed, dir.y * enemy.moveSpeed);
    }

        private bool TryStartRetreat(Vector2 backDir)
    {
        backDir = backDir.normalized;

        float retreatDistance = enemy.attackRange * retreatDistanceFactor;
        float retreatSpeed = enemy.moveSpeed * retreatSpeedMultiplier;

        // Check Behind
        float radius = enemy.bodyCircle != null
            ? enemy.bodyCircle.radius * Mathf.Max(enemy.transform.lossyScale.x, enemy.transform.lossyScale.y)
            : 0.3f;

        float safetyMargin = 0.05f;

        RaycastHit2D hit = Physics2D.CircleCast(
            enemy.rb.position,
            radius,
            backDir,
            retreatDistance + safetyMargin,
            enemy.whatIsObstacle
        );

        if (hit.collider != null)
        {
            enemy.SetVelocity(0f, 0f);
            return false;
        }

        //Safe to retreat
        isRetreating = true;
        retreatDir = backDir;
        retreatTimeLeft = retreatDistance / Mathf.Max(retreatSpeed, 0.01f);

        enemy.SetVelocity(0f, 0f);
        return true;
    }

    private void HandleRetreat()
    {
        float retreatSpeed = enemy.moveSpeed * retreatSpeedMultiplier;

        // move backwards
        enemy.SetVelocity(retreatDir.x * retreatSpeed,
                          retreatDir.y * retreatSpeed);

        retreatTimeLeft -= Time.fixedDeltaTime;

        // update facing
        enemy.lastDir = retreatDir;

        if (retreatTimeLeft <= 0f)
        {
            isRetreating = false;
            enemy.SetVelocity(0f, 0f);
        }
    }

    public override void UpdateAnimationParameters()
    {
        anim.SetFloat("xVelocity", enemy.lastDir.x);
        anim.SetFloat("yVelocity", enemy.lastDir.y);
    }
}
