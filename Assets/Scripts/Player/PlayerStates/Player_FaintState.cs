using UnityEngine;
using UnityEngine.XR;

public class Player_FaintState : PlayerState
{
    public Player_FaintState(StateMachine stateMachine, string animBoolName, Player player) : base(stateMachine, animBoolName, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        input.Disable();
        rb.simulated = false;
    }

    public override void Exit()
    {
        base.Exit();
        // re-enable input & physics when leaving faint state
        input.Enable();
        rb.simulated = true;
    }
}
