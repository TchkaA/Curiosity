using Godot;

public partial class BaseNPC : BaseEntity, IInteractable
{
    public string NpcName;

    public DialogueComponent Dialogue;
    public SoundComponent Sound;

    public override void _Ready()
    {
        base._Ready();
        GD.Print("BaseNPC is ready");

        Dialogue = new(this);
        Sound = new(this);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void Interact(Node2D interactor)
    {
        Dialogue.Talk();
    }

    public void InteractEnter()
    {
        // TODO outline
    }

    public void InteractExit()
    {
        // TODO outline
    }

    
}