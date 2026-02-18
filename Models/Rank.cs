namespace Base2.Models;

/// <summary>
/// Військове звання
/// </summary>
public class Rank
{
    public int RankId { get; set; }

    /// <summary>
    /// Назва звання
    /// </summary>
    public string RankName { get; set; } = string.Empty;

    /// <summary>
    /// Рівень звання для сортування (1 = солдат, 11 = майор)
    /// </summary>
    public int RankLevel { get; set; }

    // Навігація
    public List<Person> People { get; set; } = new();
}
