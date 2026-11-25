using Godot;


public partial class PauseMenu : CanvasLayer
{
    [Export] public Button ContinueButton;
    [Export] public Button SettingsButton;
    [Export] public Button ExitButton;

    [Export] private UIAudioStream _audioStream;

    private static bool IsPaused = false;
    public static PauseMenu Instance;

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;

        ProcessMode = Node.ProcessModeEnum.Always;
        Hide();

        ContinueButton.Pressed += OnContinuePressed;
        SettingsButton.Pressed += OnSettingsPressed;
        ExitButton.Pressed += OnExitPressed;

        ContinueButton.FocusEntered += ButtonFocused;
        SettingsButton.FocusEntered += ButtonFocused;
        ExitButton.FocusEntered += ButtonFocused;
        
        ContinueButton.MouseEntered += ButtonFocused;
        SettingsButton.MouseEntered += ButtonFocused;
        ExitButton.MouseEntered += ButtonFocused;
    }


    private void Close()
    {
        GetTree().Paused = false;
        Hide();
    }

    public void OnContinuePressed()
    {
        ButtonPressed();
        Close();
    }

    public void Open()
    {
        GetTree().Paused = true;
        ContinueButton.GrabFocus();
        Show();
    }


    public void OnSettingsPressed()
    {
        ButtonPressed();
        GD.Print("Пока нет настроек");
    }
    public void OnExitPressed()
    {
        GetTree().Quit();
    }

    public void ToggleMenu()
    {
        if (!GetTree().Paused) Open();
        else Close();
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ButtonPressed()
    {
        _audioStream.PlayMouseClickSound();
    }

    public void ButtonFocused()
    {
        _audioStream.PlayMouseEnteredSound();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("ui_cancel"))
            return;

        ToggleMenu();
        GetViewport().SetInputAsHandled();
    }
}
