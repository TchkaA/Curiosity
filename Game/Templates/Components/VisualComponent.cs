using Godot;

/// <summary>
/// Компонент визуала для существ.
/// Управляет шейдером спрайта: вспышкой при получении урона и обводкой.
/// </summary>
public class VisualComponent
{
    public Shader EntityShader = GD.Load<Shader>("res://Visual/Shaders/Entity/EntityShader.gdshader");
    public BaseEntity Entity;

    /// <summary>
    /// Ссылка на спрайт, назначается в инспекторе.
    /// </summary>
    public AnimatedSprite2D Sprite { get; set; }

    /// <summary>
    /// Цвет вспышки.
    /// </summary>
    public Color FlashColor { get; set; } = Colors.White;

    /// <summary>
    /// Сколько раз мигнуть.
    /// </summary>
    public int BlinkCount { get; set; } = 1;

    /// <summary>
    /// Длительность одной фазы (вспышка или погасание), в секундах.
    /// </summary>
    public double PhaseDuration { get; set; } = 0.14;

    /// <summary>
    /// Материал шейдера, создаётся в <see cref="Init"/> и переиспользуется.
    /// Хранится в поле, чтобы не искать его каждый раз через Sprite.Material.
    /// </summary>
    private ShaderMaterial _material;

    private bool _isFlashing;

    public VisualComponent(BaseEntity entity, AnimatedSprite2D sprite)
    {
        Entity = entity;
        Sprite = sprite;
        Init();
    }

    /// <summary>
    /// Создаёт материал шейдера, назначает его спрайту
    /// и выставляет стартовые параметры (все эффекты выключены).
    /// </summary>
    public void Init()
    {
        _material = new ShaderMaterial() {Shader = EntityShader};
        _material.SetShaderParameter("flash_color", FlashColor);
        _material.SetShaderParameter("flash_amount", 0.0f);

        _material.SetShaderParameter("outline_enabled", false);
        _material.SetShaderParameter("outline_width", 0.0f);

        _material.SetShaderParameter("outline_pulse_amount", 0.0f);
        _material.SetShaderParameter("outline_pulse_speed", 3.0f);

        _material.SetShaderParameter("outline_gradient_enabled", false);
        _material.SetShaderParameter("outline_color2", Colors.Red);

        _material.SetShaderParameter("glitch_enabled", false);

        _material.SetShaderParameter("skin_tint", Colors.White);
        _material.SetShaderParameter("skin_brightness", 1.0f);
        _material.SetShaderParameter("eye_threshold", 0.5f);

        Sprite.Material = _material;
    }

    /// <summary>
    /// Проигрывает анимацию вспышки: плавно заливает спрайт цветом
    /// <see cref="FlashColor"/> и возвращает обратно, повторяя <see cref="BlinkCount"/> раз.
    /// Повторный вызов во время уже идущей вспышки игнорируется.
    /// </summary>
    public void PlayHitFlash()
    {
        if (_isFlashing) return;
        if (_material == null) return;

        _isFlashing = true;

        Tween tween = Entity.CreateTween();
        tween.SetLoops(BlinkCount);

        tween.TweenMethod(
            Callable.From<float>(v => _material.SetShaderParameter("flash_amount", v)),
            0.0f, 1.0f, PhaseDuration
        );

        tween.TweenMethod(
            Callable.From<float>(v => _material.SetShaderParameter("flash_amount", v)),
            1.0f, 0.0f, PhaseDuration
        );

        tween.Finished += () => _isFlashing = false;
    }

    /// <summary>
    /// Включает обводку вокруг спрайта.
    /// </summary>
    /// <param name="width">Толщина обводки в пикселях текстуры. Значения больше 4 могут выглядеть рвано при малом числе сэмплов.</param>
    /// <param name="color">Цвет обводки. Если не передан — используется белый.</param>
    public void ShowOutline(float width = 0.8f, Color? color = null)
    {
        if (_material == null) return;

        _material.SetShaderParameter("outline_enabled", true);
        _material.SetShaderParameter("outline_width", width);
        _material.SetShaderParameter("outline_color", color ?? Colors.White);
    }

    /// <summary>
    /// Выключает обводку: сбрасывает её толщину в ноль и отключает флаг в шейдере.
    /// </summary>
    public void HideOutline()
    {
        if (_material == null) return;

        _material.SetShaderParameter("outline_width", 0.0f);
        _material.SetShaderParameter("outline_enabled", false);
    }

    /// <summary>
    /// Меняет цвет обводки, не трогая её толщину и видимость.
    /// Полезно для подсветки цели другим цветом без пересоздания обводки.
    /// </summary>
    /// <param name="color">Новый цвет обводки.</param>
    public void SetOutlineColor(Color color)
    {
        if (_material == null) return;
        _material.SetShaderParameter("outline_color", color);
    }


    #region Skin Tint

