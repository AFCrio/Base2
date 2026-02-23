namespace Base2.Models;

/// <summary>
/// Військова посада (курсант, начальник служби тощо).
/// Не визначає наявність зброї — це залежить від ролі в наряді.
/// </summary>
public class Position
{
    public int PositionId { get; set; }

    /// <summary>
    /// Назва посади
    /// </summary>
    public string PositionName { get; set; } = string.Empty;

    // Навігація
    public List<Person> People { get; set; } = new();
}
