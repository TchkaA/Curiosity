using System;

public class ContextAction
{
    public string Name;
    public Action Callback;

    public ContextAction(string name, Action callback)
    {
        Name = name;
        Callback = callback;
    }
}