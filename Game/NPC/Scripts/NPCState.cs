using Godot;
using System;

namespace NPC.StateMachine;

public partial class NPCIdleState : IState
{
    private BaseNPC _owner;

    public NPCIdleState(BaseNPC owner)
    {
        _owner = owner;
    }

    public void Enter()
    {
        _owner.Navigation.Stop();
        _owner.Animation?.SetAnimation("idle");
    }

    public void Exit()
    {
    }

    public void Update(double delta)
    {
    }
}

public partial class NPCFollowState : IState
{
    private BaseNPC _owner;

    public NPCFollowState(BaseNPC owner)
    {
        _owner = owner;
    }

    public void Enter()
    {
        _owner.Animation?.SetAnimation("move");
    }

    public void Exit()
    {
    }

    public void Update(double delta)
    {
        if (_owner.Navigation.IsFinished)
        {
            _owner.stateMachine.ChangeState(_owner.IdleState);
            return;
        }

        // Если хочешь, чтобы NPC поворачивался во время движения:
        if (_owner.agent != null)
        {
            _owner.directionComponent.TurnTo(_owner.agent.GetNextPathPosition());
        }
    }
}