using Godot;

public partial class DialogueComponent
{
    [Export]
    private string _speaker;

    public string DialogueText;
    private Node2D _owner;

    public DialogueComponent(Node2D owner)
    {
        _owner = owner;
    }

    public void Talk()
    {
        MakeSound("Че надо?", 0);
    }


    // TODO:
    // BubbleFactory / DialogueUI later
    public void MakeSound(string text, int act = 0)
    {
        var bubble = GD.Load<PackedScene>("res://Game/Dialogue/word_bubble.tscn").Instantiate<WordBubble>();
        _owner.GetTree().CurrentScene.AddChild(bubble);
        bubble.GlobalPosition = _owner.GlobalPosition + new Vector2(-20, -90);
        switch (act)
        {
            case 0: bubble.ShowText(text); break;
            case 1: bubble.ShowDialog(text); break;
            case 2: bubble.ShowThought(text); break;
        }
    }
}