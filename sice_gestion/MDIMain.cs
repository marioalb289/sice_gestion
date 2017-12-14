using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sice_gestion
{
    public partial class MDIMain : Form
    {
        private int childFormNumber = 0;

        public MDIMain()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Ventana " + childFormNumber++;
            childForm.Show();
        }        

        private void MDIMain_Load(object sender, EventArgs e)
        {
            FrmModulos mod = new FrmModulos();
            mod.MdiParent = this;
            mod.Dock = DockStyle.Fill;
            //mod.ControlBox = false;
            mod.Show();
        }
    }
}
