using System;

public class InventorySlotData
{
    public Item Item { get; set; }
    public int Count { get; set; }

    public InventorySlotData()
    {
    }

    public InventorySlotData(Item item, int count)
    {
        Item = item;
        Count = Math.Max(0, count);
    }
}