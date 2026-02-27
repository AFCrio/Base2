using Base2.Data;
using Base2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Base2.Forms;

/// <summary>
/// Форма управління довідниками: Локації, Особовий склад, Зброя, Транспорт, Звання, Посади.
/// Кожна вкладка — таблиця з фільтром, додаванням, редагуванням, видаленням.
/// </summary>
public class ReferenceForm : Form
{
    private AppDbContext _context = null!;
    private TabControl tabControl = null!;

    // ── Локації ──
    private DataGridView dgvLocations = null!;
    private TextBox txtLocationFilter = null!;

    // ── Люди ──
    private DataGridView dgvPeople = null!;
    private TextBox txtPeopleFilter = null!;

    // ── Зброя ──
    private DataGridView dgvWeapons = null!;
    private TextBox txtWeaponFilter = null!;

    // ── Транспорт ──
    private DataGridView dgvVehicles = null!;
    private TextBox txtVehicleFilter = null!;

    // ── Звання ──
    private DataGridView dgvRanks = null!;
    private TextBox txtRankFilter = null!;

    // ── Посади ──
    private DataGridView dgvPositions = null!;
    private TextBox txtPositionFilter = null!;

    public ReferenceForm()
    {
        _context = new AppDbContext();
        BuildUI();
        LoadAll();
    }

    // ═══════════════════════════════════════════════════
    //  UI BUILD
    // ═══════════════════════════════════════════════════

    private void BuildUI()
    {
        Text = "Довідники";
        Size = new Size(950, 650);
        MinimumSize = new Size(800, 500);
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 10F);

        tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F)
        };

        tabControl.TabPages.Add(BuildLocationsTab());
        tabControl.TabPages.Add(BuildPeopleTab());
        tabControl.TabPages.Add(BuildWeaponsTab());
        tabControl.TabPages.Add(BuildVehiclesTab());
        tabControl.TabPages.Add(BuildRanksTab());
        tabControl.TabPages.Add(BuildPositionsTab());

        Controls.Add(tabControl);
    }

    /// <summary>
    /// Створює стандартну панель: фільтр + кнопки + DataGridView
    /// </summary>
    private static (Panel toolbar, TextBox filter, DataGridView grid) BuildTabContent(
        string filterPlaceholder,
        EventHandler addClick,
        EventHandler editClick,
        EventHandler deleteClick,
        EventHandler? filterChanged)
    {
        var toolbar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 44,
            Padding = new Padding(6)
        };

        var filter = new TextBox
        {
            PlaceholderText = filterPlaceholder,
            Location = new Point(8, 8),
            Width = 250,
            Font = new Font("Consolas", 10F)
        };
        if (filterChanged != null)
            filter.TextChanged += filterChanged;

        var btnAdd = new Button
        {
            Text = "➕ Додати",
            Location = new Point(280, 6),
            Size = new Size(110, 30),
            BackColor = Color.LightGreen,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };
        btnAdd.Click += addClick;

        var btnEdit = new Button
        {
            Text = "✏️ Змінити",
            Location = new Point(400, 6),
            Size = new Size(110, 30),
            BackColor = Color.LightBlue,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };
        btnEdit.Click += editClick;

        var btnDel = new Button
        {
            Text = "🗑️ Видалити",
            Location = new Point(520, 6),
            Size = new Size(110, 30),
            BackColor = Color.MistyRose,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };
        btnDel.Click += deleteClick;

        toolbar.Controls.AddRange([filter, btnAdd, btnEdit, btnDel]);

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = SystemColors.Window,
            Font = new Font("Segoe UI", 9.5F)
        };

        return (toolbar, filter, grid);
    }

    // ─────────── Локації ───────────

    private TabPage BuildLocationsTab()
    {
        var tab = new TabPage("📍 Локації");

        var (toolbar, filter, grid) = BuildTabContent(
            "Фільтр за назвою…",
            (_, _) => AddLocation(),
            (_, _) => EditLocation(),
            (_, _) => DeleteLocation(),
            (_, _) => LoadLocations());

        txtLocationFilter = filter;
        dgvLocations = grid;
        dgvLocations.CellDoubleClick += (_, _) => EditLocation();

        tab.Controls.Add(grid);
        tab.Controls.Add(toolbar);
        return tab;
    }

    // ─────────── Люди ───────────

    private TabPage BuildPeopleTab()
    {
        var tab = new TabPage("👤 Особовий склад");

        var (toolbar, filter, grid) = BuildTabContent(
            "Фільтр за прізвищем…",
            (_, _) => AddPerson(),
            (_, _) => EditPerson(),
            (_, _) => DeletePerson(),
            (_, _) => LoadPeople());

        txtPeopleFilter = filter;
        dgvPeople = grid;
        dgvPeople.CellDoubleClick += (_, _) => EditPerson();

        tab.Controls.Add(grid);
        tab.Controls.Add(toolbar);
        return tab;
    }

    // ─────────── Зброя ───────────

    private TabPage BuildWeaponsTab()
    {
        var tab = new TabPage("🔫 Зброя");

        var (toolbar, filter, grid) = BuildTabContent(
            "Фільтр за номером…",
            (_, _) => AddWeapon(),
            (_, _) => EditWeapon(),
            (_, _) => DeleteWeapon(),
            (_, _) => LoadWeapons());

        txtWeaponFilter = filter;
        dgvWeapons = grid;
        dgvWeapons.CellDoubleClick += (_, _) => EditWeapon();

        tab.Controls.Add(grid);
        tab.Controls.Add(toolbar);
        return tab;
    }

    // ─────────── Транспорт ───────────

    private TabPage BuildVehiclesTab()
    {
        var tab = new TabPage("🚗 Транспорт");

        var (toolbar, filter, grid) = BuildTabContent(
            "Фільтр за номером…",
            (_, _) => AddVehicle(),
            (_, _) => EditVehicle(),
            (_, _) => DeleteVehicle(),
            (_, _) => LoadVehicles());

        txtVehicleFilter = filter;
        dgvVehicles = grid;
        dgvVehicles.CellDoubleClick += (_, _) => EditVehicle();

        tab.Controls.Add(grid);
        tab.Controls.Add(toolbar);
        return tab;
    }

    // ═══════════════════════════════════════════════════
    //  DATA LOADING
    // ═══════════════════════════════════════════════════

    private void LoadAll()
    {
        LoadLocations();
        LoadPeople();
        LoadWeapons();
        LoadVehicles();
        LoadRanks();
        LoadPositions();
    }

    private void LoadLocations()
    {
        var filter = txtLocationFilter?.Text?.Trim() ?? "";
        var query = _context.Locations.AsQueryable();

        if (!string.IsNullOrEmpty(filter))
            query = query.Where(l => l.LocationName.Contains(filter));

        dgvLocations.DataSource = query
            .OrderBy(l => l.LocationName)
            .Select(l => new
            {
                l.LocationId,
                Назва = l.LocationName,
                Адреса = l.Address ?? "",
                Зброї = l.StoredWeapons.Count
            })
            .ToList();

        if (dgvLocations.Columns.Contains("LocationId"))
            dgvLocations.Columns["LocationId"].Visible = false;
    }

    private void LoadPeople()
    {
        var filter = txtPeopleFilter?.Text?.Trim() ?? "";
        var query = _context.People
            .Include(p => p.Rank)
            .Include(p => p.Position)
            .Include(p => p.AssignedWeapons)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter))
            query = query.Where(p => p.LastName.Contains(filter));

        dgvPeople.DataSource = query
            .OrderBy(p => p.LastName)
            .Select(p => new
            {
                p.PersonId,
                Прізвище = p.LastName,
                Ініціали = p.Initials ?? "",
                Звання = p.Rank.RankName,
                Посада = p.Position.PositionName,
                Зброя = p.AssignedWeapons.Any()
                    ? string.Join(", ", p.AssignedWeapons.Select(w => w.WeaponType + " №" + w.WeaponNumber))
                    : ""
            })
            .ToList();

        if (dgvPeople.Columns.Contains("PersonId"))
            dgvPeople.Columns["PersonId"].Visible = false;
    }

    private void LoadWeapons()
    {
        var filter = txtWeaponFilter?.Text?.Trim() ?? "";
        var query = _context.Weapons
            .Include(w => w.StoredInLocation)
            .Include(w => w.AssignedToPerson)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter))
            query = query.Where(w => w.WeaponNumber.Contains(filter) || w.WeaponType.Contains(filter));

        dgvWeapons.DataSource = query
            .OrderBy(w => w.WeaponType)
            .ThenBy(w => w.WeaponNumber)
            .Select(w => new
            {
                w.WeaponId,
                Тип = w.WeaponType,
                Номер = w.WeaponNumber,
                Локація = w.StoredInLocation != null ? w.StoredInLocation.LocationName : "",
                Закріплена = w.AssignedToPerson != null
                    ? w.AssignedToPerson.LastName + " " + (w.AssignedToPerson.Initials ?? "")
                    : ""
            })
            .ToList();

        if (dgvWeapons.Columns.Contains("WeaponId"))
            dgvWeapons.Columns["WeaponId"].Visible = false;
    }

    private void LoadVehicles()
    {
        var filter = txtVehicleFilter?.Text?.Trim() ?? "";
        var query = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrEmpty(filter))
            query = query.Where(v => v.VehicleNumber.Contains(filter) || v.VehicleName.Contains(filter));

        dgvVehicles.DataSource = query
            .OrderBy(v => v.VehicleName)
            .ThenBy(v => v.VehicleNumber)
            .Select(v => new
            {
                v.VehicleId,
                Назва = v.VehicleName,
                Номер = v.VehicleNumber,
                Тип = v.VehicleType ?? ""
            })
            .ToList();

        if (dgvVehicles.Columns.Contains("VehicleId"))
            dgvVehicles.Columns["VehicleId"].Visible = false;
    }

    // ═══════════════════════════════════════════════════
    //  LOCATIONS CRUD
    // ═══════════════════════════════════════════════════

    private void AddLocation()
    {
        using var dlg = new EditDialog("Нова локація",
            [("Назва", ""), ("Адреса", "")]);

        if (dlg.ShowDialog() != DialogResult.OK) return;

        var name = dlg.Values[0].Trim();
        if (string.IsNullOrWhiteSpace(name)) return;

        _context.Locations.Add(new Location { LocationName = name, Address = dlg.Values[1].Trim() });
        _context.SaveChanges();
        LoadLocations();
    }

    private void EditLocation()
    {
        if (GetSelectedId(dgvLocations, "LocationId") is not int id) return;
        var loc = _context.Locations.Find(id);
        if (loc == null) return;

        using var dlg = new EditDialog("Редагування локації",
            [("Назва", loc.LocationName), ("Адреса", loc.Address ?? "")]);

        if (dlg.ShowDialog() != DialogResult.OK) return;

        loc.LocationName = dlg.Values[0].Trim();
        loc.Address = dlg.Values[1].Trim();
        _context.SaveChanges();
        LoadLocations();
    }

    private void DeleteLocation()
    {
        if (GetSelectedId(dgvLocations, "LocationId") is not int id) return;
        var loc = _context.Locations.Find(id);
        if (loc == null) return;

        if (Confirm($"Видалити локацію «{loc.LocationName}»?"))
        {
            _context.Locations.Remove(loc);
            SaveWithCatch();
            LoadLocations();
        }
    }

    // ═══════════════════════════════════════════════════
    //  PEOPLE CRUD
    // ═══════════════════════════════════════════════════

    private void AddPerson()
    {
        using var dlg = new PersonEditDialog(_context);
        if (dlg.ShowDialog() != DialogResult.OK || dlg.ResultPerson == null) return;

        _context.People.Add(dlg.ResultPerson);
        _context.SaveChanges();

        // Прив'язати зброю (тепер PersonId відомий)
        dlg.LinkWeaponToNewPerson(dlg.ResultPerson.PersonId);
        _context.SaveChanges();

        LoadPeople();
        LoadWeapons();
    }

    private void EditPerson()
    {
        if (GetSelectedId(dgvPeople, "PersonId") is not int id) return;
        var person = _context.People
            .Include(p => p.Rank)
            .Include(p => p.Position)
            .Include(p => p.AssignedWeapons)
            .FirstOrDefault(p => p.PersonId == id);
        if (person == null) return;

        using var dlg = new PersonEditDialog(_context, person);
        if (dlg.ShowDialog() != DialogResult.OK) return;

        _context.SaveChanges();
        LoadPeople();
        LoadWeapons();
    }

    private void DeletePerson()
    {
        if (GetSelectedId(dgvPeople, "PersonId") is not int id) return;
        var person = _context.People.Find(id);
        if (person == null) return;

        if (Confirm($"Видалити «{person.LastName} {person.Initials}»?"))
        {
            _context.People.Remove(person);
            SaveWithCatch();
            LoadPeople();
        }
    }

    // ═══════════════════════════════════════════════════
    //  WEAPONS CRUD
    // ═══════════════════════════════════════════════════

    private void AddWeapon()
    {
        using var dlg = new WeaponEditDialog(_context);
        if (dlg.ShowDialog() != DialogResult.OK || dlg.ResultWeapon == null) return;

        _context.Weapons.Add(dlg.ResultWeapon);
        _context.SaveChanges();
        LoadWeapons();
        LoadPeople();
    }

    private void EditWeapon()
    {
        if (GetSelectedId(dgvWeapons, "WeaponId") is not int id) return;
        var weapon = _context.Weapons
            .Include(w => w.StoredInLocation)
            .Include(w => w.AssignedToPerson)
            .FirstOrDefault(w => w.WeaponId == id);
        if (weapon == null) return;

        using var dlg = new WeaponEditDialog(_context, weapon);
        if (dlg.ShowDialog() != DialogResult.OK) return;

        _context.SaveChanges();
        LoadWeapons();
        LoadPeople();
    }

    private void DeleteWeapon()
    {
        if (GetSelectedId(dgvWeapons, "WeaponId") is not int id) return;
        var weapon = _context.Weapons.Find(id);
        if (weapon == null) return;

        if (Confirm($"Видалити зброю «{weapon.WeaponType} №{weapon.WeaponNumber}»?"))
        {
            _context.Weapons.Remove(weapon);
            SaveWithCatch();
            LoadWeapons();
        }
    }

    // ═══════════════════════════════════════════════════
    //  VEHICLES CRUD
    // ═══════════════════════════════════════════════════

    private void AddVehicle()
    {
        using var dlg = new EditDialog("Новий транспорт",
            [("Назва (марка)", ""), ("Номерний знак", ""), ("Тип (легковий, вантажний…)", "")]);

        if (dlg.ShowDialog() != DialogResult.OK) return;

        var name = dlg.Values[0].Trim();
        var number = dlg.Values[1].Trim();
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(number)) return;

        _context.Vehicles.Add(new Vehicle
        {
            VehicleName = name,
            VehicleNumber = number,
            VehicleType = string.IsNullOrWhiteSpace(dlg.Values[2]) ? null : dlg.Values[2].Trim()
        });
        _context.SaveChanges();
        LoadVehicles();
    }

    private void EditVehicle()
    {
        if (GetSelectedId(dgvVehicles, "VehicleId") is not int id) return;
        var vehicle = _context.Vehicles.Find(id);
        if (vehicle == null) return;

        using var dlg = new EditDialog("Редагування транспорту",
            [("Назва (марка)", vehicle.VehicleName),
             ("Номерний знак", vehicle.VehicleNumber),
             ("Тип (легковий, вантажний…)", vehicle.VehicleType ?? "")]);

        if (dlg.ShowDialog() != DialogResult.OK) return;

        vehicle.VehicleName = dlg.Values[0].Trim();
        vehicle.VehicleNumber = dlg.Values[1].Trim();
        vehicle.VehicleType = string.IsNullOrWhiteSpace(dlg.Values[2]) ? null : dlg.Values[2].Trim();
        _context.SaveChanges();
        LoadVehicles();
    }

    private void DeleteVehicle()
    {
        if (GetSelectedId(dgvVehicles, "VehicleId") is not int id) return;
        var vehicle = _context.Vehicles.Find(id);
        if (vehicle == null) return;

        if (Confirm($"Видалити «{vehicle.VehicleName} {vehicle.VehicleNumber}»?"))
        {
            _context.Vehicles.Remove(vehicle);
            SaveWithCatch();
            LoadVehicles();
        }
    }

    // ═══════════════════════════════════════════════════
    //  RANKS CRUD
    // ═══════════════════════════════════════════════════

    private TabPage BuildRanksTab()
    {
        var tab = new TabPage("⭐ Звання");

        var (toolbar, filter, grid) = BuildTabContent(
            "Фільтр за назвою…",
            (_, _) => AddRank(),
            (_, _) => EditRank(),
            (_, _) => DeleteRank(),
            (_, _) => LoadRanks());

        txtRankFilter = filter;
        dgvRanks = grid;
        dgvRanks.CellDoubleClick += (_, _) => EditRank();

        tab.Controls.Add(grid);
        tab.Controls.Add(toolbar);
        return tab;
    }

    private void LoadRanks()
    {
        var filter = txtRankFilter?.Text?.Trim() ?? "";
        var query = _context.Ranks.AsQueryable();

        if (!string.IsNullOrEmpty(filter))
            query = query.Where(r => r.RankName.Contains(filter));

        dgvRanks.DataSource = query
            .OrderBy(r => r.RankLevel)
            .Select(r => new
            {
                r.RankId,
                Звання = r.RankName,
                Рівень = r.RankLevel,
                Осіб = r.People.Count
            })
            .ToList();

        if (dgvRanks.Columns.Contains("RankId"))
            dgvRanks.Columns["RankId"].Visible = false;
    }

    private void AddRank()
    {
        using var dlg = new EditDialog("Нове звання",
            [("Назва звання", ""), ("Рівень (число для сортування)", "1")]);

        if (dlg.ShowDialog() != DialogResult.OK) return;
        if (string.IsNullOrWhiteSpace(dlg.Values[0])) return;
        if (!int.TryParse(dlg.Values[1], out var level)) level = 1;

        _context.Ranks.Add(new Rank { RankName = dlg.Values[0].Trim(), RankLevel = level });
        _context.SaveChanges();
        LoadRanks();
    }

    private void EditRank()
    {
        if (GetSelectedId(dgvRanks, "RankId") is not int id) return;
        var rank = _context.Ranks.Find(id);
        if (rank == null) return;

        using var dlg = new EditDialog("Редагування звання",
            [("Назва звання", rank.RankName), ("Рівень", rank.RankLevel.ToString())]);

        if (dlg.ShowDialog() != DialogResult.OK) return;

        rank.RankName = dlg.Values[0].Trim();
        if (int.TryParse(dlg.Values[1], out var level)) rank.RankLevel = level;
        _context.SaveChanges();
        LoadRanks();
    }

    private void DeleteRank()
    {
        if (GetSelectedId(dgvRanks, "RankId") is not int id) return;
        var rank = _context.Ranks.Find(id);
        if (rank == null) return;

        if (Confirm($"Видалити звання «{rank.RankName}»?"))
        {
            _context.Ranks.Remove(rank);
            SaveWithCatch();
            LoadRanks();
        }
    }

    // ═══════════════════════════════════════════════════
    //  POSITIONS CRUD
    // ═══════════════════════════════════════════════════

    private TabPage BuildPositionsTab()
    {
        var tab = new TabPage("📋 Посади");

        var (toolbar, filter, grid) = BuildTabContent(
            "Фільтр за назвою…",
            (_, _) => AddPosition(),
            (_, _) => EditPosition(),
            (_, _) => DeletePosition(),
            (_, _) => LoadPositions());

        txtPositionFilter = filter;
        dgvPositions = grid;
        dgvPositions.CellDoubleClick += (_, _) => EditPosition();

        tab.Controls.Add(grid);
        tab.Controls.Add(toolbar);
        return tab;
    }

    private void LoadPositions()
    {
        var filter = txtPositionFilter?.Text?.Trim() ?? "";
        var query = _context.Positions.AsQueryable();

        if (!string.IsNullOrEmpty(filter))
            query = query.Where(p => p.PositionName.Contains(filter));

        dgvPositions.DataSource = query
            .OrderBy(p => p.PositionName)
            .Select(p => new
            {
                p.PositionId,
                Посада = p.PositionName,
                Осіб = p.People.Count
            })
            .ToList();

        if (dgvPositions.Columns.Contains("PositionId"))
            dgvPositions.Columns["PositionId"].Visible = false;
    }

    private void AddPosition()
    {
        using var dlg = new EditDialog("Нова посада", [("Назва посади", "")]);
        if (dlg.ShowDialog() != DialogResult.OK) return;
        if (string.IsNullOrWhiteSpace(dlg.Values[0])) return;

        _context.Positions.Add(new Position { PositionName = dlg.Values[0].Trim() });
        _context.SaveChanges();
        LoadPositions();
    }

    private void EditPosition()
    {
        if (GetSelectedId(dgvPositions, "PositionId") is not int id) return;
        var pos = _context.Positions.Find(id);
        if (pos == null) return;

        using var dlg = new EditDialog("Редагування посади", [("Назва посади", pos.PositionName)]);
        if (dlg.ShowDialog() != DialogResult.OK) return;

        pos.PositionName = dlg.Values[0].Trim();
        _context.SaveChanges();
        LoadPositions();
    }

    private void DeletePosition()
    {
        if (GetSelectedId(dgvPositions, "PositionId") is not int id) return;
        var pos = _context.Positions.Find(id);
        if (pos == null) return;

        if (Confirm($"Видалити посаду «{pos.PositionName}»?"))
        {
            _context.Positions.Remove(pos);
            SaveWithCatch();
            LoadPositions();
        }
    }

    // ═══════════════════════════════════════════════════
    //  HELPERS
    // ═══════════════════════════════════════════════════

    private static int? GetSelectedId(DataGridView dgv, string columnName)
    {
        if (dgv.CurrentRow == null || !dgv.Columns.Contains(columnName)) return null;
        return dgv.CurrentRow.Cells[columnName].Value as int?;
    }

    private static bool Confirm(string message) =>
        MessageBox.Show(message, "Підтвердження",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    private void SaveWithCatch()
    {
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show(
                $"Неможливо видалити: запис використовується в інших даних.\n\n{ex.InnerException?.Message ?? ex.Message}",
                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _context?.Dispose();
        base.OnFormClosed(e);
    }
}

// ═══════════════════════════════════════════════════════════
//  ДОПОМІЖНІ ДІАЛОГИ
// ═══════════════════════════════════════════════════════════

/// <summary>
/// Універсальний діалог редагування простих текстових полів.
/// </summary>
public class EditDialog : Form
{
    private readonly List<TextBox> _textBoxes = [];
    public List<string> Values => _textBoxes.Select(t => t.Text).ToList();

    public EditDialog(string title, List<(string label, string value)> fields)
    {
        Text = title;
        Size = new Size(450, 80 + fields.Count * 60);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Font = new Font("Segoe UI", 10F);

        int y = 12;
        foreach (var (label, value) in fields)
        {
            var lbl = new Label { Text = label + ":", Location = new Point(14, y), AutoSize = true };
            Controls.Add(lbl);
            y += lbl.PreferredHeight + 2;

            var txt = new TextBox
            {
                Text = value,
                Location = new Point(14, y),
                Width = 400,
                Font = new Font("Consolas", 10F)
            };
            Controls.Add(txt);
            _textBoxes.Add(txt);
            y += 32;
        }

        y += 8;
        var btnOk = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(240, y),
            Size = new Size(80, 32)
        };
        var btnCancel = new Button
        {
            Text = "Скасувати",
            DialogResult = DialogResult.Cancel,
            Location = new Point(330, y),
            Size = new Size(90, 32)
        };
        Controls.AddRange([btnOk, btnCancel]);
        AcceptButton = btnOk;
        CancelButton = btnCancel;

        ClientSize = new Size(ClientSize.Width, y + 45);
    }
}

