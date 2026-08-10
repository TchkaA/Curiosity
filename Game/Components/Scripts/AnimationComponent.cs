using Godot;

public class AnimationComponent
{
    private readonly AnimatedSprite2D _sprite;

    private string _prefix = string.Empty;
    private string _animation = "idle";
    private string _direction = "down";

    private bool _dirty = true;

    public AnimationComponent(AnimatedSprite2D sprite)
    {
        _sprite = sprite;
    }

    public void SetPrefix(string prefix)
    {
        prefix ??= string.Empty;

        if (_prefix == prefix)
            return;

        _prefix = prefix;
        _dirty = true;
    }

    public void SetAnimation(string animation)
    {
        if (_animation == animation)
            return;

        _animation = animation;
        _dirty = true;
    }

    public void SetDirection(string direction)
    {
        direction ??= "down";

        if (_direction == direction)
            return;

        _direction = direction;
        _dirty = true;
    }

    public void Update()
    {
        if (!_dirty)
            return;

        _dirty = false;

        UpdateFlip();
        _sprite.Play(BuildAnimationName());
    }

    private string BuildAnimationName()
    {
        string direction = _direction;

        if (direction == "left" || direction == "right")
            direction = "forward";

        if (string.IsNullOrEmpty(_prefix))
            return $"{_animation}_{direction}".ToLower();

        return $"{_prefix}_{_animation}_{direction}".ToLower();
    }

    private void UpdateFlip()
    {
        _sprite.FlipH = _direction == "left";
    }
}