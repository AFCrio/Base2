
using Base2.Data;
using Base2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Base2.Forms
{
    public partial class AssignmentDialog : Form
    {
        private readonly AppDbContext _context;

        public Person? SelectedPerson { get; private set; }
        public Weapon? SelectedWeapon { get; private set; }
        public Vehicle? SelectedVehicle { get; private set; }
        public int? AmmoCount { get; private set; }
        public string? AmmoType { get; private set; }

        public AssignmentDialog(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            // Завантажуємо людей
            var people = _context.People
                .Include(p => p.Rank)
                .Include(p => p.Position)
                .OrderBy(p => p.LastName)
                .ToList();

            comboBoxPerson.DisplayMember = "DisplayText";
            comboBoxPerson.ValueMember = "PersonId";
            comboBoxPerson.DataSource = people.Select(p => new
            {
                p.PersonId,
                DisplayText = $"{p.Rank.RankName} {p.LastName} {p.Initials} ({p.Position.PositionName})"
            }).ToList();

            // Завантажуємо зброю
            var weapons = _context.Weapons
                .OrderBy(w => w.WeaponType)
                .ThenBy(w => w.WeaponNumber)
                .ToList();

            comboBoxWeapon.DisplayMember = "DisplayText";
            comboBoxWeapon.ValueMember = "WeaponId";
            comboBoxWeapon.DataSource = weapons.Select(w => new
            {
                w.WeaponId,
                DisplayText = $"{w.WeaponType} №{w.WeaponNumber}"
            }).ToList();

            // Завантажуємо транспорт
            var vehicles = _context.Vehicles
                .OrderBy(v => v.VehicleName)
                .ToList();

            comboBoxVehicle.DisplayMember = "DisplayText";
            comboBoxVehicle.ValueMember = "VehicleId";
            comboBoxVehicle.DataSource = vehicles.Select(v => new
            {
                v.VehicleId,
                DisplayText = $"{v.VehicleName} {v.VehicleNumber}"
            }).ToList();
        }

        private void checkBoxWeapon_CheckedChanged(object? sender, EventArgs e)
        {
            comboBoxWeapon.Enabled = checkBoxWeapon.Checked;
        }

        private void checkBoxAmmo_CheckedChanged(object? sender, EventArgs e)
        {
            numericAmmoCount.Enabled = checkBoxAmmo.Checked;
            textBoxAmmoType.Enabled = checkBoxAmmo.Checked;
        }

        private void checkBoxVehicle_CheckedChanged(object? sender, EventArgs e)
        {
            comboBoxVehicle.Enabled = checkBoxVehicle.Checked;
        }

        /// <summary>
        /// Попередньо встановити прапорець зброї (з прапорців вузла)
        /// </summary>
        public void PresetWeapon(bool enabled)
        {
            checkBoxWeapon.Checked = enabled;
            comboBoxWeapon.Enabled = enabled;
        }

        /// <summary>
        /// Попередньо встановити прапорець набоїв (з прапорців вузла)
        /// </summary>
        public void PresetAmmo(bool enabled)
        {
            checkBoxAmmo.Checked = enabled;
            numericAmmoCount.Enabled = enabled;
            textBoxAmmoType.Enabled = enabled;
        }

        /// <summary>
        /// Попередньо встановити прапорець транспорту (з прапорців вузла)
        /// </summary>
        public void PresetVehicle(bool enabled)
        {
            checkBoxVehicle.Checked = enabled;
            comboBoxVehicle.Enabled = enabled;
        }

        private void btnOK_Click(object? sender, EventArgs e)
        {
            if (comboBoxPerson.SelectedValue == null)
            {
                MessageBox.Show("Виберіть особу!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var personId = (int)comboBoxPerson.SelectedValue;
            SelectedPerson = _context.People
                .Include(p => p.Rank)
                .Include(p => p.Position)
                .FirstOrDefault(p => p.PersonId == personId);

            if (checkBoxWeapon.Checked && comboBoxWeapon.SelectedValue != null)
            {
                var weaponId = (int)comboBoxWeapon.SelectedValue;
                SelectedWeapon = _context.Weapons.Find(weaponId);
            }

            if (checkBoxVehicle.Checked && comboBoxVehicle.SelectedValue != null)
            {
                var vehicleId = (int)comboBoxVehicle.SelectedValue;
                SelectedVehicle = _context.Vehicles.Find(vehicleId);
            }

            if (checkBoxAmmo.Checked)
            {
                AmmoCount = (int)numericAmmoCount.Value;
                AmmoType = textBoxAmmoType.Text;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
