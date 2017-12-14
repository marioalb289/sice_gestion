using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sistema.RegistroActas;

namespace sice_gestion
{
    public partial class FrmModulos : Form
    {
        public FrmModulos()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sistema.RegistroActas.MainRegistroActas form = new Sistema.RegistroActas.MainRegistroActas();
            form.MdiParent = this.MdiParent;
            form.Dock = DockStyle.Fill;
            form.Show();

        }
    }
}
