using Godot;
using System;

/// <summary>
/// Панель выброса предметов с выбором количества.
/// </summary>
[GlobalClass]
public partial class DropPanel : GamePanel
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
    /// Отмена выбора. Срабатывает при закрытии панели без подтверждения.
    /// </summary>
    public event Action Canceled;

    protected override void OnReady()
    {
        if (ConfirmButton != null)
            ConfirmButton.Pressed += OnConfirmPressed;

        if (CancelButton != null)
            CancelButton.Pressed += OnCancelPressed;

        if (CountSlider != null)
            CountSlider.ValueChanged += OnValueChanged;
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
    /// Показывает панель и настраивает диапазон выбора.
    /// </summary>
    /// <param name="title">Заголовок панели.</param>
    /// <param name="maxCount">Максимальное доступное количество.</param>
    /// <param name="initialCount">Начальное значение слайдера.</param>
    public void Ask(string title, int maxCount, int initialCount = 1)
    {
        maxCount = Math.Max(1, maxCount);
        initialCount = Math.Clamp(initialCount, 1, maxCount);

        if (TitleLabel != null && !string.IsNullOrWhiteSpace(title))
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

    private void OnConfirmPressed()
    {
        int count = CountSlider != null
            ? Mathf.RoundToInt((float)CountSlider.Value)
            : 1;

        HidePanel();
        Confirmed?.Invoke(count);
    }

    private void OnCancelPressed()
    {
        Close();
    }

    protected override void OnClosed()
    {
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
