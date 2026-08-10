using System;
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

    private readonly Node2D _owner;

    public FacingDirection CurrentDirection { get; private set; } = FacingDirection.Down;

    public event Action<string> DirectionChanged;

    public DirectionComponent(Node2D owner = null)
    {
        _owner = owner;
    }

    public void SetDirection(Vector2 direction)
    {
        if (direction == Vector2.Zero)
            return;

        SetDirection(FromVector(direction));
    }

    public void SetDirection(FacingDirection direction)
    {
        if (CurrentDirection == direction)
            return;

        CurrentDirection = direction;
        DirectionChanged?.Invoke(ToAnimationString(CurrentDirection));
    }

    public void SetDirection(string direction)
    {
        if (Enum.TryParse(direction, true, out FacingDirection parsed))
        {
            SetDirection(parsed);
        }
        else
        {
            GD.PrintErr($"Invalid direction string: {direction}");
        }
    }

    /// <summary>
    /// Повернуться к мировой позиции цели.
    /// Использует owner, если он был передан в конструкторе.
    /// </summary>
    public void TurnTo(Vector2 targetPosition)
    {
        if (_owner == null)
        {
            GD.PrintErr("DirectionComponent.TurnTo called without owner.");
            return;
        }

        Vector2 direction = _owner.GlobalPosition.DirectionTo(targetPosition);
        SetDirection(direction);
    }

    /// <summary>
    /// Повернуться к цели без зависимости от owner.
    /// </summary>
    public void TurnTo(Vector2 targetPosition, Vector2 ownerPosition)
    {
        Vector2 direction = ownerPosition.DirectionTo(targetPosition);
        SetDirection(direction);
    }

    private static FacingDirection FromVector(Vector2 direction)
    {
        if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
        {
            return direction.X > 0f
                ? FacingDirection.Right
                : FacingDirection.Left;
        }

        return direction.Y > 0f
            ? FacingDirection.Down
            : FacingDirection.Up;
    }

    public static string ToAnimationString(FacingDirection direction)
    {
        return direction switch
        {
            FacingDirection.Up => "up",
            FacingDirection.Down => "down",
            FacingDirection.Left => "left",
            FacingDirection.Right => "right",
            _ => "down"
        };
    }
}