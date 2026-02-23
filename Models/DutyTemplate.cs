namespace Base2.Models;

/// <summary>
/// Шаблон (зразкова структура) для створення наказів добового наряду.
/// Містить лише дерево вузлів без призначень.
/// </summary>
public class DutyTemplate
{
    public int DutyTemplateId { get; set; }

    /// <summary>
    /// Назва шаблону ("Основний добовий наряд", "Святковий наряд")
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// Опис
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Чи активний шаблон (для вибору при створенні наказу)
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Навігація
    public List<DutySectionNode> Sections { get; set; } = new();
    public List<DutyOrder> Orders { get; set; } = new();
}