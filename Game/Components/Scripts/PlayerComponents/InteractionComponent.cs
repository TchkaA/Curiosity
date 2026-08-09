using Godot;

public partial class InteractionComponent
{
    private Area2D _interactionArea;

    private BaseEntity _owner;

    public InteractionComponent(BaseEntity owner)
    {
        _owner = owner;
    }

    public void Interact()
    {
        var bodies = _interactionArea.GetOverlappingBodies();
		foreach (var body in bodies)
		{
			if (body is IInteractable obj)
			{
				obj.Interact(_owner);
			}
            if (body is ISpeekable speekable)
            {
                GD.Print("sdasd");
                var owner = _owner as Player;
                owner.OpenMenu();
                owner.followMenu.Initialize(speekable.GetAnswers(_owner));
            }
		}
        // Проверяем области
        // var areas = _interactionArea.GetOverlappingAreas();
        // foreach (var area in areas)
        // {
        //     if (area is IInteractable obj)
        //     {
        //         obj.Interact(_owner);
        //         return;
        //     }
        // }
    }
    public void InteractEnter(Node2D body)
    {
        if (body is IInteractable obj)
		{
			obj.InteractEnter();
		}
    }

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
            Radius = 18.0f // Радиус зоны взаимодействия
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