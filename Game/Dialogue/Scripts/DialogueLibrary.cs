using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

/// <summary>
/// Хранилище диалогов, загруженных из JSON.
/// Один экземпляр <see cref="Shared"/> используется всей игрой.
/// </summary>
public sealed class DialogueLibrary
{
    /// <summary>
    /// Общее хранилище диалогов.
    /// </summary>
    public static DialogueLibrary Shared { get; } = new();

    private readonly Dictionary<string, Dialogue> _dialogues = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _startIds = new(StringComparer.OrdinalIgnoreCase);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private const int RecommendedAnswerLength = 24;

    /// <summary>
    /// Все загруженные узлы диалогов (только для чтения).
    /// </summary>
    public IReadOnlyDictionary<string, Dialogue> Dialogues => _dialogues;

    /// <summary>
    /// Удаляет все загруженные диалоги. Полезно при смене региона/перезапуске.
    /// </summary>
    public void Clear()
    {
        _dialogues.Clear();
        _startIds.Clear();
    }

    /// <summary>
    /// Загружает один JSON-файл диалогов.
    /// Повторная загрузка файла заменяет его узлы (удобно для hot-reload).
    /// </summary>
    /// <param name="resPath">Путь вида "res://Game/Dialogue/Data/keeper.json".</param>
    /// <returns>true, если файл успешно прочитан.</returns>
    public bool LoadFile(string resPath)
    {
        if (string.IsNullOrWhiteSpace(resPath))
            return false;

        if (!FileAccess.FileExists(resPath))
        {
            GD.PushError($"[Dialogue] Файл не найден: {resPath}");
            return false;
        }

        DialogueFile file;
        try
        {
            using var fileAccess = FileAccess.Open(resPath, FileAccess.ModeFlags.Read);
            if (fileAccess == null)
            {
                GD.PushError($"[Dialogue] Не удалось открыть {resPath}: {FileAccess.GetOpenError()}");
                return false;
            }

            file = Parse(fileAccess.GetAsText());
        }
        catch (JsonException exception)
        {
            GD.PushError($"[Dialogue] Ошибка JSON в {resPath}: {exception.Message}");
            return false;
        }

        if (file == null || file.Dialogues == null || file.Dialogues.Count == 0)
        {
            GD.PushWarning($"[Dialogue] В файле нет диалогов: {resPath}");
            return false;
        }

        var firstValidId = "";
        var loadedNodes = 0;

        foreach (var dialogue in file.Dialogues)
        {
            if (dialogue == null || string.IsNullOrWhiteSpace(dialogue.Id))
            {
                GD.PushWarning($"[Dialogue] Пропущен узел без Id в {resPath}");
                continue;
            }

            if (_dialogues.ContainsKey(dialogue.Id))
                GD.PushWarning($"[Dialogue] Узел '{dialogue.Id}' уже существует и будет перезаписан ({resPath})");

            _dialogues[dialogue.Id] = dialogue;
            Validate(dialogue);

            if (string.IsNullOrWhiteSpace(firstValidId))
                firstValidId = dialogue.Id;

            loadedNodes++;
        }

        if (loadedNodes == 0)
        {
            GD.PushWarning($"[Dialogue] В файле нет валидных узлов: {resPath}");
            return false;
        }

        var startId = string.IsNullOrWhiteSpace(file.StartId) ? firstValidId : file.StartId;
        if (!string.IsNullOrWhiteSpace(startId))
            _startIds[resPath] = startId;

        return true;
    }

    /// <summary>
    /// Загружает все JSON-файлы из папки рекурсивно.
    /// Используйте для подгрузки диалогов региона или этапа игры.
    /// </summary>
    /// <param name="resFolder">Путь вида "res://Game/Dialogue/Data/region_01".</param>
    /// <returns>Количество успешно загруженных файлов.</returns>
    public int LoadDirectory(string resFolder)
    {
        if (string.IsNullOrWhiteSpace(resFolder))
            return 0;

        var directory = DirAccess.Open(resFolder);
        if (directory == null)
        {
            GD.PushError($"[Dialogue] Папка не найдена: {resFolder}");
            return 0;
        }

        var loadedCount = 0;
        directory.ListDirBegin();

        for (var name = directory.GetNext(); !string.IsNullOrEmpty(name); name = directory.GetNext())
        {
            if (name.StartsWith("."))
                continue;

            var path = resFolder.PathJoin(name);
            if (directory.CurrentIsDir())
            {
                loadedCount += LoadDirectory(path);
                continue;
            }

            if (!name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                continue;

            if (LoadFile(path))
                loadedCount++;
        }

        directory.ListDirEnd();
        return loadedCount;
    }

    /// <summary>
    /// Возвращает узел диалога по Id или null, если узел не найден.
    /// </summary>
    public Dialogue Get(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return _dialogues.TryGetValue(id, out var dialogue) ? dialogue : null;
    }

    /// <summary>
    /// Пробует получить узел диалога по Id.
    /// </summary>
    public bool TryGet(string id, out Dialogue dialogue)
    {
        dialogue = null;
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return _dialogues.TryGetValue(id, out dialogue);
    }

    /// <summary>
    /// Возвращает стартовый Id, указанный в JSON-файле.
    /// </summary>
    /// <param name="resPath">Путь к уже загруженному JSON-файлу.</param>
    public string GetStartId(string resPath)
    {
        if (string.IsNullOrWhiteSpace(resPath))
            return "";

        return _startIds.TryGetValue(resPath, out var startId) ? startId : "";
    }

    private static void Validate(Dialogue dialogue)
    {
        if (dialogue.Answers == null)
            return;

        foreach (var answer in dialogue.Answers)
        {
            if (answer == null || string.IsNullOrWhiteSpace(answer.PlayerText))
                continue;

            if (answer.PlayerText.Length > RecommendedAnswerLength)
            {
                GD.PushWarning(
                    $"[Dialogue] Ответ '{answer.PlayerText}' в узле '{dialogue.Id}' слишком длинный. Используйте 1–2 слова.");
            }
        }
    }

    private static DialogueFile Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        // Поддерживаем как массив узлов, так и объект { "startId": "...", "dialogues": [...] }.
        if (text.TrimStart().StartsWith("["))
        {
            return new DialogueFile
            {
                Dialogues = JsonSerializer.Deserialize<List<Dialogue>>(text, JsonOptions) ?? new List<Dialogue>()
            };
        }

        return JsonSerializer.Deserialize<DialogueFile>(text, JsonOptions);
    }
}
