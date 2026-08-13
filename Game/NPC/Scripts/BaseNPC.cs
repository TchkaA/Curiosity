using System.Collections.Generic;
using Godot;
using NPC.StateMachine;

public partial class BaseNPC : BaseEntity, IInteractable, IContextMenuProvider, ISpeakable
{
    public StateMachine stateMachine;
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
    public bool InInteraction = false;

    public ContextMenuComponent contextMenu;

    private Vector2 _bubbleSize = new Vector2(0.25f,0.25f);

    public NavigationComponent Navigation
    {
        get { return navigation; }
    }

    public override void _Ready()
    {
        base._Ready();

        stateMachine = new StateMachine();


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
    base._PhysicsProcess(delta);
    
    stateMachine.Update(delta);
    navigation.Update();
}

    public void Interact(BaseEntity interactor)
    {
        Dialogue.Talk(interactor);
        Camera.Instance.AddTarget(this);
        Camera.Instance.AddZoom(new Vector2(1.5f,1.5f));
        InInteraction= true;
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


    public IEnumerable<ContextAction> GetContextAction(BaseEntity interactor)
	{
		yield return new ContextAction("Взаимодействовать", () => Interact(interactor));
		yield return new ContextAction("Осмотреть", () => GD.Print($"Осмотр: {NpcName}"));
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
        // Заглушки — диалоговая система будет переделана отдельным MR.
        yield return new ContextAnswer("Сказать правду", () => GD.Print("Игрок: правда"));
		yield return new ContextAnswer("Солгать", () => GD.Print("Игрок: ложь"));
    }


    public override void _ExitTree()
    {
        Camera.Instance.RemoveTarget(this);
        Camera.Instance.RemoveZoom();

        base._ExitTree();
    }

}