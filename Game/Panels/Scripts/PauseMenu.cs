using System;
using Godot;

[GlobalClass]
public partial class PauseMenu : CanvasLayer
{
    [Export] public Button ContinueButton;
    [Export] public Button SettingsButton;
    [Export] public Button ExitButton;

    public override void _Ready()
    {
        base._Ready();

        ProcessMode = Node.ProcessModeEnum.Always;
        GetTree().Paused = true;

        ContinueButton.Pressed += OnContinuePressed;
        SettingsButton.Pressed += OnSettingPressed;
        ExitButton.Pressed += OnExitPressed;
    }


    private void Close()
    {
        GetTree().Paused = false;
        QueueFree();
    }

    public void OnContinuePressed()
    {
        Close();
    }



    public void OnSettingPressed()
    {
        #if DEBUG
        GD.Print("Пока нет настроек");
        #endif
    }
    public void OnExitPressed()
    {
        GetTree().Quit();
    }

    public override void _Input(InputEvent @event)
    {
        if (!Visible || !@event.IsActionPressed("ui_cancel"))
            return;

        Close();
        GetViewport().SetInputAsHandled();
    }
}
