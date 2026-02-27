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
/// Форма призначення особи на позицію в наказі.
/// Пошук по прізвищу, автопідстановка зброї, валідація унікальності.
/// </summary>
public class AssignmentForm : Form
{
    private readonly AppDbContext _context;
    private readonly DutyOrder _order;
    private readonly DutySectionNode _node;
    private readonly int? _locationId;

    // Вже призначені ресурси в цьому наказі
    private readonly HashSet<int> _assignedPersonIds;
    private readonly HashSet<int> _assignedWeaponIds;
    private readonly HashSet<int> _assignedVehicleIds;

    // Дані
    private List<PersonRow> _allPersons = [];
    private List<PersonRow> _filteredPersons = [];
    private Person? _selectedPerson;

    // Результат
    public DutyAssignment? Result { get; private set; }

    // ── UI Controls ──
    private Label lblHeader = null!;
    private Label lblRequirements = null!;
    private TextBox txtSearch = null!;
    private DataGridView dgvPersons = null!;
    private Label lblSelected = null!;

    // Зброя
    private GroupBox grpWeapon = null!;
    private RadioButton rbAssignedWeapon = null!;
    private RadioButton rbOtherWeapon = null!;
    private ComboBox cmbOtherWeapon = null!;

    // Набої
    private GroupBox grpAmmo = null!;
    private ComboBox cmbAmmoType = null!;
    private NumericUpDown numAmmoCount = null!;

    // Транспорт
    private GroupBox grpVehicle = null!;
    private ComboBox cmbVehicle = null!;

    private Button btnOk = null!;
    private Button btnCancel = null!;

    /// <summary>
    /// DTO для таблиці осіб
    /// </summary>
    private record PersonRow(int PersonId, string LastName, string Initials, string Rank, string Position);

    public AssignmentForm(AppDbContext context, DutyOrder order, DutySectionNode node)
    {
        _context = context;
        _order = order;
        _node = node;
        _locationId = ResolveLocationId(node);

        // Завантажити вже призначені ресурси
        var existing = _context.DutyAssignments
            .Where(a => a.DutySectionNode.DutyOrderId == _order.DutyOrderId)
            .Select(a => new { a.PersonId, a.WeaponId, a.VehicleId })
            .ToList();

        _assignedPersonIds = existing.Select(a => a.PersonId).ToHashSet();
        _assignedWeaponIds = existing.Where(a => a.WeaponId.HasValue).Select(a => a.WeaponId!.Value).ToHashSet();
        _assignedVehicleIds = existing.Where(a => a.VehicleId.HasValue).Select(a => a.VehicleId!.Value).ToHashSet();

        BuildUI();
        LoadPersons();
    }

    /// <summary>
    /// Шукає LocationId вгору по дереву (від вузла до батьківського SectionHeader).
    /// </summary>
    private int? ResolveLocationId(DutySectionNode node)
    {
        var current = node;
        while (current != null)
        {
            if (current.LocationId.HasValue)
                return current.LocationId;

            current = current.ParentDutySectionNodeId.HasValue
                ? _context.DutySectionNodes.Find(current.ParentDutySectionNodeId)
                : null;
        }
        return null;
    }

    // ═══════════════════════════════════════════════════
    //  BUILD UI
    // ═══════════════════════════════════════════════════

