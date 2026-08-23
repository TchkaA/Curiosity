using Godot;

public partial class BaseEntity : CharacterBody2D, IDamagable, IDirectable
{
    public StatsComponent Stats { get; set; }
    public Relation CurrentRelation;

    public DirectionComponent directionComponent { get; set; }
    public AnimationComponent animationComponent;
    public AnimatedSprite2D Visual;
    
    /*
        --------------------------------
        ----------- Флаги --------------
        --------------------------------
    */
    public bool InDialogue { get; set; } 
    public bool IsAttacking { get; set; }
    // ---------------------------------
    
    public BaseEntity(Relation relation = Relation.Neutral)
    {
        CurrentRelation = relation;
    }



    public override void _Ready()
    {
        Stats = new StatsComponent(this);
        // Initialize the direction component
        directionComponent = new DirectionComponent(this);

        Visual = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        if (Visual != null)
        {
            animationComponent = new AnimationComponent(this, Visual);
        }

        
    }
    
    public override void _Process(double delta)
    {
        // directionComponent?.UpdateDirection();
        animationComponent?.UpdateAnimation();
    }


    public virtual void TakeDamage(int damage)
    {
        Stats.TakeDamage(damage);
        GD.Print(this + " - took damage");
    }


    public void SetDirection(Vector2 direction)
    {
        directionComponent.TurnTo(direction);
    }
    public virtual void Die()
    {
        // QueueFree();
    }
}

public enum Relation
{
    Hostile,
    Peaceful,
    Neutral 
}