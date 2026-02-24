namespace Base2.Forms
{
    partial class TemplateEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            btnAddNode = new System.Windows.Forms.ToolStripButton();
            btnAddChild = new System.Windows.Forms.ToolStripButton();
            btnAddSibling = new System.Windows.Forms.ToolStripButton();
            sep1 = new System.Windows.Forms.ToolStripSeparator();
            btnDelete = new System.Windows.Forms.ToolStripButton();
            sep2 = new System.Windows.Forms.ToolStripSeparator();
            btnMoveUp = new System.Windows.Forms.ToolStripButton();
            btnMoveDown = new System.Windows.Forms.ToolStripButton();
            sep3 = new System.Windows.Forms.ToolStripSeparator();
            btnSave = new System.Windows.Forms.ToolStripButton();
            splitMain = new System.Windows.Forms.SplitContainer();
            treeView1 = new System.Windows.Forms.TreeView();
            panelRight = new System.Windows.Forms.Panel();
            labelHint = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            SuspendLayout();

            // toolStrip1
            toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                btnAddNode, btnAddChild, btnAddSibling, sep1,
                btnDelete, sep2,
                btnMoveUp, btnMoveDown, sep3,
                btnSave
            });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Size = new System.Drawing.Size(1200, 27);

            // buttons
            btnAddNode.Text = "➕ Секцію";
            btnAddNode.ToolTipText = "Додати секцію верхнього рівня";
            btnAddNode.Click += btnAddNode_Click;

            btnAddChild.Text = "⬇ Дочірній";
            btnAddChild.ToolTipText = "Додати дочірній вузол";
            btnAddChild.Click += btnAddChild_Click;

            btnAddSibling.Text = "↔ Сусідній";
            btnAddSibling.ToolTipText = "Додати сусідній вузол";
            btnAddSibling.Click += btnAddSibling_Click;

            btnDelete.Text = "🗑️ Видалити";
            btnDelete.Click += btnDelete_Click;

            btnMoveUp.Text = "▲";
            btnMoveUp.ToolTipText = "Перемістити вгору";
            btnMoveUp.Click += btnMoveUp_Click;

            btnMoveDown.Text = "▼";
            btnMoveDown.ToolTipText = "Перемістити вниз";
            btnMoveDown.Click += btnMoveDown_Click;

            btnSave.Text = "💾 Зберегти";
            btnSave.Click += btnSave_Click;

            // splitMain
            splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            splitMain.Location = new System.Drawing.Point(0, 27);
            splitMain.Size = new System.Drawing.Size(1200, 673);
            splitMain.SplitterDistance = 380;
            splitMain.Panel1.Controls.Add(treeView1);
            splitMain.Panel2.Controls.Add(panelRight);
            splitMain.Panel2.Controls.Add(labelHint);

            // treeView1
            treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            treeView1.HideSelection = false;
            treeView1.Font = new System.Drawing.Font("Segoe UI", 10F);
            treeView1.ItemHeight = 24;
            treeView1.AfterSelect += treeView1_AfterSelect;

            // panelRight — контейнер для динамічних панелей
            panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            panelRight.AutoScroll = true;
            panelRight.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);

            // labelHint
            labelHint.Dock = System.Windows.Forms.DockStyle.Fill;
            labelHint.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Italic);
            labelHint.ForeColor = System.Drawing.Color.Gray;
            labelHint.Text = "Виберіть вузол у дереві для редагування його властивостей";
            labelHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // TemplateEditorForm
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1200, 700);
            Controls.Add(splitMain);
            Controls.Add(toolStrip1);
            MinimumSize = new System.Drawing.Size(900, 550);
            Name = "TemplateEditorForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Редактор шаблону";
            Load += TemplateEditorForm_Load;

            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAddNode;
        private System.Windows.Forms.ToolStripButton btnAddChild;
        private System.Windows.Forms.ToolStripButton btnAddSibling;
        private System.Windows.Forms.ToolStripSeparator sep1;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripSeparator sep2;
        private System.Windows.Forms.ToolStripButton btnMoveUp;
        private System.Windows.Forms.ToolStripButton btnMoveDown;
        private System.Windows.Forms.ToolStripSeparator sep3;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Label labelHint;
    }
}
