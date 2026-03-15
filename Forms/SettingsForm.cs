using Base2.Data;
using Base2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Base2.Forms;

public class SettingsForm : Form
{
    private const string DefaultPersonRankKey = "PersonDefaults.RankId";
    private const string DefaultPersonPositionKey = "PersonDefaults.PositionId";

    private readonly AppDbContext _context;

    private ComboBox cmbDefaultRank = null!;
    private ComboBox cmbDefaultPosition = null!;

    public SettingsForm(AppDbContext context)
    {
        _context = context;
        BuildUI();
        LoadData();
    }

    private void BuildUI()
    {
        Text = "Налаштування";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Font = new Font("Segoe UI", 10F);

        int y = 12;
        int w = 460;

        var grpPersonDefaults = new GroupBox
        {
            Text = "Нова особа — значення за замовчуванням",
            Location = new Point(14, y),
            Size = new Size(w, 120)
        };

        var lblRank = new Label { Text = "Звання:", Location = new Point(12, 32), AutoSize = true };
        cmbDefaultRank = new ComboBox
        {
            Location = new Point(90, 29),
            Width = 350,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        var lblPosition = new Label { Text = "Посада:", Location = new Point(12, 72), AutoSize = true };
        cmbDefaultPosition = new ComboBox
        {
            Location = new Point(90, 69),
            Width = 350,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        grpPersonDefaults.Controls.AddRange([lblRank, cmbDefaultRank, lblPosition, cmbDefaultPosition]);
        Controls.Add(grpPersonDefaults);

        y += grpPersonDefaults.Height + 10;

        var btnSave = new Button
        {
            Text = "Зберегти",
            Location = new Point(290, y),
            Size = new Size(90, 32)
        };
        btnSave.Click += BtnSave_Click;

        var btnCancel = new Button
        {
            Text = "Скасувати",
            DialogResult = DialogResult.Cancel,
            Location = new Point(384, y),
            Size = new Size(90, 32)
        };

        Controls.AddRange([btnSave, btnCancel]);
        AcceptButton = btnSave;
        CancelButton = btnCancel;

        ClientSize = new Size(w + 28, y + 45);
    }

    private void LoadData()
    {
        var ranks = _context.Ranks.OrderBy(r => r.RankLevel).ToList();
        cmbDefaultRank.DataSource = ranks;
        cmbDefaultRank.DisplayMember = "RankName";
        cmbDefaultRank.ValueMember = "RankId";

        var positions = _context.Positions.OrderBy(p => p.PositionName).ToList();
        cmbDefaultPosition.DataSource = positions;
        cmbDefaultPosition.DisplayMember = "PositionName";
        cmbDefaultPosition.ValueMember = "PositionId";

        var rankSetting = GetSettingInt(DefaultPersonRankKey);
        if (rankSetting.HasValue && ranks.Any(r => r.RankId == rankSetting.Value))
            cmbDefaultRank.SelectedValue = rankSetting.Value;

        var positionSetting = GetSettingInt(DefaultPersonPositionKey);
        if (positionSetting.HasValue && positions.Any(p => p.PositionId == positionSetting.Value))
            cmbDefaultPosition.SelectedValue = positionSetting.Value;
    }

    private int? GetSettingInt(string key)
    {
        var value = _context.AppSettings
            .AsNoTracking()
            .Where(s => s.Key == key)
            .Select(s => s.Value)
            .FirstOrDefault();

        return int.TryParse(value, out var parsed) ? parsed : null;
    }

    private void SetSetting(string key, string value)
    {
        var existing = _context.AppSettings.Find(key);
        if (existing == null)
        {
            _context.AppSettings.Add(new AppSetting
            {
                Key = key,
                Value = value
            });
            return;
        }

        existing.Value = value;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (cmbDefaultRank.SelectedValue is not int rankId)
        {
            MessageBox.Show("Оберіть звання за замовчуванням.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbDefaultPosition.SelectedValue is not int positionId)
        {
            MessageBox.Show("Оберіть посаду за замовчуванням.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SetSetting(DefaultPersonRankKey, rankId.ToString());
        SetSetting(DefaultPersonPositionKey, positionId.ToString());

        _context.SaveChanges();

        DialogResult = DialogResult.OK;
        Close();
    }
}
