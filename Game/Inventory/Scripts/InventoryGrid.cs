using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Godot;

public partial class InventoryGrid : GridContainer
{
    public Inventory Inventory {get; private set;}
    private readonly List<InventorySlot> _slots = new();



    [Export]
    public string CurInventory = InventoryIds.Player; 

    public override void _Ready()
    {
        switch (CurInventory)
        {
            case "player_inventory":
                Bind(MainManager.Instance.Player.Inventory, CurInventory);
                break;
            case "extra_inventory":
                break;
        }
        
    }

    public void Initialize()
    {
        GD.Print(GetGroups());
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


    /// <summary>
    /// Позваляет забиндить текущий инвентарь своим
    /// </summary>
    /// <param name="inventory"></param>
    /// <param name="inv"></param>
    public void Bind(Inventory inventory, string inv = InventoryIds.Extra)
    {
        Unbind();
        Inventory = inventory;
        Inventory.OnChanged += Refresh;
        CurInventory = inv;

        if (_slots.Count == 0)
        {
            InitializeSlotsFromChildren();
        }

        Refresh();
    }

    public void Unbind()
    {
        if (Inventory != null)
        {
            Inventory.OnChanged -= Refresh;
        }

        Inventory = null;
    }

    public void Refresh()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (i < Inventory.SlotCount)
            {
                _slots[i].SetItem(Inventory.GetSlot(i));
            }
            else
            {
                _slots[i].Clear();
            }
        }
    }

    public override void _ExitTree()
    {
        if (Inventory != null)
            Inventory.OnChanged -= Refresh;
    }

}