namespace Base2.Models;

/// <summary>
/// Налаштування застосунку (key-value).
/// </summary>
public class AppSetting
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
}
