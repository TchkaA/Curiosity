using System.Collections.Generic;
using System.Drawing;
using Godot;
using NPC.StateMachine;

public partial class BaseNPC : BaseEntity, IInteractable, IContextMenuProvider, ISpeekable
{
    public StateMachine stateMachine = new StateMachine();
    public string NpcName;

    //---------------------
    //      Компоненты
    //---------------------
    public DialogueComponent Dialogue;
    public SoundComponent Sound;
    public MovementComponent movement;
    public NavigationComponent Navigation;
    private VisionComponent vision;
    public ContextMenuComponent contextMenu;
    //----------------------

    public Shader shader => MainManager.Instance.OutlineShader;
    public ShaderMaterial material;

    [Export]
    public NavigationAgent2D agent;

    
    //------------------------
    //      States
    //------------------------
    public NPCFollowState FollowState;
    public NPCIdleState IdleState;
    public NPCDied DiedState;
    //------------------------
    
    private bool _isInRange = false;
    public bool InInteraction = false;

    

    private Vector2 _bubbleSize = new Vector2(0.25f,0.25f);

    public override void _Ready()
    {
        base._Ready();

        InitComponents();

        InitStates();

        InitShader();

        stateMachine.ChangeState(IdleState);

        InputPickable = true;
		InputEvent += OnInputEvent;
    }

    public override void _PhysicsProcess(double delta)
    {

        base._Process(delta);
        stateMachine.Update(delta);
        Navigation.Update();
        vision.Update();
    }

    public void Interact(BaseEntity interactor)
    {
        if(InDialogue) return;
        
        Camera.Instance.AddTarget(this);
        Camera.Instance.AddZoom(new Vector2(1.5f,1.5f));
        InInteraction= true;
        Dialogue.Talk(interactor);
        //TODO
        if(interactor is Player pl)
        {
            pl.stateMachine.ChangeState(pl.interactionState);
        }
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
        Camera.Instance.RemoveTarget(this);
        Camera.Instance.RemoveZoom();
        InInteraction= false;
	}


	private void InitShader()
	{
		material = new ShaderMaterial();
        material.Shader = shader;
		material.SetShaderParameter("outline_size", 0f);
        Visual.Material = material;
	}

    public void InitStates()
    {
        FollowState = new(this);
        IdleState = new(this);
        DiedState = new(this);
    }


    public IEnumerable<ContextAction> GetContextAction(BaseEntity interactor)
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

    public IEnumerable<ContextAnswer> GetAnswers(Node2D interactor)
    {
        yield return new ContextAnswer("Сказать правду", () => GD.Print("slabak"));
		yield return new ContextAnswer("Солгать", () => GD.Print("Horosh bratan"));
    }


    public override void _ExitTree()
    {
        Camera.Instance.RemoveTarget(this);
        Camera.Instance.RemoveZoom();

        base._ExitTree();
    }

    public virtual void InitComponents()
    {
        Dialogue = new(this);
        Sound = new(this);
        Navigation = new(this,agent);
        movement = new(this);
        vision = new(this);

        contextMenu = new(this, GetContextAction(MainManager.Instance.Player));
    }

}