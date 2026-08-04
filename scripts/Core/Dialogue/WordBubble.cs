using Godot;
using System.Threading.Tasks;

public partial class WordBubble : Node2D
{
    private Label _label;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
    }

    public async Task ShowText(string text)
    {
        await TypeText(text);

        var tween = CreateTween();

        tween.TweenProperty(
            this,
            "position:y",
            Position.Y - 20,
            1.0f
        );

        tween.Parallel().TweenProperty(
            this,
            "modulate:a",
            0.0f,
            1.0f
        );

        tween.Finished += QueueFree;
    }

    public async Task ShowThought(string text)
    {
        await TypeText(text);

        var tween = CreateTween();

        tween.TweenProperty(
            this,
            "position:y",
            Position.Y - 20,
            1.0f
        );

        tween.Parallel().TweenProperty(
            this,
            "modulate:a",
            0.0f,
            1.0f
        );

        tween.Finished += QueueFree;
    }

    public async Task ShowDialog(string text)
    {
        await TypeText(text);
    }



    public async Task TypeText(string text)
    {
        _label.Text = "";

        foreach (char c in text)
        {
            _label.Text += c;

            await ToSignal(
                GetTree().CreateTimer(0.08f),
                "timeout"
            );
        }
    }
}