using Godot;

public partial class Player : BaseEntity, IInventoryOwner
{
    public StateMachine stateMachine;

    public PlayerIdleState idleState;
    public PlayerMoveState moveState;
    public InteractionState interactionState;

    public InputComponent inputComponent;
    public MovementComponent movementComponent;
    public InteractionComponent Interact;
    public FollowMenuComponent followMenu;

    public Inventory Inventory { get; private set; }

    /// <summary>Игрок в диалоге: движение и действия заблокированы, выход по Esc.</summary>
    public bool IsInDialogue { get; private set; }

    /// <summary>Режим боя. Пока влияет только на префикс анимаций — заготовка.</summary>
    private bool _isInCombat;

    public Player()
    {
        Inventory = new Inventory(this);
    }

    public override void _Ready()
    {
        base._Ready();

        stateMachine = new StateMachine();

        idleState = new(this);
        moveState = new(this);
        interactionState = new(this);

        movementComponent = new MovementComponent(this);
        inputComponent = new InputComponent(this);

        Interact = new InteractionComponent(this);
        Interact.InitialInteractionArea();

        followMenu = new(this);

        stateMachine.ChangeState(idleState);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        inputComponent.Update();
        stateMachine?.Update(delta);
        followMenu.Update(delta);
    }

    internal void OpenMenu()
    {
        if (followMenu.IsMenuOpen == false)
            followMenu.OpenMenu();
        else
            followMenu.CloseMenu();
    }

    /// <summary>Войти в боевой режим (префикс анимаций combat_*).</summary>
    public void EnterCombat()
    {
        if (_isInCombat) return;
        _isInCombat = true;
        Animation.SetPrefix("combat");
        stateMachine.ChangeState(idleState);
    }

    /// <summary>Выйти из боевого режима (префикс exploring_*).</summary>
    public void ExitCombat()
    {
        if (!_isInCombat) return;
        _isInCombat = false;
        Animation.SetPrefix("exploring");
        stateMachine.ChangeState(idleState);
    }

    public void ToggleCombat()
    {
        if (_isInCombat)
            ExitCombat();
        else
            EnterCombat();
    }

    /// <summary>Начать диалог с NPC: блокирует движение, выход по Esc.</summary>
    public void EnterToTalk(BaseNPC body)
    {
        IsInDialogue = true;
        stateMachine.ChangeState(interactionState);
    }

    /// <summary>Выйти из диалога: закрыть меню и вернуться в idle.</summary>
    public void ExitDialogue()
    {
        IsInDialogue = false;
        followMenu.CloseMenu();
        stateMachine.ChangeState(idleState);
    }
}