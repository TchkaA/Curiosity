using Godot;

public partial class EntitySoundMaker : AudioStreamPlayer2D
{
    [Export] public AudioStreamRandomizer FootstepSound;
    [Export] public AudioStreamRandomizer HurtSound;
    [Export] public AudioStreamRandomizer DeathSound;

    public void PlayFootstep() => PlayWith(FootstepSound);
    public void PlayHurt() => PlayWith(HurtSound);
    public void PlayDeath() => PlayWith(DeathSound);

    private void PlayWith(AudioStreamRandomizer sound)
    {
        if (sound == null) return;
        Stream = sound;
        Play();
    }
}