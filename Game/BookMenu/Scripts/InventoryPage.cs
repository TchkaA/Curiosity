using Godot;

public partial class InventoryPage : Control
{
    [Export] public InventoryGrid PlayerInventoryGrid;
    [Export] public InventoryGrid ExtraInventoryGrid;

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
            ExtraInventoryGrid.Visible = false; // Скрываем, если второго инвентаря нет
        }
    }

    public void Unbind()
    {
        PlayerInventoryGrid.Unbind();
        ExtraInventoryGrid.Unbind();
    }
}