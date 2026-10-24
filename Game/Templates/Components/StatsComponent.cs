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
    /// Скорость рывка.
    /// </summary>
    public float DashSpeed;

    /// <summary>
    /// Перезарядка рывка в секундах (считается с начала рывка).
    /// </summary>
    public float DashCooldown;


    public StatsComponent(float maxHealth = 32,float health = 32, float speed = 160, float strength = 2, float dashSpeed = 420, float dashCooldown = 1f)
    {
        MaxHealth = maxHealth;
        CurrentHealth = health;
        Speed = speed;
        Strength = strength;
        DashSpeed = dashSpeed;
        DashCooldown = dashCooldown;
    }

}