namespace Base2.Models;

/// <summary>
/// Транспортний засіб
/// </summary>
public class Vehicle
{
    public int VehicleId { get; set; }

    /// <summary>
    /// Назва транспорту (Мерседес, Форд Транзіт)
    /// </summary>
    public string VehicleName { get; set; } = string.Empty;

    /// <summary>
    /// Номерний знак (4605Р2)
    /// </summary>
    public string VehicleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Тип транспорту (легковий, вантажний, санітарний)
    /// </summary>
    public string? VehicleType { get; set; }
}
