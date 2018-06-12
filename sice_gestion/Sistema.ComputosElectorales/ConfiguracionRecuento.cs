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
        private int totalCasillasRecuento = 0;
        private int puntos_recuento = 0;
        private string tipo_recuento;
        private DetallesComputos detalle;
        public ConfiguracionRecuento()
        {
            InitializeComponent();
            this.cargarComboDistrito();
            this.CargarDatos();

            txtHoras.KeyPress += FrmConfiguracionRecuento_KeyPressDecimal;
            txtHoras.KeyUp += Evento_KeyUp;
            txtHoras.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtHoras.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtHoras.Leave += new System.EventHandler(tbxValue_Leave);

            txtGrupos.KeyPress += FrmConfiguracionRecuento_KeyPress;
            txtGrupos.KeyUp += Evento_KeyUp;
            txtGrupos.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtGrupos.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtGrupos.Leave += new System.EventHandler(tbxValue_Leave);
        }

        private void cargarComboDistrito()
        {
            try
            {
                ComputosElectoralesGenerales CompElec = new ComputosElectoralesGenerales();
                List<sice_distritos_locales> ds = CompElec.ListaDistritos();
                //ds.Insert(1, new sice_distritos_locales() { id = 0, distrito = "TODOS" });
                cmbDistritos.SelectedValueChanged -= cmbDistritos_SelectedValueChanged;
                cmbDistritos.DataSource = null;
                cmbDistritos.DisplayMember = "romano";
                cmbDistritos.ValueMember = "id";
                cmbDistritos.DataSource = ds;
                cmbDistritos.SelectedIndex = 0;
                cmbDistritos.Enabled = true;
                cmbDistritos.SelectedValueChanged += cmbDistritos_SelectedValueChanged;


            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void CargarDatos()
        {
            try
            {

                //DateTime fecha1 = new DateTime(2018,7, 8, 8, 0, 0);
                //DateTime fecha2 = new DateTime(2018, 7, 11, 0, 0, 0);
                //double horasRestantes = Math.Floor( (fecha2 - fecha1).TotalHours );
                lblHorasDisponibles.Text = "0";
                int id_distrito = Convert.ToInt32(cmbDistritos.SelectedValue);
                ComputosElectoralesGenerales CompElec = new ComputosElectoralesGenerales();


                List<CandidatosResultados> lsCandidatos = CompElec.ListaResultadosCandidatos(id_distrito, true);
                double diferenciaPorcentajeTotal = 0;
                //listaSumaCandidatos.OrderBy(x => x.votos);
                int TotalVotosDistrito = lsCandidatos.Sum(x => (int)x.votos);
                lsCandidatos = lsCandidatos.Select(data => new CandidatosResultados
                {
                    id_candidato = data.id_candidato,
                    partido = data.partido,
                    candidato = data.candidato,
                    votos = data.votos,
                    tipo = data.tipo
                }).Where(x => x.tipo == "VOTO").OrderByDescending(x => x.votos).ToList();

               
                if (lsCandidatos.Count > 0)
                {

                    int PrimeroTotal = (int)lsCandidatos[0].votos;
                    int SeegundoTotal = (int)lsCandidatos[1].votos;
                    int diferenciaTotal = PrimeroTotal - SeegundoTotal;

                    if (TotalVotosDistrito > 0)
                    {
                        diferenciaPorcentajeTotal = Math.Round(((double)diferenciaTotal * 100) / TotalVotosDistrito, 2);
                        if (diferenciaPorcentajeTotal < 0.5)
                        {
                            lblDiferencia.Text = diferenciaPorcentajeTotal + "%";
                            lblTipoRecuento.Text = "TOTAL";
                            this.tipo_recuento = "TOTAL";
                        }
                        else
                        {
                            lblDiferencia.Text = diferenciaPorcentajeTotal + "%";
                            lblTipoRecuento.Text = "PARCIAL";
                            this.tipo_recuento = "PARCIAL";
                        }
                    }
                    
                }
                else
                {
                    lblDiferencia.Text = diferenciaPorcentajeTotal + "%";
                    this.tipo_recuento = "PARCIAL";
                    lblTipoRecuento.Text = "PARCIAL";
                }

                sice_configuracion_recuento conf = CompElec.Configuracion_Recuento("SICE", id_distrito);
                this.detalle = CompElec.DetalleComputos(id_distrito);
                if (conf != null)
                {
                    cmbDistritos.SelectedValueChanged -= cmbDistritos_SelectedValueChanged;
                    txtHoras.Text = conf.horas_disponibles.ToString();
                    txtGrupos.Text = conf.grupos_trabajo.ToString();
                    txtHoras.Text = conf.horas_disponibles.ToString();
                    cmbDistritos.SelectedValue = conf.id_distrito;
                    cmbDistritos.SelectedValueChanged += cmbDistritos_SelectedValueChanged;
                    this.tipo_recuento = conf.tipo_recuento;
                    //txtPropietarios.Text = conf.no_consejeros.ToString();
                    //this.tipo_recuento = "TOTAL";
                    if (this.tipo_recuento == "TOTAL")
                    {
                        //this.totalCasillasRecuento = 21;
                        this.totalCasillasRecuento = CompElec.ListaCasillasRecuentos(id_distrito, true,true).Count();
                        this.lblTotalCasillas.Text = this.totalCasillasRecuento.ToString();
                    }
                    else
                    {
                        this.totalCasillasRecuento = CompElec.ListaCasillasRecuentos(id_distrito, false).Count();
                        this.lblTotalCasillas.Text = this.totalCasillasRecuento.ToString();
                    }

                }
                else
                {
                    //this.tipo_recuento = "TOTAL";
                    if (this.tipo_recuento == "TOTAL")
                    {
                        //this.totalCasillasRecuento = 21;
                        this.totalCasillasRecuento = CompElec.ListaCasillasRecuentos(id_distrito, true).Count();
                        this.lblTotalCasillas.Text = this.totalCasillasRecuento.ToString();
                    }
                    else
                    {
                        this.totalCasillasRecuento = CompElec.ListaCasillasRecuentos(id_distrito, false).Count();
                        this.lblTotalCasillas.Text = this.totalCasillasRecuento.ToString();
                    }
                }

                


                

                ValidarCampos();


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
                int grupos_trabajo = 0;
                if (Convert.ToInt32(cmbDistritos.SelectedValue) == 0)
                {
                    throw new Exception("Selecciona un Distrito");
                }
                if (double.TryParse(txtHoras.Text, out num))
                {
                    horas = num;
                }
                else
                {
                    throw new Exception("Solo se Permiten números");
                }
                if (horas <= 0)
                {
                    throw new Exception("El numero de horas debe ser mayor a 0");
                }
                if (double.TryParse(txtGrupos.Text, out num))
                {
                    grupos_trabajo = Convert.ToInt32(num);
                    if (grupos_trabajo <= 0 || grupos_trabajo > 5)
                        throw new Exception("El número de Grupos de Trabajo debe ser minímo 1 y Máximo 5");
                }
                else
                {
                    throw new Exception("Solo se Permiten números");
                }
                if (this.puntos_recuento < 1 || this.puntos_recuento > 8)
                {
                    throw new Exception("El número de Puntos de Recuento debe ser minímo 1 y Máximo 8. \nVerifique la configuración");
                }

                if(this.tipo_recuento == "TOTAL")
                {
                    int totalComputado = detalle.total_capturado + detalle.total_no_conta + detalle.total_recuento + detalle.total_reserva;
                    if(totalComputado == detalle.total_actas)
                    {
                        msgBox = new MsgBox(this.MdiParent, "ADVERTENCIA ESTA POR ENVIAR A RECUENTO TOTAL EL DISTRITO ACTUAL\nLos cambios no se pueden deshacer\n¿Continuar?", "Atención", MessageBoxButtons.YesNo, "Advertencia");
                        DialogResult result = msgBox.ShowDialog(this);
                        if (result == DialogResult.No)
                            return;
                        
                    }
                    else
                    {
                        msgBox = new MsgBox(this, "Para enviar a RECUENTO TOTAL SE DEBE REGISTRAR TODAS LAS ACTAS", "Advertencia", MessageBoxButtons.OK, "Advertencia");
                        msgBox.ShowDialog(this);
                        return;
                    }
                }


                ComputosElectoralesGenerales CompElec = new ComputosElectoralesGenerales();
                if (CompElec.GuardarConfiguracionRecuento(horas, Convert.ToInt32(cmbDistritos.SelectedValue), grupos_trabajo, this.puntos_recuento, this.tipo_recuento) == 1)
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

        public int Round(double numero)
        {
            if (numero < 1.0)
                return 1;
            double decimalpoints = Math.Abs(numero - Math.Floor(numero));
            if (decimalpoints > 0.30)
                return (int)Math.Floor(numero) + 1;
            else
                return (int)Math.Floor(numero);
        }

        private void LimpiarDatos()
        {
            txtGrupos.Text = "1";
            txtHoras.Text = "0";
            lblNcr.Text = "0";
            lblGt.Text = "0";
            lblSegmento.Text = "0";
            lblPr.Text = "0";
            lblPrDecimal.Text = "0";
        }


        private void ValidarCampos(object sender = null)
        {
            try
            {

                TextBox textBox = null;
                if (sender != null)
                    textBox = (TextBox)sender;
                if (textBox != null && textBox.Text == "")
                {
                    textBox.Text = "1";
                    textBox.SelectAll();
                }

                this.puntos_recuento = 0;

                if (this.totalCasillasRecuento <= 20)
                {
                    txtGrupos.Text = "1";
                    lblNcr.Text = "0";
                    lblGt.Text = "0";
                    lblSegmento.Text = "0";
                    lblPr.Text = "NO APLICA";
                    lblPrDecimal.Text = "0";
                    return;
                }

                int grupos_trabajo = (txtGrupos.Text == "") ? 0 : Convert.ToInt32(txtGrupos.Text);
                if (grupos_trabajo <= 0 || grupos_trabajo > 5)
                {
                    txtGrupos.Text = "1";
                    grupos_trabajo = 1;
                    msgBox = new MsgBox(this, "El número de Grupos de Trabajo debe ser Mínimo 1 Máximo 5", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                }
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

                if (grupos_trabajo <= 0)
                {
                    lblGt.Text = "0";
                    return;
                }
                else
                {
                    lblGt.Text = grupos_trabajo.ToString();
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
                double parcialPuntoRecuento = (((double)this.totalCasillasRecuento / (double)grupos_trabajo) / (double)segmentos);
                this.puntos_recuento = this.Round(parcialPuntoRecuento);
                if (puntos_recuento <= 0)
                {
                    lblPrDecimal.Text = "0";
                    lblPr.Text = "0";
                }
                else
                {
                    lblPrDecimal.Text = (Math.Truncate(parcialPuntoRecuento * 100) / 100).ToString();
                    lblPr.Text = puntos_recuento.ToString() + "PR";
                }


            }
            catch (Exception ex)
            {
                txtGrupos.Text = "1";
                txtHoras.Text = "0";
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

        private void cmbDistritos_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                LimpiarDatos();
                this.CargarDatos();
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }
    }
}
