using System.Collections.Generic;
using Godot;

/// <summary>
/// Контейнер, который отображает конкретный экземпляр Inventory в виде сетки слотов.
/// </summary>
public partial class InventoryGrid : GridContainer
{
    private Inventory _boundInventory;
    private readonly List<InventorySlot> _slots = new();

    public override void _Ready()
    {
        // Собираем все слоты, которые были расставлены в редакторе (дочерние ноды)
        foreach (var child in GetChildren())
        {
            if (child is InventorySlot slot)
            {
                _slots.Add(slot);
                
                // Подписываемся на сигналы слота, чтобы знать о кликах
                slot.LeftClicked += OnSlotLeftClicked;
                slot.RightClicked += OnSlotRightClicked;
            }
        }
    }

    /// <summary>
    /// Привязывает UI к конкретному инвентарю (игрока, сундука и т.д.)
    /// </summary>
    public void Bind(Inventory inventory)
    {
        Unbind();
        _boundInventory = inventory;
        
        if (_boundInventory != null)
        {
            _boundInventory.OnChanged += RefreshUI;
            RefreshUI();
        }
    }

    public void Unbind()
    {
        if (_boundInventory != null)
        {
            _boundInventory.OnChanged -= RefreshUI;
            _boundInventory = null;
        }
        
        // Очищаем визуал
        foreach (var slot in _slots)
        {
            slot.Clear();
        }
    }

    private void RefreshUI()
    {
        if (_boundInventory == null) return;

        for (int i = 0; i < _slots.Count; i++)
        {
            var data = _boundInventory.GetSlot(i);
            _slots[i].SetData(data);
        }
    }

    // ==========================================
    // Обработчики событий от слотов
    // ==========================================

    private void OnSlotLeftClicked(InventorySlot slot)
    {
        // TODO: Логика левого клика. 
        // Если это начало Drag-and-Drop, Godot обработает это через _GetDragData.
        // Если это быстрый клик (без перетаскивания), можно сделать UseItem:
        if (slot.CurrentData != null)
        {
            _boundInventory.UseItem(slot.CurrentData.Item);
        }
    }

    private void OnSlotRightClicked(InventorySlot slot)
    {
        // TODO: Здесь вызываем твое контекстное меню.
        // Пример: ContextMenuManager.ShowAt(slot.GlobalPosition, slot.CurrentData);
        GD.Print($"Right click on slot with: {slot.CurrentData?.Item?.Name}");
    }

    public override void _ExitTree()
    {
        Unbind();
        // Отписка от сигналов слотов для предотвращения утечек памяти
        foreach (var slot in _slots)
        {
            slot.LeftClicked -= OnSlotLeftClicked;
            slot.RightClicked -= OnSlotRightClicked;
        }
    }
}