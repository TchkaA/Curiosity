using Godot;

public partial class EntitySoundMaker : AudioStreamPlayer2D
{
    [Export] public AudioStreamRandomizer FootstepSound;
    public AudioStreamRandomizer HurtSound = GD.Load<AudioStreamRandomizer>("res://Assets/Origin/Sounds/GameUI/Damage/Damage_randomizer.tres");
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