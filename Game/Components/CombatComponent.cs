// CombatComponent.cs
using System;
using System.Threading.Tasks;
using Godot;

public class CombatComponent
{
    private const float ATTACK_SIZE = 12f;
    private const float ATTACK_OFFSET = 20f;
    private const int BASE_DAMAGE = 20;
    private const float ATTACK_DURATION = 0.3f; // Длительность активной зоны атаки
    
    private BaseEntity _owner;
    private Area2D _attackArea;
    private CollisionShape2D _collisionShape;
    
    public CombatComponent(BaseEntity owner)
    {
        _owner = owner;
    }
    
    private void SetupAttackArea()
    {
        _attackArea = new Area2D();
        _collisionShape = new CollisionShape2D();
        _collisionShape.Shape = new CircleShape2D() { Radius = ATTACK_SIZE };
        
        _attackArea.AddChild(_collisionShape);
        _attackArea.BodyEntered += OnAttackAreaBodyEntered;
        
        // Добавляем Area2D в дерево сцены
        _owner.AddChild(_attackArea);
        
        // Устанавливаем начальную позицию
        UpdateAttackAreaPosition();
    }
    
    public async Task PerformAttack()
    {
        SetupAttackArea();
        
        // Ждем длительность атаки
        await Task.Delay(TimeSpan.FromSeconds(ATTACK_DURATION));
        
        Cleanup();
    }
    
    private void UpdateAttackAreaPosition()
    {
        if (_owner?.directionComponent == null) return;
        
        Vector2 offset = GetDirectionOffset(_owner.directionComponent.CurrentDirection);
        _attackArea.Position = offset;
    }
    
    private Vector2 GetDirectionOffset(DirectionComponent.FacingDirection direction)
    {
        return direction switch
        {
            DirectionComponent.FacingDirection.Up => new Vector2(0, -ATTACK_OFFSET),
            DirectionComponent.FacingDirection.Down => new Vector2(0, ATTACK_OFFSET),
            DirectionComponent.FacingDirection.Left => new Vector2(-ATTACK_OFFSET, 0),
            DirectionComponent.FacingDirection.Right => new Vector2(ATTACK_OFFSET, 0),
            _ => Vector2.Zero
        };
    }
    
    private void OnAttackAreaBodyEntered(Node2D body)
    {
        if (body is IDamagable damageable && body != _owner)
        {
            damageable.TakeDamage(BASE_DAMAGE);
        }
    }
    
    public void Cleanup()
    {
        if (_attackArea != null)
        {
            _attackArea.BodyEntered -= OnAttackAreaBodyEntered;
            _attackArea.QueueFree();
            _attackArea = null;
        }
    }
}