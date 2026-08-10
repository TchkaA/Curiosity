using Godot;
using System;
public partial class PlayerIdleState : IState, ISubState
{
    private Player _player;
    public System.Action<IState> Switch { get; set; }

    public PlayerIdleState(Player player) => _player = player;

    public void Enter()
    {
        _player.Animation.SetAnimation("idle");
    }

    public void Exit() { }

    public void Update(double delta)
    {
        var input = _player.inputComponent.MovementInput;
        if (input != Vector2.Zero)
        {
            Switch?.Invoke(_player.moveState);   // ← внутри иерархии, не корень
            return;
        }
    }
}

public partial class PlayerMoveState : IState, ISubState
{
    private Player _player;
    public System.Action<IState> Switch { get; set; }

    public PlayerMoveState(Player player) => _player = player;

    public void Enter() { }
    public void Exit() { }

    public void Update(double delta)
    {
        var input = _player.inputComponent.MovementInput;
        if (input != Vector2.Zero)
        {
            _player.movementComponent.Move(input);
            _player.Animation.SetAnimation("move");
        }
        else
        {
            Switch?.Invoke(_player.idleState);
        }
    }
}

public partial class InteractionState : IState
{
    public Player Player;

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public InteractionState(Player _player)
    {
        Player = _player;
    }

    public void Update(double delta)
    {
        
    }
}

public partial class PunchingState : IState
{
    public Player Player;

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public PunchingState(Player _player)
    {
        Player = _player;
    }

    public void Update(double delta)
    {
        
    }
}