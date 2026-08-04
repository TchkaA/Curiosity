using System;
using System.Collections.Generic;
using Godot;

public partial class ContextMenu : VBoxContainer
{
    private readonly List<ContextAction> _actions = new();

    public override void _Ready()
    {
        
    }

    public void Initialize(IEnumerable<ContextAction> actions)
    {
        _actions.Clear();
        foreach (var action in actions)
        {
            AddActionButton(action);
        }
    }

    private void AddActionButton(ContextAction action)
    {
        var button = new Button();
        button.Text = action.Name;
        button.Pressed += () => OnActionButtonPressed(action);
        AddChild(button);
    }

    private void OnActionButtonPressed(ContextAction action)
    {
        action.Callback();
        QueueFree();
    }
}