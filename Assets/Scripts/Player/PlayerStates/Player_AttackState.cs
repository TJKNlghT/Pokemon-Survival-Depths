using UnityEngine;

public class Player_AttackState : PlayerState
{

    public Player_AttackState(StateMachine stateMachine, string animBoolName, Player player) : base(stateMachine, animBoolName, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        SyncAttackSpeed();
        // Do NOT touch player.lastDir here.
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 dir = player.GetInputDir();

        if (dir.sqrMagnitude > 0.001f)
        {
            dir = dir.normalized;
            player.lastDir = dir;   // facing follows input while attacking

            if (!player.HandleCollisionDetection(dir, out _))
                player.SetVelocity(dir.x * player.moveSpeed, dir.y * player.moveSpeed);
            else
                player.SetVelocity(0f, 0f);
        }
        else
        {
            player.SetVelocity(0f, 0f);
        }
    }

    public override void UpdateAnimationParameters()
    {
        // drive attack direction using the latched dir
        anim.SetFloat("xVelocity", player.lastDir.x);
        anim.SetFloat("yVelocity", player.lastDir.y);
    }
}

