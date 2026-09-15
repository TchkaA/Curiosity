using Godot;

public partial class DamageArea : Area2D
{
    [Export]
    public float Damage = 1;

    public override void _Ready()
    {
        this.BodyEntered += Entity_Entered;
    }

    public void Entity_Entered(Node2D node)
    {
       if (node is BaseEntity body)
        {
            body.TakeDamage(Damage);
        }
    }
}