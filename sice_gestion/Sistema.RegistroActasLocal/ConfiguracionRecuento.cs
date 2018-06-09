using Sistema.DataModel;
using Sistema.Generales;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema.RegistroActasLocal
{
    public partial class ConfiguracionRecuento : Form
    {
        private MsgBox msgBox;
        private int totalCasillasRecuento = 0;
        private int horas_disponibles = 0;
        public ConfiguracionRecuento()
        {
            InitializeComponent();
            this.CargarDatos();

            txtHoras.KeyPress += FrmConfiguracionRecuento_KeyPressDecimal;
            txtHoras.KeyUp += Evento_KeyUp;
            txtHoras.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtHoras.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtHoras.Leave += new System.EventHandler(tbxValue_Leave);

            txtPropietarios.KeyPress += FrmConfiguracionRecuento_KeyPress;
            txtPropietarios.KeyUp += Evento_KeyUp;
            txtPropietarios.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtPropietarios.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtPropietarios.Leave += new System.EventHandler(tbxValue_Leave);

            txtSuplentes.KeyPress += FrmConfiguracionRecuento_KeyPress;
            txtSuplentes.KeyUp += Evento_KeyUp;
            txtSuplentes.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtSuplentes.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtSuplentes.Leave += new System.EventHandler(tbxValue_Leave);
        }

        private void CargarDatos()
        {
            try
            {

                //DateTime fecha1 = new DateTime(2018,7, 8, 8, 0, 0);
                //DateTime fecha2 = new DateTime(2018, 7, 11, 0, 0, 0);
                //double horasRestantes = Math.Floor( (fecha2 - fecha1).TotalHours );
                this.horas_disponibles = 0;
                lblHorasDisponibles.Text = "0";

                RegistroLocalGenerales reg = new RegistroLocalGenerales();
                this.totalCasillasRecuento = reg.ListaCasillasRecuentos(0, true).Count();
                this.lblTotalCasillas.Text = this.totalCasillasRecuento.ToString();
                sice_configuracion_recuento conf = reg.Configuracion_Recuento("RA");
                if(conf != null)
                {
                    txtHoras.Text = conf.horas_disponibles.ToString();
                    txtPropietarios.Text = conf.no_consejeros.ToString();
                    txtSuplentes.Text = conf.no_suplentes.ToString();
                   
                }
                ValidarCampos();

            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void GuardarDatos()
        {
            try
            {
                double num;
                double horas = 0;
                int propietarios = 0;
                int suplentes = 0;
                if (double.TryParse(txtHoras.Text, out num))
                {
                    horas = num;
                }
                else
                {
                    throw new Exception("Solo se Permiten números");
                }
                if (double.TryParse(txtPropietarios.Text, out num))
                {
                    propietarios = Convert.ToInt32( num);
                    if (propietarios != 5)
                        throw new Exception("El número de Consejeros Propietarios debe ser 5");
                }
                else
                {
                    throw new Exception("Solo se Permiten números");
                }
                if (double.TryParse(txtSuplentes.Text, out num))
                {
                    suplentes = Convert.ToInt32(num);
                    if (suplentes < 1 || suplentes > 4)
                        throw new Exception("El número de Consejeros Suplentes debe ser minímo 1 y Máximo 4");
                }
                else
                {
                    throw new Exception("Solo se Permiten números");
                }
                RegistroLocalGenerales reg = new RegistroLocalGenerales();
                if(reg.GuardarConfiguracionRecuento(horas,propietarios,suplentes) == 1)
                {
                    msgBox = new MsgBox(this, "Datos Guardados correctamente", "Atención", MessageBoxButtons.OK, "Ok");
                    msgBox.ShowDialog(this);
                }
                else
                {
                    throw new Exception("Error al guardar Datos");
                }

            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        public int Round(double numero)
        {
            if(numero < 1.0)
                return 1;
            double decimalpoints = Math.Abs(numero - Math.Floor(numero));
            if (decimalpoints > 0.30)
                return (int)Math.Floor(numero) + 1;
            else
                return (int)Math.Floor(numero);
        }


        private void ValidarCampos(object sender = null)
        {
            try
            {

                //TextBox textBox = null;
                //if (sender != null)
                //    textBox = (TextBox)sender;
                //if ( (textBox != null && textBox.Text == "") || (textBox != null && textBox.Text == "."))
                //{
                //    textBox.Text = "0";
                //    textBox.SelectAll();
                //    //return;
                //}

                if(this.totalCasillasRecuento <= 20)
                {
                    txtPropietarios.Text = "5";
                    txtSuplentes.Text = "1";
                    lblNcr.Text = "0";
                    lblGt.Text = "0";
                    lblSegmento.Text = "0";
                    lblPr.Text = "NO APLICA";
                    lblPrDecimal.Text = "0";
                    return;
                }

                int propietarios = (txtPropietarios.Text == "") ?  0 : Convert.ToInt32( txtPropietarios.Text);                
                if (propietarios != 5)
                {
                    txtPropietarios.Text = "5";
                    propietarios = 5;
                    msgBox = new MsgBox(this, "El número de Consejeros Propietarios debe ser 5", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                }
                

                int suplentes = (txtSuplentes.Text == "" ) ? 0 : Convert.ToInt32(txtSuplentes.Text);
                if (suplentes < 1 || suplentes > 4)
                {
                    txtSuplentes.Text = "1";
                    suplentes = 1;
                    msgBox = new MsgBox(this, "El número de Consejeros Suplentes debe ser minímo 1 y Máximo 4", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                }
                

                this.lblGruposFormula.Text = "GT = ( "+propietarios+" + "+suplentes+") - 3 = ";

                int grupos_tabajo = (propietarios - 3) + suplentes;
                this.lblGruposFormula.Text = "GT = ( " + propietarios + " + " + suplentes + ") - 3 = " +grupos_tabajo;
                if (grupos_tabajo > 5)
                    grupos_tabajo = 5;
                int segmentos = (txtHoras.Text == "") ? 0 : Convert.ToInt32(txtHoras.Text);

                if (segmentos > 0)
                {
                    lblHorasDisponibles.Text = segmentos.ToString();
                }
                else
                {
                    lblHorasDisponibles.Text = "0";
                    lblPrDecimal.Text = "0";
                    lblPr.Text = "0";
                    return;
                }
                segmentos = segmentos * 2;                 
                
                if (totalCasillasRecuento <= 0)
                {
                    lblNcr.Text = "0";
                    return;
                }
                else
                {
                    lblNcr.Text = totalCasillasRecuento.ToString();
                }
                    
                if (grupos_tabajo <= 0)
                {
                    lblGt.Text = "0";
                    return;
                }
                else
                {
                    lblGt.Text = grupos_tabajo.ToString();
                }
                    
                if (segmentos <= 0)
                {
                    lblSegmento.Text = "0";
                    return;
                }
                else
                {
                    lblSegmento.Text = segmentos.ToString();
                }

                //this.totalCasillasRecuento = 315;
                double parcialPuntoRecuento = (((double)this.totalCasillasRecuento / (double)grupos_tabajo) / (double)segmentos);
                int puntos_recuento = this.Round(parcialPuntoRecuento);
                if (puntos_recuento <= 0)
                {
                    lblPrDecimal.Text = "0";
                    lblPr.Text = "0";
                }
                else
                {
                    lblPrDecimal.Text = (Math.Truncate(parcialPuntoRecuento * 100) / 100).ToString();
                    lblPr.Text = puntos_recuento.ToString()+ "PR";
                }
                    

            }
            catch(Exception ex)
            {
                txtPropietarios.Text = "5";
                txtSuplentes.Text = "1";
                lblNcr.Text = "0";
                lblGt.Text = "0";
                lblSegmento.Text = "0";
                lblPr.Text = "0";
                lblPrDecimal.Text = "0";
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
            
        }

        private void FrmConfiguracionRecuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;

            }
            else if (Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsSeparator(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void FrmConfiguracionRecuento_KeyPressDecimal(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;

            }
            else if (Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsSeparator(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Evento_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Back)
            {
                this.ValidarCampos(sender);
            }
            else if (e.KeyData == Keys.Enter || e.KeyData == Keys.Space)
            {
                return;
            }
            else
            {
                this.ValidarCampos(sender);
            }

        }

        private bool selectAllOnFocus = true;
        private bool selectAllDone = false;

        void tbxValue_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (selectAllOnFocus && !selectAllDone && textBox.SelectionLength == 0)
            {
                selectAllDone = true;
                textBox.SelectAll();
            }
        }

        void tbxValue_GotFocus(object sender, System.EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (selectAllOnFocus && MouseButtons == MouseButtons.None)
            {
                textBox.SelectAll();
                selectAllDone = true;
            }
        }

        void tbxValue_Leave(object sender, System.EventArgs e)
        {
            selectAllDone = false;
        }

        ///
        /// Set to true to select all contents of the textbox when the box receives focus by clicking it with the mouse
        ///
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Set to true to select all contents of the textbox when the box receives focus by clicking it with the mouse")]
        public bool SelectAllOnFocus
        {
            get { return selectAllOnFocus; }
            set { selectAllOnFocus = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            this.GuardarDatos();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
