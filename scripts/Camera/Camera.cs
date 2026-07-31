using Godot;
using System;
using System.Collections.Generic;

public partial class Camera : Camera2D
{
    private List<Node2D> _targets = new();

    private Player _player;

    [Export]
    private float _followSpeed = 5.0f;

    public override void _Ready()
    {
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
}