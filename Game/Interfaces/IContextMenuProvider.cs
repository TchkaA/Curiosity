using System.Collections.Generic;
using Godot;

public interface IContextMenuProvider
{
    IEnumerable<ContextAction> GetContextAction(BaseEntity interactor);
}