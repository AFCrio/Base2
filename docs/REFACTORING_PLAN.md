# План рефакторингу: відділення логіки від представлення

## 🎯 Мета

Підготувати ядро додатку для повторного використання у:
1. **Blazor Server/WASM** — веб-версія з тією ж логікою
2. **WebAPI + Quasar PWA** — REST API бекенд + PWA фронт з кешуванням довідників і offline-роботою

### Принципи
- ❌ Без складних паттернів (Repository, Unit of Work, CQRS)
- ❌ Без фабрик та абстрактних фабрик
- ✅ Мінімальний DI (Microsoft.Extensions.DependencyInjection)
- ✅ Простий поділ: **Core** (логіка + дані) і **UI** (представлення)
- ✅ Сервісний шар як єдина точка доступу до бізнес-логіки

---

## 📐 Цільова архітектура

```
┌─────────────────────────────────────────────────────────┐
│                    UI / Presentation                      │
│  ┌──────────┐  ┌──────────────┐  ┌───────────────────┐  │
│  │ WinForms │  │ Blazor Server│  │ Quasar PWA (Vue)  │  │
│  │  Forms/  │  │   Pages/     │  │  + offline cache  │  │
│  └────┬─────┘  └──────┬───────┘  └─────────┬─────────┘  │
│       │               │                     │            │
│       ▼               ▼                     ▼            │
│  ┌─────────────────────────────────────────────────┐     │
│  │              WebAPI Controllers                  │     │
│  │         (тільки для Quasar PWA варіанту)         │     │
│  └───────────────────┬─────────────────────────────┘     │
└──────────────────────┼───────────────────────────────────┘
                       │
┌──────────────────────┼───────────────────────────────────┐
│                      ▼     Core / Business Logic         │
│  ┌────────────────────────────────────────────────────┐  │
│  │                   Services                          │  │
│  │  OrderService, TemplateService, ReferenceService,  │  │
│  │  AssignmentService, TemplateRenderer, Validators    │  │
│  └───────────────────┬────────────────────────────────┘  │
│                      │                                    │
│  ┌───────────────────┼────────────────────────────────┐  │
│  │                   ▼     Data                        │  │
│  │  AppDbContext, Models, Migrations, DTOs             │  │
│  └────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────┘
```

---

## 📦 Етап 1: Виділення Core-бібліотеки

### 1.1 Створити проєкт `Base2.Core`

Новий Class Library проєкт з усією бізнес-логікою та даними.

**Структура `Base2.Core`:**
```
Base2.Core/
├── Models/              # Усі моделі (без змін)
│   ├── Person.cs
│   ├── Rank.cs
│   ├── Position.cs
│   ├── Weapon.cs
│   ├── Vehicle.cs
│   ├── Location.cs
│   ├── DutyTemplate.cs
│   ├── DutySectionNode.cs
│   ├── DutyOrder.cs
│   ├── DutyAssignment.cs
│   ├── DutyTimeRange.cs
│   ├── WeaponAmmoPreset.cs
│   ├── AppSetting.cs
│   ├── TemplateChangeLog.cs
│   └── Enums.cs
├── Data/
│   └── AppDbContext.cs   # EF Core контекст
├── Services/
│   ├── OrderService.cs        # Створення/видалення наказів
│   ├── TemplateService.cs     # CRUD шаблонів (НОВИЙ)
│   ├── AssignmentService.cs   # Призначення осіб (НОВИЙ)
│   ├── ReferenceService.cs    # CRUD довідників (НОВИЙ)
│   ├── TemplateRenderer.cs    # Рендеринг (рефакторинг → нестатичний)
│   └── SettingsService.cs     # Налаштування (НОВИЙ)
├── DTOs/                       # Data Transfer Objects (НОВИЙ)
│   ├── OrderListItem.cs
│   ├── PersonSearchResult.cs
│   ├── AssignmentResult.cs
│   └── TemplateTreeNode.cs
├── Migrations/          # Усі міграції
└── Base2.Core.csproj    # netstandard2.1 або net10.0
```

### 1.2 Створити проєкт `Base2.WinForms`

Залишити в WinForms проєкті тільки UI-код.

