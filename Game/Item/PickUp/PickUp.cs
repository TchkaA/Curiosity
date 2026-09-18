using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Предмет на земле: можно осмотреть или подобрать.
/// </summary>
[GlobalClass]
public partial class PickUp : Area2D, IInteractable
{
	public Item Item;

	[ExportGroup("Настройки")]
	[Export] public float VisualScale = 3f;
	[Export] public float CollisionRadius = 8f;
	[Export] public float OutlineActiveSize = 0.8f;

	private Sprite2D _visual;
	private ShaderMaterial _material;
	private bool _isInRange;
    

    public int Count;

	/// <summary>
	/// Задать предмет до того, как нода попадёт в дерево сцены (например, при спавне из кода).
	/// Вся остальная настройка (визуал, коллизия, шейдер) происходит один раз в _Ready.
	/// </summary>
    public PickUp(Item item)
    {
        Item = item;
    }

	public override void _Ready()
	{
		if (Item == null)
		{
			GD.PushError($"{Name}: PickUp создан без Item, настройка отменена.");
			
		}

		ZIndex = -1;

		Name = Item.Name;

		SetupVisual();
		// SetupOutline();
		SetupCollision();

		InputPickable = true;
	}

	private void SetupVisual()
	{
		_visual = new Sprite2D
		{
			Texture = Item.Icon,
			Scale = Vector2.One * VisualScale
		};
		AddChild(_visual);
	}

	// private void SetupOutline()
	// {
	// 	// var shader = MainManager.Instance.OutlineShader;
	// 	_material = new ShaderMaterial { Shader = shader };
	// 	_material.SetShaderParameter("outline_size", 0f);
	// 	_visual.Material = _material;
	// }

	private void SetupCollision()
	{
		var collision = new CollisionShape2D
		{
			Shape = new CircleShape2D { Radius = CollisionRadius }
		};
		AddChild(collision);
	}

	// private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	// {
	// 	if (_isInRange && @event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Right })
	// 	{
	// 		contextMenu.ShowContextMenu();
	// 		// Важно: без этого тот же клик долетит до _UnhandledInput и сразу закроет
	// 		// меню, которое мы только что открыли.
	// 		GetViewport().SetInputAsHandled();
	// 	}
	// }

	// private void ShowContextMenu()
	// {
	// 	CloseContextMenu();

	// 	var menu = new ContextMenu { Name = "ContextMenu" };
	// 	AddChild(menu);
	// 	menu.Initialize(GetContextAction(MainManager.Instance.Player));
	// 	_activeContextMenu = menu;
	// }

	// public void CloseContextMenu()
	// {
	// 	if (IsInstanceValid(_activeContextMenu))
	// 		_activeContextMenu.QueueFree();
	// 	_activeContextMenu = null;
	// }


	public void Interact(BaseEntity interactor)
	{
        #if DEBUG
        GD.Print("Interact with item" + GetType);
		QueueFree();
        #endif
		// if (interactor is IInventoryOwner owner)
		// {
		// 	owner.Inventory.AddItem(Item);
		// 	GD.Print($"Picked up: {Item.Name} -> {interactor.Name}");
		// 	QueueFree();
		// }
		// else
		// {
		// 	GD.PushWarning($"Cannot add item {Item.Name}: у {interactor.Name} нет инвентаря");
		// }
        
	}

	public void Inspect()
	{
		GD.Print($"Название - {Item.Name}\nОписание - {Item.Description}");
	}


	public virtual void InteractEnter()
	{
		_isInRange = true;
		_material?.SetShaderParameter("outline_size", OutlineActiveSize);
	}

	public virtual void InteractExit()
	{
		_isInRange = false;
		_material?.SetShaderParameter("outline_size", 0f);
	}
}
