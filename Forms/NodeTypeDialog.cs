using Base2.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Base2.Forms
{
    public partial class NodeTypeDialog : Form
    {
        public NodeType SelectedNodeType { get; private set; }

        private Dictionary<NodeType, string> _nodeTypeDescriptions = new()
        {
            { NodeType.SectionHeader, "Заголовок секції (корінь нумерації, можна прив'язати до локації)" },
            { NodeType.SimplePosition, "Одна особа (HasWeapon/HasVehicle за прапорцями)" },
            { NodeType.DriverPosition, "Водій з транспортом" },
            { NodeType.GroupInline, "Група, список в рядок" },
            { NodeType.GroupNested, "Група з підпунктами" },
            { NodeType.TimeRange, "Часовий діапазон (зміна)" },
            { NodeType.MedicalPosition, "Черговий медпункту" },
            { NodeType.FireGroupSection, "Заголовок секції вогневих груп" },
            { NodeType.FireGroupLocation, "Вогнева група в локації" },
            { NodeType.FireGroupInline, "Конкретна вогнева група inline" }
        };

        public NodeTypeDialog()
        {
            InitializeComponent();
            LoadNodeTypes();
        }

        private void LoadNodeTypes()
        {
            comboBoxNodeType.Items.Clear();

            foreach (NodeType nodeType in Enum.GetValues(typeof(NodeType)))
            {
                comboBoxNodeType.Items.Add(nodeType);
            }

            if (comboBoxNodeType.Items.Count > 0)
            {
                comboBoxNodeType.SelectedIndex = 0;
            }
        }

        private void comboBoxNodeType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (comboBoxNodeType.SelectedItem is NodeType selectedType)
            {
                if (_nodeTypeDescriptions.TryGetValue(selectedType, out string? description))
                {
                    textBoxDescription.Text = description;
                }
            }
        }

        private void btnOK_Click(object? sender, EventArgs e)
        {
            if (comboBoxNodeType.SelectedItem is NodeType selectedType)
            {
                SelectedNodeType = selectedType;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Виберіть тип вузла!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
