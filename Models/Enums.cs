namespace Base2.Models;

/// <summary>
/// Типи вузлів документа "Добовий наряд"
/// </summary>
public enum NodeType
{
    // Структурні
    SectionHeader,             // Заголовок секції (корінь нумерації)
    LocationSection,           // Секція з контекстом локації

    // Позиції (HasWeapon/HasVehicle визначаються прапорцями вузла)
    SimplePosition,            // Одна особа
    DriverPosition,            // Водій з транспортом

    // Групи
    GroupInline,               // Група, список в рядок
    GroupNested,               // Група з підпунктами

    // Спеціальні
    TimeRange,                 // Часовий діапазон (зміна)
    MedicalPosition,           // Черговий медпункту

    // Вогневі групи
    FireGroupSection,          // Заголовок секції вогневих груп
    FireGroupLocation,         // Вогнева група в локації
    FireGroupInline            // Конкретна вогнева група inline
}

/// <summary>
/// Режим рендерингу вузла
/// </summary>
public enum RenderMode
{
    SingleLine,     // Один рядок: "1.1. Текст;"
    Inline,         // "1.6. Група в складі: A; B; C."
    Nested,         // "1.9. Група:\n  1.9.1. A;\n  1.9.2. B;"
    Header          // Просто заголовок
}
