using System;
using Godot;

public partial class BookMenu : CanvasLayer
{
    [Export]
    public Player _player {get;private set;}   

    [ExportGroup("Страницы")]
    [Export] public InventoryPage InventoryPage;
    [Export] public ProfilePage ProfilePage;

    [ExportGroup("Кнопки-закладки")]
    [Export] public TextureButton InventoryButton;
    [Export] public TextureButton ProfileButton;
    

    public Pages Page;

    public StateMachine StateMachine;
    public InventoryPageState InventoryState;
    public ProfilePageState ProfileState;
    public OpenState CurrentState;

    public Action<OpenState> IsMenuOpen;

     

    // public BookMenu(Player player)
    // {
    //     _player = player;   
    // }
    public override void _Ready()
    {
        StateMachine = new();
        InitStates();

        CurrentState = OpenState.Closed;
        // Меню изначально скрыто
        Visible = false;
        ProcessMode = Node.ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        StateMachine.Update(delta);
    }


    /// <summary>
    /// Открывает меню, ставит игру на паузу и биндит инвентари.
    /// </img>
    public void Open()
    {
        Visible = true;
        GetTree().Paused = true; // Ставим на паузу при открытии
        CurrentState = OpenState.Opened;
        OpenPage();
        // InventoryPage.Bind(_player.Inventory, extraInventory);
    }

    /// <summary>
    /// Закрывает меню, снимает паузу и отвязывает инвентари (чтобы не было утечек памяти).
    /// </summary>
    public void Close()
    {
        Visible = false;
        GetTree().Paused = false; // Снимаем с паузы

        CurrentState = OpenState.Closed;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Tab"))
        {
            IsMenuOpen?.Invoke(CurrentState);
        }
        if (@event.IsActionPressed("Profile"))
        {
            Page = Pages.Profile;
            IsMenuOpen?.Invoke(CurrentState);
        }
        if (@event.IsActionPressed("Inventory"))
        {
            Page = Pages.Inventory;
            IsMenuOpen?.Invoke(CurrentState);
        }
    }


    public void OpenPage()
    {
        switch (Page)
        {
            case Pages.Inventory:
                OpenInventory();
                break;
            case Pages.Profile:
                OpenProfile();
                break;
        }
    }

    public void OpenInventory()
    {
        Page = Pages.Inventory;
        StateMachine.ChangeState(InventoryState);
    }
    public void OpenProfile()
    {
        Page = Pages.Profile;
        StateMachine.ChangeState(InventoryState);
    }

    public void ButtonLinks()
    {
        ProfileButton.Pressed += OpenProfile;
        InventoryButton.Pressed += OpenInventory;
    }


    public void InitStates()
    {
        InventoryState = new(_player, InventoryPage, InventoryButton);
        ProfileState = new(_player, ProfilePage, ProfileButton);
    }
}


public enum OpenState
{
    Opened,
    Closed
}

public enum Pages
{
    Inventory,
    Profile,
    SkillsTree
}