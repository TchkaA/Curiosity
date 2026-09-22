using System;
using Godot;

public class Inventory
{
    private readonly InventorySlotData?[] _slots;
    private readonly Node2D _owner;
    public int Capacity => _slots.Length;

    // Событие для обновления конкретного слота
    public event Action<int> OnSlotChanged;

    public Inventory(Node2D owner, int capacity = 20)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _slots = new InventorySlotData?[capacity];
    }

    public InventorySlotData? GetSlot(int index)
    {
        if (index < 0 || index >= Capacity) return null;
        return _slots[index];
    }

    public void AddItem(Item item, int count = 1)
    {
        if (item == null || count <= 0) return;

        int remaining = count;

        // 1. Пытаемся добавить в существующие стаки с таким же ID
        for (int i = 0; i < Capacity && remaining > 0; i++)
        {
            if (_slots[i] != null && _slots[i].Item.ID == item.ID && _slots[i].Count < item.MaxStack)
            {
                int freeSpace = item.MaxStack - _slots[i].Count;
                int addCount = Math.Min(remaining, freeSpace);

                _slots[i].Count += addCount;
                remaining -= addCount;
                
                OnSlotChanged?.Invoke(i); // Обновляем только этот слот в UI
            }
        }

        // 2. Если осталось, ищем первый пустой слот
        while (remaining > 0)
        {
            int emptyIndex = Array.FindIndex(_slots, slot => slot == null);
            if (emptyIndex == -1)
            {
                GD.PrintErr("[Inventory] Инвентарь полон!");
                break; // Инвентарь заполнен
            }

            int newStack = Math.Min(remaining, item.MaxStack > 0 ? item.MaxStack : remaining);
            _slots[emptyIndex] = new InventorySlotData(item, newStack);
            remaining -= newStack;
            
            OnSlotChanged?.Invoke(emptyIndex);
        }
    }

    public bool RemoveItem(Item item, int count = 1)
    {
        if (item == null || count <= 0) return true;

        int remaining = count;

        for (int i = 0; i < Capacity && remaining > 0; i++)
        {
            if (_slots[i] != null && _slots[i].Item.ID == item.ID)
            {
                int removeCount = Math.Min(remaining, _slots[i].Count);
                _slots[i].Count -= removeCount;
                remaining -= removeCount;

                int changedIndex = i;
                if (_slots[i].Count <= 0)
                {
                    _slots[i] = null; // Очищаем слот полностью
                }
                
                OnSlotChanged?.Invoke(changedIndex);
            }
        }
        return remaining == 0;
    }

    public bool UseItem(int slotIndex, Node2D user = null)
    {
        if (slotIndex < 0 || slotIndex >= Capacity || _slots[slotIndex] == null) return false;

        user ??= _owner;
        var data = _slots[slotIndex];

        if (data.Item.Use(user))
        {
            data.Count -= 1;
            if (data.Count <= 0)
            {
                _slots[slotIndex] = null;
            }
            OnSlotChanged?.Invoke(slotIndex);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Меняет местами содержимое двух слотов. Поддерживает частичное слияние стаков.
    /// </summary>
    public void SwapSlots(int sourceIndex, int targetIndex)
    {
        if (sourceIndex < 0 || sourceIndex >= Capacity) return;
        if (targetIndex < 0 || targetIndex >= Capacity) return;
        if (sourceIndex == targetIndex) return;

        var sourceData = _slots[sourceIndex];
        var targetData = _slots[targetIndex];

        // Если целевой слот пустой, просто меняем местами
        if (targetData == null)
        {
            _slots[targetIndex] = sourceData;
            _slots[sourceIndex] = null;
        }
        // Если предметы одинаковые и в целевом есть место, пытаемся слить
        else if (sourceData != null && sourceData.Item.ID == targetData.Item.ID && targetData.Count < targetData.Item.MaxStack)
        {
            int spaceInTarget = targetData.Item.MaxStack - targetData.Count;
            int amountToMove = Math.Min(sourceData.Count, spaceInTarget);

            targetData.Count += amountToMove;
            sourceData.Count -= amountToMove;

            if (sourceData.Count <= 0)
            {
                _slots[sourceIndex] = null;
            }
        }
        // Если предметы разные или стак полный, просто меняем их местами
        else
        {
            _slots[targetIndex] = sourceData;
            _slots[sourceIndex] = targetData;
        }

        OnSlotChanged?.Invoke(sourceIndex);
        OnSlotChanged?.Invoke(targetIndex);
    }
}