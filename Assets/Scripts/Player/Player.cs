using System;
using System.Collections;
using UnityEngine;
public class Player : Entity
{
    public static event Action OnPlayerFaint;
    public PlayerInputSet input { get; private set; }
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_FaintState faintState { get; private set; }
    public Player_AttackState attackState { get; private set; }

    public Vector2 moveInput { get; private set; }

    public Entity_Combat combat { get; private set; }

    public float moveSpeed = 5f;

    protected override void Awake()
    {
        base.Awake();

        whatIsObstacle |= LayerMask.GetMask("Enemy");

        input = new PlayerInputSet();

        idleState = new Player_IdleState(stateMachine, "idle", this);
        moveState = new Player_MoveState(stateMachine, "move", this);
        faintState = new Player_FaintState(stateMachine, "faint", this);
        attackState = new Player_AttackState(stateMachine, "attack", this);
        combat = GetComponent<Entity_Combat>();
    }

    protected override void Start()
    {
        base.Start();


        var buffs = GetComponent<Entity_Buffs>();
        if (RunManager.Instance != null && buffs != null)
            RunManager.Instance.RegisterPlayer(buffs);

        stateMachine.Initialize(idleState);
    }

    public void ResetForRetry()
    {
        var health = GetComponent<Entity_Health>();
        if (health != null)
            health.ResetFullHealth();   // isDead = false, HP = max

        var xp = GetComponent<Player_XP>();
        xp?.ResetXPAndLevel(1);

        // reset facing / velocity
        lastDir = Vector2.down;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // force state machine out of faint and into idle
        if (stateMachine != null && idleState != null)
            stateMachine.ChangeState(idleState);
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSped = moveSpeed;
        float originalAnimSpeed = anim.speed;

        float speedMultiplier = 1 - slowMultiplier;

        moveSpeed = moveSpeed * speedMultiplier;
        anim.speed = anim.speed * speedMultiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSped;
        anim.speed = originalAnimSpeed;
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        input.Disable();
    }
    public Vector2 GetInputDir()
    {
        Vector2 input = moveInput;

        if (input.sqrMagnitude < 0.01f)
            return Vector2.zero;

        input = input.normalized;

        return new Vector2(input.x, input.y);
    }

    public override void EntityFaint()
    {
        base.EntityFaint();

        OnPlayerFaint?.Invoke();

        stateMachine.ChangeState(faintState);
    }
}
