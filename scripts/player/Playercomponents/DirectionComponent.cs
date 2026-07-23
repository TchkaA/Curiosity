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
    private BaseEntity _owner;
    public FacingDirection CurrentDirection { get; private set; } = FacingDirection.Down;
    public DirectionComponent(BaseEntity owner)
    {
        _owner = owner;
    }
    // public void UpdateDirection()
    // {
    //     if (_owner.inputComponent?.MovementInput != Vector2.Zero)
    //     {
    //         UpdateCurrentDirection();
    //     }
    // }

    // private void UpdateCurrentDirection()
    // {
    //     var _playerDirection = _owner.inputComponent.MovementInput;
    //     if (_playerDirection.X > 0)
    //     {
    //         CurrentDirection = FacingDirection.Right;
    //     }
    //     else if (_playerDirection.X < 0)
    //     {
    //         CurrentDirection = FacingDirection.Left;
    //     }
    //     else if (_playerDirection.Y > 0)
    //     {
    //         CurrentDirection = FacingDirection.Down;
    //     }
    //     else if (_playerDirection.Y < 0)
    //     {
    //         CurrentDirection = FacingDirection.Up;
    //     }
    // }

    public void SetDirection(FacingDirection direction)
    {
        CurrentDirection = direction;
    }


    public void SetDirection(Vector2 direction)
    {
        if (direction.X > 0)
        {
            CurrentDirection = FacingDirection.Right;
        }
        else if (direction.X < 0)
        {
            CurrentDirection = FacingDirection.Left;
        }
        else if (direction.Y > 0)
        {
            CurrentDirection = FacingDirection.Down;
        }
        else if (direction.Y < 0)
        {
            CurrentDirection = FacingDirection.Up;
        }
    }

    public void SetDirection(string direction)
    {
        switch (direction.ToLower())
        {
            case "up":
                CurrentDirection = FacingDirection.Up;
                break;
            case "down":
                CurrentDirection = FacingDirection.Down;
                break;
            case "left":
                CurrentDirection = FacingDirection.Left;
                break;
            case "right":
                CurrentDirection = FacingDirection.Right;
                break;
            default:
                GD.PrintErr("Invalid direction string: " + direction);
                break;
        }
    }
}