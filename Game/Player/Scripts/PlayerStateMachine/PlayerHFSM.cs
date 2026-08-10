using Godot;

namespace PlayerHFSM;

public abstract class HierarchicalState : IState
{
    private readonly AnimationComponent _animation;
    private readonly StateMachine _rootStateMachine;
    private IState _currentState;

    protected abstract string Prefix { get; }
    protected abstract IState InitialState { get; }

    protected HierarchicalState(AnimationComponent animation, StateMachine rootStateMachine)
    {
        _animation = animation;
        _rootStateMachine = rootStateMachine;
    }

    public void Enter()
    {
        _animation?.SetPrefix(Prefix);
        ChangeState(InitialState);
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
            _currentState.Exit();

        _currentState = state;

        if (_currentState is ISubState sub)
            sub.Switch = ChangeState;

        if (_currentState != null)
            _currentState.Enter();
    }

    public void ChangeRootState(IState newRootState)
    {
        _rootStateMachine.ChangeState(newRootState);
    }
}

/// <summary>
/// Состояние исследования. Иерархическое.
/// </summary>
public class ExploringState : HierarchicalState
{
    private readonly IState _idleState;
    private readonly IState _moveState;
    private readonly IState _interactionState;

    protected override string Prefix => "exploring";
    protected override IState InitialState => _idleState;

    public ExploringState(
        AnimationComponent animation,
        StateMachine rootStateMachine,
        IState idleState,
        IState moveState,
        IState interactionState)
        : base(animation, rootStateMachine)
    {
        _idleState = idleState;
        _moveState = moveState;
        _interactionState = interactionState;
    }
}

/// <summary>
/// Состояние боя. Иерархическое.
/// </summary>
public class CombatState : HierarchicalState
{
    private readonly IState _idleState;
    private readonly IState _moveState;
    private readonly IState _punchingState;

    protected override string Prefix => "combat";
    protected override IState InitialState => _idleState;

    public CombatState(
        AnimationComponent animation,
        StateMachine rootStateMachine,
        IState idleState,
        IState moveState,
        IState punchingState)
        : base(animation, rootStateMachine)
    {
        _idleState = idleState;
        _moveState = moveState;
        _punchingState = punchingState;
    }
}

