using Base2.Data;
using Base2.Models;
using Base2.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Base2.Forms
{
    public partial class OrderCreatorForm : Form
    {
        private AppDbContext _context;
        private DutyOrder? _order;
        private DutyTemplate? _template;
        private DutySectionNode? _selectedNode;

        /// <summary>
        /// TimeRange node ID → (DateTimePicker start, DateTimePicker end, DutyTimeRange entity)
        /// </summary>
        private readonly Dictionary<int, (DateTimePicker start, DateTimePicker end, DutyTimeRange timeRange)> _timeRangePickers = new();

        /// <summary>
        /// Створення нового наказу з шаблону
        /// </summary>
        public OrderCreatorForm(int templateId)
        {
            InitializeComponent();
            _context = AppServices.DbContext;
            _template = _context.DutyTemplates.Find(templateId);
        }

        /// <summary>
        /// Відкриття існуючого наказу для редагування призначень
        /// </summary>
        public OrderCreatorForm(DutyOrder existingOrder)
        {
            InitializeComponent();
            _context = AppServices.DbContext;
            _order = _context.DutyOrders
                .Include(o => o.SourceTemplate)
                .Include(o => o.TimeRanges)
                .FirstOrDefault(o => o.DutyOrderId == existingOrder.DutyOrderId);
            _template = _order?.SourceTemplate;
        }

        private void OrderCreatorForm_Load(object sender, EventArgs e)
        {
            if (_template == null)
            {
                MessageBox.Show("Шаблон не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            if (_order == null)
            {
                // Новий наказ — показуємо порожні метадані
                lblTemplateValue.Text = _template.TemplateName;
                dtpStart.Value = DateTime.Today.AddHours(8);
                dtpEnd.Value = DateTime.Today.AddDays(1).AddHours(8);
            }
            else
            {
                // Існуючий наказ — заповнюємо метадані
                LoadOrderMetadata();
            }

            if (_order != null)
            {
                LoadOrderTree();
                BuildTimeRangePanel();
            }
        }

        #region Order Creation

        /// <summary>
        /// Створити наказ з шаблону (викликається при першому збереженні)
        /// </summary>
        private bool CreateOrderFromTemplate()
        {
            if (_template == null) return false;

            if (string.IsNullOrWhiteSpace(txtOrderNumber.Text))
            {
                MessageBox.Show("Введіть номер наказу!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtpStart.Value >= dtpEnd.Value)
            {
                MessageBox.Show("Дата початку має бути раніше дати закінчення!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Перевірка унікальності номера
            var orderNumber = txtOrderNumber.Text.Trim();
            if (_context.DutyOrders.Any(o => o.OrderNumber == orderNumber))
            {
                MessageBox.Show("Наказ з таким номером вже існує!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                var service = new OrderService(_context);
                _order = service.CreateOrderFromTemplate(
                    _template.DutyTemplateId,
                    orderNumber,
                    DateOnly.FromDateTime(dtpOrderDate.Value),
                    dtpStart.Value,
                    dtpEnd.Value,
                    txtCommander.Text.Trim());

                // Перезавантажуємо з навігацією
                _order = _context.DutyOrders
                    .Include(o => o.SourceTemplate)
                    .Include(o => o.TimeRanges)
                    .First(o => o.DutyOrderId == _order.DutyOrderId);

                LoadOrderMetadata();
                LoadOrderTree();
                BuildTimeRangePanel();

                toolStripLabelStatus.Text = "Наказ створено!";
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка створення наказу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region Load / Refresh

        private void LoadOrderMetadata()
        {
            if (_order == null) return;

            lblTemplateValue.Text = _order.SourceTemplate?.TemplateName ?? "(невідомо)";
            txtOrderNumber.Text = _order.OrderNumber;
            dtpOrderDate.Value = _order.OrderDate.ToDateTime(TimeOnly.MinValue);
            dtpStart.Value = _order.StartDateTime;
            dtpEnd.Value = _order.EndDateTime;
            txtCommander.Text = _order.CommanderInfo;

            this.Text = $"Наказ {_order.OrderNumber} від {_order.OrderDate}";
        }

        private void LoadOrderTree()
        {
            if (_order == null) return;

            // Завантажуємо всі вузли наказу з навігацією
            var allNodes = _context.DutySectionNodes
                .Where(n => n.DutyOrderId == _order.DutyOrderId)
                .Include(n => n.Assignments)
                    .ThenInclude(a => a.Person)
                        .ThenInclude(p => p.Rank)
                .Include(n => n.Assignments)
                    .ThenInclude(a => a.Person)
                        .ThenInclude(p => p.Position)
                .Include(n => n.Assignments)
                    .ThenInclude(a => a.Weapon)
                .Include(n => n.Assignments)
                    .ThenInclude(a => a.Vehicle)
                .Include(n => n.DutyTimeRange)
                .OrderBy(n => n.OrderIndex)
                .ToList();

            // O(N) побудова lookup: ParentId → дочірні вузли (ILookup підтримує null-ключі)
            var childrenLookup = allNodes.ToLookup(n => n.ParentDutySectionNodeId);

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            foreach (var root in childrenLookup[null])
            {
                treeView1.Nodes.Add(CreateTreeNode(root, childrenLookup));
            }

            treeView1.ExpandAll();
            treeView1.EndUpdate();

            UpdateStatusLabel(allNodes);
        }

        private TreeNode CreateTreeNode(DutySectionNode section, ILookup<int?, DutySectionNode> childrenLookup)
        {
            string text = GetNodeDisplayText(section);

            var node = new TreeNode(text)
            {
                Tag = section
            };

            // Колір залежно від заповненості
            if (IsAssignableNode(section))
            {
                if (section.Assignments.Count == 0)
                {
                    node.ForeColor = Color.Red;
                }
                else if (section.MaxAssignments > 0 && section.Assignments.Count < section.MaxAssignments)
                {
                    node.ForeColor = Color.DarkOrange;
                }
                else
                {
                    node.ForeColor = Color.DarkGreen;
                }
            }

            // TimeRange вузли — синій
            if (section.NodeType == NodeType.TimeRange)
            {
                node.ForeColor = Color.DarkBlue;
            }

            // Віртуальні дочірні вузли для груп з призначеннями (тільки UI)
            if (section.NodeType is NodeType.GroupInline or NodeType.GroupNested
                && section.Assignments.Count > 0)
            {
                int idx = 0;
                foreach (var assignment in section.Assignments)
                {
                    idx++;
                    var personText = TemplateRenderer.FormatAssignmentInline(assignment, section);

                    // GroupNested — з нумерацією підпунктів
                    if (section.NodeType == NodeType.GroupNested && !string.IsNullOrEmpty(section.Title))
                        personText = $"{section.Title}.{idx}. {personText};";

                    var childNode = new TreeNode(personText)
                    {
                        Tag = section,
                        ForeColor = Color.DarkGreen
                    };
                    node.Nodes.Add(childNode);
                }
            }

            // Реальні дочірні вузли з БД
            foreach (var child in childrenLookup[section.DutySectionNodeId])
            {
                node.Nodes.Add(CreateTreeNode(child, childrenLookup));
            }

            return node;
        }
        private string GetNodeDisplayText(DutySectionNode section)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(section.Title))
                parts.Add(section.Title + ".");

            if (!string.IsNullOrEmpty(section.DutyPositionTitle))
            {
                if (section.NodeType is NodeType.GroupInline or NodeType.GroupNested)
                {
                    // Тільки заголовок шаблону — особи будуть у дочірніх вузлах
                    var header = TemplateRenderer.Render(
                        section.DutyPositionTitle,
                        assignment: null,
                        timeRange: section.DutyTimeRange,
                        order: _order,
                        node: null);
                    parts.Add(header);
                }
                else
                {
                    var rendered = TemplateRenderer.RenderNode(section, _order);
                    if (rendered != null)
                        parts.Add(rendered);
                }
            }
            else
            {
                parts.Add($"[{section.NodeType}]");
            }

            // Кількість призначень для груп
            if ((section.NodeType == NodeType.GroupInline || section.NodeType == NodeType.GroupNested)
                && section.Assignments.Count > 0)
            {
                parts.Add($"({section.Assignments.Count} осіб)");
            }

            // Незаповнені SimplePosition
            if (section.MaxAssignments == 1 && section.Assignments.Count == 0 && IsAssignableNode(section))
            {
                parts.Add("⚠ не призначено");
            }

            return string.Join(" ", parts);
        }

        private static bool IsAssignableNode(DutySectionNode section)
        {
            return section.NodeType is NodeType.SimplePosition
                or NodeType.DriverPosition
                or NodeType.MedicalPosition
                or NodeType.GroupInline
                or NodeType.GroupNested
                or NodeType.FireGroupInline;
        }

        private void UpdateStatusLabel(List<DutySectionNode> allNodes)
        {
            var assignable = allNodes.Where(IsAssignableNode).ToList();
            var filled = assignable.Count(n => n.Assignments.Count > 0);
            toolStripLabelStatus.Text = $"Заповнено: {filled}/{assignable.Count}";
        }

        #endregion

        #region TimeRange Panel

        private void BuildTimeRangePanel()
        {
            if (_order == null) return;

            flowTimeRanges.Controls.Clear();
            _timeRangePickers.Clear();

            // Знаходимо всі TimeRange-вузли наказу
            var timeRangeNodes = _context.DutySectionNodes
                .Where(n => n.DutyOrderId == _order.DutyOrderId && n.NodeType == NodeType.TimeRange)
                .Include(n => n.DutyTimeRange)
                .OrderBy(n => n.OrderIndex)
                .ToList();

            if (timeRangeNodes.Count == 0)
            {
                grpTimeRanges.Visible = false;
                return;
            }

            grpTimeRanges.Visible = true;

            foreach (var trNode in timeRangeNodes)
            {
                if (trNode.DutyTimeRange == null) continue;

                var panel = new Panel
                {
                    Width = 310,
                    Height = 55,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(4)
                };

                var lblTitle = new Label
                {
                    Text = trNode.TimeRangeLabel ?? $"Зміна {trNode.OrderIndex}",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(6, 4)
                };

                var lblFrom = new Label
                {
                    Text = "З:",
                    AutoSize = true,
                    Location = new Point(6, 30)
                };

                var dtpStart = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "dd.MM.yyyy HH:mm",
                    Location = new Point(24, 27),
                    Size = new Size(130, 23),
                    Value = trNode.DutyTimeRange.Start
                };

                var lblTo = new Label
                {
                    Text = "До:",
                    AutoSize = true,
                    Location = new Point(160, 30)
                };

                var dtpEnd = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "dd.MM.yyyy HH:mm",
                    Location = new Point(182, 27),
                    Size = new Size(130, 23),
                    Value = trNode.DutyTimeRange.End
                };

                // Збереження при зміні
                var timeRange = trNode.DutyTimeRange;
                dtpStart.ValueChanged += (s, ev) =>
                {
                    timeRange.Start = dtpStart.Value;
                    _context.SaveChanges();
                };
                dtpEnd.ValueChanged += (s, ev) =>
                {
                    timeRange.End = dtpEnd.Value;
                    _context.SaveChanges();
                };

                panel.Controls.AddRange([lblTitle, lblFrom, dtpStart, lblTo, dtpEnd]);
                flowTimeRanges.Controls.Add(panel);

                _timeRangePickers[trNode.DutySectionNodeId] = (dtpStart, dtpEnd, timeRange);
            }
        }

        #endregion

        #region TreeView Selection

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is DutySectionNode section)
            {
                _selectedNode = section;
                ShowNodeInfo(section);
                LoadAssignments(section);
            }
            else
            {
                _selectedNode = null;
                ClearNodeInfo();
                dataGridView1.DataSource = null;
            }
        }

        private void ShowNodeInfo(DutySectionNode section)
        {
            lblNodeTitle.Text = !string.IsNullOrEmpty(section.Title)
                ? $"{section.Title}. {section.DutyPositionTitle ?? section.NodeType.ToString()}"
                : section.DutyPositionTitle ?? section.NodeType.ToString();

            lblNodeType.Text = $"Тип: {section.NodeType}";

            var flags = new List<string>();
            if (section.HasWeapon) flags.Add("🔫 Зброя");
            if (section.HasAmmo) flags.Add("💣 Набої");
            if (section.HasVehicle) flags.Add("🚗 Транспорт");
            lblNodeFlags.Text = flags.Count > 0
                ? $"Прапорці: {string.Join(", ", flags)}"
                : "Прапорці: (немає)";

            // ── Рендеринг для txtRenderedText ──
            if (!string.IsNullOrEmpty(section.DutyPositionTitle))
            {
                txtRenderedText.Text = TemplateRenderer.RenderNode(section, _order) ?? "";
            }
            else
            {
                txtRenderedText.Text = "";
            }

            // Ліміт призначень
            if (IsAssignableNode(section))
            {
                var limit = section.MaxAssignments > 0
                    ? $"(макс. {section.MaxAssignments})"
                    : "(без обмежень)";
                lblAssignmentLimit.Text = $"{section.Assignments.Count} осіб {limit}";
            }
            else
            {
                lblAssignmentLimit.Text = "(вузол не для призначень)";
            }
        }

        private void ClearNodeInfo()
        {
            lblNodeTitle.Text = "";
            lblNodeType.Text = "";
            lblNodeFlags.Text = "";
            txtRenderedText.Text = "";
            lblAssignmentLimit.Text = "";
        }

        private void LoadAssignments(DutySectionNode section)
        {
            dataGridView1.DataSource = null;

            if (!section.Assignments.Any()) return;

            var assignments = section.Assignments.Select(a => new
            {
                a.DutyAssignmentId,
                ПІБ = $"{a.Person.Rank?.RankName} {a.Person.LastName} {a.Person.Initials}",
                Посада = a.Person.Position?.PositionName ?? "",
                Зброя = section.HasWeapon && a.Weapon != null
                    ? $"{a.Weapon.WeaponType} №{a.Weapon.WeaponNumber}"
                    : "-",
                Набої = section.HasAmmo && a.AmmoCount.HasValue
                    ? $"{a.AmmoType} – {a.AmmoCount} шт."
                    : "-",
                Транспорт = section.HasVehicle && a.Vehicle != null
                    ? $"{a.Vehicle.VehicleName} {a.Vehicle.VehicleNumber}"
                    : "-"
            }).ToList();

            dataGridView1.DataSource = assignments;

            // Ховаємо колонки, які не потрібні для цього вузла
            if (dataGridView1.Columns.Contains("DutyAssignmentId"))
                dataGridView1.Columns["DutyAssignmentId"].Visible = false;
            if (!section.HasWeapon && dataGridView1.Columns.Contains("Зброя"))
                dataGridView1.Columns["Зброя"].Visible = false;
            if (!section.HasAmmo && dataGridView1.Columns.Contains("Набої"))
                dataGridView1.Columns["Набої"].Visible = false;
            if (!section.HasVehicle && dataGridView1.Columns.Contains("Транспорт"))
                dataGridView1.Columns["Транспорт"].Visible = false;
        }

        #endregion

        #region Assignments

        private void btnAddAssignment_Click(object sender, EventArgs e)
        {
            if (_order == null)
            {
                // Перший збереження — створюємо наказ з шаблону
                if (!CreateOrderFromTemplate()) return;
            }

            if (_selectedNode == null)
            {
                MessageBox.Show("Виберіть вузол для призначення!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsAssignableNode(_selectedNode))
            {
                MessageBox.Show("Цей вузол не призначений для призначення осіб.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Перевірка ліміту
            if (_selectedNode.MaxAssignments > 0 && _selectedNode.Assignments.Count >= _selectedNode.MaxAssignments)
            {
                MessageBox.Show($"Досягнуто максимум призначень ({_selectedNode.MaxAssignments}) для цього вузла.",
                    "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var form = new AssignmentForm(_context, _order, _selectedNode);

            if (form.ShowDialog() == DialogResult.OK && form.Result != null)
            {
                _context.DutyAssignments.Add(form.Result);
                _context.SaveChanges();

                ReloadSelectedNode();
            }
        }

        private void btnRemoveAssignment_Click(object sender, EventArgs e)
        {
            if (_selectedNode == null || dataGridView1.SelectedRows.Count == 0) return;

            if (!dataGridView1.Columns.Contains("DutyAssignmentId")) return;

            var assignmentId = (int)dataGridView1.SelectedRows[0].Cells["DutyAssignmentId"].Value;
            var assignment = _context.DutyAssignments.Find(assignmentId);

            if (assignment != null)
            {
                var result = MessageBox.Show(
                    "Зняти призначення?",
                    "Підтвердження",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _context.DutyAssignments.Remove(assignment);
                    _context.SaveChanges();

                    ReloadSelectedNode();
                }
            }
        }

        /// <summary>
        /// Перезавантажує дерево і повертає вибір на поточний вузол
        /// </summary>
        private void ReloadSelectedNode()
        {
            var selectedNodeId = _selectedNode?.DutySectionNodeId;
            LoadOrderTree();

            if (selectedNodeId.HasValue)
            {
                SelectTreeNodeById(treeView1.Nodes, selectedNodeId.Value);
            }
        }

        private bool SelectTreeNodeById(TreeNodeCollection nodes, int nodeId)
        {
            foreach (TreeNode tn in nodes)
            {
                if (tn.Tag is DutySectionNode sn && sn.DutySectionNodeId == nodeId)
                {
                    treeView1.SelectedNode = tn;
                    return true;
                }

                if (SelectTreeNodeById(tn.Nodes, nodeId))
                    return true;
            }
            return false;
        }

        #endregion

        #region Toolbar

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_order == null)
            {
                CreateOrderFromTemplate();
                return;
            }

            try
            {
                // Оновлюємо метадані наказу
                _order.OrderNumber = txtOrderNumber.Text.Trim();
                _order.OrderDate = DateOnly.FromDateTime(dtpOrderDate.Value);
                _order.StartDateTime = dtpStart.Value;
                _order.EndDateTime = dtpEnd.Value;
                _order.CommanderInfo = txtCommander.Text.Trim();

                _context.SaveChanges();

                toolStripLabelStatus.Text = "Збережено!";
                MessageBox.Show("Наказ збережено успішно!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (_order == null) return;

            LoadOrderTree();
            BuildTimeRangePanel();
            toolStripLabelStatus.Text = "Оновлено";
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _context.ChangeTracker.Clear();
            base.OnFormClosing(e);
        }
    }
}
