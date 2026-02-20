using Base2.Data;
using Base2.Forms;
using Base2.Models;
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
            this.FormClosed += MainForm_FormClosed;

            RefreshOrdersGrid();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _context?.Dispose(); // Освобождаем контекст БД
        }

        private void RefreshOrdersGrid()
        {
            dataGridViewOrders.DataSource = _context.DutyOrders
                .Include(o => o.Sections)
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.DutyOrderId)
                .Select(o => new
                {
                    ID = o.DutyOrderId,
                    Номер = o.OrderNumber,
                    Дата = o.OrderDate.ToString("dd.MM.yyyy"),
                    Початок = o.StartDateTime.ToString("dd.MM.yyyy HH:mm"),
                    Кінець = o.EndDateTime.ToString("dd.MM.yyyy HH:mm"),
                    Секцій = o.Sections.Count(s => s.ParentDutySectionNodeId == null),
                    Командир = o.CommanderInfo.Substring(0, Math.Min(50, o.CommanderInfo.Length)) + "..."
                })
                .ToList();

            dataGridViewOrders.Columns["ID"].Visible = false;
            dataGridViewOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnNewOrder_Click(object sender, EventArgs e)
        {
            using var dialog = new NewDutyOrderDialog(_context);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                RefreshOrdersGrid();
                MessageBox.Show($"Створено наказ {dialog.CreatedOrder!.OrderNumber}", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnEditStructure_Click(object sender, EventArgs e)
        {
            if (dataGridViewOrders.CurrentRow?.Cells["ID"].Value is int orderId)
            {
                using var editor = new OrderEditorForm(orderId);
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    RefreshOrdersGrid();
                }
            }
            else
            {
                MessageBox.Show("Виберіть наказ для редагування!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteOrder_Click(object sender, EventArgs e)
        {
            if (dataGridViewOrders.CurrentRow?.Cells["ID"].Value is int orderId)
            {
                var result = MessageBox.Show("Видалити вибраний наказ разом з усіма секціями?",
                    "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        var order = _context.DutyOrders.Find(orderId);
                        if (order != null)
                        {
                            _context.DutyOrders.Remove(order);
                            _context.SaveChanges();
                            RefreshOrdersGrid();
                            MessageBox.Show("Наказ видалено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка видалення: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshOrdersGrid();
        }

        private void dataGridViewOrders_SelectionChanged(object sender, EventArgs e)
        {
            btnEditStructure.Enabled = dataGridViewOrders.CurrentRow != null;
            btnDeleteOrder.Enabled = dataGridViewOrders.CurrentRow != null;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _context.Dispose();
            base.OnFormClosed(e);
        }
    }
}
