using Godot;
using System;
using System.Collections.Generic;

public partial class PickUp : RigidBody2D, IContextMenuProvider, IInteractable
{
	[Export]
	public Item item;
	public Sprite2D Visual;
	public Shader shader;
	public ShaderMaterial material;
	private PopupMenu _popup;
	public bool isinRange = false;
	private static ContextMenu _previousContextMenu;
	public override void _Ready()
	{
		shader = GD.Load<Shader>("res://shaders/outline/outline.gdshader");
		InitShader();
		Connect("input_event", Callable.From<Node, InputEvent, long>(OnInputEvent));
		Visual.Texture = item.Icon;
        Name = item.Name;
	}

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed && mouseButtonEvent.ButtonIndex == MouseButton.Right)
        {
			if(isinRange)
			{
				FreeContextMenu();
				var menu = new ContextMenu();
				menu.Name = "ContextMenu";
				AddChild(menu);
				menu.Initialize(GetContextAction(MainManager.Instance.Player));
				_previousContextMenu = menu;
			}
		}
	}
    public void Interact(Node2D interactor)
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
			"Подобрать", () => Interact(interactor)
		);
		yield return new ContextAction(
			"Осмотреть", Inspect
		);
	}

	public void Inspect()
	{
		GD.Print($"Название - {item.Name}\nОписание - {item.Description}");
	}


    public virtual void InteractEnter()
    {
        if (material == null)
            return;

		isinRange = true;
        material.SetShaderParameter("outline_size", 0.8f);
    }

    public virtual void InteractExit()
	{
		if (material == null)
			return;
		
			
		isinRange = false;
		material.SetShaderParameter("outline_size", 0f);
	}


	private void InitShader()
	{
		Visual = GetNode<Sprite2D>("Sprite2D");

		material = new ShaderMaterial();
        material.Shader = shader;
		material.SetShaderParameter("outline_size", 0f);
        Visual.Material = material;
	}


	public void FreeContextMenu()
	{
		if (_previousContextMenu != null)
		{
			_previousContextMenu.QueueFree();
			_previousContextMenu = null;
		}
	}

}
