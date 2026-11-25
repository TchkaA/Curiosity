using Godot;

public partial class UIAudioStream : AudioStreamPlayer
{
    [Export] public AudioStream MouseEnteredSound;
    [Export] public AudioStream MouseClickSound;

    public override void _Ready()
    {
        ProcessMode = Node.ProcessModeEnum.Always;
    }

    public void PlayMouseEnteredSound()
    {
        if(Stream != MouseEnteredSound) Stream = MouseEnteredSound;
        Play();
    }

    public void PlayMouseClickSound()
    {
        if(Stream != MouseClickSound) Stream = MouseClickSound;
        Play();
    }
}