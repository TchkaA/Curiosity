using Godot;

/// <summary>
/// Предмет на земле: можно осмотреть или подобрать.
/// Скрипт помечен [Tool], поэтому выполняется и в редакторе —
/// изменения Item, VisualScale и CollisionRadius сразу видны в сцене.
/// Созданные узлы (PickUpVisual, PickUpCollision) не сохраняются в сцену:
/// они являются предпросмотром и пересоздаются при загрузке.
/// </summary>
[Tool]
[GlobalClass]
public partial class PickUp : Area2D, IInteractable
{
    /// <summary>
    /// Предмет, который лежит на земле.
    /// В редакторе изменение сразу обновляет спрайт.
    /// </summary>
    [Export]
    public Item Item
    {
        get
        {
            return _item;
        }
        set
        {
            _item = value;

            if (Visual != null)
            {
                RefreshVisual();
            }
        }
    }
    private Item _item;

    [ExportGroup("Настройки")]

    /// <summary>
    /// Масштаб спрайта предмета.
    /// </summary>
    [Export]
    public float VisualScale
    {
        get
        {
            return _visualScale;
        }
        set
        {
            _visualScale = value;

            if (Visual != null)
            {
                Visual.Scale = new Vector2(_visualScale, _visualScale);
            }
        }
    }
    private float _visualScale = 2.0f;

    /// <summary>
    /// Радиус коллизии для взаимодействия.
    /// </summary>
    [Export]
    public float CollisionRadius
    {
        get
        {
            return _collisionRadius;
        }
        set
        {
            _collisionRadius = value;
            UpdateCollisionShape();
        }
    }
    private float _collisionRadius = 14.0f;

    /// <summary>
    /// Толщина обводки при наведении (используется только в рантайме).
    /// </summary>
    [Export]
    public float OutlineActiveSize = 0.8f;

    /// <summary>
    /// Спрайт предмета. Создаётся автоматически.
    /// </summary>
    public Sprite2D Visual;

    private ShaderMaterial _material;
    private CollisionShape2D _collision;
    private bool _isInRange;

    public int Count;

    /// <summary>
    /// Задать предмет до того, как нода попадёт в дерево сцены.
    /// Вся остальная настройка (визуал, коллизия, шейдер) произойдёт один раз в _Ready.
    /// </summary>
    public void Init(Item item)
    {
        Item = item;
    }

    public override void _Ready()
    {
        if (Item == null && !Engine.IsEditorHint())
        {
            GD.PrintErr(Name + ": PickUp создан без Item, настроен как Empty item.");
            Item = GD.Load<Item>("res://Objects/Items/Empty_item/Empty_item.tres");
        }

        if (!Engine.IsEditorHint() && Item != null)
        {
            Name = Item.Name;
        }

        ZIndex = -1;

        EnsureVisual();
        EnsureMaterial();
        EnsureCollision();

        RefreshVisual();

        CollisionLayer = Consts.EntityLayer;
        InputPickable = true;
    }

    /// <summary>
    /// Гарантирует наличие спрайта. Ищет уже созданный узел по имени,
    /// чтобы не плодить дубликаты при перезагрузке tool-скрипта.
    /// </summary>
    private void EnsureVisual()
    {
        if (Visual == null)
        {
            Visual = GetNodeOrNull<Sprite2D>("PickUpVisual");
        }

        if (Visual == null)
        {
            Visual = new Sprite2D();
            Visual.Name = "PickUpVisual";
            AddChild(Visual);
        }

        Visual.Scale = new Vector2(_visualScale, _visualScale);
    }

    /// <summary>
    /// Применяет текстуру предмета к спрайту. Если предмета нет — скрывает текстуру.
    /// </summary>
    private void RefreshVisual()
    {
        if (Visual == null)
        {
            return;
        }

        if (Item == null)
        {
            Visual.Texture = null;
            return;
        }

        Visual.Texture = Item.Icon;
        Visual.Scale = new Vector2(_visualScale, _visualScale);
    }

    /// <summary>
    /// Создаёт материал обводки, если он ещё не создан и шейдер найден.
    /// </summary>
    private void EnsureMaterial()
    {
        if (Visual == null || _material != null)
        {
            return;
        }

        Shader shader = GD.Load<Shader>("res://Visual/Shaders/Outline/outline.gdshader");
        if (shader == null)
        {
            return;
        }

        _material = new ShaderMaterial();
        _material.Shader = shader;
        _material.SetShaderParameter("outline_size", 0.0f);
        Visual.Material = _material;
    }

    /// <summary>
    /// Гарантирует наличие коллизии. Ищет уже созданный узел по имени.
    /// </summary>
    private void EnsureCollision()
    {
        if (_collision == null)
        {
            _collision = GetNodeOrNull<CollisionShape2D>("PickUpCollision");
        }

        if (_collision == null)
        {
            _collision = new CollisionShape2D();
            _collision.Name = "PickUpCollision";
            AddChild(_collision);
        }

        UpdateCollisionShape();
    }

    /// <summary>
    /// Обновляет радиус круговой коллизии.
    /// </summary>
    private void UpdateCollisionShape()
    {
        if (_collision == null)
        {
            return;
        }

        CircleShape2D circle = _collision.Shape as CircleShape2D;
        if (circle == null)
        {
            circle = new CircleShape2D();
            _collision.Shape = circle;
        }

        circle.Radius = _collisionRadius;
    }

    public void Interact(BaseEntity interactor)
    {
        #if DEBUG
        GD.Print("Interact with item " + Name);
        QueueFree();
        #endif
        // TODO: Реализовать добавление в инвентарь через IInventoryOwner.
        // Пример:
        // if (interactor is IInventoryOwner owner)
        // {
        //     owner.Inventory.AddItem(Item);
        //     QueueFree();
        // }
        // else
        // {
        //     GD.PushWarning("Cannot add item " + Item.Name + ": у " + interactor.Name + " нет инвентаря");
        // }
    }

    public void Inspect()
    {
        if (Item == null)
        {
            return;
        }

        GD.Print("Название - " + Item.Name + "\nОписание - " + Item.Description);
    }

    public virtual void InteractEnter()
    {
        _isInRange = true;

        if (_material != null)
        {
            _material.SetShaderParameter("outline_size", OutlineActiveSize);
        }
    }

    public virtual void InteractExit()
    {
        _isInRange = false;

        if (_material != null)
        {
            _material.SetShaderParameter("outline_size", 0.0f);
        }
    }
}