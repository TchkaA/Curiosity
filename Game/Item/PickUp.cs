using Godot;
using System.Collections.Generic;

/// <summary>
/// Предмет на земле: можно осмотреть или подобрать через контекстное меню.
/// Area2D — специально НЕ RigidBody2D, чтобы игрок физически проходил сквозь предмет,
/// но при этом зона (Area2D) могла ловить вход/выход игрока для взаимодействия.
/// </summary>
public partial class PickUp : Area2D, IContextMenuProvider, IInteractable
{
	[Export] public Item Item;

	[ExportGroup("Настройки")]
	[Export] public float VisualScale = 3f;
	[Export] public float CollisionRadius = 8f;
	[Export] public float OutlineActiveSize = 0.8f;

	private Sprite2D _visual;
	private ShaderMaterial _material;
	private bool _isInRange;

	private ContextMenuComponent contextMenu;

	/// <summary>
	/// Задать предмет до того, как нода попадёт в дерево сцены (например, при спавне из кода).
	/// Вся остальная настройка (визуал, коллизия, шейдер) происходит один раз в _Ready.
	/// </summary>
	public void Init(Item item) => Item = item;

	public override void _Ready()
	{
		if (Item == null)
		{
			GD.PushError($"{Name}: PickUp создан без Item, настройка отменена.");
			return;
		}

		ZIndex = -1;

		Name = Item.Name;

		SetupVisual();
		SetupOutline();
		SetupCollision();

		contextMenu = new(this,  GetContextAction(MainManager.Instance.Player));

		InputPickable = true;
		InputEvent += OnInputEvent;
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

	private void SetupOutline()
	{
		var shader = MainManager.Instance.OutlineShader;
		_material = new ShaderMaterial { Shader = shader };
		_material.SetShaderParameter("outline_size", 0f);
		_visual.Material = _material;
	}

	private void SetupCollision()
	{
		var collision = new CollisionShape2D
		{
			Shape = new CircleShape2D { Radius = CollisionRadius }
		};
		AddChild(collision);
	}

	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (_isInRange && @event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Right })
		{
			contextMenu.ShowContextMenu();
			// Важно: без этого тот же клик долетит до _UnhandledInput и сразу закроет
			// меню, которое мы только что открыли.
			GetViewport().SetInputAsHandled();
		}
	}

	/// <summary>
	/// Любой клик, который не был "съеден" GUI-элементом меню (например, клик по пустому
	/// пространству или по другому предмету), долетает сюда — закрываем открытое меню.
	/// Клик по кнопкам самого ContextMenu сюда не дойдёт, если ContextMenu — это Control
	/// (стандартные Button/PopupMenu сами останавливают распространение события).
	/// </summary>
	public override void _UnhandledInput(InputEvent @event)
	{
		var menu = ContextMenuComponent.activeContextMenu;
		if (IsInstanceValid(menu) && @event is InputEventMouseButton { Pressed: true })
		{
			contextMenu.CloseContextMenu();
		}
	}

	public void Interact(BaseEntity interactor)
	{
		if (interactor is IInventoryOwner owner)
		{
			owner.Inventory.AddItem(Item);
			GD.Print($"Picked up: {Item.Name} -> {interactor.Name}");
			QueueFree();
		}
		else
		{
			GD.PushWarning($"Cannot add item {Item.Name}: у {interactor.Name} нет инвентаря");
		}
	}

	public void Inspect()
	{
		GD.Print($"Название - {Item.Name}\nОписание - {Item.Description}");
	}

	public IEnumerable<ContextAction> GetContextAction(BaseEntity interactor)
	{
		yield return new ContextAction("Подобрать", () => Interact(interactor));
		yield return new ContextAction("Осмотреть", Inspect);
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
		contextMenu.CloseContextMenu();
	}
}
