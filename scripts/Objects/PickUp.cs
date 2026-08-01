using Godot;
using System;

public partial class PickUp : InteractableObject
{
	[Export]
	public Item item;

	public override void _Ready()
	{
		base._Ready();
		Visual.Texture = item.Icon;
		Name = item.Name;
	}

    public override void Interact(Node2D interactor)
    {
		if (interactor is IInventoryOwner inventoryOwner)
		{
			inventoryOwner.Inventory.AddItem(item);
			GD.Print($"Picked up: {item.Name} -> {inventoryOwner}");
			QueueFree();
		}
		else
		{
			GD.Print($"Cannot add item {item.Name}: interactor has no inventory");
		}
    }
}
