using Godot;
using System;

public partial class ChickenMeat : Item
{
    [Export] public int HealAmount { get; set; } = 10; // Уникальное поле только для зелья!

    public ChickenMeat()
    {
        // Задаем дефолтные значения, которые можно переопределить в инспекторе Godot
        Name = "Chicken Meat";
        Description = "КУРОЧКА!!!!";
        Icon = GD.Load<Texture2D>("res://Objects/Items/Food/CheckenMeat.tres");
        MaxStack = 4;
        ID = 2; 
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
