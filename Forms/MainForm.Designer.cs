using System;
using System.Drawing;
using System.Windows.Forms;

namespace Base2.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelTemplate = new Panel();
            labelTemplate = new Label();
            comboBoxTemplates = new ComboBox();
            btnEditTemplate = new Button();
            btnNewTemplate = new Button();
            btnDeleteTemplate = new Button();
            btnReferences = new Button();
            panelOrders = new Panel();
            labelOrders = new Label();
            btnNewOrder = new Button();
            btnEditOrder = new Button();
            btnDeleteOrder = new Button();
            btnRefresh = new Button();
            dataGridViewOrders = new DataGridView();
            labelOutdated = new Label();
            panelTemplate.SuspendLayout();
            panelOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewOrders).BeginInit();
            SuspendLayout();
            // 
            // panelTemplate
            // 
            panelTemplate.BackColor = Color.FromArgb(240, 245, 255);
            panelTemplate.BorderStyle = BorderStyle.FixedSingle;
            panelTemplate.Controls.Add(labelTemplate);
            panelTemplate.Controls.Add(comboBoxTemplates);
            panelTemplate.Controls.Add(btnEditTemplate);
            panelTemplate.Controls.Add(btnNewTemplate);
            panelTemplate.Controls.Add(btnDeleteTemplate);
            panelTemplate.Controls.Add(btnReferences);
            panelTemplate.Dock = DockStyle.Top;
            panelTemplate.Location = new Point(0, 0);
            panelTemplate.Name = "panelTemplate";
            panelTemplate.Padding = new Padding(12, 8, 12, 8);
            panelTemplate.Size = new Size(1200, 60);
            panelTemplate.TabIndex = 0;
            // 
            // labelTemplate
            // 
            labelTemplate.AutoSize = true;
            labelTemplate.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            labelTemplate.ForeColor = Color.DarkSlateBlue;
            labelTemplate.Location = new Point(12, 18);
            labelTemplate.Name = "labelTemplate";
            labelTemplate.Size = new Size(72, 19);
            labelTemplate.TabIndex = 0;
            labelTemplate.Text = "Шаблон:";
            // 
            // comboBoxTemplates
            // 
            comboBoxTemplates.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxTemplates.Font = new Font("Segoe UI", 10F);
            comboBoxTemplates.Location = new Point(90, 15);
            comboBoxTemplates.Name = "comboBoxTemplates";
            comboBoxTemplates.Size = new Size(400, 25);
            comboBoxTemplates.TabIndex = 1;
            comboBoxTemplates.SelectedIndexChanged += comboBoxTemplates_SelectedIndexChanged;
            // 
            // btnEditTemplate
            // 
            btnEditTemplate.BackColor = Color.LightBlue;
            btnEditTemplate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEditTemplate.Location = new Point(510, 13);
            btnEditTemplate.Name = "btnEditTemplate";
            btnEditTemplate.Size = new Size(170, 32);
            btnEditTemplate.TabIndex = 2;
            btnEditTemplate.Text = "✏️ Редагувати шаблон";
            btnEditTemplate.UseVisualStyleBackColor = false;
            btnEditTemplate.Click += btnEditTemplate_Click;
            // 
            // btnNewTemplate
            // 
            btnNewTemplate.BackColor = Color.LightGreen;
            btnNewTemplate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnNewTemplate.Location = new Point(690, 13);
            btnNewTemplate.Name = "btnNewTemplate";
            btnNewTemplate.Size = new Size(150, 32);
            btnNewTemplate.TabIndex = 3;
            btnNewTemplate.Text = "➕ Новий шаблон";
            btnNewTemplate.UseVisualStyleBackColor = false;
            btnNewTemplate.Click += btnNewTemplate_Click;
            // 
            // btnDeleteTemplate
            // 
            btnDeleteTemplate.BackColor = Color.MistyRose;
            btnDeleteTemplate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDeleteTemplate.Location = new Point(850, 13);
            btnDeleteTemplate.Name = "btnDeleteTemplate";
            btnDeleteTemplate.Size = new Size(160, 32);
            btnDeleteTemplate.TabIndex = 4;
            btnDeleteTemplate.Text = "🗑️ Видалити шаблон";
            btnDeleteTemplate.UseVisualStyleBackColor = false;
            btnDeleteTemplate.Click += btnDeleteTemplate_Click;
            // 
            // btnReferences
            // 
            btnReferences.BackColor = Color.Lavender;
            btnReferences.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnReferences.Location = new Point(1020, 13);
            btnReferences.Name = "btnReferences";
            btnReferences.Size = new Size(160, 32);
            btnReferences.TabIndex = 5;
            btnReferences.Text = "📖 Довідники";
            btnReferences.UseVisualStyleBackColor = false;
            btnReferences.Click += btnReferences_Click;
            // 
            // panelOrders
            // 
            panelOrders.Controls.Add(labelOrders);
            panelOrders.Controls.Add(btnNewOrder);
            panelOrders.Controls.Add(btnEditOrder);
            panelOrders.Controls.Add(btnDeleteOrder);
            panelOrders.Controls.Add(btnRefresh);
            panelOrders.Controls.Add(labelOutdated);
            panelOrders.Dock = DockStyle.Top;
            panelOrders.Location = new Point(0, 60);
            panelOrders.Name = "panelOrders";
            panelOrders.Size = new Size(1200, 50);
            panelOrders.TabIndex = 1;
            // 
            // labelOrders
            // 
            labelOrders.AutoSize = true;
            labelOrders.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            labelOrders.ForeColor = Color.DarkBlue;
            labelOrders.Location = new Point(12, 14);
            labelOrders.Name = "labelOrders";
            labelOrders.Size = new Size(66, 19);
            labelOrders.TabIndex = 0;
            labelOrders.Text = "Накази:";
            // 
            // btnNewOrder
            // 
            btnNewOrder.BackColor = Color.LightGreen;
            btnNewOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnNewOrder.Location = new Point(90, 10);
            btnNewOrder.Name = "btnNewOrder";
            btnNewOrder.Size = new Size(160, 32);
            btnNewOrder.TabIndex = 1;
            btnNewOrder.Text = "📄 Створити наказ";
            btnNewOrder.UseVisualStyleBackColor = false;
            btnNewOrder.Click += btnNewOrder_Click;
            // 
            // btnEditOrder
            // 
            btnEditOrder.BackColor = Color.LightBlue;
            btnEditOrder.Enabled = false;
            btnEditOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEditOrder.Location = new Point(260, 10);
            btnEditOrder.Name = "btnEditOrder";
            btnEditOrder.Size = new Size(170, 32);
            btnEditOrder.TabIndex = 2;
            btnEditOrder.Text = "✏️ Редагувати наказ";
            btnEditOrder.UseVisualStyleBackColor = false;
            btnEditOrder.Click += btnEditOrder_Click;
            // 
            // btnDeleteOrder
            // 
            btnDeleteOrder.BackColor = Color.LightCoral;
            btnDeleteOrder.Enabled = false;
            btnDeleteOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDeleteOrder.Location = new Point(440, 10);
            btnDeleteOrder.Name = "btnDeleteOrder";
            btnDeleteOrder.Size = new Size(140, 32);
            btnDeleteOrder.TabIndex = 3;
            btnDeleteOrder.Text = "🗑️ Видалити";
            btnDeleteOrder.UseVisualStyleBackColor = false;
            btnDeleteOrder.Click += btnDeleteOrder_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.LightYellow;
            btnRefresh.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRefresh.Location = new Point(590, 10);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(120, 32);
            btnRefresh.TabIndex = 4;
            btnRefresh.Text = "🔄 Оновити";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // labelOutdated
            // 
            labelOutdated.AutoSize = true;
            labelOutdated.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelOutdated.ForeColor = Color.OrangeRed;
            labelOutdated.Location = new Point(720, 17);
            labelOutdated.Name = "labelOutdated";
            labelOutdated.Size = new Size(0, 15);
            labelOutdated.TabIndex = 5;
            // 
            // dataGridViewOrders
            // 
            dataGridViewOrders.AllowUserToAddRows = false;
            dataGridViewOrders.AllowUserToDeleteRows = false;
            dataGridViewOrders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewOrders.BackgroundColor = Color.White;
            dataGridViewOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewOrders.Location = new Point(12, 115);
            dataGridViewOrders.MultiSelect = false;
            dataGridViewOrders.Name = "dataGridViewOrders";
            dataGridViewOrders.ReadOnly = true;
            dataGridViewOrders.RowHeadersVisible = false;
            dataGridViewOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOrders.Size = new Size(1176, 485);
            dataGridViewOrders.TabIndex = 2;
            dataGridViewOrders.SelectionChanged += dataGridViewOrders_SelectionChanged;
            dataGridViewOrders.CellDoubleClick += dataGridViewOrders_CellDoubleClick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 650);
            Controls.Add(dataGridViewOrders);
            Controls.Add(panelOrders);
            Controls.Add(panelTemplate);
            MinimumSize = new Size(900, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Добові наряди — Головна";
            panelTemplate.ResumeLayout(false);
            panelTemplate.PerformLayout();
            panelOrders.ResumeLayout(false);
            panelOrders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewOrders).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTemplate;
        private System.Windows.Forms.Label labelTemplate;
        private System.Windows.Forms.ComboBox comboBoxTemplates;
        private System.Windows.Forms.Button btnEditTemplate;
        private System.Windows.Forms.Button btnNewTemplate;
        private System.Windows.Forms.Button btnDeleteTemplate;
        private System.Windows.Forms.Panel panelOrders;
        private System.Windows.Forms.Label labelOrders;
        private System.Windows.Forms.Button btnNewOrder;
        private System.Windows.Forms.Button btnEditOrder;
        private System.Windows.Forms.Button btnDeleteOrder;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label labelOutdated;
        private System.Windows.Forms.DataGridView dataGridViewOrders;
        private System.Windows.Forms.Button btnReferences;
    }
}
