using System;
using System.Collections.Generic;

/// <summary>
/// Глобальное состояние диалоговой системы: флаги, отношения и достижения.
/// Используется условиями и эффектами. Пока хранится в памяти;
/// для сейвов достаточно прочитать <see cref="Flags"/>, <see cref="Relationships"/>
/// и <see cref="Achievements"/> и восстановить их через <see cref="SetFlag"/> и т.п.
/// </summary>
public sealed class DialogueState
{
    /// <summary>
    /// Общее состояние диалогов игры.
    /// </summary>
    public static DialogueState Shared { get; } = new();

    private readonly Dictionary<string, int> _flags = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _relationships = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _achievements = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Сюжетные флаги и счётчики.
    /// </summary>
    public IReadOnlyDictionary<string, int> Flags => _flags;

    /// <summary>
    /// Отношение к персонажам и фракциям.
    /// </summary>
    public IReadOnlyDictionary<string, int> Relationships => _relationships;

    /// <summary>
    /// Полученные достижения.
    /// </summary>
    public IReadOnlyCollection<string> Achievements => _achievements;

    /// <summary>
    /// Возвращает значение флага (0, если флаг не установлен).
    /// </summary>
    public int GetFlag(string key)
    {
        return !string.IsNullOrWhiteSpace(key) && _flags.TryGetValue(key, out var value) ? value : 0;
    }

    /// <summary>
    /// Устанавливает значение флага.
    /// </summary>
    public void SetFlag(string key, int value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        _flags[key] = value;
    }

    /// <summary>
    /// Прибавляет значение к флагу.
    /// </summary>
    public void AddFlag(string key, int value)
    {
        SetFlag(key, GetFlag(key) + value);
    }

    /// <summary>
    /// Возвращает значение отношений (0, если запись отсутствует).
    /// </summary>
    public int GetRelationship(string key)
    {
        return !string.IsNullOrWhiteSpace(key) && _relationships.TryGetValue(key, out var value) ? value : 0;
    }

    /// <summary>
    /// Прибавляет значение к отношениям.
    /// </summary>
    public void AddRelationship(string key, int value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        _relationships[key] = GetRelationship(key) + value;
    }

    /// <summary>
    /// Проверяет наличие достижения.
    /// </summary>
    public bool HasAchievement(string key)
    {
        return !string.IsNullOrWhiteSpace(key) && _achievements.Contains(key);
    }

    /// <summary>
    /// Выдаёт достижение.
    /// </summary>
    public void AddAchievement(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        _achievements.Add(key);
    }

    /// <summary>
    /// Полностью очищает состояние (например, при новой игре).
    /// </summary>
    public void Reset()
    {
        _flags.Clear();
        _relationships.Clear();
        _achievements.Clear();
    }
}
