using Godot;
using System;

public class SoundComponent
{
    private Node2D _owner;

    public static event Action<SoundData> OnSound;

    public SoundComponent(Node2D owner)
    {
        _owner = owner;
    }

    public void MakeSound(float radius, int type)
    {
        MakeSound(radius, type, _owner.GlobalPosition);
    }

    public void MakeSound(float radius, int type, Vector2 pos)
    {
        var soundData = new SoundData
        {
            Position = pos,
            radius = radius,
            Type = type
        };

        OnSound?.Invoke(soundData);
    }

}