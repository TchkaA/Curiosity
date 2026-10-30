using Godot;

/// <summary>
/// Базовый класс оружия
/// </summary>
public abstract partial class Weapon : Resource
{
    /// <summary>
    /// Типы оружия.
    /// </summary>
    public enum Type
    {
        Melee,
        Ranged,
        Magic
    }

    [Export] public Type WeaponType;
    [Export] public float Damage;
    [Export] public int Cooldown;
    [Export] public SpriteFrames UserAnimation;
    [Export] public int Radious;

    //TODO добавить базовые баффы/дебаффы при испольховании или произведении атаки.

    public virtual void Baff()
    {
        
    }
}


