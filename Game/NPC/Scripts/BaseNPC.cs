using System.Collections.Generic;
using System.Drawing;
using Godot;
using NPC.StateMachine;

public partial class BaseNPC : BaseEntity, IInteractable, IContextMenuProvider
{
    public StateMachine stateMachine = new StateMachine();
    public string NpcName;

    public DialogueComponent Dialogue;
    public SoundComponent Sound;

    public Shader shader => MainManager.Instance.OutlineShader;
    public ShaderMaterial material;

    [Export]
    public NavigationAgent2D agent;

    public MovementComponent movement;

    private NavigationComponent navigation;
    public NPCFollowState FollowState;
    public NPCIdleState IdleState;
    private bool _isInRange = false;

    public ContextMenuComponent contextMenu;

    private Vector2 _bubbleSize = new Vector2(0.25f,0.25f);

    public NavigationComponent Navigation
    {
        get { return navigation; }
    }

    public override void _Ready()
    {
        base._Ready();
        GD.Print("BaseNPC is ready");

        Dialogue = new(this);
        Sound = new(this);
        navigation = new(this,agent);
        movement = new(this);

        FollowState = new(this);
        IdleState = new(this);

        contextMenu = new(this, GetContextAction(MainManager.Instance.Player));

        InitShader();

        stateMachine.ChangeState(IdleState);

        InputPickable = true;
		InputEvent += OnInputEvent;
    }

    public override void _PhysicsProcess(double delta)
    {

        base._Process(delta);
        stateMachine.Update(delta);
        navigation.Update();
        
    }

    public void Interact(Node2D interactor)
    {
        Dialogue.Talk();
        
        // Указываем навигации двигаться к текущей позиции игрока
        navigation.GoTo(MainManager.Instance.Player.GlobalPosition);
        
        // Переводим NPC в состояние следования
        stateMachine.ChangeState(FollowState);
    }

    public virtual void InteractEnter()
    {
        _isInRange = true;
        if (material == null)
            return;

        material.SetShaderParameter("outline_size", 0.5f);
    }

    public virtual void InteractExit()
	{
        _isInRange = false;
		if (material == null)
			return;

		material.SetShaderParameter("outline_size", 0f);
        contextMenu.CloseContextMenu();
	}


	private void InitShader()
	{
		material = new ShaderMaterial();
        material.Shader = shader;
		material.SetShaderParameter("outline_size", 0f);
        Visual.Material = material;
	}


    public IEnumerable<ContextAction> GetContextAction(Node2D interactor)
	{
		yield return new ContextAction("Взаимодействовать", () => Interact(interactor));
		yield return new ContextAction("Осмотреть", () => GD.Print("Ne bratan"));
	}

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (_isInRange && @event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Right })
		{
			contextMenu.ShowContextMenu(_bubbleSize);
			// Важно: без этого тот же клик долетит до _UnhandledInput и сразу закроет
			// меню, которое мы только что открыли.
			GetViewport().SetInputAsHandled();
		}
	}


    public override void _UnhandledInput(InputEvent @event)
	{
        if (IsInstanceValid(ContextMenuComponent.activeContextMenu) && @event is InputEventMouseButton { Pressed: true })
		{
			contextMenu.CloseContextMenu();
		}
	}
	
    
}