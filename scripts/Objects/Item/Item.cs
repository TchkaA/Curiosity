using Godot;
using System;

public abstract partial class Item : Resource
{
	[Export]
	public string Name { get; private set; }
	[Export]
	public string Description { get; private set; }
	[Export]
	public Texture2D Icon { get; private set; }
	[Export]
	public int MaxStack { get; private set; }

	public abstract void Use(Node2D user);
}
