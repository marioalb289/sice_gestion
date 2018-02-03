using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema.RegistroActas
{
    public partial class MainRegistroActas : Form
    {
        public MainRegistroActas()
        {
            InitializeComponent();
        }

        private void btnRegistrarActas_Click(object sender, EventArgs e)
        {
            frmRegistroActas form = new frmRegistroActas();
            form.MdiParent = this.MdiParent;
            form.Dock = DockStyle.Fill;
            form.Show();
        }

        private void btnConsultarActas_Click(object sender, EventArgs e)
        {
            frmConsultaActas form = new frmConsultaActas();
            form.MdiParent = this.MdiParent;
            form.Dock = DockStyle.Fill;
            form.Show();
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            Reportes form = new Reportes();
            form.MdiParent = this.MdiParent;
            form.Dock = DockStyle.Fill;
            form.Show();

        }
    }
}
