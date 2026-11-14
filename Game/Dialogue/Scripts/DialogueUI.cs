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
        }

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
        _onSelected = null;
        _isOpen = false;
        Visible = false;
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

    private void OnMouseEntered(int index)
    {
        if (!_isOpen)
            return;
        
        MouseEnteredSound.Play();
    }

    private void StartAnimations()
    {
        
    }
}
