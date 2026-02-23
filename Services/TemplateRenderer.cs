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
    /// Замінює плейсхолдери в DutyPositionTitle на реальні дані з Assignment.
    /// Підтримувані плейсхолдери:
    ///   {Rank}, {LastName}, {Initials}, {FullName},
    ///   {WeaponType}, {WeaponNumber}, {AmmoType}, {AmmoCount},
    ///   {VehicleName}, {VehicleNumber}, {Position}
    /// </summary>
    public static string Render(string template, DutyAssignment? assignment)
    {
        if (assignment is null)
            return _placeholderRegex.Replace(template, "___");

        var result = template;
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

        return result;
    }
}