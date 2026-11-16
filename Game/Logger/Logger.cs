using Godot;
using System.Collections.Generic;

public partial class Logger : VBoxContainer
{
    [Export] public float TimeOfMessages = 5f;
    [Export] public AudioStreamPlayer Notif;
    [Export] public int MaxMessages = 8;

    public static Logger instance;

    // Храним Control, а не Button — не привязываемся к типу
    private readonly List<Control> _messages = new();

    public override void _Ready()
    {
        base._Ready();
        instance = this;
    }

    // ---------- Статические врапперы (сахар) ----------
    public static void Print(string text)
    {
        if (instance == null) return;
        instance.AddMessage(text, null);
    }

    public static void Print(string text, Texture2D icon)
    {
        if (instance == null) return;
        instance.AddMessage(text, icon);
    }

    public static void PrintItem(Item item, int count)
    {
        if (instance == null) return;
        instance.AddItemNotification(item, count);
    }

    // ---------- Публичное API ----------

    public void AddMessage(string text, Texture2D icon)
    {
        if (string.IsNullOrEmpty(text)) return;

        if (MaxMessages > 0)
        {
            while (_messages.Count >= MaxMessages)
                RemoveMessage(_messages[0]);
        }

        var button = new Button
        {
            Text = text,
            Icon = icon,
            CustomMinimumSize = new Vector2(0, 20),
            SizeFlagsHorizontal = SizeFlags.ShrinkBegin,
            Flat = true,
            // Не мешаем кликам по миру и не ловим мышь вообще
            MouseFilter = Control.MouseFilterEnum.Ignore,
            // На всякий случай — не даём фокусу уходить в лог
            FocusMode = Control.FocusModeEnum.None,
            // ExpandIcon = true,
            IconAlignment = HorizontalAlignment.Right,
        };

        AddChild(button);
        _messages.Add(button);

        PlayNotification();
        ScheduleRemoval(button);
    }

    public void AddMessage(string text)
    {
        AddMessage(text, null);
    }

    public void AddItemNotification(Item item, int count)
    {
        if (item == null) return;
        AddMessage($"+{count} {item.Name}", item.Icon);
    }

    // ---------- Внутреннее ----------

    private void PlayNotification()
    {
        if (Notif?.Stream == null) return;
        Notif.Play();
    }

    private void ScheduleRemoval(Control message)
    {
        var timer = GetTree().CreateTimer(TimeOfMessages);
        timer.Timeout += () =>
        {
            if (IsInstanceValid(message))
                RemoveMessage(message);
        };
    }

    private void RemoveMessage(Control message)
    {
        if (message == null) return;
        _messages.Remove(message);
        message.QueueFree();
    }
}