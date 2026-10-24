using Godot;

public class MovementComponent
{
    /// <summary>
    /// Скорость ходьбы.
    /// </summary>
    private const float WalkSpeed = 90;

    private BaseEntity _owner;

    public MovementComponent(BaseEntity owner)
    {
        _owner = owner;
    }

    public void Move(Vector2 direction)
    {
        MoveAt(direction, WalkSpeed);
    }

    public void Run(Vector2 direction)
    {
        MoveAt(direction, _owner.Stats.Speed);
    }

    /// <summary>
    /// Движение рывком с заданной скоростью.
    /// </summary>
    /// <param name="direction">Нормализованное направление рывка</param>
    /// <param name="speed">Скорость рывка</param>
    public void Dash(Vector2 direction, float speed)
    {
        MoveAt(direction, speed);
    }

    private void MoveAt(Vector2 direction, float speed)
    {
        _owner.Velocity = direction * speed;
        _owner.MoveAndSlide();
    }
}