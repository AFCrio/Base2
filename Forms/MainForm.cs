using Base2.Data;
using Base2.Forms;
using Base2.Models;
using Base2.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Base2.Forms
{
    public partial class MainForm : Form
    {
        private readonly AppDbContext _context;

        public MainForm()
        {
            InitializeComponent();
            _context = new AppDbContext();

            LoadTemplates();
        }

        #region Templates

        private void LoadTemplates()
        {
            var templates = _context.DutyTemplates
                .OrderBy(t => t.TemplateName)
                .ToList();

            comboBoxTemplates.DataSource = templates;
            comboBoxTemplates.DisplayMember = "TemplateName";
            comboBoxTemplates.ValueMember = "DutyTemplateId";

            var hasTemplates = templates.Count > 0;
            btnEditTemplate.Enabled = hasTemplates;
            btnDeleteTemplate.Enabled = hasTemplates;
            btnNewOrder.Enabled = hasTemplates;

            if (hasTemplates)
                RefreshOrdersGrid();
            else
                dataGridViewOrders.DataSource = null;
        }

        private void comboBoxTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshOrdersGrid();
        }

        private DutyTemplate? GetSelectedTemplate()
        {
            return comboBoxTemplates.SelectedItem as DutyTemplate;
        }

        private void btnNewTemplate_Click(object sender, EventArgs e)
        {
            var name = PromptForText("Новий шаблон", "Назва шаблону:");
            if (string.IsNullOrWhiteSpace(name)) return;

            var template = new DutyTemplate
            {
                TemplateName = name.Trim(),
                IsActive = true
            };

            _context.DutyTemplates.Add(template);
            _context.SaveChanges();

            LoadTemplates();
            comboBoxTemplates.SelectedValue = template.DutyTemplateId;
        }

        private void btnEditTemplate_Click(object sender, EventArgs e)
        {
            var template = GetSelectedTemplate();
            if (template == null) return;

            using var editor = new TemplateEditorForm(template.DutyTemplateId);
            editor.ShowDialog();

            // Після редагування оновлюємо версію
            _context.Entry(template).Reload();
            LoadTemplates();
        }

        private void btnDeleteTemplate_Click(object sender, EventArgs e)
        {
            var template = GetSelectedTemplate();
            if (template == null) return;

            var orderCount = _context.DutyOrders.Count(o => o.SourceTemplateId == template.DutyTemplateId);
            var message = orderCount > 0
                ? $"Шаблон «{template.TemplateName}» використовується в {orderCount} наказ(ах).\n\nВідмітити як неактивний?"
                : $"Видалити шаблон «{template.TemplateName}»?\n\nВсі вузли шаблону будуть видалені!";

            var result = MessageBox.Show(message, "Підтвердження",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            try
            {
                if (orderCount > 0)
                {
                    template.IsActive = false;
                    template.UpdatedAt = DateTime.Now;
                }
                else
                {
                    _context.DutyTemplates.Remove(template);
                }

                _context.SaveChanges();
                LoadTemplates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Orders

        private void RefreshOrdersGrid()
        {
            var template = GetSelectedTemplate();
            if (template == null)
            {
                dataGridViewOrders.DataSource = null;
                labelOutdated.Text = "";
                return;
            }

            var orders = _context.DutyOrders
                .Where(o => o.SourceTemplateId == template.DutyTemplateId)
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.DutyOrderId)
                .Select(o => new
                {
                    ID = o.DutyOrderId,
                    Номер = o.OrderNumber,
                    Дата = o.OrderDate.ToString(),
                    Початок = o.StartDateTime.ToString("dd.MM.yyyy HH:mm"),
                    Кінець = o.EndDateTime.ToString("dd.MM.yyyy HH:mm"),
                    Версія = o.SourceTemplateVersion,
                    Секцій = o.Sections.Count(s => s.ParentDutySectionNodeId == null),
                    Командир = o.CommanderInfo
                })
                .ToList();

            dataGridViewOrders.DataSource = orders;

            if (dataGridViewOrders.Columns.Contains("ID"))
                dataGridViewOrders.Columns["ID"].Visible = false;

            dataGridViewOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Показуємо кількість застарілих наказів
            var outdatedCount = orders.Count(o => o.Версія < template.Version);
            labelOutdated.Text = outdatedCount > 0
                ? $"⚠️ {outdatedCount} наказ(ів) створено зі старої версії шаблону (v{template.Version})"
                : "";

            UpdateOrderButtons();
        }

        private void btnNewOrder_Click(object sender, EventArgs e)
        {
            var template = GetSelectedTemplate();
            if (template == null) return;

            using var creator = new OrderCreatorForm(template.DutyTemplateId);
            creator.ShowDialog();

            _context.ChangeTracker.Clear();
            RefreshOrdersGrid();
        }

        private void btnEditOrder_Click(object sender, EventArgs e)
        {
            OpenSelectedOrder();
        }

        private void btnDeleteOrder_Click(object sender, EventArgs e)
        {
            if (GetSelectedOrderId() is not int orderId) return;

            var order = _context.DutyOrders.Find(orderId);
            if (order == null) return;

            var result = MessageBox.Show(
                $"Видалити наказ «{order.OrderNumber}» разом з усіма секціями та призначеннями?",
                "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            try
            {
                // 1. Завантажуємо всі вузли наказу
                var nodes = _context.DutySectionNodes
                    .Where(n => n.DutyOrderId == orderId)
                    .ToList();

                var nodeIds = nodes.Select(n => n.DutySectionNodeId).ToList();

                // 2. Видаляємо призначення (DutyAssignment → DutySectionNode FK)
                var assignments = _context.DutyAssignments
                    .Where(a => nodeIds.Contains(a.DutySectionNodeId))
                    .ToList();
                _context.DutyAssignments.RemoveRange(assignments);

                // 3. Видаляємо часові діапазони (DutyTimeRange → DutyOrder FK)
                var timeRanges = _context.DutyTimeRanges
                    .Where(tr => tr.DutyOrderId == orderId)
                    .ToList();
                _context.DutyTimeRanges.RemoveRange(timeRanges);

                // 4. Видаляємо вузли знизу вгору (через self-ref FK з Restrict)
                //    Спочатку обнуляємо DutyTimeRangeId
                foreach (var n in nodes)
                    n.DutyTimeRangeId = null;
                _context.SaveChanges();

                //    Видаляємо листя → батьків (за глибиною)
                while (nodes.Count > 0)
                {
                    var leaves = nodes
                        .Where(n => !nodes.Any(c => c.ParentDutySectionNodeId == n.DutySectionNodeId))
                        .ToList();

                    _context.DutySectionNodes.RemoveRange(leaves);
                    _context.SaveChanges();

                    foreach (var leaf in leaves)
                        nodes.Remove(leaf);
                }

                // 5. Видаляємо сам наказ
                _context.DutyOrders.Remove(order);
                _context.SaveChanges();

                RefreshOrdersGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка видалення: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _context.ChangeTracker.Clear();
            LoadTemplates();
        }

        private void dataGridViewOrders_SelectionChanged(object sender, EventArgs e)
        {
            UpdateOrderButtons();
        }

        private void dataGridViewOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                OpenSelectedOrder();
        }

        private void OpenSelectedOrder()
        {
            if (GetSelectedOrderId() is not int orderId) return;

            var order = _context.DutyOrders.Find(orderId);
            if (order == null) return;

            using var creator = new OrderCreatorForm(order);
            creator.ShowDialog();

            _context.ChangeTracker.Clear();
            RefreshOrdersGrid();
        }

        private int? GetSelectedOrderId()
        {
            if (dataGridViewOrders.CurrentRow?.Cells["ID"].Value is int orderId)
                return orderId;

            MessageBox.Show("Виберіть наказ!", "Увага",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        private void UpdateOrderButtons()
        {
            var hasSelection = dataGridViewOrders.CurrentRow != null;
            btnEditOrder.Enabled = hasSelection;
            btnDeleteOrder.Enabled = hasSelection;
        }

        #endregion

        #region Helpers

        private static string? PromptForText(string title, string label)
        {
            using var form = new Form
            {
                Text = title,
                Size = new System.Drawing.Size(400, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label { Text = label, Left = 12, Top = 15, AutoSize = true };
            var txt = new TextBox { Left = 12, Top = 38, Width = 355 };
            var btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Left = 210,
                Top = 70,
                Width = 75
            };
            var btnCancel = new Button
            {
                Text = "Скасувати",
                DialogResult = DialogResult.Cancel,
                Left = 292,
                Top = 70,
                Width = 75
            };

            form.Controls.AddRange([lbl, txt, btnOk, btnCancel]);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txt.Text)
                ? txt.Text
                : null;
        }

        #endregion

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _context.Dispose();
            base.OnFormClosed(e);
        }
    }
}
