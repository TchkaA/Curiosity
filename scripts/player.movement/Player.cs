using Godot;
using System;



public partial class Player : CharacterBody2D
{
	public static Player Instance { get; private set; }
	public StateMachine stateMachine = new StateMachine();
	public PlayerMoveState moveState;
	public PlayerIdleState idleState;

	public InputComponent inputComponent;

	public int speed {get; set; }

	public override void _Ready()
	{
		Instance = this;
		moveState = new PlayerMoveState();
		idleState = new PlayerIdleState();
		stateMachine.ChangeState(idleState);

		// Initialize the input component
		inputComponent = new InputComponent();
		speed = 200; // Set the player's speed
	}

	public override void _Process(double delta)
	{
		inputComponent.Update();
		stateMachine.Update(delta);
	}
}
