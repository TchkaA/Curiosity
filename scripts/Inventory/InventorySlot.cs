using Godot;

public partial class InventorySlot : Control
{
    public InventorySlotData CurrentItem { get; set; }
    public int Count { get; set; }
    [Export]
    public TextureButton Icon;
    [Export]
    public Label LabelCount;
    private Player _player => MainManager.Instance.Player;
    [Export]
    public MenuButton menuButton;
    public override void _Ready()
    {
        if (Icon != null)
        {
            Icon.Pressed += UseItem;
        }
    }
    public void SetItem(InventorySlotData item)
    {
        if (item == null || item.Item == null || item.Item.Icon == null || item.Count == 0)
        {
            Clear();
            return;
        }

        if (Icon != null)
        {
            Icon.TextureNormal = item.Item.Icon;
        }

        CurrentItem = item;
        Count = item.Count;
        LabelCount.Text = Count.ToString();
    }

    public void Clear()
    {
        CurrentItem = null;
        Count = 0;
        LabelCount.Text = null;

        if (Icon != null)
        {
            Icon.TextureNormal = null;
        }
    }

    public void UseItem()
    {
        if(CurrentItem == null) return; // Maybe later add notification
        _player.Inventory.Use(CurrentItem.Item);
        SetItem(CurrentItem);
    }
}