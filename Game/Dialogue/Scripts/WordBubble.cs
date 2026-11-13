using Godot;
using System.Threading.Tasks;

/// <summary>
/// Всплывающий текстовый пузырь над говорящим существом.
/// Поддерживает печать текста по буквам и плавное исчезновение.
/// </summary>
public partial class WordBubble : Node2D
{
    private Label _label;

    /// <inheritdoc/>
    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
    }

    /// <summary>
    /// Показывает текст, затем плавно скрывает пузырь.
    /// </summary>
    public async Task ShowText(string text)
    {
        await TypeText(text);
        await FadeOutAndFree(0f);
    }

    /// <summary>
    /// Показывает текст мысли, затем плавно скрывает пузырь.
    /// </summary>
    public async Task ShowThought(string text)
    {
        await ShowText(text);
    }

    /// <summary>
    /// Показывает текст и оставляет пузырь на экране до вызова <see cref="FadeOutAndFree"/>.
    /// </summary>
    public async Task ShowDialog(string text)
    {
        await TypeText(text);
    }

    /// <summary>
    /// Печатает текст по буквам. Безопасно прекращает работу, если пузырь удалён.
    /// </summary>
    public async Task TypeText(string text)
    {
        if (_label == null || !GodotObject.IsInstanceValid(_label))
            return;

        _label.Text = "";

        foreach (var character in text ?? "")
        {
            if (!GodotObject.IsInstanceValid(this) || !GodotObject.IsInstanceValid(_label))
                return;

            _label.Text += character;

            var tree = GetTree();
            if (tree == null)
                return;

            await ToSignal(tree.CreateTimer(0.11f), "timeout");
        }
    }

    /// <summary>
    /// Ждёт delay секунд, затем плавно скрывает и удаляет пузырь.
    /// </summary>
    /// <param name="delay">Задержка перед исчезновением в секундах.</param>
    public async Task FadeOutAndFree(float delay = 1.2f)
    {
        var tree = GetTree();
        if (tree == null)
        {
            QueueFree();
            return;
        }

        if (delay > 0f)
            await ToSignal(tree.CreateTimer(delay), "timeout");

        if (!GodotObject.IsInstanceValid(this) || !IsInsideTree())
        {
            if (GodotObject.IsInstanceValid(this))
                QueueFree();
            return;
        }

        var tween = CreateTween();
        tween.TweenProperty(this, "position:y", Position.Y - 15f, 1f);
        tween.Parallel().TweenProperty(this, "modulate:a", 0f, 1f);
        tween.Finished += QueueFree;
    }
}
