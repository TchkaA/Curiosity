using Godot;
using NPC.StateMachine;

public partial class BaseNPC : BaseEntity, IInteractable
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

        InitShader();

        stateMachine.ChangeState(IdleState);
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
        if (material == null)
            return;

        material.SetShaderParameter("outline_size", 0.5f);
    }

    public virtual void InteractExit()
	{
		if (material == null)
			return;

		material.SetShaderParameter("outline_size", 0f);
	}


	private void InitShader()
	{
		material = new ShaderMaterial();
        material.Shader = shader;
		material.SetShaderParameter("outline_size", 0f);
        Visual.Material = material;
	}

    
}