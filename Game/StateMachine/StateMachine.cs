using Godot;
using System;

public partial class StateMachine: IState
{
	public IState CurrentState { get; private set; }
	public IState PreviousState { get; private set; }
	public BaseEntity player {get; private set;}
	public string CurrentHierarchy = "Exploring";
	public StateMachine(BaseEntity Player)
	{
		player = Player;
	}


	public void ChangeState(IState newState)
	{
		if (CurrentState != null)
		{
			CurrentState.Exit();
		}

		CurrentState = newState;

		if (CurrentState != null)
		{
			CurrentState.Enter();
		}
	}

	
	public void Enter()
	{
		
	}

	public void Exit()
	{
	}

	public void Update(double delta)
	{
		if (CurrentState != null)
		{
			CurrentState.Update(delta);
		}
	}

	
}
