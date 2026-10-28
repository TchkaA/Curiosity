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


    //По-сути должны быть методы PerformAtack, которые делают атаку, проигрывают по-нужному анимацию(кадр или целая анимка), а также создают зону атаки. Думаю тут можно дергать по-другому SpriteFrames и AnimatedSprite2D,
    // но все равно стоит делать малосвязную систему, то есть отсюда мы не должны дергать саму анимацию, только если засовывать ее как параметр.
}