**Структура `Base2.WinForms`:**
```
Base2.WinForms/
├── Forms/               # Усі форми (рефакторинг — видалити логіку)
├── Program.cs           # Точка входу
├── AppServices.cs       # DI конфігурація
└── Base2.WinForms.csproj  # Посилання на Base2.Core
```

---

## 🔨 Етап 2: Витягнення бізнес-логіки з форм у сервіси

### 2.1 Новий `TemplateService` (витягнути з TemplateEditorForm + MainForm)

Вся логіка роботи з шаблонами зібрана в одному сервісі.

```csharp
public class TemplateService
{
    private readonly AppDbContext _context;
    
    public TemplateService(AppDbContext context) => _context = context;
    
    // З MainForm:
    public List<DutyTemplate> GetActiveTemplates();
    public DutyTemplate CreateTemplate(string name, string? description);
    public void RenameTemplate(int templateId, string newName);
    public void DeleteTemplate(int templateId); // з перевіркою наявності наказів
    
    // З TemplateEditorForm:
    public DutyTemplate? LoadTemplateWithSections(int templateId);
    public DutySectionNode AddNode(int templateId, int? parentId, NodeType nodeType);
    public DutySectionNode AddSiblingNode(int afterNodeId, NodeType nodeType);
    public void UpdateNode(DutySectionNode node); // зберегти зміни вузла
    public void DeleteNode(int nodeId); // каскадне видалення
    public void MoveNode(int nodeId, int? newParentId, int newIndex);
    public void ReorderNodes(int parentId, List<int> orderedNodeIds);
    public void GenerateTitles(int templateId); // автонумерація
}
```

### 2.2 Розширити `OrderService` (витягнути з MainForm + OrderCreatorForm)

```csharp
public class OrderService
{
    // Існуючі методи (без змін):
    public DutyOrder CreateOrderFromTemplate(...);
    public void IncrementTemplateVersion(...);
    public List<DutyOrder> GetOutdatedOrders(int templateId);
    public void GenerateTitles(int orderId);
    
    // НОВІ (з MainForm):
    public List<OrderListItem> GetOrdersForTemplate(int templateId);
    public void DeleteOrder(int orderId); // каскадне видалення (зараз у MainForm 40 рядків)
    
    // НОВІ (з OrderCreatorForm):
    public DutyOrder? LoadOrderWithDetails(int orderId);
    public void UpdateOrderMetadata(int orderId, string orderNumber, DateOnly date, DateTime start, DateTime end, string commander);
    public void UpdateTimeRanges(int orderId, List<DutyTimeRange> ranges);
    public string RenderOrderPreview(int orderId); // повний текст наказу
}
```

### 2.3 Новий `AssignmentService` (витягнути з AssignmentForm)

```csharp
public class AssignmentService
{
    // З AssignmentForm:
    public List<PersonSearchResult> SearchAvailablePersons(int orderId, string? lastNameFilter);
    public HashSet<int> GetAssignedPersonIds(int orderId);
    public HashSet<int> GetAssignedWeaponIds(int orderId);
    public HashSet<int> GetAssignedVehicleIds(int orderId);
    public List<Weapon> GetAvailableWeapons(int orderId, int? locationId);
    public List<Vehicle> GetAvailableVehicles(int orderId);
    public Weapon? GetPersonAssignedWeapon(int personId);
    public WeaponAmmoPreset? GetAmmoPreset(string weaponType);
    public int? ResolveLocationId(int nodeId); // рекурсивний пошук по батьках
    
    public DutyAssignment CreateAssignment(int nodeId, int personId, int? weaponId, int? vehicleId, string? ammoType, int? ammoCount);
    public void RemoveAssignment(int assignmentId);
}
```

### 2.4 Новий `ReferenceService` (витягнути з ReferenceForm)

```csharp
public class ReferenceService
{
    // Єдиний сервіс для всіх довідників
    // Замість 7 окремих блоків коду у ReferenceForm
    
    // Locations
    public List<Location> GetLocations(string? filter = null);
    public Location CreateLocation(string name, string? address);
    public void UpdateLocation(int id, string name, string? address);
    public void DeleteLocation(int id);
    
    // People
    public List<Person> GetPeople(string? lastNameFilter = null);
    public Person CreatePerson(string lastName, string? firstName, string? middleName, int rankId, int positionId);
    public void UpdatePerson(int id, ...);
    public void DeletePerson(int id);
    
    // Weapons, Vehicles, Ranks, Positions, WeaponAmmoPresets — аналогічно
    // ...
}
```

