using Base2.Data;
using Base2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Base2.Forms
{
    public partial class OrderEditorForm : Form
    {
        private AppDbContext _context;
        private DutyOrder? _currentOrder;
        private DutySectionNode? _selectedNode;

        public OrderEditorForm()
        {
            InitializeComponent();
            _context = new AppDbContext();
        }

        public OrderEditorForm(int orderId) : this()
        {
            LoadOrder(orderId);
        }

        private void OrderEditorForm_Load(object sender, EventArgs e)
        {
            // Налаштування TreeView
            treeView1.AllowDrop = true;
            treeView1.ItemDrag += TreeView1_ItemDrag;
            treeView1.DragEnter += TreeView1_DragEnter;
            treeView1.DragDrop += TreeView1_DragDrop;

            // Налаштування DataGridView
            SetupDataGridView();

            // Контекстне меню для TreeView
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Додати дочірній вузол", null, (s, ev) => AddChildNode());
            contextMenu.Items.Add("Додати сусідній вузол", null, (s, ev) => AddSiblingNode());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Видалити", null, (s, ev) => DeleteNode());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Перемістити вгору", null, (s, ev) => MoveNodeUp());
            contextMenu.Items.Add("Перемістити вниз", null, (s, ev) => MoveNodeDown());
            treeView1.ContextMenuStrip = contextMenu;
        }

        private void LoadOrder(int orderId)
        {
            _currentOrder = _context.DutyOrders
                .Include(o => o.Sections)
                    .ThenInclude(s => s.Children)
                .Include(o => o.Sections)
                    .ThenInclude(s => s.Assignments)
                        .ThenInclude(a => a.Person)
                            .ThenInclude(p => p.Rank)
                .Include(o => o.Sections)
                    .ThenInclude(s => s.Assignments)
                        .ThenInclude(a => a.Person)
                            .ThenInclude(p => p.Position)
                .Include(o => o.Sections)
                    .ThenInclude(s => s.Assignments)
                        .ThenInclude(a => a.Weapon)
                .Include(o => o.Sections)
                    .ThenInclude(s => s.Assignments)
                        .ThenInclude(a => a.Vehicle)
                .FirstOrDefault(o => o.DutyOrderId == orderId);

            if (_currentOrder == null)
            {
                MessageBox.Show("Наказ не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            RefreshTree();
        }

        private void RefreshTree()
        {
            if (_currentOrder == null) return;

            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            // Корінь
            var rootNode = new TreeNode($"Наказ {_currentOrder.OrderNumber} від {_currentOrder.OrderDate}")
            {
                Tag = _currentOrder,
                ImageIndex = 0,
                SelectedImageIndex = 0
            };
            treeView1.Nodes.Add(rootNode);

            // Секції верхнього рівня
            var rootSections = _currentOrder.Sections
                .Where(s => s.ParentDutySectionNodeId == null)
                .OrderBy(s => s.OrderIndex)
                .ToList();

            foreach (var section in rootSections)
            {
                var node = CreateTreeNode(section);
                rootNode.Nodes.Add(node);
            }

            rootNode.Expand();
            treeView1.EndUpdate();

            // Автоматично генеруємо Title для всіх вузлів
            GenerateTitles();
        }

        private TreeNode CreateTreeNode(DutySectionNode section)
        {
            string text = GetNodeText(section);

            var node = new TreeNode(text)
            {
                Tag = section,
                ImageIndex = GetIconIndex(section.NodeType),
                SelectedImageIndex = GetIconIndex(section.NodeType)
            };

            // Рекурсивно додаємо дочірні вузли
            foreach (var child in section.Children.OrderBy(c => c.OrderIndex))
            {
                node.Nodes.Add(CreateTreeNode(child));
            }

            return node;
        }

        private string GetNodeText(DutySectionNode section)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(section.Title))
                parts.Add(section.Title);

            parts.Add($"[{section.NodeType}]");

            if (!string.IsNullOrEmpty(section.DutyPositionTitle))
                parts.Add(section.DutyPositionTitle);

            if (section.Assignments.Any())
                parts.Add($"({section.Assignments.Count} осіб)");

            return string.Join(" ", parts);
        }

        private int GetIconIndex(NodeType nodeType)
        {
            // TODO: Додати ImageList з іконками
            return nodeType switch
            {
                NodeType.LocationSection => 1,
                NodeType.SimplePosition => 2,
                NodeType.GroupInline => 3,
                NodeType.GroupNested => 4,
                _ => 0
            };
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is DutySectionNode section)
            {
                _selectedNode = section;
                LoadSectionDetails(section);
            }
            else
            {
                _selectedNode = null;
                propertyGrid1.SelectedObject = null;
                dataGridView1.DataSource = null;
            }
        }

        private void LoadSectionDetails(DutySectionNode section)
        {
            // PropertyGrid для редагування властивостей вузла
            propertyGrid1.SelectedObject = section;

            // DataGridView для редагування призначень
            dataGridView1.DataSource = null; // Скидаємо

            if (section.Assignments.Any())
            {
                var assignments = section.Assignments.Select(a => new
                {
                    a.DutyAssignmentId,
                    ПІБ = $"{a.Person.Rank.RankName} {a.Person.LastName} {a.Person.Initials}",
                    Посада = a.Person.Position.PositionName,
                    Зброя = a.Weapon != null ? $"{a.Weapon.WeaponType} №{a.Weapon.WeaponNumber}" : "-",
                    Набої = a.AmmoCount.HasValue ? $"{a.AmmoType} – {a.AmmoCount} шт." : "-",
                    Транспорт = a.Vehicle != null ? $"{a.Vehicle.VehicleName} {a.Vehicle.VehicleNumber}" : "-"
                }).ToList();

                dataGridView1.DataSource = assignments;
            }
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // Автозбереження при зміні властивостей
            try
            {
                _context.SaveChanges();
                RefreshTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Toolbar Actions

        private void btnAddNode_Click(object sender, EventArgs e)
        {
            AddSectionNode();
        }

        private void btnAddChild_Click(object sender, EventArgs e)
        {
            AddChildNode();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteNode();
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            MoveNodeUp();
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            MoveNodeDown();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshTree();
        }

        #endregion

        #region Node Operations

        private void AddSectionNode()
        {
            if (_currentOrder == null) return;

            using var dialog = new NodeTypeDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var maxOrder = _currentOrder.Sections
                    .Where(s => s.ParentDutySectionNodeId == null)
                    .Select(s => (int?)s.OrderIndex)
                    .Max() ?? 0;

                var newNode = new DutySectionNode
                {
                    NodeType = dialog.SelectedNodeType,
                    DutyOrderId = _currentOrder.DutyOrderId,
                    OrderIndex = maxOrder + 1,
                    DutyPositionTitle = $"Нова секція {dialog.SelectedNodeType}"
                };

                _context.DutySectionNodes.Add(newNode);
                _context.SaveChanges();

                RefreshTree();
            }
        }

        private void AddChildNode()
        {
            if (_selectedNode == null)
            {
                MessageBox.Show("Виберіть батьківський вузол!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dialog = new NodeTypeDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var maxOrder = _selectedNode.Children
                    .Select(c => (int?)c.OrderIndex)
                    .Max() ?? 0;

                var newNode = new DutySectionNode
                {
                    NodeType = dialog.SelectedNodeType,
                    ParentDutySectionNodeId = _selectedNode.DutySectionNodeId,
                    DutyOrderId = _selectedNode.DutyOrderId,
                    OrderIndex = maxOrder + 1,
                    DutyPositionTitle = $"Новий вузол {dialog.SelectedNodeType}"
                };

                _context.DutySectionNodes.Add(newNode);
                _context.SaveChanges();

                RefreshTree();
            }
        }

        private void AddSiblingNode()
        {
            if (_selectedNode == null)
            {
                AddSectionNode();
                return;
            }

            using var dialog = new NodeTypeDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var newNode = new DutySectionNode
                {
                    NodeType = dialog.SelectedNodeType,
                    ParentDutySectionNodeId = _selectedNode.ParentDutySectionNodeId,
                    DutyOrderId = _selectedNode.DutyOrderId,
                    OrderIndex = _selectedNode.OrderIndex + 1,
                    DutyPositionTitle = $"Новий вузол {dialog.SelectedNodeType}"
                };

                // Зсуваємо OrderIndex для наступних siblings
                var siblings = _context.DutySectionNodes
                    .Where(s => s.ParentDutySectionNodeId == _selectedNode.ParentDutySectionNodeId
                             && s.OrderIndex > _selectedNode.OrderIndex)
                    .ToList();

                foreach (var sibling in siblings)
                {
                    sibling.OrderIndex++;
                }

                _context.DutySectionNodes.Add(newNode);
                _context.SaveChanges();

                RefreshTree();
            }
        }

        private void DeleteNode()
        {
            if (_selectedNode == null) return;

            var result = MessageBox.Show(
                $"Видалити вузол '{_selectedNode.DutyPositionTitle}'?\n\nУвага: всі дочірні вузли також будуть видалені!",
                "Підтвердження",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                DeleteNodeRecursive(_selectedNode);
                _context.SaveChanges();
                RefreshTree();
            }
        }

        private void DeleteNodeRecursive(DutySectionNode node)
        {
            // Видаляємо всіх дітей рекурсивно
            foreach (var child in node.Children.ToList())
            {
                DeleteNodeRecursive(child);
            }

            _context.DutySectionNodes.Remove(node);
        }

        private void MoveNodeUp()
        {
            if (_selectedNode == null || _selectedNode.OrderIndex <= 1) return;

            var siblings = _context.DutySectionNodes
                .Where(s => s.ParentDutySectionNodeId == _selectedNode.ParentDutySectionNodeId
                         && s.DutyOrderId == _selectedNode.DutyOrderId)
                .OrderBy(s => s.OrderIndex)
                .ToList();

            var currentIndex = siblings.IndexOf(_selectedNode);
            if (currentIndex > 0)
            {
                var prevNode = siblings[currentIndex - 1];

                // Міняємо місцями OrderIndex
                var temp = _selectedNode.OrderIndex;
                _selectedNode.OrderIndex = prevNode.OrderIndex;
                prevNode.OrderIndex = temp;

                _context.SaveChanges();
                RefreshTree();
            }
        }

        private void MoveNodeDown()
        {
            if (_selectedNode == null) return;

            var siblings = _context.DutySectionNodes
                .Where(s => s.ParentDutySectionNodeId == _selectedNode.ParentDutySectionNodeId
                         && s.DutyOrderId == _selectedNode.DutyOrderId)
                .OrderBy(s => s.OrderIndex)
                .ToList();

            var currentIndex = siblings.IndexOf(_selectedNode);
            if (currentIndex < siblings.Count - 1)
            {
                var nextNode = siblings[currentIndex + 1];

                // Міняємо місцями OrderIndex
                var temp = _selectedNode.OrderIndex;
                _selectedNode.OrderIndex = nextNode.OrderIndex;
                nextNode.OrderIndex = temp;

                _context.SaveChanges();
                RefreshTree();
            }
        }

        private void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Зміни збережено успішно!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Drag & Drop

        private void TreeView1_ItemDrag(object? sender, ItemDragEventArgs e)
        {
            if (e.Item is TreeNode node && node.Tag is DutySectionNode)
            {
                treeView1.DoDragDrop(node, DragDropEffects.Move);
            }
        }

        private void TreeView1_DragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void TreeView1_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(typeof(TreeNode)) is not TreeNode draggedNode) return;
            if (draggedNode.Tag is not DutySectionNode draggedSection) return;

            var targetPoint = treeView1.PointToClient(new System.Drawing.Point(e.X, e.Y));
            var targetNode = treeView1.GetNodeAt(targetPoint);

            if (targetNode?.Tag is not DutySectionNode targetSection) return;
            if (draggedSection.DutySectionNodeId == targetSection.DutySectionNodeId) return;

            // Переміщуємо вузол
            draggedSection.ParentDutySectionNodeId = targetSection.DutySectionNodeId;
            draggedSection.OrderIndex = targetSection.Children.Count + 1;

            _context.SaveChanges();
            RefreshTree();
        }

        #endregion

        #region Assignments

        private void btnAddAssignment_Click(object sender, EventArgs e)
        {
            if (_selectedNode == null)
            {
                MessageBox.Show("Виберіть вузол для додавання призначення!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dialog = new AssignmentDialog(_context);
            if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedPerson != null)
            {
                var assignment = new DutyAssignment
                {
                    DutySectionNodeId = _selectedNode.DutySectionNodeId,
                    PersonId = dialog.SelectedPerson.PersonId,
                    WeaponId = dialog.SelectedWeapon?.WeaponId,
                    VehicleId = dialog.SelectedVehicle?.VehicleId,
                    AmmoCount = dialog.AmmoCount,
                    AmmoType = dialog.AmmoType
                };

                _context.DutyAssignments.Add(assignment);
                _context.SaveChanges();

                LoadSectionDetails(_selectedNode);
                RefreshTree();
            }
        }

        private void btnRemoveAssignment_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            var assignmentId = (int)dataGridView1.SelectedRows[0].Cells["DutyAssignmentId"].Value;
            var assignment = _context.DutyAssignments.Find(assignmentId);

            if (assignment != null)
            {
                var result = MessageBox.Show(
                    "Видалити призначення?",
                    "Підтвердження",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    _context.DutyAssignments.Remove(assignment);
                    _context.SaveChanges();

                    if (_selectedNode != null)
                        LoadSectionDetails(_selectedNode);

                    RefreshTree();
                }
            }
        }

        #endregion

        #region Auto-Generate Titles

        private void GenerateTitles()
        {
            if (_currentOrder == null) return;

            var rootSections = _currentOrder.Sections
                .Where(s => s.ParentDutySectionNodeId == null)
                .OrderBy(s => s.OrderIndex)
                .ToList();

            for (int i = 0; i < rootSections.Count; i++)
            {
                AssignTitle(rootSections[i], $"{i + 1}");
            }

            _context.SaveChanges();
        }

        private void AssignTitle(DutySectionNode node, string prefix)
        {
            node.Title = prefix;

            var children = node.Children.OrderBy(c => c.OrderIndex).ToList();
            for (int i = 0; i < children.Count; i++)
            {
                AssignTitle(children[i], $"{prefix}.{i + 1}");
            }
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _context?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
