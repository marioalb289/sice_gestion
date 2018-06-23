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
    public partial class RecuentoVotos : Form
    {
        private Image _previewImage;
        private List<SeccionCasillaConsecutivo> sc;
        private ComputosElectoralesGenerales CompElec;
        private int flagCombo = 0;
        Image imageLoad;
        string nameImageLoad = "";
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
        private int flagSelectSupuesto = 0;
        private int totalVotos = 0;
        private bool recuento = false;
        private bool reservaConsejo = false;
        private int flagDistritoActual = 0;
        private bool flagEspecialMr = false;
        private bool flagEspecialRp = false;
        private bool flagCasillasRecuento = false;
        const int SB_HORZ = 0;
        private int boletasRecibidas = 0;
        private List<sice_ar_supuestos> supuestos;
        [DllImport("user32.dll")]

        static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        public RecuentoVotos()
        {

            //this.MdiParent.WindowState = FormWindowState.Maximized;
            InitializeComponent();
            this.CargarComboEstatusActaPaqueteIncidenciasSupuestos();

        }
        private void RecuentoVotos_Load(object sender, EventArgs e)
        {
            //this.cargarComboSeccion();

            this.btnGuardar.Enabled = false;

            txtSobrantes.KeyPress += FrmRegistroActas_KeyPress;
            txtSobrantes.KeyUp += Evento_KeyUp;
            txtSobrantes.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtSobrantes.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtSobrantes.Leave += new System.EventHandler(tbxValue_Leave);

            txtPersonasVotaron.KeyPress += FrmRegistroActas_KeyPress;
            txtPersonasVotaron.KeyUp += Evento_KeyUp;
            txtPersonasVotaron.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtPersonasVotaron.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtPersonasVotaron.Leave += new System.EventHandler(tbxValue_Leave);

            txtRepresentantes.KeyPress += FrmRegistroActas_KeyPress;
            txtRepresentantes.KeyUp += Evento_KeyUp;
            txtRepresentantes.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtRepresentantes.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtRepresentantes.Leave += new System.EventHandler(tbxValue_Leave);

            txtVotosSacados.KeyPress += FrmRegistroActas_KeyPress;
            txtVotosSacados.KeyUp += Evento_KeyUp;
            txtVotosSacados.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtVotosSacados.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtVotosSacados.Leave += new System.EventHandler(tbxValue_Leave);

            txtTotalCapturado.KeyPress += TxtPreventCaptura_KeyPress;
            txtBoletasR.KeyPress += TxtPreventCaptura_KeyPress;

        }

        private void AsginarCasilla()
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                txtRepresentantes.Enabled = true;
                if (this.sc == null)
                {
                    this.sc = CompElec.ListaSescciones(true);
                    this.PosActual = 0;
                }
                
                if (PosActual + 1 > this.sc.Count)
                    throw new Exception("No hay mas Casillas disponibles");
                if (flagDistritoActual == 0)
                {
                    flagEspecialMr = false;
                    flagEspecialRp = false;
                    flagCasillasRecuento = false;
                    flagDistritoActual = this.sc[PosActual].distrito;
                    msgBox = new MsgBox(this, "CAPTURANDO DISTRITO " + this.sc[PosActual].distrito, "Atención", MessageBoxButtons.OK, "Informacion");
                    msgBox.ShowDialog(this);
                }
                else if(flagDistritoActual != this.sc[PosActual].distrito)
                {
                    flagEspecialMr = false;
                    flagEspecialRp = false;
                    flagCasillasRecuento = false;
                    flagDistritoActual = this.sc[PosActual].distrito;
                    msgBox = new MsgBox(this, "CAPTURANDO DISTRITO " + this.sc[PosActual].distrito, "Atención", MessageBoxButtons.OK, "Informacion");
                    msgBox.ShowDialog(this);
                }

                if(this.sc[PosActual].casilla == "S1" && !flagEspecialMr)
                {
                    flagEspecialMr = true;
                    msgBox = new MsgBox(this, "CAPTURANDO CASILLA ESPECIAL MR ", "Atención", MessageBoxButtons.OK, "Informacion");
                    msgBox.ShowDialog(this);
                }

                if (this.sc[PosActual].casilla == "S1-RP" && !flagEspecialRp)
                {
                    flagEspecialRp = true;
                    msgBox = new MsgBox(this, "CAPTURANDO CASILLA ESPECIAL RP " , "Atención", MessageBoxButtons.OK, "Informacion");
                    msgBox.ShowDialog(this);
                }

                int casillasRecuento = CompElec.ListaCasillasRecuentos(this.sc[PosActual].distrito, false, false).Count();
                if(casillasRecuento > 20 && !flagCasillasRecuento)
                {
                    msgBox = new MsgBox(this, "EL TOTAL DE CASILLAS A RECUENTO ES: "+casillasRecuento, "Atención", MessageBoxButtons.OK, "Informacion");
                    msgBox.ShowDialog(this);
                }


                SeccionCasillaConsecutivo tempSec = this.sc[PosActual];
                this.lblConsecutivo.Text = tempSec.consecutivo.ToString();
                this.lblSeccion.Text = tempSec.seccion.ToString();             
                this.lblCasilla.Text = tempSec.casilla;
                this.lblDistrito.Text = tempSec.distrito.ToString();
                this.distritoActual = tempSec.distrito;
                this.Lnominal = this.sc[PosActual].casilla == "S1" || this.sc[PosActual].tipo == "RP" ? Configuracion.BoletasEspecial:tempSec.listaNominal;
                this.lblListaNominal.Text = this.Lnominal.ToString();                
                this.idCasillaActual = tempSec.id;                

                this.ClearDataTable();

                

                this.btnGuardar.Enabled = true;
                this.btnSiguiente.Enabled = false;
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void CargarComboEstatusActaPaqueteIncidenciasSupuestos()
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                cmbSupuesto.DataSource = null;
                cmbSupuesto.DisplayMember = "Supuesto";
                cmbSupuesto.ValueMember = "id";
                if (this.supuestos == null)
                {
                    this.supuestos = CompElec.ListaSupuestos();
                    this.supuestos.Insert(0, new sice_ar_supuestos() { id = 0, supuesto = "SIN MOTIVO DE RECUENTO" });
                }
                cmbSupuesto.DataSource = this.supuestos;
                cmbSupuesto.Enabled = false;

                cmbEstatusActa.DataSource = null;
                cmbEstatusActa.DisplayMember = "estatus";
                cmbEstatusActa.ValueMember = "id";
                cmbEstatusActa.DataSource = CompElec.ListaEstatusActa();
                cmbEstatusActa.SelectedValue = 1;

                this.flagCombo = 1;
                //cmbCasilla.SelectedIndex = 1;

                cmbEstatusActa.SelectedValueChanged += cmbEstatusActa_SelectedValueChanged;


            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void guardarRegistroVotos()
        {
            try
            {
                int boletasSobrantes = Convert.ToInt32(txtSobrantes.Text);
                int personas_votaron = Convert.ToInt32(txtPersonasVotaron.Text);
                int votos_sacados = Convert.ToInt32(txtVotosSacados.Text);

                CompElec = new ComputosElectoralesGenerales();
                this.panelCaptura.Enabled = false;

                List<sice_votos> lista_votos = new List<sice_votos>();
                int id_casilla = this.idCasillaActual;
                if (id_casilla == 0)
                    throw new Exception("Error al guardar los datos");

                int totalVotacionEmitida = 0;
                int estatus_acta = Convert.ToInt32(cmbEstatusActa.SelectedValue);

                if (estatus_acta == 1 || estatus_acta == 2 || estatus_acta == 8)
                {
                    if (totalVotos < boletasRecibidas)
                    {
                        if (boletasSobrantes == 0)
                        {
                            if (votos_sacados != boletasRecibidas)
                                throw new Exception("Debes capturar el numero de boletas sobrantes");
                        }
                    }
                    if (personas_votaron == 0)
                        throw new Exception("Debes capturar el numero de personas que votaron");
                    if (votos_sacados == 0)
                        throw new Exception("Debes capturar el numero de votos sacados de la urna");
                    if (!this.VerificarApartados())
                        return;
                }

                int selectedSupuesto = 0;
                if (estatus_acta != 4)
                {
                    selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                    if ((estatus_acta == 3 || estatus_acta == 5))
                    {
                        if (this.recuento)
                            throw new Exception("Esta Casilla ya fue enviada a Recuento.\nNO SE PUEDE ENVIAR A RECUENTO DE NUEVO");
                        if (selectedSupuesto == 0)
                            throw new Exception("Debes seleccionar un Motivo de Recuento");
                    }

                    estatus_acta = Convert.ToInt32(cmbEstatusActa.SelectedValue);

                    if (this.flagSelectSupuesto == 4)
                    {
                        selectedSupuesto = flagSelectSupuesto;
                        //this.panelCaptura.Enabled = true;
                        //msgBox = new MsgBox(this, "El total de Captura excede el Número de Boletas recibidas", "Atención", MessageBoxButtons.OK, "Error");
                        //msgBox.ShowDialog(this);
                        //return;
                        estatus_acta = 5;
                    }
                    else if (this.flagSelectSupuesto == 5)
                    {
                        //this.panelCaptura.Enabled = true;
                        //msgBox = new MsgBox(this, "Número de VOTOS NULOS mayor a la diferencia entre el 1ER y 2DO lugar", "Atención", MessageBoxButtons.OK, "Advertencia");
                        //msgBox.ShowDialog(this);
                        //return;
                        selectedSupuesto = flagSelectSupuesto;
                        estatus_acta = 5;
                    }
                }

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
                        else if (tempIdCandidato == -2)
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
                    int incidencias = 0;
                    int estatus_paquete = 0;

                    int res2 = CompElec.guardarDatosVotos(lista_votos, id_casilla, selectedSupuesto, Convert.ToInt32(txtSobrantes.Text),
                        0, Convert.ToInt32(txtPersonasVotaron.Text), Convert.ToInt32(txtRepresentantes.Text), Convert.ToInt32(txtVotosSacados.Text),
                        incidencias, estatus_acta, estatus_paquete);
                    if (res2 == 1)
                    {
                        //this.tableLayoutPanel2.Enabled = true;
                        this.PosActual++;
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

        private void guardarRegistroVotosRP()
        {
            try
            {
                int boletasSobrantes = Convert.ToInt32(txtSobrantes.Text);
                int personas_votaron = Convert.ToInt32(txtPersonasVotaron.Text);
                int votos_sacados = Convert.ToInt32(txtVotosSacados.Text);

                CompElec = new ComputosElectoralesGenerales();
                this.panelCaptura.Enabled = false;

                List<sice_votos_rp> lista_votos = new List<sice_votos_rp>();
                int id_casilla = this.idCasillaActual;
                if (id_casilla == 0)
                    throw new Exception("Error al guardar los datos");

                int totalVotacionEmitida = 0;
                int estatus_acta = Convert.ToInt32(cmbEstatusActa.SelectedValue);

                if (estatus_acta == 1 || estatus_acta == 2 || estatus_acta == 8)
                {
                    if (totalVotos < boletasRecibidas)
                    {
                        if (boletasSobrantes == 0)
                        {
                            if (votos_sacados != boletasRecibidas)
                                throw new Exception("Debes capturar el numero de boletas sobrantes");
                        }
                    }
                    if (personas_votaron == 0)
                        throw new Exception("Debes capturar el numero de personas que votaron");
                    if (votos_sacados == 0)
                        throw new Exception("Debes capturar el numero de votos sacados de la urna");
                    if (!this.VerificarApartados())
                        return;
                }

                int selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                if ((estatus_acta == 3 || estatus_acta == 5 || estatus_acta == 4))
                {
                    if (this.recuento)
                        throw new Exception("Esta Casilla ya fue enviada a Recuento.\nNO SE PUEDE ENVIAR A RECUENTO DE NUEVO");
                    if (selectedSupuesto == 0)
                        throw new Exception("Debes seleccionar un Motivo de Recuento");
                }

                estatus_acta = Convert.ToInt32(cmbEstatusActa.SelectedValue);
                if (this.flagSelectSupuesto > 0)
                {
                    estatus_acta = 5;
                    selectedSupuesto = this.flagSelectSupuesto;
                    //this.ReservarCasilla("RESERVA", selectedSupuesto);
                    //return;
                }

                foreach (TextBox datos in this.textBoxes)
                {
                    double num;

                    if (double.TryParse(datos.Text, out num))
                    {
                        //Es numero proceder guardar
                        int? id_partido = null;
                        string tipo_voto = "VOTO";
                        int tempIdPartido = Convert.ToInt32(datos.Tag);
                        if (tempIdPartido > 0)
                        {
                            id_partido = tempIdPartido;
                        }
                        else if (tempIdPartido == -2)
                        {
                            tipo_voto = "NULO";
                        }
                        else
                        {
                            tipo_voto = "NO REGISTRADO";
                        }
                        lista_votos.Add(new sice_votos_rp()
                        {
                            id_partido = id_partido,
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
                    int incidencias = 0;
                    int estatus_paquete = 0;

                    int res2 = CompElec.guardarDatosVotosRP(lista_votos, id_casilla, selectedSupuesto, Convert.ToInt32(txtSobrantes.Text),
                        0, Convert.ToInt32(txtPersonasVotaron.Text), Convert.ToInt32(txtRepresentantes.Text), Convert.ToInt32(txtVotosSacados.Text),
                        incidencias, estatus_acta, estatus_paquete);
                    if (res2 == 1)
                    {
                        //this.tableLayoutPanel2.Enabled = true;
                        this.PosActual++;
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
        private bool VerificarApartados()
        {
            try
            {
                int selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                if (this.flagSelectSupuesto > 0)
                    return true;

                string mensaje = "";
                int personas_votaron = Convert.ToInt32(txtPersonasVotaron.Text);
                int representantes = Convert.ToInt32(txtRepresentantes.Text);
                int votos_sacados = Convert.ToInt32(txtVotosSacados.Text);
                int suma = personas_votaron + representantes;
                int totalCapturado = Convert.ToInt32(txtTotalCapturado.Text);
                int totalBoletasRecibidas = Convert.ToInt32(txtBoletasR.Text);
                int sobrantes = Convert.ToInt32(txtSobrantes.Text);
                int sumaSobrantes = suma + sobrantes;


                if (suma != votos_sacados)
                {
                    mensaje = "La Sumatoria de Personas y Representantes que votaron es Diferente de los Votos Sacados de la urna";
                }
                else if (votos_sacados != totalCapturado)
                {
                    mensaje = "El total de Votacion es Diferente de los Votos Sacados de la Urna";
                }
                else if (sumaSobrantes != totalBoletasRecibidas)
                {
                    mensaje = "La Sumatoria de Personas y Representantes que votaron mas Boletas Sobrantes es Diferente de el Numero de boletas Recibidas";
                }

                if (mensaje != "")
                {
                    if (this.recuento)
                    {
                        msgBox = new MsgBox(this, mensaje, "Atención", MessageBoxButtons.OK, "Advertencia");
                        msgBox.ShowDialog(this);
                        this.panelCaptura.Enabled = true;
                        return false;
                    }
                    else
                    {
                        this.cmbSupuesto.SelectedValue = 4;
                        this.cmbEstatusActa.SelectedValue = 5;
                        msgBox = new MsgBox(this.MdiParent, mensaje + "¿Enviar Acta a Recuento?", "Atención", MessageBoxButtons.YesNo, "Advertencia");
                        DialogResult result = msgBox.ShowDialog(this);
                        if (result == DialogResult.Yes)
                        {
                            return true;
                        }
                        else
                        {
                            this.panelCaptura.Enabled = true;
                            cmbSupuesto.SelectedValue = 0;
                            cmbEstatusActa.SelectedValue = 1;
                            return false;
                        }
                    }

                    
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        

        private void ReservarCasilla(string motivo,int? supuesto = null)
        {
            try
            {
                string mensaje = motivo == "NO CONTABILIZABLE" ? "Acta marcada como NO CONTABILIZABLE" : "Acta enviada a Reserva";
                int id_casilla = this.idCasillaActual;
                if (id_casilla == 0)
                    throw new Exception("Error al Reservar Casilla");
                CompElec = new ComputosElectoralesGenerales();
                if (CompElec.CasillaReserva(id_casilla, motivo,supuesto) == 1)
                {
                    msgBox = new MsgBox(this, "Datos Guardados correctamente.\n"+mensaje, "Atención", MessageBoxButtons.OK, "Ok");
                    msgBox.ShowDialog(this);

                    this.BloquearControles();
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void cargarCandidatosResultados()
        {
            try
            {
                if (this.sc[PosActual].tipo == "RP")
                {
                    cargarPartidosPoliticosRp();
                    return;
                }
                    CompElec = new ComputosElectoralesGenerales();
                if (this.distritoActual == 0)
                    throw new Exception("No se pudo cargar lista de Candidatos");
                List<Candidatos> lsCandidatos = CompElec.ListaCandidatos(this.distritoActual);
                this.totalCandidatos = lsCandidatos.Count();
                if (lsCandidatos != null)
                {

                    int TotalRepresentantes = 1;
                    foreach (Candidatos cnd in lsCandidatos)
                    {
                        if (cnd.coalicion != "" && cnd.coalicion != null && cnd.tipo_partido != "COALICION")
                        {
                            TotalRepresentantes += CompElec.RepresentantesCComun(cnd.coalicion);
                        }
                        else if (cnd.tipo_partido != "COALICION")
                        {
                            if (cnd.partido_local == 1)
                                TotalRepresentantes += 1;
                            else if (cnd.partido_local == 0)
                                TotalRepresentantes += 2;
                        }
                    }
                    //if (this.sc[PosActual].casilla == "S1")
                    //    TotalRepresentantes = 0;

                    this.totalCandidatos = lsCandidatos.Count() + 2;
                    //this.cmbSupuesto.Enabled = true;
                    this.boletasRecibidas = lsCandidatos.Count();


                    this.pictureBoxes = new PictureBox[lsCandidatos.Count + 2];
                    this.textBoxes = new TextBox[lsCandidatos.Count + 2];
                    this.panels = new Panel[lsCandidatos.Count + 2];
                    this.labelsName = new Label[lsCandidatos.Count + 2];
                    this.tablePanelPartidos.RowCount = 1;
                    this.btnGuardar.Enabled = true;

                    this.boletasRecibidas = this.Lnominal + TotalRepresentantes; //Lista nominal + 2 veces el numero de representantes de casillas
                    this.txtBoletasR.Text = this.boletasRecibidas.ToString();
                    if (this.sc[PosActual].casilla == "S1")
                    {
                        txtRepresentantes.Enabled = false;
                        this.lblListaNominal.Text = "0";
                    }

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

                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Properties.Resources));
                    //Agregar Imagen, Etiqueta, TextBox por fila
                    for (int i = 0; i < lsCandidatos.Count + 2; i++)
                    {

                        pictureBoxes[i] = new PictureBox();
                        textBoxes[i] = new TextBox();
                        labelsName[i] = new Label();
                        panels[i] = new Panel();

                        //Imagen
                        pictureBoxes[i].Anchor = System.Windows.Forms.AnchorStyles.None;
                        pictureBoxes[i].Image = (i > lsCandidatos.Count - 1) ? (i == lsCandidatos.Count ? (System.Drawing.Image)(Properties.Resources.no_regis) : (System.Drawing.Image)(Properties.Resources.nulos1)) : ((System.Drawing.Image)(resources.GetObject(lsCandidatos[i].imagen)));
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
                        labelsName[i].Text = (i > lsCandidatos.Count - 1) ? i == lsCandidatos.Count ? "Candidato No Registrado" : "Votos Nulos" : lsCandidatos[i].candidato;

                        //TextBox
                        textBoxes[i].Dock = System.Windows.Forms.DockStyle.Top;
                        textBoxes[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        textBoxes[i].Location = new System.Drawing.Point(46, 157);
                        textBoxes[i].Name = "textBox" + i;
                        textBoxes[i].Size = new System.Drawing.Size(63, 29);
                        textBoxes[i].TabIndex = 1 + i;
                        textBoxes[i].Tag = (i > lsCandidatos.Count - 1) ? i == lsCandidatos.Count ? "-1" : "-2" : lsCandidatos[i].id_candidato.ToString();
                        textBoxes[i].KeyPress += FrmRegistroActas_KeyPress;
                        textBoxes[i].KeyUp += Evento_KeyUp;
                        textBoxes[i].GotFocus += new System.EventHandler(tbxValue_GotFocus);
                        textBoxes[i].MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
                        textBoxes[i].Leave += new System.EventHandler(tbxValue_Leave);
                        textBoxes[i].MaxLength = 3;
                        textBoxes[i].Text = "0";
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
                    this.txtSobrantes.Focus();
                    this.panelCaptura.Visible = true;
                    this.panelCaptura.Enabled = true;
                    //textBoxes[0].Focus();
                    //ShowScrollBar(this.tableLayoutPanel2.Handle, SB_HORZ, false);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void cargarPartidosPoliticosRp()
        {
            try
            {
                
                CompElec = new ComputosElectoralesGenerales();

                List<sice_partidos_politicos> lsPartidos = CompElec.ListaPartidosPoliticos();
                if (lsPartidos != null)
                {
                    int totalPartidos = lsPartidos.Count() + 2;
                    //this.cmbSupuesto.Enabled = true;
                    this.boletasRecibidas = lsPartidos.Count();


                    this.pictureBoxes = new PictureBox[lsPartidos.Count + 2];
                    this.textBoxes = new TextBox[lsPartidos.Count + 2];
                    this.panels = new Panel[lsPartidos.Count + 2];
                    this.labelsName = new Label[lsPartidos.Count + 2];
                    this.tablePanelPartidos.RowCount = 1;
                    this.btnGuardar.Enabled = true;

                    this.boletasRecibidas = Configuracion.BoletasEspecial; //Lista nominal + 2 veces el numero de representantes de casillas
                    this.txtBoletasR.Text = this.boletasRecibidas.ToString();
                    txtRepresentantes.Enabled = false;

                    //Agregar Columnas
                    this.tablePanelPartidos.AutoScroll = true;
                    this.tablePanelPartidos.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                    this.tablePanelPartidos.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
                    this.tablePanelPartidos.ColumnCount = totalPartidos;
                    decimal anchoColumnas = Math.Round(100 / (Convert.ToDecimal(totalPartidos)), 6);

                    int TotalRepresentantes = 1;
                    foreach (sice_partidos_politicos cnd in lsPartidos)
                    {
                        if (cnd.info_creado != "" && cnd.info_creado != null && cnd.tipo != "COALICION")
                        {
                            TotalRepresentantes += CompElec.RepresentantesCComun(cnd.info_creado);
                        }
                        else if (cnd.tipo != "COALICION")
                        {
                            if (cnd.local == 1)
                                TotalRepresentantes += 1;
                            else if (cnd.local == 0)
                                TotalRepresentantes += 2;
                        }
                    }
                    for (int i = 0; i < totalPartidos; i++)
                    {
                        this.tablePanelPartidos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float)anchoColumnas));
                        //this.tablePanelPartidos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
                    }

                    this.lblListaNominal.Text = "0";
                    this.boletasRecibidas = this.Lnominal + TotalRepresentantes; //Lista nominal + 2 veces el numero de representantes de casillas
                    this.txtBoletasR.Text = this.boletasRecibidas.ToString();

                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Properties.Resources));
                    //Agregar Imagen, Etiqueta, TextBox por fila
                    for (int i = 0; i < lsPartidos.Count + 2; i++)
                    {

                        pictureBoxes[i] = new PictureBox();
                        textBoxes[i] = new TextBox();
                        labelsName[i] = new Label();
                        panels[i] = new Panel();

                        //Imagen
                        pictureBoxes[i].Anchor = System.Windows.Forms.AnchorStyles.None;
                        pictureBoxes[i].Image = (i > lsPartidos.Count - 1) ? (i == lsPartidos.Count ? (System.Drawing.Image)(Properties.Resources.no_regis) : (System.Drawing.Image)(Properties.Resources.nulos1)) : ((System.Drawing.Image)(resources.GetObject(lsPartidos[i].img_par)));
                        pictureBoxes[i].Location = new System.Drawing.Point(125, 8);
                        pictureBoxes[i].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                        pictureBoxes[i].Name = "pictureBox" + i;
                        pictureBoxes[i].Size = new System.Drawing.Size(49, 70);
                        pictureBoxes[i].SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                        pictureBoxes[i].TabIndex = 20 + i;
                        pictureBoxes[i].TabStop = false;

                        //Etiqueta
                        labelsName[i].Dock = System.Windows.Forms.DockStyle.Fill;
                        labelsName[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        labelsName[i].Location = new System.Drawing.Point(910, 86);
                        labelsName[i].Name = "labelNameCandidato" + i;
                        labelsName[i].Size = new System.Drawing.Size(68, 65);
                        labelsName[i].TabIndex = 51;
                        labelsName[i].TextAlign = ContentAlignment.MiddleCenter;
                        labelsName[i].Text = (i > lsPartidos.Count - 1) ? i == lsPartidos.Count ? "Candidato No Registrado" : "Votos Nulos" : lsPartidos[i].siglas_par;

                        //TextBox
                        textBoxes[i].Dock = System.Windows.Forms.DockStyle.Top;
                        textBoxes[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        textBoxes[i].Location = new System.Drawing.Point(46, 157);
                        textBoxes[i].Name = "textBox" + i;
                        textBoxes[i].Size = new System.Drawing.Size(63, 29);
                        textBoxes[i].TabIndex = 1 + i;
                        textBoxes[i].Tag = (i > lsPartidos.Count - 1) ? i == lsPartidos.Count ? "-1" : "-2" : lsPartidos[i].id.ToString();
                        textBoxes[i].KeyPress += FrmRegistroActas_KeyPress;
                        textBoxes[i].KeyUp += Evento_KeyUp;
                        textBoxes[i].GotFocus += new System.EventHandler(tbxValue_GotFocus);
                        textBoxes[i].MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
                        textBoxes[i].Leave += new System.EventHandler(tbxValue_Leave);
                        textBoxes[i].MaxLength = 3;
                        textBoxes[i].Text = "0";
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
                    this.txtSobrantes.Focus();
                    this.panelCaptura.Visible = true;
                    this.panelCaptura.Enabled = true;
                    //textBoxes[0].Focus();
                    //ShowScrollBar(this.tableLayoutPanel2.Handle, SB_HORZ, false);
                }

            }
            catch (Exception ex)
            {
                throw ex;
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
                this.txtSobrantes.Text = "0";
                this.txtTotalCapturado.Text = "0";
                this.boletasRecibidas = 0;

                if (!soloBloq)
                {
                    this.cargarCandidatosResultados();
                }

                else
                {
                    this.tablePanelPartidos.ResumeLayout();
                    this.tablePanelPartidos.Visible = true;
                    this.panelCaptura.Visible = false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BloquearControles()
        {
            //this.panelCaptura.Enabled = true;
            this.ClearDataTable(true);
            this.btnGuardar.Enabled = false;
            this.btnSiguiente.Enabled = true;

            this.lblCasilla.Text = "----";
            this.lblConsecutivo.Text = "No.";
            this.lblSeccion.Text = "No.";
            this.lblListaNominal.Text = "No.";
            this.lblDistrito.Text = "No.";

            this.txtTotalCapturado.Text = "0";
            this.txtBoletasR.Text = "0";
            this.txtSobrantes.Text = "0";
            this.boletasRecibidas = 0;
            this.txtPersonasVotaron.Text = "0";
            this.txtRepresentantes.Text = "0";
            this.txtVotosSacados.Text = "0";

            this.cmbSupuesto.SelectedValue = 1;
            this.cmbEstatusActa.SelectedValue = 1;



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

        private void VerificarTotal(object sender = null)
        {
            try
            {
                TextBox textBox = null;
                if (sender != null)
                    textBox = (TextBox)sender;
                if (textBox != null && textBox.Text == "")
                {
                    textBox.Text = "0";
                    textBox.SelectAll();
                }

                double totalVotos = 0;
                this.flagSelectSupuesto = 0;
                List<double> listaVotos = new List<double>();
                double votosNulos = 0;
                int flagError = 0;
                double boletasSobrantes = 0;
                double.TryParse(this.txtSobrantes.Text, out boletasSobrantes);
                this.txtSobrantes.Text = boletasSobrantes.ToString();
                foreach (TextBox datos in this.textBoxes)
                {
                    double num;
                    int tempIdCandidato = Convert.ToInt32(datos.Tag);//Identificador para votos nulos
                    if (double.TryParse(datos.Text, out num))
                    {
                        totalVotos = totalVotos + num;
                        double tmpVotosSacados = Convert.ToDouble(txtVotosSacados.Text);
                        if (tmpVotosSacados > 0 && num == tmpVotosSacados)
                        {
                            flagError = 2;
                        }
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
                    txtTotalCapturado.Text = totalVotos.ToString();// + "  +  "+boletasSobrantes+ "  =  " + totales ;


                }
                this.totalVotos = Convert.ToInt32(totalVotos + boletasSobrantes);
                if (flagError == 1)
                {
                    this.flagSelectSupuesto = 4;
                    this.cmbSupuesto.SelectedValue = 4;
                    this.cmbEstatusActa.SelectedValue = 5;
                    //this.cmbSupuesto.Enabled = false;
                    //this.DesactivarTextBoxes();
                    msgBox = new MsgBox(this, "El total de Captura excede el Número de Boletas recibidas", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                    return;
                }
                else if (flagError == 2)
                {
                    this.flagSelectSupuesto = 6;
                    this.cmbSupuesto.SelectedValue = 6;
                    this.cmbEstatusActa.SelectedValue = 5;
                    //this.cmbSupuesto.Enabled = false;
                    //this.DesactivarTextBoxes();
                    msgBox = new MsgBox(this, "TODOS LOS VOTOS A FAVOR DE UN PARTIDO", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                    return;
                }

                listaVotos.Sort();
                double primero = listaVotos[listaVotos.Count - 1];
                double segundo = listaVotos[listaVotos.Count - 2];
                double diferencia = primero - segundo;
                if (votosNulos > diferencia)
                {
                    this.flagSelectSupuesto = 5;
                    this.cmbSupuesto.SelectedValue = 5;
                    this.cmbEstatusActa.SelectedValue = 5;
                    //this.cmbSupuesto.Enabled = false;
                    //this.DesactivarTextBoxes();
                    msgBox = new MsgBox(this, "Número de VOTOS NULOS mayor a la diferencia entre el 1ER y 2DO lugar", "Atención", MessageBoxButtons.OK, "Advertencia");
                    msgBox.ShowDialog(this);
                }
                else
                {
                    //this.cmbSupuesto.Enabled = true;
                    int selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                    if (selectedSupuesto == 5 || selectedSupuesto == 4 || selectedSupuesto == 6)
                    {
                        this.cmbSupuesto.SelectedIndex = 0;
                        this.cmbEstatusActa.SelectedValue = 1;
                    }
                }


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
                this.VerificarTotal(sender);
            }
            else if (e.KeyData == Keys.Enter || e.KeyData == Keys.Space)
            {
                return;
            }
            else
            {
                this.VerificarTotal(sender);
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
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }
        private void TxtPreventCaptura_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ClearDataTable(true);
            this.Close();
        }

        private void RecuentoVotos_Shown(object sender, EventArgs e)
        {
            this.MdiParent.WindowState = FormWindowState.Maximized;
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            try
            {
                this.AsginarCasilla();
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnReserva_Click(object sender, EventArgs e)
        {
            try
            {
                msgBox = new MsgBox(this.MdiParent, "¿Enviar la casilla a Reserva?", "Atención", MessageBoxButtons.YesNo, "Question");
                DialogResult result = msgBox.ShowDialog(this);
                if (result == DialogResult.Yes)
                {
                    this.ReservarCasilla("RESERVA");
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnNoConta_Click(object sender, EventArgs e)
        {
            try
            {
                this.cmbEstatusActa.SelectedValue = 9;
                msgBox = new MsgBox(this.MdiParent, "¿Marcar la Casilla como NO CONTABILIZABLE?\nLos cambios no se pueden deshacer", "Atención", MessageBoxButtons.YesNo, "Advertencia");
                DialogResult result = msgBox.ShowDialog(this);
                if (result == DialogResult.Yes)
                {
                    //this.ReservarCasilla("NO CONTABILIZABLE");
                    this.guardarRegistroVotos();
                }
                else
                {
                    this.cmbEstatusActa.SelectedValue = 1;
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
                msgBox = new MsgBox(this.MdiParent, "¿Guardar datos del Acta?\nLos cambios no se pueden deshacer", "Atención", MessageBoxButtons.YesNo, "Advertencia");
                DialogResult result = msgBox.ShowDialog(this);
                if (result == DialogResult.Yes)
                {
                    if(this.sc[PosActual].tipo == "RP")
                    {
                        this.guardarRegistroVotosRP();
                    }
                    else
                    {
                        this.guardarRegistroVotos();
                    }
                    
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void cmbEstatusActa_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                int sel = Convert.ToInt32(cmbEstatusActa.SelectedValue);
                //Habilitar cmbSupuesto solo si
                if (sel == 3 || sel == 5 || sel == 4)
                {
                    
                    cmbSupuesto.Enabled = true;
                    if(this.flagSelectSupuesto == 0)
                    {
                        cmbSupuesto.SelectedValue = 0;
                        tblPanelBoletas.Enabled = false;
                        tablePanelPartidos.Enabled = false;
                    }

                    
                }
                //No se debe capturar
                else if (sel == 6 || sel == 7 || sel == 9 || sel == 11)
                {
                    cmbSupuesto.Enabled = false;
                    cmbSupuesto.SelectedValue = 0;

                    tblPanelBoletas.Enabled = false;
                    tablePanelPartidos.Enabled = false;

                    //if (sel == 6 || sel == 7)
                    //{
                    //    cmbEstatusPaquete.SelectedValueChanged -= cmbEstatusPaquete_SelectedValueChanged;

                    //    cmbEstatusPaquete.SelectedValue = 1;

                    //    cmbEstatusPaquete.SelectedValueChanged += cmbEstatusPaquete_SelectedValueChanged;
                    //}
                }
                else if (sel == 1 || sel == 2 || sel == 8)
                {
                    tblPanelBoletas.Enabled = true;
                    tablePanelPartidos.Enabled = true;

                    cmbSupuesto.Enabled = false;
                    cmbSupuesto.SelectedValue = 0;
                }
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }
    }
}
