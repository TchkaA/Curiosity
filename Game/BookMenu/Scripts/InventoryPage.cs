using Godot;

/// <summary>
/// Inventory page. Binds grids to inventories and handles slot actions:
/// use, loot, transfer, inspect and drop.
/// </summary>
public partial class InventoryPage : Control
{
    [Export] public InventoryGrid PlayerInventoryGrid;
    [Export] public InventoryGrid ExtraInventoryGrid;
    [Export] public DropPanel DropPanel;
    [Export] public InspectPanel InspectPanel;
    [Export] public Button TakeAllButton;
    [Export] public Button StoreAllButton;

    private Player _player;
    private InventoryGrid _dropGrid;
    private InventorySlot _dropSlot;

    public override void _Ready()
    {
        PlayerInventoryGrid.SlotDropped += OnSlotDropped;
        ExtraInventoryGrid.SlotDropped += OnSlotDropped;

        PlayerInventoryGrid.SlotLeftClicked += OnSlotLeftClicked;
        ExtraInventoryGrid.SlotLeftClicked += OnSlotLeftClicked;

        PlayerInventoryGrid.SlotRightClicked += OnSlotRightClicked;
        ExtraInventoryGrid.SlotRightClicked += OnSlotRightClicked;

        PlayerInventoryGrid.SlotMiddleClicked += OnSlotMiddleClicked;
        ExtraInventoryGrid.SlotMiddleClicked += OnSlotMiddleClicked;

        if (TakeAllButton != null)
            TakeAllButton.Pressed += OnTakeAllPressed;

        if (StoreAllButton != null)
            StoreAllButton.Pressed += OnStoreAllPressed;

        if (DropPanel != null)
        {
            DropPanel.Confirmed += OnDropConfirmed;
            DropPanel.Canceled += OnDropCanceled;
        }
    }

    public override void _ExitTree()
    {
        if (PlayerInventoryGrid != null)
        {
            PlayerInventoryGrid.SlotDropped -= OnSlotDropped;
            PlayerInventoryGrid.SlotLeftClicked -= OnSlotLeftClicked;
            PlayerInventoryGrid.SlotRightClicked -= OnSlotRightClicked;
            PlayerInventoryGrid.SlotMiddleClicked -= OnSlotMiddleClicked;
        }

        if (ExtraInventoryGrid != null)
        {
            ExtraInventoryGrid.SlotDropped -= OnSlotDropped;
            ExtraInventoryGrid.SlotLeftClicked -= OnSlotLeftClicked;
            ExtraInventoryGrid.SlotRightClicked -= OnSlotRightClicked;
            ExtraInventoryGrid.SlotMiddleClicked -= OnSlotMiddleClicked;
        }

        if (TakeAllButton != null)
            TakeAllButton.Pressed -= OnTakeAllPressed;

        if (StoreAllButton != null)
            StoreAllButton.Pressed -= OnStoreAllPressed;

        if (DropPanel != null)
        {
            DropPanel.Confirmed -= OnDropConfirmed;
            DropPanel.Canceled -= OnDropCanceled;
        }
    }

    public void Bind(Inventory playerInv, Inventory extraInv = null, Player player = null)
    {
        if (player != null)
            _player = player;

        PlayerInventoryGrid.Bind(playerInv);

        bool hasExtra = extraInv != null;

        if (hasExtra)
        {
            ExtraInventoryGrid.Bind(extraInv);
            ExtraInventoryGrid.Visible = true;
        }
        else
        {
            ExtraInventoryGrid.Unbind();
            ExtraInventoryGrid.Visible = false;
        }

        if (TakeAllButton != null)
            TakeAllButton.Visible = hasExtra;

        if (StoreAllButton != null)
            StoreAllButton.Visible = hasExtra;
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

        if (sourceGrid == targetGrid)
        {
            targetGrid.BoundInventory.SwapSlots(source.SlotIndex, target.SlotIndex);
            return;
        }

        sourceGrid.BoundInventory.MoveTo(targetGrid.BoundInventory, source.SlotIndex);
    }

    private void OnSlotLeftClicked(InventoryGrid grid, InventorySlot slot)
    {
        if (slot?.CurrentData?.Item == null)
            return;

        if (InspectPanel == null)
        {
            GD.PushError("[InventoryPage] InspectPanel is not assigned");
            return;
        }

        InspectPanel.ShowItem(slot.CurrentData.Item);
    }

    private void OnSlotRightClicked(InventoryGrid grid, InventorySlot slot, bool shift)
    {
        if (slot?.CurrentData?.Item == null)
            return;

        if (shift)
        {
            if (grid == PlayerInventoryGrid)
                MoveSlot(PlayerInventoryGrid, ExtraInventoryGrid, slot);
            else if (grid == ExtraInventoryGrid)
                MoveSlot(ExtraInventoryGrid, PlayerInventoryGrid, slot);

            return;
        }

        if (grid == ExtraInventoryGrid)
        {
            MoveSlot(ExtraInventoryGrid, PlayerInventoryGrid, slot);
            return;
        }

        if (grid == PlayerInventoryGrid && PlayerInventoryGrid.BoundInventory?._owner is Player)
        {
            PlayerInventoryGrid.BoundInventory.UseItem(slot.SlotIndex);
        }
    }

    private void MoveSlot(InventoryGrid source, InventoryGrid target, InventorySlot slot)
    {
        var sourceInventory = source?.BoundInventory;
        var targetInventory = target?.BoundInventory;

        if (sourceInventory == null || targetInventory == null)
            return;

        sourceInventory.MoveTo(targetInventory, slot.SlotIndex);
    }

    private void OnTakeAllPressed()
    {
        var extra = ExtraInventoryGrid.BoundInventory;
        var player = PlayerInventoryGrid.BoundInventory;

        if (extra == null || player == null)
            return;

        extra.MoveAllTo(player);
    }

    private void OnStoreAllPressed()
    {
        var extra = ExtraInventoryGrid.BoundInventory;
        var player = PlayerInventoryGrid.BoundInventory;

        if (extra == null || player == null)
            return;

        player.MoveAllTo(extra);
    }

    private void OnSlotMiddleClicked(InventoryGrid grid, InventorySlot slot)
    {
        if (slot?.CurrentData?.Item == null || grid?.BoundInventory == null)
            return;

        if (DropPanel == null)
        {
            GD.PushError("[InventoryPage] DropPanel is not assigned");
            return;
        }

        _dropGrid = grid;
        _dropSlot = slot;
        DropPanel.Ask(null, slot.CurrentData.Count);
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
            GD.PushError("[InventoryPage] Cannot drop item: player is not bound");
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

        if (DropPanel != null)
            DropPanel.HidePanel();
    }
}