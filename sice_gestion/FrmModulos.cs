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
using Sistema.RegistroActas;

namespace sice_gestion
{
    public partial class FrmModulos : Form
    {
        private MsgBox msgBox;

        public FrmModulos()
        {
            InitializeComponent();
            this.Permisos();
        }

        public void Permisos()
        {
            try
            {
                //throw new Exception("Pribando");
                if(LoginInfo.privilegios < 4)
                {
                    this.btnRegistroActas.Enabled = true;
                    this.btnComputos.Enabled = false;
                }
            }
            catch(Exception ex)
            {                
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog();
            }
        }

        private void btnRegistroActas_Click(object sender, EventArgs e)
        {
            Sistema.RegistroActas.MainRegistroActas form = new Sistema.RegistroActas.MainRegistroActas();
            form.MdiParent = this.MdiParent;
            form.Dock = DockStyle.Fill;
            form.Show();
        }
    }
}
