using Godot;

/// <summary>
/// Страница инвентаря. Связывает сетки с инвентарями и обрабатывает
/// действия со слотами: использование, лут и выброс предметов.
/// </summary>
public partial class InventoryPage : Control
{
    [Export] public InventoryGrid PlayerInventoryGrid;
    [Export] public InventoryGrid ExtraInventoryGrid;
    [Export] public ActionMessage ActionMessage;

    private Player _player;
    private InventoryGrid _dropGrid;
    private InventorySlot _dropSlot;

    public override void _Ready()
    {
        PlayerInventoryGrid.SlotDropped += OnSlotDropped;
        ExtraInventoryGrid.SlotDropped += OnSlotDropped;

        PlayerInventoryGrid.SlotRightClicked += OnSlotRightClicked;
        ExtraInventoryGrid.SlotRightClicked += OnSlotRightClicked;

        PlayerInventoryGrid.SlotMiddleClicked += OnSlotMiddleClicked;
        ExtraInventoryGrid.SlotMiddleClicked += OnSlotMiddleClicked;

        if (ActionMessage != null)
        {
            ActionMessage.Confirmed += OnDropConfirmed;
            ActionMessage.Canceled += OnDropCanceled;
        }
    }

    public override void _ExitTree()
    {
        if (PlayerInventoryGrid != null)
        {
            PlayerInventoryGrid.SlotDropped -= OnSlotDropped;
            PlayerInventoryGrid.SlotRightClicked -= OnSlotRightClicked;
            PlayerInventoryGrid.SlotMiddleClicked -= OnSlotMiddleClicked;
        }

        if (ExtraInventoryGrid != null)
        {
            ExtraInventoryGrid.SlotDropped -= OnSlotDropped;
            ExtraInventoryGrid.SlotRightClicked -= OnSlotRightClicked;
            ExtraInventoryGrid.SlotMiddleClicked -= OnSlotMiddleClicked;
        }

        if (ActionMessage != null)
        {
            ActionMessage.Confirmed -= OnDropConfirmed;
            ActionMessage.Canceled -= OnDropCanceled;
        }
    }

    /// <summary>
    /// Привязывает инвентари к сеткам.
    /// </summary>
    /// <param name="playerInv">Инвентарь игрока.</param>
    /// <param name="extraInv">Второй инвентарь для лута, если он открыт.</param>
    /// <param name="player">Игрок, рядом с которым появляются выброшенные предметы.</param>
    public void Bind(Inventory playerInv, Inventory extraInv = null, Player player = null)
    {
        if (player != null)
            _player = player;

        PlayerInventoryGrid.Bind(playerInv);
        
        if (extraInv != null)
        {
            ExtraInventoryGrid.Bind(extraInv);
            ExtraInventoryGrid.Visible = true;
        }
        else
        {
            ExtraInventoryGrid.Unbind();
            ExtraInventoryGrid.Visible = false; // Скрываем, если второго инвентаря нет
        }
    }

    public void Unbind()
    {
        CancelPendingDrop();
        PlayerInventoryGrid.Unbind();
        ExtraInventoryGrid.Unbind();
    }

    private void OnSlotDropped(InventorySlot source, InventorySlot target)
    {
        var sourceGrid = source.GetParent() as InventoryGrid;
        var targetGrid = target.GetParent() as InventoryGrid;

        if (sourceGrid?.BoundInventory == null || targetGrid?.BoundInventory == null)
            return;

        // Один инвентарь — обычный обмен слотов.
        if (sourceGrid == targetGrid)
        {
            targetGrid.BoundInventory.SwapSlots(source.SlotIndex, target.SlotIndex);
            return;
        }

        // Разные инвентари — перенос предмета (лут).
        sourceGrid.BoundInventory.MoveTo(targetGrid.BoundInventory, source.SlotIndex);
    }

    /// <summary>
    /// ПКМ по инвентарю игрока использует предмет, ПКМ по инвентарю лута
    /// переносит предмет игроку.
    /// </summary>
    private void OnSlotRightClicked(InventoryGrid grid, InventorySlot slot)
    {
        if (slot?.CurrentData?.Item == null)
            return;

        if (grid == ExtraInventoryGrid)
        {
            var extra = ExtraInventoryGrid.BoundInventory;
            var playerInventory = PlayerInventoryGrid.BoundInventory;

            if (extra == null || playerInventory == null)
                return;

            extra.MoveTo(playerInventory, slot.SlotIndex);
            return;
        }

        if (grid == PlayerInventoryGrid && PlayerInventoryGrid.BoundInventory?._owner is Player)
        {
            PlayerInventoryGrid.BoundInventory.UseItem(slot.SlotIndex);
        }
    }

    /// <summary>
    /// СКМ по слоту открывает окно выброса с выбором количества.
    /// </summary>
    private void OnSlotMiddleClicked(InventoryGrid grid, InventorySlot slot)
    {
        if (slot?.CurrentData?.Item == null || grid?.BoundInventory == null)
            return;

        if (ActionMessage == null)
        {
            GD.PushError("[InventoryPage] Не назначено окно ActionMessage");
            return;
        }

        _dropGrid = grid;
        _dropSlot = slot;
        ActionMessage.Ask("Выбросить", slot.CurrentData.Count);
    }

    private void OnDropConfirmed(int count)
    {
        if (_dropGrid?.BoundInventory == null || !GodotObject.IsInstanceValid(_dropSlot))
        {
            CancelPendingDrop();
            return;
        }

        var data = _dropSlot.CurrentData;
        if (data?.Item == null)
        {
            CancelPendingDrop();
            return;
        }

        var item = data.Item;
        int removed = _dropGrid.BoundInventory.RemoveFromSlot(_dropSlot.SlotIndex, count);

        if (removed > 0)
            SpawnPickUp(item, removed);

        CancelPendingDrop();
    }

    private void OnDropCanceled()
    {
        CancelPendingDrop();
    }

    private void SpawnPickUp(Item item, int count)
    {
        if (item == null || count <= 0)
            return;

        if (_player == null || _player.GetParent() == null)
        {
            GD.PushError("[InventoryPage] Не удалось выбросить предмет: игрок не привязан");
            return;
        }

        var pickup = new PickUp();
        pickup.Init(item, count);

        _player.GetParent().AddChild(pickup);
        pickup.GlobalPosition = _player.GlobalPosition + new Vector2(24f, 0f);
    }

    private void CancelPendingDrop()
    {
        _dropGrid = null;
        _dropSlot = null;

        if (ActionMessage != null)
            ActionMessage.Hide();
    }
}
