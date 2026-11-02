using Godot;

public partial class Hands : Weapon
{
    public Hands()
    {
        WeaponType = Type.Melee;
        Damage = 3;
        StrongDamage = 6;
        Cooldown = 0.1f;
        UserAnimation = GD.Load<SpriteFrames>("res://Game/Weapons/Melee/Hands/HandsAnimation.tres");
        Radious = 30;
        ComboWindow = 2f;
        MaxCombo = 2;
    }
}