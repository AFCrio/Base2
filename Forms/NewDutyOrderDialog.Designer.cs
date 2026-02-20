using System;
using System.Drawing;
using System.Windows.Forms;

namespace Base2.Forms
{
    partial class NewDutyOrderDialog
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
            labelOrderNumber = new Label();
            textBoxOrderNumber = new TextBox();
            labelDate = new Label();
            dateTimeOrderDate = new DateTimePicker();
            labelStart = new Label();
            dateTimeStart = new DateTimePicker();
            labelEnd = new Label();
            dateTimeEnd = new DateTimePicker();
            labelCommander = new Label();
            textBoxCommander = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            labelLocation = new Label();
            comboBoxLocation = new ComboBox();
            SuspendLayout();
            // 
            // labelOrderNumber
            // 
            labelOrderNumber.AutoSize = true;
            labelOrderNumber.Location = new Point(12, 15);
            labelOrderNumber.Name = "labelOrderNumber";
            labelOrderNumber.Size = new Size(87, 15);
            labelOrderNumber.TabIndex = 0;
            labelOrderNumber.Text = "Номер наказу:";
            // 
            // textBoxOrderNumber
            // 
            textBoxOrderNumber.Location = new Point(130, 12);
            textBoxOrderNumber.Name = "textBoxOrderNumber";
            textBoxOrderNumber.Size = new Size(250, 23);
            textBoxOrderNumber.TabIndex = 1;
            textBoxOrderNumber.Text = "А4463/";
            // 
            // labelDate
            // 
            labelDate.AutoSize = true;
            labelDate.Location = new Point(12, 50);
            labelDate.Name = "labelDate";
            labelDate.Size = new Size(74, 15);
            labelDate.TabIndex = 2;
            labelDate.Text = "Дата наказу:";
            // 
            // dateTimeOrderDate
            // 
            dateTimeOrderDate.Format = DateTimePickerFormat.Short;
            dateTimeOrderDate.Location = new Point(130, 47);
            dateTimeOrderDate.Name = "dateTimeOrderDate";
            dateTimeOrderDate.Size = new Size(120, 23);
            dateTimeOrderDate.TabIndex = 3;
            // 
            // labelStart
            // 
            labelStart.AutoSize = true;
            labelStart.Location = new Point(12, 85);
            labelStart.Name = "labelStart";
            labelStart.Size = new Size(72, 15);
            labelStart.TabIndex = 4;
            labelStart.Text = "Початок дії:";
            // 
            // dateTimeStart
            // 
            dateTimeStart.CustomFormat = "dd.MM.yyyy HH:mm";
            dateTimeStart.Format = DateTimePickerFormat.Custom;
            dateTimeStart.Location = new Point(130, 82);
            dateTimeStart.Name = "dateTimeStart";
            dateTimeStart.Size = new Size(150, 23);
            dateTimeStart.TabIndex = 5;
            dateTimeStart.ValueChanged += dateTimeStart_ValueChanged;
            // 
            // labelEnd
            // 
            labelEnd.AutoSize = true;
            labelEnd.Location = new Point(12, 120);
            labelEnd.Name = "labelEnd";
            labelEnd.Size = new Size(61, 15);
            labelEnd.TabIndex = 6;
            labelEnd.Text = "Кінець дії:";
            // 
            // dateTimeEnd
            // 
            dateTimeEnd.CustomFormat = "dd.MM.yyyy HH:mm";
            dateTimeEnd.Format = DateTimePickerFormat.Custom;
            dateTimeEnd.Location = new Point(130, 117);
            dateTimeEnd.Name = "dateTimeEnd";
            dateTimeEnd.Size = new Size(150, 23);
            dateTimeEnd.TabIndex = 7;
            // 
            // labelCommander
            // 
            labelCommander.AutoSize = true;
            labelCommander.Location = new Point(12, 155);
            labelCommander.Name = "labelCommander";
            labelCommander.Size = new Size(66, 15);
            labelCommander.TabIndex = 8;
            labelCommander.Text = "Командир:";
            // 
            // textBoxCommander
            // 
            textBoxCommander.Location = new Point(130, 152);
            textBoxCommander.Multiline = true;
            textBoxCommander.Name = "textBoxCommander";
            textBoxCommander.Size = new Size(380, 60);
            textBoxCommander.TabIndex = 9;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(324, 251);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(90, 30);
            btnOK.TabIndex = 10;
            btnOK.Text = "Створити";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(420, 251);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 30);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Скасувати";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // labelLocation
            // 
            labelLocation.AutoSize = true;
            labelLocation.Location = new Point(12, 225);
            labelLocation.Name = "labelLocation";
            labelLocation.Size = new Size(53, 15);
            labelLocation.TabIndex = 12;
            labelLocation.Text = "Локація:";
            // 
            // comboBoxLocation
            // 
            comboBoxLocation.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxLocation.FormattingEnabled = true;
            comboBoxLocation.Location = new Point(130, 222);
            comboBoxLocation.Name = "comboBoxLocation";
            comboBoxLocation.Size = new Size(250, 23);
            comboBoxLocation.TabIndex = 13;
            // 
            // NewDutyOrderDialog
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(532, 301);
            Controls.Add(comboBoxLocation);
            Controls.Add(labelLocation);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(textBoxCommander);
            Controls.Add(labelCommander);
            Controls.Add(dateTimeEnd);
            Controls.Add(labelEnd);
            Controls.Add(dateTimeStart);
            Controls.Add(labelStart);
            Controls.Add(dateTimeOrderDate);
            Controls.Add(labelDate);
            Controls.Add(textBoxOrderNumber);
            Controls.Add(labelOrderNumber);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewDutyOrderDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Новий добовий наряд";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelOrderNumber;
        private System.Windows.Forms.TextBox textBoxOrderNumber;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.DateTimePicker dateTimeOrderDate;
        private System.Windows.Forms.Label labelStart;
        private System.Windows.Forms.DateTimePicker dateTimeStart;
        private System.Windows.Forms.Label labelEnd;
        private System.Windows.Forms.DateTimePicker dateTimeEnd;
        private System.Windows.Forms.Label labelCommander;
        private System.Windows.Forms.TextBox textBoxCommander;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelLocation;
        private System.Windows.Forms.ComboBox comboBoxLocation;
    }
}