    private void BuildUI()
    {
        Text = "Призначення особи";
        Size = new Size(680, 700);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Font = new Font("Segoe UI", 10F);

        int y = 12;
        int w = ClientSize.Width - 28;

        // ── Заголовок ──
        lblHeader = new Label
        {
            Text = $"Призначення: {_node.DutyPositionTitle ?? _node.NodeType.ToString()}",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            Location = new Point(14, y),
            AutoSize = true,
            MaximumSize = new Size(w, 0)
        };
        Controls.Add(lblHeader);
        y += lblHeader.PreferredHeight + 4;

        // Вимоги
        var reqs = new List<string>();
        if (_node.HasWeapon) reqs.Add("зброя");
        if (_node.HasAmmo) reqs.Add("набої");
        if (_node.HasVehicle) reqs.Add("транспорт");

        var locText = _locationId.HasValue
            ? _context.Locations.Find(_locationId)?.LocationName ?? ""
            : "(без локації)";

        lblRequirements = new Label
        {
            Text = $"Вимоги: {(reqs.Count > 0 ? string.Join(" + ", reqs) : "немає")}    |    Локація: {locText}",
            ForeColor = Color.DimGray,
            Location = new Point(14, y),
            AutoSize = true,
            MaximumSize = new Size(w, 0)
        };
        Controls.Add(lblRequirements);
        y += lblRequirements.PreferredHeight + 10;

        // ── Пошук ──
        var lblSearch = new Label
        {
            Text = "Пошук за прізвищем:",
            Location = new Point(14, y),
            AutoSize = true
        };
        Controls.Add(lblSearch);
        y += lblSearch.PreferredHeight + 2;

        var btnAddPerson = new Button
        {
            Text = "➕ Нова особа",
            Location = new Point(w - 130, y),
            Size = new Size(144, 28),
            BackColor = Color.LightGreen,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };
        btnAddPerson.Click += BtnAddPerson_Click;
        Controls.Add(btnAddPerson);

        txtSearch = new TextBox
        {
            Location = new Point(14, y),
            Width = w - 150,
            Font = new Font("Consolas", 11F),
            PlaceholderText = "Почніть вводити прізвище…"
        };
        txtSearch.TextChanged += TxtSearch_TextChanged;
        txtSearch.KeyDown += TxtSearch_KeyDown;
        Controls.Add(txtSearch);
        y += txtSearch.Height + 6;

        // ── Таблиця осіб ──
        dgvPersons = new DataGridView
        {
            Location = new Point(14, y),
            Size = new Size(w, 180),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Font = new Font("Segoe UI", 9.5F),
            BackgroundColor = SystemColors.Window
        };
        dgvPersons.CellDoubleClick += DgvPersons_CellDoubleClick;
        Controls.Add(dgvPersons);
        y += dgvPersons.Height + 8;

        // ── Вибрана особа ──
        lblSelected = new Label
        {
            Text = "Вибрано: (натисніть Enter або двічі клікніть)",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.DarkBlue,
            Location = new Point(14, y),
            AutoSize = true,
            MaximumSize = new Size(w, 0)
        };
        Controls.Add(lblSelected);
        y += lblSelected.PreferredHeight + 10;

        // ── Секція зброї ──
        grpWeapon = new GroupBox
        {
            Text = "Зброя",
            Location = new Point(14, y),
            Size = new Size(w, 80),
            Visible = _node.HasWeapon
        };

        rbAssignedWeapon = new RadioButton
        {
            Text = "(закріплена зброя)",
            Location = new Point(10, 22),
            AutoSize = true,
            Enabled = false
        };

        rbOtherWeapon = new RadioButton
        {
            Text = "Зі зброєкімнати:",
            Location = new Point(10, 48),
            AutoSize = true,
            Checked = true
        };
        rbOtherWeapon.CheckedChanged += (_, _) => cmbOtherWeapon.Enabled = rbOtherWeapon.Checked;

        cmbOtherWeapon = new ComboBox
        {
            Location = new Point(170, 46),
            Width = w - 190,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        grpWeapon.Controls.AddRange([rbAssignedWeapon, rbOtherWeapon, cmbOtherWeapon]);
        Controls.Add(grpWeapon);
        if (_node.HasWeapon) y += grpWeapon.Height + 6;

        // ── Секція набоїв ──
        grpAmmo = new GroupBox
        {
            Text = "Набої",
            Location = new Point(14, y),
            Size = new Size(w, 60),
            Visible = _node.HasAmmo
        };

        var lblAmmoType = new Label { Text = "Тип:", Location = new Point(10, 25), AutoSize = true };
        cmbAmmoType = new ComboBox
        {
            Location = new Point(50, 22),
            Width = 120,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbAmmoType.Items.AddRange(["5,45 мм", "7,62 мм", "9 мм", "5,56 мм"]);
        cmbAmmoType.SelectedIndex = 0;

        var lblAmmoCount = new Label { Text = "Кількість:", Location = new Point(200, 25), AutoSize = true };
        numAmmoCount = new NumericUpDown
        {
            Location = new Point(290, 22),
            Width = 80,
            Minimum = 1,
            Maximum = 9999,
            Value = 120
        };

        grpAmmo.Controls.AddRange([lblAmmoType, cmbAmmoType, lblAmmoCount, numAmmoCount]);
        Controls.Add(grpAmmo);
        if (_node.HasAmmo) y += grpAmmo.Height + 6;

        // ── Секція транспорту ──
        grpVehicle = new GroupBox
        {
            Text = "Транспорт",
            Location = new Point(14, y),
            Size = new Size(w, 60),
            Visible = _node.HasVehicle
        };

        cmbVehicle = new ComboBox
        {
            Location = new Point(10, 22),
            Width = w - 30,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        grpVehicle.Controls.Add(cmbVehicle);
        Controls.Add(grpVehicle);
        if (_node.HasVehicle) y += grpVehicle.Height + 6;

        // ── Кнопки ──
        y += 10;
        btnOk = new Button
        {
            Text = "Призначити",
            DialogResult = DialogResult.None,
            Size = new Size(120, 35),
            Location = new Point(w - 240, y),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };
        btnOk.Click += BtnOk_Click;

        btnCancel = new Button
        {
            Text = "Скасувати",
            DialogResult = DialogResult.Cancel,
            Size = new Size(110, 35),
            Location = new Point(w - 110, y)
        };

        Controls.AddRange([btnOk, btnCancel]);
        AcceptButton = null; // Enter обробляється вручну
        CancelButton = btnCancel;

        // Підлаштовуємо висоту форми
        ClientSize = new Size(ClientSize.Width, y + 50);

        // Завантажити ресурси
        if (_node.HasWeapon) LoadAvailableWeapons();
        if (_node.HasVehicle) LoadAvailableVehicles();
    }

    // ═══════════════════════════════════════════════════
    //  DATA LOADING
    // ═══════════════════════════════════════════════════

    private void LoadPersons()
    {
        var people = _context.People
            .Include(p => p.Rank)
            .Include(p => p.Position)
            .Where(p => !_assignedPersonIds.Contains(p.PersonId))
            .OrderBy(p => p.LastName)
            .ToList();

        _allPersons = people.Select(p => new PersonRow(
            p.PersonId,
            p.LastName,
            p.Initials ?? "",
            p.Rank?.RankName ?? "",
            p.Position?.PositionName ?? ""
        )).ToList();

        _filteredPersons = _allPersons;
        RefreshGrid();
    }

    private void LoadAvailableWeapons()
    {
        var query = _context.Weapons
            .Where(w => !_assignedWeaponIds.Contains(w.WeaponId));

        if (_locationId.HasValue)
            query = query.Where(w => w.StoredInLocationId == _locationId);

        var weapons = query
            .OrderBy(w => w.WeaponType)
            .ThenBy(w => w.WeaponNumber)
            .ToList();

        cmbOtherWeapon.DisplayMember = "Display";
        cmbOtherWeapon.ValueMember = "WeaponId";
        cmbOtherWeapon.DataSource = weapons.Select(w => new
        {
            w.WeaponId,
            Display = $"{w.WeaponType} №{w.WeaponNumber}"
        }).ToList();
    }

    private void LoadAvailableVehicles()
    {
        var vehicles = _context.Vehicles
            .Where(v => !_assignedVehicleIds.Contains(v.VehicleId))
            .OrderBy(v => v.VehicleName)
            .ThenBy(v => v.VehicleNumber)
            .ToList();

        cmbVehicle.DisplayMember = "Display";
        cmbVehicle.ValueMember = "VehicleId";
        cmbVehicle.DataSource = vehicles.Select(v => new
        {
            v.VehicleId,
            Display = $"{v.VehicleName} {v.VehicleNumber}"
        }).ToList();
    }

    // ═══════════════════════════════════════════════════
    //  SEARCH & GRID
    // ═══════════════════════════════════════════════════

    private void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        var filter = txtSearch.Text.Trim();

        _filteredPersons = string.IsNullOrEmpty(filter)
            ? _allPersons
            : _allPersons.Where(p => p.LastName.StartsWith(filter, StringComparison.OrdinalIgnoreCase)).ToList();

        RefreshGrid();

        if (dgvPersons.Rows.Count > 0)
        {
            dgvPersons.ClearSelection();
            dgvPersons.Rows[0].Selected = true;
            var col = dgvPersons.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
            if (col != null)
                dgvPersons.CurrentCell = dgvPersons.Rows[0].Cells[col.Index];
        }
    }

    private void TxtSearch_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Down:
                MoveGridSelection(1);
                e.Handled = true;
                e.SuppressKeyPress = true;
                break;

            case Keys.Up:
                MoveGridSelection(-1);
                e.Handled = true;
                e.SuppressKeyPress = true;
                break;

            case Keys.Enter:
                SelectCurrentPerson();
                e.Handled = true;
                e.SuppressKeyPress = true;
                break;
        }
    }

