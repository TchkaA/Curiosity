using Godot;

public interface IInteractable
{
    void Interact(BaseEntity interactor);
    void InteractEnter();
    void InteractExit();
}