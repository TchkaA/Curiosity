using Godot;
using System;



public partial class Player : CharacterBody2D
{
	public static Player Instance { get; private set; }
	public StateMachine stateMachine = new StateMachine();
	public PlayerMoveState moveState;
	public PlayerIdleState idleState;
	public int speed = 300;

	public override void _Ready()
	{
		Instance = this;
		moveState = new PlayerMoveState();
		idleState = new PlayerIdleState();
		stateMachine.ChangeState(idleState);
	}

	public override void _Process(double delta)
	{
		stateMachine.Update(delta);
	}
}
