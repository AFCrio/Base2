using Base2.Data;
using Base2.Models;
using Base2.Services;
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
public partial class AssignmentForm : Form
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

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        BeginInvoke(() =>
        {
            txtSearch.Focus();
            txtSearch.SelectAll();
        });
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
            .OrderBy(w => w.LastUsedDate)
            .ThenBy(w => w.WeaponType)
            .ThenBy(w => w.WeaponNumber)
            .ToList();

        cmbOtherWeapon.DisplayMember = "Display";
        cmbOtherWeapon.ValueMember = "WeaponId";
        cmbOtherWeapon.DataSource = weapons.Select(w => new
        {
            w.WeaponId,
            Display = $"{w.WeaponType} №{w.WeaponNumber} ({w.LastUsedDate:dd.MM.yyyy})"
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

    private void DgvPersons_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;

        var hit = dgvPersons.HitTest(e.X, e.Y);
        if (hit.RowIndex < 0) return;

        dgvPersons.ClearSelection();
        dgvPersons.Rows[hit.RowIndex].Selected = true;

        var col = dgvPersons.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        if (col != null)
            dgvPersons.CurrentCell = dgvPersons.Rows[hit.RowIndex].Cells[col.Index];
    }

    private void EditSelectedPerson()
    {
        if (dgvPersons.CurrentRow == null) return;
        if (!dgvPersons.Columns.Contains("PersonId")) return;

        var personId = (int)dgvPersons.CurrentRow.Cells["PersonId"].Value;
        var person = _context.People
            .Include(p => p.Rank)
            .Include(p => p.Position)
            .FirstOrDefault(p => p.PersonId == personId);

        if (person == null) return;

        using var dlg = new PersonEditDialog(_context, person);
        if (dlg.ShowDialog() != DialogResult.OK) return;

        _context.SaveChanges();

        LoadPersons();

        foreach (DataGridViewRow row in dgvPersons.Rows)
        {
            if ((int)row.Cells["PersonId"].Value != personId) continue;

            row.Selected = true;
            var col = dgvPersons.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
            if (col != null)
                dgvPersons.CurrentCell = row.Cells[col.Index];

            SelectCurrentPerson();
            break;
        }
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

        // Після вибору особи фокус одразу на кнопку підтвердження
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

            TryApplyAmmoPresetByWeaponType(assignedWeapon.WeaponType);
        }
        else
        {
            rbAssignedWeapon.Text = "Закріплена зброя не знайдена";
            rbAssignedWeapon.Tag = null;
            rbAssignedWeapon.Enabled = false;
            rbOtherWeapon.Checked = true;
            cmbOtherWeapon.Enabled = true;

            if (cmbOtherWeapon.SelectedValue is int otherWeaponId)
                TryApplyAmmoPresetByWeaponId(otherWeaponId);
            else
                ClearAmmoFields();
        }
    }

    private void RbAssignedWeapon_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_node.HasAmmo || !rbAssignedWeapon.Checked) return;

        if (rbAssignedWeapon.Tag is int weaponId)
            TryApplyAmmoPresetByWeaponId(weaponId);
        else
            ClearAmmoFields();
    }

    private void RbOtherWeapon_CheckedChanged(object? sender, EventArgs e)
    {
        cmbOtherWeapon.Enabled = rbOtherWeapon.Checked;

        if (!_node.HasAmmo || !rbOtherWeapon.Checked) return;

        if (cmbOtherWeapon.SelectedValue is int weaponId)
            TryApplyAmmoPresetByWeaponId(weaponId);
        else
            ClearAmmoFields();
    }

    private void CmbOtherWeapon_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_node.HasAmmo || !rbOtherWeapon.Checked) return;

        if (cmbOtherWeapon.SelectedValue is int weaponId)
            TryApplyAmmoPresetByWeaponId(weaponId);
        else
            ClearAmmoFields();
    }

    private void TryApplyAmmoPresetByWeaponId(int weaponId)
    {
        var weaponType = _context.Weapons
            .Where(w => w.WeaponId == weaponId)
            .Select(w => w.WeaponType)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(weaponType))
        {
            ClearAmmoFields();
            return;
        }

        TryApplyAmmoPresetByWeaponType(weaponType);
    }

    private void TryApplyAmmoPresetByWeaponType(string weaponType)
    {
        if (!_node.HasAmmo) return;

        var normalizedWeaponType = weaponType.Trim().ToUpper();
        var preset = _context.WeaponAmmoPresets
            .AsNoTracking()
            .FirstOrDefault(p => p.WeaponType.ToUpper() == normalizedWeaponType);

        if (preset == null)
        {
            ClearAmmoFields();
            return;
        }

        if (!cmbAmmoType.Items.Contains(preset.AmmoType))
            cmbAmmoType.Items.Add(preset.AmmoType);

        cmbAmmoType.SelectedItem = preset.AmmoType;

        var value = Math.Clamp(preset.AmmoCount, (int)numAmmoCount.Minimum, (int)numAmmoCount.Maximum);
        numAmmoCount.Value = value;
    }

    private void ClearAmmoFields()
    {
        if (!_node.HasAmmo) return;

        cmbAmmoType.SelectedIndex = -1;
        numAmmoCount.Value = numAmmoCount.Minimum;
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
        var weapon = weaponId.HasValue ? _context.Weapons.Find(weaponId.Value) : null;
        var vehicle = vehicleId.HasValue ? _context.Vehicles.Find(vehicleId.Value) : null;

        var assignment = new DutyAssignment
        {
            DutySectionNodeId = _node.DutySectionNodeId,
            PersonId = _selectedPerson.PersonId,
            WeaponId = weaponId,
            VehicleId = vehicleId,
            AmmoType = _node.HasAmmo ? cmbAmmoType.SelectedItem?.ToString() : null,
            AmmoCount = _node.HasAmmo ? (int)numAmmoCount.Value : null,
            Person = _selectedPerson,
            Weapon = weapon,
            Vehicle = vehicle,
            DutySectionNode = _node
        };

        assignment.RenderedLine = BuildRenderedLine(assignment);
        Result = assignment;

        DialogResult = DialogResult.OK;
        Close();
    }

    private string BuildRenderedLine(DutyAssignment assignment)
    {
        if (_node.NodeType is NodeType.GroupInline or NodeType.GroupNested)
            return TemplateRenderer.FormatAssignmentInline(assignment, _node);

        if (string.IsNullOrWhiteSpace(_node.DutyPositionTitle))
            return TemplateRenderer.FormatAssignmentInline(assignment, _node);

        return TemplateRenderer.Render(
            _node.DutyPositionTitle,
            assignment,
            _node.DutyTimeRange,
            _order,
            _node);
    }
}