/// <summary>
/// Діалог редагування особи: ПІБ, ініціали, звання, посада, закріплена зброя.
/// </summary>
public class PersonEditDialog : Form
{
    private readonly AppDbContext _context;
    private readonly Person? _existing;

    private TextBox txtLastName = null!;
    private TextBox txtFirstName = null!;
    private TextBox txtMiddleName = null!;
    private TextBox txtInitials = null!;
    private ComboBox cmbRank = null!;
    private ComboBox cmbPosition = null!;

    // Зброя
    private GroupBox grpWeapon = null!;
    private RadioButton rbNoWeapon = null!;
    private RadioButton rbExistingWeapon = null!;
    private ComboBox cmbWeapon = null!;
    private RadioButton rbNewWeapon = null!;
    private TextBox txtNewWeaponType = null!;
    private TextBox txtNewWeaponNumber = null!;

    public Person? ResultPerson { get; private set; }

    public PersonEditDialog(AppDbContext context, Person? existing = null)
    {
        _context = context;
        _existing = existing;
        BuildUI();
    }

    private void BuildUI()
    {
        Text = _existing != null ? "Редагування особи" : "Нова особа";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Font = new Font("Segoe UI", 10F);

        int y = 12;
        int w = 440;

        // ── ПІБ ──
        AddLabel("Прізвище (ВЕЛИКИМИ):", ref y);
        txtLastName = AddTextBox(_existing?.LastName ?? "", ref y, w);

        AddLabel("Ім'я (необов'язково):", ref y);
        txtFirstName = AddTextBox(_existing?.FirstName ?? "", ref y, w);

        AddLabel("По батькові (необов'язково):", ref y);
        txtMiddleName = AddTextBox(_existing?.MiddleName ?? "", ref y, w);

        // ── Ініціали ──
        AddLabel("Ініціали (В.М. — заповнюються автоматично або вручну):", ref y);
        txtInitials = AddTextBox(_existing?.Initials ?? "", ref y, 120);

        // Автозаповнення ініціалів
        txtFirstName.TextChanged += (_, _) => AutoFillInitials();
        txtMiddleName.TextChanged += (_, _) => AutoFillInitials();

        y += 4;

        // ── Звання, Посада ──
        AddLabel("Звання:", ref y);
        cmbRank = AddComboBox(ref y, w);
        var ranks = _context.Ranks.OrderBy(r => r.RankLevel).ToList();
        cmbRank.DataSource = ranks;
        cmbRank.DisplayMember = "RankName";
        cmbRank.ValueMember = "RankId";
        if (_existing != null) cmbRank.SelectedValue = _existing.RankId;

        AddLabel("Посада:", ref y);
        cmbPosition = AddComboBox(ref y, w);
        var positions = _context.Positions.OrderBy(p => p.PositionName).ToList();
        cmbPosition.DataSource = positions;
        cmbPosition.DisplayMember = "PositionName";
        cmbPosition.ValueMember = "PositionId";
        if (_existing != null) cmbPosition.SelectedValue = _existing.PositionId;

        y += 6;

        // ── Зброя ──
        grpWeapon = new GroupBox
        {
            Text = "Закріплена зброя",
            Location = new Point(14, y),
            Size = new Size(w, 132)
        };

        rbNoWeapon = new RadioButton
        {
            Text = "Без зброї",
            Location = new Point(10, 22),
            AutoSize = true,
            Checked = true
        };

        rbExistingWeapon = new RadioButton
        {
            Text = "Обрати існуючу:",
            Location = new Point(10, 48),
            AutoSize = true
        };

        cmbWeapon = new ComboBox
        {
            Location = new Point(170, 46),
            Width = w - 190,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Enabled = false
        };

        rbNewWeapon = new RadioButton
        {
            Text = "Створити нову:",
            Location = new Point(10, 78),
            AutoSize = true
        };

        var lblNewType = new Label { Text = "Тип:", Location = new Point(30, 104), AutoSize = true };
        txtNewWeaponType = new TextBox
        {
            Location = new Point(70, 101),
            Width = 120,
            Enabled = false,
            Font = new Font("Consolas", 9.5F)
        };

        var lblNewNum = new Label { Text = "№:", Location = new Point(200, 104), AutoSize = true };
        txtNewWeaponNumber = new TextBox
        {
            Location = new Point(225, 101),
            Width = 100,
            Enabled = false,
            Font = new Font("Consolas", 9.5F)
        };

        rbExistingWeapon.CheckedChanged += (_, _) => cmbWeapon.Enabled = rbExistingWeapon.Checked;
        rbNewWeapon.CheckedChanged += (_, _) =>
        {
            var en = rbNewWeapon.Checked;
            txtNewWeaponType.Enabled = en;
            txtNewWeaponNumber.Enabled = en;
        };

        grpWeapon.Controls.AddRange([
            rbNoWeapon, rbExistingWeapon, cmbWeapon,
            rbNewWeapon, lblNewType, txtNewWeaponType, lblNewNum, txtNewWeaponNumber
        ]);
        Controls.Add(grpWeapon);
        y += grpWeapon.Height + 8;

        // Заповнити списки зброї
        LoadWeaponLists();

        // ── Кнопки ──
        var btnOk = new Button
        {
            Text = "OK",
            Location = new Point(270, y),
            Size = new Size(80, 32)
        };
        btnOk.Click += BtnOk_Click;

        var btnCancel = new Button
        {
            Text = "Скасувати",
            DialogResult = DialogResult.Cancel,
            Location = new Point(360, y),
            Size = new Size(100, 32)
        };

        Controls.AddRange([btnOk, btnCancel]);
        AcceptButton = btnOk;
        CancelButton = btnCancel;
        ClientSize = new Size(w + 30, y + 45);
    }

