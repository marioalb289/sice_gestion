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
                DateTime fechaFinPruebaActa = new DateTime(2018, 6, 28, 0, 0, 0);
                DateTime fechaFinRegistroActas = new DateTime(2018, 7, 8, 8, 0, 0);
                DateTime fechaActual = DateTime.Now;

                if (fechaActual <= fechaFinPruebaActa)
                {
                    switch (LoginInfo.privilegios)
                    {
                        case 4:
                            btnIdentificar.Enabled = false;
                            btnIdentificar.Visible = false;
                            btnRegistrarActas.Enabled = false;
                            btnModificar.Enabled = false;
                            btnConfRecuento.Enabled = false;
                            btnConsultarActas.Enabled = true;
                            btnReportes.Enabled = true;
                            btnRespaldo.Enabled = false;
                            btnImportarRespaldo.Visible = false;
                            break;
                        case 5:
                            btnIdentificar.Enabled = false;
                            btnIdentificar.Visible = false;
                            btnRegistrarActas.Enabled = true;
                            btnModificar.Enabled = true;
                            btnConfRecuento.Enabled = true;
                            btnConsultarActas.Enabled = true;
                            btnReportes.Enabled = true;
                            btnRespaldo.Enabled = true;
                            btnImportarRespaldo.Visible = false;
                            break;
                        case 7:
                            btnIdentificar.Enabled = true;
                            btnRegistrarActas.Enabled = true;
                            btnModificar.Enabled = true;
                            btnConfRecuento.Enabled = true;
                            btnConsultarActas.Enabled = true;
                            btnReportes.Enabled = true;
                            btnRespaldo.Enabled = true;
                            btnImportarRespaldo.Visible = true;
                            break;
                        default:
                            btnIdentificar.Enabled = false;
                            btnIdentificar.Visible = false;
                            btnRegistrarActas.Enabled = false;
                            btnModificar.Enabled = false;
                            btnConfRecuento.Enabled = false;
                            btnConsultarActas.Enabled = true;
                            btnReportes.Enabled = true;
                            btnRespaldo.Enabled = false;
                            btnImportarRespaldo.Visible = false;
                            break;
                    }
                }
                else
                {
                    DateTime fechaActualProduccion = DateTime.Now;
                    if (fechaActualProduccion <= fechaFinRegistroActas)
                    {
                        switch (LoginInfo.privilegios)
                        {
                            case 4:
                                btnIdentificar.Enabled = false;
                                btnIdentificar.Visible = false;
                                btnRegistrarActas.Enabled = false;
                                btnModificar.Enabled = false;
                                btnConfRecuento.Enabled = false;
                                btnConsultarActas.Enabled = true;
                                btnReportes.Enabled = true;
                                btnRespaldo.Enabled = false;
                                btnImportarRespaldo.Visible = false;
                                break;
                            case 5:
                                btnIdentificar.Enabled = false;
                                btnIdentificar.Visible = false;
                                btnRegistrarActas.Enabled = true;
                                btnModificar.Enabled = true;
                                btnConfRecuento.Enabled = true;
                                btnConsultarActas.Enabled = true;
                                btnReportes.Enabled = true;
                                btnRespaldo.Enabled = true;
                                btnImportarRespaldo.Visible = false;
                                break;
                            case 7:
                                btnIdentificar.Enabled = true;
                                btnRegistrarActas.Enabled = true;
                                btnModificar.Enabled = true;
                                btnConfRecuento.Enabled = true;
                                btnConsultarActas.Enabled = true;
                                btnReportes.Enabled = true;
                                btnRespaldo.Enabled = true;
                                btnImportarRespaldo.Visible = true;
                                break;
                            default:
                                btnIdentificar.Enabled = false;
                                btnIdentificar.Visible = false;
                                btnRegistrarActas.Enabled = false;
                                btnModificar.Enabled = false;
                                btnConfRecuento.Enabled = false;
                                btnConsultarActas.Enabled = true;
                                btnReportes.Enabled = true;
                                btnRespaldo.Enabled = false;
                                btnImportarRespaldo.Visible = false;
                                break;
                        }
                    }
                    else
                    {
                        switch (LoginInfo.privilegios)
                        {
                            case 4:
                                btnIdentificar.Enabled = false;
                                btnIdentificar.Visible = false;
                                btnRegistrarActas.Enabled = false;
                                btnModificar.Enabled = false;
                                btnConfRecuento.Enabled = false;
                                btnConsultarActas.Enabled = true;
                                btnReportes.Enabled = true;
                                btnRespaldo.Enabled = false;
                                btnImportarRespaldo.Visible = false;
                                break;
                            case 5:
                                btnIdentificar.Enabled = false;
                                btnIdentificar.Visible = false;
                                btnRegistrarActas.Enabled = false;
                                btnModificar.Enabled = false;
                                btnConfRecuento.Enabled = false;
                                btnConsultarActas.Enabled = true;
                                btnReportes.Enabled = true;
                                btnRespaldo.Enabled = true;
                                btnImportarRespaldo.Visible = false;
                                break;
                            case 7:
                                btnIdentificar.Enabled = true;
                                btnRegistrarActas.Enabled = true;
                                btnModificar.Enabled = true;
                                btnConfRecuento.Enabled = true;
                                btnConsultarActas.Enabled = true;
                                btnReportes.Enabled = true;
                                btnRespaldo.Enabled = true;
                                btnImportarRespaldo.Visible = true;
                                break;
                            default:
                                btnIdentificar.Enabled = false;
                                btnIdentificar.Visible = false;
                                btnRegistrarActas.Enabled = false;
                                btnModificar.Enabled = false;
                                btnConfRecuento.Enabled = false;
                                btnConsultarActas.Enabled = true;
                                btnReportes.Enabled = true;
                                btnRespaldo.Enabled = false;
                                btnImportarRespaldo.Visible = false;
                                break;
                        }
                    }
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

        private void btnRespaldo_Click(object sender, EventArgs e)
        {
            try
            {
                //int? selected = Convert.ToInt32(cmbDistrito.SelectedValue);
                //if (selected > 0 && selected != null)
                //{
                //this.ValidarRecuento();
                btnRespaldo.Enabled = false;
                ((MDIMainRegistroActas)this.MdiParent).GenerarExcel(0, false,"RESPALDO");

                //}

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
                //int? selected = Convert.ToInt32(cmbDistrito.SelectedValue);
                //if (selected > 0 && selected != null)
                //{
                //this.ValidarRecuento();
                btnRespaldo.Enabled = false;
                ((MDIMainRegistroActas)this.MdiParent).ImportarExcel();

                //}

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }
    }
}
