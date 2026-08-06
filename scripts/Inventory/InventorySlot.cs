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
        
        // Добавляем пункты меню
        var actions = new List<ContextAction>(GetContextAction(_player));
        foreach (var action in actions)
        {
            popup.AddItem(action.Name);
        }
        
        // Создаем локальный обработчик
        void OnIdPressed(long id)
        {
            if (id >= 0 && id < actions.Count)
            {
                actions[(int)id].Callback.Invoke();
            }
            // Отписываемся после использования, чтобы не накапливать
            popup.IdPressed -= OnIdPressed;
        }
        
        popup.IdPressed += OnIdPressed;
    }

    // Отдельный метод для обработки нажатий в меню
    private void OnPopupIdPressed(long id)
    {
        var actions = new List<ContextAction>(GetContextAction(_player));
        if (id >= 0 && id < actions.Count)
        {
            actions[(int)id].Callback.Invoke();
        }
    }

    public IEnumerable<ContextAction> GetContextAction(Node2D interactor)
    {   
		yield return new ContextAction(
			"Использовать", () => Interact(interactor)
		);
		yield return new ContextAction(
			"Осмотреть", Inspect
		);
        yield return new ContextAction(
			"Бросить", () => drop(interactor)
		);
	}

    private void drop(Node2D interactor)
    {
        if (CurrentItem != null)
        {
            var pickUp = new PickUp();
            pickUp.Init(CurrentItem.Item);
            
            // Проверяем, что CurrentScene не null
            var currentScene = GetTree().CurrentScene;
            if (currentScene != null)
            {
                currentScene.AddChild(pickUp);
                pickUp.GlobalPosition = interactor.GlobalPosition;
                
                // Очищаем слот
                CurrentItem = null;
                Clear(); // Обновляем UI
            }
            else
            {
                GD.PrintErr("CurrentScene is null!");
            }
        }
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