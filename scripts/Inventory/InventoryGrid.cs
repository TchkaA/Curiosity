using System.Collections.Generic;
using Godot;

public partial class InventoryGrid : GridContainer
{
    private Inventory _inventory;
    private readonly List<InventorySlot> _slots = new();

    public override void _Ready()
    {
        InitializeSlotsFromChildren();

        if (MainManager.Instance != null && MainManager.Instance.Player != null)
        {
            Bind(MainManager.Instance.Player.Inventory);
        }
    }

    public void InitializeSlotsFromChildren()
    {
        _slots.Clear();

        foreach (Node child in GetChildren())
        {
            if (child is InventorySlot slot)
            {
                _slots.Add(slot);
            }
        }

        // Refresh();
    }

    public void Bind(Inventory inventory)
    {
        _inventory = inventory;

        if (_slots.Count == 0)
        {
            InitializeSlotsFromChildren();
        }

        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (i < _inventory.SlotCount)
            {
                _slots[i].SetItem(_inventory.GetSlot(i));
            }
            else
            {
                _slots[i].Clear();
            }
        }
    }
}