using System;
using System.Windows.Forms;

namespace BNDL_Explorer
{
    public partial class AskForm : Form
    {
        private int modalResult = 0;
        public AskForm()
        {
            InitializeComponent();
        }

        public int ShowModal(string title, string question, string s1, bool b1, string s2, bool b2, string s3, bool b3)
        {
            Text = title;

            labelQuestion.Text = question;

            button1.Text = s1;
            button1.Enabled = b1;
            button2.Text = s2;
            button2.Enabled = b2;
            button3.Text = s3;
            button3.Enabled = b3;

            ShowDialog();
            return DialogResult == DialogResult.OK ? modalResult : 0;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            modalResult = 1;
            Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            modalResult = 2;
            Close();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            modalResult = 3;
            Close();
        }
    }
}
