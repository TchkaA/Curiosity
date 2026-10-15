using Godot;
using System;

public partial class ProfileInterface : Node
{
    [Export] public Label Health;
    [Export] public Label Stamina;
    

    public void ChangeHealthUI(float health)
    {
        Health.Text = $"Health - {health}";
    }
    

}
