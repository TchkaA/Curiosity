using Godot;
using System;

namespace NPC.StateMachine;


public partial class NPCIdleState : IState
{
    private BaseNPC _owner;

    public void Enter()
    {
        // Останавливаем любое движение – цель навигации становится равна текущей позиции
        _owner.Navigation.Stop();
        _owner.animationComponent.SetAnimation("idle"); // можно раскомментировать при наличии
    }

    public void Exit()
    {
    }

    public NPCIdleState(BaseNPC owner)
    {
        _owner = owner;
    }

    public void Update(double delta)
    {

    }
}

public partial class NPCFollowState : IState
{
    private BaseNPC _owner;
    public Vector2 _targetPosition;
    public void Enter()
    {
        
    }

    public void Exit()
    {
    }

    public NPCFollowState(BaseNPC owner)
    {
        _owner = owner;
    }

    public void Update(double delta)
    {
        _owner.animationComponent.SetAnimation("move");

        // Если навигация сообщает, что цель достигнута, переходим обратно в состояние покоя
        if (_owner.Navigation.IsFinished)
        {
            _owner.stateMachine.ChangeState(_owner.IdleState);
        }
    }
}

public partial class NPCDied : IState
{
    private BaseNPC _owner;
    public void Enter()
    {
        _owner.Navigation.Stop();
        _owner.animationComponent.SetAnimation("die");
    }

    public void Exit()
    {
    }

    public NPCDied(BaseNPC owner)
    {
        _owner = owner;
    }

    public void Update(double delta)
    {
        
    }
}