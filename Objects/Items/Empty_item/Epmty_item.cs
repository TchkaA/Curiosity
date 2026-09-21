using Godot;
using System;

public partial class Epmty_item : Item
{
    public override bool Use(Node2D user)
    {
        GD.PrintErr("Empty obj");
        return false;
    }
}
