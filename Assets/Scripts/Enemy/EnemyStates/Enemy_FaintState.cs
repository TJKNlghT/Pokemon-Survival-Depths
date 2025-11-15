using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy_FaintState : EnemyState
{
    private Entity_VFX entityVfx;
    public Enemy_FaintState(StateMachine stateMachine, string animBoolName, Enemy enemy) : base(stateMachine, animBoolName, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateMachine.SwitchOffStateMachine();
        rb.simulated = false;
    }
}
