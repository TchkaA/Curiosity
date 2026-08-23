using System;
using System.Reflection.Metadata;
using Godot;

public class VisionComponent
{
    private BaseEntity _owner { get; set; }
    private Area2D area {get; set;}
    private RayCast2D rayCast {get;set;}
    public bool InVision {get; private set;} = false;

    private const float OFFSET = 45f;
    private const float Radius = 50f;

    public bool IsFolloving {get;set;} = false;

    public VisionComponent(BaseEntity owner)
    {
        _owner = owner;
        InitVisionArea();
    }

    private void InitVisionArea()
    {
        area = new Area2D() {Name = "VisionArea"};
        var shape = new CollisionShape2D() {Shape = new CircleShape2D(){Radius = Radius} };

        InitRayCast();
        
        area.AddChild(shape);
        area.BodyEntered += OnVisionAreaEntered;
        area.BodyExited += OnVicisonAreaExited;
        
        // Добавляем Area2D в дерево сцены
        _owner.AddChild(area);

        Update();
    }

    private void OnVicisonAreaExited(Node2D body)
    {
        InVision = false;
    }


    private void OnVisionAreaEntered(Node2D body)
    {
        InVision = true;

        rayCast.TargetPosition = body.GlobalPosition - _owner.GlobalPosition;
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
        if (InVision)
        {
            if (rayCast.GetCollider() is Node2D collider)
            {
                rayCast.TargetPosition = collider.GlobalPosition - _owner.GlobalPosition;
                rayCast.ForceRaycastUpdate(); // Принудительно обновляем
                
                // Проверяем, видит ли персонаж цель
                if (rayCast.IsColliding())
                {
                    var hit = rayCast.GetCollider();
                    followPlayer(collider);
                }
            }
        }

        if (_owner.directionComponent == null || !GodotObject.IsInstanceValid(area)) return;
        Vector2 offset = GetDirectionOffset(_owner.directionComponent.CurrentDirection);
        area.Position = offset;
    }

    private void InitRayCast()
    {
        if (rayCast == null)
        {
            rayCast = new RayCast2D() { Name = "RayCast2D" };
            rayCast.Enabled = false; // Отключаем автоматическое обновление
            _owner.AddChild(rayCast);
        }
        
        // Настраиваем RayCast
        rayCast.Scale = new Vector2(0.235f,0.235f);
        rayCast.GlobalPosition = _owner.GlobalPosition;
        rayCast.Enabled = true;
    }

    private void followPlayer(Node2D obj)
    {
        if(IsFolloving)
        {
            if (obj is Player player)
            {
                _owner.SetDirection(player.GlobalPosition);
            }
        }
    }

    public void Cleanup()
    {
        InVision = false;

        rayCast?.QueueFree();
        area?.QueueFree();
    }
}