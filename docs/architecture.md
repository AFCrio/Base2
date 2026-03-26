# Архітектура Base2

## Огляд

Base2 — Windows Forms застосунок на .NET 10 із SQLite базою даних через EF Core. Архітектура монолітна, але зі чітким розподілом відповідальностей: Models → Data → Services → Forms.

```
┌──────────────────────────────────────────────────────────────┐
│                         Forms (UI)                            │
│  MainForm  TemplateEditorForm  OrderCreatorForm  ReferenceForm│
└───────────────────────┬──────────────────────────────────────┘
                        │  викликають
┌───────────────────────▼──────────────────────────────────────┐
│                       Services                                │
│         OrderService        AppSettingsService                │
│              +                                                │
│         Helpers: NodeRenderer, RenderedLineBuilder            │
└───────────────────────┬──────────────────────────────────────┘
                        │  звертаються через
┌───────────────────────▼──────────────────────────────────────┐
│                  Data / AppDbContext (EF Core)                 │
│                     DutyOrder.db (SQLite)                     │
└──────────────────────────────────────────────────────────────┘
```

## DI-контейнер

`AppServices.cs` — точка реєстрації (`Microsoft.Extensions.DependencyInjection`):

```csharp
services.AddSingleton<AppDbContext>();   // єдиний екземпляр на весь сеанс
services.AddTransient<OrderService>();  // новий екземпляр при кожному запиті
```

Ініціалізація у `Program.cs`:

```csharp
AppServices.Initialize();          // DI
DatabaseInitializer.Initialize();  // EF міграції
Application.Run(new MainForm());
```

## Схема бази даних

### Довідники (Reference data)

```
Rank          Position        Location
─────         ────────        ────────
RankId (PK)   PositionId (PK) LocationId (PK)
RankName      PositionName    LocationName
RankLevel                     Address

Person
──────
PersonId (PK)
LastName / FirstName / MiddleName / Initials
RankId (FK → Rank, RESTRICT)
PositionId (FK → Position, RESTRICT)

Weapon                          Vehicle
──────                          ───────
WeaponId (PK)                   VehicleId (PK)
WeaponType / WeaponNumber       VehicleName / VehicleNumber / VehicleType
StoredInLocationId (FK, SET NULL)
AssignedToPersonId (FK, SET NULL)
LastUsedDate

WeaponAmmoPreset                AppSetting
────────────────                ──────────
WeaponAmmoPresetId (PK)         Key (PK)
WeaponType (unique)             Value
AmmoType / AmmoCount
```

### Шаблони та наряди

```
DutyTemplate
────────────
DutyTemplateId (PK)
TemplateName / Description
IsActive / Version
CreatedAt / UpdatedAt
    │
    ├──► TemplateChangeLog
    │    ─────────────────
    │    TemplateChangeLogId (PK)
    │    DutyTemplateId (FK, CASCADE)
    │    Version / ChangedAt / ChangedBy / ChangeDescription
    │
    └──► DutyOrder
         ─────────
         DutyOrderId (PK)
         OrderNumber (unique) / OrderDate
         StartDateTime / EndDateTime
         CommanderInfo
         SourceTemplateId (FK → DutyTemplate, RESTRICT)
         SourceTemplateVersion
             │
             ├──► DutyTimeRange
             │    ────────────
             │    DutyTimeRangeId (PK)
             │    DutyOrderId (FK, CASCADE)
             │    Label / Start / End
             │    [computed: StartTime, StartDate, EndTime, EndDate]
             │
             └──► DutySectionNode  (self-referencing tree)
                  ─────────────────
                  DutySectionNodeId (PK)
                  ParentDutySectionNodeId (FK self, CASCADE)
                  DutyTemplateId (FK, CASCADE)
                  DutyOrderId (FK, CASCADE)
                  DutyTimeRangeId (FK, SET NULL)
                  LocationId (FK, SET NULL)
                  NodeType (enum as string)
                  OrderIndex / Title
                  DutyPositionTitle / GroupItemTemplate
                  HasWeapon / HasAmmo / HasVehicle
                  MaxAssignments / TimeRangeLabel
                      │
                      └──► DutyAssignment
                           ──────────────
                           DutyAssignmentId (PK)
                           DutySectionNodeId (FK, CASCADE)
                           PersonId (FK, RESTRICT)
                           WeaponId (FK, SET NULL)
                           VehicleId (FK, SET NULL)
                           AmmoCount / AmmoType
                           RenderedLine  ← текстовий знімок
```

### NodeType (перелік)

| Значення       | Призначення                                          |
|----------------|------------------------------------------------------|
| `Section`      | Розділ з локацією (пост, об'єкт)                    |
| `Group`        | Підгрупа позицій усередині розділу                  |
| `DutyPosition` | Конкретна посада, приймає призначення               |
| `TimeRange`    | Мітка зміни (перша/друга зміна тощо)                |

## Потік даних: створення наряду

```
1. Користувач обирає DutyTemplate
2. OrderService.CreateOrderFromTemplate()
   → Копіює дерево DutySectionNode з template → order
   → Зберігає DutyOrder із метаданими
3. Користувач налаштовує DutyTimeRange (конкретні години)
4. Для кожної DutyPosition:
   → AssignmentForm: пошук Person за прізвищем
   → Вибір Weapon (відфільтровано за Location вузла)
   → Амуніція підставляється з WeaponAmmoPreset
   → Вибір Vehicle
   → NodeRenderer.Render() → RenderedLine (snapshot)
   → Зберігається DutyAssignment
```

## Ключові архітектурні рішення

| Рішення | Обґрунтування |
|---------|---------------|
| `RenderedLine` у DutyAssignment | Зміна прізвища/звання у довіднику не псує архівні наряди |
| Зберігати лише PersonId/WeaponId/VehicleId для аналітики | Знеособлення: текст у RenderedLine без аналітичних потреб |
| Enum NodeType як string у БД | Читабельність міграцій, легке розширення |
| `AddSingleton<AppDbContext>` | WinForms — однопотоковий, один контекст на сесію |
| Версіонування шаблонів | Зміни у шаблоні не впливають на вже створені наряди |
| FK RESTRICT на Person при DutyAssignment | Неможливо видалити особу, яка є у наряді |
| FK SET NULL на Weapon/Vehicle | Зброю/транспорт можна переприсвоїти без втрати наряду |

## Міграції (хронологія)

| Міграція | Дата | Зміни |
|----------|------|-------|
| InitialCreate | 2026-02-18 | Базова схема |
| ref3 | 2026-02-20 | Коригування довідників |
| AddVersioningAndChangeLog | 2026-02-24 | Версіонування шаблонів |
| AddLocationToSectionNode | 2026-02-26 | LocationId у вузлі (фільтрація зброї) |
| AddWeaponAssignedToPerson | 2026-02-26 | Прив'язка зброї до особи |
| RemoveTemplateNode | 2026-02-27 | Консолідація у DutySectionNode |
| AddUniqueIndexesAndCascadeDelete | 2026-02-27 | Унікальні індекси та каскади |
| AddWeaponLastUsedDate | 2026-03-04 | Дата останнього використання |
| AddWeaponAmmoPresetsTable | 2026-03-15 | Пресети амуніції + seed |
| AddAppSettingsTable | 2026-03-15 | Налаштування застосунку |
| AddDutyAssignmentRenderedLine | 2026-03-25 | Текстовий знімок призначення |
| AddGroupTemplatesToDutySectionNode | 2026-03-25 | GroupItemTemplate |
| RemoveGroupHeaderTemplateFromDutySectionNode | 2026-03-25 | Прибрати GroupHeaderTemplate |
