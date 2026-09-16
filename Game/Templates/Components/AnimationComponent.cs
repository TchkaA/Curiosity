using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Godot;

public class AnimationComponent
{
    public bool isChanged;

    private BaseEntity _owner;
    private AnimatedSprite2D _sprite;

    public string Direction = "down";
    public string Animation = "idle";


    
    public AnimationComponent(BaseEntity owner, AnimatedSprite2D sprite)
    {
        _owner = owner;
        _sprite = sprite;
    }

    public void Update()
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


    public async Task PlayAndWaitAsync(string anim)
    {
        Animation = anim;
        isChanged = true;
        
        var tcs = new TaskCompletionSource<bool>();
        
        _sprite.Play(AnimationRequest());
        _sprite.AnimationFinished += OnAnimationFinished;
        
        void OnAnimationFinished()
        {
            _sprite.AnimationFinished -= OnAnimationFinished;
            tcs.SetResult(true);
        }
        
        await tcs.Task;
    }

    public void NotifyAnimation(DirectionComponent.FacingDirection direction)
    {
        switch (direction)
        {
            case DirectionComponent.FacingDirection.Up:
                SetDirection("up");
                break;
            case DirectionComponent.FacingDirection.Down:
                SetDirection("down");
                break;
            case DirectionComponent.FacingDirection.Left:
                SetDirection("left");
                break;
            case DirectionComponent.FacingDirection.Right:
                SetDirection("right");
                break;
            default:
                GD.PrintErr("Invalid direction string");
                SetDirection("down");
                break;
        }
    }

}