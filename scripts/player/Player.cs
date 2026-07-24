using Godot;
using System;



public partial class Player : BaseEntity
{
	public StateMachine stateMachine = new StateMachine();
	public PlayerMoveState moveState;
	public PlayerIdleState idleState;
	public InputComponent inputComponent;
	public MovementComponent movementComponent;
	public AnimatedSprite2D sprite;

	public override void _Ready()
	{
		base._Ready();

		moveState = new PlayerMoveState(this);
		idleState = new PlayerIdleState(this);
		stateMachine.ChangeState(idleState);


		// movement component
		movementComponent = new MovementComponent(this);

		// Initialize the input component
		inputComponent = new InputComponent(this);


		InitialInteractionArea();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		inputComponent.Update();
		stateMachine?.Update(delta);
	}




	public virtual void Interact()
    {
        var bodies = _interactionArea.GetOverlappingBodies();
		foreach (var body in bodies)
		{
			if (body is IInteractable obj)
			{
				obj.Interact();
			}
		}
    }
    public virtual void InteractEnter(Node2D body)
    {
        if (body is IInteractable obj)
		{
			obj.InteractEnter();
		}
    }

    public virtual void InteractExit(Node2D body)
    {
        if (body is IInteractable obj)
		{
			obj.InteractExit();
		}
    }

	private void InitialInteractionArea()
    {
        _interactionArea = new Area2D
        {
            Name = "InteractionArea"
        };

        var shape = new CollisionShape2D();
        var circleShape = new CircleShape2D
        {
            Radius = 40.0f // Радиус зоны взаимодействия
        };
        shape.Shape = circleShape;
        _interactionArea.CollisionLayer = 0; // Не участвует в коллизиях физики
        _interactionArea.CollisionMask = 1 << 2;
        _interactionArea.InputPickable = true;
        
        // Добавляем Area2D в текущую ноду
        AddChild(_interactionArea);

        _interactionArea.BodyEntered += InteractEnter;
        _interactionArea.BodyExited += InteractExit;
    }
}
