using Godot;

/// <summary>
/// Визуальное представление одного слота инвентаря.
/// Отвечает ТОЛЬКО за отображение и передачу событий ввода (клики, перетаскивание).
/// Не принимает решений о том, куда переместить предмет.
/// </summary>
public partial class InventorySlot : Control
{
    // Сигналы для внешней логики
    [Signal] public delegate void LeftClickedEventHandler(InventorySlot slot);
    [Signal] public delegate void RightClickedEventHandler(InventorySlot slot);
    [Signal] public delegate void DragStartedEventHandler(InventorySlot slot, Variant dragData);

    [Export] public TextureRect IconTexture; // Лучше использовать TextureRect, а не TextureButton для DnD
    [Export] public Label CountLabel;

    public InventorySlotData CurrentData { get; private set; }

    public override void _Ready()
    {
        // Убедимся, что узел может принимать фокус и события мыши
        MouseFilter = MouseFilterEnum.Stop;
    }

    public void SetData(InventorySlotData data)
    {
        CurrentData = data;

        if (data == null || data.Item == null || data.Count <= 0)
        {
            Clear();
            return;
        }

        if (IconTexture != null)
            IconTexture.Texture = data.Item.Icon;

        if (CountLabel != null)
        {
            CountLabel.Visible = data.Count > 1;
            CountLabel.Text = data.Count.ToString();
        }
    }

    public void Clear()
    {
        CurrentData = null;
        if (IconTexture != null) IconTexture.Texture = null;
        if (CountLabel != null) 
        {
            CountLabel.Visible = false;
            CountLabel.Text = "";
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                EmitSignal(SignalName.LeftClicked, this);
                AcceptEvent(); // Сообщаем Godot, что мы обработали клик
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Right)
            {
                EmitSignal(SignalName.RightClicked, this);
                AcceptEvent();
            }
        }
    }

    // ==========================================
    // TODO: DRAG AND DROP (Godot 4.x)
    // ==========================================
    
    /// <summary>
    /// Вызывается, когда пользователь начинает тащить этот узел.
    /// </summary>
    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (CurrentData == null) return default;

        // Создаем визуальное представление перетаскиваемого предмета (preview)
        var preview = new TextureRect
        {
            Texture = CurrentData.Item.Icon,
            Size = new Vector2(32, 32) // Подстрой под размер своих иконок
        };
        SetDragPreview(preview);

        // Возвращаем данные, которые будут переданы в _DropData
        // Можно вернуть сам CurrentData или словарь с информацией
        var dragData = new Godot.Collections.Dictionary
        {
            ["source_slot"] = this,
            ["item"] = CurrentData.Item,
            ["count"] = CurrentData.Count
        };

        EmitSignal(SignalName.DragStarted, this, dragData);
        return dragData;
    }

    /// <summary>
    /// Вызывается, когда над этим узлом пролетает перетаскиваемый объект.
    /// Возвращает true, если этот слот может принять данный объект.
    /// </summary>
    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        // TODO: Проверить, что data - это наш формат словаря
        // TODO: Проверить, можно ли сюда положить этот предмет (например, не тащим ли мы слот сам в себя)
        return data.VariantType == Variant.Type.Dictionary; 
    }

    /// <summary>
    /// Вызывается, когда объект "отпускают" над этим узлом.
    /// Здесь должна быть логика обмена/перемещения, но лучше делегировать её менеджеру через сигнал.
    /// </summary>
    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (data.VariantType != Variant.Type.Dictionary) return;

        var dict = (Godot.Collections.Dictionary)data;
        var sourceSlot = (InventorySlot)dict["source_slot"];
        
        // TODO: Вместо прямой логики здесь, лучше эмитить сигнал:
        // EmitSignal(SignalName.ItemDroppedHere, sourceSlot, this, dict);
        // А менеджер инвентаря уже решит, как поменять местами предметы.
        
        GD.Print($"Drop detected from {sourceSlot} to {this}");
    }
}