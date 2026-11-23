using Godot;
using System.Globalization;

/// <summary>
/// Преобразует ссылку из JSON диалога в предмет каталога.
/// Предмет указывается числовым ID: <c>{ "type": "item", "arg": 2 }</c>.
/// </summary>
internal static class DialogueItemLookup
{
    /// <summary>
    /// Находит предмет по числовому ID из поля <c>arg</c>.
    /// </summary>
    public static Item Find(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        if (!int.TryParse(key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
        {
            GD.PushWarning($"[Dialogue] Предмет '{key}' должен быть указан числовым ID из ItemCatalog");
            return null;
        }

        if (ItemDatabase.TryGet(id, out var item))
        {
            return item;
        }

        GD.PushWarning($"[Dialogue] Предмет с ID {id} не найден в ItemCatalog");
        return null;
    }
}
