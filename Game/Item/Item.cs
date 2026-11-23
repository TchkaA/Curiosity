using Godot;

/// <summary>
/// Базовый класс всех предметов.
/// Предмет — это ресурс (<c>.tres</c>), который настраивается в редакторе
/// и используется сценами напрямую, без создания экземпляров из кода.
/// </summary>
[Tool]
[GlobalClass]
public abstract partial class Item : Resource
{
    /// <summary>
    /// Уникальный числовой идентификатор предмета.
    /// Используется в сейвах и диалогах, поэтому после релиза
    /// его нельзя менять или переиспользовать для другого предмета.
    /// </summary>
    [Export] public int Id { get; set; }

    /// <summary>
    /// Отображаемое название предмета.
    /// </summary>
    [Export] public string Name { get; set; } = "Unnamed Item";

    /// <summary>
    /// Иконка для инвентаря и предмета на земле.
    /// </summary>
    [Export] public Texture2D Icon { get; set; }

    /// <summary>
    /// Описание, которое показывается игроку.
    /// </summary>
    [Export] public string Description { get; set; } = "No description.";

    /// <summary>
    /// Максимальное количество предметов в одном слоте. 1 = не стакается.
    /// </summary>
    [Export] public int MaxStack { get; set; } = 1;

    /// <summary>
    /// Логика использования предмета.
    /// Переопределяется в наследниках (зелье, еда, оружие, ключ).
    /// </summary>
    /// <param name="user">Сущность, использующая предмет.</param>
    /// <returns>true, если предмет был использован и его можно списать.</returns>
    public abstract bool Use(Node2D user);
}