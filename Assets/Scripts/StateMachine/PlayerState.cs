using UnityEngine;

public abstract class PlayerState : EntityState
{
    protected Player player;

    protected PlayerInputSet input;

    public PlayerState(StateMachine stateMachine, string animBoolName, Player player) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
        stats = player.stats;
    }
}
