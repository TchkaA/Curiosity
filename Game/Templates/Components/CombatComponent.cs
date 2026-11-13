using Godot;

public class CombatComponent
{
    private Player _player;

    /// <summary>
    /// Имя текущей атаки: base_attack или strong_attack.
    /// </summary>
    public string CurrentAttack;

    /// <summary>
    /// Сколько секунд осталось до конца текущей анимации атаки.
    /// </summary>
    private double _elipse;

    /// <summary>
    /// Полная длительность текущей анимации атаки.
    /// </summary>
    private double _attackDuration;

    /// <summary>
    /// Сколько секунд осталось на продолжение серии.
    /// </summary>
    private double _comboWindow;

    /// <summary>
    /// Текущий шаг серии базовой атаки.
    /// </summary>
    public int comboAttack;

    /// <summary>
    /// Атака закончилась.
    /// </summary>
    public bool StopAttack;

    public Area2D damageArea;

    public CombatComponent(Player player)
    {
        _player = player;
        StopAttack = false;
    }

    /// <summary>
    /// Метод инициализации атаки
    /// </summary>
    public void PerformAttack(TypeAttack type)
    {
        switch (type)
        {
            case TypeAttack.BaseAttack:
                CurrentAttack = "base_attack";
                break;
            case TypeAttack.StrongAttack:
                CurrentAttack = "strong_attack";
                break;
            default:
                CurrentAttack = "base_attack";
                break;
        }
        _player.TryAttack();
    }

    public void update(double delta)
    {
        if (_elipse > 0)
        {
            _elipse -= delta;
            if (_elipse <= 0)
            {
                _elipse = 0;
                StopAttack = true;
                RemoveDamageArea();
            }
            if (_elipse <= 0.1)
            {
                RemoveDamageArea();
            }
        }

        if (_comboWindow > 0)
        {
            _comboWindow -= delta;
            if (_comboWindow <= 0)
            {
                _comboWindow = 0;
                comboAttack = 0;
            }
        }
    }

    /// <summary>
    /// Старт удара. Вызывается из состояния атаки.
    /// </summary>
    public void StartAttack()
    {
        StopAttack = false;
        AddDamageArea();
        _comboWindow = _player.weapon.ComboWindow;

        if (CurrentAttack == "base_attack")
        {
            if (comboAttack >= _player.weapon.MaxCombo)
            {
                comboAttack = 0;
            }
            comboAttack += 1;
            PlayAttackAnimation("base_attack_" + comboAttack);
        }
        else
        {
            PlayAttackAnimation(CurrentAttack);
        }
    }

    public void EndAttack()
    {
        _player.animation.EndWeaponAnimation();
    }

    /// <summary>
    /// Можно ли начать следующий удар: либо атака уже закончилась,
    /// либо до конца анимации осталась последняя треть.
    /// </summary>
    public bool CanStartNextAttack()
    {
        if (StopAttack)
        {
            return true;
        }
        return _elipse <= _attackDuration / 3.0;
    }

    /// <summary>
    /// Играет анимацию удара и запоминает её длительность.
    /// </summary>
    private void PlayAttackAnimation(string animation)
    {
        SpriteFrames frames = _player.animation.WeaponAnimation;

        // Пока нет отдельной анимации для этой атаки - играем базовый удар.
        if (frames == null || !frames.HasAnimation(animation))
        {
            animation = "base_attack_1";
        }

        _attackDuration = GetAnimationLength(animation);
        _elipse = _attackDuration;

        _player.animation.PlayWeaponAnimation(animation);
    }

    /// <summary>
    /// Длительность анимации оружия в секундах (кадры / скорость).
    /// </summary>
    private float GetAnimationLength(string animation)
    {
        SpriteFrames frames = _player.animation.WeaponAnimation;

        if (frames == null || !frames.HasAnimation(animation))
        {
            return 0.4f;
        }

        int count = frames.GetFrameCount(animation);
        float speed = (float)frames.GetAnimationSpeed(animation);

        if (speed <= 0f)
        {
            return 0.4f;
        }

        return count / speed;
    }


    public void AddDamageArea()
    {
        if (GodotObject.IsInstanceValid(damageArea))
        {
            return;
        }
        damageArea = new Area2D();
        CollisionShape2D collision = new CollisionShape2D();
        CapsuleShape2D shape = new CapsuleShape2D() { Radius = _player.weapon.Radious };
        collision.Shape = shape;

        damageArea.AddChild(collision);
        damageArea.SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, Consts.EntityLayer);
        damageArea.SetDeferred(CollisionObject2D.PropertyName.CollisionMask, Consts.EntityLayer);
        _player.AddChild(damageArea);

        
        
        damageArea.Position = _player.direction.ToVector() * 50;
        damageArea.BodyEntered += Entity_Entered;
    }

    public void RemoveDamageArea()
    {
        if (GodotObject.IsInstanceValid(damageArea))
        {
            damageArea.QueueFree();
            damageArea.BodyEntered -= Entity_Entered;
        }
    }

    public void Entity_Entered(Node2D node)
    {
       if (node is BaseEntity body && node != _player)
        {
            if(CurrentAttack == "base_attack")
                body.TakeDamage(_player.weapon.Damage);
            else
                body.TakeDamage(_player.weapon.StrongDamage);
        }
        
    }
}

public enum TypeAttack
{
    BaseAttack,
    StrongAttack,
    Block
}
