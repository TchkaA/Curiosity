using System.Collections.Generic;
using Godot;

public interface ISpeakable
{
    IEnumerable<ContextAnswer> GetAnswers(Node2D interactor);
}