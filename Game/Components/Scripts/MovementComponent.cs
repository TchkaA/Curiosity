using Godot;

public class MovementComponent
{
    private BaseEntity _owner;

    public MovementComponent(BaseEntity owner)
    {
        _owner = owner;
    }

    public void Move(Vector2 direction)
    {
        Vector2 velocity = direction * _owner.Stats.Speed;
        _owner.Velocity = velocity;
        _owner.MoveAndSlide();
    }

    public void MoveNPC(Vector2 velocity)
    {
        _owner.Velocity = velocity;
        _owner.MoveAndSlide();
    }
}