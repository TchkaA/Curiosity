using Godot;

/// <summary>
/// Зелье лечения. Восстанавливает здоровье при использовании.
/// Все значения задаются в ресурсе <c>Healing_potion.tres</c>.
/// </summary>
[GlobalClass]
public partial class HealingPotion : Item
{
    /// <summary>
    /// Сколько здоровья восстанавливает одно зелье.
    /// </summary>
    [Export] public int HealAmount { get; set; } = 50;

    public override bool Use(Node2D user)
    {
        if (user is BaseEntity entity)
        {
            entity.Heal(HealAmount);
            GD.Print($"[HealingPotion] {entity.Name} восстановил {HealAmount} HP");
            return true;
        }

        GD.PushError("[HealingPotion] Пользователь не является BaseEntity");
        return false;
    }
}