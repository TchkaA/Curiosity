using Godot;
using System;
public partial class PlayerIdleState : IState
{
    public Player player;
    Vector2 input;
    public void Enter()
    {
        GD.Print("Entering Idle State");
    }

    public void Exit()
    {
        GD.Print("Exiting Idle State");
    }

    public PlayerIdleState(Player _player)
    {
        player = _player;
    }

    public void Update(double delta)
    {
        input = player.inputComponent.MovementInput;
        if (input != Vector2.Zero)
        {
            player.stateMachine.ChangeState(player.moveState);
        }
    }
}