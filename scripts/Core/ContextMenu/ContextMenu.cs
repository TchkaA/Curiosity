using System.Collections.Generic;
using Godot;

public partial class ContextMenu : Control
{
    [Export] private VBoxContainer _container;

    public override void _Ready()
    {
        Visible = false;
    }

    public void ShowMenu(IEnumerable<ContextAction> actions, Vector2 position)
    {
        Clear();

        Position = position;

        foreach (var action in actions)
        {
            var button = new Button();
            button.Text = action.Name;

            button.Pressed += () =>
            {
                HideMenu();
                action.Callback?.Invoke();
            };

            _container.AddChild(button);
        }

        Visible = true;
    }

    public void HideMenu()
    {
        Visible = false;
        Clear();
    }

    private void Clear()
    {
        foreach (Node child in _container.GetChildren())
        {
            child.QueueFree();
        }
    }
}