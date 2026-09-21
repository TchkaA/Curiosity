using System;
using Godot;

public partial class BookMenu : CanvasLayer
{
    [Export] public InventoryPage InventoryPage;
    public OpenState CurrentState;

    public Action<OpenState> IsMenuOpen;

    // Мы больше не храним Player внутри меню. Мы принимаем инвентарь как данные.
    
    public override void _Ready()
    {
        
        CurrentState = OpenState.Closed;
        // Меню изначально скрыто
        Visible = false;
        ProcessMode = Node.ProcessModeEnum.Always;
    }

    /// <summary>
    /// Открывает меню, ставит игру на паузу и биндит инвентари.
    /// </img>
    public void Open(Inventory playerInventory, Inventory extraInventory = null)
    {
        Visible = true;
        GetTree().Paused = true; // Ставим на паузу при открытии
        CurrentState = OpenState.Opened;
        InventoryPage?.Bind(playerInventory, extraInventory);
    }

    /// <summary>
    /// Закрывает меню, снимает паузу и отвязывает инвентари (чтобы не было утечек памяти).
    /// </summary>
    public void Close()
    {
        Visible = false;
        GetTree().Paused = false; // Снимаем с паузы

        CurrentState = OpenState.Closed;
        InventoryPage?.Unbind();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Tab"))
        {
            IsMenuOpen?.Invoke(CurrentState);
        }
    }
}


public enum OpenState
{
    Opened,
    Closed
}