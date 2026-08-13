using Godot;

/// <summary>
/// Простая машина состояний. Не знает об игроке — состояния сами решают,
/// куда переходить, вызывая stateMachine.ChangeState(...).
/// </summary>
public partial class StateMachine : IState
{
    public IState CurrentState { get; private set; }

    public void ChangeState(IState newState)
    {
        if (newState == CurrentState)
            return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public void Enter() { }
    public void Exit() { }

    public void Update(double delta)
    {
        CurrentState?.Update(delta);
    }
}
