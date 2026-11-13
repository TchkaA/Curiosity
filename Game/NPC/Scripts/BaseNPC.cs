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
    //----------------------

    /// <summary>
    /// NavigationAgent используемый для навигации
    /// </summary>
    [Export] public NavigationAgent2D NavigationAgent;

    //----------------------
    //      Components
    //----------------------
    public NavigationComponent Navigation;          
    public VisionComponent vision;
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
    }

    public virtual void InitStates()
    {
        attackState = new(this);
        followState = new(this);
        IdleState = new(this);
        DiedState = new(this);
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
        throw new NotImplementedException();
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
