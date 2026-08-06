using Godot;
using System;

public partial class MainManager : Node
{
    public static MainManager Instance {get; private set; }
    public Player Player{get; private set; }

    public Shader OutlineShader  = GD.Load<Shader>("res://Game/shaders/outline/outline.gdshader");
    public PackedScene menuScene = ResourceLoader.Load<PackedScene>("res://Game/UI/Scenes/BookMenus/book_menu.tscn");


    public ContextMenu ContextMenu;
    private bool _isPaused = false;

    public Action<bool> isPaused;
    private Node _menu;

    public override void _EnterTree()
    {
        Instance = this;
        ProcessMode = Node.ProcessModeEnum.Always;
    }

    public override void _Ready()
    {
        Player = GetTree().GetFirstNodeInGroup("Player") as Player;
        ContextMenu = GetNodeOrNull<ContextMenu>("/root/ContextMenu");

        if (ContextMenu == null)
        {
            GD.PushWarning("ContextMenu autoload not found. Check project.godot autoload configuration.");
        }

    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            TogglePause();
        }
        if (@event.IsActionPressed("context_menu"))
        {
            // Later add logic of player context menu
        }
    }
    
    private void TogglePause()
    {
        if (_isPaused)
        {
                      
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    public void PauseGame()
    {
        _menu = menuScene.Instantiate();
        AddChild(_menu);
        GetTree().Paused = true;
        _isPaused = true;
        isPaused?.Invoke(true);
    }
    


    private void ResumeGame()
    {
        _menu.QueueFree(); 
        GetTree().Paused = false;
        _isPaused = false;
        isPaused?.Invoke(false);
    }
}
