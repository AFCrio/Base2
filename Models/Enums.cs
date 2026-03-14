namespace Base2.Models;

/// <summary>
/// Типи вузлів документа "Добовий наряд"
/// </summary>
public enum NodeType
{
    // Структурні
    SectionHeader,             // Заголовок секції (корінь нумерації, опціонально з локацією)

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


