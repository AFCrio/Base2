namespace Base2.Forms
{
    partial class OrderCreatorForm
    {
        private System.ComponentModel.IContainer components = null;

        // ── Toolbar ──
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnAddAssignment;
        private System.Windows.Forms.ToolStripButton btnRemoveAssignment;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabelStatus;

        // ── Top panel: order metadata ──
        private System.Windows.Forms.Panel panelMeta;
        private System.Windows.Forms.Label lblTemplate;
        private System.Windows.Forms.Label lblTemplateValue;
        private System.Windows.Forms.Label lblOrderNumber;
        private System.Windows.Forms.TextBox txtOrderNumber;
        private System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.DateTimePicker dtpOrderDate;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.Label lblCommander;
        private System.Windows.Forms.TextBox txtCommander;

        // ── TimeRange panel ──
        private System.Windows.Forms.GroupBox grpTimeRanges;
        private System.Windows.Forms.FlowLayoutPanel flowTimeRanges;

        // ── Main area ──
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.SplitContainer splitRight;

        // ── Node info panel ──
        private System.Windows.Forms.GroupBox grpNodeInfo;
        private System.Windows.Forms.Label lblNodeTitle;
        private System.Windows.Forms.Label lblNodeType;
        private System.Windows.Forms.Label lblNodeFlags;
        private System.Windows.Forms.Label lblRenderedText;
        private System.Windows.Forms.TextBox txtRenderedText;

        // ── Assignments grid ──
        private System.Windows.Forms.Panel panelAssignmentHeader;
        private System.Windows.Forms.Label lblAssignments;
        private System.Windows.Forms.Label lblAssignmentLimit;
        private System.Windows.Forms.DataGridView dataGridView1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddAssignment = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveAssignment = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelStatus = new System.Windows.Forms.ToolStripLabel();

            this.panelMeta = new System.Windows.Forms.Panel();
            this.lblTemplate = new System.Windows.Forms.Label();
            this.lblTemplateValue = new System.Windows.Forms.Label();
            this.lblOrderNumber = new System.Windows.Forms.Label();
            this.txtOrderNumber = new System.Windows.Forms.TextBox();
            this.lblOrderDate = new System.Windows.Forms.Label();
            this.dtpOrderDate = new System.Windows.Forms.DateTimePicker();
            this.lblStart = new System.Windows.Forms.Label();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.lblEnd = new System.Windows.Forms.Label();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.lblCommander = new System.Windows.Forms.Label();
            this.txtCommander = new System.Windows.Forms.TextBox();

            this.grpTimeRanges = new System.Windows.Forms.GroupBox();
            this.flowTimeRanges = new System.Windows.Forms.FlowLayoutPanel();

            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitRight = new System.Windows.Forms.SplitContainer();

            this.grpNodeInfo = new System.Windows.Forms.GroupBox();
            this.lblNodeTitle = new System.Windows.Forms.Label();
            this.lblNodeType = new System.Windows.Forms.Label();
            this.lblNodeFlags = new System.Windows.Forms.Label();
            this.lblRenderedText = new System.Windows.Forms.Label();
            this.txtRenderedText = new System.Windows.Forms.TextBox();

            this.panelAssignmentHeader = new System.Windows.Forms.Panel();
            this.lblAssignments = new System.Windows.Forms.Label();
            this.lblAssignmentLimit = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();

            this.panelMeta.SuspendLayout();
            this.grpTimeRanges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).BeginInit();
            this.splitRight.Panel1.SuspendLayout();
            this.splitRight.Panel2.SuspendLayout();
            this.splitRight.SuspendLayout();
            this.grpNodeInfo.SuspendLayout();
            this.panelAssignmentHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();

            // ═══════════════════════════════════════
            // toolStrip1
            // ═══════════════════════════════════════
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnSave,
                this.btnRefresh,
                this.toolStripSeparator1,
                this.btnAddAssignment,
                this.btnRemoveAssignment,
                this.toolStripSeparator2,
                this.toolStripLabelStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1300, 25);
            this.toolStrip1.TabIndex = 0;

            this.btnSave.Text = "💾 Зберегти";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnRefresh.Text = "🔄 Оновити";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            this.btnAddAssignment.Text = "➕ Призначити особу";
            this.btnAddAssignment.Click += new System.EventHandler(this.btnAddAssignment_Click);

            this.btnRemoveAssignment.Text = "➖ Зняти призначення";
            this.btnRemoveAssignment.Click += new System.EventHandler(this.btnRemoveAssignment_Click);

            this.toolStripLabelStatus.Text = "";
            this.toolStripLabelStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;

            // ═══════════════════════════════════════
            // panelMeta (order metadata, top)
            // ═══════════════════════════════════════
            this.panelMeta.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMeta.Height = 75;
            this.panelMeta.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.panelMeta.Name = "panelMeta";
            this.panelMeta.TabIndex = 1;

            // Row 1
            this.lblTemplate.Text = "Шаблон:";
            this.lblTemplate.AutoSize = true;
            this.lblTemplate.Location = new System.Drawing.Point(10, 8);
            this.lblTemplate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            this.lblTemplateValue.Text = "(не обрано)";
            this.lblTemplateValue.AutoSize = true;
            this.lblTemplateValue.Location = new System.Drawing.Point(75, 8);
            this.lblTemplateValue.ForeColor = System.Drawing.Color.DarkBlue;

            this.lblOrderNumber.Text = "Номер:";
            this.lblOrderNumber.AutoSize = true;
            this.lblOrderNumber.Location = new System.Drawing.Point(350, 8);

            this.txtOrderNumber.Location = new System.Drawing.Point(405, 5);
            this.txtOrderNumber.Size = new System.Drawing.Size(180, 23);

            this.lblOrderDate.Text = "Дата:";
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.Location = new System.Drawing.Point(600, 8);

            this.dtpOrderDate.Location = new System.Drawing.Point(640, 5);
            this.dtpOrderDate.Size = new System.Drawing.Size(140, 23);
            this.dtpOrderDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            // Row 2
            this.lblStart.Text = "Початок:";
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(10, 38);

            this.dtpStart.Location = new System.Drawing.Point(75, 35);
            this.dtpStart.Size = new System.Drawing.Size(180, 23);
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStart.CustomFormat = "dd.MM.yyyy HH:mm";

            this.lblEnd.Text = "Кінець:";
            this.lblEnd.AutoSize = true;
            this.lblEnd.Location = new System.Drawing.Point(270, 38);

            this.dtpEnd.Location = new System.Drawing.Point(325, 35);
            this.dtpEnd.Size = new System.Drawing.Size(180, 23);
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEnd.CustomFormat = "dd.MM.yyyy HH:mm";

            this.lblCommander.Text = "Командир:";
            this.lblCommander.AutoSize = true;
            this.lblCommander.Location = new System.Drawing.Point(520, 38);

            this.txtCommander.Location = new System.Drawing.Point(595, 35);
            this.txtCommander.Size = new System.Drawing.Size(350, 23);
            this.txtCommander.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

            this.panelMeta.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblTemplate, this.lblTemplateValue,
                this.lblOrderNumber, this.txtOrderNumber,
                this.lblOrderDate, this.dtpOrderDate,
                this.lblStart, this.dtpStart,
                this.lblEnd, this.dtpEnd,
                this.lblCommander, this.txtCommander
            });

            // ═══════════════════════════════════════
            // grpTimeRanges (dynamic TimeRange panel)
            // ═══════════════════════════════════════
            this.grpTimeRanges.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTimeRanges.Height = 90;
            this.grpTimeRanges.Text = "Часові діапазони (зміни)";
            this.grpTimeRanges.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpTimeRanges.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.grpTimeRanges.Name = "grpTimeRanges";
            this.grpTimeRanges.TabIndex = 2;

            this.flowTimeRanges.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowTimeRanges.AutoScroll = true;
            this.flowTimeRanges.WrapContents = false;
            this.flowTimeRanges.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flowTimeRanges.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.flowTimeRanges.Name = "flowTimeRanges";

            this.grpTimeRanges.Controls.Add(this.flowTimeRanges);

            // ═══════════════════════════════════════
            // splitMain
            // ═══════════════════════════════════════
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Name = "splitMain";
            this.splitMain.SplitterDistance = 420;
            this.splitMain.TabIndex = 3;

            // treeView1 (left)
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.Name = "treeView1";
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);

            this.splitMain.Panel1.Controls.Add(this.treeView1);

            // splitRight (right: node info + assignments grid)
            this.splitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitRight.SplitterDistance = 160;
            this.splitRight.Name = "splitRight";

            this.splitMain.Panel2.Controls.Add(this.splitRight);

            // ═══════════════════════════════════════
            // grpNodeInfo (top-right)
            // ═══════════════════════════════════════
            this.grpNodeInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpNodeInfo.Text = "Інформація про вузол";
            this.grpNodeInfo.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.grpNodeInfo.Name = "grpNodeInfo";

            this.lblNodeTitle.AutoSize = true;
            this.lblNodeTitle.Location = new System.Drawing.Point(12, 22);
            this.lblNodeTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblNodeTitle.Text = "";

            this.lblNodeType.AutoSize = true;
            this.lblNodeType.Location = new System.Drawing.Point(12, 46);
            this.lblNodeType.Text = "Тип:";

            this.lblNodeFlags.AutoSize = true;
            this.lblNodeFlags.Location = new System.Drawing.Point(12, 66);
            this.lblNodeFlags.Text = "Прапорці:";

            this.lblRenderedText.AutoSize = true;
            this.lblRenderedText.Location = new System.Drawing.Point(12, 92);
            this.lblRenderedText.Text = "Результат рендерингу:";

            this.txtRenderedText.Location = new System.Drawing.Point(12, 110);
            this.txtRenderedText.ReadOnly = true;
            this.txtRenderedText.BackColor = System.Drawing.Color.LightYellow;
            this.txtRenderedText.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.txtRenderedText.Size = new System.Drawing.Size(820, 23);

            this.grpNodeInfo.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblNodeTitle, this.lblNodeType, this.lblNodeFlags,
                this.lblRenderedText, this.txtRenderedText
            });

            this.splitRight.Panel1.Controls.Add(this.grpNodeInfo);

            // ═══════════════════════════════════════
            // Assignments area (bottom-right)
            // ═══════════════════════════════════════
            this.panelAssignmentHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAssignmentHeader.Height = 30;
            this.panelAssignmentHeader.Name = "panelAssignmentHeader";

            this.lblAssignments.Text = "Призначення:";
            this.lblAssignments.AutoSize = true;
            this.lblAssignments.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAssignments.Location = new System.Drawing.Point(8, 6);

            this.lblAssignmentLimit.Text = "";
            this.lblAssignmentLimit.AutoSize = true;
            this.lblAssignmentLimit.ForeColor = System.Drawing.Color.Gray;
            this.lblAssignmentLimit.Location = new System.Drawing.Point(120, 6);

            this.panelAssignmentHeader.Controls.Add(this.lblAssignments);
            this.panelAssignmentHeader.Controls.Add(this.lblAssignmentLimit);

            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Name = "dataGridView1";

            this.splitRight.Panel2.Controls.Add(this.dataGridView1);
            this.splitRight.Panel2.Controls.Add(this.panelAssignmentHeader);

            // ═══════════════════════════════════════
            // OrderCreatorForm
            // ═══════════════════════════════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 800);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.grpTimeRanges);
            this.Controls.Add(this.panelMeta);
            this.Controls.Add(this.toolStrip1);
            this.Name = "OrderCreatorForm";
            this.Text = "Створення наказу добового наряду";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.OrderCreatorForm_Load);

            this.panelMeta.ResumeLayout(false);
            this.panelMeta.PerformLayout();
            this.grpTimeRanges.ResumeLayout(false);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.splitRight.Panel1.ResumeLayout(false);
            this.splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).EndInit();
            this.splitRight.ResumeLayout(false);
            this.grpNodeInfo.ResumeLayout(false);
            this.grpNodeInfo.PerformLayout();
            this.panelAssignmentHeader.ResumeLayout(false);
            this.panelAssignmentHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
