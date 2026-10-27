using Godot;

/// <summary>
/// Базовый класс оружия
/// </summary>
public partial class Weapon : Resource
{
    /// <summary>
    /// Типы оружия.
    /// </summary>
    public enum Type
    {
        Ranged,
        Magic,
        Melee
    }

    [Export] public Type WeaponType;
    [Export] public float Damage;
    [Export] public int Cooldown;
    [Export] public SpriteFrames UserAnimation;

    //TODO добавить базовые баффы/дебаффы при испольховании или произведении атаки.
}


