using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory
{
    public List<Item> inventory = new();
    private readonly List<InventorySlot> _slots = new();

    public void AddItem(Item item)
    {
        inventory.Add(item);
    }

    public void RemoveItem(Item item)
    {
        inventory.Remove(item);
    }

    public void RemoveItems(Item item)
    {
        inventory.RemoveAll(x => x == item);
    }

    public bool ContainsItem(Item item)
    {
        return inventory.Contains(item);
    }
}
