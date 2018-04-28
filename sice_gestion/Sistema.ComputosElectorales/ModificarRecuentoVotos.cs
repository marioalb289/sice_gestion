using Cyotek.Windows.Forms;
using Sistema.DataModel;
using Sistema.Generales;
using Sistema.RegistroActas.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema.ComputosElectorales
{
    public partial class ModificarRecuentoVotos : Form
    {
        private Image _previewImage;
        private List<SeccionCasillaConsecutivo> sc;
        private ComputosElectoralesGenerales CompElec;
        private int flagCombo = 0;
        Image imageLoad;
        string nameImageLoad = "";
        private int flagSelectSupuesto = 0;
        private MsgBox msgBox;
        private PictureBox[] pictureBoxes;
        private TextBox[] textBoxes;
        private Panel[] panels;
        private Label[] labelsName;
        private Loading Loadingbox;
        private int distritoActual = 0;
        private int idCasillaActual = 0;
        private int totalCandidatos;
        private int PosActual = 0;
        private int Lnominal = 0;
        private int totalVotos = 0;
        private bool recuento = false;
        private int boletasRecibidas = 0;
        private List<sice_ar_supuestos> supuestos;
        const int SB_HORZ = 0;
        [DllImport("user32.dll")]

        static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        public ModificarRecuentoVotos()
        {

            //this.MdiParent.WindowState = FormWindowState.Maximized;
            InitializeComponent();

        }
        private void ModificarRecuentoVotos_Load(object sender, EventArgs e)
        {
            //this.cargarComboSeccion();

            this.btnGuardar.Enabled = false;
            this.btnNoConta.Enabled = false;
            //this.btnReserva.Enabled = false;

            txtBoletasR.KeyPress += FrmRegistroActas_KeyPress;
            txtBoletasR.KeyUp += Evento_KeyUp;
            txtBoletasR.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtBoletasR.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtBoletasR.Leave += new System.EventHandler(tbxValue_Leave);

            txtBoletasS.KeyPress += FrmRegistroActas_KeyPress;
            txtBoletasS.KeyUp += Evento_KeyUp;
            txtBoletasS.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtBoletasS.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtBoletasS.Leave += new System.EventHandler(tbxValue_Leave);
        }

        private void cargarComboSeccion()
        {
            try
            {
                cmbSeccion.DataSource = null;
                cmbSeccion.DisplayMember = "Seccion";
                cmbSeccion.ValueMember = "Seccion";
                CompElec = new ComputosElectoralesGenerales();
                if (this.sc == null)
                {
                    this.sc = CompElec.ListaSescciones();
                }
                var seGp = sc.GroupBy(x => x.seccion, x => x.id, (seccion, idSe) => new { IdSeccion = idSe, Seccion = seccion }).Select(g => g.Seccion).ToList();
                cmbSeccion.DataSource = seGp;
                cmbSeccion.Enabled = true;                

                this.cargarComboCasilla();

                cmbSupuesto.DataSource = null;
                cmbSupuesto.DisplayMember = "Supuesto";
                cmbSupuesto.ValueMember = "id";
                if (this.supuestos == null)
                {
                    this.supuestos = CompElec.ListaSupuestos();
                    this.supuestos.Insert(0, new sice_ar_supuestos() { id = 0, supuesto = "Seleccionar Motivo" });
                }
                cmbSupuesto.DataSource = this.supuestos;

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void cargarComboCasilla()
        {
            try
            {
                if (sc != null)
                {
                    cmbCasilla.DataSource = null;
                    cmbCasilla.DisplayMember = "casilla";
                    cmbCasilla.ValueMember = "id";
                    var caGp = (from p in this.sc where p.seccion == Convert.ToInt32(cmbSeccion.SelectedValue) select p).ToList();
                    if (caGp.Count > 0)
                        this.distritoActual = caGp[0].distrito;
                    caGp.Insert(0, new SeccionCasillaConsecutivo() { id = 0, casilla = "Seleccionar Casilla" });
                    cmbCasilla.DataSource = caGp;
                    cmbCasilla.Enabled = true;
                    //cmbCasilla.SelectedIndex = 1;
                }
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void verificarCasilla()
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                int res = CompElec.verificarCasillaRegistrada(Convert.ToInt32(cmbCasilla.SelectedValue));
                if (res == 0)
                {
                    msgBox = new MsgBox(this, "Casilla NO Registrada", "Atención", MessageBoxButtons.OK, "Advertencia");
                    msgBox.ShowDialog(this);
                    cmbCasilla.SelectedIndex = 0;
                    this.btnGuardar.Enabled = false;

                }
                else
                {
                    this.btnGuardar.Enabled = true;
                    this.ClearDataTable(); //Limpia tabla y carga lista de resultados

                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AsginarCasilla()
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                if (this.sc == null)
                {
                    this.sc = CompElec.ListaSescciones();
                    this.PosActual = 0;
                }

                if (PosActual + 1 > this.sc.Count)
                    throw new Exception("No hay mas Casillas disponibles");

                SeccionCasillaConsecutivo tempSec = this.sc[PosActual];
                this.lblConsecutivo.Text = tempSec.consecutivo.ToString();
                //this.lblSeccion.Text = tempSec.seccion.ToString();
                //this.lblCasilla.Text = tempSec.casilla;
                this.distritoActual = tempSec.distrito;
                this.lblListaNominal.Text = tempSec.listaNominal.ToString();
                this.Lnominal = tempSec.listaNominal;
                this.idCasillaActual = tempSec.id;
                this.lblDistrito.Text = tempSec.distrito.ToString();

                this.ClearDataTable();

                this.PosActual++;

                this.btnGuardar.Enabled = true;
                this.btnNoConta.Enabled = true;
                //this.btnReserva.Enabled = true;
                //this.btnSiguiente.Enabled = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool buscarRecuento()
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                if (CompElec.verificarRecuento(this.idCasillaActual) == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void guardarRegistroVotos()
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                int selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                this.panelCaptura.Enabled = false;
                List<sice_votos> lista_votos = new List<sice_votos>();
                int id_casilla = this.idCasillaActual;
                int totalVotacionEmitida = 0;
                if (this.flagSelectSupuesto == 2)
                    throw new Exception("El total de Captura excede la Lista Nominal");
                if (id_casilla == 0)
                    throw new Exception("Error al guardar los datos");
                foreach (TextBox datos in this.textBoxes)
                {
                    double num;

                    if (double.TryParse(datos.Text, out num))
                    {
                        //Es numero proceder guardar
                        int? id_candidato = null;
                        string tipo_voto = "VOTO";
                        int tempIdCandidato = Convert.ToInt32(datos.Tag);
                        if (tempIdCandidato > 0)
                        {
                            id_candidato = tempIdCandidato;
                        }
                        else if (tempIdCandidato == 0)
                        {
                            tipo_voto = "NULO";
                        }
                        else
                        {
                            tipo_voto = "NO REGISTRADO";
                        }
                        lista_votos.Add(new sice_votos()
                        {
                            id_candidato = id_candidato,
                            id_casilla = id_casilla,
                            votos = Convert.ToInt32(datos.Text),
                            tipo = tipo_voto
                        });
                        totalVotacionEmitida += Convert.ToInt32(datos.Text);

                    }
                    else
                    {
                        throw new Exception("Solo se Permiten Numeros");
                    }

                }
                if (lista_votos.Count > 0)
                {                    
                    int res2 = CompElec.guardarDatosVotos(lista_votos, id_casilla, this.totalCandidatos,true,selectedSupuesto);
                    if (res2 == 1)
                    {
                        this.panelCaptura.Enabled = true;
                        msgBox = new MsgBox(this, "Datos Guardados correctamente", "Atención", MessageBoxButtons.OK, "Ok");
                        msgBox.ShowDialog(this);
                        this.BloquearControles();
                    }
                    else
                    {
                        throw new Exception("Error al guardar Datos");
                    }                    
                }
                else
                {
                    throw new Exception("Error al guardar Datos");
                }
            }
            catch (Exception ex)
            {
                this.panelCaptura.Enabled = true;
                throw ex;
            }
        }

        private void ReservarCasilla(string motivo)
        {
            try
            {
                string mensaje = motivo == "NO CONTABILIZABLE" ? "Acta marcada como NO CONTABILIZABLE" : "Acta enviada a Reserva";
                int id_casilla = this.idCasillaActual;
                if (id_casilla == 0)
                    throw new Exception("Error al Reservar Casilla");
                CompElec = new ComputosElectoralesGenerales();
                if (CompElec.CasillaReserva(id_casilla, motivo) == 1)
                {
                    msgBox = new MsgBox(this, "Datos Guardados correctamente.\n" + mensaje, "Atención", MessageBoxButtons.OK, "Ok");
                    msgBox.ShowDialog(this);

                    this.BloquearControles();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cargarCandidatosResultados()
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                idCasillaActual = Convert.ToInt32(cmbCasilla.SelectedValue);
                if (this.idCasillaActual == 0)
                    throw new Exception("No se pudo cargar lista de Candidatos");
                int totalVotos = 0;
                this.recuento = this.buscarRecuento();
                List<CandidatosVotos> lsCandidatosVotos = CompElec.ListaResultadosCasilla(Convert.ToInt32(cmbCasilla.SelectedValue), "sice_votos");
                this.totalCandidatos = lsCandidatosVotos.Count();
                if (lsCandidatosVotos != null)
                {
                    this.totalCandidatos = lsCandidatosVotos.Count();
                    this.cmbSupuesto.Enabled = true;
                    this.boletasRecibidas = lsCandidatosVotos.Count();


                    this.pictureBoxes = new PictureBox[lsCandidatosVotos.Count];
                    this.textBoxes = new TextBox[lsCandidatosVotos.Count];
                    this.panels = new Panel[lsCandidatosVotos.Count];
                    this.labelsName = new Label[lsCandidatosVotos.Count];
                    this.tablePanelPartidos.RowCount = 1;
                    this.btnGuardar.Enabled = true;
                    this.btnNoConta.Enabled = true;
                    cmbSupuesto.Enabled = true;

                    sice_ar_supuestos supuesto = CompElec.getSupuesto(Convert.ToInt32(cmbCasilla.SelectedValue));
                    if (supuesto != null)
                        cmbSupuesto.SelectedIndex = supuesto.id;
                    else
                        cmbSupuesto.SelectedIndex = 0;

                    SeccionCasillaConsecutivo tempSec = (from p in this.sc where p.id == Convert.ToInt32(cmbCasilla.SelectedValue) select p).FirstOrDefault();
                    this.lblListaNominal.Text = tempSec.listaNominal.ToString();
                    this.lblDistrito.Text = tempSec.distrito.ToString();
                    this.Lnominal = tempSec.listaNominal;
                    this.boletasRecibidas = this.Lnominal + (lsCandidatosVotos.Count()-2 * 2); //Lista nominal + 2 veces el numero de representantes de casillas
                    this.txtBoletasR.Text = this.boletasRecibidas.ToString();
                    this.txtBoletasR.Enabled = false;

                    //Agregar Columnas
                    this.tablePanelPartidos.AutoScroll = true;
                    this.tablePanelPartidos.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                    this.tablePanelPartidos.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
                    this.tablePanelPartidos.ColumnCount = totalCandidatos;
                    decimal anchoColumnas = Math.Round(100 / (Convert.ToDecimal(totalCandidatos)), 6);
                    for (int i = 0; i < totalCandidatos; i++)
                    {
                        this.tablePanelPartidos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float)anchoColumnas));
                        //this.tablePanelPartidos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
                    }

                    //Agregar Imagen, Etiqueta, TextBox por fila
                    for (int i = 0; i < lsCandidatosVotos.Count; i++)
                    {

                        pictureBoxes[i] = new PictureBox();
                        textBoxes[i] = new TextBox();
                        labelsName[i] = new Label();
                        panels[i] = new Panel();

                        //Imagen
                        pictureBoxes[i].Anchor = System.Windows.Forms.AnchorStyles.None;
                        pictureBoxes[i].Image = (lsCandidatosVotos[i].tipo == "NULO") ? (System.Drawing.Image)(Properties.Resources.nulos1) : (lsCandidatosVotos[i].tipo == "NO REGISTRADO") ? (System.Drawing.Image)(Properties.Resources.no_regis) : (System.Drawing.Image)(Properties.Resources.pri);
                        pictureBoxes[i].Location = new System.Drawing.Point(125, 8);
                        pictureBoxes[i].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                        pictureBoxes[i].Name = "pictureBox" + i;
                        pictureBoxes[i].Size = new System.Drawing.Size(49, 70);
                        pictureBoxes[i].SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                        pictureBoxes[i].TabIndex = 20 + i;
                        pictureBoxes[i].TabStop = false;

                        //Etiqueta
                        labelsName[i].Dock = System.Windows.Forms.DockStyle.Fill;
                        labelsName[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        labelsName[i].Location = new System.Drawing.Point(910, 86);
                        labelsName[i].Name = "labelNameCandidato" + i;
                        labelsName[i].Size = new System.Drawing.Size(68, 65);
                        labelsName[i].TabIndex = 51;
                        labelsName[i].Text = lsCandidatosVotos[i].tipo == "NULO" ? "Votos Nulos" : lsCandidatosVotos[i].tipo == "NO REGISTRADO" ? "Candidato No Registrado" : lsCandidatosVotos[i].candidato;

                        //TextBox
                        textBoxes[i].Dock = System.Windows.Forms.DockStyle.Top;
                        textBoxes[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        textBoxes[i].Location = new System.Drawing.Point(46, 157);
                        textBoxes[i].Name = "textBox" + i;
                        textBoxes[i].Size = new System.Drawing.Size(63, 29);
                        textBoxes[i].TabIndex = 1 + i;
                        textBoxes[i].Tag = lsCandidatosVotos[i].id_candidato.ToString();
                        textBoxes[i].KeyPress += FrmRegistroActas_KeyPress;
                        textBoxes[i].KeyUp += Evento_KeyUp;
                        textBoxes[i].GotFocus += new System.EventHandler(tbxValue_GotFocus);
                        textBoxes[i].MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
                        textBoxes[i].Leave += new System.EventHandler(tbxValue_Leave);
                        textBoxes[i].MaxLength = 3;
                        textBoxes[i].Text = lsCandidatosVotos[i].votos.ToString();
                        textBoxes[i].TextAlign = HorizontalAlignment.Center;

                        //Agregar Imagen
                        this.tablePanelPartidos.Controls.Add(pictureBoxes[i], i, 0);
                        //Agregar Etiqueta
                        this.tablePanelPartidos.Controls.Add(labelsName[i], i, 1);
                        //Agregar Textbox
                        this.tablePanelPartidos.Controls.Add(textBoxes[i], i, 2);


                    }

                    //Agregar Filas
                    this.tablePanelPartidos.RowCount = 3;
                    this.tablePanelPartidos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
                    this.tablePanelPartidos.RowStyles.Add(new System.Windows.Forms.RowStyle());
                    this.tablePanelPartidos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                    //this.tablePanelPartidos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
                    //this.tblPanaelPartidos.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
                    this.tablePanelPartidos.ResumeLayout(false);
                    this.tablePanelPartidos.Visible = true;
                    this.tblPanelBoletas.Visible = true;
                    this.txtBoletasS.Focus();
                    //textBoxes[0].Focus();
                    //ShowScrollBar(this.tableLayoutPanel2.Handle, SB_HORZ, false);

                    this.btnGuardar.Enabled = true;
                    this.btnNoConta.Enabled = true;
                    this.lblTotalCapturado.Text = totalVotos.ToString();
                    this.VerificarTotal();
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }

        }

        private void ClearDataTable(bool soloBloq = false)
        {
            try
            {
                this.tablePanelPartidos.Visible = false;
                this.tablePanelPartidos.Controls.Clear();
                this.tablePanelPartidos.RowStyles.Clear();
                this.tablePanelPartidos.ColumnStyles.Clear();
                this.tablePanelPartidos.RowCount = 0;
                this.tablePanelPartidos.ColumnCount = 0;
                this.tablePanelPartidos.SuspendLayout();
                this.tblPanelBoletas.Visible = false;
                this.txtBoletasR.Text = "0";
                this.txtBoletasS.Text = "0";
                this.boletasRecibidas = 0;

                if (!soloBloq)
                {
                    this.cargarCandidatosResultados();
                }

                else
                {
                    this.tablePanelPartidos.ResumeLayout();
                    this.tablePanelPartidos.Visible = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BloquearControles()
        {
            this.ClearDataTable(true);
            this.btnGuardar.Enabled = false;
            this.btnNoConta.Enabled = false;
            this.lblConsecutivo.Text = "No.";
            this.lblListaNominal.Text = "No.";
            this.lblDistrito.Text = "No.";
            this.lblTotalCapturado.Text = "No.";
            this.cmbSupuesto.SelectedIndex = 0;
            //this.cargarComboSeccion();


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

        private void VerificarTotal()
        {
            try
            {
                double totalVotos = 0;
                this.flagSelectSupuesto = 0;
                List<double> listaVotos = new List<double>();
                double votosNulos = 0;
                int flagError = 0;
                double boletasSobrantes = 0;
                double.TryParse(this.txtBoletasS.Text, out boletasSobrantes);
                this.txtBoletasS.Text = boletasSobrantes.ToString();
                foreach (TextBox datos in this.textBoxes)
                {
                    double num;
                    int tempIdCandidato = Convert.ToInt32(datos.Tag);//Identificador para votos nulos
                    if (double.TryParse(datos.Text, out num))
                    {
                        totalVotos = totalVotos + num;
                        listaVotos.Add(num);
                        if (tempIdCandidato == -2)
                            votosNulos = num;
                    }
                    else
                    {
                        datos.Text = "0";
                        listaVotos.Add(0);
                        if (tempIdCandidato == 0)
                            votosNulos = 0;
                    }

                    //Numero de Votos Nulos

                    if (tempIdCandidato == 0)
                        votosNulos = num;

                    if (totalVotos + boletasSobrantes > Convert.ToDouble(this.boletasRecibidas))
                    {
                        flagError = 1;
                        //datos.Text = "0";
                    }
                    double totales = totalVotos + boletasSobrantes;
                    lblTotalCapturado.Text = totalVotos.ToString() + "  +  " + boletasSobrantes + "  =  " + totales;


                }
                this.totalVotos = Convert.ToInt32(totalVotos + boletasSobrantes);
                if (flagError > 0)
                {
                    //this.flagSelectSupuesto = 4;
                    //this.cmbSupuesto.SelectedIndex = 4;
                    this.btnGuardar.Enabled = false;
                    //this.cmbSupuesto.Enabled = false;
                    //this.DesactivarTextBoxes();
                    msgBox = new MsgBox(this, "El total de Captura excede el Número de Boletas recibidas", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                    return;
                }
                //this.cmbSupuesto.Enabled = true;
                this.btnGuardar.Enabled = true;
                //int selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                //if (selectedSupuesto == 5 || selectedSupuesto == 4)
                //{
                //    this.cmbSupuesto.SelectedIndex = 0;
                //}



            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void Evento_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Back)
            {
                this.VerificarTotal();
            }
            else if (e.KeyData == Keys.Enter || e.KeyData == Keys.Space)
            {
                return;
            }
            else
            {
                this.VerificarTotal();
            }

        }

        private void FrmRegistroActas_KeyPress(object sender, KeyPressEventArgs e)
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
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ClearDataTable(true);
            this.Close();
        }

        private void ModificarRecuentoVotos_Shown(object sender, EventArgs e)
        {
            this.MdiParent.WindowState = FormWindowState.Maximized;
            this.cargarComboSeccion();
        }

        private void btnNoConta_Click(object sender, EventArgs e)
        {
            try
            {
                msgBox = new MsgBox(this.MdiParent, "¿Marcar la Casilla como NO CONTABILIZABLE?", "Atención", MessageBoxButtons.YesNo, "Advertencia");
                DialogResult result = msgBox.ShowDialog(this);
                if (result == DialogResult.Yes)
                {
                    this.ReservarCasilla("NO CONTABILIZABLE");
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                msgBox = new MsgBox(this.MdiParent, "¿Guardar datos del Acta?", "Atención", MessageBoxButtons.YesNo, "Advertencia");
                DialogResult result = msgBox.ShowDialog(this);
                if (result == DialogResult.Yes)
                {
                    this.guardarRegistroVotos();
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void cmbCasilla_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                int? selected = Convert.ToInt32(cmbCasilla.SelectedValue);
                if (selected != null && selected != 0)
                {
                    this.verificarCasilla();
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void cmbSeccion_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.btnGuardar.Enabled = false;
                this.btnNoConta.Enabled = false;
                this.lblTotalCapturado.Text = "No.";
                this.lblConsecutivo.Text = "No.";
                this.lblDistrito.Text = "No.";
                this.lblListaNominal.Text = "No.";
                this.ClearDataTable(true);
                this.cargarComboCasilla();

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }
    }
}
