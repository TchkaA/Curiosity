using System;
using System.Collections.Generic;
using Godot;

public partial class InventoryGrid : GridContainer
{
    private Inventory _boundInventory;
    private readonly List<InventorySlot> _slots = new();

    public Inventory BoundInventory => _boundInventory;

    public event Action<InventorySlot, InventorySlot> SlotDropped;

    public event Action<InventoryGrid, InventorySlot> SlotLeftClicked;

    public event Action<InventoryGrid, InventorySlot, bool> SlotRightClicked;

    public event Action<InventoryGrid, InventorySlot> SlotMiddleClicked;

    public override void _Ready()
    {
        int index = 0;
        foreach (var child in GetChildren())
        {
            if (child is InventorySlot slot)
            {
                slot.SlotIndex = index++;
                _slots.Add(slot);

                slot.LeftClicked += OnSlotLeftClicked;
                slot.RightClicked += OnSlotRightClicked;
                slot.MiddleClicked += OnSlotMiddleClicked;
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

    private void OnSlotLeftClicked(InventorySlot slot)
    {
        SlotLeftClicked?.Invoke(this, slot);
    }

    private void OnSlotRightClicked(InventorySlot slot, bool shift)
    {
        if (SlotRightClicked != null)
        {
            SlotRightClicked.Invoke(this, slot, shift);
            return;
        }

        if (_boundInventory?._owner is Player)
        {
            _boundInventory.UseItem(slot.SlotIndex);
        }
    }

    private void OnSlotMiddleClicked(InventorySlot slot)
    {
        SlotMiddleClicked?.Invoke(this, slot);
    }

    private void OnSlotItemDropped(InventorySlot sourceSlot, InventorySlot targetSlot)
    {
        if (SlotDropped != null)
        {
            SlotDropped.Invoke(sourceSlot, targetSlot);
            return;
        }

        _boundInventory?.SwapSlots(sourceSlot.SlotIndex, targetSlot.SlotIndex);
    }

    public override void _ExitTree()
    {
        Unbind();
        foreach (var slot in _slots)
        {
            slot.LeftClicked -= OnSlotLeftClicked;
            slot.RightClicked -= OnSlotRightClicked;
            slot.MiddleClicked -= OnSlotMiddleClicked;
            slot.ItemDropped -= OnSlotItemDropped;
        }
    }
}