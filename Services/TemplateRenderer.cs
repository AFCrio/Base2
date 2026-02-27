using Base2.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Base2.Services;

/// <summary>
/// Рендеринг текстових шаблонів DutyPositionTitle
/// </summary>
public static class TemplateRenderer
{
    private static readonly Regex _placeholderRegex = new(@"\{[A-Za-z]+\}", RegexOptions.Compiled);

    private static readonly string[] MonthsGenitive =
    [
        "", "січня", "лютого", "березня", "квітня", "травня",
        "червня", "липня", "серпня", "вересня", "жовтня", "листопада", "грудня"
    ];

    /// <summary>
    /// Замінює плейсхолдери в DutyPositionTitle на реальні дані.
    /// Порядок підстановки: наказ → часовий діапазон → особові дані.
    /// Якщо вузол має прапорці (HasWeapon, HasAmmo, HasVehicle), але шаблон
    /// не містить відповідних плейсхолдерів — дані дописуються автоматично.
    /// Незамінені плейсхолдери стають "___".
    /// </summary>
    public static string Render(
        string template,
        DutyAssignment? assignment = null,
        DutyTimeRange? timeRange = null,
        DutyOrder? order = null,
        DutySectionNode? node = null)
    {
        var result = template;

        // ── Дані наказу ──
        if (order != null)
        {
            result = result.Replace("{OrderPeriod}", FormatOrderPeriod(order.StartDateTime, order.EndDateTime));
            result = result.Replace("{CommanderInfo}", order.CommanderInfo);
        }

        // ── Часовий діапазон (зміна) ──
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

        // ── Автодоповнення за прапорцями вузла ──
        if (node != null && assignment != null)
        {
            var extras = new List<string>();

            // Зброя
            if (!template.Contains("{WeaponType}") && !template.Contains("{WeaponNumber}"))
            {
                if (node.HasWeapon)
                {
                    var w = assignment.Weapon;
                    if (w != null)
                        extras.Add($"озброїти {w.WeaponType} №{w.WeaponNumber}");
                }
                else
                {
                    extras.Add("без зброї");
                }
            }

            // Набої
            if (node.HasAmmo && !template.Contains("{AmmoType}") && !template.Contains("{AmmoCount}"))
            {
                if (assignment.AmmoCount.HasValue)
                    extras.Add($"видати набої {assignment.AmmoType} – {assignment.AmmoCount} шт.");
            }

            // Транспорт
            if (node.HasVehicle && !template.Contains("{VehicleName}") && !template.Contains("{VehicleNumber}"))
            {
                var v = assignment.Vehicle;
                if (v != null)
                    extras.Add($"транспорт {v.VehicleName} {v.VehicleNumber}");
            }

            if (extras.Count > 0)
                result += ", " + string.Join(", ", extras) + ";";
        }

        return result;
    }

    /// <summary>
    /// Форматує період дії наказу українською мовою.
    /// Приклади:
    ///   Один місяць:    "з 19 по 20 лютого 2026 року"
    ///   Різні місяці:   "з 31 січня по 1 лютого 2026 року"
    ///   Різні роки:     "з 31 грудня 2025 по 1 січня 2026 року"
    /// </summary>
    public static string FormatOrderPeriod(DateTime start, DateTime end)
    {
        int sd = start.Day, sm = start.Month, sy = start.Year;
        int ed = end.Day, em = end.Month, ey = end.Year;

        if (sy != ey)
        {
            // з 31 грудня 2025 по 1 січня 2026 року
            return $"з {sd} {MonthsGenitive[sm]} {sy} по {ed} {MonthsGenitive[em]} {ey} року";
        }

        if (sm != em)
        {
            // з 31 січня по 1 лютого 2026 року
            return $"з {sd} {MonthsGenitive[sm]} по {ed} {MonthsGenitive[em]} {ey} року";
        }

        // з 19 по 20 лютого 2026 року
        return $"з {sd} по {ed} {MonthsGenitive[em]} {ey} року";
    }
}