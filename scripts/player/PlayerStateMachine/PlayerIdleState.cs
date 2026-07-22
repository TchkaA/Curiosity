using Godot;
using System;
public partial class PlayerIdleState : IState
{
    private Player _player;
    Vector2 input;
    public void Enter()
    {
        GD.Print("Entering Idle State");
        GD.Print(_player.directionComponent.CurrentDirection);
    }

    public void Exit()
    {
        GD.Print("Exiting Idle State");
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
        }
    }
}