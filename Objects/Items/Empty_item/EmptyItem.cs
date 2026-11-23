using Godot;

/// <summary>
/// Предмет-заглушка для пустых слотов и тестовых сцен.
/// Использование не даёт эффекта.
/// </summary>
[GlobalClass]
public partial class EmptyItem : Item
{
    public override bool Use(Node2D user)
    {
        GD.PushWarning("[EmptyItem] Предмет-заглушка нельзя использовать");
        return false;
    }
}
