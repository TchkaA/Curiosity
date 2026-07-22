using Godot;

public partial class BaseEntity : CharacterBody2D, IDamagable, IInteractable, IDirectable
{
    public StatsComponent Stats { get; set; }
    public DirectionComponent directionComponent;
    
    public override void _Ready()
    {
        GD.Print("BaseEntity is ready");
        Stats = new StatsComponent();

        // Initialize the direction component
        directionComponent = new DirectionComponent(this);

    }
    
    public override void _Process(double delta)
    {
        // directionComponent?.UpdateDirection();
    }


    public virtual void TakeDamage(int damage)
    {
        Stats.TakeDamage(damage);
    }

    public virtual void Interact()
    {
        // TODO
    }

    public virtual void InteractEnter()
    {
        // TODO
    }

    public virtual void InteractExit()
    {
        // TODO
    }


    public virtual void SetDirection()
    {
        // TODO
    }



}