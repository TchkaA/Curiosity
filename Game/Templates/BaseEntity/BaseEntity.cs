using Godot;
using System;

/// <summary>
/// Базовый шаблон, от которого наследуются все существа
/// </summary>
public abstract partial class BaseEntity : CharacterBody2D, IDamageble
{
	/// <summary>
	/// Статы персонажа
	/// </summary>
	public StatsComponent Stats = new();

    [Export]
    /// <summary>
    /// Компонент Анимации
    /// </summary>
    public AnimationComponent animation;

    /// <summary>
    /// Компонент движения
    /// </summary>
    public MovementComponent movement;

    /// <summary>
    /// Компонент направления
    /// </summary>
    public DirectionComponent direction;

    /// <summary>
    /// Компонент визуала существа
    /// </summary>
    public VisualComponent visualComponent;

    /// <summary>
    /// Машина состояний сущесвта
    /// </summary>
    public StateMachine stateMachine = new();

    //---------------------
	//		Audio
	//---------------------
	[Export]
	public EntitySoundMaker SoundMaker;

    //---------------------
    //      Collision
    //---------------------
    [Export] public CollisionShape2D Collision;
	// Events
	public event Action<BaseEntity> Died;
    public event Action<float> OnHealthChanged;

    //---------------------
	// Flags                //TODO пепренести логику в компонеенет.
	//---------------------
	public bool isRunning = false; // TODO перенести логику в protected
    protected bool CanDash = true;
    public virtual void ChangeDashFlag(bool flag) => CanDash = flag;
    public bool CanAttack = true;
    protected bool CanInteract = true;
    public bool InDialogue = false;
    /// <summary>
    /// Активен ли блок. Влияет на входящий урон.
    /// </summary>
    public bool IsBlocking = false;

    /// <summary>
	/// Остаток перезарядки рывка в секундах.
	/// </summary>
	protected float _dashCooldown;


    //Weapon
    public Weapon weapon;

    //Inventory
    public Inventory Inventory {get; private set;}

    [Export]
    public Color color; 


	public BaseEntity()
	{
	}

	public override void _Ready()
    {
        InitComponents();
        LinksActions();
        ResetConsts();

        SetUpSkin();

        //Init Inventory TODO: create InventoryComponent
        Inventory = new(this);

    }

    public override void _PhysicsProcess(double delta)
    {
        stateMachine.Update(delta);
        CountDashTime((float)delta);
    }


	#region Stats Methods

	/// <summary>
    /// Метод получения урона.
    /// Есть событие Died.
    /// </summary>
    /// <param name="damage">Урон</param>
    public void TakeDamage(float damage)
    {
        if (damage <= 0f || Stats.CurrentHealth <= 0f)
            return;

        Stats.CurrentHealth -= damage;
        if(Stats.CurrentHealth <= 0)
        {
            Stats.CurrentHealth = 0;
            Died?.Invoke(this);
        }
        visualComponent.PlayHitFlash();
        SoundMaker.PlayHurt();
        OnHealthChanged?.Invoke(Stats.CurrentHealth);
    }

    /// <summary>
    /// Считает урон атаки с учётом оружия и силы атакующего.
    /// </summary>
    public float CalculateOutgoingDamage(Weapon attackWeapon, bool strongAttack)
    {
        if (attackWeapon == null)
            return 0f;

        float baseDamage = strongAttack ? attackWeapon.StrongDamage : attackWeapon.Damage;
        return Mathf.Max(0f, baseDamage + Stats.Strength);
    }

    /// <summary>
    /// Считает, сколько из входящего урона существо реально получит.
    /// Учитывает броню, сопротивление и активный блок.
    /// </summary>
    public float CalculateIncomingDamage(float damage)
    {
        float result = Mathf.Max(0f, damage - Stats.Armor);
        float resistance = Stats.DamageResistance + (IsBlocking ? Stats.BlockResistance : 0f);
        result *= 1f - Mathf.Clamp(resistance, 0f, 0.95f);
        return Mathf.Max(0f, result);
    }

