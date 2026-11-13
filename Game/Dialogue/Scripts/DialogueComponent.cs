using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Компонент диалогов существа.
/// Загружает JSON, показывает реплики NPC, варианты ответа, проверяет условия и выполняет эффекты.
/// Добавляется в <see cref="BaseNPC"/> и может переиспользоваться в других существах.
/// </summary>
public sealed class DialogueComponent
{
    /// <summary>
    /// Путь к сцене речевого пузыря по умолчанию.
    /// </summary>
    public const string DefaultWordBubbleScenePath = "res://Game/Dialogue/Scene/WordBubble.tscn";

    /// <summary>
    /// Максимальное количество вариантов ответа на экране.
    /// </summary>
    public const int MaxAnswers = 3;

    private readonly BaseEntity _owner;
    private readonly DialogueLibrary _library;
    private readonly DialogueConditionChecker _conditions;
    private readonly DialogueEffectExecutor _effects;
    private readonly List<DialogueAnswer> _visibleAnswers = new();

    private string _file;
    private string _startId;
    private BaseEntity _player;
    private DialogueContext _context;
    private DialogueUI _ui;
    private WordBubble _bubble;
    private Dialogue _current;

    /// <summary>
    /// Смещение речевого пузыря относительно существа.
    /// </summary>
    public Vector2 BubbleOffset { get; set; } = new(0f, -70f);

    /// <summary>
    /// Масштаб речевого пузыря.
    /// </summary>
    public float BubbleScale { get; set; } = 0.3f;

    /// <summary>
    /// Текущий узел диалога или null, если диалог не идёт.
    /// </summary>
    public Dialogue Current => _current;

    /// <summary>
    /// Идёт ли сейчас диалог.
    /// </summary>
    public bool IsActive => _player != null && _current != null;

    /// <summary>
    /// Вызывается после завершения диалога.
    /// </summary>
    public event Action Ended;

