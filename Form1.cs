using Base2.Data;
using Base2.Forms;
namespace Base2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var context = new AppDbContext())
            {
                AssignmentDialog assignmentDialog = new AssignmentDialog(context);
                assignmentDialog.ShowDialog();
            }
        }

        private void bOrderForm_Click(object sender, EventArgs e)
        {
            using (var context = new AppDbContext())
            {
                OrderEditorForm orderEditorForm = new OrderEditorForm();
                orderEditorForm.ShowDialog();
            }
        }
    }
}
