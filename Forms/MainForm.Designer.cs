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
            panelButtons = new Panel();
            btnRefresh = new Button();
            btnDeleteOrder = new Button();
            btnEditStructure = new Button();
            btnNewOrder = new Button();
            labelOrders = new Label();
            dataGridViewOrders = new DataGridView();
            panelButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewOrders).BeginInit();
            SuspendLayout();
            // 
            // panelButtons
            // 
            panelButtons.Controls.Add(btnRefresh);
            panelButtons.Controls.Add(btnDeleteOrder);
            panelButtons.Controls.Add(btnEditStructure);
            panelButtons.Controls.Add(btnNewOrder);
            panelButtons.Dock = DockStyle.Top;
            panelButtons.Location = new Point(0, 0);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(1200, 60);
            panelButtons.TabIndex = 0;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.LightYellow;
            btnRefresh.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRefresh.Image = Properties.Resources.refresh_icon;
            btnRefresh.Location = new Point(485, 12);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(120, 40);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "🔄 Оновити";
            btnRefresh.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnDeleteOrder
            // 
            btnDeleteOrder.BackColor = Color.LightCoral;
            btnDeleteOrder.Enabled = false;
            btnDeleteOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDeleteOrder.Image = Properties.Resources.delete_icon;
            btnDeleteOrder.Location = new Point(345, 12);
            btnDeleteOrder.Name = "btnDeleteOrder";
            btnDeleteOrder.Size = new Size(120, 40);
            btnDeleteOrder.TabIndex = 2;
            btnDeleteOrder.Text = "🗑️ Видалити";
            btnDeleteOrder.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDeleteOrder.UseVisualStyleBackColor = false;
            btnDeleteOrder.Click += btnDeleteOrder_Click;
            // 
            // btnEditStructure
            // 
            btnEditStructure.BackColor = Color.LightBlue;
            btnEditStructure.Enabled = false;
            btnEditStructure.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEditStructure.Image = Properties.Resources.edit_icon;
            btnEditStructure.Location = new Point(165, 12);
            btnEditStructure.Name = "btnEditStructure";
            btnEditStructure.Size = new Size(160, 40);
            btnEditStructure.TabIndex = 1;
            btnEditStructure.Text = "✏️ Редагувати структуру";
            btnEditStructure.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnEditStructure.UseVisualStyleBackColor = false;
            btnEditStructure.Click += btnEditStructure_Click;
            // 
            // btnNewOrder
            // 
            btnNewOrder.BackColor = Color.LightGreen;
            btnNewOrder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnNewOrder.Image = Properties.Resources.add_icon;
            btnNewOrder.ImageAlign = ContentAlignment.TopLeft;
            btnNewOrder.Location = new Point(12, 12);
            btnNewOrder.Name = "btnNewOrder";
            btnNewOrder.Size = new Size(140, 40);
            btnNewOrder.TabIndex = 0;
            btnNewOrder.Text = "📄 Новий наказ";
            btnNewOrder.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnNewOrder.UseVisualStyleBackColor = false;
            btnNewOrder.Click += btnNewOrder_Click;
            // 
            // labelOrders
            // 
            labelOrders.AutoSize = true;
            labelOrders.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            labelOrders.ForeColor = Color.DarkBlue;
            labelOrders.Location = new Point(12, 70);
            labelOrders.Name = "labelOrders";
            labelOrders.Size = new Size(195, 21);
            labelOrders.TabIndex = 1;
            labelOrders.Text = "Існуючі добові наряди:";
            // 
            // dataGridViewOrders
            // 
            dataGridViewOrders.AllowUserToAddRows = false;
            dataGridViewOrders.AllowUserToDeleteRows = false;
            dataGridViewOrders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewOrders.BackgroundColor = Color.White;
            dataGridViewOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewOrders.Location = new Point(12, 100);
            dataGridViewOrders.MultiSelect = false;
            dataGridViewOrders.Name = "dataGridViewOrders";
            dataGridViewOrders.ReadOnly = true;
            dataGridViewOrders.RowHeadersVisible = false;
            dataGridViewOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOrders.Size = new Size(1176, 500);
            dataGridViewOrders.TabIndex = 2;
            dataGridViewOrders.SelectionChanged += dataGridViewOrders_SelectionChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 650);
            Controls.Add(dataGridViewOrders);
            Controls.Add(labelOrders);
            Controls.Add(panelButtons);
            MinimumSize = new Size(800, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Добові наряди - Головна";
            panelButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewOrders).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnNewOrder;
        private System.Windows.Forms.Button btnEditStructure;
        private System.Windows.Forms.Button btnDeleteOrder;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label labelOrders;
        private System.Windows.Forms.DataGridView dataGridViewOrders;
    }
}
