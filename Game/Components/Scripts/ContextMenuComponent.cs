using System.Collections.Generic;
using Godot;

public class ContextMenuComponent
{
    public static ContextMenu activeContextMenu;
    private Node2D _owner;
    private List<ContextAction> _contextActions = new();

	private static readonly Vector2 _defaultSize = new(1, 1);
	public ContextMenuComponent(Node2D owner, IEnumerable<ContextAction> contextActions)
    {
        _owner = owner;
		_contextActions.AddRange(contextActions);
    }


	public void ShowContextMenu(Vector2? size = null)
	{
		CloseContextMenu();

		var menu = new ContextMenu { Name = "ContextMenu" };
		_owner.AddChild(menu);
		menu.Initialize(_contextActions);
		menu.Scale = size ?? _defaultSize;
        menu.ZIndex = 15;
        activeContextMenu = menu;
	}

    public void CloseContextMenu()
	{
		if (GodotObject.IsInstanceValid(activeContextMenu))
			activeContextMenu.QueueFree();
		activeContextMenu = null;
	}

    
}