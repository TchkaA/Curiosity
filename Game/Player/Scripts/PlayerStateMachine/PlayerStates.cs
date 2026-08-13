using Godot;

/// <summary>
/// Плоский FSM игрока: состояния сами решают, куда перейти
/// и вызывают _player.stateMachine.ChangeState(...).
/// </summary>
public partial class PlayerIdleState : IState
{
    private readonly Player _player;

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
            _player.stateMachine.ChangeState(_player.moveState);
        }
    }
}

public partial class PlayerMoveState : IState
{
    private readonly Player _player;

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
            _player.stateMachine.ChangeState(_player.idleState);
        }
    }
}

/// <summary>
/// Игрок в диалоге: движение заблокировано, выход по Esc (pause).
/// </summary>
public partial class InteractionState : IState
{
    private readonly Player _player;

    public InteractionState(Player player) => _player = player;

    public void Enter() { }
    public void Exit() { }

    public void Update(double delta)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            _player.ExitDialogue();
        }
    }
}