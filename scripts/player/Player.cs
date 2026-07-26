using Godot;
using System;
using System.Security.Cryptography.X509Certificates;



public partial class Player : BaseEntity
{
	public StateMachine stateMachine = new StateMachine();
	public PlayerMoveState moveState;
	public PlayerIdleState idleState;
	public InputComponent inputComponent;
	public MovementComponent movementComponent;
	public InteractionComponent Interact;
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

		// Interact Component
		Interact = new InteractionComponent(this);
		Interact.InitialInteractionArea();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		inputComponent.Update();
		stateMachine?.Update(delta);
	}

}
