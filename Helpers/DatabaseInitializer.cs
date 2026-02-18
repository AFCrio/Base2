using Base2.Data;
using Microsoft.EntityFrameworkCore;

namespace Base2.Helpers;

/// <summary>
/// Допоміжний клас для ініціалізації БД
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Створити БД та застосувати всі міграції
    /// </summary>
    public static void Initialize()
    {
        using var context = new AppDbContext();

        try
        {
            // Створюємо БД якщо не існує та застосовуємо міграції
            context.Database.Migrate();

            Console.WriteLine("База даних успішно створена/оновлена!");
            Console.WriteLine($"Шлях до БД: {GetDatabasePath()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при створенні БД: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Перевірити чи існує БД
    /// </summary>
    public static bool DatabaseExists()
    {
        using var context = new AppDbContext();
        return context.Database.CanConnect();
    }

    /// <summary>
    /// Отримати шлях до файлу БД
    /// </summary>
    public static string GetDatabasePath()
    {
        return Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "DutyOrder.db"
        );
    }

    /// <summary>
    /// Видалити БД (для розробки)
    /// </summary>
    public static void DeleteDatabase()
    {
        string dbPath = GetDatabasePath();
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
            Console.WriteLine("База даних видалена!");
        }
    }
}
