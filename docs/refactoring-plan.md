# План рефакторингу Base2

## Мета

Відокремити бізнес-логіку від UI так, щоб те саме ядро можна було підключити до:
- **Blazor Server** — веб-інтерфейс, що працює подібно до поточного застосунку
- **Web API + Quasar PWA** — offline-first PWA із кешуванням довідників

## Принципи

- Мінімальний DI (без репозиторіїв, фабрик, медіаторів)
- EF Core залишається — прибирається лише пряме звернення до нього з UI-шарів
- Один проект ядра (`Base2.Core`), кілька UI-проектів
- Лише `Microsoft.Extensions.DependencyInjection` — без Autofac/Lamar тощо

---

## Поточний стан (проблема)

Форми звертаються до `AppDbContext` напряму:

```csharp
// AssignmentForm.cs — проблема
var ctx = AppServices.GetService<AppDbContext>();
var persons = ctx.Persons.Include(p => p.Rank).Where(...).ToList();
```

Це унеможливлює:
- Повторне використання логіки в Blazor/API
- Написання юніт-тестів без реальної БД
- Поступову міграцію на інший UI

---

## Фаза 1 — Виділення ядра `Base2.Core` *(обов'язкова)*

### 1.1 Створити `Base2.Core` (Class Library, `net10`)

```
Base2.Core/
├── Models/          ← перемістити без змін з Base2
├── Data/            ← AppDbContext, DatabaseInitializer
├── Services/        ← розширити існуючі + нові
├── Helpers/         ← NodeRenderer, RenderedLineBuilder
├── Dtos/            ← нові: легкі об'єкти передачі даних
├── Migrations/      ← перемістити без змін
└── AppServices.cs   ← DI registration
```

`Base2.csproj` додає `ProjectReference` на `Base2.Core.csproj`.  
`Base2.Core.csproj` не має `UseWindowsForms` — чиста .NET бібліотека.

### 1.2 Ввести DTO (об'єкти передачі даних)

Замість передачі EF-сутностей у форми — прості DTO без навігаційних властивостей:

```csharp
// Base2.Core/Dtos/

record PersonDto(int PersonId, string FullName, string Rank, string Position, string Initials);

record WeaponDto(int WeaponId, string Type, string Number, bool IsAvailableForAssignment);

record VehicleDto(int VehicleId, string Name, string Number, string Type);

record DutyOrderSummaryDto(int OrderId, string OrderNumber, DateTime OrderDate, string Status);

record AssignmentRequest(
    int DutySectionNodeId,
    int PersonId,
    int? WeaponId,
    int? VehicleId,
    int? AmmoCount,
    string? AmmoType
);
```

### 1.3 Розширити сервіси

#### `OrderService` (вже існує — розширити)

```csharp
// Вже є:
CreateOrderFromTemplate(int templateId, ...) → DutyOrder

// Додати:
GetOrdersForTemplate(int templateId) → List<DutyOrderSummaryDto>
GetOrderDetail(int orderId) → DutyOrderDetailDto   // з деревом вузлів
DeleteOrder(int orderId)
SaveAssignment(AssignmentRequest req) → DutyAssignment
RemoveAssignment(int assignmentId)
```

#### `TemplateService` (новий)

```csharp
GetTemplates(bool includeInactive = false) → List<DutyTemplateSummaryDto>
GetTemplateWithNodes(int templateId) → DutyTemplate
SaveTemplate(DutyTemplate template)
DeactivateTemplate(int templateId)
DeleteTemplate(int templateId)       // throws if orders exist
```

#### `ReferenceService` (новий)

```csharp
// Особи
GetPersons(string? filter = null) → List<PersonDto>
SavePerson(PersonDto dto)
DeletePerson(int personId)

// Зброя
GetWeapons(string? filter = null, int? locationId = null) → List<WeaponDto>
SaveWeapon(WeaponDto dto)
DeleteWeapon(int weaponId)

// Транспорт / Локації / Звання / Посади — аналогічно
```

#### `AppSettingsService` (вже є — залишити)

```csharp
GetSetting(string key) → string?
SetSetting(string key, string value)
```

### 1.4 Замінити прямі виклики DbContext у формах

**До:**
```csharp
var ctx = AppServices.GetService<AppDbContext>();
var persons = ctx.Persons.Include(p => p.Rank).ToList();
```

**Після:**
```csharp
var persons = _referenceService.GetPersons(filter: searchBox.Text);
```

