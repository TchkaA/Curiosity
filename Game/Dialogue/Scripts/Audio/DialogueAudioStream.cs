using Godot;

public partial class DialogueAudioStream : AudioStreamPlayer
{
    [Export] public AudioStream MouseEnteredSound;
    [Export] public AudioStream MouseClickSound;

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