    /// <summary>
    /// Наносит урон от атаки другого существа с учётом всех бафов.
    /// </summary>
    public void TakeAttackDamage(BaseEntity attacker, Weapon attackWeapon, bool strongAttack)
    {
        if (attacker == null)
            return;

        float damage = CalculateIncomingDamage(attacker.CalculateOutgoingDamage(attackWeapon, strongAttack));
        TakeDamage(damage);
    }

	/// <summary>
    /// Метод восстановления здоровья.
    /// Есть событие OnHealth Changed, которое возвращает текущее хп.
    /// </summary>
    /// <param name="health"></param>
    public void Heal(float health)
    {
        Stats.CurrentHealth += health;
        if (Stats.CurrentHealth > Stats.MaxFloatingHealth) 
        {
            Stats.CurrentHealth = Stats.MaxFloatingHealth;
            return;
        }
        OnHealthChanged?.Invoke(Stats.CurrentHealth);
    }


	/// <summary>
    /// Уменьшить максимальное здоровье
    /// С событием игрока
    /// </summary>
    /// <param name="health"></param>
    public void ReduceMaxHealth(float health)
    {
        Stats.MaxHealth -= health;
        if (Stats.CurrentHealth > Stats.MaxHealth)
        {
            Stats.CurrentHealth = Stats.MaxHealth;
            return;
        }
        if (Stats.CurrentHealth <= 0)
        {
            Stats.CurrentHealth = 0;
            Stats.MaxHealth = 0;
            Died?.Invoke(this);
        }
        OnHealthChanged?.Invoke(Stats.CurrentHealth);
    }


	/// <summary>
    /// Метод восстановления плавающего здоровья
    /// Не может быть больше максимального здоровья
    /// Есть событие MaxCurrentHealth, если плавающее здоровье равно максимальному здоровью.
    /// </summary>
    /// <param name="num">Число для увелечения плавающего здоровья</param>
    public void AddFloatingHealth(float num)
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
    public void IncreaseMaxHealth(float health)
    {
        Stats.MaxHealth += health;
    }

	#endregion

    #region Components
    public virtual void InitComponents()
    {
        Stats = new();
        movement = new(this);
        direction = new(this);
        visualComponent = new(this, animation);

    }

    /// <summary>
    /// Событие, которое используется для связки несвязных событий.
    /// Каждое событие должно поясняться 
    /// </summary>
    public virtual void LinksActions()
    {
        // Уведомляем анимации, что они 
        direction.DirectionChanged += animation.NotifyAnimation;
        Died += Die;
    }

    #endregion



    #region Moving
    public bool CanRun()
	{
		if(Stats.CurrentHealth > 12)
		{
			return true;
		}
		return false;
	}

	public void ToggleRun()
	{
		if (isRunning) { isRunning = false; return; }
		if (CanRun()) isRunning = true;
	}

    #endregion

    #region Visual
    public void SetUpSkin()
    {
        visualComponent.SetSkinTint(color);
    }

    #endregion

    #region Other

    public void CountDashTime(float time)
    {
        if (_dashCooldown < Stats.DashCooldown)
        {
            _dashCooldown += time;
        }
        // GD.Print(_dashCooldown);
    }

    public void ChangeVisibleLayerPosition(Node2D obj)
    {
		if(GlobalPosition.Y > obj.GlobalPosition.Y)
		{
			obj.ZIndex = Consts.EntityIndex - 1;
		}
		else
		{
			obj.ZIndex = Consts.EntityIndex + 1;
		}
	}

    public void ReturnZIndex()
    {
        ZIndex = Consts.EntityIndex;
    }


    public virtual void Die(BaseEntity died)
    {
        
    }

    #endregion

    #region Interaction

    public virtual void GoToInteraction(BaseEntity entity)
    {
        //Используется для малосвязанности и не переписывания кучи систем.
    }


    public virtual void Follow(BaseEntity entity)
    {
        
    }

    #endregion

    public void ResetConsts()
    {
        SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, Consts.EntityLayer);
        SetDeferred(CollisionObject2D.PropertyName.CollisionMask, Consts.EntityLayer);
        ZIndex = Consts.EntityIndex;
    }

    public void test()
    {
        GD.Print("it works");
    }
    public void test(object obj)
    {
        GD.Print(obj + "is working");
    }
}
