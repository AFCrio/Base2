using System;

namespace Base2.Models;

/// <summary>
/// Конкретний часовий діапазон для наказу.
/// </summary>
public class DutyTimeRange
{
    public int DutyTimeRangeId { get; set; }

    /// <summary>
    /// FK до наказу
    /// </summary>
    public int DutyOrderId { get; set; }

    /// <summary>
    /// Мітка (копіюється з TimeRangeLabel вузла): "Зміна 1", "Нічна зміна"
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Початок чергування
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// Кінець чергування
    /// </summary>
    public DateTime End { get; set; }

    // Форматовані властивості для шаблонів
    public string StartTime => Start.ToString("HH:mm");
    public string StartDate => Start.ToString("dd.MM.yyyy");
    public string EndTime => End.ToString("HH:mm");
    public string EndDate => End.ToString("dd.MM.yyyy");

    // Навігація
    public DutyOrder DutyOrder { get; set; } = null!;
}
