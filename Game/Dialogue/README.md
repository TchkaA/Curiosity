# Диалоговая система

Диалоги хранятся в JSON-файлах и не требуют кода для добавления новых NPC, боссов и веток.
Реплики NPC показываются в `WordBubble`, варианты ответа — в `DialogueUI`.

## Быстрый старт

1. Создайте JSON-файл, например `Game/Dialogue/Data/keeper.json`.
2. Укажите его в инспекторе NPC: **Диалог → Dialogue File**.
3. Опционально задайте **Start Dialogue Id** (иначе берётся `startId` из файла).
4. Нажмите `F` рядом с NPC.

UI ответов создаётся автоматически (сцена `Game/Dialogue/Scene/DialogueUI.tscn`).
Речевой пузырь создаётся из `Game/Dialogue/Scene/WordBubble.tscn`.
Размер и смещение пузыря настраиваются в инспекторе NPC.

## Формат файла

```json
{
  "startId": "keeper.intro",
  "dialogues": [
    {
      "id": "keeper.intro",
      "npcText": "Чужак. Чего тебе?",
      "conditions": [],
      "answers": [
        {
          "playerText": "О крае.",
          "nextId": "keeper.land",
          "conditions": [],
          "effects": [
            { "type": "flag", "arg": "met_keeper", "value": 1 }
          ]
        },
        {
          "playerText": "Прощай.",
          "nextId": ""
        }
      ]
    }
  ]
}
```

- `id` — уникальный id узла. Используйте префиксы: `keeper.intro`, `boss.phase_2`.
- `npcText` — реплика NPC.
- `playerText` — **1–2 слова**, например «Да.», «Нет.», «Правда.», «Ложь.», «Продолжить.». Длинные фразы не помещаются в кнопки.
- `answers` — от 0 до 3 вариантов. Пустой список = конец диалога.
- `nextId` — следующий узел. Пусто = конец диалога.
- Можно загрузить массив узлов без обёртки: `[ { "id": "...", ... } ]`.

## Условия

Условия можно ставить на узел (`conditions` у диалога) и на отдельный ответ.
Все условия объединяются через **И**. Если условие узла не выполнено — диалог завершается.
Если условие ответа не выполнено — кнопка скрывается.

```json
{ "type": "flag", "key": "met_keeper", "op": ">=", "value": 1 }
```

| type | key | Что проверяет |
| --- | --- | --- |
| `flag` | имя флага | сюжетный флаг/счётчик |
| `stat` | поле `StatsComponent` | например, `strength` или будущая `charisma` |
| `relationship` | персонаж/фракция | отношение |
| `achievement` | id достижения | получено ли достижение |
| `item` | имя класса или ID предмета | количество в инвентаре игрока |
| `always` | — | всегда true |

Операторы: `==`, `!=`, `>`, `<`, `>=`, `<=`. По умолчанию `>=`, поэтому
`{ "type": "flag", "key": "met_keeper" }` означает «флаг установлен».

`stat` читает публичное поле из `StatsComponent` по имени. Чтобы добавить харизму,
достаточно добавить поле в `StatsComponent` — код диалогов менять не нужно.

## Эффекты

Эффекты выполняются сразу после выбора ответа.

```json
{ "type": "item", "arg": "HealingPotion", "value": 1 }
```

| type | arg | value |
| --- | --- | --- |
| `flag` | имя флага | новое значение |
| `achievement` | id достижения | игнорируется |
| `relationship` | персонаж/фракция | прибавка |
| `stat` | поле `StatsComponent` | прибавка |
| `item` | имя класса (`HealingPotion`) или ID (`2`) | количество |

Пути к сценам/ресурсам в JSON не нужны: предмет создаётся через конструктор класса.

## Регионы и этапы игры

Хранилище умеет загружать папку целиком:

```csharp
DialogueLibrary.Shared.LoadDirectory("res://Game/Dialogue/Data/region_01");
```

Файлы загружаются рекурсивно. Повторная загрузка узла с тем же `id` перезаписывает его.
Если NPC указывает конкретный файл, он загрузится сам при первом разговоре.

## Свои условия и эффекты

Реестры глобальные, регистрировать можно из любого места игры:

```csharp
DialogueRules.Conditions.Register("quest_done", (context, condition) =>
    QuestSystem.IsDone(condition.Key));

DialogueRules.Effects.Register("shake", (context, effect) =>
    Camera.Instance.Shake(effect.Value));

// Реакция на любой эффект (в том числе для UI/анимаций):
DialogueRules.Effects.EffectApplied += (context, effect) =>
{
    if (effect.Type == "screen_shake")
        GD.Print($"Тряска экрана силой {effect.Value}");
};
```

Примеры из JSON:

```json
{ "type": "quest_done", "key": "find_blacksmith" }
{ "type": "shake", "value": 2 }
```

## API в коде

```csharp
// Запуск диалога из кода:
npc.Dialogue.Start(player);

// Выбор ответа (0..2), -1 — закрыть:
npc.Dialogue.SelectAnswer(1);

// Завершение:
npc.Dialogue.End();

// Состояние (для сейвов):
DialogueState.Shared.Flags;
DialogueState.Shared.Relationships;
DialogueState.Shared.Achievements;
```

## Важные ограничения MVP

- Состояние диалогов хранится в памяти; для сейвов используйте словари из `DialogueState`.
- На экране максимум 3 ответа: лишние доступные ответы обрезаются с предупреждением.
- Ответы должны быть короткими (1–2 слова): при загрузке JSON длинные ответы дают предупреждение.
- Эффекты `stat` применяются к игроку.
