using Godot;
using System;

public partial class MainManager : Node
{
    public static MainManager Instance {get; private set; }
    public Player Player{get; private set; }
    public Shader OutlineShader  = GD.Load<Shader>("res://shaders/outline/outline.gdshader");
    public PackedScene menuScene = GD.Load<PackedScene>("res://scenes/UI/PlayerMenu/book_menu.tscn");
    
    public ContextMenu ContextMenu;

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
        ContextMenu = GetTree().GetFirstNodeInGroup("ContextMenu") as ContextMenu;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            TogglePause();
        }
        if (@event.IsActionPressed("context_menu"))
        {
            OpenContextMenu();
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


    private void OpenContextMenu()
    {
        var mousePosition = GetViewport().GetMousePosition();

        var world = GetViewport()
            .GetCamera2D()
            .GetWorld2D()
            .DirectSpaceState;

        var query = new PhysicsPointQueryParameters2D
        {
            Position = mousePosition
        };

        var results = world.IntersectPoint(query);

        foreach (var result in results)
        {
            var collider = result["collider"].AsGodotObject();

            if (collider is Node2D node &&
                node is IContextMenuProvider provider)
            {
                ContextMenu.ShowMenu(
                    provider.GetContextAction(Player),
                    mousePosition);

                return;
            }
        }
    }
}