### 2.5 Рефакторинг `TemplateRenderer` (зробити нестатичним)

```csharp
public class TemplateRenderer
{
    private readonly AppDbContext _context;
    
    public TemplateRenderer(AppDbContext context) => _context = context;
    
    // Існуючі методи (змінити static → instance):
    public string Render(string template, DutyAssignment? assignment, ...);
    public string FormatOrderPeriod(DateTime start, DateTime end);
    public string RenderNode(DutySectionNode section, DutyOrder? order);
    public string FormatAssignmentInline(DutyAssignment assignment, DutySectionNode section);
}
```

---

## 🔄 Етап 3: Виправлення DI та DbContext

### 3.1 Замінити Singleton DbContext на Scoped/Transient

**Проблема:** Singleton `AppDbContext` спричиняє накопичення ChangeTracker та ручні виклики `ChangeTracker.Clear()` у 5+ місцях.

**Рішення:**
```csharp
// AppServices.cs — WinForms
services.AddTransient<AppDbContext>(); // нова інстанція для кожної операції

// Або для Blazor/WebAPI:
services.AddScoped<AppDbContext>(); // одна на HTTP-запит
```

### 3.2 Оновити реєстрацію сервісів

```csharp
public static class AppServices
{
    public static void Initialize()
    {
        var services = new ServiceCollection();
        
        // Data
        services.AddTransient<AppDbContext>();
        
        // Services
        services.AddTransient<OrderService>();
        services.AddTransient<TemplateService>();
        services.AddTransient<AssignmentService>();
        services.AddTransient<ReferenceService>();
        services.AddTransient<TemplateRenderer>();
        services.AddTransient<SettingsService>();
        
        _provider = services.BuildServiceProvider();
    }
}
```

### 3.3 Видалити прямі виклики `AppServices.DbContext` з форм

Форми повинні працювати тільки через сервіси:
```csharp
// ❌ Було (у формі):
var orders = AppServices.DbContext.DutyOrders.Where(...).ToList();

// ✅ Стало:
var orderService = AppServices.Get<OrderService>();
var orders = orderService.GetOrdersForTemplate(templateId);
```

---

## 📋 Етап 4: Рефакторинг форм (видалити бізнес-логіку)

### 4.1 MainForm — мінімальний рефакторинг

**Видалити з форми:**
- Каскадне видалення наказу (40 рядків → `OrderService.DeleteOrder()`)
- Завантаження шаблонів (→ `TemplateService.GetActiveTemplates()`)
- Перевірку застарілих наказів (→ `OrderService.GetOutdatedOrders()`)
- Перейменування шаблону (→ `TemplateService.RenameTemplate()`)
- Створення/видалення шаблону (→ `TemplateService`)

**Залишити у формі:**
- Обробку подій (click, selection changed)
- Прив'язку даних до контролів
- Відображення повідомлень (MessageBox)
- Навігацію (відкриття інших форм)

### 4.2 TemplateEditorForm — найбільший рефакторинг

**Видалити з форми:**
- CRUD вузлів (→ `TemplateService`)
- Drag-drop логіку переміщення (→ `TemplateService.MoveNode()`)
- Автонумерацію (→ `TemplateService.GenerateTitles()`)
- Інкремент версії (→ `OrderService.IncrementTemplateVersion()`)

**Залишити у формі:**
- TreeView візуалізацію та побудову
- Побудову динамічної панелі редагування (ShowEditorPanel)
- Обробку drag-drop UI-подій
- Контекстне меню

### 4.3 OrderCreatorForm — середній рефакторинг

**Видалити з форми:**
- Створення наказу з шаблону (→ `OrderService.CreateOrderFromTemplate()`)
- Рендеринг прев'ю (→ `OrderService.RenderOrderPreview()`)
- Збереження метаданих (→ `OrderService.UpdateOrderMetadata()`)

**Залишити у формі:**
- Побудову TreeView
- Панель часових діапазонів (DateTimePicker)
- RichTextBox прев'ю

