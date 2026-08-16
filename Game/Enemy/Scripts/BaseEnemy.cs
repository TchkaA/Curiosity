using Godot;
using System;

public partial class BaseEnemy : BaseEntity
{
    
    // public 
    public CombatComponent combat;

    public override void _Ready()
    {
        combat = new(this);
    }




}
