using Godot;
using System;
using System.Collections.Generic;

public partial class PickUp : InteractableObject, IContextMenuProvider
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

    public IEnumerable<ContextAction> GetContextAction(Node2D interactor)
    {
        yield return new ContextAction(
			"Подобрать", () => Interact(this)
		);
		yield return new ContextAction(
			"Осмотреть", Inspect
		);
	}


	public void Inspect()
	{
		GD.Print($"Название - {item.Name}\nОписание - {item.Description}");
	}
}
