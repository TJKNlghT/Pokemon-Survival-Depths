using Unity.VisualScripting;
using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    public Enemy_AttackState(StateMachine stateMachine, string animBoolName, Enemy enemy) : base(stateMachine, animBoolName, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.SetVelocity(0f, 0f);

        // face the player when starting the attack
        if (enemy.target != null)
        {
            Vector2 toTarget = (Vector2)enemy.target.position - enemy.rb.position;
            if (toTarget.sqrMagnitude > 0.001f)
                enemy.lastDir = toTarget.normalized;
        }

        SyncAttackSpeed();
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // stay still during attack
        enemy.SetVelocity(0f, 0f);
    }

    public override void UpdateAnimationParameters()
    {
        anim.SetFloat("xVelocity", enemy.lastDir.x);
        anim.SetFloat("yVelocity", enemy.lastDir.y);
    }
}
