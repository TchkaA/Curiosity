using Godot;

public interface IInteractable
{
    void Interact(Node2D interactor);
    void InteractEnter();
    void InteractExit();
}