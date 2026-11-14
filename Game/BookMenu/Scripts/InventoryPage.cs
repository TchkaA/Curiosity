using Godot;

public partial class InventoryPage : Control
{
    [Export] public InventoryGrid PlayerInventoryGrid;
    [Export] public InventoryGrid ExtraInventoryGrid;
    public override void _Ready()
    {
        PlayerInventoryGrid.SlotDropped += OnSlotDropped;
        ExtraInventoryGrid.SlotDropped += OnSlotDropped;
    }

    public override void _ExitTree()
    {
        if (PlayerInventoryGrid != null)
            PlayerInventoryGrid.SlotDropped -= OnSlotDropped;

        if (ExtraInventoryGrid != null)
            ExtraInventoryGrid.SlotDropped -= OnSlotDropped;
    }



    public void Bind(Inventory playerInv, Inventory extraInv = null)
    {
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

}
