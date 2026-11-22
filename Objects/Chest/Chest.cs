using Godot;

[Tool]
[GlobalClass]
public partial class Chest : WorldObject
{
    /// <summary>
    /// Предметы, которые лежат в инвентаре NPC и доступны после смерти.
    /// </summary>
    [ExportCategory("Лут")]
    [Export] public Godot.Collections.Array<Item> Loot = new();

    private Inventory _inventory;

    public override void _Ready()
    {
        base._Ready();

        _inventory = new(this);

        foreach(var item in Loot)
        {
            _inventory.AddItem(item);
        }
    }

    public override void Interact(BaseEntity interactor)
    {
        if(interactor is Player player)
        {
            player.GlobalBookMenu.OpenLoot(_inventory);
        }
    }
}