Форми отримують сервіси через конструктор або `AppServices.GetService<T>()`.  
У формах — **лише UI-логіка**: відображення, прийом введення, виклик сервісів.

---

## Фаза 2 — WinForms стає чистим UI *(після Фази 1)*

```
Base2/
├── Forms/         ← UI-логіка, без LINQ і бізнес-умов
├── Program.cs
└── Base2.csproj   ← посилання на Base2.Core
```

Реєстрація DI:

```csharp
// AppServices.cs у Base2.Core
services.AddSingleton<AppDbContext>();
services.AddTransient<OrderService>();
services.AddTransient<TemplateService>();
services.AddTransient<ReferenceService>();
services.AddSingleton<AppSettingsService>();
```

---

## Фаза 3 — Blazor Server *(опційна)*

```
Base2.BlazorApp/
├── Pages/           ← Razor-компоненти
├── Shared/          ← Layout, NavMenu
└── Program.cs
    builder.Services.AddDbContext<AppDbContext>(...);
    builder.Services.AddScoped<OrderService>();
    builder.Services.AddScoped<TemplateService>();
    builder.Services.AddScoped<ReferenceService>();
```

- `AddScoped` замість `AddSingleton` (Blazor — багатокористувацьке середовище)
- Компоненти викликають ті самі сервіси з `Base2.Core`
- БД — той самий SQLite або PostgreSQL

---

## Фаза 4 — Web API + Quasar PWA *(опційна)*

### 4.1 `Base2.Api` (ASP.NET Core Web API)

```
Base2.Api/
├── Controllers/
│   ├── OrdersController.cs
│   ├── TemplatesController.cs
│   ├── PersonsController.cs
│   └── ReferenceController.cs
└── Program.cs
```

Тонкі контролери:

```csharp
[HttpGet("persons")]
public IActionResult GetPersons([FromQuery] string? filter)
    => Ok(_referenceService.GetPersons(filter));

[HttpPost("orders/{orderId}/assignments")]
public IActionResult Assign(int orderId, AssignmentRequest req)
    => Ok(_orderService.SaveAssignment(req));
```

### 4.2 Quasar PWA (offline-first)

```
base2-quasar/
├── src/
│   ├── stores/       ← Pinia: personsStore, weaponsStore, templatesStore
│   ├── pages/        ← MainPage, OrderPage, ReferencePage
│   ├── components/   ← AssignmentPanel, PositionTree, FilterGrid
│   └── services/
│       └── api.ts    ← axios з offline fallback
├── public/sw.js      ← Service Worker (Workbox)
└── quasar.config.js  ← mode: pwa
```

**Стратегія offline:**

| Дані | Стратегія |
|------|-----------|
| Довідники (особи, зброя, транспорт, локації) | Cache-first, оновлення при старті |
| Шаблони | Cache-first, оновлення за версією |
| Поточний наряд (заповнення) | Local-first, синхронізація при підключенні |
| Архівні наряди | Network-first із fallback на кеш |

---

## Структура рішення після рефакторингу

```
Base2.sln
├── Base2.Core/        ← ЯДРО: Models + Data + Services + Helpers + Migrations
├── Base2/             ← WinForms UI
├── Base2.BlazorApp/   ← Blazor Server        [опційно]
├── Base2.Api/         ← ASP.NET Web API      [опційно]
└── base2-quasar/      ← Quasar PWA Frontend  [опційно]
```

---

## Дорожня карта

| Крок | Дія | Складність | Пріоритет |
|------|-----|-----------|-----------|
| 1 | Створити `Base2.Core`, перемістити Models + Data + Migrations | Низька | 🔴 Обов'язковий |
| 2 | Виділити DTO (PersonDto, WeaponDto, DutyOrderSummaryDto) | Низька | 🔴 Обов'язковий |
| 3 | Реалізувати `TemplateService` та `ReferenceService` | Середня | 🔴 Обов'язковий |
| 4 | Розширити `OrderService` методами DTO | Середня | 🔴 Обов'язковий |
| 5 | Замінити прямий DbContext у всіх формах → сервіси | Середня | 🔴 Обов'язковий |
| 6 | Blazor Server proof-of-concept (MainPage + OrderPage) | Середня | 🟡 Бажаний |
| 7 | Web API skeleton + Quasar PWA | Висока | 🟢 Майбутнє |
