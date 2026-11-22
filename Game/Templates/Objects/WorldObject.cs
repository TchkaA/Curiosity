using Godot;

/// <summary>
/// Базовый физический объект мира: сундук, дверь, ящик и т.п.
/// Наследуется от StaticBody2D, поэтому не двигается силами физики,
/// но является препятствием для других тел.
///
/// Внимание: скрипт помечен [Tool] и выполняется в редакторе.
/// Созданные им узлы являются предпросмотром и не сохраняются в сцену,
/// если им не назначить Owner. Это сделано специально, чтобы редактор
/// не накапливал дубликаты при перезагрузке скрипта.
/// </summary>
[Tool]
[GlobalClass]
public partial class WorldObject : StaticBody2D, IInteractable
{
    [ExportCategory("Setup")]

    /// <summary>
    /// Начальное направление взгляда объекта.
    /// </summary>
    [Export]
    public DirectionComponent.FacingDirection Direction
    {
        get
        {
            return _directionValue;
        }
        set
        {
            _directionValue = value;

            if (_direction != null)
            {
                _direction.SetDirection(value);
            }
            else if (Engine.IsEditorHint())
            {
                ChangeVisualDirection(value);
            }
        }
    }
    private DirectionComponent.FacingDirection _directionValue;

    /// <summary>
    /// Ресурс с настройками внешнего вида и данными объекта.
    /// </summary>
    [Export]
    public ObjectRes Resource
    {
        get
        {
            return _resource;
        }
        set
        {
            _resource = value;

            if (_resource != null && Visual != null)
            {
                ChangeVisualDirection(Direction);
            }
        }
    }
    private ObjectRes _resource;

    /// <summary>
    /// Размер коллизии. Меняется в инспекторе и сразу применяется в редакторе.
    /// </summary>
    [Export]
    public Vector2 CollisionSize
    {
        get
        {
            return _collisionSize;
        }
        set
        {
            _collisionSize = value;

            if (Engine.IsEditorHint())
            {
                UpdateCollisionShapeSize();
            }
        }
    }
    private Vector2 _collisionSize = new Vector2(40.0f, 10.0f);

    /// <summary>
    /// Масштаб визуального спрайта. Меняется в инспекторе и сразу применяется в редакторе.
    /// </summary>
    [Export]
    public Vector2 VisualSize
    {
        get
        {
            return _visualSize;
        }
        set
        {
            _visualSize = value;

            if (Engine.IsEditorHint())
            {
                UpdateVisualScale();
            }
        }
    }
    private Vector2 _visualSize = new Vector2(2.5f, 2.5f);

    /// <summary>
    /// Материал обводки. Создаётся автоматически при наличии шейдера.
    /// </summary>
    public ShaderMaterial material;

    /// <summary>
    /// Спрайт объекта. Создаётся автоматически, если его нет.
    /// </summary>
    public Sprite2D Visual;

    private DirectionComponent _direction;
    private CollisionShape2D _collisionShape;
    private bool _actionsLinked;

    public override void _Ready()
    {
        EnsureCollision();
        EnsureVisual();
        InitComponents();
        LinkActions();

        if (_direction != null)
        {
            _direction.SetDirection(Direction);
        }

        if (Resource != null)
        {
            ChangeVisualDirection(Direction);
        }
    }

    /// <summary>
    /// Создаёт компонент направления, если он ещё не создан.
    /// </summary>
    public void InitComponents()
    {
        if (_direction == null)
        {
            _direction = new DirectionComponent(this);
        }
    }

    /// <summary>
    /// Подписывает компонент направления на обновление визуала.
    /// Повторная подписка не выполняется.
    /// </summary>
    public void LinkActions()
    {
        if (_actionsLinked)
        {
            return;
        }

        if (_direction != null)
        {
            _direction.DirectionChanged += ChangeVisualDirection;
            _actionsLinked = true;
        }
    }

