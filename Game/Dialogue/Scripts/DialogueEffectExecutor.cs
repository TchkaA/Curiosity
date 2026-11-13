using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Выполняет эффекты диалогов. Поддерживает свои типы через <see cref="Register"/>.
/// Событие <see cref="EffectApplied"/> позволяет реагировать на любые эффекты
/// (тряска экрана, анимации персонажей, подсветка кнопок и т.п.) без изменения компонента.
/// </summary>
public sealed class DialogueEffectExecutor
{
    private readonly Dictionary<string, Action<DialogueContext, DialogueEffect>> _handlers =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Вызывается для каждого эффекта до его обработки.
    /// Можно использовать для визуальных/звуковых реакций.
    /// </summary>
    public event Action<DialogueContext, DialogueEffect> EffectApplied;

    /// <summary>
    /// Создаёт исполнитель со встроенными типами эффектов.
    /// </summary>
    public DialogueEffectExecutor()
    {
        Register("flag", (context, effect) => context.State.SetFlag(effect.Arg, Mathf.RoundToInt(effect.Value)));
        Register("achievement", (context, effect) => context.State.AddAchievement(effect.Arg));
        Register("relationship", (context, effect) =>
            context.State.AddRelationship(effect.Arg, Mathf.RoundToInt(effect.Value)));
        Register("stat", (context, effect) => context.AddStat(effect.Arg, effect.Value));
        Register("item", (context, effect) => context.GiveItem(effect.Arg, Mathf.RoundToInt(effect.Value)));
    }

    /// <summary>
    /// Регистрирует свой тип эффекта. Возвращает этот же объект для цепочки вызовов.
    /// </summary>
    /// <param name="type">Тип из поля "type" в JSON.</param>
    /// <param name="handler">Обработчик эффекта.</param>
    public DialogueEffectExecutor Register(string type, Action<DialogueContext, DialogueEffect> handler)
    {
        if (string.IsNullOrWhiteSpace(type) || handler == null)
            return this;

        _handlers[type] = handler;
        return this;
    }

    /// <summary>
    /// Выполняет один эффект.
    /// </summary>
    /// <returns>true, если для типа найден обработчик.</returns>
    public bool Execute(DialogueContext context, DialogueEffect effect)
    {
        if (effect == null || string.IsNullOrWhiteSpace(effect.Type))
            return false;

        EffectApplied?.Invoke(context, effect);

        if (_handlers.TryGetValue(effect.Type, out var handler))
        {
            handler(context, effect);
            return true;
        }

        GD.PushWarning($"[Dialogue] Неизвестный тип эффекта '{effect.Type}'");
        return false;
    }

    /// <summary>
    /// Выполняет все эффекты списка по порядку.
    /// </summary>
    public void ExecuteAll(DialogueContext context, IEnumerable<DialogueEffect> effects)
    {
        if (effects == null)
            return;

        foreach (var effect in effects)
            Execute(context, effect);
    }
}
