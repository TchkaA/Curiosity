using Godot;
using System;

public partial class Chest : InteractableObject
{

    public Inventory Inventory;

    public override void _EnterTree()
    {
        Inventory = new Inventory(this);
        Inventory.AddItem(GD.Load<Item>("res://assets/Origin/objects/Resources/HealthPoitions/health_poition.tres"), 4);

    }


    public override void Interact(Node2D interactor)
    {
        MainManager.Instance.PauseGame();
        BookMenu.Instance.ShowPage("inventory");
        BookMenu.Instance.BindExtraInventory(Inventory);
    }

}
