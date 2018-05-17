﻿using System;
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
                        btnModificar.Visible = false;
                        btnRecuentoVotos.Enabled = false;
                        break;
                    case 5:
                        btnModificar.Visible = false;
                        break;
                    case 6:
                        btnRecuentoVotos.Enabled = false;
                        btnConsultarActas.Enabled = false;
                        break;
                    case 7:
                        btnRecuentoVotos.Enabled = false;
                        btnConsultarActas.Enabled = false;
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

        private void btnConsultarActas_Click(object sender, EventArgs e)
        {
            try
            {
                frmReserva form = new frmReserva();
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
                Reportes form = new Reportes();
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
    }
}
