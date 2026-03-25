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
/// Редактор структури шаблону добового наряду.
/// Ліва частина — дерево вузлів, права — панель редагування обраного вузла,
/// специфічна для його NodeType.
/// </summary>
public partial class TemplateEditorForm : Form
{
    private AppDbContext _context;
    private DutyTemplate? _template;
    private DutySectionNode? _selectedNode;

    // ── Контроли панелі (створюються динамічно) ──
    private TextBox? _txtTitle;
    private TextBox? _txtGroupItem;
    private CheckBox? _chkWeapon;
    private CheckBox? _chkAmmo;
    private CheckBox? _chkVehicle;
    private NumericUpDown? _nudMax;
    private TextBox? _txtTimeLabel;
    private Label? _lblNodeType;
    private TextBox? _lastPlaceholderTarget;

    public TemplateEditorForm()
    {
        InitializeComponent();
        _context = AppServices.DbContext;
    }

    public TemplateEditorForm(int templateId) : this()
    {
        LoadTemplate(templateId);
    }

    private void TemplateEditorForm_Load(object sender, EventArgs e)
    {
        // Drag & Drop
        treeView1.AllowDrop = true;
        treeView1.ItemDrag += TreeView1_ItemDrag;
        treeView1.DragEnter += TreeView1_DragEnter;
        treeView1.DragOver += TreeView1_DragOver;
        treeView1.DragDrop += TreeView1_DragDrop;

        // Контекстне меню
        var ctx = new ContextMenuStrip();
        ctx.Items.Add("⬇ Додати дочірній", null, (_, _) => AddChildNode());
        ctx.Items.Add("↔ Додати сусідній", null, (_, _) => AddSiblingNode());
        ctx.Items.Add("-");
        ctx.Items.Add("🗑️ Видалити", null, (_, _) => DeleteNode());
        ctx.Items.Add("-");
        ctx.Items.Add("▲ Вгору", null, (_, _) => MoveNodeUp());
        ctx.Items.Add("▼ Вниз", null, (_, _) => MoveNodeDown());
        treeView1.ContextMenuStrip = ctx;
    }

    // ═════════════════════════════════════════════════════
    //  TEMPLATE LOAD / TREE
    // ═════════════════════════════════════════════════════

    private void LoadTemplate(int templateId)
    {
        _template = _context.DutyTemplates
            .Include(t => t.Sections)
                .ThenInclude(s => s.Children)
            .Include(t => t.Sections)
                .ThenInclude(s => s.Location)
            .FirstOrDefault(t => t.DutyTemplateId == templateId);

        if (_template == null)
        {
            MessageBox.Show("Шаблон не знайдено!", "Помилка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
            return;
        }

        Text = $"Редактор шаблону — {_template.TemplateName} (v{_template.Version})";
        RefreshTree();
    }

    private void RefreshTree()
    {
        if (_template == null) return;

        treeView1.BeginUpdate();
        treeView1.Nodes.Clear();

        var root = new TreeNode($"📋 {_template.TemplateName} (v{_template.Version})")
        {
            Tag = _template,
            NodeFont = new Font(treeView1.Font, FontStyle.Bold)
        };
        treeView1.Nodes.Add(root);

        var topLevel = _template.Sections
            .Where(s => s.ParentDutySectionNodeId == null)
            .OrderBy(s => s.OrderIndex);

        foreach (var section in topLevel)
            root.Nodes.Add(BuildTreeNode(section));

        root.ExpandAll();
        treeView1.EndUpdate();

        GenerateTitles();
    }

    private TreeNode BuildTreeNode(DutySectionNode section)
    {
        var tn = new TreeNode(FormatNodeText(section))
        {
            Tag = section,
            ForeColor = GetNodeColor(section.NodeType)
        };

        foreach (var child in section.Children.OrderBy(c => c.OrderIndex))
            tn.Nodes.Add(BuildTreeNode(child));

        return tn;
    }

    private static string FormatNodeText(DutySectionNode s)
    {
        var icon = s.NodeType switch
        {
            NodeType.SectionHeader => s.LocationId != null ? "📍" : "📑",
            NodeType.SimplePosition or NodeType.MedicalPosition => "👤",
            NodeType.DriverPosition => "🚗",
            NodeType.GroupInline or NodeType.GroupNested => "👥",
            NodeType.TimeRange => "🕐",
            NodeType.FireGroupSection or NodeType.FireGroupLocation => "🔥",
            NodeType.FireGroupInline => "🔥",
            _ => "•"
        };

        var title = !string.IsNullOrEmpty(s.Title) ? $"{s.Title}. " : "";
        var text = s.DutyPositionTitle ?? s.NodeType.ToString();

        // Показуємо назву локації якщо прив'язано
        var locInfo = s.Location != null ? $" [{s.Location.LocationName}]" : "";

        var flags = "";
        if (s.HasWeapon) flags += "🔫";
        if (s.HasAmmo) flags += "🎯";
        if (s.HasVehicle) flags += "🚗";
        if (s.MaxAssignments == 0) flags += " (∞)";
        else if (s.MaxAssignments > 1) flags += $" (max:{s.MaxAssignments})";

        return $"{icon} {title}{text}{locInfo} {flags}".Trim();
    }

    private static Color GetNodeColor(NodeType t) => t switch
    {
        NodeType.SectionHeader or NodeType.FireGroupSection => Color.DarkBlue,
        NodeType.FireGroupLocation => Color.Teal,
        NodeType.TimeRange => Color.Purple,
        NodeType.DriverPosition => Color.SaddleBrown,
        NodeType.GroupInline or NodeType.GroupNested => Color.DarkGreen,
        _ => Color.Black
    };

    // ═════════════════════════════════════════════════════
    //  NODE SELECTION → DYNAMIC PANEL
    // ═════════════════════════════════════════════════════

    private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is DutySectionNode section)
        {
            _selectedNode = section;
            ShowEditorPanel(section);
            labelHint.Visible = false;
            panelRight.Visible = true;
        }
        else
        {
            _selectedNode = null;
            panelRight.Visible = false;
            labelHint.Visible = true;
        }

