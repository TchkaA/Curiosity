using Godot;

public partial class BaseEntity : CharacterBody2D, IDamagable, IDirectable
{
    public StatsComponent Stats { get; set; }

    public DirectionComponent directionComponent { get; set; }

    public AnimationComponent Animation;

    [Export]
    public AnimatedSprite2D Visual;

    public override void _Ready()
    {
        GD.Print("BaseEntity is ready");

        Stats = new StatsComponent();

        if (Visual == null)
            Visual = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        if (Visual != null)
        {
            Animation = new AnimationComponent(Visual);
        }
        else
        {
            GD.PrintErr($"{GetType().Name} '{Name}' has no AnimatedSprite2D for AnimationComponent.");
        }

        directionComponent = new DirectionComponent(this);
        directionComponent.DirectionChanged += direction => Animation?.SetDirection(direction);
    }

    public override void _Process(double delta)
    {
        Animation?.Update();
    }

    public virtual void TakeDamage(int damage)
    {
        Stats.TakeDamage(damage);
    }

    public virtual void SetDirection()
    {
        // TODO: если интерфейс IDirectable требует конкретную логику,
        // можно вызывать directionComponent.SetDirection(...)
    }
}