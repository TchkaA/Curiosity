using Godot;

public partial class PlayerUI : CanvasLayer
{
    [Export] private TextureProgressBar HealthBar;

    /// <summary>
    /// Сущность, на события которой подписан UI.
    /// </summary>
    private BaseEntity _entity;

    public override void _ExitTree()
    {
        Unbind();
    }

    /// <summary>
    /// Подписывает UI на события сущности и сразу синхронизирует показания.
    /// </summary>
    /// <param name="entity">Сущность (игрок), за которой следит UI</param>
    public void Bind(BaseEntity entity)
    {
        if (entity == null)
        {
            GD.PushWarning("PlayerUI.Bind: не передана сущность для подписки.");
            return;
        }

        // Чтобы не было двойной подписки при повторном бинде.
        Unbind();
        _entity = entity;
        _entity.OnHealthChanged += ChangeHealth;

        if (HealthBar != null)
        {
            HealthBar.MaxValue = _entity.Stats.MaxHealth;
        }
        ChangeHealth(_entity.Stats.CurrentHealth);
    }

    /// <summary>
    /// Отписывает UI от событий сущности.
    /// </summary>
    public void Unbind()
    {
        if (_entity == null) return;

        _entity.OnHealthChanged -= ChangeHealth;
        _entity = null;
    }

    /// <summary>
    /// Обновляет полосу здоровья.
    /// </summary>
    /// <param name="health">Текущее здоровье сущности</param>
    public void ChangeHealth(float health)
    {
        if (HealthBar == null) return;

        if (_entity != null)
        {
            HealthBar.MaxValue = _entity.Stats.MaxHealth;
        }
        HealthBar.Value = health;
    }
}
