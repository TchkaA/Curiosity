using Godot;

/// <summary>
/// Базовый класс модальных панелей.
/// Отвечает за показ, скрытие и закрытие по <c>Esc</c>.
/// </summary>
[GlobalClass]
public abstract partial class GamePanel : Control
{
    public override void _Ready()
    {
        Hide();
        OnReady();
    }

    /// <summary>
    /// Инициализация наследника. Вызывается после скрытия панели.
    /// </summary>
    protected virtual void OnReady()
    {
    }

    /// <summary>
    /// Скрывает панель без вызова <see cref="OnClosed"/>.
    /// </summary>
    public void HidePanel()
    {
        Hide();
    }

    /// <summary>
    /// Закрывает панель и сообщает наследнику о закрытии.
    /// </summary>
    public virtual void Close()
    {
        HidePanel();
        OnClosed();
    }

    /// <summary>
    /// Реакция наследника на закрытие.
    /// </summary>
    protected virtual void OnClosed()
    {
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Visible || !@event.IsActionPressed("ui_cancel"))
            return;

        Close();
        GetViewport().SetInputAsHandled();
    }
}
