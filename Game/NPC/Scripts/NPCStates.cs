using Godot;
using System;
using System.Threading.Tasks;

namespace NPC.StateMachine;


public partial class NPCIdleState : IState
{
    private BaseNPC _owner;

    public void Enter()
    {
        // Останавливаем любое движение – цель навигации становится равна текущей позиции
        _owner.Navigation.Stop();
        _owner.animation.SetAnimation("idle");
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
                _owner.animation.SetAnimation("walk");
                _owner.Navigation.GoTo(targetPoint);
            }
            if (_owner.Navigation.IsFinished)
            {
                _owner.animation.SetAnimation("idle");
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
        _owner.animation.SetAnimation("lying");
        _owner.SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, 0);
        _owner.SetDeferred(CollisionObject2D.PropertyName.CollisionMask, 0);

        _owner.vision.Cleanup();

        _owner.ZIndex = Consts.DiedIndex;
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

public partial class NPCFightState : IState
{
    private BaseNPC _owner;
    public Node2D _targetPosition;

    public float FOLLOW_DISTANCE { get; private set; } = 50f;  


    public void Enter()
    {
        _owner.vision.IsFolloving = true;
    }

    public void Exit()
    {
        _owner.vision.IsFolloving = false;
    }

    public NPCFightState(BaseNPC owner)
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
            
            if (distance > FOLLOW_DISTANCE * 1.2f)
            {
                _owner.animation.SetAnimation("move");
                _owner.Navigation.GoTo(targetPoint);
            }
            if (_owner.Navigation.IsFinished)
            {
                _owner.stateMachine.ChangeState(_owner.attackState);
            }
        }
    }

    internal void SelectTarget(Node2D interactor)
    {
        _targetPosition = interactor;
    }
}

public partial class NPCAttackState(BaseNPC owner) : IState
{
    private readonly BaseNPC _owner = owner;

    public async void Enter()
    {
        _owner.IsAttacking = true;
    }

    public void Exit()
    {
        _owner.IsAttacking = false;
    }

    public void Update(double delta)
    {
    }
}


public partial class NPCInteractState : IState
{
    private BaseNPC _owner;
    public void Enter()
    {
        _owner.animation.SetAnimation("idle");
    }

    public void Exit()
    {
    }

    public NPCInteractState(BaseNPC owner)
    {
        _owner = owner;
    }

    public void Update(double delta)
    {
        
    }
    public void Interact(Node2D interactor)
    {
        _owner.direction.TurnTo(interactor.GlobalPosition);
    }
}