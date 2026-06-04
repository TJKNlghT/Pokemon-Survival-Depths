using UnityEngine;

public class Player_AttackState : PlayerState
{

    // safety timeout so we never get stuck
    private float elapsedAttackTime;
    private const float MAX_ATTACK_DURATION = 0.6f; // tune to match your anim length

    public Player_AttackState(StateMachine stateMachine, string animBoolName, Player player)
        : base(stateMachine, animBoolName, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        SyncAttackSpeed();

        // reset timer every time we enter
        elapsedAttackTime = 0f;

        // IMPORTANT: ensure we start fresh each attack
        triggerCalled = false;
    }

    public override void Update()
    {
        base.Update();

        // track real time spent in this state
        elapsedAttackTime += Time.deltaTime;

        // 1) normal path: animation event calls AnimationFinishTrigger() -> triggerCalled = true
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.moveState);
            return;
        }

        // 2) safety fallback: if for some reason the event never fires,
        //    force-exit after some time so we don't get stuck.
        if (elapsedAttackTime >= MAX_ATTACK_DURATION)
        {
            // Debug.LogWarning("[Player_AttackState] Exiting via timeout fallback.");
            stateMachine.ChangeState(player.moveState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // same movement logic as before
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