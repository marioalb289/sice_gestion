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
                if(LoginInfo.privilegios == 1 || LoginInfo.privilegios == 2 || LoginInfo.privilegios == 3 || LoginInfo.privilegios == 5 || LoginInfo.privilegios == 6)
                {
                    this.btnRegistroActas.Enabled = true;
                    this.btnComputos.Enabled = true;
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
            this.MdiParent.Hide();
            if (LoginInfo.privilegios == 5 || LoginInfo.privilegios == 6)
            {
                Sistema.RegistroActasLocal.MDIMainRegistroActas form = new Sistema.RegistroActasLocal.MDIMainRegistroActas();
                form.FormClosed += Form_FormClosed;
                form.Show();
                
            }
            else
            {
                msgBox = new MsgBox(this, "No tienes permisos para acceder", "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog();
            }
            //else if(LoginInfo.privilegios == 1 || LoginInfo.privilegios == 2 || LoginInfo.privilegios == 3 )
            //{
            //    Sistema.RegistroActas.MainRegistroActas form = new Sistema.RegistroActas.MainRegistroActas();
            //    form.MdiParent = this.MdiParent;
            //    form.Dock = DockStyle.Fill;
            //    form.Show();
            //    form.FormClosed += Form_FormClosed;
            //}
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.MdiParent.Show();
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog();
            }
        }

        private void btnComputos_Click(object sender, EventArgs e)
        {
            this.MdiParent.Hide();
            Sistema.ComputosElectorales.MDIMainComputosElectorales form = new Sistema.ComputosElectorales.MDIMainComputosElectorales();
            form.FormClosed += Form_FormClosed;
            form.Show();
            
        }

        private void btnConf_Click(object sender, EventArgs e)
        {
            this.Hide();
            Configuracion form = new Configuracion();
            form.MdiParent = this.MdiParent;
            form.Dock = DockStyle.Fill;
            form.Show();
            form.FormClosed += Form_FormClosed;
        }
    }
}
