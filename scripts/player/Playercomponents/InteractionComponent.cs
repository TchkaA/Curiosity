using Godot;

public partial class InteractionComponent
{
    private Area2D _interactionArea;

    private Node2D _owner;

    public InteractionComponent(Node2D owner)
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
                break;
			}
		}
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
            Radius = 30.0f // Радиус зоны взаимодействия
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
    }
}