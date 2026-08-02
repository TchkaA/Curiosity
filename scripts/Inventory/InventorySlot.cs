using Godot;

public partial class InventorySlot : Control
{
    public InventorySlotData CurrentItem { get; set; }
    public int Count { get; set; }
    [Export]
    public TextureButton Icon;
    [Export]
    public Label LabelCount;

    public void SetItem(InventorySlotData item)
    {
        if (item == null || item.Item == null || item.Item.Icon == null)
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

        if (Icon != null)
        {
            Icon.TextureNormal = null;
        }
    }
}