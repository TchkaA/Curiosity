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

    public void Update(double delta)
    {
        input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (input != Vector2.Zero)
        {
            Player.Instance.stateMachine.ChangeState(Player.Instance.moveState);
        }
    }
}