using Godot;

/// <summary>
/// Компонент визуала дя существ
/// </summary>
public class VisualComponent
{
    public Shader EntityShader = GD.Load<Shader>("res://Visual/Shaders/Entity/EntityShader.gdshader");
    public BaseEntity Entity;

    /// <summary>
    /// Ссылка на спрайт, назначается в инспекторе
    /// </summary>
    public AnimatedSprite2D Sprite { get; set; }

    /// <summary>
    /// Цвет вспышки
    /// </summary>
    public Color FlashColor { get; set; } = Colors.White;

    /// <summary>
    /// Сколько раз мигнуть
    /// </summary>
    public int BlinkCount { get; set; } = 1;

    /// <summary>
    /// Длительность одной фазы (вспышка или погасание), в секундах
    /// </summary>
    public double PhaseDuration { get; set; } = 0.14;

    private bool _isFlashing;

    public VisualComponent(BaseEntity entity,AnimatedSprite2D sprite)
    {
        Entity = entity;
        Sprite = sprite;
        Init();
    }

    public void Init()
    {
        ShaderMaterial shader = new ShaderMaterial();
        shader.Shader = EntityShader;
        shader.SetShaderParameter("flash_color", FlashColor);
        shader.SetShaderParameter("flash_amount", 0.0f);

        Sprite.Material = shader;
    }

    public void PlayHitFlash()
    {
        if (_isFlashing) return; // Если уже мигаем — не перезапускаем

        if (Sprite?.Material is not ShaderMaterial mat) return;

        _isFlashing = true;

        Tween tween = Entity.CreateTween();
        // Кол-во циклов = blink_count * 2 (вспышка + погасание)
        tween.SetLoops(BlinkCount);

        // Анимируем flash_amount 0 -> 1
        tween.TweenMethod(
            Callable.From<float>(v => mat.SetShaderParameter("flash_amount", v)),
            0.0f, 1.0f, PhaseDuration
        );

        // И обратно 1 -> 0
        tween.TweenMethod(
            Callable.From<float>(v => mat.SetShaderParameter("flash_amount", v)),
            1.0f, 0.0f, PhaseDuration
        );

        tween.Finished += () => _isFlashing = false;
    }
}