    private void LoadWeaponLists()
    {
        // Вільна зброя (не закріплена, або закріплена за цією особою)
        var freeWeapons = _context.Weapons
            .Where(w => w.AssignedToPersonId == null
                     || (_existing != null && w.AssignedToPersonId == _existing.PersonId))
            .OrderBy(w => w.WeaponType)
            .ThenBy(w => w.WeaponNumber)
            .ToList();

        cmbWeapon.DisplayMember = "Display";
        cmbWeapon.ValueMember = "WeaponId";
        cmbWeapon.DataSource = freeWeapons.Select(w => new
        {
            w.WeaponId,
            Display = $"{w.WeaponType} №{w.WeaponNumber}"
        }).ToList();

        // Перевірити чи є закріплена зброя
        if (_existing != null)
        {
            var assignedList = _context.Weapons
                .Where(w => w.AssignedToPersonId == _existing.PersonId)
                .ToList();
            if (assignedList.Count > 0)
            {
                rbExistingWeapon.Checked = true;
                cmbWeapon.Enabled = true;
                // Вибрати першу
                for (int i = 0; i < cmbWeapon.Items.Count; i++)
                {
                    var item = cmbWeapon.Items[i];
                    if (item is not null)
                    {
                        var prop = item.GetType().GetProperty("WeaponId");
                        if (prop != null && (int)prop.GetValue(item)! == assignedList[0].WeaponId)
                        {
                            cmbWeapon.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void AutoFillInitials()
    {
        var first = txtFirstName.Text.Trim();
        var middle = txtMiddleName.Text.Trim();

        if (string.IsNullOrEmpty(first)) return;

        var init = $"{first[0]}.";
        if (!string.IsNullOrEmpty(middle))
            init += $"{middle[0]}.";

        txtInitials.Text = init;
    }

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtLastName.Text))
        {
            MessageBox.Show("Введіть прізвище!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbRank.SelectedValue == null)
        {
            MessageBox.Show("Оберіть звання!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbPosition.SelectedValue == null)
        {
            MessageBox.Show("Оберіть посаду!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var lastName = txtLastName.Text.Trim().ToUpper();
        var firstName = txtFirstName.Text.Trim();
        var middleName = txtMiddleName.Text.Trim();
        var initials = txtInitials.Text.Trim();

        if (_existing != null)
        {
            _existing.LastName = lastName;
            _existing.FirstName = string.IsNullOrWhiteSpace(firstName) ? null : firstName;
            _existing.MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName;
            _existing.Initials = initials;
            _existing.RankId = (int)cmbRank.SelectedValue!;
            _existing.PositionId = (int)cmbPosition.SelectedValue!;
            ResultPerson = _existing;
        }
        else
        {
            ResultPerson = new Person
            {
                LastName = lastName,
                FirstName = string.IsNullOrWhiteSpace(firstName) ? null : firstName,
                MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName,
                Initials = initials,
                RankId = (int)cmbRank.SelectedValue!,
                PositionId = (int)cmbPosition.SelectedValue!
            };
        }

        // Обробка зброї
        HandleWeaponAssignment();

        DialogResult = DialogResult.OK;
        Close();
    }

    private void HandleWeaponAssignment()
    {
        // Зняти стару прив'язку якщо є
        if (_existing != null)
        {
            var oldWeapons = _context.Weapons
                .Where(w => w.AssignedToPersonId == _existing.PersonId)
                .ToList();
            foreach (var w in oldWeapons)
                w.AssignedToPersonId = null;
        }

        if (rbExistingWeapon.Checked && cmbWeapon.SelectedValue is int weaponId)
        {
            var weapon = _context.Weapons.Find(weaponId);
            if (weapon != null && _existing != null)
            {
                weapon.AssignedToPersonId = _existing.PersonId;
                weapon.StoredInLocationId = null; // зброя або на людині, або на локації
            }
        }
        else if (rbNewWeapon.Checked)
        {
            var type = txtNewWeaponType.Text.Trim();
            var number = txtNewWeaponNumber.Text.Trim();

            if (!string.IsNullOrWhiteSpace(type) && !string.IsNullOrWhiteSpace(number))
            {
                var newWeapon = new Weapon
                {
                    WeaponType = type,
                    WeaponNumber = number,
                    StoredInLocationId = null, // зброя закріплена за особою, не на локації
                    AssignedToPersonId = _existing?.PersonId
                };

                _context.Weapons.Add(newWeapon);
            }
        }
    }

    /// <summary>
    /// Після збереження нової особи — прив'язати зброю
    /// </summary>
    public void LinkWeaponToNewPerson(int personId)
    {
        if (rbExistingWeapon.Checked && cmbWeapon.SelectedValue is int weaponId)
        {
            var weapon = _context.Weapons.Find(weaponId);
            if (weapon != null)
            {
                weapon.AssignedToPersonId = personId;
                weapon.StoredInLocationId = null;
            }
        }
        else if (rbNewWeapon.Checked)
        {
            // Нова зброя вже додана, знайти і прив'язати
            var pending = _context.Weapons.Local
                .FirstOrDefault(w => w.AssignedToPersonId == null
                    && w.WeaponType == txtNewWeaponType.Text.Trim()
                    && w.WeaponNumber == txtNewWeaponNumber.Text.Trim());
            if (pending != null)
            {
                pending.AssignedToPersonId = personId;
                pending.StoredInLocationId = null;
            }
        }
    }

    private void AddLabel(string text, ref int y)
    {
        var lbl = new Label { Text = text, Location = new Point(14, y), AutoSize = true };
        Controls.Add(lbl);
        y += lbl.PreferredHeight + 2;
    }

    private TextBox AddTextBox(string value, ref int y, int width)
    {
        var txt = new TextBox
        {
            Text = value,
            Location = new Point(14, y),
            Width = width,
            Font = new Font("Consolas", 10F)
        };
        Controls.Add(txt);
        y += 30;
        return txt;
    }

    private ComboBox AddComboBox(ref int y, int width)
    {
        var cmb = new ComboBox
        {
            Location = new Point(14, y),
            Width = width,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        Controls.Add(cmb);
        y += 32;
        return cmb;
    }
}

/// <summary>
/// Діалог редагування зброї: тип, номер + RadioButton (локація / особа).
/// Зброя може бути або на локації, або закріплена за особою, але не одночасно.
/// </summary>
public class WeaponEditDialog : Form
{
    private readonly AppDbContext _context;
    private readonly Weapon? _existing;

    private TextBox txtType = null!;
    private TextBox txtNumber = null!;

    private RadioButton rbNone = null!;
    private RadioButton rbLocation = null!;
    private ComboBox cmbLocation = null!;
    private RadioButton rbPerson = null!;
    private ComboBox cmbPerson = null!;

    public Weapon? ResultWeapon { get; private set; }

    public WeaponEditDialog(AppDbContext context, Weapon? existing = null)
    {
        _context = context;
        _existing = existing;
        BuildUI();
    }

    private void BuildUI()
    {
        Text = _existing != null ? "Редагування зброї" : "Нова зброя";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Font = new Font("Segoe UI", 10F);

        int y = 12;
        int w = 440;

        var lblType = new Label { Text = "Тип зброї (АК-74, ПМ…):", Location = new Point(14, y), AutoSize = true };
        Controls.Add(lblType);
        y += lblType.PreferredHeight + 2;
        txtType = new TextBox { Text = _existing?.WeaponType ?? "", Location = new Point(14, y), Width = w, Font = new Font("Consolas", 10F) };
        Controls.Add(txtType);
        y += 30;

        var lblNum = new Label { Text = "Номер зброї:", Location = new Point(14, y), AutoSize = true };
        Controls.Add(lblNum);
        y += lblNum.PreferredHeight + 2;
        txtNumber = new TextBox { Text = _existing?.WeaponNumber ?? "", Location = new Point(14, y), Width = w, Font = new Font("Consolas", 10F) };
        Controls.Add(txtNumber);
        y += 34;

        // ── Призначення ──
        var grpAssign = new GroupBox
        {
            Text = "Призначення (локація або особа)",
            Location = new Point(14, y),
            Size = new Size(w, 130)
        };

        rbNone = new RadioButton
        {
            Text = "Не призначена",
            Location = new Point(10, 22),
            AutoSize = true,
            Checked = true
        };

        rbLocation = new RadioButton
        {
            Text = "На локації:",
            Location = new Point(10, 48),
            AutoSize = true
        };

        cmbLocation = new ComboBox
        {
            Location = new Point(140, 46),
            Width = w - 160,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Enabled = false
        };

        rbPerson = new RadioButton
        {
            Text = "За особою:",
            Location = new Point(10, 78),
            AutoSize = true
        };

        cmbPerson = new ComboBox
        {
            Location = new Point(140, 76),
            Width = w - 160,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Enabled = false
        };

        rbLocation.CheckedChanged += (_, _) => cmbLocation.Enabled = rbLocation.Checked;
        rbPerson.CheckedChanged += (_, _) => cmbPerson.Enabled = rbPerson.Checked;

        grpAssign.Controls.AddRange([rbNone, rbLocation, cmbLocation, rbPerson, cmbPerson]);
        Controls.Add(grpAssign);
        y += grpAssign.Height + 10;

        // Заповнюємо локації
        var locations = _context.Locations.OrderBy(l => l.LocationName).ToList();
        foreach (var loc in locations)
            cmbLocation.Items.Add(loc);
        cmbLocation.DisplayMember = "LocationName";
        if (cmbLocation.Items.Count > 0) cmbLocation.SelectedIndex = 0;

        // Заповнюємо осіб
        var people = _context.People
            .Include(p => p.Rank)
            .OrderBy(p => p.LastName)
            .ToList();
        foreach (var p in people)
            cmbPerson.Items.Add(p);
        cmbPerson.DisplayMember = "LastName";
        cmbPerson.Format += (_, args) =>
        {
            if (args.ListItem is Person p)
                args.Value = $"{p.Rank?.RankName} {p.LastName} {p.Initials}";
        };
        cmbPerson.FormattingEnabled = true;
        if (cmbPerson.Items.Count > 0) cmbPerson.SelectedIndex = 0;

        // Встановити поточне значення
        if (_existing?.StoredInLocationId != null)
        {
            rbLocation.Checked = true;
            var current = locations.FirstOrDefault(l => l.LocationId == _existing.StoredInLocationId);
            if (current != null) cmbLocation.SelectedItem = current;
        }
        else if (_existing?.AssignedToPersonId != null)
        {
            rbPerson.Checked = true;
            var current = people.FirstOrDefault(p => p.PersonId == _existing.AssignedToPersonId);
            if (current != null) cmbPerson.SelectedItem = current;
        }

        // ── Кнопки ──
        var btnOk = new Button { Text = "OK", Location = new Point(270, y), Size = new Size(80, 32) };
        btnOk.Click += BtnOk_Click;

        var btnCancel = new Button
        {
            Text = "Скасувати",
            DialogResult = DialogResult.Cancel,
            Location = new Point(360, y),
            Size = new Size(100, 32)
        };

        Controls.AddRange([btnOk, btnCancel]);
        AcceptButton = btnOk;
        CancelButton = btnCancel;
        ClientSize = new Size(w + 30, y + 45);
    }

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtType.Text) || string.IsNullOrWhiteSpace(txtNumber.Text))
        {
            MessageBox.Show("Заповніть тип та номер зброї!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Взаємовиключні: або локація, або особа, або нічого
        int? locationId = null;
        int? personId = null;

        if (rbLocation.Checked && cmbLocation.SelectedItem is Location loc)
            locationId = loc.LocationId;
        else if (rbPerson.Checked && cmbPerson.SelectedItem is Person p)
            personId = p.PersonId;

        if (_existing != null)
        {
            _existing.WeaponType = txtType.Text.Trim();
            _existing.WeaponNumber = txtNumber.Text.Trim();
            _existing.StoredInLocationId = locationId;
            _existing.AssignedToPersonId = personId;
            ResultWeapon = _existing;
        }
        else
        {
            ResultWeapon = new Weapon
            {
                WeaponType = txtType.Text.Trim(),
                WeaponNumber = txtNumber.Text.Trim(),
                StoredInLocationId = locationId,
                AssignedToPersonId = personId
            };
        }

        DialogResult = DialogResult.OK;
        Close();
    }
}
