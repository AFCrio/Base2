namespace Base2.Models;

/// <summary>
/// Підстановка типу і кількості набоїв за типом зброї.
/// </summary>
public class WeaponAmmoPreset
{
    public int WeaponAmmoPresetId { get; set; }

    /// <summary>
    /// Тип зброї (наприклад: ПМ, АК-47).
    /// </summary>
    public string WeaponType { get; set; } = string.Empty;

    /// <summary>
    /// Тип набоїв (наприклад: 9 мм, 5,45 мм).
    /// </summary>
    public string AmmoType { get; set; } = string.Empty;

    /// <summary>
    /// Кількість набоїв за замовчуванням.
    /// </summary>
    public int AmmoCount { get; set; }
}
