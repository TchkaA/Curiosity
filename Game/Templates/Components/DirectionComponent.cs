using System;
using Godot;

/// <summary>
/// Компонент направления.
/// </summary>
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

    /// <summary>
    /// Текущее направление
    /// </summary>
    public FacingDirection CurrentDirection { get; private set; } = FacingDirection.Down;

    /// <summary>
    /// Предыдущее направление.
    /// Используется только для проверки и дебага
    /// </summary>
    public FacingDirection PreviousDirection { get; private set; }


	/// <summary>
	/// Событие изменения направления.
	/// </summary>
	public event Action<FacingDirection> DirectionChanged;
	//---------------------------------
    public DirectionComponent(BaseEntity owner)
    {
        _owner = owner;
    }

    public void SetDirection(FacingDirection direction)
    {
		if (CurrentDirection == direction) return;
		CurrentDirection = direction;
		DirectionChanged?.Invoke(direction);
    }


    /// <summary>
    /// Изменить направление с помощью вектора направления
    /// </summary>
    /// <param name="direction">Вектор направления</param>
    public void SetDirectionFromVector(Vector2 direction)
    {
        PreviousDirection = CurrentDirection;
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
        

        if(PreviousDirection != CurrentDirection)
        {
            GD.Print(CurrentDirection);
            DirectionChanged?.Invoke(CurrentDirection);
        }

        
    }

    public void TurnTo(Vector2 direction)
    {
        var targetDirection = _owner.GlobalPosition.DirectionTo(direction);

        // Сравниваем модули, чтобы понять, какая ось важнее
        if (Math.Abs(targetDirection.X) > Math.Abs(targetDirection.Y))
        {
            // Горизонтальное преобладает
            if (targetDirection.X > 0)
                CurrentDirection = FacingDirection.Right;
            else if (targetDirection.X < 0)
                CurrentDirection = FacingDirection.Left;
        }
        else
        {
            // Вертикальное преобладает (или равны – тогда вертикаль)
            if (targetDirection.Y > 0)
                CurrentDirection = FacingDirection.Down;
            else if (targetDirection.Y < 0)
                CurrentDirection = FacingDirection.Up;
        }

        DirectionChanged?.Invoke(CurrentDirection);
    }
}