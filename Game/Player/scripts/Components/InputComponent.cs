using System;
using Godot;

public class InputComponent
{

    public Vector2 MovementInput { get; private set; }
    // public bool AttackPressed { get; private set; }
    // public bool RollPressed { get; private set; }
    // public bool InteractPressed { get; private set; }
    private Player _player;
    
    public Action UIOpen;

    public InputComponent(Player player)
    {
        _player = player;

    }

    public void Update()
    {
        // В диалоге действия недоступны (Esc закрывает диалог).
        // if (_player.InDialogue || _player.IsAttacking)
        //     return;

        MovementInput = new Vector2(
            Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
            Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up")
        ).Normalized();
        UpdateDirection(MovementInput);
        if(Input.IsActionJustPressed("interact")) _player.interaction.Interact();

        // if(Input.IsActionJustPressed("Tab")) UIOpen?.Invoke();
    }

    /// <summary>
    /// Метод для обнавления текущего направления
    /// </summary>
    /// <param name="direction">Нужное направление</param>
    public void UpdateDirection(Vector2 direction)
    {
        if (direction != Vector2.Zero)
        {
            _player.direction.SetDirectionFromVector(direction);
        }
    }
}