using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sistema.Generales;

namespace Sistema.ComputosElectorales
{
    public partial class MainComputosElectorales : Form
    {
        private MsgBox msgBox;

        public MainComputosElectorales()
        {
            InitializeComponent();
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.Show();
                this.MdiParent.WindowState = FormWindowState.Normal;
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnRecuentoVotos_Click(object sender, EventArgs e)
        {
            try
            {
                RecuentoVotos form = new RecuentoVotos();
                form.MdiParent = this.MdiParent;
                form.Dock = DockStyle.Fill;
                form.FormClosed += Form_FormClosed;
                form.Show();
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }
    }
}
