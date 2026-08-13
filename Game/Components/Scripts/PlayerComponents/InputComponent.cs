using Godot;

public class InputComponent
{
    public Vector2 MovementInput { get; private set; }

    private Player _player;

    public InputComponent(Player player)
    {
        _player = player;
    }

    public void Update()
    {
        MovementInput = new Vector2(
            Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
            Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up")
        ).Normalized();

        UpdateDirection(MovementInput);

        // В диалоге действия недоступны (Esc закрывает диалог).
        if (_player.IsInDialogue)
            return;

        if (Input.IsActionJustPressed("interact"))
        {
            _player.Interact.Interact();
        }

        if (Input.IsActionJustPressed("change_state"))
        {
            _player.ToggleCombat();
        }
    }

    public void UpdateDirection(Vector2 direction)
    {
        if (direction != Vector2.Zero)
        {
            _player.directionComponent.SetDirection(direction);
        }
    }
}