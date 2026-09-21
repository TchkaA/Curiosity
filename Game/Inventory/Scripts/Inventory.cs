using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Логическое хранилище предметов. Не знает ничего об UI.
/// </summary>
public class Inventory
{
    private readonly List<InventorySlotData> _slots = new();
    private readonly Node2D _owner;

    /// <summary>
    /// Событие, вызываемое при любом изменении содержимого инвентаря.
    /// UI должен подписываться на это событие для обновления.
    /// </summary2>
    public event Action OnChanged;

    public int SlotCount => _slots.Count;
    public IReadOnlyList<InventorySlotData> Slots => _slots;

    public Inventory(Node2D owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }

    public InventorySlotData GetSlot(int index)
    {
        if (index < 0 || index >= _slots.Count) return null;
        return _slots[index];
    }

    /// <summary>
    /// Добавляет предмет в инвентарь, учитывая стаки (MaxStack).
    /// </summary>
    public void AddItem(Item item, int count = 1)
    {
        if (item == null || count <= 0) return;

        int remaining = count;

        // 1. Пытаемся добавить в существующие стаки
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

        // 2. Если осталось, создаем новые слоты
        while (remaining > 0)
        {
            int newStack = Math.Min(remaining, item.MaxStack > 0 ? item.MaxStack : remaining);
            _slots.Add(new InventorySlotData(item, newStack));
            remaining -= newStack;
        }
        
        OnChanged?.Invoke();
    }

    /// <summary>
    /// Удаляет предмет из инвентаря. Возвращает true, если удалось удалить всё запрошенное количество.
    /// </summary>
    public bool RemoveItem(Item item, int count = 1)
    {
        if (item == null || count <= 0) return true;

        int remaining = count;

        // Идем с конца, чтобы безопасно удалять элементы из списка
        for (int i = _slots.Count - 1; i >= 0 && remaining > 0; i--)
        {
            var slot = _slots[i];
            if (slot.Item != item) continue;

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

    /// <summary>
    /// Использует предмет. 
    /// TODO: В будущем здесь можно добавить проверку "можно ли использовать предмет в текущем состоянии".
    /// </summary>
    public bool UseItem(Item item, Node2D user = null)
    {
        user ??= _owner;
        
        // Сначала пытаемся использовать. Если метод вернул false (например, мачту нельзя использовать вне боя), мы её не удаляем.
        if (item.Use(user))
        {
            // Если предмет одноразовый или мы хотим снять 1 шт. при использовании:
            // TODO: Обсудить логику удаления. Сейчас удаляется 1 шт. Если item.Use сам решает, сколько удалить, этот код нужно менять.
            RemoveItem(item, 1); 
            return true;
        }
        
        return false;
    }
}