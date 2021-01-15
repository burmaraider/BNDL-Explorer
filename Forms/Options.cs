using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BNDL_Explorer.Forms
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        public Form1 parent;

        public void SetParent(Form1 form)
        {
            parent = form;
            pictureBox1.BackColor = parent.opt.GetTexBackgroundColor();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() != DialogResult.Cancel)
            {
                parent.opt.SetTexBackgroundColor(colorDialog1.Color);
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() != DialogResult.Cancel)
            {
                parent.opt.SetTexBackgroundColor(colorDialog1.Color);
                pictureBox1.BackColor = parent.opt.GetTexBackgroundColor();
                parent.Refresh();
            }
        }
    }
}
