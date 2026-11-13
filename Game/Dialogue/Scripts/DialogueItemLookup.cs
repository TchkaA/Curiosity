using Godot;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Небольшой каталог предметов для диалоговых эффектов и условий.
/// Ищет предмет по пути "res://...", имени, ID или имени файла.
/// При первом поиске сканирует res://Objects/Items.
/// </summary>
internal static class DialogueItemLookup
{
    private const string ItemsRoot = "res://Objects/Items";

    private static readonly Dictionary<string, Item> Cache = new(StringComparer.OrdinalIgnoreCase);
    private static bool _scanned;

    /// <summary>
    /// Находит предмет по ключу. Ключ может быть путём к ресурсу или обычным именем.
    /// </summary>
    public static Item Find(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (key.StartsWith("res://", StringComparison.OrdinalIgnoreCase))
            return ResourceLoader.Load(key) as Item;

        if (Cache.TryGetValue(key, out var cached))
            return cached;

        Scan();

        if (Cache.TryGetValue(key, out cached))
            return cached;

        GD.PushWarning($"[Dialogue] Предмет '{key}' не найден. Укажите путь к .tres или имя предмета.");
        return null;
    }

    /// <summary>
    /// Регистрирует предмет вручную под дополнительным ключом.
    /// </summary>
    public static void Register(string key, Item item)
    {
        if (string.IsNullOrWhiteSpace(key) || item == null)
            return;

        Cache[key] = item;
    }

    private static void Scan()
    {
        if (_scanned)
            return;

        _scanned = true;
        ScanDirectory(ItemsRoot);
    }

    private static void ScanDirectory(string path)
    {
        var directory = DirAccess.Open(path);
        if (directory == null)
            return;

        directory.ListDirBegin();

        for (var name = directory.GetNext(); !string.IsNullOrEmpty(name); name = directory.GetNext())
        {
            if (name.StartsWith("."))
                continue;

            var fullPath = path.PathJoin(name);
            if (directory.CurrentIsDir())
            {
                ScanDirectory(fullPath);
                continue;
            }

            // В экспортированном проекте ресурсы могут быть переименованы в .remap.
            if (name.EndsWith(".remap", StringComparison.OrdinalIgnoreCase))
                fullPath = fullPath[..^".remap".Length];

            if (!fullPath.EndsWith(".tres", StringComparison.OrdinalIgnoreCase) &&
                !fullPath.EndsWith(".res", StringComparison.OrdinalIgnoreCase))
                continue;

            var item = ResourceLoader.Load(fullPath) as Item;
            if (item == null)
                continue;

            Register(item.Name, item);
            Register(item.ID.ToString(), item);
            Register(item.ResourcePath, item);
            Register(Path.GetFileNameWithoutExtension(item.ResourcePath), item);
        }

        directory.ListDirEnd();
    }
}
