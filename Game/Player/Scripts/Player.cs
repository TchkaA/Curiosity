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
    public FollowMenuComponent followMenu;

    public Inventory Inventory { get; private set; }

    public Player()
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
            Animation,
            stateMachine,
            idleState,
            moveState,
            interactionState
        );

        CombatState = new CombatState(
            Animation,
            stateMachine,
            idleState,
            moveState,
            punchingState
        );

        movementComponent = new MovementComponent(this);
        inputComponent = new InputComponent(this);

        Interact = new InteractionComponent(this);
        Interact.InitialInteractionArea();

        followMenu = new(this);

        // Inventory.AddItem(
        //     GD.Load<Item>("res://assets/Origin/objects/Resources/HealthPoitions/health_poition.tres")
        // );

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
        if (followMenu.IsMenuOpen == false)
            followMenu.OpenMenu();
        else
            followMenu.CloseMenu();
    }

    public void EnterCombat()
    {
        stateMachine.ChangeState(CombatState);
    }

    public void ExitCombat()
    {
        stateMachine.ChangeState(ExploringState);
    }

    public void ToggleCombat()
	{
		GD.Print($"CurrentState = {stateMachine.CurrentState?.GetType().Name ?? "NULL"}");

		if (stateMachine.CurrentState is CombatState)
		{
			GD.Print("-> ExitCombat");
			ExitCombat();
		}
		else if (stateMachine.CurrentState is ExploringState)
		{
			GD.Print("-> EnterCombat");
			EnterCombat();
		}
		else
		{
			GD.Print("-> НЕ CombatState и НЕ ExploringState!");
		}
	}
}