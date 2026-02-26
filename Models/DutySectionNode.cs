using System.Collections.Generic;

namespace Base2.Models;

/// <summary>
/// Вузол дерева документа "Добовий наряд".
/// Належить або шаблону (DutyTemplateId), або конкретному наказу (DutyOrderId).
/// </summary>
public class DutySectionNode
{
    public int DutySectionNodeId { get; set; }

    // FK — самопосилання
    public int? ParentDutySectionNodeId { get; set; }

    // Належність: шаблон АБО наказ (одне з двох)
    public int? DutyTemplateId { get; set; }
    public int? DutyOrderId { get; set; }

    /// <summary>
    /// Тип вузла (визначає шаблон рендерингу)
    /// </summary>
    public NodeType NodeType { get; set; }

    /// <summary>
    /// Порядок серед siblings (для сортування та нумерації)
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Номер пункту (генерується автоматично: "1.2.3").
    /// Нумерація починається з SectionHeader.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Текстовий шаблон з плейсхолдерами.
    /// Приклади:
    ///   "Черговий містечка – {Rank} {LastName} {Initials}"
    ///   "Добовий наряд по парку в складі:"
    ///   "Військове містечко №1 (Седнів)"
    /// </summary>
    public string? DutyPositionTitle { get; set; }

    // ── Прапорці ролі (визначають UI та рендеринг) ──

    /// <summary>
    /// Чи передбачена зброя для цієї ролі в наряді
    /// </summary>
    public bool HasWeapon { get; set; }

    /// <summary>
    /// Чи передбачені набої
    /// </summary>
    public bool HasAmmo { get; set; }

    /// <summary>
    /// Чи передбачений транспорт
    /// </summary>
    public bool HasVehicle { get; set; }

    /// <summary>
    /// Максимальна кількість призначень.
    /// 1 = SimplePosition (одна особа), 0 = необмежено (для груп).
    /// </summary>
    public int MaxAssignments { get; set; } = 1;

    // ── Для вузлів TimeRange ──

    /// <summary>
    /// Підпис часового діапазону для відображення на панелі форми.
    /// Наприклад: "Зміна 1 (08:00–20:00)", "Нічна зміна".
    /// </summary>
    public string? TimeRangeLabel { get; set; }

    /// <summary>
    /// FK до конкретного часового діапазону (заповнюється лише в наказі, не в шаблоні)
    /// </summary>
    public int? DutyTimeRangeId { get; set; }

    /// <summary>
    /// FK до локації (опціонально).
    /// Використовується для фільтрації зброї при призначенні.
    /// Якщо null — зброя не фільтрується за локацією.
    /// </summary>
    public int? LocationId { get; set; }

    // ── Навігація ──
    public DutySectionNode? Parent { get; set; }
    public List<DutySectionNode> Children { get; set; } = new();
    public List<DutyAssignment> Assignments { get; set; } = new();
    public DutyTimeRange? DutyTimeRange { get; set; }
    public DutyTemplate? DutyTemplate { get; set; }
    public DutyOrder? DutyOrder { get; set; }
    public Location? Location { get; set; }
}
