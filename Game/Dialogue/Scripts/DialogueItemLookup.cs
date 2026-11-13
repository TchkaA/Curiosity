using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Каталог предметов для диалоговых эффектов и условий.
/// Предмет ищется по имени C#-класса ("HealingPotion") или по числовому ID ("2").
/// Экземпляр создаётся через конструктор класса, поэтому пути к сценам не нужны.
/// </summary>
internal static class DialogueItemLookup
{
    private static readonly Dictionary<string, Type> ItemTypes = BuildItemTypes();

    /// <summary>
    /// Находит предмет по имени класса или ID.
    /// </summary>
    public static Item Find(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        var normalized = Normalize(key);
        if (ItemTypes.TryGetValue(normalized, out var type))
            return Create(type);

        if (int.TryParse(key, out var id))
        {
            Item match = null;
            foreach (var candidate in ItemTypes.Values)
            {
                var item = Create(candidate);
                if (item == null || item.ID != id)
                    continue;

                if (match != null)
                {
                    GD.PushWarning(
                        $"[Dialogue] ID {id} есть у нескольких предметов: {match.GetType().Name} и {candidate.Name}. Укажите имя класса.");
                    return match;
                }

                match = item;
            }

            if (match != null)
                return match;
        }

        GD.PushWarning(
            $"[Dialogue] Предмет '{key}' не найден. Укажите имя класса или ID. Доступны: {string.Join(", ", ItemTypes.Keys)}");
        return null;
    }

    private static Dictionary<string, Type> BuildItemTypes()
    {
        var result = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in typeof(Item).Assembly.GetTypes())
        {
            if (type == typeof(Item) || type.IsAbstract || !typeof(Item).IsAssignableFrom(type))
                continue;

            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                GD.PushWarning($"[Dialogue] У предмета {type.Name} нет конструктора без параметров");
                continue;
            }

            result[Normalize(type.Name)] = type;
        }

        return result;
    }

    private static Item Create(Type type)
    {
        try
        {
            return Activator.CreateInstance(type) as Item;
        }
        catch (Exception exception)
        {
            GD.PushError($"[Dialogue] Не удалось создать предмет {type.Name}: {exception.Message}");
            return null;
        }
    }

    private static string Normalize(string value)
    {
        return new string(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
    }
}
