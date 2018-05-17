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
    public partial class MainRegistroLocal : Form
    {
        private MsgBox msgBox;

        public MainRegistroLocal()
        {
            InitializeComponent();
            this.cargar();
        }
        public void cargar()
        {
            try
            {
                switch (LoginInfo.privilegios)
                {
                    case 5:
                        this.btnIdentificar.Enabled = true;
                        this.btnRegistrarActas.Enabled = true;
                        break;
                    default:
                        this.btnIdentificar.Enabled = false;
                        this.btnRegistrarActas.Enabled = false;
                        this.btnModificar.Enabled = false;
                        break;
                }
                //asignar la imagen aqui

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }


        }

        private void btnRegistrarActas_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                RegistroActas form3 = new RegistroActas();
                form3.MdiParent = this.MdiParent;
                form3.Dock = DockStyle.Fill;
                form3.FormClosed += Form_FormClosed;
                form3.Show();

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
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

        private void btnConsultarActas_Click(object sender, EventArgs e)
        {

            this.Hide();


            ConsultaActas form = new ConsultaActas();
            form.Load += Form_Load;
            form.MdiParent = this.MdiParent;
            form.Dock = DockStyle.Fill;
            form.Show();
            form.FormClosed += Form_FormClosed;


            //this.MdiParent.Show();
            //this.MdiParent.WindowState = FormWindowState.Maximized;
        }

        private void Form_Load(object sender, EventArgs e)
        {

            this.MdiParent.WindowState = FormWindowState.Maximized;
            //this.MdiParent.Show();
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            this.Hide();

            Reportes form = new Reportes();
            form.Load += Form_Load;
            form.MdiParent = this.MdiParent;
            form.Dock = DockStyle.Fill;
            form.Show();
            form.FormClosed += Form_FormClosed;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.MdiParent.Close();
            this.MdiParent.Dispose();
        }

        private void btnIdentificar_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainIdentificarActas form3 = new MainIdentificarActas();
            form3.MdiParent = this.MdiParent;
            form3.Dock = DockStyle.Fill;
            form3.FormClosed += Form_FormClosed;
            form3.Show();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            this.Hide();
            ModificarRegistroActas form3 = new ModificarRegistroActas();
            form3.MdiParent = this.MdiParent;
            form3.Dock = DockStyle.Fill;
            form3.FormClosed += Form_FormClosed;
            form3.Show();
        }

        private void btnConfRecuento_Click(object sender, EventArgs e)
        {
            this.Hide();
            ConfiguracionRecuento form3 = new ConfiguracionRecuento();
            form3.MdiParent = this.MdiParent;
            form3.Dock = DockStyle.Fill;
            form3.FormClosed += Form_FormClosed;
            form3.Show();
        }
    }
}
