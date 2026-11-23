using Godot;
using System;

/// <summary>
/// Модальное окно подтверждения с выбором количества.
/// Используется для выброса предметов из инвентаря.
/// </summary>
[GlobalClass]
public partial class ActionMessage : Control
{
    [Export] public Label TitleLabel;
    [Export] public Label CountLabel;
    [Export] public HSlider CountSlider;
    [Export] public Button ConfirmButton;
    [Export] public Button CancelButton;

    /// <summary>
    /// Подтверждение выбора. Передаёт выбранное количество.
    /// </summary>
    public event Action<int> Confirmed;

    /// <summary>
    /// Отмена выбора.
    /// </summary>
    public event Action Canceled;

    public override void _Ready()
    {
        if (ConfirmButton != null)
            ConfirmButton.Pressed += OnConfirmPressed;

        if (CancelButton != null)
            CancelButton.Pressed += OnCancelPressed;

        if (CountSlider != null)
            CountSlider.ValueChanged += OnValueChanged;

        Hide();
    }

    public override void _ExitTree()
    {
        if (ConfirmButton != null)
            ConfirmButton.Pressed -= OnConfirmPressed;

        if (CancelButton != null)
            CancelButton.Pressed -= OnCancelPressed;

        if (CountSlider != null)
            CountSlider.ValueChanged -= OnValueChanged;
    }

    /// <summary>
    /// Показывает окно и настраивает диапазон выбора.
    /// </summary>
    /// <param name="title">Заголовок окна.</param>
    /// <param name="maxCount">Максимальное доступное количество.</param>
    /// <param name="initialCount">Начальное значение слайдера.</param>
    public void Ask(string title, int maxCount, int initialCount = 1)
    {
        maxCount = Math.Max(1, maxCount);
        initialCount = Math.Clamp(initialCount, 1, maxCount);

        if (TitleLabel != null)
            TitleLabel.Text = title;

        if (CountSlider != null)
        {
            CountSlider.MinValue = 1;
            CountSlider.MaxValue = maxCount;
            CountSlider.Step = 1;
            CountSlider.Value = initialCount;
        }

        UpdateCountLabel();
        Show();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Visible || !@event.IsActionPressed("ui_cancel"))
            return;

        Hide();
        Canceled?.Invoke();
        GetViewport().SetInputAsHandled();
    }

    private void OnConfirmPressed()
    {
        int count = CountSlider != null
            ? Mathf.RoundToInt((float)CountSlider.Value)
            : 1;

        Hide();
        Confirmed?.Invoke(count);
    }

    private void OnCancelPressed()
    {
        Hide();
        Canceled?.Invoke();
    }

    private void OnValueChanged(double value)
    {
        UpdateCountLabel();
    }

    private void UpdateCountLabel()
    {
        if (CountLabel == null || CountSlider == null)
            return;

        CountLabel.Text = $"x{Mathf.RoundToInt((float)CountSlider.Value)}";
    }
}
