using System.Collections.Generic;
using Godot;

public class FollowMenuComponent
{
    private static FollowMenu _followMenu;
    private BaseEntity _owner;
    public bool IsMenuOpen = false;

    [ExportGroup("Константы")]
    [Export]
    public const float FOLLOW_SPEED = 5.0f;      // скорость интерполяции (чем больше, тем быстрее)
    [Export]
    public const float OFFSET_DISTANCE = 80.0f;  // расстояние от игрока до меню

    private Vector2 _previousDirection = new Vector2(OFFSET_DISTANCE, 0);

    public FollowMenuComponent(BaseEntity owner)
    {
        _owner = owner;
    }

    public void OpenMenu()
    {
        if (IsMenuOpen) return;

        var menu = new FollowMenu();
        _owner.GetTree().CurrentScene.AddChild(menu);
        menu.GlobalPosition = _owner.GlobalPosition;
        IsMenuOpen = true;
        _followMenu = menu;

        // Подписка на удаление меню
        _followMenu.TreeExited += OnFollowMenuTreeExited;
    }

    public void CloseMenu()
    {
        if (_followMenu != null)
        {
            _followMenu.TreeExited -= OnFollowMenuTreeExited;
            _followMenu.QueueFree();
            _followMenu = null;
        }
        IsMenuOpen = false;
        
    }

    public void Update(double delta)
    {
        if (!IsMenuOpen || _followMenu == null) return;

        UpdateMenuPosition(instant: false, delta);
    }


    private void UpdateMenuPosition(bool instant, double delta = 0f)
    {
        if (_followMenu == null) return;

        Vector2 ownerPos = _owner.GlobalPosition;
        var dir = _owner.directionComponent.CurrentDirection;

        Vector2 offset = GetOffsetFromDirection(dir);

        Vector2 menuSize = _followMenu.Size;
        Vector2 targetPos = ownerPos + offset - menuSize;

        if (instant)
        {
            _followMenu.GlobalPosition = targetPos;
        }
        else
        {
            _followMenu.GlobalPosition = _followMenu.GlobalPosition.Lerp(targetPos, FOLLOW_SPEED * (float)delta);
        }

    }

    private DirectionComponent.FacingDirection GetOwnerDirection()
    {
        if (_owner is IDirectable hasDirection)
            return hasDirection.directionComponent.CurrentDirection;

        return DirectionComponent.FacingDirection.Down;
    }

    private Vector2 GetOffsetFromDirection(DirectionComponent.FacingDirection dir)
    {
        switch (dir)
        {
            case DirectionComponent.FacingDirection.Left:  return _previousDirection = new Vector2(OFFSET_DISTANCE, 0);
            case DirectionComponent.FacingDirection.Right: return _previousDirection = new Vector2(-OFFSET_DISTANCE, 0);
            default: return _previousDirection;
        }
    }

    public void Initialize(IEnumerable<ContextAnswer> actions)
    {
        _followMenu.Initialize(actions);
    }

    private void OnFollowMenuTreeExited()
    {
        // Меню было удалено (например, через QueueFree)
        _followMenu = null;
        IsMenuOpen = false;
        
    }

}