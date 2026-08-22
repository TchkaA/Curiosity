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
    public Node2D _targetPosition;

    public float FOLLOW_DISTANCE { get; private set; } = 70f;  


    public void Enter()
    {
        _owner.vision.IsFolloving = true;
    }

    public void Exit()
    {
        _owner.vision.IsFolloving = false;
    }

    public NPCFollowState(BaseNPC owner)
    {
        _owner = owner;
    }

    public void Update(double delta)
    {
        if (_targetPosition != null)
        {
            // Вычисляем точку на расстоянии от цели
            Vector2 directionToTarget = (_owner.GlobalPosition - _targetPosition.GlobalPosition).Normalized();
            Vector2 targetPoint = _targetPosition.GlobalPosition + directionToTarget * FOLLOW_DISTANCE;
            
            float distance = _owner.GlobalPosition.DistanceTo(_targetPosition.GlobalPosition);
            
            if (distance > (FOLLOW_DISTANCE * 2.2))
            {
                _owner.animationComponent.SetAnimation("move");
                _owner.Navigation.GoTo(targetPoint);
            }
            if (_owner.Navigation.IsFinished)
            {
                _owner.animationComponent.SetAnimation("idle");
            }
        }
    }

    internal void SelectTarget(Node2D interactor)
    {
        _targetPosition = interactor;
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

public partial class NPCFight : IState
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

    public NPCFight(BaseNPC owner)
    {
        _owner = owner;
    }

    public void Update(double delta)
    {
        
    }
}