using Godot;
using NPC.StateMachine;
using System;
using System.Collections.Generic;

public partial class BaseNPC : BaseEntity, IInteractable
{

    //----------------------
    //      TEST
    //----------------------
    [ExportCategory("Тесты")]
    [Export] public bool TEST = false;
    // [Export] public List<Action> Tests;
    //----------------------
    //      flags
    //----------------------
    public bool IsAttacking;


    //----------------------
    //      states
    //----------------------
    public NPCAttackState attackState;
    public NPCFollowState followState;
    public NPCIdleState IdleState;
    public NPCDied DiedState;
    public NPCInteractState InteractState;
    //----------------------

    /// <summary>
    /// NavigationAgent используемый для навигации
    /// </summary>
    [ExportCategory("Агент навигации")]
    [Export] public NavigationAgent2D NavigationAgent;

    //----------------------
    //      Dialogue
    //----------------------
    /// <summary>
    /// Путь к JSON-файлу диалогов NPC. Пустая строка — диалог отключён.
    /// </summary>
    [ExportCategory("Диалог")]
    [Export(PropertyHint.File, "*.json")] public string DialogueFile = "res://Game/Dialogue/Data/BaseAnswer.json";

    /// <summary>
    /// Стартовый узел диалога. Если пусто, берётся startId из JSON-файла.
    /// </summary>
    [Export] public string StartDialogueId = "";

    /// <summary>
    /// Смещение речевого пузыря относительно NPC.
    /// </summary>
    [Export] public Vector2 DialogueBubbleOffset = new(0f, -70f);

    /// <summary>
    /// Масштаб речевого пузыря.
    /// </summary>
    [Export] public float DialogueBubbleScale = 0.3f;

    /// <summary>
    /// Предметы, которые лежат в инвентаре NPC и доступны после смерти.
    /// </summary>
    [ExportCategory("Лут")]
    [Export] public Godot.Collections.Array<Item> Loot = new();

    //----------------------
    //      Components
    //----------------------
    public NavigationComponent Navigation;          
    public VisionComponent vision;
    /// <summary>
    /// Компонент диалогов NPC.
    /// </summary>
    public DialogueComponent Dialogue;
    //----------------------


    public override void _Ready()
    {
        base._Ready();

        foreach (var item in Loot)
            Inventory.AddItem(item);

        #if DEBUG
        // Временный лут для проверки MVP, пока NPC не заполняются из кода.
        if (Loot.Count == 0)
        {
            Inventory.AddItem(new HealingPotion(), 2);
            Inventory.AddItem(new ChickenMeat(), 1);
        }
        #endif

        InitStates();

        stateMachine.ChangeState(IdleState);      
        #if DEBUG
        if(TEST) TryTest();
        #endif  
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Navigation.Update();
        vision.Update();
    }




    public override void InitComponents()
    {
        base.InitComponents();
        Navigation = new(this,NavigationAgent);
        vision = new(this);
        Dialogue = new(this, DialogueFile, StartDialogueId)
        {
            BubbleOffset = DialogueBubbleOffset,
            BubbleScale = DialogueBubbleScale
        };
    }

    public virtual void InitStates()
    {
        attackState = new(this);
        followState = new(this);
        IdleState = new(this);
        DiedState = new(this);
        InteractState = new(this);
    }

    public override void LinksActions()
    {
        base.LinksActions();
        Interaction += DialogueInteraction;
    }


    public override void Die(BaseEntity entity = null)
    {
        stateMachine.ChangeState(DiedState);
        Interaction = DiedInteraction;
    }

    public Action<BaseEntity> Interaction;

    public void Interact(BaseEntity interactor)
    {
        if (CanInteract)
        {
            Interaction?.Invoke(interactor);
        }
    }

    public void DialogueInteraction(BaseEntity interactor)
    {
        stateMachine.ChangeState(InteractState);
        InteractState.Interact(interactor);
        interactor.GoToInteraction(this);
        Dialogue?.Start(interactor);
    }

    public void DiedInteraction(BaseEntity interactor)
    {
        if(interactor is Player player)
        {
            player.GlobalBookMenu.OpenLoot(Inventory);
        }
    }

    public void InteractEnter()
    {
        if(TEST)
        {
            Test1();
            return;
        }
        visualComponent.ShowOutline();
    }

    public void InteractExit()
    {
        visualComponent.HideOutline();
    }


    public void TryTest()
    {
        
    }

    public void Test1()
    {
        visualComponent.ShowOutline(2.0f, Colors.Red);
        visualComponent.EnableOutlineGradient(Colors.Cyan, vertical: false);
    }

    public override void Follow(BaseEntity entity)
    {
        stateMachine.ChangeState(followState);
        followState.SelectTarget(entity);
    }

}
