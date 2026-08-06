using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// Профиль игрока, открывается на esc
/// </summary>
public partial class BookMenu : CanvasLayer
{
    public static BookMenu Instance {get; private set; }

    [ExportGroup("Страницы")]
    [Export]
    private Control InventoryPage;
    [Export]
    private CanvasLayer ProfilePage;

    [ExportGroup("Кнопки")]
    [Export]
    public TextureButton InventoryButton;
    [Export]
    public TextureButton ProfileButton;

    [ExportGroup("Инвентари")]
    [Export]
    private InventoryGrid _playerInventory;
    [Export]
    private InventoryGrid _extraInventory;

    public override void _EnterTree()
    {
        Instance = this;
        ProcessMode = Node.ProcessModeEnum.Always;
        ShowPage();
        InventoryButton.Pressed += OpenInventory;
        ProfileButton.Pressed += OpenProfile;


        // Bind
        _playerInventory.Bind(MainManager.Instance.Player.Inventory);
    }

    private void PauseGame(bool IsPaused)
    {
        switch (IsPaused)
        {
            case true:
                ShowPage();
                break;
            case false:
                QueueFree();
                break;
        }
    }


    public void ShowPage(string page = "profile")
    {
        bool isProfile = page == "profile";
        bool isInventory = page == "inventory";

        ProfilePage.Visible = isProfile;
        ProfileButton.ButtonPressed = isProfile;

        InventoryPage.Visible = isInventory;
        InventoryButton.ButtonPressed = isInventory;
    }

    private void OpenInventory()
    {
        ShowPage("inventory");
        InventoryButton.ButtonPressed = true;
        ProfileButton.ButtonPressed = false;
    }

    private void OpenProfile()
    {
        ShowPage("profile");
        ProfileButton.ButtonPressed = true;
        InventoryButton.ButtonPressed = false;

    }


    public override void _ExitTree()
    { 

        InventoryButton.Pressed -= OpenInventory;
        ProfileButton.Pressed -= OpenProfile;
        _extraInventory.Visible = false;
    }

    public void BindExtraInventory(Inventory inventory)
    {
        if (inventory == null)
        {
            _extraInventory.Unbind();
            _extraInventory.Visible = false;
            return;
        }

        _extraInventory.Bind(inventory);
        _extraInventory.Visible = true;
    }

}