### 4.4 AssignmentForm — середній рефакторинг

**Видалити з форми:**
- Пошук доступних осіб (→ `AssignmentService.SearchAvailablePersons()`)
- Пошук зброї за локацією (→ `AssignmentService.GetAvailableWeapons()`)
- Автопідбір боєкомплекту (→ `AssignmentService.GetAmmoPreset()`)
- Створення призначення (→ `AssignmentService.CreateAssignment()`)

**Залишити у формі:**
- Пошук з клавіатурною навігацією (UI-поведінка)
- Вибір радіокнопок зброї
- Заповнення комбобоксів

### 4.5 ReferenceForm — великий рефакторинг

**Видалити з форми:**
- Усі CRUD-операції для 7 сутностей (→ `ReferenceService`)
- Валідацію (→ Валідація в `ReferenceService`)
- Фільтрацію (→ `ReferenceService.GetXxx(filter)`)

**Залишити у формі:**
- TabControl та DataGridView прив'язку
- Обробку подій кнопок
- Відкриття діалогів редагування

---

## 🌐 Етап 5: Підготовка для Blazor

### 5.1 Blazor Server додаток

```
Base2.Blazor/
├── Pages/
│   ├── Index.razor          # Головна (шаблони + накази)
│   ├── TemplateEditor.razor # Редактор шаблонів (MudBlazor TreeView)
│   ├── OrderEditor.razor    # Створення/редагування наказу
│   ├── Assignment.razor     # Діалог призначення
│   ├── References.razor     # Довідники (вкладки)
│   └── Settings.razor       # Налаштування
├── Program.cs
└── Base2.Blazor.csproj      # Посилання на Base2.Core
```

**DI для Blazor:**
```csharp
// Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=DutyOrder.db"));
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<TemplateService>();
// ... інші сервіси
```

Усі сервіси з `Base2.Core` працюють без змін, змінюється тільки реєстрація (Scoped замість Transient).

### 5.2 Рекомендовані UI-компоненти для Blazor

- **MudBlazor** — Material Design компоненти (TreeView, DataGrid, Dialogs)
- Або **Radzen** — швидка альтернатива

---

## 🌐 Етап 6: Підготовка для WebAPI + Quasar PWA

### 6.1 WebAPI проєкт

```
Base2.Api/
├── Controllers/
│   ├── TemplatesController.cs    # GET/POST/PUT/DELETE шаблони
│   ├── OrdersController.cs       # GET/POST/PUT/DELETE накази
│   ├── AssignmentsController.cs  # POST/DELETE призначення
│   ├── ReferenceController.cs    # GET/POST/PUT/DELETE довідники
│   └── SettingsController.cs     # GET/PUT налаштування
├── Program.cs
└── Base2.Api.csproj              # Посилання на Base2.Core
```

