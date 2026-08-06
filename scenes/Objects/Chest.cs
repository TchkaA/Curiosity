using Godot;
using System;

public partial class Chest : InteractableObject
{
    [Export]
    public SpriteFrames spriteFrames;

    public Inventory Inventory;

    public override void Interact(Node2D interactor)
    {
        base.Interact(interactor);
    }

}
