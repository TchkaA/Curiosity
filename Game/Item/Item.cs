using Godot;

/// <summary>
/// Базовый класс для всех предметов в игре.
/// Наследуется от Resource, что позволяет создавать предметы как ассеты в редакторе Godot.
/// </summary>
public abstract partial class Item : Resource
{
    [Export] public string Name { get; set; } = "Unnamed Item";
    [Export] public Texture2D Icon { get; set; }
    [Export] public string Description { get; set; } = "No description.";
    
    /// <summary>
    /// Максимальное количество предметов в одном слоте. 1 = не стакается.
    /// </summary>
    [Export] public int MaxStack { get; set; } = 1;
    
    [Export] public int ID { get; set; }

    /// <summary>
    /// Логика использования предмета. 
    /// Переопределяется в наследниках (зелье, оружие, ключ).
    /// </summary>
    /// <param name="user">Сущность, использующая предмет</param>
    /// <returns>True, если использование прошло успешно (например, зелье выпито), False иначе.</returns>
    public abstract bool Use(Node2D user);
}