    /// <summary>
    /// Создаёт компонент диалога для существа.
    /// </summary>
    /// <param name="owner">Существо-владелец.</param>
    /// <param name="file">Путь к JSON-файлу диалогов.</param>
    /// <param name="startId">Стартовый узел; если пусто — берётся startId из файла.</param>
    /// <param name="library">Хранилище диалогов; по умолчанию общее.</param>
    /// <param name="conditions">Проверка условий; по умолчанию общая.</param>
    /// <param name="effects">Исполнитель эффектов; по умолчанию общий.</param>
    public DialogueComponent(
        BaseEntity owner,
        string file = "",
        string startId = "",
        DialogueLibrary library = null,
        DialogueConditionChecker conditions = null,
        DialogueEffectExecutor effects = null)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _file = file ?? "";
        _startId = startId ?? "";
        _library = library ?? DialogueLibrary.Shared;
        _conditions = conditions ?? DialogueRules.Conditions;
        _effects = effects ?? DialogueRules.Effects;
    }

    /// <summary>
    /// Меняет JSON-файл и стартовый узел компонента.
    /// </summary>
    public void Configure(string file, string startId = "")
    {
        _file = file ?? "";
        _startId = startId ?? "";
    }

    /// <summary>
    /// Начинает диалог с указанным существом.
    /// </summary>
    /// <returns>true, если диалог начался.</returns>
    public bool Start(BaseEntity player)
    {
        if (player == null)
            return false;

        if (IsActive)
        {
            GD.PushWarning($"[Dialogue] {_owner.Name}: диалог уже идёт");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_file))
            return false;

        if (!_library.LoadFile(_file))
            return false;

        var startId = string.IsNullOrWhiteSpace(_startId) ? _library.GetStartId(_file) : _startId;
        if (string.IsNullOrWhiteSpace(startId))
        {
            GD.PushWarning($"[Dialogue] {_owner.Name}: не задан стартовый узел диалога");
            return false;
        }

        _ui = DialogueUI.GetOrCreate(_owner);
        if (_ui == null)
            return false;

        _player = player;
        _context = new DialogueContext(_owner, player);
        return ShowNode(startId);
    }

    /// <summary>
    /// Выбирает вариант ответа по индексу (индексы соответствуют видимым ответам).
    /// Отрицательный индекс завершает диалог.
    /// </summary>
    /// <returns>true, если выбор обработан.</returns>
    public bool SelectAnswer(int index)
    {
        if (!IsActive)
            return false;

        if (index < 0 || index >= _visibleAnswers.Count)
        {
            End();
            return false;
        }

        var answer = _visibleAnswers[index];
        _visibleAnswers.Clear();

        _effects.ExecuteAll(_context, answer.Effects);

        if (string.IsNullOrWhiteSpace(answer.NextId))
        {
            End();
            return true;
        }

        ShowNode(answer.NextId);
        return true;
    }

    /// <summary>
    /// Завершает диалог, закрывает UI и возвращает игрока в обычное состояние.
    /// </summary>
    public void End()
    {
        if (_player == null && _current == null)
            return;

        var player = _player;

        _current = null;
        _player = null;
        _context = null;
        _visibleAnswers.Clear();

        if (_ui != null && GodotObject.IsInstanceValid(_ui))
            _ui.Close();

        HideBubble(true);

        if (player is Player playerEntity)
            playerEntity.EndInteraction();

        Ended?.Invoke();
    }

    private bool ShowNode(string id)
    {
        var dialogue = _library.Get(id);
        if (dialogue == null)
        {
            GD.PushWarning($"[Dialogue] {_owner.Name}: узел '{id}' не найден");
            End();
            return false;
        }

        if (!_conditions.CheckAll(_context, dialogue.Conditions))
        {
            End();
            return false;
        }

        if (_ui == null || !GodotObject.IsInstanceValid(_ui))
            _ui = DialogueUI.GetOrCreate(_owner);

        if (_ui == null)
        {
            End();
            return false;
        }

        _current = dialogue;
        ShowNpcText(dialogue.NpcText);

        _visibleAnswers.Clear();
        if (dialogue.Answers != null)
        {
            foreach (var answer in dialogue.Answers)
            {
                if (answer == null || !_conditions.CheckAll(_context, answer.Conditions))
                    continue;

                _visibleAnswers.Add(answer);
            }
        }

        if (_visibleAnswers.Count > MaxAnswers)
        {
            GD.PushWarning(
                $"[Dialogue] {_owner.Name}: у узла '{dialogue.Id}' больше {MaxAnswers} доступных ответов, лишние скрыты");
            _visibleAnswers.RemoveRange(MaxAnswers, _visibleAnswers.Count - MaxAnswers);
        }

        if (_visibleAnswers.Count == 0)
        {
            if (string.IsNullOrWhiteSpace(dialogue.NpcText))
            {
                End();
                return false;
            }

            _ui.ShowContinue(OnContinueSelected);
            return true;
        }

        var texts = new List<string>(_visibleAnswers.Count);
        foreach (var answer in _visibleAnswers)
            texts.Add(answer.PlayerText);

        _ui.ShowAnswers(texts, index => SelectAnswer(index));
        return true;
    }

    private void OnContinueSelected()
    {
        End();
    }

    private void ShowNpcText(string text)
    {
        HideBubble(false);

        if (string.IsNullOrWhiteSpace(text))
            return;

        var scene = GD.Load<PackedScene>(DefaultWordBubbleScenePath);
        if (scene == null)
        {
            GD.PushError($"[Dialogue] Не найдена сцена речевого пузыря: {DefaultWordBubbleScenePath}");
            return;
        }

        _bubble = scene.Instantiate<WordBubble>();
        _owner.AddChild(_bubble);
        _bubble.GlobalPosition = _owner.GlobalPosition + BubbleOffset;
        _bubble.Scale = Vector2.One * BubbleScale;
        _ = _bubble.ShowDialog(text);
    }

    private void HideBubble(bool fade)
    {
        if (_bubble == null || !GodotObject.IsInstanceValid(_bubble))
        {
            _bubble = null;
            return;
        }

        var bubble = _bubble;
        _bubble = null;

        if (fade)
            _ = bubble.FadeOutAndFree();
        else
            bubble.QueueFree();
    }
}
