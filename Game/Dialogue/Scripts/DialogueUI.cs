using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Экран вариантов ответа в диалоге.
/// Показывает до трёх кнопок и автоматически создаётся компонентом диалога,
/// если в текущей сцене нет экземпляра <see cref="ScenePath"/>.
/// </summary>
public partial class DialogueUI : Control
{
    /// <summary>
    /// Путь к сцене этого UI.
    /// </summary>
    public const string ScenePath = "res://Game/Dialogue/Scene/DialogueUI.tscn";

    /// <summary>
    /// Текст кнопки для узла без ответов (конец диалога).
    /// </summary>
    [Export] public string ContinueText = "...";

    private static DialogueUI _instance;

    private readonly TextureButton[] _buttons = new TextureButton[3];
    private readonly Label[] _labels = new Label[3];
    [Export] public AudioStreamPlayer MouseEnteredSound;
    private Action<int> _onSelected;
    private bool _isOpen;

    // Активные анимации «вылета» и «плавания» — убиваем их при закрытии
    private readonly System.Collections.Generic.List<Tween> _animTweens = new();

    // Твин наведения на каждую кнопку — чтобы не плодить их при быстром mouse-in/out
    private readonly Tween[] _hoverTweens = new Tween[3];

    /// <summary>
    /// Существующий экземпляр UI или null.
    /// </summary>
    public static DialogueUI Instance =>
        _instance != null && GodotObject.IsInstanceValid(_instance) ? _instance : null;

    /// <inheritdoc/>
    public override void _Ready()
    {
        _instance = this;
        AddToGroup("DialogueUI");

        for (var i = 0; i < _buttons.Length; i++)
        {
            _buttons[i] = GetNode<TextureButton>($"HBoxContainer/Answer{i + 1}");
            _labels[i] = _buttons[i].GetNode<Label>("Label");

            var index = i;
            _buttons[i].Pressed += () => OnPressed(index);
            _buttons[i].MouseEntered += () => OnMouseEntered(index);
        _buttons[i].MouseExited += () => OnMouseExited(index);   // <-- новое
        }

        // Пивот для scale/rotation выставим, когда контейнер посчитает размеры
        CallDeferred(nameof(UpdatePivots));
        Visible = false;
    }

    /// <inheritdoc/>
    public override void _ExitTree()
    {
        if (_instance == this)
            _instance = null;
    }

    /// <summary>
    /// Возвращает существующий UI или создаёт его в текущей сцене.
    /// </summary>
    /// <param name="context">Любая нода, через которую доступно дерево сцены.</param>
    public static DialogueUI GetOrCreate(Node context)
    {
        if (Instance != null)
            return Instance;

        if (context?.GetTree() == null)
            return null;

        if (context.GetTree().GetFirstNodeInGroup("DialogueUI") is DialogueUI existing)
            return existing;

        var scene = GD.Load<PackedScene>(ScenePath);
        if (scene == null)
        {
            GD.PushError($"[Dialogue] Не найдена сцена UI: {ScenePath}");
            return null;
        }

        var layer = new CanvasLayer
        {
            Name = "DialogueUILayer",
            Layer = 100
        };

        var root = context.GetTree().CurrentScene ?? context.GetTree().Root;
        root.AddChild(layer);

        var ui = scene.Instantiate<DialogueUI>();
        layer.AddChild(ui);
        return ui;
    }

    /// <summary>
    /// Показывает варианты ответа. Лишние кнопки скрываются.
    /// </summary>
    /// <param name="answerTexts">Тексты кнопок (максимум 3).</param>
    /// <param name="onSelected">Колбэк с индексом выбранного варианта (-1 — отмена).</param>
    public void ShowAnswers(IReadOnlyList<string> answerTexts, Action<int> onSelected)
    {
        if (answerTexts == null || answerTexts.Count == 0)
        {
            Close();
            return;
        }

        _onSelected = onSelected;

        for (var i = 0; i < _buttons.Length; i++)
        {
            var hasAnswer = i < answerTexts.Count;
            _buttons[i].Visible = hasAnswer;

            if (hasAnswer)
                _labels[i].Text = answerTexts[i] ?? "";
        }

        _isOpen = true;
        Visible = true;

        StartAnimations();
    }

    /// <summary>
    /// Показывает одну кнопку продолжения (для реплики без ответов).
    /// </summary>
    public void ShowContinue(Action onSelected)
    {
        ShowAnswers(new[] { ContinueText }, _ => onSelected?.Invoke());
    }

    /// <summary>
    /// Скрывает UI и забывает текущий колбэк.
    /// </summary>
    public void Close()
    {
        KillAllAnimations();

        // Возвращаем кнопки в исходное состояние (после прерванных твинов)
        foreach (var btn in _buttons)
        {
            if (btn == null || !GodotObject.IsInstanceValid(btn))
                continue;
            btn.Scale = Vector2.One;
            btn.Rotation = 0;
            btn.Modulate = new Color(1, 1, 1, 1);
        }

        _onSelected = null;
        _isOpen = false;
        Visible = false;
    }

