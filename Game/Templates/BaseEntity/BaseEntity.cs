using Godot;
using System;

/// <summary>
/// Базовый шаблон, от которого наследуются все существа
/// </summary>
public abstract partial class BaseEntity : CharacterBody2D
{
	/// <summary>
	/// Статы персонажа
	/// </summary>
	public StatsComponent Stats = new();

    /// <summary>
    /// Компонент Анимации
    /// </summary>
    public AnimationComponent animation;

    public MovementComponent movement;

    /// <summary>
    /// Машина состояний сущесвта
    /// </summary>
    public StateMachine stateMachine = new();

    //визуальный спрайт анимации
    public AnimatedSprite2D sprite;

	// Events
	public event Action<BaseEntity> Died;


	public BaseEntity()
	{
	}

	public override void _Ready()
    {
        InitComponents();
    }

	public override void _Process(double delta)
	{
	}


	#region Stats Methods

	/// <summary>
    /// Метод получения урона.
    /// Есть событие Died.
    /// </summary>
    /// <param name="damage">Урон</param>
    public void TakeDamage(int damage)
    {
        Stats.CurrentHealth -= damage;
        if(Stats.CurrentHealth <= 0)
        {
            Stats.CurrentHealth = 0;
            Died?.Invoke(this);
        }
    }

	/// <summary>
    /// Метод восстановления здоровья.
    /// Есть событие OnHealth Changed, которое возвращает текущее хп.
    /// </summary>
    /// <param name="health"></param>
    public void Heal(int health)
    {
        Stats.CurrentHealth += health;
        if (Stats.CurrentHealth > Stats.MaxFloatingHealth) Stats.CurrentHealth = Stats.MaxFloatingHealth;
    }


	/// <summary>
    /// Уменьшить максимальное здоровье
    /// С событием игрока
    /// </summary>
    /// <param name="health"></param>
    public void ReduceMaxHealth(int health)
    {
        Stats.MaxHealth -= health;
        if (Stats.CurrentHealth > Stats.MaxHealth)
        {
            Stats.CurrentHealth = Stats.MaxHealth;
        }
        if (Stats.CurrentHealth <= 0)
        {
            Stats.CurrentHealth = 0;
            Stats.MaxHealth = 0;
            Died?.Invoke(this);
        }
    }


	/// <summary>
    /// Метод восстановления плавающего здоровья
    /// Не может быть больше максимального здоровья
    /// Есть событие MaxCurrentHealth, если плавающее здоровье равно максимальному здоровью.
    /// </summary>
    /// <param name="num">Число для увелечения плавающего здоровья</param>
    public void AddFloatingHealth(int num)
    {
        if(Stats.MaxFloatingHealth == Stats.MaxHealth)
        {
            return; 
        }
        Stats.MaxFloatingHealth += num;
        if(Stats.MaxFloatingHealth >= Stats.MaxHealth)
        {
            Stats.MaxFloatingHealth = Stats.MaxHealth;
        }
    }

	/// <summary>
	/// Метод увеличения максимального здоровья игрока
	/// </summary>
	/// <param name="health">Кол-во увеличиваемого макимального здоровья</param>
    public void IncreaseMaxHealth(int health)
    {
        Stats.MaxHealth += health;
    }

	#endregion

    #region Components
    public virtual void InitComponents()
    {
        animation = new(this, sprite);
        movement = new(this);
    }

    #endregion
}
