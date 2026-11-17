using System;
using System.Collections.Generic;

public sealed class DialogueMethodsExecutor
{
    public Dictionary<string, Action> CompareMethods = new();

    private BaseEntity _owner;

    private BaseEntity _player;

    public DialogueMethodsExecutor(BaseEntity owner, BaseEntity player)
    {
        _owner = owner;
        _player = player;

        LinksActions();
    }


    public void ParseMethod(string Methods)
    {
        foreach (string i in CompareMethods.Keys)
        {
            if(Methods == i)
            {
                CompareMethods[i]?.Invoke();
            }
        }
    }

    private void Follow()
    {
        if(_owner == null && _player == null) return;

        _owner.Follow(_player);
    }

    private void Die()
    {
        if(_owner == null) return;

        _owner.Die(_owner);
    }

    private void Fight()
    {
        // TODO
    }

    public void LinksActions()
    {
        CompareMethods["follow"] = Follow;
        CompareMethods["die"] = Die;
        CompareMethods["fight"] = Fight;
    }
}

public enum AvailableEntityMethods
{
    follow,
    die,
    fight
}