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

namespace sice_gestion
{
    public partial class Configuracion : Form
    {
        SistemaConfiguracion conf;
        MsgBox msgBox;
        public Configuracion()
        {
            InitializeComponent();
        }

        private void EjecutarProceso()
        {
            try
            {
                conf = new SistemaConfiguracion();
                int res = conf.Inicializar();
                msgBox = new MsgBox(this, "Respuesta: " + res, "Atención", MessageBoxButtons.OK, "Advertencia");
                msgBox.ShowDialog(this);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void btnInicializarTablas_Click(object sender, EventArgs e)
        {
           

            try
            {
                btnInicializarTablas.Enabled = false;
                //Creamos el delegado 
                ThreadStart delegado = new ThreadStart(EjecutarProceso);
                delegado += () => {
                    // Do what you want in the callback
                    this.btnInicializarTablas.Enabled = true;
                };
                //Creamos la instancia del hilo 
                Thread hilo = new Thread(delegado) { IsBackground = true };
                //Iniciamos el hilo 
                hilo.Start();                
                
            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }

        }
    }
}
