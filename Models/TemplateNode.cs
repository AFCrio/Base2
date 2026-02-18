namespace Base2.Models;

/// <summary>
/// Шаблон рендерингу для типу вузла
/// </summary>
public class TemplateNode
{
    public int TemplateNodeId { get; set; }

    /// <summary>
    /// Тип вузла, для якого цей шаблон
    /// </summary>
    public NodeType NodeType { get; set; }

    /// <summary>
    /// Шаблон тексту з плейсхолдерами
    /// Приклад: "{Title}. {DutyPositionTitle} – {Assignment.Person.Rank.RankName} {Assignment.Person.LastName} {Assignment.Person.Initials}"
    /// </summary>
    public string TemplateText { get; set; } = string.Empty;

    /// <summary>
    /// Чи потрібна зброя для цього типу вузла
    /// </summary>
    public bool RequiresWeapon { get; set; }

    /// <summary>
    /// Чи потрібні набої для цього типу вузла
    /// </summary>
    public bool RequiresAmmo { get; set; }

    /// <summary>
    /// Чи потрібен транспорт для цього типу вузла
    /// </summary>
    public bool RequiresVehicle { get; set; }

    /// <summary>
    /// Режим рендерингу
    /// </summary>
    public RenderMode RenderMode { get; set; }

    /// <summary>
    /// Роздільник між заголовком та списком (" в складі: ")
    /// </summary>
    public string? Separator { get; set; }

    /// <summary>
    /// Роздільник між елементами списку ("; ")
    /// </summary>
    public string? ItemSeparator { get; set; }

    /// <summary>
    /// Завершувач рядка (";" або ".")
    /// </summary>
    public string? Terminator { get; set; }
}
