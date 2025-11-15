using UnityEngine;

public class Enemy_IdleState : EnemyState
{
    public Enemy_IdleState(StateMachine stateMachine, string animBoolName, Enemy enemy) : base(stateMachine, animBoolName, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.SetVelocity(0f, 0f);

        anim.SetFloat("xVelocity", enemy.lastDir.x);
        anim.SetFloat("yVelocity", enemy.lastDir.y);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.target == null) return;

        stateMachine.ChangeState(enemy.moveState);
    }
}
