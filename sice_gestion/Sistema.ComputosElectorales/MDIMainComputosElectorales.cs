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
using System.Threading;

namespace Sistema.ComputosElectorales
{
    public partial class MDIMainComputosElectorales : Form
    {
        private MsgBox msgBox;
        private ComputosElectoralesGenerales CompElec;
        delegate void DelegateOcultar(int res);
        delegate void DelegateOcultarExcel(int res, bool completo);

        public MDIMainComputosElectorales()
        {
            InitializeComponent();
            this.InicializarComputos();
        }
        public void InicializarComputos()
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                CompElec.InicializarComputos();
            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }
        private void MDIMain_Load(object sender, EventArgs e)
        {
            this.lblUsuario.Text = LoginInfo.nombre_formal;
            MainComputosElectorales mod = new MainComputosElectorales();
            mod.MdiParent = this;
            mod.Dock = DockStyle.Fill;
            mod.ControlBox = false;
            mod.Show();
            //this.RunWatchFile();
        }

        private void ProcesoDescargaDatos(int distrito)
        {
            try
            {

                Thread.Sleep(5000);
                CompElec = new ComputosElectoralesGenerales();
                int res = CompElec.DescargarDatos(distrito);

                if (this.IsDisposed)
                {
                    switch (res)
                    {
                        case 0:
                            MessageBox.Show("Hubo un error en la descarga de arhvios. Intentalo de nuevo", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 1:
                            MessageBox.Show("Descarga Completa", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case 2:
                            MessageBox.Show("No hay datos Recientes para descargar", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;
                    }
                }
                else
                {
                    DelegateOcultar MD = new DelegateOcultar(showMesage);
                    this.Invoke(MD, new object[] { res });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar Datos. Intentalo de nuevo", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void DescargarDatosLocal(int selected)
        {
            try
            {

                //Creamos el delegado 
                //this.pnlDescarga.Visible = true;
                lblDescargando.Visible = true;
                pictureDownload.Visible = true;
                ThreadStart delegado = new ThreadStart(() => ProcesoDescargaDatos(selected));
                //Creamos la instancia del hilo 
                Thread hilo = new Thread(delegado) { IsBackground = true };
                //Iniciamos el hilo 
                hilo.Start();
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void ProcesoGeneraExcel(int distrito, bool completo, SaveFileDialog fichero)
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                int res = CompElec.generarExcel(fichero, distrito, completo);

                if (this.IsDisposed)
                {
                    switch (res)
                    {
                        case 0:
                            MessageBox.Show("Se produjo un error al Generar el archivo. Intentalo de nuevo. \nSi el problema persiste notifique al administrador del sistema", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 1:
                            MessageBox.Show("Archivo en Excel generado correctamente", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                    }
                }
                else
                {
                    DelegateOcultarExcel MD = new DelegateOcultarExcel(showMesageExcel);
                    this.Invoke(MD, new object[] { res, completo });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar Datos. Intentalo de nuevo", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void GenerarExcel(int selected, bool completo = false)
        {
            try
            {
                DateTime localDate = DateTime.Now;
                string date = localDate.ToString("MM-dd-yyyy_HH-mm-ss");
                string namefile = (completo) ? "Reporte_Excel_Completo_" + date : "Reporte_Excel_Distrito_" + selected + "_" + date;
                SaveFileDialog fichero = new SaveFileDialog();
                fichero.Filter = "Excel (*.xlsx)|*.xlsx";
                fichero.FileName = "Reporte_Excel_Distrito_" + selected + "_" + date;
                if (fichero.ShowDialog() == DialogResult.OK)
                {
                    //Creamos el delegado 
                    lblGenerarExcel.Visible = true;
                    pictureExcel.Visible = true;
                    ThreadStart delegado = new ThreadStart(() => ProcesoGeneraExcel(selected, completo, fichero));
                    //Creamos la instancia del hilo 
                    Thread hilo = new Thread(delegado) { IsBackground = true };
                    //Iniciamos el hilo 
                    hilo.Start();
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void showMesage(int res)
        {
            try
            {
                this.lblDescargando.Visible = false;
                this.pictureDownload.Visible = false;
                Form active = this.ActiveMdiChild;
                string formname = active.Name.ToString();
                if (formname == "Reportes")
                {
                    BuscarControl(active.Controls, "btnDescargar");
                }
                switch (res)
                {
                    case 0:
                        msgBox = new MsgBox(this, "Hubo un error en la descarga de arhvios. Intentalo de nuevo", "Atención", MessageBoxButtons.OK, "Error");
                        msgBox.ShowDialog(this);
                        break;
                    case 1:
                        msgBox = new MsgBox(this, "Descarga Completa", "Atención", MessageBoxButtons.OK, "Ok");
                        msgBox.ShowDialog(this);
                        break;
                    case 2:
                        msgBox = new MsgBox(this, "No hay datos Recientes para descargar", "Atención", MessageBoxButtons.OK, "Advertencia");
                        msgBox.ShowDialog(this);
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al Activar controles");
            }

        }

        private void showMesageExcel(int res, bool completo)
        {
            try
            {
                this.lblGenerarExcel.Visible = false;
                this.pictureExcel.Visible = false;
                Form active = this.ActiveMdiChild;
                string formname = active.Name.ToString();
                if (formname == "Reportes")
                {
                    BuscarControl(active.Controls, (completo) ? "btnGenerarExcelTodo" : "btnGenerarExcel");
                }
                switch (res)
                {
                    case 0:
                        msgBox = new MsgBox(this, "Se produjo un error al Generar el archivo.Intentalo de nuevo. \nSi el problema persiste notifique al administrador del sistema", "Atención", MessageBoxButtons.OK, "Error");
                        msgBox.ShowDialog(this);
                        break;
                    case 1:
                        msgBox = new MsgBox(this, "Archivo en Excel generado correctamente", "Atención", MessageBoxButtons.OK, "Ok");
                        msgBox.ShowDialog(this);
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al Activar controles");
            }

        }
        private void BuscarControl(Control.ControlCollection controles, string nameControlBuscar)
        {
            try
            {
                foreach (Control item in controles)
                {
                    string name = item.Name.ToString();
                    if (name == nameControlBuscar)
                    {
                        item.Enabled = true;
                        break;
                    }

                    if (item.HasChildren)
                        BuscarControl(item.Controls, nameControlBuscar);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
