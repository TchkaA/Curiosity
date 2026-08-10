using Godot;
using System;
using PlayerHFSM;


public partial class Player : BaseEntity, IInventoryOwner
{
	public StateMachine stateMachine;
	public ExploringState ExploringState;
	public CombatState CombatState;
	public PlayerMoveState moveState;
	public PlayerIdleState idleState;
	public InteractionState interactionState;
	public PunchingState punchingState;

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

		stateMachine = new(this);
		moveState = new(this);
		idleState = new(this);
		interactionState = new(this);
		punchingState = new(this);

		ExploringState = new ExploringState(
            stateMachine,
            idleState,
            moveState,
            interactionState
        );

		CombatState = new CombatState(
			stateMachine,
            idleState,
            moveState,
			punchingState
			);

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

		stateMachine.ChangeState(ExploringState);
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
	public void EnterCombat()
    {
        stateMachine.ChangeState(CombatState);
    }
    
    // Возврат к исследованию
    public void ExitCombat()
    {
        stateMachine.ChangeState(ExploringState);
    }
}
