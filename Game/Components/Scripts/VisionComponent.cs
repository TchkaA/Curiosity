using System;
using System.Reflection.Metadata;
using Godot;

public class VisionComponent
{
    private BaseEntity _owner { get; set; }
    private Area2D area {get; set;}

    private const float OFFSET = 45f;
    private const float Radius = 50f;

    public VisionComponent(BaseEntity owner)
    {
        _owner = owner;
        InitVisionArea();
    }

    private void InitVisionArea()
    {
        area = new Area2D() {Name = "VisionArea"};
        var shape = new CollisionShape2D() {Shape = new CircleShape2D(){Radius = Radius} };

        area.AddChild(shape);
        area.BodyEntered += OnVisionAreaEntered;
        
        // Добавляем Area2D в дерево сцены
        _owner.AddChild(area);

        Update();
    }

    private void OnVisionAreaEntered(Node2D body)
    {
        GD.Print("Entered to vision component");
    }

    private Vector2 GetDirectionOffset(DirectionComponent.FacingDirection direction)
    {
        return direction switch
        {
            DirectionComponent.FacingDirection.Up => new Vector2(0, -OFFSET),
            DirectionComponent.FacingDirection.Down => new Vector2(0, OFFSET),
            DirectionComponent.FacingDirection.Left => new Vector2(-OFFSET, 0),
            DirectionComponent.FacingDirection.Right => new Vector2(OFFSET, 0),
            _ => Vector2.Zero
        };
    }

    public void Update()
    {
        if (_owner?.directionComponent == null) return;

        Vector2 offset = GetDirectionOffset(_owner.directionComponent.CurrentDirection);
        area.Position = offset;
    }
}