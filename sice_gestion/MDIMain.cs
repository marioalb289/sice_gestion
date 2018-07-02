using sice_gestion.Properties;
using Sistema.Generales;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sice_gestion
{
    public partial class MDIMain : Form
    {
        private int childFormNumber = 0;
        private int flagWatcher = 0;
        private MsgBox msgBox;
        SincronizarDatos data;
        SincronizarDatosComputos dataComputos;

        public MDIMain()
        {
            InitializeComponent();
            this.Icon = Resources.logo;
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
            this.lblUsuario.Text = LoginInfo.nombre_formal;
            FrmModulos mod = new FrmModulos();
            mod.MdiParent = this;
            mod.Dock = DockStyle.Fill;
            //mod.ControlBox = false;
            mod.Show();
            //this.RunWatchFile();
            if(LoginInfo.privilegios == 5)
            {
                this.data = new SincronizarDatos();
                this.dataComputos = new SincronizarDatosComputos();
            }
            
        }

        private void MDIMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (LoginInfo.privilegios == 5)
                {
                    this.data.detener();
                    this.dataComputos.detener();
                }

               
                this.Dispose();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
