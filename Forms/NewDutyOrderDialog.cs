using Base2.Data;
using Base2.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Base2.Forms
{
    public partial class NewDutyOrderDialog : Form
    {
        private readonly AppDbContext _context;
        private int _selectedTemplateId;
        public DutyOrder? CreatedOrder { get; private set; }

        public NewDutyOrderDialog(AppDbContext? context = null)
        {
            InitializeComponent();
            _context = context ?? new AppDbContext();
            LoadLocations();
            LoadTemplates();
        }

        private void LoadLocations()
        {
            comboBoxLocation.DataSource = _context.Locations.ToList();
            comboBoxLocation.DisplayMember = "LocationName";
            comboBoxLocation.ValueMember = "LocationId";
        }

        private void LoadTemplates()
        {
            var templates = _context.DutyTemplates
                .Where(t => t.IsActive)
                .ToList();

            if (templates.Count > 0)
            {
                _selectedTemplateId = templates[0].DutyTemplateId;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            CreatedOrder = new DutyOrder
            {
                OrderNumber = textBoxOrderNumber.Text.Trim(),
                OrderDate = DateOnly.FromDateTime(dateTimeOrderDate.Value),
                StartDateTime = dateTimeStart.Value,
                EndDateTime = dateTimeEnd.Value,
                CommanderInfo = textBoxCommander.Text.Trim(),
                SourceTemplateId = _selectedTemplateId
            };

            _context.DutyOrders.Add(CreatedOrder);
            _context.SaveChanges();

            // Автоматически добавляем кореневий вузол
            var rootNode = new DutySectionNode
            {
                DutyOrderId = CreatedOrder.DutyOrderId,
                NodeType = NodeType.SectionHeader,
                OrderIndex = 0,
                Title = "1"
            };
            _context.DutySectionNodes.Add(rootNode);
            _context.SaveChanges();

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBoxOrderNumber.Text))
            {
                MessageBox.Show("Введіть номер наказу!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dateTimeStart.Value >= dateTimeEnd.Value)
            {
                MessageBox.Show("Дата початку не може бути пізніше дати закінчення!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Проверка уникальности номера
            if (_context.DutyOrders.Any(o => o.OrderNumber == textBoxOrderNumber.Text.Trim()))
            {
                MessageBox.Show("Наказ з таким номером уже існує!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void dateTimeStart_ValueChanged(object sender, EventArgs e)
        {
            dateTimeEnd.Value = dateTimeStart.Value.AddDays(1); // Автозаполнение на 24ч вперед
        }
    }
}
