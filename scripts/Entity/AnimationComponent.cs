using System.Runtime.InteropServices;
using System.Security;
using Godot;

public class AnimationComponent
{
    public bool isChanged;
    private AnimatedSprite2D _sprite;

    public string Direction = "down";
    public string Animation = "idle";
    public AnimationComponent(BaseEntity owner, AnimatedSprite2D sprite)
    {
        _sprite = sprite;
    }

    public void UpdateAnimation()
    {
        if (isChanged == true)
        {
            _sprite.Play(AnimationRequest());
            isChanged = false;
        }
    }

    public string AnimationRequest()
    {
        switch (Direction)
        {
            case "left":
                _sprite.FlipH = true;
                return $"{Animation}_forward".ToLower();
            case "right":
                _sprite.FlipH = false;
                return $"{Animation}_forward".ToLower();
            default:
                return $"{Animation}_{Direction}".ToLower();
        }
        
    }

    public void SetDirection(string dir)
    {
        Direction = dir;
        isChanged = true;
    }

    public void SetAnimation(string anim)
    {
        Animation = anim;
        isChanged = true;
    }



}