using System.Collections.Generic;
using Godot;

public interface ISpeekable
{
    IEnumerable<ContextAnswer> GetAnswers(Node2D interactor);
}