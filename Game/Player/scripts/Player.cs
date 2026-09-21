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

	//---------------------



	public override void _Ready()
	{
		base._Ready();
		GD.Print("-- PLAYER READY");
		InitStates();
		stateMachine.ChangeState(idleState);
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		inputComponent.Update();
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
	}

    public override void LinksActions()
    {
        base.LinksActions();
		GlobalBookMenu.IsMenuOpen += UI.ToggleMenu;
	}

}