    /// <summary>
    /// Красит кожу существа в заданный цвет. Глаза (всё, что ярче <see cref="eye_threshold"/>)
    /// не затрагиваются. Оригинальные тени сохраняются: пиксель умножается на оттенок,
    /// поэтому светлые участки кожи остаются светлее тёмных, просто в новом цвете.
    /// </summary>
    /// <param name="tint">
    /// Цвет кожи. Так как базовый спрайт тёмно-серый (#191919 / #2a2a2a),
    /// для насыщенных цветов используй яркий тинт (например, чистый красный),
    /// а видимость компенсируй параметром <paramref name="brightness"/>.
    /// </param>
    /// <param name="brightness">
    /// Множитель яркости. 1.0 — тускло (почти как оригинал), 5–8 — нормально для тёмно-красного,
    /// 10+ — кислотно-яркие цвета.
    /// </param>
    /// <param name="eyeThreshold">
    /// Порог яркости, выше которого пиксель считается «глазом» и не тонируется.
    /// По умолчанию 0.5 — с запасом выше кожи (0.10–0.16) и ниже белка глаз (1.0).
    /// Меняй только если у персонажа есть промежуточные яркости (например, серые рога).
    /// </param>
    public void SetSkinTint(Color tint, float brightness = 6.0f, float eyeThreshold = 0.5f)
    {
        if (_material == null) return;

        _material.SetShaderParameter("skin_tint", tint);
        _material.SetShaderParameter("skin_brightness", brightness);
        _material.SetShaderParameter("eye_threshold", eyeThreshold);
    }

    /// <summary>
    /// Сбрасывает тонирование кожи: спрайт становится таким, каким нарисован.
    /// </summary>
    public void ClearSkinTint()
    {
        if (_material == null) return;

        _material.SetShaderParameter("skin_tint", Colors.White);
        _material.SetShaderParameter("skin_brightness", 1.0f);
    }

    #endregion


    #region Outline Pulse

    /// <summary>
    /// Включает пульсацию обводки: толщина колеблется по синусоиде
    /// от <see cref="outline_width"/> до <c>outline_width + amplitude</c>.
    /// </summary>
    /// <param name="amplitude">Насколько пикселей обводка расширяется в пике.</param>
    /// <param name="speed">Скорость пульсации (радиан в секунду). 3–6 — спокойно, 10+ — нервно.</param>
    public void EnableOutlinePulse(float amplitude = 1.5f, float speed = 3.0f)
    {
        if (_material == null) return;

        _material.SetShaderParameter("outline_pulse_amount", amplitude);
        _material.SetShaderParameter("outline_pulse_speed", speed);
    }

    /// <summary>
    /// Выключает пульсацию, оставляя обводку статичной.
    /// </summary>
    public void DisableOutlinePulse()
    {
        if (_material == null) return;
        _material.SetShaderParameter("outline_pulse_amount", 0.0f);
    }

    #endregion

    #region Outline Gradient

    /// <summary>
    /// Включает двухцветную обводку с резкой границей: верхняя половина спрайта
    /// окрашивается в текущий <see cref="outline_color"/> (задаётся через <see cref="ShowOutline"/>),
    /// нижняя — во <paramref name="secondColor"/>.
    /// </summary>
    /// <param name="secondColor">Цвет нижней половины (или правой, если <paramref name="vertical"/> = false).</param>
    /// <param name="vertical">true — деление сверху вниз, false — слева направо.</param>
    public void EnableOutlineGradient(Color secondColor, bool vertical = true)
    {
        if (_material == null) return;

        _material.SetShaderParameter("outline_color2", secondColor);
        _material.SetShaderParameter("outline_gradient_vertical", vertical);
        _material.SetShaderParameter("outline_gradient_enabled", true);
    }

    /// <summary>
    /// Выключает двухцветную обводку, возвращая её к однотонному <see cref="outline_color"/>.
    /// </summary>
    public void DisableOutlineGradient()
    {
        if (_material == null) return;
        _material.SetShaderParameter("outline_gradient_enabled", false);
    }

    #endregion

    #region Glitch

    /// <summary>
    /// Включает эффект «телевизионного глитча»: строки спрайта (вместе с обводкой)
    /// случайно сдвигаются по горизонтали, как рябь на старом ТВ.
    /// </summary>
    /// <param name="intensity">Максимальный сдвиг в пикселях текстуры.</param>
    /// <param name="speed">Частота смены «кадра» глитча (раз в секунду).</param>
    /// <param name="rows">На сколько горизонтальных полос бьётся спрайт. Больше — мельче рябь.</param>
    public void EnableGlitch(float intensity = 5.0f, float speed = 10.0f, float rows = 30.0f)
    {
        if (_material == null) return;

        _material.SetShaderParameter("glitch_intensity", intensity);
        _material.SetShaderParameter("glitch_speed", speed);
        _material.SetShaderParameter("glitch_rows", rows);
        _material.SetShaderParameter("glitch_enabled", true);
    }

    /// <summary>
    /// Выключает глитч.
    /// </summary>
    public void DisableGlitch()
    {
        if (_material == null) return;
        _material.SetShaderParameter("glitch_enabled", false);
    }

    #endregion
}