using Godot;

public class MovementComponent
{
    private Player player;

    public MovementComponent(Player player)
    {
        this.player = player;
    }

    public void Move(Vector2 direction)
    {
        Vector2 velocity = direction * player.speed;
        player.Velocity = velocity;
        player.MoveAndSlide();
    }
}