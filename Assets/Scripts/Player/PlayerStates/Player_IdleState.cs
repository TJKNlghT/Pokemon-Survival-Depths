using UnityEngine;

public class Player_IdleState : PlayerState
{
    public Player_IdleState(StateMachine stateMachine, string animBoolName, Player player) : base(stateMachine, animBoolName, player) {

    }

    public override void Enter(){
        base.Enter();

        player.SetVelocity(0f, 0f);

        // Don't update animation parameters if animator is disabled (during hurt sprite) or if knocked back
        if (anim != null && anim.enabled && !player.IsKnocked)
        {
            anim.SetFloat("xVelocity", player.lastDir.x);
            anim.SetFloat("yVelocity", player.lastDir.y);
        }
    }

    public override void Update(){
        if (player.combat != null && player.combat.CanAttack())
        {
            stateMachine.ChangeState(player.attackState);
            return;
        }

        base.Update();

        Vector2 currentDir = player.GetInputDir();

        if (player.lastDir == currentDir && player.HandleCollisionDetection(currentDir, out _))
            return;

        if (player.moveInput.sqrMagnitude > 0.01f)
            stateMachine.ChangeState(player.moveState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.SetVelocity(0f, 0f);

    }
}
