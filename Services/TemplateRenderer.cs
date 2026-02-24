using Base2.Models;
using System.Text.RegularExpressions;

namespace Base2.Services;

/// <summary>
/// Рендеринг текстових шаблонів DutyPositionTitle
/// </summary>
public static class TemplateRenderer
{
    private static readonly Regex _placeholderRegex = new(@"\{[A-Za-z]+\}", RegexOptions.Compiled);

    /// <summary>
    /// Замінює плейсхолдери в DutyPositionTitle на реальні дані.
    /// Спочатку підставляє дані часового діапазону, потім особові дані.
    /// Незамінені плейсхолдери стають "___".
    /// </summary>
    /// <param name="template">Текстовий шаблон з плейсхолдерами</param>
    /// <param name="assignment">Призначення (особа, зброя, транспорт) — може бути null</param>
    /// <param name="timeRange">Часовий діапазон (зміна) — може бути null</param>
    public static string Render(string template, DutyAssignment? assignment = null, DutyTimeRange? timeRange = null)
    {
        var result = template;

        // ── Часовий діапазон ──
        if (timeRange != null)
        {
            result = result.Replace("{TimeLabel}", timeRange.Label);
            result = result.Replace("{StartTime}", timeRange.StartTime);
            result = result.Replace("{EndTime}", timeRange.EndTime);
            result = result.Replace("{StartDate}", timeRange.StartDate);
            result = result.Replace("{EndDate}", timeRange.EndDate);
        }

        // ── Особові дані ──
        if (assignment != null)
        {
            var person = assignment.Person;

            result = result.Replace("{Rank}", person.Rank?.RankName ?? "");
            result = result.Replace("{LastName}", person.LastName);
            result = result.Replace("{Initials}", person.Initials ?? "");
            result = result.Replace("{FullName}", $"{person.LastName} {person.Initials}");
            result = result.Replace("{Position}", person.Position?.PositionName ?? "");

            result = result.Replace("{WeaponType}", assignment.Weapon?.WeaponType ?? "");
            result = result.Replace("{WeaponNumber}", assignment.Weapon?.WeaponNumber ?? "");
            result = result.Replace("{AmmoType}", assignment.AmmoType ?? "");
            result = result.Replace("{AmmoCount}", assignment.AmmoCount?.ToString() ?? "");

            result = result.Replace("{VehicleName}", assignment.Vehicle?.VehicleName ?? "");
            result = result.Replace("{VehicleNumber}", assignment.Vehicle?.VehicleNumber ?? "");
        }

        // Незамінені плейсхолдери → "___"
        result = _placeholderRegex.Replace(result, "___");

        return result;
    }
}