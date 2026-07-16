using Godot;
using System;

public partial class PlayerMoveState : IState
{
    public Player Player;
    Vector2 input;
    Vector2 velocity;
    public void Enter()
    {
        GD.Print("Entering Move State");
    }

    public void Exit()
    {
        GD.Print("Exiting Move State");
    }

    public void Update(double delta)
    {
        input = Player.Instance.inputComponent.MovementInput;
        if (input != Vector2.Zero)
        {
            velocity = input * Player.Instance.speed;
            Player.Instance.Velocity = velocity;
            Player.Instance.MoveAndSlide();
        }
        else
        {
            Player.Instance.stateMachine.ChangeState(Player.Instance.idleState);
        }
    }
}