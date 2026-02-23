using System;
using System.Collections.Generic;

namespace Base2.Models;

/// <summary>
/// Наказ "Добовий наряд", створений на основі шаблону.
/// </summary>
public class DutyOrder
{
    public int DutyOrderId { get; set; }

    /// <summary>
    /// Шаблон, з якого створено наказ
    /// </summary>
    public int SourceTemplateId { get; set; }

    /// <summary>
    /// Номер наказу (А4463/19-01-2026)
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Дата наказу
    /// </summary>
    public DateOnly OrderDate { get; set; }

    /// <summary>
    /// Початок дії наказу
    /// </summary>
    public DateTime StartDateTime { get; set; }

    /// <summary>
    /// Кінець дії наказу
    /// </summary>
    public DateTime EndDateTime { get; set; }

    /// <summary>
    /// Інформація про командира
    /// </summary>
    public string CommanderInfo { get; set; } = string.Empty;

    // Навігація
    public DutyTemplate SourceTemplate { get; set; } = null!;
    public List<DutySectionNode> Sections { get; set; } = new();
    public List<DutyTimeRange> TimeRanges { get; set; } = new();
}
