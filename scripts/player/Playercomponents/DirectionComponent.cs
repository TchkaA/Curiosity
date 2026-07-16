using Godot;

public partial class DirectionComponent
{
    public enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    private Player _player;
    public FacingDirection CurrentDirection { get; private set; } = FacingDirection.Down;
    public DirectionComponent(Player player)
    {
        _player = player;
    }
    public void UpdateDirection()
    {
        if (_player.inputComponent.MovementInput != Vector2.Zero)
        {
            UpdateCurrentDirection();
        }
    }

    private void UpdateCurrentDirection()
    {
        var _playerDirection = _player.inputComponent.MovementInput;
        if (_playerDirection.X > 0)
        {
            CurrentDirection = FacingDirection.Right;
        }
        else if (_playerDirection.X < 0)
        {
            CurrentDirection = FacingDirection.Left;
        }
        else if (_playerDirection.Y > 0)
        {
            CurrentDirection = FacingDirection.Down;
        }
        else if (_playerDirection.Y < 0)
        {
            CurrentDirection = FacingDirection.Up;
        }
    }
}