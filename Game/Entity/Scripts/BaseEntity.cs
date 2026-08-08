using Godot;

public partial class BaseEntity : CharacterBody2D, IDamagable, IDirectable
{
    public StatsComponent Stats { get; set; }


    public DirectionComponent directionComponent { get; set; }
    public AnimationComponent animationComponent;
    public AnimatedSprite2D Visual;
    
    public override void _Ready()
    {
        GD.Print("BaseEntity is ready");
        Stats = new StatsComponent();
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
    }



    public virtual void SetDirection()
    {
        // TODO
    }





}