using Godot;
using System;

public partial class Player : BaseEntity
{

	//---------------------
	// 		Components
	//---------------------
	public InputComponent inputComponent;
	public InteractionComponent interaction;
	public UIComponent UI;

	//---------------------

	//---------------------
	//		Objects
	//---------------------
	[Export] 
	public BookMenu GlobalBookMenu; 

	//---------------------
	// 		States
	//---------------------
	public PlayerIdleState idleState;
	public PlayerMoveState moveState;
	public PlayerRunState runState;
	public PlayerDashState dashState;

	/// <summary>
	/// Остаток перезарядки рывка в секундах.
	/// </summary>
	private float _dashCooldown;

	


	public override void _Ready()
	{
		base._Ready();
		GD.Print("-- PLAYER READY");
		InitStates();
		stateMachine.ChangeState(idleState);

		#if DEBUG
		Inventory.AddItem(new HealingPotion(), 5);
		Inventory.AddItem(new ChickenMeat(),10);
		#endif
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		inputComponent.Update();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_dashCooldown > 0) _dashCooldown -= (float)delta;

        // Рывок прерывает любое состояние, кроме самого рывка, и доступен после перезарядки.
        if (inputComponent.DashPressed && _dashCooldown <= 0 && stateMachine.CurrentState != dashState)
        {
            _dashCooldown = Stats.DashCooldown;
            stateMachine.ChangeState(dashState);
        }

        base._PhysicsProcess(delta);
    }


	/// <summary>
	/// Билдер Компонентов.
	/// </summary>
	public override void InitComponents()
	{
		base.InitComponents();
		inputComponent = new(this);
		UI = new(this, GlobalBookMenu);
		interaction = new(this);
	}

	/// <summary>
	/// Билдер состояний.
	/// </summary>
	public void InitStates()
	{
		idleState = new(this);
		moveState = new(this);
		runState = new(this);
		dashState = new(this);
	}

    public override void LinksActions()
    {
        base.LinksActions();
		GlobalBookMenu.IsMenuOpen += UI.ToggleMenu;
	}

	

}