using Godot;
using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;



public partial class Player : BaseEntity, IInventoryOwner
{
	public StateMachine stateMachine = new StateMachine();
	public PlayerMoveState moveState;
	public PlayerIdleState idleState;
	public InputComponent inputComponent;
	public MovementComponent movementComponent;
	public InteractionComponent Interact;
	public AnimatedSprite2D sprite;
	public Inventory Inventory { get; private set; }
	public FollowMenuComponent followMenu;

	public Player()	//TODO: Перенести логику создания в конструктор.
	{
		Inventory = new Inventory(this);
	}

	public override void _Ready()
	{
		base._Ready();

		moveState = new PlayerMoveState(this);
		idleState = new PlayerIdleState(this);
		stateMachine.ChangeState(idleState);
		Inventory.AddItem(GD.Load<Item>("res://assets/Origin/objects/Resources/HealthPoitions/health_poition.tres"));

		// movement component
		movementComponent = new MovementComponent(this);

		// Initialize the input component
		inputComponent = new InputComponent(this);

		// Interact Component
		Interact = new InteractionComponent(this);
		Interact.InitialInteractionArea();

		//Follow Menu
		followMenu = new(this);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		inputComponent.Update();
		stateMachine?.Update(delta);
		followMenu.Update(delta);
	}

    internal void OpenMenu()
    {
        if(followMenu.IsMenuOpen == false)
		{
			followMenu.OpenMenu();
		}
		else
		{
			followMenu.CloseMenu();
		}
    }
}
