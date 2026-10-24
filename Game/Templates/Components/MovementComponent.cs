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
        Vector2 velocity = direction * 90; //Ну типа базовая ходьба, в будущем надо превратить в константу
        _owner.Velocity = velocity;
        _owner.MoveAndSlide();
    }
    public void Run(Vector2 direction)
    {
        Vector2 velocity = direction * _owner.Stats.Speed;
        _owner.Velocity = velocity;
        _owner.MoveAndSlide();
    }
}