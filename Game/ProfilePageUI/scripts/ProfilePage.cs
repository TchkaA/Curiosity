using Godot;
using System;


/// <summary>
/// Профиль игрока с его информацией.
/// </summary>
public partial class ProfilePage : CanvasLayer
{
	private Player _player => MainManager.Instance.Player;

	[ExportGroup("Основаня информация")]
	[Export]
	public Label NameLabel {get;private set;}
	[Export]
	public Label StatsLabel {get; private set;}
	public string Stats;


	public override void _EnterTree()
	{
		UpdateInfo();
	}

	public void UpdateInfo()
	{
		UpdateStats();
	}

	public void UpdateStats()
	{
		var info = $"Health - {_player.Stats.Health}\n"+
			$"Stamina - {_player.Stats.Stamina}\n"+
			$"Speed - {_player.Stats.Speed}";
		StatsLabel.Text = info;
	}
}
