using Godot;
using System;


public partial class Player : BaseEntity, IInventoryOwner
{
	public StateMachine stateMachine = new StateMachine();
	public PlayerMoveState moveState;
	public PlayerIdleState idleState;
    public PlayerInteractionState interactionState;
	public PlayerPunchState punchState;

	public InputComponent inputComponent;
	public MovementComponent movementComponent;
	public InteractionComponent Interact;
	public CombatComponent combatC;


	public AnimatedSprite2D sprite;
	public Inventory Inventory { get; private set; }
	public FollowMenuComponent followMenu;


    


    //----------------------------------
	public Player()	//TODO: Перенести логику создания в конструктор.
	{
		Inventory = new Inventory(this);
	}

	public override void _Ready()
	{
		base._Ready();

		moveState = new(this);
		idleState = new(this);
        interactionState = new(this);
		punchState = new(this);
		stateMachine.ChangeState(idleState);
		Inventory.AddItem(GD.Load<Item>("res://assets/Origin/objects/Resources/HealthPoitions/health_poition.tres"));

		// movement component
		movementComponent = new(this);

		// Initialize the input component
		inputComponent = new(this);

		// Interact Component
		Interact = new(this);
		Interact.InitialInteractionArea();

		//Follow Menu
		followMenu = new(this);
		
		combatC = new(this);
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

    /// <summary>Начать диалог с NPC: блокирует движение, выход по Esc.</summary>
    public void EnterToTalk(BaseNPC body)
    {
        InDialogue = true;
        stateMachine.ChangeState(interactionState);
    }

    public void ReturnToBase()
    {
        InDialogue = false;
        stateMachine?.ChangeState(idleState);
        followMenu?.CloseMenu();
        Camera.Instance.RemoveZoom();
    }


    /// <summary>Выйти из диалога: закрыть меню и вернуться в idle.</summary>
    public void ExitDialogue()
    {
        InDialogue = false;
        followMenu.CloseMenu();
        stateMachine.ChangeState(idleState);
    }

    internal void attack()
    {
		combatC.PerformAttack();
        stateMachine.ChangeState(punchState);
		
    }

}
