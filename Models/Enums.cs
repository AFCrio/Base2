namespace Base2.Models;

/// <summary>
/// Типи вузлів документа "Добовий наряд"
/// </summary>
public enum NodeType
{
    // Структурні
    DocumentRoot,              // Корінь документа (заголовок наказу)
    SectionHeader,             // Заголовок секції без даних
    LocationSection,           // Секція з локацією

    // Прості позиції
    SimplePosition,            // Особа зі зброєю
    SimplePositionNoWeapon,    // Особа без зброї
    DriverPosition,            // Водій з транспортом

    // Групи inline
    GroupInline,               // Група зі зброєю, в один рядок
    GroupInlineNoWeapon,       // Група без зброї, в один рядок

    // Групи nested
    GroupNested,               // Група без зброї, з підпунктами
    GroupNestedWithWeapon,     // Група зі зброєю, з підпунктами

    // Спеціальні
    TimeRange,                 // Часовий діапазон
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
