using System.Collections.Generic;
using Godot;

public partial class InventoryGrid : GridContainer
{
    private Inventory _boundInventory;
    private readonly List<InventorySlot> _slots = new();

    public override void _Ready()
    {
        int index = 0;
        foreach (var child in GetChildren())
        {
            if (child is InventorySlot slot)
            {
                slot.SlotIndex = index++;
                _slots.Add(slot);
                
                slot.RightClicked += OnSlotRightClicked;
                slot.ItemDropped += OnSlotItemDropped;
            }
        }
    }

    public void Bind(Inventory inventory)
    {
        Unbind();
        _boundInventory = inventory;
        
        if (_boundInventory != null)
        {
            _boundInventory.OnSlotChanged += UpdateSingleSlot;
            
            // Первоначальная отрисовка всех слотов
            for (int i = 0; i < _slots.Count; i++)
            {
                UpdateSingleSlot(i);
            }
        }
    }

    public void Unbind()
    {
        if (_boundInventory != null)
        {
            _boundInventory.OnSlotChanged -= UpdateSingleSlot;
            _boundInventory = null;
        }
        
        foreach (var slot in _slots)
        {
            slot.Clear();
        }
    }

    private void UpdateSingleSlot(int index)
    {
        if (index < 0 || index >= _slots.Count || _boundInventory == null) return;
        
        var data = _boundInventory.GetSlot(index);
        _slots[index].SetData(data);
    }

    private void OnSlotRightClicked(InventorySlot slot)
    {
        if (_boundInventory != null)
        {
            _boundInventory.UseItem(slot.SlotIndex);
        }
    }

    private void OnSlotItemDropped(InventorySlot sourceSlot, InventorySlot targetSlot)
    {
        if (_boundInventory != null)
        {
            _boundInventory.SwapSlots(sourceSlot.SlotIndex, targetSlot.SlotIndex);
        }
    }

    public override void _ExitTree()
    {
        Unbind();
        foreach (var slot in _slots)
        {
            slot.RightClicked -= OnSlotRightClicked;
            slot.ItemDropped -= OnSlotItemDropped;
        }
    }
}