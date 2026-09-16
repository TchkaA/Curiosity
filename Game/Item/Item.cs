using Godot;

/// <summary>
/// Абстрактный класс игрового объекта
/// </summary>
public abstract class Item
{
	public string Name;

	public Texture2D Icon;

	public string Description;

	public int MaxStack;
}