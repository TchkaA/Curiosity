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


    public StatsComponent(float maxHealth = 32,float health = 32, float speed = 90, float strength = 2)
    {
        MaxHealth = maxHealth;
        CurrentHealth = health;
        Speed = speed;
        Strength = strength;
    }

}