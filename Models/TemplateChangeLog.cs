namespace Base2.Models;

/// <summary>
/// Журнал змін шаблону добового наряду.
/// </summary>
public class TemplateChangeLog
{
    public int TemplateChangeLogId { get; set; }

    /// <summary>
    /// FK до шаблону
    /// </summary>
    public int DutyTemplateId { get; set; }

    /// <summary>
    /// Версія шаблону на момент зміни
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Дата та час зміни
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Хто вніс зміну (ім'я користувача або системне ім'я)
    /// </summary>
    public string ChangedBy { get; set; } = Environment.UserName;

    /// <summary>
    /// Опис зміни
    /// </summary>
    public string ChangeDescription { get; set; } = string.Empty;

    // Навігація
    public DutyTemplate DutyTemplate { get; set; } = null!;
}