    /// <summary>
    /// Создаёт твин и кладёт его в список, чтобы убить при закрытии/перезапуске.
    /// </summary>
    private Tween TrackTween()
    {
        var t = CreateTween();
        _animTweens.Add(t);
        return t;
    }

    private void KillAllAnimations()
    {
        for (var i = 0; i < _hoverTweens.Length; i++)
        {
            _hoverTweens[i]?.Kill();
            _hoverTweens[i] = null;
        }

        foreach (var t in _animTweens)
            t?.Kill();
        _animTweens.Clear();
    }

    /// <inheritdoc/>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_isOpen || !@event.IsActionPressed("ui_cancel"))
            return;

        var callback = _onSelected;
        Close();
        callback?.Invoke(-1);
        GetViewport().SetInputAsHandled();
    }

    private void OnPressed(int index)
    {
        if (!_isOpen)
            return;

        var callback = _onSelected;
        Close();
        callback?.Invoke(index);
    }

    /// <summary>
    /// Ставит pivot_offset в центр каждой кнопки — нужно для масштабирования
    /// и вращения «вокруг себя», а не вокруг левого верхнего угла.
    /// </summary>
    private void UpdatePivots()
    {
        foreach (var btn in _buttons)
        {
            if (btn != null && GodotObject.IsInstanceValid(btn))
                btn.PivotOffset = btn.Size / 2f;
        }
    }

    private void OnMouseEntered(int index)
    {
        if (!_isOpen)
            return;

        var btn = _buttons[index];
        if (btn == null || !GodotObject.IsInstanceValid(btn))
            return;

        btn.PivotOffset = btn.Size / 2f;

        // Убиваем предыдущий твин этой кнопки, чтобы они не спорили
        _hoverTweens[index]?.Kill();

        var t = CreateTween();
        t.SetEase(Tween.EaseType.Out)
        .SetTrans(Tween.TransitionType.Back);
        t.TweenProperty(btn, "scale", Vector2.One * 1.1f, 0.15f);
        _hoverTweens[index] = t;

        MouseEnteredSound?.Play();
    }

    private void OnMouseExited(int index)
    {
        if (!_isOpen)
            return;

        var btn = _buttons[index];
        if (btn == null || !GodotObject.IsInstanceValid(btn))
            return;

        _hoverTweens[index]?.Kill();

        var t = CreateTween();
        t.SetEase(Tween.EaseType.Out)
        .SetTrans(Tween.TransitionType.Quad);
        t.TweenProperty(btn, "scale", Vector2.One, 0.15f);
        _hoverTweens[index] = t;
    }

    private void StartAnimations()
    {
        KillAllAnimations();
        // Ждём один кадр, чтобы HBoxContainer расставил кнопки —
        // только после этого мы знаем их настоящие basePos.
        CallDeferred(nameof(PlayIntroAnimations));
    }

    private void PlayIntroAnimations()
    {
        if (!_isOpen)
            return;

        for (var i = 0; i < _buttons.Length; i++)
        {
            var btn = _buttons[i];
            if (btn == null || !GodotObject.IsInstanceValid(btn) || !btn.Visible)
                continue;

            btn.PivotOffset = btn.Size / 2f;

            var basePos = btn.Position;    // куда приземлиться

            // Старт: ниже на 120 px и прозрачная
            btn.Position = basePos + new Vector2(0, 120);
            btn.Rotation = 0;
            btn.Modulate = new Color(1, 1, 1, 0);

            var flyIn = TrackTween();
            flyIn.SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Back);
            flyIn.TweenProperty(btn, "position", basePos, 0.5f)
                .SetDelay(i * 0.08f);
            flyIn.Parallel().TweenProperty(btn, "modulate:a", 1.0f, 0.3f)
                .SetDelay(i * 0.08f);

            // Как только долетела — стартуем «плавание»
            flyIn.TweenCallback(Callable.From(() => StartFloating(btn, basePos, i)));
        }
    }

    private void StartFloating(TextureButton btn, Vector2 basePos, int index)
    {
        if (!_isOpen || btn == null || !GodotObject.IsInstanceValid(btn))
            return;

        var t = TrackTween();
        t.SetLoops();
        t.SetEase(Tween.EaseType.InOut)
        .SetTrans(Tween.TransitionType.Sine);

        // Фаза 1 (вверх + наклон вправо)
        t.TweenProperty(btn, "position:y", basePos.Y - 8f, 1.5f + index * 0.1f);

        // Фаза 2 (вниз + наклон влево)
        t.TweenProperty(btn, "position:y", basePos.Y + 8f, 1.5f + index * 0.1f);

    }
}
