using UnityEngine;

public class Enemy_Vespiquen : Enemy
{
    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(stateMachine, "idle", this);
        moveState = new Enemy_MoveState(stateMachine, "move", this);
        attackState = new Enemy_AttackState(stateMachine, "attack", this);
        faintState = new Enemy_FaintState(stateMachine, "faint", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }
}
