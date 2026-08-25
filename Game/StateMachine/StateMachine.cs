using Godot;
using System;

public partial class StateMachine: IState
{
	public IState CurrentState { get; private set; }
	public IState PreviousState { get; private set; }
	public void ChangeState(IState newState)
	{
		if (CurrentState != null)
		{
			CurrentState.Exit();
		}

		PreviousState = CurrentState;
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
