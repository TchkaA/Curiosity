using Godot;

// Наследуемся от Item!
public partial class HealingPotion : Item
{
    [Export] public int HealAmount { get; set; } = 50; // Уникальное поле только для зелья!

    public HealingPotion()
    {
        // Задаем дефолтные значения, которые можно переопределить в инспекторе Godot
        Name = "Зелье лечения";
        Description = "Восстанавливает здоровье.";
        Icon = GD.Load<Texture2D>("res://Objects/Items/Potions/HealingPotion/Healing_potion_texture.tres");
        MaxStack = 6;
        ID = 1; 
    }

    public override bool Use(Node2D user)
    {
        // Проверяем, что пользователь - это BaseEntity, чтобы дать ему хил
        if (user is BaseEntity entity)
        {
            entity.Heal(HealAmount); // Используем твой готовый метод из BaseEntity!
            GD.Print($"[HealingPotion] {entity.Name} восстановил {HealAmount} HP!");
            return true; // Использование успешно, предмет можно удалить из инвентаря
        }
        
        GD.PrintErr("[HealingPotion] Не удалось использовать: пользователь не является BaseEntity.");
        return false;
    }
}