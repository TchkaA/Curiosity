using System;
using Godot;

public class NavigationComponent
{
    NavigationAgent2D _navigationAgent;
    private BaseNPC _owner;

    private float _stopDistance = 30f;

    // Getters
    public Vector2 TargetPosition => _navigationAgent.TargetPosition;
    public bool HasPath => !_navigationAgent.IsNavigationFinished();
    public bool IsFinished =>
        _navigationAgent.IsNavigationFinished();

    public float DistanceToTarget =>
        _owner.GlobalPosition.DistanceTo(_navigationAgent.TargetPosition);





    public NavigationComponent(BaseNPC owner, NavigationAgent2D agent2D)
    {
        _owner = owner;
        _navigationAgent = agent2D;
        _navigationAgent.VelocityComputed += OnVelocityComputed;   
    }

    private void AvoidanceDone() { }

    public void GoTo(Vector2 target, float stopDistance = 0)
    {
        _stopDistance = stopDistance;
        
        if (_navigationAgent.TargetPosition == target)
            return;
        if (_navigationAgent.TargetPosition.IsEqualApprox(target))
            return;

        _navigationAgent.TargetPosition = target;
    }

    public void Update()
    {
        if (NavigationServer2D.MapGetIterationId(_navigationAgent.GetNavigationMap()) == 0)
        {
            return;
        }

        // Проверяем, достигли ли нужной дистанции
        if (DistanceToTarget <= _stopDistance)
        {
            // Если достигли - останавливаемся
            _owner.movement.MoveNPC(Vector2.Zero);
            return;
        }

        if (_navigationAgent.IsNavigationFinished())
        {
            return;
        }

        Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();
        Vector2 direction = _owner.GlobalPosition.DirectionTo(nextPathPosition);
        
        // Желаемая скорость = направление × максимальная скорость персонажа
        Vector2 desiredVelocity = direction * _owner.Stats.Speed;

        // Обновляем направление взгляда/анимации (если нужно)
        _owner.directionComponent.SetDirection(direction);

        if (_navigationAgent.AvoidanceEnabled)
        {
            // Передаём желаемую скорость агенту, он сам вызовет VelocityComputed с безопасной скоростью
            _navigationAgent.Velocity = desiredVelocity;
        }
        else
        {
            // Если обход препятствий выключен, сразу двигаемся с желаемой скоростью
            OnVelocityComputed(desiredVelocity);
        }
    }

    private void OnVelocityComputed(Vector2 safeVelocity)
    {
        // Теперь Move просто применяет переданную скорость
        _owner.movement.MoveNPC(safeVelocity);
    }

    public void Stop()
    {
        GoTo(_owner.GlobalPosition);
    }



}