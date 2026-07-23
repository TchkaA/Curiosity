using Godot;
using System;



public partial class Player : BaseEntity
{
	public StateMachine stateMachine = new StateMachine();
	public PlayerMoveState moveState;
	public PlayerIdleState idleState;
	public InputComponent inputComponent;
	public MovementComponent movementComponent;
	public AnimatedSprite2D sprite;

	public override void _Ready()
	{
		base._Ready();

		moveState = new PlayerMoveState(this);
		idleState = new PlayerIdleState(this);
		stateMachine.ChangeState(idleState);


		// movement component
		movementComponent = new MovementComponent(this);

		// Initialize the input component
		inputComponent = new InputComponent(this);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		inputComponent.Update();
		stateMachine?.Update(delta);
	}
}
