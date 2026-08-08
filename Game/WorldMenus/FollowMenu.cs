using System.Collections.Generic;
using System.Reflection.Metadata;
using Godot;

public partial class FollowMenu : VBoxContainer
{
    private readonly List<ContextAnswer> _actions = new();
    private Tween _tween;

    public override void _Ready()
    {

        AddChild(new Button(){Text = "ыфафыа"});
        AddChild(new Button(){Text = "ыфафыа"});
        AddChild(new Button(){Text = "ыфафыа"});

        // Начальный масштаб – ноль (невидимо)
        Scale = Vector2.Zero;
        AddThemeConstantOverride("separation", 10);

        // Анимация появления (масштаб 0 → 1)
        _tween = CreateTween();
        _tween.TweenProperty(this, "scale", new Vector2(2f,2f), 0.3f)
              .SetTrans(Tween.TransitionType.Back)
              .SetEase(Tween.EaseType.Out);
    }

    public void Initialize(IEnumerable<ContextAnswer> actions)
    {
        _actions.Clear();
        foreach (var action in actions)
        {
            AddActionButton(action);
        }
    }

    private void AddActionButton(ContextAnswer action)
    {
        var button = new Button();
        button.Text = action.Answer;
        button.Pressed += () => OnActionButtonPressed(action);
        AddChild(button);
    }

    private void OnActionButtonPressed(ContextAnswer action)
    {
        action.Callback?.Invoke();
        QueueFree(); // меню закрывается после выбора
    }
}