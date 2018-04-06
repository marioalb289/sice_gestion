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

namespace Sistema.RegistroActasLocal
{
    public partial class MainIdentificarActas : Form
    {
        private MsgBox msgBox;

        public MainIdentificarActas()
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

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void btnIdentificar_Click(object sender, EventArgs e)
        {
            this.Hide();
            IdentificarActas form3 = new IdentificarActas();
            form3.MdiParent = this.MdiParent;
            form3.Dock = DockStyle.Fill;
            form3.FormClosed += Form_FormClosed;
            form3.Show();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            this.Hide();
            ModificarActas form3 = new ModificarActas();
            form3.MdiParent = this.MdiParent;
            form3.Dock = DockStyle.Fill;
            form3.FormClosed += Form_FormClosed;
            form3.Show();
        }
    }
}
