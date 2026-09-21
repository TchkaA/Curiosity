using Godot;
using System;

public class UIComponent
{
    private Player _player;
    private BookMenu _bookMenu;

    public Action<bool> OnPausedStateChanged;

    public UIComponent(Player player, BookMenu bookMenu)
    {
        _player = player;
        _bookMenu = bookMenu;
    }

    public void ToggleMenu(OpenState state)
    {
        if (state == OpenState.Opened)
        {
            _bookMenu.Close();
            OnPausedStateChanged?.Invoke(false);
        }
        else if(state == OpenState.Closed)
        {
            // Передаем инвентарь игрока напрямую. Никаких поисков по группам!
            _bookMenu.Open(_player.Inventory); 
            OnPausedStateChanged?.Invoke(true);
        }
    }
}