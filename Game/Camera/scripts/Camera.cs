using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Класс основной и единственной камеры в игре.
/// Отвечает за зум, положение, следование и тд.
/// </summary>
public partial class Camera : Camera2D
{
    private List<Node2D> _targets = new();

    private Player _player;
    public static Camera Instance;

    [Export]
    private float _followSpeed = 5.0f;

    [Export]
    private Vector2 _zoom = Vector2.One;
    private Tween _tween;

    public override void _Ready()
    {
        Instance = this;
        _player = GetTree().GetFirstNodeInGroup("Player") as Player;

        if (_player != null)
        {
            _targets.Add(_player);
        }
    }

    public override void _Process(double delta)
    {
        FollowTargets(delta);
    }

    private void FollowTargets(double delta)
    {
        if (_targets.Count == 0)
        {
            return;
        }

        Vector2 targetPosition;

        if (_targets.Count == 1)
        {
            targetPosition = _targets[0].GlobalPosition;
        }
        else
        {
            targetPosition = GetAveragePosition();
        }

        GlobalPosition = GlobalPosition.Lerp(
            targetPosition,
            _followSpeed * (float)delta
        );
    }

    private Vector2 GetAveragePosition()
    {
        Vector2 averagePosition = Vector2.Zero;

        foreach (Node2D target in _targets)
        {
            averagePosition += target.GlobalPosition;
        }

        return averagePosition / _targets.Count;
    }

    public void AddTarget(Node2D target)
    {
        if (!_targets.Contains(target))
        {
            _targets.Add(target);
        }
    }

    public void RemoveTarget(Node2D target)
    {
        _targets.Remove(target);
    }

    public void AddZoom(Vector2 zoomVect)
    {
        _tween = CreateTween();
        _tween.TweenProperty(this, "zoom", zoomVect, 0.3f).SetTrans(Tween.TransitionType.Linear);
    }

    public void RemoveZoom()
    {
        _tween = CreateTween();
        _tween.TweenProperty(this, "zoom", _zoom, 0.3f).SetTrans(Tween.TransitionType.Linear);
    }
}