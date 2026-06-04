using UnityEngine;

public class Player_MoveState : PlayerState
{
    private Vector2 dir;
    public Player_MoveState(StateMachine stateMachine, string animBoolName, Player player) : base(stateMachine, animBoolName, player) {
        
    }

    public override void Update()
    {
        if (player.combat != null && player.combat.CanAttack())
        {
            stateMachine.ChangeState(player.attackState);
            return;
        }

        dir = player.GetInputDir();

        // Don't update lastDir during knockback to prevent animation conflicts
        if (dir != Vector2.zero && !player.IsKnocked)
            player.lastDir = dir;

        base.Update();
    }

    public override void FixedUpdate() {

        base.FixedUpdate();

        if (dir == Vector2.zero || player.HandleCollisionDetection(dir, out _))
        {
            stateMachine.ChangeState(player.idleState);
        } else
        {
            player.SetVelocity(dir.x * player.moveSpeed, dir.y * player.moveSpeed);
        }
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();

        // Don't update animation parameters if animator is disabled (during hurt sprite) or if knocked back
        if (anim != null && anim.enabled && !player.IsKnocked)
        {
            anim.SetFloat("xVelocity", player.lastDir.x);
            anim.SetFloat("yVelocity", player.lastDir.y);
        }
    }
}