    private void MoveGridSelection(int delta)
    {
        if (dgvPersons.Rows.Count == 0) return;

        var current = dgvPersons.CurrentRow?.Index ?? 0;
        var next = Math.Clamp(current + delta, 0, dgvPersons.Rows.Count - 1);

        dgvPersons.ClearSelection();
        dgvPersons.Rows[next].Selected = true;
        var col = dgvPersons.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        if (col != null)
            dgvPersons.CurrentCell = dgvPersons.Rows[next].Cells[col.Index];
    }

    private void RefreshGrid()
    {
        dgvPersons.DataSource = _filteredPersons.Select(p => new
        {
            p.PersonId,
            Прізвище = p.LastName,
            Ініціали = p.Initials,
            Звання = p.Rank,
            Посада = p.Position
        }).ToList();

        if (dgvPersons.Columns.Contains("PersonId"))
            dgvPersons.Columns["PersonId"].Visible = false;
    }

    private void DgvPersons_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
            SelectCurrentPerson();
    }

    // ═══════════════════════════════════════════════════
    //  PERSON SELECTION
    // ═══════════════════════════════════════════════════

    private void SelectCurrentPerson()
    {
        if (dgvPersons.CurrentRow == null) return;

        var personId = (int)dgvPersons.CurrentRow.Cells["PersonId"].Value;
        _selectedPerson = _context.People
            .Include(p => p.Rank)
            .Include(p => p.Position)
            .First(p => p.PersonId == personId);

        // Оновити заголовок
        lblSelected.Text = $"Вибрано: {_selectedPerson.Rank?.RankName} {_selectedPerson.LastName} {_selectedPerson.Initials}";

        // Підставити прізвище в поле пошуку
        txtSearch.TextChanged -= TxtSearch_TextChanged;
        txtSearch.Text = $"{_selectedPerson.LastName} {_selectedPerson.Initials}";
        txtSearch.TextChanged += TxtSearch_TextChanged;

        // Налаштувати секцію зброї
        ConfigureWeaponForPerson();

        // Фокус на наступне поле
        if (_node.HasWeapon)
            grpWeapon.Focus();
        else if (_node.HasVehicle)
            cmbVehicle.Focus();
        else
            btnOk.Focus();
    }

    private void ConfigureWeaponForPerson()
    {
        if (!_node.HasWeapon || _selectedPerson == null) return;

        // Шукаємо зброю, закріплену за цією особою
        var assignedWeapon = _context.Weapons
            .FirstOrDefault(w => w.AssignedToPersonId == _selectedPerson.PersonId
                              && !_assignedWeaponIds.Contains(w.WeaponId));

        if (assignedWeapon != null)
        {
            rbAssignedWeapon.Text = $"Закріплена: {assignedWeapon.WeaponType} №{assignedWeapon.WeaponNumber}";
            rbAssignedWeapon.Tag = assignedWeapon.WeaponId;
            rbAssignedWeapon.Enabled = true;
            rbAssignedWeapon.Checked = true;
            cmbOtherWeapon.Enabled = false;
        }
        else
        {
            rbAssignedWeapon.Text = "Закріплена зброя не знайдена";
            rbAssignedWeapon.Tag = null;
            rbAssignedWeapon.Enabled = false;
            rbOtherWeapon.Checked = true;
            cmbOtherWeapon.Enabled = true;
        }
    }

    // ═══════════════════════════════════════════════════
    //  ADD NEW PERSON
    // ═══════════════════════════════════════════════════

    private void BtnAddPerson_Click(object? sender, EventArgs e)
    {
        using var dlg = new PersonEditDialog(_context);
        if (dlg.ShowDialog() != DialogResult.OK || dlg.ResultPerson == null) return;

        _context.People.Add(dlg.ResultPerson);
        _context.SaveChanges();

        // Прив'язати зброю (тепер PersonId відомий)
        dlg.LinkWeaponToNewPerson(dlg.ResultPerson.PersonId);
        _context.SaveChanges();

        // Оновити список і вибрати нову особу
        LoadPersons();

        _selectedPerson = _context.People
            .Include(p => p.Rank)
            .Include(p => p.Position)
            .First(p => p.PersonId == dlg.ResultPerson.PersonId);

        lblSelected.Text = $"Вибрано: {_selectedPerson.Rank?.RankName} {_selectedPerson.LastName} {_selectedPerson.Initials}";

        txtSearch.TextChanged -= TxtSearch_TextChanged;
        txtSearch.Text = $"{_selectedPerson.LastName} {_selectedPerson.Initials}";
        txtSearch.TextChanged += TxtSearch_TextChanged;

        ConfigureWeaponForPerson();

        if (_node.HasWeapon)
            grpWeapon.Focus();
        else if (_node.HasVehicle)
            cmbVehicle.Focus();
        else
            btnOk.Focus();
    }

    // ═══════════════════════════════════════════════════
    //  OK / VALIDATION
    // ═══════════════════════════════════════════════════

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        if (_selectedPerson == null)
        {
            MessageBox.Show("Оберіть особу!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtSearch.Focus();
            return;
        }

        // Перевірка унікальності особи
        if (_assignedPersonIds.Contains(_selectedPerson.PersonId))
        {
            MessageBox.Show(
                $"{_selectedPerson.LastName} {_selectedPerson.Initials} вже призначено в цьому наказі!",
                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Зброя
        int? weaponId = null;
        if (_node.HasWeapon)
        {
            if (rbAssignedWeapon.Checked && rbAssignedWeapon.Tag is int assignedId)
            {
                weaponId = assignedId;
            }
            else if (rbOtherWeapon.Checked && cmbOtherWeapon.SelectedValue is int otherId)
            {
                weaponId = otherId;
            }
            else
            {
                MessageBox.Show("Оберіть зброю!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_assignedWeaponIds.Contains(weaponId.Value))
            {
                MessageBox.Show("Ця зброя вже призначена в цьому наказі!", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        // Транспорт
        int? vehicleId = null;
        if (_node.HasVehicle)
        {
            if (cmbVehicle.SelectedValue is int vId)
            {
                vehicleId = vId;
            }
            else
            {
                MessageBox.Show("Оберіть транспорт!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_assignedVehicleIds.Contains(vehicleId.Value))
            {
                MessageBox.Show("Цей транспорт вже призначений в цьому наказі!", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        // Створюємо результат
        Result = new DutyAssignment
        {
            DutySectionNodeId = _node.DutySectionNodeId,
            PersonId = _selectedPerson.PersonId,
            WeaponId = weaponId,
            VehicleId = vehicleId,
            AmmoType = _node.HasAmmo ? cmbAmmoType.SelectedItem?.ToString() : null,
            AmmoCount = _node.HasAmmo ? (int)numAmmoCount.Value : null
        };

        DialogResult = DialogResult.OK;
        Close();
    }
}
