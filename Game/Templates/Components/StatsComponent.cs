using System;

public class StatsComponent
{
    // BaseStats
    public float CurrentHealth; 
    public float MaxHealth;
    /// <summary>
    /// Плавающее максимальное хп, которое игрок может восстановить. 
    /// </summary>
    public float MaxFloatingHealth;
    
    public float Speed;
    public float Strength;

    /// <summary>
    /// Плоское поглощение входящего урона.
    /// </summary>
    public float Armor;

    /// <summary>
    /// Процентное сопротивление урону (0..1).
    /// </summary>
    public float DamageResistance;

    /// <summary>
    /// Дополнительное сопротивление урону во время блока (0..1).
    /// </summary>
    public float BlockResistance = 0.5f;

    /// <summary>
    /// Скорость рывка.
    /// </summary>
    public float DashSpeed;

    /// <summary>
    /// Перезарядка рывка в секундах (считается с начала рывка).
    /// </summary>
    public float DashCooldown;


    public StatsComponent(float maxHealth = 32,float health = 32, float floatingHealth = 32, float speed = 160, float strength = 2, float dashSpeed = 410, float dashCooldown = 4f)
    {
        MaxHealth = maxHealth;
        CurrentHealth = health;
        MaxFloatingHealth = floatingHealth;
        Speed = speed;
        Strength = strength;
        DashSpeed = dashSpeed;
        DashCooldown = dashCooldown;
    }

}