using System;
using System.Collections.Generic;
using Godot;

public partial class InventorySlot : Control, IContextMenuProvider
{
    public InventorySlotData CurrentItem { get; set; }
    public int Count { get; set; }
    [Export]
    public TextureButton Icon;
    [Export]
    public Label LabelCount;
    private Player _player => MainManager.Instance.Player;
    [Export]
    public MenuButton menuButton;
    public override void _Ready()
    {
        if (Icon != null)
        {
            Icon.MouseFilter = MouseFilterEnum.Ignore;
        }
        GuiInput += OnGuiInput;
        menuButton.Pressed += OnMenuButtonPressed;
    }


    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed)
        {
            switch (mouseButtonEvent.ButtonIndex)
            {
                case MouseButton.Left:
                    Interact(_player);
                    break;
            }
        }
    }
    public void SetItem(InventorySlotData item)
    {
        if (item == null || item.Item == null || item.Item.Icon == null || item.Count == 0)
        {
            Clear();
            return;
        }

        if (Icon != null)
        {
            Icon.TextureNormal = item.Item.Icon;
        }

        CurrentItem = item;
        Count = item.Count;
        LabelCount.Text = Count.ToString();
    }

    public void Clear()
    {
        CurrentItem = null;
        Count = 0;
        LabelCount.Text = null;

        if (Icon != null)
        {
            Icon.TextureNormal = null;
        }
    }

    public void UseItem()
    {
        if(CurrentItem == null) return; // Maybe later add notification
        _player.Inventory.Use(CurrentItem.Item);
        SetItem(CurrentItem);
    }


    private void OnMenuButtonPressed()
    {
        var popup = menuButton.GetPopup();

        popup.Clear();
        foreach (var action in GetContextAction(_player))
        {
            var itemIndex = popup.ItemCount;
            popup.AddItem(action.Name);
            popup.IdPressed += id =>
            {
                if (id == itemIndex)
                {
                    action.Callback.Invoke();
                }
            };
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

    private void Interact(Node2D interactor)
    {
        GD.Print($"Interacting with {CurrentItem.Item.Name} by {interactor.Name}");
    }


    private void Inspect()
    {
        GD.Print($"Название - {CurrentItem.Item.Name}\nОписание - {CurrentItem.Item.Description}");
    }

}