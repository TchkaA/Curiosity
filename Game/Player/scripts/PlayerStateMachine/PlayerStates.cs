using Godot;
using System;

public partial class PlayerMoveState : IState
{
    public Player Player;
    public Vector2 input;
    public Vector2 velocity;
    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public PlayerMoveState(Player _player)
    {
        Player = _player;
    }

    public void Update(double delta)
    {
        Vector2 input = Player.inputComponent.MovementInput;
        if (input != Vector2.Zero)
        {
            Player.movement.Move(input);
            Player.animation.SetAnimation("move");
        }
        else
        {
            Player.stateMachine.ChangeState(Player.idleState);
        }
    }
}


/// <summary>
/// Состояние бездействия игрока.
/// </summary>
public partial class PlayerIdleState : IState
{
    private Player _player;
    Vector2 input;
    public void Enter()
    {
        _player.animation.SetAnimation("idle");
    }

    public void Exit()
    {
    }

    public PlayerIdleState(Player _player)
    {
        this._player = _player;
    }

    public void Update(double delta)
    {
        input = _player.inputComponent.MovementInput;
        if (input != Vector2.Zero)
        {
            _player.stateMachine.ChangeState(_player.moveState);
            return;
        }
    }
}

