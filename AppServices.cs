using Base2.Data;
using Base2.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Base2;

/// <summary>
/// Простий DI-контейнер для додатку.
/// Надає єдиний AppDbContext для всіх форм.
/// </summary>
public static class AppServices
{
    private static IServiceProvider? _provider;

    public static void Initialize()
    {
        var services = new ServiceCollection();

        // Singleton: один контекст на весь час життя додатку
        services.AddSingleton<AppDbContext>();

        // Transient: новий екземпляр при кожному запиті
        services.AddTransient<OrderService>();

        _provider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Отримати зареєстрований сервіс
    /// </summary>
    public static T Get<T>() where T : notnull
        => _provider!.GetRequiredService<T>();

    /// <summary>
    /// Швидкий доступ до AppDbContext
    /// </summary>
    public static AppDbContext DbContext => Get<AppDbContext>();

    /// <summary>
    /// Звільнити ресурси при завершенні додатку
    /// </summary>
    public static void Dispose()
    {
        (_provider as IDisposable)?.Dispose();
        _provider = null;
    }
}
