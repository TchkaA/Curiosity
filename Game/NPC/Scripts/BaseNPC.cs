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
        Died += Die;
    }


    public void Die(BaseEntity entity)
    {
        stateMachine.ChangeState(DiedState);
    }



    public void Interact(BaseEntity interactor)
    {
        if (CanInteract)
        {
            stateMachine.ChangeState(InteractState);
            InteractState.Interact(interactor);
            interactor.GoToInteraction(this);
            Dialogue?.Start(interactor);
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

}