**Контролери — тонка обгортка над сервісами:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    
    public OrdersController(OrderService orderService)
        => _orderService = orderService;
    
    [HttpGet("template/{templateId}")]
    public ActionResult<List<OrderListItem>> GetOrders(int templateId)
        => Ok(_orderService.GetOrdersForTemplate(templateId));
    
    [HttpPost]
    public ActionResult<DutyOrder> Create([FromBody] CreateOrderRequest request)
        => Ok(_orderService.CreateOrderFromTemplate(...));
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _orderService.DeleteOrder(id);
        return NoContent();
    }
}
```

### 6.2 Quasar PWA (Vue 3) з offline-підтримкою

```
base2-pwa/
├── src/
│   ├── pages/
│   │   ├── MainPage.vue           # Шаблони + накази
│   │   ├── TemplateEditor.vue     # Редактор (q-tree)
│   │   ├── OrderEditor.vue        # Наказ
│   │   ├── AssignmentDialog.vue   # Призначення
│   │   └── ReferencesPage.vue     # Довідники (q-tabs)
│   ├── stores/                    # Pinia stores
│   │   ├── referenceStore.js      # Кешування довідників
│   │   ├── templateStore.js
│   │   └── orderStore.js
│   ├── services/
│   │   └── api.js                 # Axios HTTP-клієнт
│   └── offline/
│       ├── syncQueue.js           # Черга змін для синхронізації
│       └── cacheManager.js        # IndexedDB кешування
├── quasar.config.js               # PWA конфігурація
└── package.json
```

### 6.3 Стратегія offline-кешування

```
┌─────────────────────────────────────────────┐
│                Quasar PWA                    │
│                                              │
│  ┌──────────────────────────────────────┐   │
│  │         Pinia Store (reactive)        │   │
│  │  ranks[], positions[], persons[],     │   │
│  │  weapons[], vehicles[], locations[]   │   │
│  └──────────┬──────────────┬────────────┘   │
│             │              │                 │
│     ┌───────▼───────┐ ┌───▼──────────┐      │
│     │  IndexedDB    │ │  API Client  │      │
│     │  (offline)    │ │  (online)    │      │
│     └───────────────┘ └──────────────┘      │
│                                              │
│  Стратегія:                                  │
│  1. При старті: завантажити довідники        │
│     з IndexedDB (кеш)                        │
│  2. Фонове оновлення: fetch з API            │
│  3. Offline: працювати з кешем               │
│  4. Накази: зберігати локально,              │
│     синхронізувати при онлайні               │
│  5. Конфлікти: last-write-wins або           │
│     manual merge                             │
└─────────────────────────────────────────────┘
```

**Що кешувати (завжди доступно offline):**
- ✅ Довідники (звання, посади, локації, пресети) — рідко змінюються
- ✅ Особовий склад — середня зміна
- ✅ Зброя, транспорт — середня зміна
- ✅ Активні шаблони — рідко змінюються

**Що синхронізувати (online-first):**
- 🔄 Накази — створюються часто, потребують актуальних даних
- 🔄 Призначення — залежать від доступності осіб/зброї

---

## 📋 Порядок виконання

### Фаза 1: Підготовка (1-2 тижні)
1. ☐ Створити solution з двох проєктів (Base2.Core + Base2.WinForms)
2. ☐ Перемістити Models/, Data/, Migrations/ у Base2.Core
3. ☐ Перемістити Services/ у Base2.Core
4. ☐ Перемістити Helpers/ у Base2.Core
5. ☐ Налаштувати посилання Base2.WinForms → Base2.Core
6. ☐ Переконатися що додаток працює без змін логіки

### Фаза 2: Сервісний шар (2-3 тижні)
7. ☐ Створити `TemplateService` — витягнути з TemplateEditorForm + MainForm
8. ☐ Розширити `OrderService` — витягнути каскадне видалення з MainForm
9. ☐ Створити `AssignmentService` — витягнути з AssignmentForm
10. ☐ Створити `ReferenceService` — витягнути з ReferenceForm
11. ☐ Рефакторинг `TemplateRenderer` — static → instance
12. ☐ Створити `SettingsService`
13. ☐ Створити DTOs для передачі даних між шарами

### Фаза 3: Рефакторинг форм (2-3 тижні)
14. ☐ Рефакторинг MainForm — використовувати сервіси
15. ☐ Рефакторинг TemplateEditorForm — використовувати сервіси
16. ☐ Рефакторинг OrderCreatorForm — використовувати сервіси
17. ☐ Рефакторинг AssignmentForm — використовувати сервіси
18. ☐ Рефакторинг ReferenceForm — використовувати сервіси
19. ☐ Виправити DI (Transient DbContext, видалити ChangeTracker.Clear())
20. ☐ Видалити Form1.cs, NewDutyOrderDialog.cs, AssignmentDialog.cs (мертвий код)

### Фаза 4: Blazor (3-4 тижні)
21. ☐ Створити проєкт Base2.Blazor
22. ☐ Реалізувати сторінки (Index, TemplateEditor, OrderEditor, References, Settings)
23. ☐ Налаштувати DI (Scoped)
24. ☐ Тестування

### Фаза 5: WebAPI + Quasar PWA (4-6 тижнів)
25. ☐ Створити проєкт Base2.Api — REST контролери
26. ☐ Створити Quasar PWA проєкт
27. ☐ Реалізувати кешування довідників (IndexedDB + Pinia)
28. ☐ Реалізувати offline-чергу змін (sync queue)
29. ☐ Тестування offline/online режимів
