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
            this.cargar();
            
        }

        private void cargar()
        {
            try
            {
                switch (LoginInfo.privilegios)
                {
                    case 4:
                        btnRecuentoVotos.Enabled = false;
                        btnCasillasReserva.Enabled = false;
                        btnCasillasRecuento.Enabled = false;
                        btnModificar.Visible = false;
                        btnConfRecuento.Enabled = false;
                        btnRespaldo.Enabled = false;
                        btnImportarRespaldo.Visible = false;
                        break;
                    case 5:
                        btnRecuentoVotos.Enabled = true;
                        btnCasillasReserva.Enabled = true;
                        btnCasillasRecuento.Enabled = true;
                        btnModificar.Visible = false;
                        btnConfRecuento.Enabled = true;
                        btnRespaldo.Enabled = true;
                        btnImportarRespaldo.Visible = false;
                        break;
                    case 6:
                        btnRecuentoVotos.Enabled = false;
                        btnCasillasReserva.Enabled = false;
                        btnCasillasRecuento.Enabled = true;
                        btnModificar.Visible = false;
                        btnConfRecuento.Enabled = false;
                        btnRespaldo.Enabled = false;
                        btnImportarRespaldo.Visible = false;
                        break;
                    case 7:
                        btnRecuentoVotos.Enabled = true;
                        btnCasillasReserva.Enabled = true;
                        btnCasillasRecuento.Enabled = true;
                        btnModificar.Visible = true;
                        btnConfRecuento.Enabled = true;
                        btnRespaldo.Enabled = true;
                        btnImportarRespaldo.Visible = true;
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

        private void btnReportes_Click(object sender, EventArgs e)
        {
            try
            {
                //Reportes form = new Reportes();
                Form1 form = new Form1();
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

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.MdiParent.Close();
            this.MdiParent.Dispose();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                ModificarRecuentoVotos form = new ModificarRecuentoVotos();
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

        private void MainComputosElectorales_Load(object sender, EventArgs e)
        {
            try
            {
                ComputosElectoralesGenerales CompElec = new ComputosElectoralesGenerales(); 
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
            
        }

        private void btnConfRecuento_Click(object sender, EventArgs e)
        {
            try
            {
                ConfiguracionRecuento form = new ConfiguracionRecuento();
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

        private void btnRespaldo_Click(object sender, EventArgs e)
        {
            try
            {                
                btnRespaldo.Enabled = false;
                ((MDIMainComputosElectorales)this.MdiParent).GenerarExcel(0, false, "RESPALDO");

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnImportarRespaldo_Click(object sender, EventArgs e)
        {
            try
            {
                btnRespaldo.Enabled = false;
                ((MDIMainComputosElectorales)this.MdiParent).GenerarExcel(0, false, "RESPALDO");

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnCasillasReserva_Click(object sender, EventArgs e)
        {
            try
            {
                frmReserva form = new frmReserva(true);
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

        private void btnCasillasRecuento_Click(object sender, EventArgs e)
        {
            frmRecuento form = new frmRecuento(false);
            form.MdiParent = this.MdiParent;
            form.Dock = DockStyle.Fill;
            form.FormClosed += Form_FormClosed;
            form.Show();
        }
    }
}
