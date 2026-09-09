using Godot;
using System;

public partial class BaseEntity : CharacterBody2D
{
	public StatsComponent Stats;

	public BaseEntity()
	{
		Stats = new();
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}
