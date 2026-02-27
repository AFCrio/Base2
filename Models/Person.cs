using Base2.Models;

namespace Base2.Models;

/// <summary>
/// Військовослужбовець
/// </summary>
public class Person
{
    public int PersonId { get; set; }

    /// <summary>
    /// Прізвище (ВЕЛИКИМИ ЛІТЕРАМИ)
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Ім'я (перша літера)
    /// </summary>
    public string? FirstName { get; set; } = string.Empty;

    /// <summary>
    /// По батькові (перша літера, може бути порожнім)
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Ініціали (В.М. або В.)
    /// </summary>
    public string? Initials { get; set; } = string.Empty;

    // FK
    public int RankId { get; set; }
    public int PositionId { get; set; }

    // Навігація
    public Rank Rank { get; set; } = null!;
    public Position Position { get; set; } = null!;
    public List<Weapon> AssignedWeapons { get; set; } = new();
}
