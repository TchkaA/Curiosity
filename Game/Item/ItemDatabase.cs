using Godot;
using System.Collections.Generic;

/// <summary>
/// Реестр предметов, собранный из <see cref="ItemCatalog"/>.
/// Каталог загружается при первом обращении, поэтому для нового предмета
/// достаточно создать ресурс и добавить его в каталог — код менять не нужно.
/// </summary>
public static class ItemDatabase
{
    /// <summary>
    /// Путь к ресурсу каталога. Это единственное место, где каталог
    /// подключается по пути; все остальные системы работают через ID.
    /// </summary>
    public const string CatalogPath = "res://Objects/Items/ItemCatalog.tres";

    private static readonly Dictionary<int, Item> _items = new();
    private static bool _initialized;

    /// <summary>
    /// Все зарегистрированные предметы.
    /// </summary>
    public static IReadOnlyCollection<Item> All
    {
        get
        {
            EnsureInitialized();
            return _items.Values;
        }
    }

    /// <summary>
    /// Возвращает предмет по ID.
    /// Если предмет не найден, пишет ошибку и возвращает null.
    /// </summary>
    public static Item Get(int id)
    {
        if (TryGet(id, out var item))
        {
            return item;
        }

        GD.PushError($"ItemDatabase: предмет с ID {id} не найден");
        return null;
    }

    /// <summary>
    /// Пробует получить предмет по ID без вывода ошибки.
    /// </summary>
    public static bool TryGet(int id, out Item item)
    {
        EnsureInitialized();
        return _items.TryGetValue(id, out item);
    }

    /// <summary>
    /// Перезагружает каталог и заново индексирует предметы.
    /// </summary>
    public static void Reload()
    {
        _items.Clear();
        _initialized = false;
        EnsureInitialized();
    }

    private static void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        var catalog = GD.Load<ItemCatalog>(CatalogPath);
        if (catalog == null)
        {
            GD.PushError($"ItemDatabase: не удалось загрузить каталог {CatalogPath}");
            return;
        }

        foreach (var item in catalog.Items)
        {
            Register(item);
        }
    }

    private static void Register(Item item)
    {
        if (item == null)
        {
            GD.PushError($"ItemDatabase: в каталоге {CatalogPath} есть пустая ссылка на предмет");
            return;
        }

        if (item.Id <= 0)
        {
            GD.PushError(
                $"ItemDatabase: у предмета '{item.Name}' ({item.GetType().Name}) не задан положительный Id");
            return;
        }

        if (_items.TryGetValue(item.Id, out var existing))
        {
            GD.PushError(
                $"ItemDatabase: ID {item.Id} занят предметами '{existing.Name}' и '{item.Name}'");
            return;
        }

        _items[item.Id] = item;
    }
}