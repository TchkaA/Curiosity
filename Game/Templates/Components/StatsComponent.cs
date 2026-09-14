using System;

public class StatsComponent
{
    // BaseStats
    public int CurrentHealth; 
    public int MaxHealth;
    /// <summary>
    /// Плавающее максимальное хп, которое игрок может восстановить. 
    /// </summary>
    public int MaxFloatingHealth;
    
    public int Speed;
    public int Strength;


    public StatsComponent(int maxHealth = 32,int health = 32, int speed = 80, int strength = 2)
    {
        MaxHealth = maxHealth;
        CurrentHealth = health;
        Speed = speed;
        Strength = strength;
    }

}