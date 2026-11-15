using System;
using System.Collections.Generic;
using Godot;

public partial class EntitySoundMaker : Node2D
{
    [Export] public AudioStreamRandomizer FootstepSound;
    [Export] public AudioStreamRandomizer HurtSound = GD.Load<AudioStreamRandomizer>("res://Assets/Origin/Sounds/GameUI/Damage/Damage_randomizer.tres");
    [Export] public AudioStreamRandomizer DeathSound;

    public Dictionary<EntitySound, Action> Sounds { get; } = new();

    private AudioStreamPlayer2D _footstepPlayer;

    private AudioStreamPlayer2D _entityPlayer;

    public Action FootStep;
    public void PlayFootstep() => PlayOn(_footstepPlayer, FootstepSound);
    public void PlayHurt() => PlayOn(_entityPlayer, HurtSound);
    public void PlayDeath() => PlayOn(_entityPlayer, DeathSound);

    private static void PlayOn(AudioStreamPlayer2D player, AudioStreamRandomizer sound)
    {
        if (sound == null) return;
        player.Stream = sound;
        player.Play();
    }

    public override void _Ready()
    {
        CreatePlayers();
        LinksActions();
    }


    public void CreatePlayers()
    {
        _footstepPlayer = new AudioStreamPlayer2D
        {
            Bus = "Game",
            MaxDistance = 200,
            Attenuation = 1,
            VolumeDb = -10,
        };
        AddChild(_footstepPlayer);
        _entityPlayer = new AudioStreamPlayer2D
        {
            Bus = "Game",
            MaxDistance = 400,
            Attenuation = 1,
            VolumeDb = -10,
        };
        AddChild(_entityPlayer);
    }

    public void Play(EntitySound sound)
    {
        Sounds[sound]?.Invoke();
    }

    public void LinksActions()
    {
        Sounds[EntitySound.FootStep] = PlayFootstep;
        Sounds[EntitySound.Hurt]     = PlayHurt;
        Sounds[EntitySound.Death]    = PlayDeath;
    }
}


public enum EntitySound
{
    FootStep,
    Hurt,
    Death,
    Voice,
    Punch,
    Scream,
    Dash
}