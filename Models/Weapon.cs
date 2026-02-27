namespace Base2.Models;

/// <summary>
/// Зброя
/// </summary>
public class Weapon
{
    public int WeaponId { get; set; }

    /// <summary>
    /// Тип зброї (АК-74, ПМ, АКМ-47, FNC)
    /// </summary>
    public string WeaponType { get; set; } = string.Empty;

    /// <summary>
    /// Номер зброї (175978, ШИ2840)
    /// </summary>
    public string WeaponNumber { get; set; } = string.Empty;

    /// <summary>
    /// Де зберігається (опціонально)
    /// </summary>
    public int? StoredInLocationId { get; set; }

    /// <summary>
    /// За ким закріплена (опціонально)
    /// </summary>
    public int? AssignedToPersonId { get; set; }

    // Навігація
    public Location? StoredInLocation { get; set; }
    public Person? AssignedToPerson { get; set; }
}
