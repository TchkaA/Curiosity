using Godot;
public interface IDirectable
{
    void SetDirection(Vector2 direction);
    DirectionComponent directionComponent { get; set; }
}