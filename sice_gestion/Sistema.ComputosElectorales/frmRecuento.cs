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
    public partial class frmRecuento : Form
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
        private int totalCandidatos;
        private int boletasRecibidas = 0;
        private List<sice_ar_supuestos> supuestos;
        private int Lnominal = 0;
        private int flagSelectSupuesto = 0;
        private int totalVotos = 0;
        private bool recuento = false;
        private bool reservaConsejo = false;

        const int SB_HORZ = 0;
        [DllImport("user32.dll")]

        static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        public frmRecuento(bool reserva)
        {
            this.reservaConsejo = reserva;
            //this.MdiParent.WindowState = FormWindowState.Maximized;
            InitializeComponent();

        }
        private void frmRecuento_Load(object sender, EventArgs e)
        {

            if (LoginInfo.privilegios == 5)
                txtVotosReserva.Enabled = false;
            else
                txtVotosReserva.Enabled = true;
            this.btnGuardar.Enabled = false;

            txtSobrantes.KeyPress += FrmRegistroActas_KeyPress;
            txtSobrantes.KeyUp += Evento_KeyUp;
            txtSobrantes.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtSobrantes.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtSobrantes.Leave += new System.EventHandler(tbxValue_Leave);

            txtVotosReserva.KeyPress += FrmRegistroActas_KeyPress;
            txtVotosReserva.KeyUp += Evento_KeyUp;
            txtVotosReserva.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtVotosReserva.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtVotosReserva.Leave += new System.EventHandler(tbxValue_Leave);

            txtTotalCapturado.KeyPress += TxtPreventCaptura_KeyPress;
            txtBoletasR.KeyPress += TxtPreventCaptura_KeyPress;
        }

        private void guardarRegistroVotos()
        {
            try
            {
                int boletasSobrantes = Convert.ToInt32(txtSobrantes.Text);
                int personas_votaron = Convert.ToInt32(txtTotalCapturado.Text);
                int votos_sacados = Convert.ToInt32(txtTotalCapturado.Text);
                int votos_reserva = Convert.ToInt32(txtVotosReserva.Text);

                CompElec = new ComputosElectoralesGenerales();
                this.panelCaptura.Enabled = false;

                List<sice_votos> lista_votos = new List<sice_votos>();
                int id_casilla = Convert.ToInt32(cmbCasilla.SelectedValue);
                if (id_casilla == 0)
                    throw new Exception("Error al guardar los datos");

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
                }

                //Validar casillas para reserva del consejo
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
                        this.panelCaptura.Enabled = true;
                        msgBox = new MsgBox(this, "El total de Captura excede el Número de Boletas recibidas", "Atención", MessageBoxButtons.OK, "Error");
                        msgBox.ShowDialog(this);
                        return;
                    }
                    //else if (this.flagSelectSupuesto == 5)
                    //{
                    //    this.panelCaptura.Enabled = true;
                    //    msgBox = new MsgBox(this, "Número de VOTOS NULOS mayor a la diferencia entre el 1ER y 2DO lugar", "Atención", MessageBoxButtons.OK, "Advertencia");
                    //    msgBox.ShowDialog(this);
                    //    return;
                    //}
                }
                else
                {
                    if (totalVotos < boletasRecibidas)
                    {
                        if (boletasSobrantes == 0)
                        {
                            if (votos_sacados != boletasRecibidas)
                                throw new Exception("Debes capturar el numero de boletas sobrantes");
                        }

                    }
                    if (votos_reserva == 0)
                        throw new Exception("Debes Capturar el Número de Votos a Reserva");
                    if (this.reservaConsejo)
                    {
                        throw new Exception("Esta Casilla ya fue Reservada para el Consejo.\n NO SE PUEDE ENVIAR A RESERVA DE NUEVO");
                    }
                    if (this.flagSelectSupuesto == 4)
                    {
                        this.panelCaptura.Enabled = true;
                        msgBox = new MsgBox(this, "El total de Captura excede el Número de Boletas recibidas", "Atención", MessageBoxButtons.OK, "Error");
                        msgBox.ShowDialog(this);
                        return;
                    }
                }

                if(votos_sacados == 0 && votos_reserva == 0)
                {
                    if(LoginInfo.privilegios == 5)
                        throw new Exception("El total de Captura debe ser mayor a 0");
                    else
                        throw new Exception("El total de Captura mas Votos Reservados debe ser mayor a 0");
                }
                

                List<double> tmpListaVotos = new List<double>();
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
                            tmpListaVotos.Add(num);
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
                       0, personas_votaron, 0, votos_sacados,
                        incidencias, estatus_acta, estatus_paquete, votos_reserva);
                    if (res2 == 1)
                    {
                        //this.tableLayoutPanel2.Enabled = true;
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
                int personas_votaron = Convert.ToInt32(txtTotalCapturado.Text);
                int votos_sacados = Convert.ToInt32(txtTotalCapturado.Text);
                int votos_reserva = Convert.ToInt32(txtVotosReserva.Text);

                CompElec = new ComputosElectoralesGenerales();
                this.panelCaptura.Enabled = false;

                List<sice_votos_rp> lista_votos = new List<sice_votos_rp>();
                int id_casilla = Convert.ToInt32(cmbCasilla.SelectedValue);
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
                }
                int selectedSupuesto = 0;
                //Validar casillas para reserva del consejo
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
                        this.panelCaptura.Enabled = true;
                        msgBox = new MsgBox(this, "El total de Captura excede el Número de Boletas recibidas", "Atención", MessageBoxButtons.OK, "Error");
                        msgBox.ShowDialog(this);
                        return;
                    }
                    //else if (this.flagSelectSupuesto == 5)
                    //{
                    //    this.panelCaptura.Enabled = true;
                    //    msgBox = new MsgBox(this, "Número de VOTOS NULOS mayor a la diferencia entre el 1ER y 2DO lugar", "Atención", MessageBoxButtons.OK, "Advertencia");
                    //    msgBox.ShowDialog(this);
                    //    return;
                    //}
                }
                else
                {
                    if (totalVotos < boletasRecibidas)
                    {
                        if (boletasSobrantes == 0)
                        {
                            if (votos_sacados != boletasRecibidas)
                                throw new Exception("Debes capturar el numero de boletas sobrantes");
                        }

                    }
                    if (votos_reserva == 0)
                        throw new Exception("Debes Capturar el Número de Votos a Reserva");
                    if (this.reservaConsejo)
                    {
                        throw new Exception("Esta Casilla ya fue Reservada para el Consejo.\n NO SE PUEDE ENVIAR A RESERVA DE NUEVO");
                    }
                    if (this.flagSelectSupuesto == 4)
                    {
                        this.panelCaptura.Enabled = true;
                        msgBox = new MsgBox(this, "El total de Captura excede el Número de Boletas recibidas", "Atención", MessageBoxButtons.OK, "Error");
                        msgBox.ShowDialog(this);
                        return;
                    }
                }

                if (votos_sacados == 0 && votos_reserva == 0)
                {
                    throw new Exception("El total de Captura mas Votos Reservados debe ser mayor a 0");
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
                        0, personas_votaron,0, votos_sacados,
                        incidencias, estatus_acta, estatus_paquete, votos_reserva);
                    if (res2 == 1)
                    {
                        //this.tableLayoutPanel2.Enabled = true;
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

        private void buscarRecuentoReserva()
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                if (CompElec.verificarRecuento(Convert.ToInt32(cmbCasilla.SelectedValue)) == 1)
                    this.recuento = true;
                else
                    this.recuento = false;
                if (CompElec.verificarReservaConsejo(Convert.ToInt32(cmbCasilla.SelectedValue)) == 1)
                    this.reservaConsejo = true;
                else
                    this.reservaConsejo = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ReservarCasilla(string motivo)
        {
            try
            {
                string mensaje = motivo == "NO CONTABILIZABLE" ? "Acta marcada como NO CONTABILIZABLE" : "Acta enviada a Reserva";
                int id_casilla = Convert.ToInt32(cmbCasilla.SelectedValue);
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

        private void cargarComboSeccion()
        {
            try
            {
                cmbSeccion.DataSource = null;
                cmbSeccion.DisplayMember = "Seccion";
                cmbSeccion.ValueMember = "Seccion";
                CompElec = new ComputosElectoralesGenerales();
                this.sc = CompElec.ListaSesccionesReserva(this.reservaConsejo);
                if (this.sc.Count < 1)
                {
                    msgBox = new MsgBox(this, "No hay Actas en " + (this.reservaConsejo ? "Reserva" : "Recuento"), "Atención", MessageBoxButtons.OK, "Advertencia");
                    msgBox.ShowDialog(this);
                    return;
                }
                var seGp = sc.GroupBy(x => x.seccion, x => x.id, (seccion, idSe) => new { IdSeccion = idSe, Seccion = seccion }).Select(g => g.Seccion).ToList();
                cmbSeccion.DataSource = seGp;
                cmbSeccion.Enabled = true;

                this.cargarComboCasilla();

                this.CargarComboEstatusActaPaqueteIncidenciasSupuestos();


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
                cmbEstatusActa.DataSource = CompElec.ListaEstatusActa("RECUENTO");
                cmbEstatusActa.SelectedValue = 1;

                this.flagCombo = 1;               

                cmbEstatusActa.SelectedValueChanged += cmbEstatusActa_SelectedValueChanged;


            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void cargarCandidatosResultados()
        {
            try
            {
                SeccionCasillaConsecutivo SelectedCasilla = (SeccionCasillaConsecutivo)cmbCasilla.SelectedItem;
                if (SelectedCasilla.tipo == "RP")
                {
                    this.cargarPartidosRP();
                    return;
                }

                CompElec = new ComputosElectoralesGenerales();
                if (this.distritoActual == 0)
                    throw new Exception("No se pudo cargar lista de Candidatos");
                this.buscarRecuentoReserva();

                List<Candidatos> lsCandidatos = CompElec.ListaCandidatos(this.distritoActual);
                this.totalCandidatos = lsCandidatos.Count();
                sice_reserva_captura detallesActa = CompElec.DetallesActa(Convert.ToInt32(cmbCasilla.SelectedValue),"MR");
                if (lsCandidatos != null && detallesActa != null)
                {
                    this.cmbEstatusActa.SelectedValueChanged -= cmbEstatusActa_SelectedValueChanged;
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
                    //if (SelectedCasilla.casilla == "S1")
                    //    TotalRepresentantes = 0;

                    this.totalCandidatos = lsCandidatos.Count() + 2;
                    this.cmbSupuesto.Enabled = true;
                    sice_reserva_captura rs = CompElec.EstatusActa(Convert.ToInt32(cmbCasilla.SelectedValue));
                    this.lblEstatus.Text = rs.tipo_reserva;

                    this.pictureBoxes = new PictureBox[lsCandidatos.Count + 2];
                    this.textBoxes = new TextBox[lsCandidatos.Count + 2];
                    this.panels = new Panel[lsCandidatos.Count + 2];
                    this.labelsName = new Label[lsCandidatos.Count + 2];
                    this.tablePanelPartidos.RowCount = 1;
                    this.btnGuardar.Enabled = true;
                    cmbSupuesto.Enabled = false;

                    sice_ar_supuestos supuesto = CompElec.getSupuesto(Convert.ToInt32(cmbCasilla.SelectedValue));
                    if (supuesto != null)
                        cmbSupuesto.SelectedIndex = supuesto.id;
                    else
                        cmbSupuesto.SelectedIndex = 0;

                    SeccionCasillaConsecutivo tempSec = (from p in this.sc where p.id == Convert.ToInt32(cmbCasilla.SelectedValue) select p).FirstOrDefault();
                    this.lblListaNominal.Text = tempSec.listaNominal.ToString();
                    if (SelectedCasilla.casilla == "S1")
                    {
                        this.lblListaNominal.Text = "0";
                    }
                    this.lblDistrito.Text = tempSec.distrito.ToString();
                    this.Lnominal = tempSec.listaNominal;
                    this.boletasRecibidas = this.Lnominal + TotalRepresentantes; //Lista nominal + 2 veces el numero de representantes de casillas
                    this.txtBoletasR.Text = this.boletasRecibidas.ToString();

                    this.cmbEstatusActa.SelectedValue = detallesActa.id_estatus_acta != null ? detallesActa.id_estatus_acta : 1;
                    this.cmbSupuesto.Enabled = true;

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

                    this.cmbEstatusActa.SelectedValueChanged += cmbEstatusActa_SelectedValueChanged;
                    //textBoxes[0].Focus();
                    //ShowScrollBar(this.tableLayoutPanel2.Handle, SB_HORZ, false);
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }

        }

        private void cargarPartidosRP()
        {
            try
            {

                CompElec = new ComputosElectoralesGenerales();
                this.buscarRecuentoReserva();

                List<sice_partidos_politicos> lsPartidos = CompElec.ListaPartidosPoliticos();
                if (lsPartidos != null)
                {
                    this.cmbEstatusActa.SelectedValueChanged -= cmbEstatusActa_SelectedValueChanged;
                    int totalPartidos = lsPartidos.Count() + 2;
                    //this.cmbSupuesto.Enabled = true;

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

                    sice_reserva_captura detallesActa = CompElec.DetallesActa(Convert.ToInt32(cmbCasilla.SelectedValue), "RP");


                    this.pictureBoxes = new PictureBox[lsPartidos.Count + 2];
                    this.textBoxes = new TextBox[lsPartidos.Count + 2];
                    this.panels = new Panel[lsPartidos.Count + 2];
                    this.labelsName = new Label[lsPartidos.Count + 2];
                    this.tablePanelPartidos.RowCount = 1;
                    this.btnGuardar.Enabled = true;

                    SeccionCasillaConsecutivo tempSec = (from p in this.sc where p.id == Convert.ToInt32(cmbCasilla.SelectedValue) select p).FirstOrDefault();
                    this.lblListaNominal.Text = "0";
                    this.lblDistrito.Text = tempSec.distrito.ToString();
                    this.Lnominal = tempSec.listaNominal;
                    this.boletasRecibidas = this.Lnominal + TotalRepresentantes; //Lista nominal + 2 veces el numero de representantes de casillas
                    this.txtBoletasR.Text = this.boletasRecibidas.ToString();

                    this.cmbEstatusActa.SelectedValue = detallesActa.id_estatus_acta != null ? detallesActa.id_estatus_acta : 1;
                    this.cmbSupuesto.SelectedValue = detallesActa.id_supuesto != null ? detallesActa.id_supuesto : 0;
                    this.lblEstatus.Text = detallesActa.tipo_reserva;
                    this.cmbSupuesto.Enabled = true;

                    //Agregar Columnas
                    this.tablePanelPartidos.AutoScroll = true;
                    this.tablePanelPartidos.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                    this.tablePanelPartidos.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
                    this.tablePanelPartidos.ColumnCount = totalPartidos;
                    decimal anchoColumnas = Math.Round(100 / (Convert.ToDecimal(totalPartidos)), 6);
                    for (int i = 0; i < totalPartidos; i++)
                    {
                        this.tablePanelPartidos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float)anchoColumnas));
                        //this.tablePanelPartidos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
                    }

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
                    //ShowScrollBar(this.tableLayoutPanel2.Handle, SB_HORZ, false);.
                    this.cmbEstatusActa.SelectedValueChanged += cmbEstatusActa_SelectedValueChanged;
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

            this.lblConsecutivo.Text = "No.";
            this.lblListaNominal.Text = "No.";
            this.lblDistrito.Text = "No.";
            this.lblEstatus.Text = "---";

            this.txtTotalCapturado.Text = "0";
            this.txtBoletasR.Text = "0";
            this.txtSobrantes.Text = "0";
            this.boletasRecibidas = 0;
            this.txtVotosReserva.Text = "0";

            this.cmbSupuesto.SelectedValue = 1;
            this.cmbEstatusActa.SelectedValue = 1;
            this.cargarComboSeccion();
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

                double votosReserva = 0;
                double.TryParse(this.txtVotosReserva.Text, out votosReserva);

                this.txtSobrantes.Text = boletasSobrantes.ToString();
                foreach (TextBox datos in this.textBoxes)
                {
                    double num;
                    int tempIdCandidato = Convert.ToInt32(datos.Tag);//Identificador para votos nulos
                    if (double.TryParse(datos.Text, out num))
                    {
                        totalVotos = totalVotos + num ;                        
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

                    if (totalVotos + boletasSobrantes + votosReserva > Convert.ToDouble(this.boletasRecibidas))
                    {
                        flagError = 1;
                        //datos.Text = "0";
                    }
                    double totales = totalVotos + boletasSobrantes + votosReserva;
                    txtTotalCapturado.Text = totalVotos.ToString();// + "  +  "+boletasSobrantes+ "  =  " + totales ;


                }
                this.totalVotos = Convert.ToInt32(totalVotos + boletasSobrantes + votosReserva);
                if (flagError == 1)
                {
                    this.flagSelectSupuesto = 4;
                    //this.cmbSupuesto.SelectedValue = 4;
                    //this.cmbEstatusActa.SelectedValue = 5;
                    //this.cmbSupuesto.Enabled = false;
                    //this.DesactivarTextBoxes();
                    msgBox = new MsgBox(this, "El total de Captura excede el Número de Boletas recibidas", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                    return;
                }
                //else if (flagError == 2)
                //{
                //    this.flagSelectSupuesto = 6;
                //    this.cmbSupuesto.SelectedValue = 6;
                //    this.cmbEstatusActa.SelectedValue = 5;
                //    //this.cmbSupuesto.Enabled = false;
                //    //this.DesactivarTextBoxes();
                //    msgBox = new MsgBox(this, "TODOS LOS VOTOS A FAVOR DE UN PARTIDO", "Atención", MessageBoxButtons.OK, "Error");
                //    msgBox.ShowDialog(this);
                //    return;
                //}

                listaVotos.Sort();              


                double primero = listaVotos[listaVotos.Count - 1];
                double segundo = listaVotos[listaVotos.Count - 2];
                double diferencia = primero - segundo;
                //if (votosNulos > diferencia)
                //{
                //    this.flagSelectSupuesto = 5;
                //    this.cmbSupuesto.SelectedValue = 5;
                //    this.cmbEstatusActa.SelectedValue = 5;
                //    //this.cmbSupuesto.Enabled = false;
                //    //this.DesactivarTextBoxes();
                //    msgBox = new MsgBox(this, "Número de VOTOS NULOS mayor a la diferencia entre el 1ER y 2DO lugar", "Atención", MessageBoxButtons.OK, "Advertencia");
                //    msgBox.ShowDialog(this);
                //}
                //else
                //{
                    //this.cmbSupuesto.Enabled = true;
                int selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                if (selectedSupuesto == 5 || selectedSupuesto == 4 || selectedSupuesto == 6)
                {
                    if (sender != null)
                    {
                        this.cmbSupuesto.SelectedIndex = 0;
                        this.cmbEstatusActa.SelectedValue = 1;
                    }
                }
                else
                {
                    if (votosReserva > 0)
                    {
                        this.cmbSupuesto.SelectedIndex = 0;
                        this.cmbEstatusActa.SelectedValue = 4;
                    }
                    else
                    {
                        this.cmbSupuesto.SelectedIndex = 0;
                        this.cmbEstatusActa.SelectedValue = 1;
                    }

                }
                
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

        private void cmbSeccion_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.ClearDataTable(true);
                this.cargarComboCasilla();
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }

        }

        private void cmbCasilla_SelectedValueChanged_1(object sender, EventArgs e)
        {
            //this.cmbEstatusActa.SelectedValue = 1;
            tblPanelBoletas.Enabled = true;
            tablePanelPartidos.Enabled = true;
            int? selected = Convert.ToInt32(cmbCasilla.SelectedValue);
            if (selected != null && selected != 0)
                this.ClearDataTable(); //Limpia tabla y carga lista de candidatos
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ClearDataTable(true);
            this.Close();
        }

        private void frmRecuento_Shown(object sender, EventArgs e)
        {
            this.MdiParent.WindowState = FormWindowState.Maximized;
            this.cargarComboSeccion();
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
                msgBox = new MsgBox(this.MdiParent, "¿Guardar datos del Acta?", "Atención", MessageBoxButtons.YesNo, "Advertencia");
                DialogResult result = msgBox.ShowDialog(this);
                if (result == DialogResult.Yes)
                {
                    SeccionCasillaConsecutivo SelectedCasilla = (SeccionCasillaConsecutivo)cmbCasilla.SelectedItem;
                    if (SelectedCasilla.tipo == "RP")
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
                if (sel == 3 || sel == 5 )
                {
                    cmbSupuesto.Enabled = true;

                    cmbSupuesto.SelectedValue = 0;
                    //tblPanelBoletas.Enabled = false;
                    //tablePanelPartidos.Enabled = false;
                    //txtEscritos.Enabled = false;
                }
                //No se debe capturar
                else if (sel == 6 || sel == 7 || sel == 9 || sel == 11 )  
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
                else if (sel == 1 || sel == 2 || sel == 8 || sel == 4)
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

