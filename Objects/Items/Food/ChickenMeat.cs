using Godot;

/// <summary>
/// Съедобное мясо. Восстанавливает здоровье при использовании.
/// Все значения задаются в ресурсе <c>Chicken_meat.tres</c>.
/// </summary>
[GlobalClass]
public partial class ChickenMeat : Item
{
    /// <summary>
    /// Сколько здоровья восстанавливает одна порция.
    /// </summary>
    [Export] public int HealAmount { get; set; } = 10;

    public override bool Use(Node2D user)
    {
        if (user is BaseEntity entity)
        {
            entity.Heal(HealAmount);
            GD.Print($"[ChickenMeat] {entity.Name} восстановил {HealAmount} HP");
            return true;
        }

        GD.PushError("[ChickenMeat] Пользователь не является BaseEntity");
        return false;
    }
}
