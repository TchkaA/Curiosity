using Godot;

public class InventoryPageState : IState
{
    private Player _player;
    private InventoryPage _inventoryPage;
    private TextureButton _inventoryButton;
    public InventoryPageState(Player player, InventoryPage inventoryPage, TextureButton inventoryButton)
    {
        _player = player;
        _inventoryPage = inventoryPage;
        _inventoryButton = inventoryButton;
    }
    public void Enter()
    {
        _inventoryPage.Bind(_player.Inventory);
        _inventoryPage.Visible = true;
        _inventoryButton.ButtonPressed = true;
    }

    public void Exit()
    {
        GD.Print("Exit method works Inv");
        _inventoryPage.Unbind();
        _inventoryPage.Visible = false;
        _inventoryButton.ButtonPressed = false;
    }

    // public void 

    public void Update(double delta)
    {
        
    }
}

public class ProfilePageState : IState
{
    private Player _player;
    private ProfilePage _profilePage;
    private TextureButton _profleButton;
    public ProfilePageState(Player player, ProfilePage profilePage, TextureButton profileButton)
    {
        _player = player;
        _profilePage = profilePage;
        _profleButton = profileButton;
    }
    public void Enter()
    {
        _profilePage.Visible = true;
        _profleButton.ButtonPressed = true;
    }

    public void Exit()
    {
        GD.Print("Exit method works Prof");
        _profilePage.Visible = false;
        _profleButton.ButtonPressed = false;
    }

    public void Update(double delta)
    {
        
    }
}