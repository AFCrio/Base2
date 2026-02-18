namespace Base2.Models;

/// <summary>
/// Військова посада
/// </summary>
public class Position
{
    public int PositionId { get; set; }

    /// <summary>
    /// Назва посади
    /// </summary>
    public string PositionName { get; set; } = string.Empty;

    /// <summary>
    /// Чи передбачена зброя для посади
    /// </summary>
    public bool HasWeapon { get; set; }

    /// <summary>
    /// Чи передбачені набої для посади
    /// </summary>
    public bool HasAmmo { get; set; }

    /// <summary>
    /// Чи це водійська посада
    /// </summary>
    public bool IsDriver { get; set; }

    // Навігація
    public List<Person> People { get; set; } = new();
}
