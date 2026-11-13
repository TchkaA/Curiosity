using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Глобальные реестры условий и эффектов диалогов.
/// Через них можно добавить свои типы, не меняя диалоговый компонент.
/// </summary>
public static class DialogueRules
{
    /// <summary>
    /// Проверка условий. Пример регистрации своего условия:
    /// <c>DialogueRules.Conditions.Register("quest_done", (ctx, c) => ...);</c>
    /// </summary>
    public static DialogueConditionChecker Conditions { get; } = new();

    /// <summary>
    /// Исполнение эффектов. Пример регистрации своего эффекта:
    /// <c>DialogueRules.Effects.Register("shake", (ctx, e) => ...);</c>
    /// </summary>
    public static DialogueEffectExecutor Effects { get; } = new();
}

/// <summary>
/// Проверяет условия диалогов. Поддерживает свои типы через <see cref="Register"/>.
/// </summary>
public sealed class DialogueConditionChecker
{
    private readonly Dictionary<string, Func<DialogueContext, DialogueCondition, bool>> _handlers =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Создаёт проверяльщик со встроенными типами условий.
    /// </summary>
    public DialogueConditionChecker()
    {
        Register("always", (_, _) => true);
        Register("flag", (context, condition) => Compare(context.State.GetFlag(condition.Key), condition));
        Register("stat", (context, condition) => Compare(context.GetStat(condition.Key), condition));
        Register("relationship", (context, condition) => Compare(context.State.GetRelationship(condition.Key), condition));
        Register("achievement", (context, condition) =>
            Compare(context.State.HasAchievement(condition.Key) ? 1f : 0f, condition));
        Register("item", (context, condition) => Compare(context.GetItemCount(condition.Key), condition));
    }

    /// <summary>
    /// Регистрирует свой тип условия. Возвращает этот же объект для цепочки вызовов.
    /// </summary>
    /// <param name="type">Тип из поля "type" в JSON.</param>
    /// <param name="handler">Функция проверки.</param>
    public DialogueConditionChecker Register(
        string type,
        Func<DialogueContext, DialogueCondition, bool> handler)
    {
        if (string.IsNullOrWhiteSpace(type) || handler == null)
            return this;

        _handlers[type] = handler;
        return this;
    }

    /// <summary>
    /// Проверяет одно условие.
    /// Неизвестный тип считается непройденным условием.
    /// </summary>
    public bool Check(DialogueContext context, DialogueCondition condition)
    {
        if (condition == null || string.IsNullOrWhiteSpace(condition.Type))
            return true;

        if (!_handlers.TryGetValue(condition.Type, out var handler))
        {
            GD.PushWarning($"[Dialogue] Неизвестный тип условия '{condition.Type}'");
            return false;
        }

        return handler(context, condition);
    }

    /// <summary>
    /// Проверяет все условия (логическое И). Пустой список всегда проходит.
    /// </summary>
    public bool CheckAll(DialogueContext context, IEnumerable<DialogueCondition> conditions)
    {
        if (conditions == null)
            return true;

        foreach (var condition in conditions)
        {
            if (!Check(context, condition))
                return false;
        }

        return true;
    }

    private static bool Compare(float actual, DialogueCondition condition)
    {
        var expected = condition.Value;

        return condition.Op switch
        {
            "==" => Mathf.IsEqualApprox(actual, expected),
            "!=" => !Mathf.IsEqualApprox(actual, expected),
            ">" => actual > expected,
            "<" => actual < expected,
            "<=" => actual <= expected,
            _ => actual >= expected
        };
    }
}
