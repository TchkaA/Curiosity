using Godot;
using System;



public partial class Player : CharacterBody2D
{
	public static Player Instance { get; private set; }
	public StateMachine stateMachine = new StateMachine();
	public PlayerMoveState moveState;
	public PlayerIdleState idleState;
	public DirectionComponent directionComponent;
	public InputComponent inputComponent;
	public MovementComponent movementComponent;
	public StatsComponent statsComponent = new StatsComponent();
	public int speed = 200;

	public override void _Ready()
	{
		Instance = this;
		moveState = new PlayerMoveState(this);
		idleState = new PlayerIdleState(this);
		stateMachine.ChangeState(idleState);

		// Initialize the input component
		inputComponent = new InputComponent();

		// movement component
		movementComponent = new MovementComponent(this);

		// direction
		directionComponent = new DirectionComponent(this);
	}

	public override void _Process(double delta)
	{
		inputComponent.Update();
		directionComponent.UpdateDirection();
		stateMachine.Update(delta);
	}
}
