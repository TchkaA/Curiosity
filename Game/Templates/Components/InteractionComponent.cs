using Godot;

/// <summary>
/// Компонент взаимодействия с объектами.
/// </summary>
public partial class InteractionComponent
{
    private Area2D _interactionArea;

    private BaseEntity _owner;

    public InteractionComponent(BaseEntity owner)
    {
        _owner = owner;

        InitialInteractionArea(); // Инициализируется Area сразу в конструкторе, тк в любом случае писали бы в Ready или метод иницииализации
    }


    /// <summary>
    /// метод взаимодействия, который проходит по всем элементам, что находятся в зоне действия.
    /// TODO: сделать возможность выбора элементов
    /// </summary>
    public void Interact()
    {
        var bodies = _interactionArea.GetOverlappingBodies();
		foreach (var body in bodies)
		{
			if (body is IInteractable obj)
			{
				obj.Interact(_owner);
			}
		}
        // Проверяем области
        var areas = _interactionArea.GetOverlappingAreas();
        foreach (var area in areas)
        {
            if (area is IInteractable obj)
            {
                obj.Interact(_owner);
                return;
            }
        }
    }

    /// <summary>
    /// Метод входа в зону действия.
    /// Просто уведомляет
    /// </summary>
    /// <param name="body">Тело вошедшее в зону</param>
    public void InteractEnter(Node2D body)
    {
        if (body is IInteractable obj)
		{
			obj.InteractEnter();
		}
    }

    /// <summary>
    /// Метод выхода из зоны действия
    /// Просто уведомляет
    /// </summary>
    /// <param name="body">Тело вышедшее из зоны</param>
    public void InteractExit(Node2D body)
    {
        if (body is IInteractable obj)
		{
			obj.InteractExit();
		}
    }

	public void InitialInteractionArea()
    {
        _interactionArea = new Area2D
        {
            Name = "InteractionArea"
        };

        var shape = new CollisionShape2D();
        var circleShape = new CircleShape2D
        {
            Radius = 80.0f // Радиус зоны взаимодействия
        };
        shape.Shape = circleShape;
        // _interactionArea.CollisionLayer = 0; // Не участвует в коллизиях физики
        // _interactionArea.CollisionMask = 1 << 2;
        _interactionArea.InputPickable = true;
        
        // Добавляем Area2D в текущую ноду
        _owner.AddChild(_interactionArea);
        _interactionArea.AddChild(shape);

        _interactionArea.BodyEntered += InteractEnter;
        _interactionArea.BodyExited += InteractExit;
        _interactionArea.AreaEntered += InteractEnter;
        _interactionArea.AreaExited += InteractExit;
    }
}