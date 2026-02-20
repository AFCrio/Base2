namespace Base2.Forms
{
    partial class AssignmentDialog
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxPerson;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxWeapon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxVehicle;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericAmmoCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxAmmoType;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox checkBoxWeapon;
        private System.Windows.Forms.CheckBox checkBoxVehicle;
        private System.Windows.Forms.CheckBox checkBoxAmmo;

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
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxPerson = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxWeapon = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxVehicle = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericAmmoCount = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxAmmoType = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkBoxWeapon = new System.Windows.Forms.CheckBox();
            this.checkBoxVehicle = new System.Windows.Forms.CheckBox();
            this.checkBoxAmmo = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericAmmoCount)).BeginInit();
            this.SuspendLayout();

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Особа:";

            // comboBoxPerson
            this.comboBoxPerson.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPerson.FormattingEnabled = true;
            this.comboBoxPerson.Location = new System.Drawing.Point(12, 35);
            this.comboBoxPerson.Name = "comboBoxPerson";
            this.comboBoxPerson.Size = new System.Drawing.Size(460, 23);
            this.comboBoxPerson.TabIndex = 1;

            // checkBoxWeapon
            this.checkBoxWeapon.AutoSize = true;
            this.checkBoxWeapon.Location = new System.Drawing.Point(12, 75);
            this.checkBoxWeapon.Name = "checkBoxWeapon";
            this.checkBoxWeapon.Size = new System.Drawing.Size(63, 19);
            this.checkBoxWeapon.TabIndex = 2;
            this.checkBoxWeapon.Text = "Зброя";
            this.checkBoxWeapon.UseVisualStyleBackColor = true;
            this.checkBoxWeapon.CheckedChanged += new System.EventHandler(this.checkBoxWeapon_CheckedChanged);

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Зброя:";

            // comboBoxWeapon
            this.comboBoxWeapon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWeapon.Enabled = false;
            this.comboBoxWeapon.FormattingEnabled = true;
            this.comboBoxWeapon.Location = new System.Drawing.Point(32, 120);
            this.comboBoxWeapon.Name = "comboBoxWeapon";
            this.comboBoxWeapon.Size = new System.Drawing.Size(440, 23);
            this.comboBoxWeapon.TabIndex = 4;

            // checkBoxAmmo
            this.checkBoxAmmo.AutoSize = true;
            this.checkBoxAmmo.Location = new System.Drawing.Point(12, 160);
            this.checkBoxAmmo.Name = "checkBoxAmmo";
            this.checkBoxAmmo.Size = new System.Drawing.Size(67, 19);
            this.checkBoxAmmo.TabIndex = 5;
            this.checkBoxAmmo.Text = "Набої";
            this.checkBoxAmmo.UseVisualStyleBackColor = true;
            this.checkBoxAmmo.CheckedChanged += new System.EventHandler(this.checkBoxAmmo_CheckedChanged);

            // label4
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 185);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Кількість:";

            // numericAmmoCount
            this.numericAmmoCount.Enabled = false;
            this.numericAmmoCount.Location = new System.Drawing.Point(32, 205);
            this.numericAmmoCount.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numericAmmoCount.Name = "numericAmmoCount";
            this.numericAmmoCount.Size = new System.Drawing.Size(120, 23);
            this.numericAmmoCount.TabIndex = 7;

            // label5
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(170, 185);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "Тип набоїв:";

            // textBoxAmmoType
            this.textBoxAmmoType.Enabled = false;
            this.textBoxAmmoType.Location = new System.Drawing.Point(170, 205);
            this.textBoxAmmoType.Name = "textBoxAmmoType";
            this.textBoxAmmoType.Size = new System.Drawing.Size(302, 23);
            this.textBoxAmmoType.TabIndex = 9;
            this.textBoxAmmoType.Text = "5,45 мм";

            // checkBoxVehicle
            this.checkBoxVehicle.AutoSize = true;
            this.checkBoxVehicle.Location = new System.Drawing.Point(12, 245);
            this.checkBoxVehicle.Name = "checkBoxVehicle";
            this.checkBoxVehicle.Size = new System.Drawing.Size(88, 19);
            this.checkBoxVehicle.TabIndex = 10;
            this.checkBoxVehicle.Text = "Транспорт";
            this.checkBoxVehicle.UseVisualStyleBackColor = true;
            this.checkBoxVehicle.CheckedChanged += new System.EventHandler(this.checkBoxVehicle_CheckedChanged);

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 270);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Транспорт:";

            // comboBoxVehicle
            this.comboBoxVehicle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxVehicle.Enabled = false;
            this.comboBoxVehicle.FormattingEnabled = true;
            this.comboBoxVehicle.Location = new System.Drawing.Point(32, 290);
            this.comboBoxVehicle.Name = "comboBoxVehicle";
            this.comboBoxVehicle.Size = new System.Drawing.Size(440, 23);
            this.comboBoxVehicle.TabIndex = 12;

            // btnOK
            this.btnOK.Location = new System.Drawing.Point(316, 330);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 30);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);

            // btnCancel
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(397, 330);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Скасувати";
            this.btnCancel.UseVisualStyleBackColor = true;

            // AssignmentDialog
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(484, 372);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.comboBoxVehicle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxVehicle);
            this.Controls.Add(this.textBoxAmmoType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericAmmoCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkBoxAmmo);
            this.Controls.Add(this.comboBoxWeapon);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxWeapon);
            this.Controls.Add(this.comboBoxPerson);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssignmentDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Додати призначення";
            ((System.ComponentModel.ISupportInitialize)(this.numericAmmoCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
