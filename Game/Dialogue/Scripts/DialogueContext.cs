using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Контекст одного диалога: кто говорит, с кем говорят и доступ к общим данным.
/// Все условия и эффекты получают этот объект.
/// </summary>
public sealed class DialogueContext
{
    private static readonly Dictionary<string, FieldInfo> StatFields = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> MissingStatFields = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Существо, которое ведёт диалог (NPC, босс, враг).
    /// </summary>
    public BaseEntity Owner { get; }

    /// <summary>
    /// Существо, с которым говорят (обычно игрок).
    /// </summary>
    public BaseEntity Player { get; }

    /// <summary>
    /// Глобальное состояние диалогов (флаги, отношения, достижения).
    /// </summary>
    public DialogueState State => DialogueState.Shared;

    /// <summary>
    /// Создаёт контекст диалога.
    /// </summary>
    public DialogueContext(BaseEntity owner, BaseEntity player)
    {
        Owner = owner;
        Player = player;
    }

    /// <summary>
    /// Возвращает значение публичного поля <see cref="StatsComponent"/> по имени.
    /// Например, "charisma" найдёт поле Charisma, если оно добавлено в StatsComponent.
    /// </summary>
    /// <returns>0, если поле не найдено.</returns>
    public float GetStat(string key)
    {
        var field = ResolveStatField(key);
        if (field == null || Player?.Stats == null)
            return 0f;

        return Convert.ToSingle(field.GetValue(Player.Stats));
    }

    /// <summary>
    /// Прибавляет значение к публичному полю <see cref="StatsComponent"/> игрока.
    /// Поддерживаются поля float/int/double.
    /// </summary>
    /// <returns>true, если поле найдено и изменено.</returns>
    public bool AddStat(string key, float delta)
    {
        var field = ResolveStatField(key);
        if (field == null || Player?.Stats == null)
            return false;

        var current = Convert.ToSingle(field.GetValue(Player.Stats));
        var updated = current + delta;

        if (field.FieldType == typeof(int))
            field.SetValue(Player.Stats, Mathf.RoundToInt(updated));
        else
            field.SetValue(Player.Stats, Convert.ChangeType(updated, field.FieldType));

        return true;
    }

    /// <summary>
    /// Возвращает количество предмета в инвентаре игрока.
    /// Ключом может быть имя класса предмета или его числовой ID.
    /// </summary>
    public int GetItemCount(string itemKey)
    {
        var item = DialogueItemLookup.Find(itemKey);
        if (item == null || Player?.Inventory == null)
            return 0;

        var count = 0;
        for (var i = 0; i < Player.Inventory.Capacity; i++)
        {
            var slot = Player.Inventory.GetSlot(i);
            if (slot?.Item == null)
                continue;

            if (slot.Item == item || slot.Item.Name == item.Name)
                count += slot.Count;
        }

        return count;
    }

    /// <summary>
    /// Выдаёт предмет игроку.
    /// </summary>
    /// <returns>true, если предмет найден и добавлен.</returns>
    public bool GiveItem(string itemKey, int count)
    {
        var item = DialogueItemLookup.Find(itemKey);
        if (item == null || Player?.Inventory == null)
            return false;

        Player.Inventory.AddItem(item, count);
        return true;
    }

    private static FieldInfo ResolveStatField(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (StatFields.TryGetValue(key, out var field))
            return field;

        if (MissingStatFields.Contains(key))
            return null;

        field = typeof(StatsComponent).GetField(
            key,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (field == null)
        {
            MissingStatFields.Add(key);
            GD.PushWarning($"[Dialogue] Поле '{key}' не найдено в StatsComponent");
            return null;
        }

        StatFields[key] = field;
        return field;
    }
}
