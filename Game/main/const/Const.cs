using System;

/// <summary>
/// Общие константы игры: Z-индексы, слои коллизий и глобальные флаги.
/// Слои задаются битовыми масками Godot.
/// </summary>
public static class Consts
{
    //--------------------------------
    //              СЛОИ
    //--------------------------------

    /// <summary>
    /// Z-индекс умерших существ.
    /// </summary>
    public const int DiedIndex = 13;

    /// <summary>
    /// Базовый Z-индекс живых существ.
    /// </summary>
    public const int EntityIndex = 18;

    /// <summary>
    /// Слой живых существ: игрок, NPC, враги.
    /// </summary>
    public const int EntityLayer = 1 << 0; // 1

    /// <summary>
    /// Слой трупов. Не блокирует движение и не попадает под атаки.
    /// </summary>
    public const int CorpseLayer = 1 << 1; // 2

    /// <summary>
    /// Всё, с чем можно взаимодействовать: живые существа и трупы.
    /// </summary>
    public const int InteractableLayer = EntityLayer | CorpseLayer; // 3

    /// <summary>
    /// Слой хитбокса атаки.
    /// </summary>
    public const int AttackLayer = 1 << 2; // 4

    /// <summary>
    /// Глобальная остановка игрового процесса.
    /// </summary>
    public static bool GameStopedState = false;
}