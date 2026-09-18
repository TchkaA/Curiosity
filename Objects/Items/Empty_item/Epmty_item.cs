using Godot;
using System;

public partial class Epmty_item : Item
{
    public override void Use(Node2D user)
    {
        GD.PrintErr("Empty obj");
    }
}
