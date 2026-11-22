using System.Reflection.Metadata;
using Godot;


[GlobalClass]
public partial class WorldObject : StaticBody2D, IInteractable
{
    [ExportCategory("Setup")]
    [Export]
    public DirectionComponent.FacingDirection Direction;

    private DirectionComponent _direction;
    [Export]
    public ObjectRes Resource;

    [Export]
    public Vector2 CollisionSize = new Vector2(40,10);

    [Export]
    public Vector2 VisualSize = new Vector2(2.5f, 2.5f);

    public ShaderMaterial material;
    
    public Sprite2D Visual;

    public override void _Ready()
    {
        if (Resource == null) return;

        SetupCollision();
        SetupVisual();

        Visual.Texture = Resource.SpriteDown;

        InitComponents();
        LinkActions();

        _direction.SetDirection(Direction);

        
    }

    public void InitComponents()
    {
        _direction = new(this);
    }

    public void LinkActions()
    {
        _direction.DirectionChanged += ChangeVisualDirection;
    }


    public void ChangeVisualDirection(DirectionComponent.FacingDirection direction)
    {
        switch (direction)
        {
            case DirectionComponent.FacingDirection.Up: 
                Visual.Texture = Resource.SpriteUp; break;
            case DirectionComponent.FacingDirection.Down: 
                Visual.Texture = Resource.SpriteDown; break;
            case DirectionComponent.FacingDirection.Left: 
                Visual.Texture = Resource.SpriteLeft; break;
            case DirectionComponent.FacingDirection.Right: 
                Visual.Texture = Resource.SpriteRight; break;
        }
    }


    public virtual void Interact(BaseEntity interactor)
	{
	}

    public virtual void InteractEnter()
    {
        if (material == null)
            return;

        material.SetShaderParameter("outline_size", 0.8f);
    }

    public virtual void InteractExit()
	{
		if (material == null)
			return;

		material.SetShaderParameter("outline_size", 0f);
	}


	private void SetupVisual()
	{
		Visual = new() {Scale = VisualSize};
        AddChild(Visual);

		var shader = GD.Load<Shader>("res://Visual/Shaders/Outline/outline.gdshader");
		material = new ShaderMaterial { Shader = shader };
		material.SetShaderParameter("outline_size", 0f);
        Visual.Material = material;
	}

    private void SetupCollision()
	{
        SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, Consts.EntityLayer);
        SetDeferred(CollisionObject2D.PropertyName.CollisionMask, Consts.EntityLayer);

		var collision = new CollisionShape2D
		{
			Shape = new RectangleShape2D { Size = CollisionSize }
		};
		AddChild(collision);
	}
}