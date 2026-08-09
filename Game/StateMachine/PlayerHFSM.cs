using System.Transactions;
using Godot;

namespace PlayerHFSM;

//TODO Сделать абстрактный класс для иерахичных состояний

/// <summary>
/// Состояние иследования(Иерархическое)
/// </summary>
public class ExploringState : IState
{
    private IState _idleState;
    private IState _moveState;
    private IState _interactionState;
    private IState _currentState;
    private StateMachine _rootStateMachine;  // Ссылка на корень HFSM
    
    public ExploringState(
        StateMachine rootStateMachine,
        IState idleState, 
        IState moveState, 
        IState interactionState)
    {
        _rootStateMachine = rootStateMachine;
        _idleState = idleState;
        _moveState = moveState;
        _interactionState = interactionState;
    }

    public void Enter()
    {
        ChangeState(_idleState);  // Устанавливаем начальное состояние
        _rootStateMachine.CurrentHierarchy = "Exploring";
        GD.Print(_rootStateMachine.CurrentHierarchy);
    }

    public void Exit()
    {
        _currentState?.Exit();
        _currentState = null;
    }

    public void Update(double delta)
    {
        _currentState?.Update(delta);
    }

    public void ChangeState(IState state)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }
        
        _currentState = state;
        
        if (_currentState != null)
        {
            _currentState.Enter();
        }
    }
    
    /// <summary>
    /// Метод для переключения на другой уровень HFSM
    /// </summary>
    /// <param name="newRootState">Другой уровень HFSM</param>
    public void ChangeRootState(IState newRootState)
    {
        _rootStateMachine.ChangeState(newRootState);
    }
}

/// <summary>
/// Состояние иследования(Иерархическое)
/// </summary>
public class CombatState : IState
{
    private IState _idleState;
    private IState _moveState;
    private IState _punchingState;
    private IState _currentState;
    private StateMachine _rootStateMachine;  // Ссылка на корень HFSM
    
    public CombatState(
        StateMachine rootStateMachine,
        IState idleState, 
        IState moveState, 
        IState punchingState)
    {
        _rootStateMachine = rootStateMachine;
        _idleState = idleState;
        _moveState = moveState;
        _punchingState = punchingState;
    }

    public void Enter()
    {
        ChangeState(_idleState);  // Устанавливаем начальное состояние
        _rootStateMachine.CurrentHierarchy = "Combat";
        GD.Print(_rootStateMachine.CurrentHierarchy);
    }

    public void Exit()
    {
        _currentState?.Exit();
        _currentState = null;
    }

    public void Update(double delta)
    {
        _currentState?.Update(delta);
    }

    public void ChangeState(IState state)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }
        
        _currentState = state;
        
        if (_currentState != null)
        {
            _currentState.Enter();
        }
    }
    
    /// <summary>
    /// Метод для переключения на другой уровень HFSM
    /// </summary>
    /// <param name="newRootState">Другой уровень HFSM</param>
    public void ChangeRootState(IState newRootState)
    {
        _rootStateMachine.ChangeState(newRootState);
    }
}