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

namespace Sistema.ComputosElectorales
{
    public partial class ConfiguracionRecuento : Form
    {
        private MsgBox msgBox;
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
                ComputosElectoralesGenerales comp = new ComputosElectoralesGenerales();
                sice_configuracion_recuento conf = comp.Configuracion_Recuento("SICE");
                if (conf != null)
                {
                    txtHoras.Text = conf.horas_disponibles.ToString();
                    txtPropietarios.Text = conf.no_consejeros.ToString();
                    txtSuplentes.Text = conf.no_suplentes.ToString();
                }


            }
            catch (Exception ex)
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
                    propietarios = Convert.ToInt32(num);
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
                ComputosElectoralesGenerales comp = new ComputosElectoralesGenerales();
                if (comp.GuardarConfiguracionRecuento(horas, propietarios, suplentes) == 1)
                {
                    msgBox = new MsgBox(this, "Datos Guardados correctamente", "Atención", MessageBoxButtons.OK, "Ok");
                    msgBox.ShowDialog(this);
                }
                else
                {
                    throw new Exception("Error al guardar Datos");
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }


        private void ValidarCampos(object sender = null)
        {
            try
            {
                TextBox textBox = null;
                if (sender != null)
                    textBox = (TextBox)sender;
                if ((textBox != null && textBox.Text == "") || (textBox != null && textBox.Text == "."))
                {
                    textBox.Text = "0";
                    textBox.SelectAll();
                }

            }
            catch (Exception ex)
            {
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
            else if (e.KeyChar == '.')
            {
                e.Handled = false;
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
    }
}
