using Godot;

/// <summary>
/// Каталог всех предметов игры.
/// Хранится как ресурс и служит единственным источником данных
/// для <see cref="ItemDatabase"/>.
/// </summary>
[GlobalClass]
public partial class ItemCatalog : Resource
{
    /// <summary>
    /// Все предметы игры. Порядок не важен, ID должны быть уникальными.
    /// </summary>
    [Export] public Godot.Collections.Array<Item> Items { get; set; } = new();
}
