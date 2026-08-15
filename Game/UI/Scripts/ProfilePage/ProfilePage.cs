using Godot;
using System;

public partial class ProfilePage : CanvasLayer
{
    /// <summary>
    /// Информация о игроке
    /// </summary>
    [ExportGroup("Базовые надписи")]
    [Export]
    public Label name;
    [Export]
    public Label Race { get; set; }
    [Export]
    public Label Stats { get; set; }


    private Player _player => MainManager.Instance.Player;

    /// <summary>
    /// В будущем можно будет заполнять разными статами
    /// </summary>
    public override void _EnterTree()
    {
        Stats.Text = _player.Stats.ReturnStats();
    }



}
