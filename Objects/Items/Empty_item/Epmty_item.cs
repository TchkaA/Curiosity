using Godot;
using System;

public partial class Epmty_item : Item
{
    [Export]
    public string Name = "Empty_item";

    [Export]
    public int ID = 1;

    public override void Use(Node2D user)
    {
        GD.PrintErr("Empty obj");
    }
}
