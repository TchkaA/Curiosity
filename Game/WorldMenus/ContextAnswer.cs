using System;

public class ContextAnswer
{
    public string Answer;
    public Action Callback;

    public ContextAnswer(string answer, Action callback)
    {
        Answer = answer;
        Callback = callback;
    }
}