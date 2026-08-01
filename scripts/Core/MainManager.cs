using Godot;
using System;

public partial class MainManager : Node
{
    public static MainManager Instance {get; private set; }
    public Player Player{get; private set; }
    public Shader OutlineShader  = GD.Load<Shader>("res://shaders/outline/outline.gdshader");
    public PackedScene menuScene = GD.Load<PackedScene>("res://scenes/UI/PlayerMenu/book_menu.tscn");
    
    private bool _isPaused = false;

    private Node pauseMenuInstance;

    public override void _EnterTree()
    {
        Instance = this;
        ProcessMode = Node.ProcessModeEnum.Always;
    }

    public override void _Ready()
    {
        Player = GetTree().GetFirstNodeInGroup("Player") as Player;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            TogglePause();
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
    
    private void PauseGame()
    {
        pauseMenuInstance = menuScene.Instantiate();
        AddChild(pauseMenuInstance);
        
        GetTree().Paused = true;
        _isPaused = true;
    }
    
    private void ResumeGame()
    {
        if (pauseMenuInstance != null)
        {
            RemoveChild(pauseMenuInstance);
            pauseMenuInstance.QueueFree();
            pauseMenuInstance = null;
        }
        GetTree().Paused = false;
        _isPaused = false;
    }


}
