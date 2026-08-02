using Godot;

public partial class HealingPotion : Item
{
    [Export]
    public int HealAmount = 50;

    public override void Use(Node2D user)
    {
        if (user is BaseEntity entity)
        {
            entity.Stats.Heal(HealAmount);
            GD.Print($"Heal {HealAmount}hp");
        }
    }
}
