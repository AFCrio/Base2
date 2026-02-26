
namespace Base2.Models;

/// <summary>
/// Локація (військове містечко)
/// </summary>
public class Location
{
    public int LocationId { get; set; }

    /// <summary>
    /// Назва локації
    /// </summary>
    public string LocationName { get; set; } = string.Empty;

    /// <summary>
    /// Повна адреса (опціонально)
    /// </summary>
    public string? Address { get; set; }

    // Навігація
    public List<Weapon> StoredWeapons { get; set; } = new();
    public List<DutySectionNode> SectionNodes { get; set; } = new();
}
