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

namespace Sistema.RegistroActas
{
    public partial class MainRegistroActas : Form
    {
        private MsgBox msgBox;

        public MainRegistroActas()
        {
            InitializeComponent();
            this.cargar();
        }
        public void cargar()
        {
            try
            {
                string mensaje = "";
                string imagen = "";
                switch (LoginInfo.privilegios)
                {
                    case 1:
                        mensaje = "Registrar Actas";
                        imagen = "ImagenActas";
                        break;
                    case 2:
                        mensaje = "Revisión Actas";
                        imagen = "";
                        break;
                    case 3:
                        mensaje = "Cotejo Actas";
                        imagen = "";
                        break;
                    default:
                        mensaje = "Registrar Actas";
                        imagen = "";
                        this.btnRegistrarActas.Enabled = false;
                        break;
                }

                this.btnRegistrarActas.Text = mensaje;
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
                //this.MdiParent.WindowState = FormWindowState.Maximized;
                //this.MdiParent.Hide();
                
                switch (LoginInfo.privilegios)
                {
                    case 1:
                        frmRegistroActas form = new frmRegistroActas();                        
                        form.MdiParent = this.MdiParent;
                        form.Dock = DockStyle.Fill;
                        form.Show();
                        form.FormClosed += Form_FormClosed;
                        break;
                    case 2:
                        RevisionActas form2 = new RevisionActas();
                        form2.MdiParent = this.MdiParent;
                        form2.Dock = DockStyle.Fill;
                        form2.FormClosed += Form_FormClosed;
                        form2.Show();
                        break;
                    case 3:
                        CotejoActas form3 = new CotejoActas();
                        form3.MdiParent = this.MdiParent;
                        form3.Dock = DockStyle.Fill;
                        form3.FormClosed += Form_FormClosed;
                        form3.Show();
                        break;
                }
                //this.MdiParent.WindowState = FormWindowState.Maximized;
                //this.MdiParent.Show();

            }
            catch(Exception ex)
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
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnConsultarActas_Click(object sender, EventArgs e)
        {

            this.Hide();
            

            frmConsultaActas form = new frmConsultaActas();
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
            Reportes form = new Reportes();
            form.MdiParent = this.MdiParent;
            form.Dock = DockStyle.Fill;
            form.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