        UpdateToolbarState();
    }

    private void UpdateToolbarState()
    {
        var hasNode = _selectedNode != null;
        btnDelete.Enabled = hasNode;
        btnMoveUp.Enabled = hasNode;
        btnMoveDown.Enabled = hasNode;
        btnAddChild.Enabled = hasNode;
        btnAddSibling.Enabled = hasNode;
    }

    /// <summary>
    /// Будує панель редагування, специфічну для типу вузла.
    /// </summary>
    private void ShowEditorPanel(DutySectionNode node)
    {
        panelRight.SuspendLayout();
        panelRight.Controls.Clear();
        _txtTitle = null;
        _chkWeapon = null;
        _chkAmmo = null;
        _chkVehicle = null;
        _nudMax = null;
        _txtTimeLabel = null;
        _txtGroupItem = null;
        _lastPlaceholderTarget = null;

        int y = 0;

        // ── Заголовок: тип вузла ──
        _lblNodeType = AddLabel($"Тип: {GetNodeTypeLabel(node.NodeType)}", ref y, bold: true, color: GetNodeColor(node.NodeType));
        y += 8;

        // ── Загальне для всіх: текстовий шаблон ──
        AddLabel(GetTitleFieldLabel(node.NodeType), ref y);
        _txtTitle = AddTextBox(node.DutyPositionTitle ?? "", ref y, multiline: IsMultilineTitle(node.NodeType));
        _txtTitle.Tag = node;
        _txtTitle.Leave += TxtTitle_Leave;
        _lastPlaceholderTarget = _txtTitle;
        y += 8;

        // ── Специфічні поля за типом ──
        switch (node.NodeType)
        {
            case NodeType.SimplePosition:
            case NodeType.MedicalPosition:
                AddSeparator(ref y);
                AddLabel("Екіпірування позиції", ref y, bold: true);
                _chkWeapon = AddCheckBox("Зброя", node.HasWeapon, ref y);
                _chkAmmo = AddCheckBox("Набої", node.HasAmmo, ref y);
                _chkWeapon.CheckedChanged += Flag_Changed;
                _chkAmmo.CheckedChanged += Flag_Changed;
                break;

            case NodeType.DriverPosition:
                AddSeparator(ref y);
                AddLabel("Екіпірування позиції", ref y, bold: true);
                _chkVehicle = AddCheckBox("Транспорт", node.HasVehicle, ref y);
                _chkVehicle.Enabled = false; // Завжди true для водія
                _chkWeapon = AddCheckBox("Зброя", node.HasWeapon, ref y);
                _chkAmmo = AddCheckBox("Набої", node.HasAmmo, ref y);
                _chkVehicle.CheckedChanged += Flag_Changed;
                _chkWeapon.CheckedChanged += Flag_Changed;
                _chkAmmo.CheckedChanged += Flag_Changed;
                break;

            case NodeType.GroupInline:
            case NodeType.GroupNested:
            case NodeType.FireGroupInline:
                AddSeparator(ref y);
                AddLabel("Параметри групи", ref y, bold: true);
                AddLabel("Шаблон рядка учасника групи:", ref y);
                _txtGroupItem = AddTextBox(node.GroupItemTemplate ?? "", ref y);
                _txtGroupItem.Tag = node;
                _txtGroupItem.Leave += TxtGroupItem_Leave;

                _chkWeapon = AddCheckBox("Зброя", node.HasWeapon, ref y);
                _chkAmmo = AddCheckBox("Набої", node.HasAmmo, ref y);
                _chkWeapon.CheckedChanged += Flag_Changed;
                _chkAmmo.CheckedChanged += Flag_Changed;
                y += 4;
                AddLabel("Макс. осіб (0 = необмежено):", ref y);
                _nudMax = AddNumericUpDown(node.MaxAssignments, 0, 100, ref y);
                _nudMax.ValueChanged += NudMax_Changed;
                break;

            case NodeType.TimeRange:
                AddSeparator(ref y);
                AddLabel("Мітка зміни (для панелі наказу)", ref y, bold: true);
                _txtTimeLabel = AddTextBox(node.TimeRangeLabel ?? "", ref y);
                _txtTimeLabel.Tag = node;
                _txtTimeLabel.Leave += TxtTimeLabel_Leave;
                break;

            case NodeType.SectionHeader:
                AddSeparator(ref y);
                AddLabel("Прив'язка до локації (для фільтрації зброї)", ref y, bold: true);
                AddLocationPicker(node, ref y);
                break;

            case NodeType.FireGroupSection:
            case NodeType.FireGroupLocation:
                // Тільки текстовий шаблон — вже додано вище
                break;
        }

        // ── Підказки-плейсхолдери (кліком вставляються у текст) ──
        var placeholders = GetPlaceholdersForNodeType(node.NodeType);
        if (placeholders.Count > 0)
        {
            y += 12;
            AddSeparator(ref y);
            AddLabel("Натисніть на плейсхолдер — він вставиться в текст:", ref y, bold: true);
            y += 4;

            foreach (var (placeholder, description) in placeholders)
            {
                AddPlaceholderLink(placeholder, description, ref y);
            }
        }

        panelRight.ResumeLayout(true);
    }

    /// <summary>
    /// Повертає список плейсхолдерів з описами для конкретного типу вузла.
    /// </summary>
    private static List<(string placeholder, string description)> GetPlaceholdersForNodeType(NodeType nodeType)
    {
        var list = new List<(string, string)>();

        // ══════════ Дані наказу — доступні для ВСІХ типів вузлів ══════════
        list.Add(("{OrderPeriod}", "з 19 по 20 лютого 2026 року"));
        list.Add(("{CommanderInfo}", "Інформація про командира (ПІБ, звання, посада)"));

        // ══════════ Особові дані — для позицій та груп ══════════
        if (nodeType is NodeType.SimplePosition or NodeType.MedicalPosition
            or NodeType.DriverPosition or NodeType.GroupInline
            or NodeType.GroupNested or NodeType.FireGroupInline)
        {
            list.Add(("{Rank}", "Військове звання (солдат, сержант, лейтенант…)"));
            list.Add(("{LastName}", "Прізвище (Петренко, Іваненко…)"));
            list.Add(("{Initials}", "Ініціали (І.П.)"));
            list.Add(("{FullName}", "Прізвище + ініціали (Петренко І.П.)"));
            list.Add(("{Position}", "Штатна посада особи"));
        }

        // ══════════ Зброя та набої ══════════
        if (nodeType is NodeType.SimplePosition or NodeType.MedicalPosition
            or NodeType.DriverPosition or NodeType.GroupInline
            or NodeType.GroupNested or NodeType.FireGroupInline)
        {
            list.Add(("{WeaponType}", "Тип зброї (АК-74, ПМ…)"));
            list.Add(("{WeaponNumber}", "Номер зброї (АВ-1234)"));
            list.Add(("{AmmoType}", "Тип набоїв (5,45 мм, 9 мм)"));
            list.Add(("{AmmoCount}", "Кількість набоїв (120, 16)"));
        }

        // ══════════ Транспорт — для водіїв ══════════
        if (nodeType is NodeType.DriverPosition)
        {
            list.Add(("{VehicleName}", "Марка/назва транспорту (УАЗ-469, КамАЗ)"));
            list.Add(("{VehicleNumber}", "Номерний знак (АА1234ВВ)"));
        }

        // ══════════ Часові діапазони — для TimeRange ══════════
        if (nodeType is NodeType.TimeRange)
        {
            list.Add(("{TimeLabel}", "Мітка зміни (Зміна 1, Нічна зміна)"));
            list.Add(("{StartTime}", "Час початку зміни (08:00)"));
            list.Add(("{EndTime}", "Час закінчення зміни (20:00)"));
            list.Add(("{StartDate}", "Дата початку зміни (19.01.2026)"));
            list.Add(("{EndDate}", "Дата закінчення зміни (20.01.2026)"));
        }

        return list;
    }

    /// <summary>
    /// Додає клікабельний рядок-плейсхолдер з описом.
    /// При натисканні вставляє плейсхолдер у _txtTitle на позицію курсора.
    /// </summary>
    private void AddPlaceholderLink(string placeholder, string description, ref int y)
    {
        var link = new LinkLabel
        {
            Text = $"{placeholder}  —  {description}",
            AutoSize = true,
            Location = new Point(4, y),
            Font = new Font("Segoe UI", 9.5F),
            LinkArea = new LinkArea(0, placeholder.Length),
            Tag = placeholder,
            Padding = new Padding(0, 2, 0, 2),
            MaximumSize = new Size(panelRight.ClientSize.Width - 40, 0)
        };

        link.LinkClicked += PlaceholderLink_Clicked;
        panelRight.Controls.Add(link);
        y += link.PreferredHeight + 2;
    }

    private void PlaceholderLink_Clicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        if (sender is not LinkLabel link) return;
        if (link.Tag is not string placeholder) return;
        if (_txtTitle == null && _lastPlaceholderTarget == null) return;

        var target = _lastPlaceholderTarget ?? _txtTitle!;
        var pos = target.SelectionStart;
        target.Text = target.Text.Insert(pos, placeholder);
        target.SelectionStart = pos + placeholder.Length;
        target.Focus();
    }

    /// <summary>
    /// Додає ComboBox для вибору локації (опціонально).
    /// Перший елемент — "(без локації)".
    /// </summary>
    private void AddLocationPicker(DutySectionNode node, ref int y)
    {
        var locations = _context.Locations.OrderBy(l => l.LocationName).ToList();

        var cmb = new ComboBox
        {
            Location = new Point(0, y),
            Width = Math.Max(panelRight.ClientSize.Width - 40, 300),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Font = new Font("Segoe UI", 10F),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Tag = node
        };

        cmb.Items.Add("(без локації)");
        foreach (var loc in locations)
            cmb.Items.Add(loc);

        cmb.DisplayMember = "LocationName";

        // Встановлюємо поточне значення
        if (node.LocationId != null)
        {
            var current = locations.FirstOrDefault(l => l.LocationId == node.LocationId);
            if (current != null)
                cmb.SelectedItem = current;
            else
                cmb.SelectedIndex = 0;
        }
        else
        {
            cmb.SelectedIndex = 0;
        }

        cmb.SelectedIndexChanged += LocationPicker_Changed;
        panelRight.Controls.Add(cmb);
        y += 34;

        // Підказка
        var hint = new Label
        {
            Text = "Локація визначає, з якого містечка фільтрувати зброю при призначенні",
            AutoSize = true,
            Location = new Point(0, y),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
            ForeColor = Color.Gray,
            MaximumSize = new Size(panelRight.ClientSize.Width - 40, 0)
        };
        panelRight.Controls.Add(hint);
        y += hint.PreferredHeight + 4;
    }

    private void LocationPicker_Changed(object? sender, EventArgs e)
    {
        if (sender is not ComboBox cmb) return;
        if (_selectedNode == null) return;

        if (cmb.SelectedItem is Location loc)
        {
            _selectedNode.LocationId = loc.LocationId;
            _selectedNode.Location = loc;
        }
        else
        {
            _selectedNode.LocationId = null;
            _selectedNode.Location = null;
        }

        SaveQuiet();
        RefreshSelectedTreeNode();
    }

    // ═════════════════════════════════════════════════════
    //  PANEL CONTROL FACTORY
    // ═════════════════════════════════════════════════════

    private Label AddLabel(string text, ref int y, bool bold = false, Color? color = null)
    {
        var lbl = new Label
        {
            Text = text,
            AutoSize = true,
            Location = new Point(0, y),
            Font = bold ? new Font("Segoe UI", 10F, FontStyle.Bold) : new Font("Segoe UI", 9.5F),
            ForeColor = color ?? Color.Black,
            MaximumSize = new Size(panelRight.ClientSize.Width - 40, 0)
        };
        panelRight.Controls.Add(lbl);
        y += lbl.PreferredHeight + 4;
        return lbl;
    }

    private TextBox AddTextBox(string value, ref int y, bool multiline = false)
    {
        var txt = new TextBox
        {
            Text = value,
            Location = new Point(0, y),
            Width = Math.Max(panelRight.ClientSize.Width - 40, 300),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Font = new Font("Consolas", 10F),
            Multiline = multiline,
            Height = multiline ? 60 : 27,
            ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None
        };
        txt.Enter += (_, _) => _lastPlaceholderTarget = txt;
        panelRight.Controls.Add(txt);
        y += txt.Height + 6;
        return txt;
    }

    private CheckBox AddCheckBox(string text, bool isChecked, ref int y)
    {
        var chk = new CheckBox
        {
            Text = text,
            Checked = isChecked,
            AutoSize = true,
            Location = new Point(0, y),
            Font = new Font("Segoe UI", 10F)
        };
        panelRight.Controls.Add(chk);
        y += 28;
        return chk;
    }

    private NumericUpDown AddNumericUpDown(int value, int min, int max, ref int y)
    {
        var nud = new NumericUpDown
        {
            Value = Math.Clamp(value, min, max),
            Minimum = min,
            Maximum = max,
            Location = new Point(0, y),
            Width = 100,
            Font = new Font("Segoe UI", 10F)
        };
        panelRight.Controls.Add(nud);
        y += 32;
        return nud;
    }

    private void AddSeparator(ref int y)
    {
        y += 4;
        var line = new Label
        {
            BorderStyle = BorderStyle.Fixed3D,
            Height = 2,
            Width = Math.Max(panelRight.ClientSize.Width - 40, 300),
            Location = new Point(0, y),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        panelRight.Controls.Add(line);
        y += 10;
    }

    // ═════════════════════════════════════════════════════
    //  HELPERS: NODE TYPE LABELS & LOGIC
    // ═════════════════════════════════════════════════════

    private static string GetNodeTypeLabel(NodeType t) => t switch
    {
        NodeType.SectionHeader => "Секція (заголовок / локація)",
        NodeType.SimplePosition => "Проста позиція (1 особа)",
        NodeType.DriverPosition => "Водій з транспортом",
        NodeType.GroupInline => "Група (рядком)",
        NodeType.GroupNested => "Група (з підпунктами)",
        NodeType.TimeRange => "Часовий діапазон (зміна)",
        NodeType.MedicalPosition => "Черговий медпункту",
        NodeType.FireGroupSection => "Заголовок вогневих груп",
        NodeType.FireGroupLocation => "Вогнева група (локація)",
        NodeType.FireGroupInline => "Вогнева група (рядком)",
        _ => t.ToString()
    };

    private static string GetTitleFieldLabel(NodeType t) => t switch
    {
        NodeType.SectionHeader => "Заголовок секції:",
        NodeType.TimeRange => "Текстовий шаблон зміни (з плейсхолдерами):",
        NodeType.FireGroupSection => "Заголовок:",
        NodeType.FireGroupLocation => "Назва локації:",
        _ => "Текстовий шаблон (з плейсхолдерами):"
    };

    private static bool IsMultilineTitle(NodeType t) =>
        t is NodeType.SectionHeader or NodeType.FireGroupSection or NodeType.FireGroupLocation;

    // ═════════════════════════════════════════════════════
    //  EVENT HANDLERS: PANEL → MODEL
    // ═════════════════════════════════════════════════════

    private void TxtTitle_Leave(object? sender, EventArgs e)
    {
        if (_selectedNode == null || _txtTitle == null) return;
        if (_selectedNode.DutyPositionTitle == _txtTitle.Text) return;

        _selectedNode.DutyPositionTitle = _txtTitle.Text;
        SaveQuiet();
        RefreshSelectedTreeNode();
    }

    private void TxtTimeLabel_Leave(object? sender, EventArgs e)
    {
        if (_selectedNode == null || _txtTimeLabel == null) return;
        if (_selectedNode.TimeRangeLabel == _txtTimeLabel.Text) return;

        _selectedNode.TimeRangeLabel = _txtTimeLabel.Text;
        SaveQuiet();
    }

    private void TxtGroupItem_Leave(object? sender, EventArgs e)
    {
        if (_selectedNode == null || _txtGroupItem == null) return;
        if (_selectedNode.GroupItemTemplate == _txtGroupItem.Text) return;

        _selectedNode.GroupItemTemplate = _txtGroupItem.Text;
        SaveQuiet();
        RefreshSelectedTreeNode();
    }

    private void Flag_Changed(object? sender, EventArgs e)
    {
        if (_selectedNode == null) return;

        if (_chkWeapon != null) _selectedNode.HasWeapon = _chkWeapon.Checked;
        if (_chkAmmo != null) _selectedNode.HasAmmo = _chkAmmo.Checked;
        if (_chkVehicle != null) _selectedNode.HasVehicle = _chkVehicle.Checked;

        SaveQuiet();
        RefreshSelectedTreeNode();
    }

    private void NudMax_Changed(object? sender, EventArgs e)
    {
        if (_selectedNode == null || _nudMax == null) return;

        _selectedNode.MaxAssignments = (int)_nudMax.Value;
        SaveQuiet();
        RefreshSelectedTreeNode();
    }

    private void RefreshSelectedTreeNode()
    {
        if (treeView1.SelectedNode?.Tag is DutySectionNode s)
            treeView1.SelectedNode.Text = FormatNodeText(s);
    }

    /// <summary>
    /// Тихе збереження без повідомлень і без перебудови дерева.
    /// </summary>
    private void SaveQuiet()
    {
        try
        {
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Збереження з інкрементом версії та перебудовою дерева.
    /// </summary>
    private void SaveAndRefresh(string changeDescription)
    {
        try
        {
            if (_template != null)
            {
                var service = new OrderService(_context);
                service.IncrementTemplateVersion(_template, changeDescription);
            }
            else
            {
                _context.SaveChanges();
            }

            RefreshTree();
            Text = $"Редактор шаблону — {_template?.TemplateName} (v{_template?.Version})";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ═════════════════════════════════════════════════════
    //  TOOLBAR
    // ═════════════════════════════════════════════════════

    private void btnAddNode_Click(object sender, EventArgs e) => AddSectionNode();
    private void btnAddChild_Click(object sender, EventArgs e) => AddChildNode();
    private void btnAddSibling_Click(object sender, EventArgs e) => AddSiblingNode();
    private void btnDelete_Click(object sender, EventArgs e) => DeleteNode();
    private void btnMoveUp_Click(object sender, EventArgs e) => MoveNodeUp();
    private void btnMoveDown_Click(object sender, EventArgs e) => MoveNodeDown();

    private void btnSave_Click(object sender, EventArgs e)
    {
        SaveAndRefresh("Ручне збереження");
        MessageBox.Show("Збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // ═════════════════════════════════════════════════════
    //  NODE OPERATIONS
    // ═════════════════════════════════════════════════════

    private void AddSectionNode()
    {
        if (_template == null) return;

        using var dlg = new NodeTypeDialog();
        if (dlg.ShowDialog() != DialogResult.OK) return;

        var maxOrder = _template.Sections
            .Where(s => s.ParentDutySectionNodeId == null)
            .Select(s => (int?)s.OrderIndex)
            .Max() ?? 0;

        var node = new DutySectionNode
        {
            NodeType = dlg.SelectedNodeType,
            DutyTemplateId = _template.DutyTemplateId,
            OrderIndex = maxOrder + 1,
            DutyPositionTitle = GetDefaultTitle(dlg.SelectedNodeType),
            GroupItemTemplate = IsGroupType(dlg.SelectedNodeType) ? "{Rank} {LastName} {Initials}" : null,
            HasVehicle = dlg.SelectedNodeType == NodeType.DriverPosition,
            MaxAssignments = IsGroupType(dlg.SelectedNodeType) ? 0 : 1
        };

        _context.DutySectionNodes.Add(node);
        SaveAndRefresh("Додано секцію");
    }

    private void AddChildNode()
    {
        if (_selectedNode == null)
        {
            MessageBox.Show("Виберіть батьківський вузол!", "Увага",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dlg = new NodeTypeDialog();
        if (dlg.ShowDialog() != DialogResult.OK) return;

        var maxOrder = _selectedNode.Children
            .Select(c => (int?)c.OrderIndex)
            .Max() ?? 0;

        var node = new DutySectionNode
        {
            NodeType = dlg.SelectedNodeType,
            ParentDutySectionNodeId = _selectedNode.DutySectionNodeId,
            DutyTemplateId = _selectedNode.DutyTemplateId,
            OrderIndex = maxOrder + 1,
            DutyPositionTitle = GetDefaultTitle(dlg.SelectedNodeType),
            GroupItemTemplate = IsGroupType(dlg.SelectedNodeType) ? "{Rank} {LastName} {Initials}" : null,
            HasVehicle = dlg.SelectedNodeType == NodeType.DriverPosition,
            MaxAssignments = IsGroupType(dlg.SelectedNodeType) ? 0 : 1
        };

        _context.DutySectionNodes.Add(node);
        SaveAndRefresh("Додано дочірній вузол");
    }

    private void AddSiblingNode()
    {
        if (_selectedNode == null) { AddSectionNode(); return; }

        using var dlg = new NodeTypeDialog();
        if (dlg.ShowDialog() != DialogResult.OK) return;

        var node = new DutySectionNode
        {
            NodeType = dlg.SelectedNodeType,
            ParentDutySectionNodeId = _selectedNode.ParentDutySectionNodeId,
            DutyTemplateId = _selectedNode.DutyTemplateId,
            OrderIndex = _selectedNode.OrderIndex + 1,
            DutyPositionTitle = GetDefaultTitle(dlg.SelectedNodeType),
            GroupItemTemplate = IsGroupType(dlg.SelectedNodeType) ? "{Rank} {LastName} {Initials}" : null,
            HasVehicle = dlg.SelectedNodeType == NodeType.DriverPosition,
            MaxAssignments = IsGroupType(dlg.SelectedNodeType) ? 0 : 1
        };

        // Зсуваємо наступних
        var later = _context.DutySectionNodes
            .Where(s => s.ParentDutySectionNodeId == _selectedNode.ParentDutySectionNodeId
                     && s.DutyTemplateId == _selectedNode.DutyTemplateId
                     && s.OrderIndex > _selectedNode.OrderIndex)
            .ToList();
        foreach (var s in later) s.OrderIndex++;

        _context.DutySectionNodes.Add(node);
        SaveAndRefresh("Додано сусідній вузол");
    }

    private void DeleteNode()
    {
        if (_selectedNode == null) return;

        var msg = $"Видалити «{_selectedNode.DutyPositionTitle}»?\n" +
                  "Всі дочірні вузли також будуть видалені!";
        if (MessageBox.Show(msg, "Підтвердження",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        DeleteRecursive(_selectedNode);
        _selectedNode = null;
        SaveAndRefresh("Видалено вузол");
    }

    private void DeleteRecursive(DutySectionNode node)
    {
        foreach (var child in node.Children.ToList())
            DeleteRecursive(child);
        _context.DutySectionNodes.Remove(node);
    }

    private void MoveNodeUp()
    {
        if (_selectedNode == null) return;

        var siblings = GetSiblings(_selectedNode);
        var idx = siblings.IndexOf(_selectedNode);
        if (idx <= 0) return;

        SwapOrder(_selectedNode, siblings[idx - 1]);
        SaveAndRefresh("Переміщено вгору");
    }

    private void MoveNodeDown()
    {
        if (_selectedNode == null) return;

        var siblings = GetSiblings(_selectedNode);
        var idx = siblings.IndexOf(_selectedNode);
        if (idx < 0 || idx >= siblings.Count - 1) return;

        SwapOrder(_selectedNode, siblings[idx + 1]);
        SaveAndRefresh("Переміщено вниз");
    }

    private List<DutySectionNode> GetSiblings(DutySectionNode node) =>
        _context.DutySectionNodes
            .Where(s => s.ParentDutySectionNodeId == node.ParentDutySectionNodeId
                     && s.DutyTemplateId == node.DutyTemplateId)
            .OrderBy(s => s.OrderIndex)
            .ToList();

    private static void SwapOrder(DutySectionNode a, DutySectionNode b)
    {
        (a.OrderIndex, b.OrderIndex) = (b.OrderIndex, a.OrderIndex);
    }

    private static string GetDefaultTitle(NodeType t) => t switch
    {
        NodeType.SectionHeader => "Новий розділ",
        NodeType.SimplePosition => "Черговий – {Rank} {LastName} {Initials}",
        NodeType.DriverPosition => "Водій – {Rank} {LastName} {Initials}",
        NodeType.GroupInline => "Наряд в складі:",
        NodeType.GroupNested => "Наряд в складі:",
        NodeType.TimeRange => "",
        NodeType.MedicalPosition => "Черговий медпункту – {Rank} {LastName} {Initials}",
        NodeType.FireGroupSection => "Вогневі групи",
        NodeType.FireGroupLocation => "Вогнева група",
        NodeType.FireGroupInline => "Вогнева група – {Rank} {LastName} {Initials}",
        _ => ""
    };

    private static bool IsGroupType(NodeType t) =>
        t is NodeType.GroupInline or NodeType.GroupNested or NodeType.FireGroupInline;

    // ═════════════════════════════════════════════════════
    //  DRAG & DROP
    // ═════════════════════════════════════════════════════

    private void TreeView1_ItemDrag(object? sender, ItemDragEventArgs e)
    {
        if (e.Item is TreeNode tn && tn.Tag is DutySectionNode)
            treeView1.DoDragDrop(tn, DragDropEffects.Move);
    }

    private void TreeView1_DragEnter(object? sender, DragEventArgs e) =>
        e.Effect = DragDropEffects.Move;

    private void TreeView1_DragOver(object? sender, DragEventArgs e)
    {
        var pt = treeView1.PointToClient(new Point(e.X, e.Y));
        var target = treeView1.GetNodeAt(pt);
        if (target != null)
            treeView1.SelectedNode = target;
    }

    private void TreeView1_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(typeof(TreeNode)) is not TreeNode draggedTn) return;
        if (draggedTn.Tag is not DutySectionNode dragged) return;

        var pt = treeView1.PointToClient(new Point(e.X, e.Y));
        var targetTn = treeView1.GetNodeAt(pt);
        if (targetTn?.Tag is not DutySectionNode target) return;
        if (dragged.DutySectionNodeId == target.DutySectionNodeId) return;

        dragged.ParentDutySectionNodeId = target.DutySectionNodeId;
        dragged.OrderIndex = target.Children.Count + 1;

        SaveAndRefresh("Drag & drop");
    }

    // ═════════════════════════════════════════════════════
    //  AUTO TITLES
    // ═════════════════════════════════════════════════════

    private void GenerateTitles()
    {
        if (_template == null) return;

        var roots = _template.Sections
            .Where(s => s.ParentDutySectionNodeId == null)
            .OrderBy(s => s.OrderIndex)
            .ToList();

        int counter = 0;
        foreach (var root in roots)
        {
            if (root.NodeType == NodeType.SectionHeader)
            {
                counter++;
                AssignTitle(root, $"{counter}");
            }
            else
            {
                root.Title = null;
            }
        }

        _context.SaveChanges();
    }

    private void AssignTitle(DutySectionNode node, string prefix)
    {
        node.Title = prefix;
        var children = node.Children.OrderBy(c => c.OrderIndex).ToList();
        for (int i = 0; i < children.Count; i++)
            AssignTitle(children[i], $"{prefix}.{i + 1}");
    }

    // ═════════════════════════════════════════════════════

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _context.ChangeTracker.Clear();
        base.OnFormClosing(e);
    }
}
