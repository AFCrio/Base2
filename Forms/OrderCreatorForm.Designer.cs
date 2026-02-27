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
            toolStrip1 = new ToolStrip();
            btnSave = new ToolStripButton();
            btnRefresh = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            btnAddAssignment = new ToolStripButton();
            btnRemoveAssignment = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripLabelStatus = new ToolStripLabel();
            panelMeta = new Panel();
            lblTemplate = new Label();
            lblTemplateValue = new Label();
            lblOrderNumber = new Label();
            txtOrderNumber = new TextBox();
            lblOrderDate = new Label();
            dtpOrderDate = new DateTimePicker();
            lblStart = new Label();
            dtpStart = new DateTimePicker();
            lblEnd = new Label();
            dtpEnd = new DateTimePicker();
            lblCommander = new Label();
            txtCommander = new TextBox();
            grpTimeRanges = new GroupBox();
            flowTimeRanges = new FlowLayoutPanel();
            splitMain = new SplitContainer();
            treeView1 = new TreeView();
            splitRight = new SplitContainer();
            grpNodeInfo = new GroupBox();
            lblNodeTitle = new Label();
            lblNodeType = new Label();
            lblNodeFlags = new Label();
            lblRenderedText = new Label();
            txtRenderedText = new TextBox();
            dataGridView1 = new DataGridView();
            panelAssignmentHeader = new Panel();
            lblAssignments = new Label();
            lblAssignmentLimit = new Label();
            toolStrip1.SuspendLayout();
            panelMeta.SuspendLayout();
            grpTimeRanges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitRight).BeginInit();
            splitRight.Panel1.SuspendLayout();
            splitRight.Panel2.SuspendLayout();
            splitRight.SuspendLayout();
            grpNodeInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panelAssignmentHeader.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnSave, btnRefresh, toolStripSeparator1, btnAddAssignment, btnRemoveAssignment, toolStripSeparator2, toolStripLabelStatus });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1300, 25);
            toolStrip1.TabIndex = 0;
            // 
            // btnSave
            // 
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(76, 22);
            btnSave.Text = "💾 Зберегти";
            btnSave.Click += btnSave_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(74, 22);
            btnRefresh.Text = "🔄 Оновити";
            btnRefresh.Click += btnRefresh_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // btnAddAssignment
            // 
            btnAddAssignment.Name = "btnAddAssignment";
            btnAddAssignment.Size = new Size(129, 22);
            btnAddAssignment.Text = "➕ Призначити особу";
            btnAddAssignment.Click += btnAddAssignment_Click;
            // 
            // btnRemoveAssignment
            // 
            btnRemoveAssignment.Name = "btnRemoveAssignment";
            btnRemoveAssignment.Size = new Size(133, 22);
            btnRemoveAssignment.Text = "➖ Зняти призначення";
            btnRemoveAssignment.Click += btnRemoveAssignment_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // toolStripLabelStatus
            // 
            toolStripLabelStatus.Alignment = ToolStripItemAlignment.Right;
            toolStripLabelStatus.Name = "toolStripLabelStatus";
            toolStripLabelStatus.Size = new Size(0, 22);
            // 
            // panelMeta
            // 
            panelMeta.Controls.Add(lblTemplate);
            panelMeta.Controls.Add(lblTemplateValue);
            panelMeta.Controls.Add(lblOrderNumber);
            panelMeta.Controls.Add(txtOrderNumber);
            panelMeta.Controls.Add(lblOrderDate);
            panelMeta.Controls.Add(dtpOrderDate);
            panelMeta.Controls.Add(lblStart);
            panelMeta.Controls.Add(dtpStart);
            panelMeta.Controls.Add(lblEnd);
            panelMeta.Controls.Add(dtpEnd);
            panelMeta.Controls.Add(lblCommander);
            panelMeta.Controls.Add(txtCommander);
            panelMeta.Dock = DockStyle.Top;
            panelMeta.Location = new Point(0, 25);
            panelMeta.Name = "panelMeta";
            panelMeta.Padding = new Padding(8, 4, 8, 4);
            panelMeta.Size = new Size(1300, 75);
            panelMeta.TabIndex = 1;
            // 
            // lblTemplate
            // 
            lblTemplate.AutoSize = true;
            lblTemplate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTemplate.Location = new Point(10, 8);
            lblTemplate.Name = "lblTemplate";
            lblTemplate.Size = new Size(56, 15);
            lblTemplate.TabIndex = 0;
            lblTemplate.Text = "Шаблон:";
            // 
            // lblTemplateValue
            // 
            lblTemplateValue.AutoSize = true;
            lblTemplateValue.ForeColor = Color.DarkBlue;
            lblTemplateValue.Location = new Point(75, 8);
            lblTemplateValue.Name = "lblTemplateValue";
            lblTemplateValue.Size = new Size(72, 15);
            lblTemplateValue.TabIndex = 1;
            lblTemplateValue.Text = "(не обрано)";
            // 
            // lblOrderNumber
            // 
            lblOrderNumber.AutoSize = true;
            lblOrderNumber.Location = new Point(350, 8);
            lblOrderNumber.Name = "lblOrderNumber";
            lblOrderNumber.Size = new Size(48, 15);
            lblOrderNumber.TabIndex = 2;
            lblOrderNumber.Text = "Номер:";
            // 
            // txtOrderNumber
            // 
            txtOrderNumber.Location = new Point(405, 5);
            txtOrderNumber.Name = "txtOrderNumber";
            txtOrderNumber.Size = new Size(180, 23);
            txtOrderNumber.TabIndex = 3;
            // 
            // lblOrderDate
            // 
            lblOrderDate.AutoSize = true;
            lblOrderDate.Location = new Point(600, 8);
            lblOrderDate.Name = "lblOrderDate";
            lblOrderDate.Size = new Size(35, 15);
            lblOrderDate.TabIndex = 4;
            lblOrderDate.Text = "Дата:";
            // 
            // dtpOrderDate
            // 
            dtpOrderDate.Format = DateTimePickerFormat.Short;
            dtpOrderDate.Location = new Point(640, 5);
            dtpOrderDate.Name = "dtpOrderDate";
            dtpOrderDate.Size = new Size(140, 23);
            dtpOrderDate.TabIndex = 5;
            // 
            // lblStart
            // 
            lblStart.AutoSize = true;
            lblStart.Location = new Point(10, 38);
            lblStart.Name = "lblStart";
            lblStart.Size = new Size(57, 15);
            lblStart.TabIndex = 6;
            lblStart.Text = "Початок:";
            // 
            // dtpStart
            // 
            dtpStart.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpStart.Format = DateTimePickerFormat.Custom;
            dtpStart.Location = new Point(75, 35);
            dtpStart.Name = "dtpStart";
            dtpStart.Size = new Size(180, 23);
            dtpStart.TabIndex = 7;
            // 
            // lblEnd
            // 
            lblEnd.AutoSize = true;
            lblEnd.Location = new Point(270, 38);
            lblEnd.Name = "lblEnd";
            lblEnd.Size = new Size(46, 15);
            lblEnd.TabIndex = 8;
            lblEnd.Text = "Кінець:";
            // 
            // dtpEnd
            // 
            dtpEnd.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpEnd.Format = DateTimePickerFormat.Custom;
            dtpEnd.Location = new Point(325, 35);
            dtpEnd.Name = "dtpEnd";
            dtpEnd.Size = new Size(180, 23);
            dtpEnd.TabIndex = 9;
            // 
            // lblCommander
            // 
            lblCommander.AutoSize = true;
            lblCommander.Location = new Point(520, 38);
            lblCommander.Name = "lblCommander";
            lblCommander.Size = new Size(66, 15);
            lblCommander.TabIndex = 10;
            lblCommander.Text = "Командир:";
            // 
            // txtCommander
            // 
            txtCommander.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtCommander.Location = new Point(595, 35);
            txtCommander.Name = "txtCommander";
            txtCommander.Size = new Size(1450, 23);
            txtCommander.TabIndex = 11;
            // 
            // grpTimeRanges
            // 
            grpTimeRanges.Controls.Add(flowTimeRanges);
            grpTimeRanges.Dock = DockStyle.Top;
            grpTimeRanges.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpTimeRanges.Location = new Point(0, 100);
            grpTimeRanges.Name = "grpTimeRanges";
            grpTimeRanges.Padding = new Padding(8, 4, 8, 4);
            grpTimeRanges.Size = new Size(1300, 90);
            grpTimeRanges.TabIndex = 2;
            grpTimeRanges.TabStop = false;
            grpTimeRanges.Text = "Часові діапазони (зміни)";
            // 
            // flowTimeRanges
            // 
            flowTimeRanges.AutoScroll = true;
            flowTimeRanges.Dock = DockStyle.Fill;
            flowTimeRanges.Font = new Font("Segoe UI", 9F);
            flowTimeRanges.Location = new Point(8, 20);
            flowTimeRanges.Name = "flowTimeRanges";
            flowTimeRanges.Size = new Size(1284, 66);
            flowTimeRanges.TabIndex = 0;
            flowTimeRanges.WrapContents = false;
            // 
            // splitMain
            // 
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 190);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(treeView1);
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(splitRight);
            splitMain.Size = new Size(1300, 610);
            splitMain.SplitterDistance = 800;
            splitMain.TabIndex = 3;
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.HideSelection = false;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(800, 610);
            treeView1.TabIndex = 0;
            treeView1.AfterSelect += treeView1_AfterSelect;
            // 
            // splitRight
            // 
            splitRight.Dock = DockStyle.Fill;
            splitRight.Location = new Point(0, 0);
            splitRight.Name = "splitRight";
            splitRight.Orientation = Orientation.Horizontal;
            // 
            // splitRight.Panel1
            // 
            splitRight.Panel1.Controls.Add(grpNodeInfo);
            // 
            // splitRight.Panel2
            // 
            splitRight.Panel2.Controls.Add(dataGridView1);
            splitRight.Panel2.Controls.Add(panelAssignmentHeader);
            splitRight.Size = new Size(496, 610);
            splitRight.SplitterDistance = 433;
            splitRight.TabIndex = 0;
            // 
            // grpNodeInfo
            // 
            grpNodeInfo.Controls.Add(lblNodeTitle);
            grpNodeInfo.Controls.Add(lblNodeType);
            grpNodeInfo.Controls.Add(lblNodeFlags);
            grpNodeInfo.Controls.Add(lblRenderedText);
            grpNodeInfo.Controls.Add(txtRenderedText);
            grpNodeInfo.Dock = DockStyle.Fill;
            grpNodeInfo.Location = new Point(0, 0);
            grpNodeInfo.Name = "grpNodeInfo";
            grpNodeInfo.Padding = new Padding(10, 6, 10, 6);
            grpNodeInfo.Size = new Size(496, 433);
            grpNodeInfo.TabIndex = 0;
            grpNodeInfo.TabStop = false;
            grpNodeInfo.Text = "Інформація про вузол";
            // 
            // lblNodeTitle
            // 
            lblNodeTitle.AutoSize = true;
            lblNodeTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblNodeTitle.Location = new Point(12, 22);
            lblNodeTitle.Name = "lblNodeTitle";
            lblNodeTitle.Size = new Size(0, 19);
            lblNodeTitle.TabIndex = 0;
            // 
            // lblNodeType
            // 
            lblNodeType.AutoSize = true;
            lblNodeType.Location = new Point(12, 46);
            lblNodeType.Name = "lblNodeType";
            lblNodeType.Size = new Size(30, 15);
            lblNodeType.TabIndex = 1;
            lblNodeType.Text = "Тип:";
            // 
            // lblNodeFlags
            // 
            lblNodeFlags.AutoSize = true;
            lblNodeFlags.Location = new Point(12, 66);
            lblNodeFlags.Name = "lblNodeFlags";
            lblNodeFlags.Size = new Size(63, 15);
            lblNodeFlags.TabIndex = 2;
            lblNodeFlags.Text = "Прапорці:";
            // 
            // lblRenderedText
            // 
            lblRenderedText.AutoSize = true;
            lblRenderedText.Location = new Point(12, 92);
            lblRenderedText.Name = "lblRenderedText";
            lblRenderedText.Size = new Size(130, 15);
            lblRenderedText.TabIndex = 3;
            lblRenderedText.Text = "Результат рендерингу:";
            // 
            // txtRenderedText
            // 
            txtRenderedText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRenderedText.BackColor = Color.LightYellow;
            txtRenderedText.Location = new Point(12, 110);
            txtRenderedText.Multiline = true;
            txtRenderedText.Name = "txtRenderedText";
            txtRenderedText.ReadOnly = true;
            txtRenderedText.Size = new Size(472, 193);
            txtRenderedText.TabIndex = 4;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 30);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(496, 143);
            dataGridView1.TabIndex = 0;
            // 
            // panelAssignmentHeader
            // 
            panelAssignmentHeader.Controls.Add(lblAssignments);
            panelAssignmentHeader.Controls.Add(lblAssignmentLimit);
            panelAssignmentHeader.Dock = DockStyle.Top;
            panelAssignmentHeader.Location = new Point(0, 0);
            panelAssignmentHeader.Name = "panelAssignmentHeader";
            panelAssignmentHeader.Size = new Size(496, 30);
            panelAssignmentHeader.TabIndex = 1;
            // 
            // lblAssignments
            // 
            lblAssignments.AutoSize = true;
            lblAssignments.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblAssignments.Location = new Point(8, 6);
            lblAssignments.Name = "lblAssignments";
            lblAssignments.Size = new Size(88, 15);
            lblAssignments.TabIndex = 0;
            lblAssignments.Text = "Призначення:";
            // 
            // lblAssignmentLimit
            // 
            lblAssignmentLimit.AutoSize = true;
            lblAssignmentLimit.ForeColor = Color.Gray;
            lblAssignmentLimit.Location = new Point(120, 6);
            lblAssignmentLimit.Name = "lblAssignmentLimit";
            lblAssignmentLimit.Size = new Size(0, 15);
            lblAssignmentLimit.TabIndex = 1;
            // 
            // OrderCreatorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1300, 800);
            Controls.Add(splitMain);
            Controls.Add(grpTimeRanges);
            Controls.Add(panelMeta);
            Controls.Add(toolStrip1);
            Name = "OrderCreatorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Створення наказу добового наряду";
            Load += OrderCreatorForm_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            panelMeta.ResumeLayout(false);
            panelMeta.PerformLayout();
            grpTimeRanges.ResumeLayout(false);
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            splitRight.Panel1.ResumeLayout(false);
            splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitRight).EndInit();
            splitRight.ResumeLayout(false);
            grpNodeInfo.ResumeLayout(false);
            grpNodeInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            panelAssignmentHeader.ResumeLayout(false);
            panelAssignmentHeader.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