    /// <summary>
    /// Меняет текстуру спрайта в зависимости от направления.
    /// </summary>
    public void ChangeVisualDirection(DirectionComponent.FacingDirection direction)
    {
        if (Visual == null || Resource == null)
        {
            return;
        }

        switch (direction)
        {
            case DirectionComponent.FacingDirection.Up:
                Visual.Texture = Resource.SpriteUp;
                break;
            case DirectionComponent.FacingDirection.Down:
                Visual.Texture = Resource.SpriteDown;
                break;
            case DirectionComponent.FacingDirection.Left:
                Visual.Texture = Resource.SpriteLeft;
                break;
            case DirectionComponent.FacingDirection.Right:
                Visual.Texture = Resource.SpriteRight;
                break;
        }
    }

    /// <summary>
    /// Вызывается при взаимодействии с объектом.
    /// Переопределяется в наследниках: дверях, сундуках и т.п.
    /// </summary>
    public virtual void Interact(BaseEntity interactor)
    {
    }

    /// <summary>
    /// Вызывается, когда interactor входит в зону взаимодействия.
    /// Включает обводку, если материал доступен.
    /// </summary>
    public virtual void InteractEnter()
    {
        if (material == null)
        {
            return;
        }

        material.SetShaderParameter("outline_size", 0.8f);
    }

    /// <summary>
    /// Вызывается, когда interactor выходит из зоны взаимодействия.
    /// Выключает обводку, если материал доступен.
    /// </summary>
    public virtual void InteractExit()
    {
        if (material == null)
        {
            return;
        }

        material.SetShaderParameter("outline_size", 0.0f);
    }

    /// <summary>
    /// Гарантирует наличие спрайта и материала обводки.
    /// Ищет уже созданный узел по имени, чтобы не плодить дубликаты в редакторе.
    /// </summary>
    private void EnsureVisual()
    {
        if (Visual == null)
        {
            Visual = GetNodeOrNull<Sprite2D>("WorldObjectVisual");
        }

        if (Visual == null)
        {
            Visual = new Sprite2D();
            Visual.Name = "WorldObjectVisual";
            AddChild(Visual);
        }

        Visual.Scale = _visualSize;
        EnsureMaterial();
    }

    /// <summary>
    /// Создаёт материал обводки, если он ещё не создан и шейдер найден.
    /// </summary>
    private void EnsureMaterial()
    {
        if (Visual == null || material != null)
        {
            return;
        }

        Shader shader = GD.Load<Shader>("res://Visual/Shaders/Outline/outline.gdshader");
        if (shader == null)
        {
            return;
        }

        material = new ShaderMaterial();
        material.Shader = shader;
        material.SetShaderParameter("outline_size", 0.0f);
        Visual.Material = material;
    }

    /// <summary>
    /// Гарантирует наличие коллизии и применяет текущий размер.
    /// </summary>
    private void EnsureCollision()
    {
        if (_collisionShape == null)
        {
            _collisionShape = GetNodeOrNull<CollisionShape2D>("WorldObjectCollision");
        }

        if (_collisionShape == null)
        {
            _collisionShape = new CollisionShape2D();
            _collisionShape.Name = "WorldObjectCollision";
            AddChild(_collisionShape);
        }

        SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, Consts.EntityLayer);
        SetDeferred(CollisionObject2D.PropertyName.CollisionMask, Consts.EntityLayer);

        UpdateCollisionShapeSize();
    }

    /// <summary>
    /// Обновляет размер прямоугольной коллизии.
    /// </summary>
    private void UpdateCollisionShapeSize()
    {
        if (_collisionShape == null)
        {
            return;
        }

        RectangleShape2D rectangle = _collisionShape.Shape as RectangleShape2D;
        if (rectangle == null)
        {
            rectangle = new RectangleShape2D();
            _collisionShape.Shape = rectangle;
        }

        rectangle.Size = _collisionSize;
    }

    /// <summary>
    /// Обновляет масштаб спрайта.
    /// </summary>
    private void UpdateVisualScale()
    {
        if (Visual == null)
        {
            return;
        }

        Visual.Scale = _visualSize;
    }
}