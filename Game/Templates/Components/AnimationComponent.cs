using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Godot;

public partial class AnimationComponent : AnimatedSprite2D
{
    /// <summary>
    /// Анимации существа
    /// </summary>
    public SpriteFrames EntityAnimation = GD.Load<SpriteFrames>("res://Game/Templates/BaseEntity/Resourses/BaseAnimations.tres");
    /// <summary>
    /// Анимации оружия
    /// </summary>
    public SpriteFrames WeaponAnimation;
    /// <summary>
    /// Анимации выбранного пути
    /// </summary>
    public SpriteFrames PathAnimations;


    public bool isChanged;

    private BaseEntity _owner;

    public string Direction = "down";
    public string CurrentAnimation = "idle";


    
    public override void _Ready()
    {
        _owner = GetParent<BaseEntity>();
    }

    public override void _Process(double delta)
    {
        if (isChanged == true)
        {
            Play(AnimationRequest());
            isChanged = false;
        }
    }

    public void ChangeAnimation(SpriteFrames spriteFrames) => SpriteFrames = spriteFrames;

    public string AnimationRequest()
    {
        string request;
        switch (Direction)
        {
            case "left":
                FlipH = true;
                request = $"{CurrentAnimation}_forward".ToLower();
                break;
            case "right":
                FlipH = false;
                request = $"{CurrentAnimation}_forward".ToLower();
                break;
            default:
                request = $"{CurrentAnimation}_{Direction}".ToLower();
                break;
        }

        // Анимации без направлений (например, "dash") играются как есть.
        return SpriteFrames.HasAnimation(request) ? request : CurrentAnimation.ToLower();
    }

    public void SetDirection(string dir)
    {
        Direction = dir;
        isChanged = true;
    }

    public void SetAnimation(string anim)
    {
        CurrentAnimation = anim;
        isChanged = true;
    }


    public async Task PlayAndWaitAsync(string anim)
    {
        CurrentAnimation = anim;
        isChanged = true;
        
        var tcs = new TaskCompletionSource<bool>();
        
        Play(AnimationRequest());
        AnimationFinished += OnAnimationFinished;
        
        void OnAnimationFinished()
        {
            AnimationFinished -= OnAnimationFinished;
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

    #region WeaponAnimations
    public void PlayWeaponAnimation(string Animation)
    {
        ChangeAnimation(WeaponAnimation);
        SetAnimation(Animation);
        ChangeAnimation(EntityAnimation); 
    }
    #endregion
}