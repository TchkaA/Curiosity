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
    /// <summary>
    /// Урон сильной атаки.
    /// </summary>
    [Export] public float StrongDamage;
    [Export] public float Cooldown;
    [Export] public SpriteFrames UserAnimation;
    [Export] public int Radious;
    [Export] public float ComboWindow;
    [Export] public int MaxCombo;

    //TODO добавить базовые баффы/дебаффы при испольховании или произведении атаки.

    public virtual void Baff()
    {
        
    }
}


