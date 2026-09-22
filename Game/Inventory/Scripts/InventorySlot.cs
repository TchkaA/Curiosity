using Godot;

/// <summary>
/// Визуальное представление одного слота инвентаря.
/// Отвечает за отображение данных и передачу событий ввода (клики, перетаскивание).
/// Не принимает решений о перемещении предметов, делегируя это внешним обработчикам.
/// </summary>
public partial class InventorySlot : Control
{
    [Signal] public delegate void LeftClickedEventHandler(InventorySlot slot);
    [Signal] public delegate void RightClickedEventHandler(InventorySlot slot);
    [Signal] public delegate void ItemDroppedEventHandler(InventorySlot sourceSlot, InventorySlot targetSlot);

    [Export] public TextureRect IconTexture;
    [Export] public Label CountLabel;

    public InventorySlotData CurrentData { get; private set; }
    public int SlotIndex { get; set; }

    public override void _Ready()
    {
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

        if (IconTexture != null && data.Item.Icon != null)
        {
            IconTexture.Texture = data.Item.Icon;
        }

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
            if (mouseEvent.ButtonIndex == MouseButton.Right)
            {
                EmitSignal(SignalName.RightClicked, this);
                AcceptEvent();
            }
        }
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (CurrentData == null) return default;

        var preview = new TextureRect
        {
            Texture = CurrentData.Item.Icon,
            Size = new Vector2(32, 32),
            Modulate = new Color(1, 1, 1, 0.8f)
        };
        SetDragPreview(preview);

        return this;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if (data.VariantType != Variant.Type.Object) return false;
        
        var sourceSlot = data.As<InventorySlot>();
        return sourceSlot != null && sourceSlot != this && IsInstanceValid(sourceSlot);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (data.VariantType != Variant.Type.Object) return;

        var sourceSlot = data.As<InventorySlot>();
        if (sourceSlot != null)
        {
            EmitSignal(SignalName.ItemDropped, sourceSlot, this);
        }
    }
}