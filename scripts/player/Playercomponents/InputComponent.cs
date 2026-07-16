using Godot;

public class InputComponent
{

    public Vector2 MovementInput { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool RollPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    public void Update()
    {
        MovementInput = new Vector2(
            Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
            Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up")
        ).Normalized();
    }
}