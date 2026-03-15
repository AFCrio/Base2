using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Base2.Forms;

public partial class AssignmentForm
{
    // ═══════════════════════════════════════════════════
    //  BUILD UI
    // ═══════════════════════════════════════════════════

    private void BuildUI()
    {
        Text = "Призначення особи -";
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

        var personsContextMenu = new ContextMenuStrip();
        var miEditPerson = new ToolStripMenuItem("Редагувати особу");
        miEditPerson.Click += (_, _) => EditSelectedPerson();
        personsContextMenu.Items.Add(miEditPerson);

        dgvPersons.ContextMenuStrip = personsContextMenu;
        dgvPersons.MouseDown += DgvPersons_MouseDown;
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
        rbAssignedWeapon.CheckedChanged += RbAssignedWeapon_CheckedChanged;
        rbOtherWeapon.CheckedChanged += RbOtherWeapon_CheckedChanged;

        cmbOtherWeapon = new ComboBox
        {
            Location = new Point(170, 46),
            Width = w - 190,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbOtherWeapon.SelectedIndexChanged += CmbOtherWeapon_SelectedIndexChanged;

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

        var lblAmmoCount = new Label { Text = "Кількість:", Location = new Point(200, 25), AutoSize = true };
        numAmmoCount = new NumericUpDown
        {
            Location = new Point(290, 22),
            Width = 80,
            Minimum = 0,
            Maximum = 9999,
            Value = 0
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
}
