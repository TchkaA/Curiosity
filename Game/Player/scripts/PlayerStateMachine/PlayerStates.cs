using Godot;
using System;

public partial class PlayerMoveState : IState
{
    public Player Player;
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
        if (Player.isRunning)
        {
            Player.stateMachine.ChangeState(Player.runState);
            return;
        }
        if (input != Vector2.Zero)
        {
            Player.movement.Move(input);
            Player.animation.SetAnimation("walk");
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
            if (_player.isRunning)
            {
                _player.stateMachine.ChangeState(_player.runState);
                return;
            }
            _player.stateMachine.ChangeState(_player.moveState);
            return;
        }
    }
}

public partial class PlayerRunState : IState
{
    public Player Player;
    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public PlayerRunState(Player _player)
    {
        Player = _player;
    }

    public void Update(double delta)
    {
        if (!Player.isRunning)
        {
            Player.stateMachine.ChangeState(Player.moveState);
            return;
        }
        Vector2 input = Player.inputComponent.MovementInput;
        if (input != Vector2.Zero)
        {
            Player.movement.Run(input);
            Player.animation.SetAnimation("run");
        }
        else
        {
            Player.stateMachine.ChangeState(Player.idleState);
        }

    }
}

/// <summary>
/// Состояние рывка игрока.
/// Направление берётся из ввода (8 сторон), без ввода — рывок назад от текущего направления.
/// </summary>
public partial class PlayerDashState : IState
{
    /// <summary>
    /// Длительность рывка в секундах.
    /// </summary>
    private const double Duration = 0.46;

    public Player Player;

    private double _elapsed;
    private Vector2 _dashDirection;

    public PlayerDashState(Player player)
    {
        Player = player;
    }

    public void Enter()
    {
        Player.ChangeDashFlag(false);
        _elapsed = 0;
        _dashDirection = GetDashDirection();
        Player.animation.SetAnimation("dash");
    }

    public void Exit()
    {
        Player.ChangeDashFlag(true);
        Player.Velocity = Vector2.Zero;
    }

    public void Update(double delta)
    {
        _elapsed += delta;
        if (_elapsed >= Duration)
        {
            Player.stateMachine.ChangeState(Player.NextState());
            return;
        }

        Player.movement.Dash(_dashDirection, Player.Stats.DashSpeed);
    }

    /// <summary>
    /// Ввод задаёт направление рывка (8 сторон).
    /// Без ввода — рывок назад от текущего направления.
    /// </summary>
    private Vector2 GetDashDirection()
    {
        Vector2 input = Player.inputComponent.MovementInput;
        return input != Vector2.Zero ? input : -Player.direction.ToVector();
    }

    
}


public partial class PlayerAttackState : IState
{
    private Player Player;
    
    public void Enter()
    {
        Player.combatComponent.StartAttack();
    }

    public void Exit()
    {
        Player.combatComponent.EndAttack();
    }

    public PlayerAttackState(Player _player)
    {
        Player = _player;
    }

    public void Update(double delta)
    {
        if (Player.combatComponent.StopAttack)
        {
            Player.stateMachine.ChangeState(Player.NextState());
            return;
        }
    }

    
}


public partial class PlayerInteractionState : IState
{
    private Player Player;
    public BaseEntity interactor;

    public bool CanStopDialogue = false;
    public void Enter()
    {
        Player.InDialogue = true;


        Player.animation.SetAnimation("idle");
        Camera.Instance.AddZoom(new Vector2(2,2));
    }

    public void Exit()
    {
        Player.InDialogue = false;
        Camera.Instance.RemoveZoom();
        Camera.Instance.RemoveTarget(interactor);
    }

    public PlayerInteractionState(Player _player)
    {
        Player = _player;
    }

    public void Update(double delta)
    {
        if (Input.IsKeyPressed(Key.Escape))
        {
            if (CanStopDialogue) return;
            Player.stateMachine.ChangeState(Player.idleState);
        }
    }

    public void Interact(BaseEntity entity)
    {
        interactor = entity;
        Player.direction.TurnTo(entity.GlobalPosition);
        Camera.Instance.AddTarget(interactor);
    }
}
