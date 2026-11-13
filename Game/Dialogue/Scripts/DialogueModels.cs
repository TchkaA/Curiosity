using System.Collections.Generic;

/// <summary>
/// Один узел диалога: реплика NPC и варианты ответа игрока.
/// Узлы связываются между собой через <see cref="DialogueAnswer.NextId"/>.
/// </summary>
public sealed class Dialogue
{
    /// <summary>
    /// Уникальный идентификатор узла внутри всех загруженных файлов.
    /// Рекомендуется использовать префикс: "keeper.intro", "boss.first_meeting".
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// Реплика NPC. Показывается в речевом пузыре над говорящим.
    /// </summary>
    public string NpcText { get; set; } = "";

    /// <summary>
    /// Условия показа узла. Если хотя бы одно не выполнено, диалог завершается.
    /// Пустой список означает, что узел доступен всегда.
    /// </summary>
    public List<DialogueCondition> Conditions { get; set; } = new();

    /// <summary>
    /// Варианты ответа игрока. Пустой список означает конец диалога
    /// (игрок увидит кнопку продолжения, чтобы успеть прочитать реплику).
    /// </summary>
    public List<DialogueAnswer> Answers { get; set; } = new();

    /// <inheritdoc/>
    public override string ToString() => Id;
}

/// <summary>
/// Вариант ответа игрока в узле диалога.
/// </summary>
public sealed class DialogueAnswer
{
    /// <summary>
    /// Текст варианта ответа на кнопке.
    /// </summary>
    public string PlayerText { get; set; } = "";

    /// <summary>
    /// Id следующего узла диалога. Пустое значение или null завершает диалог.
    /// </summary>
    public string NextId { get; set; } = "";

    /// <summary>
    /// Условия показа ответа. Если хотя бы одно не выполнено, ответ скрывается.
    /// </summary>
    public List<DialogueCondition> Conditions { get; set; } = new();

    /// <summary>
    /// Эффекты, которые выполняются сразу после выбора ответа.
    /// </summary>
    public List<DialogueEffect> Effects { get; set; } = new();
}

/// <summary>
/// Условие показа узла или ответа.
/// Все условия объединяются логическим И.
/// </summary>
/// <example>
/// { "type": "stat", "key": "charisma", "op": ">=", "value": 3 }
/// </example>
public sealed class DialogueCondition
{
    /// <summary>
    /// Тип условия. Встроенные: flag, stat, relationship, achievement, item, always.
    /// Свой тип можно зарегистрировать в <see cref="DialogueRules.Conditions"/>.
    /// </summary>
    public string Type { get; set; } = "flag";

    /// <summary>
    /// Ключ проверяемого значения: имя флага, поля <see cref="StatsComponent"/>,
    /// фракции/персонажа, достижения или предмета.
    /// </summary>
    public string Key { get; set; } = "";

    /// <summary>
    /// Оператор сравнения: ==, !=, &gt;, &lt;, &gt;=, &lt;=.
    /// По умолчанию &gt;=, поэтому одиночный флаг без оператора считается установленным.
    /// </summary>
    public string Op { get; set; } = ">=";

    /// <summary>
    /// Значение для сравнения.
    /// </summary>
    public float Value { get; set; } = 1f;
}

/// <summary>
/// Эффект, выполняемый при выборе ответа или из кода.
/// </summary>
/// <example>
/// { "type": "item", "arg": "res://Objects/Items/.../Healing_potion.tres", "value": 1 }
/// </example>
public sealed class DialogueEffect
{
    /// <summary>
    /// Тип эффекта. Встроенные: item, achievement, relationship, stat, flag.
    /// Свой тип можно зарегистрировать в <see cref="DialogueRules.Effects"/>.
    /// </summary>
    public string Type { get; set; } = "";

    /// <summary>
    /// Аргумент эффекта: id/имя/путь предмета, ключ флага, id достижения и т.п.
    /// </summary>
    public string Arg { get; set; } = "";

    /// <summary>
    /// Числовое значение эффекта: количество предметов, прибавка отношений/стата.
    /// По умолчанию 1.
    /// </summary>
    public float Value { get; set; } = 1f;
}

/// <summary>
/// Содержимое JSON-файла диалогов.
/// Один файл может содержать целую ветку диалогов NPC или региона.
/// </summary>
public sealed class DialogueFile
{
    /// <summary>
    /// Id узла, с которого начинается диалог по умолчанию.
    /// Если не задан, используется первый узел из <see cref="Dialogues"/>.
    /// </summary>
    public string StartId { get; set; } = "";

    /// <summary>
    /// Все узлы диалога из файла.
    /// </summary>
    public List<Dialogue> Dialogues { get; set; } = new();
}
