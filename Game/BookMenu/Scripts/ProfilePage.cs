using Godot;
public partial class ProfilePage : Control
{
    public Player _player {get; private set;}
    [Export]
    public ProfileInterface profile;

    public void ShowInfo(Player player)
    {
        _player = player;
        LinkActions();
    }

    public void LinkActions()
    {
        _player.OnHealthChanged += profile.ChangeHealthUI;
    }

    public void ShowStats()
    {
        profile.ChangeHealthUI(_player.Stats.CurrentHealth);
    }

}