using Godot;
using System;
using System.Collections.Generic;

public class Inventory
{
    private readonly List<InventorySlotData> _slots = new();

    public int SlotCount => _slots.Count;
    public IReadOnlyList<InventorySlotData> Slots => _slots;
    private Node2D _owner;

    public Action OnChanged;

    public Inventory(Node2D owner)
    {
        _owner = owner;
    }

    public InventorySlotData GetSlot(int index)
    {
        if (index < 0 || index >= _slots.Count)
        {
            return default;
        }

        return _slots[index];
    }

    public void AddItem(Item item, int count = 1)
    {
        if (item is null)
        {
            GD.Print("Ошибочка инвентарь");
        }

        if (count <= 0)
        {
            return;
        }

        int remaining = count;

        for (int i = 0; i < _slots.Count && remaining > 0; i++)
        {
            var slot = _slots[i];

            if (slot.Item == item && slot.Count < item.MaxStack)
            {
                int freeSpace = item.MaxStack - slot.Count;
                int addCount = Math.Min(remaining, freeSpace);

                slot.Count += addCount;
                remaining -= addCount;
            }
        }

        while (remaining > 0)
        {
            int newStack = Math.Min(remaining, item.MaxStack > 0 ? item.MaxStack : remaining);
            _slots.Add(new InventorySlotData(item, newStack));
            remaining -= newStack;
        }
        
        OnChanged?.Invoke();
    }

    public bool RemoveItem(Item item, int count = 1)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (count <= 0)
        {
            return true;
        }

        int remaining = count;

        for (int i = _slots.Count - 1; i >= 0 && remaining > 0; i--)
        {
            var slot = _slots[i];

            if (slot.Item != item)
            {
                continue;
            }

            int removeCount = Math.Min(remaining, slot.Count);
            slot.Count -= removeCount;
            remaining -= removeCount;

            if (slot.Count <= 0)
            {
                _slots.RemoveAt(i);
            }
        }
        OnChanged?.Invoke();
        return remaining == 0;
    }

    public bool ContainsItem(Item item, int count = 1)
    {
        if (item is null)
        {
            return false;
        }

        if (count <= 0)
        {
            return true;
        }

        int total = 0;

        foreach (var slot in _slots)
        {
            if (slot.Item == item)
            {
                total += slot.Count;
            }
        }

        return total >= count;
    }

    public int GetTotalCount(Item item)
    {
        if (item is null)
        {
            return 0;
        }

        int total = 0;

        foreach (var slot in _slots)
        {
            if (slot.Item == item)
            {
                total += slot.Count;
            }
        }

        return total;
    }

    public void Use(Item item, Node2D iterator = null)
    {
        iterator ??= _owner;
        item.Use(iterator);
        RemoveItem(item);
        OnChanged?.Invoke();
    }
}

