using Godot;
using System;

public partial class PickUp : InteractableObject
{
	public override void _Ready()
	{
		base._Ready();
	}

    public override void Interact()
    {
		GD.Print("Picked up");
		QueueFree();
    }

}
