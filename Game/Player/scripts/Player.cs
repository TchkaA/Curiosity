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


	#region Dash
	/// <summary>
	/// Метод для проверки возможности произведения рывка
	/// </summary>
	public void PerformDash()
	{
		if (CanDash && _dashCooldown >= Stats.DashCooldown)
		{
			_dashCooldown = 0f;
			stateMachine.ChangeState(dashState);
		}
		//Тут позже добавить сообщение выводящееся.
	}

	public void ChangeDashFlag(bool flag) => CanDash = flag;
	

	#endregion

}