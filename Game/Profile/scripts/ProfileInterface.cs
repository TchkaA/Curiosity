using Godot;
using System;

public partial class ProfileInterface : Node
{
    [Export] public Label Health;
    [Export] public Label Stamina;

    public override void _Ready()
    {
        base._Ready();
        
    }


    public void ChangeHealthUI(float health)
    {
        Health.Text = $"{health}";
    }
    

}
