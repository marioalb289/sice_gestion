﻿using Cyotek.Windows.Forms;
using Sistema.DataModel;
using Sistema.Generales;
using Sistema.RegistroActasLocal.Properties;
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

namespace Sistema.RegistroActasLocal
{
    public partial class ModificarRegistroActas : Form
    {
        private Image _previewImage;
        private List<SeccionCasillaConsecutivo> sc;
        private List<sice_ar_supuestos> supuestos;
        private RegistroLocalGenerales regActas;
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
        private int Lnominal = 0;
        private int flagSelectSupuesto = 0;
        private int totalVotos = 0;
        private int boletasRecibidas = 0;
        private List<int> listaIdCandidatoPTMORENA;
        private bool flagRP = false;

        const int SB_HORZ = 0;
        [DllImport("user32.dll")]

        static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        public ModificarRegistroActas()
        {

            //this.MdiParent.WindowState = FormWindowState.Maximized;
            InitializeComponent();

        }
        private void ModificarRegistroActas_Load(object sender, EventArgs e)
        {
            this.btnGuardar.Enabled = false;
            
           

            txtSobrantes.KeyPress += FrmModificarRegistroActas_KeyPress;
            txtSobrantes.KeyUp += Evento_KeyUp;
            txtSobrantes.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtSobrantes.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtSobrantes.Leave += new System.EventHandler(tbxValue_Leave);

            txtPersonasVotaron.KeyPress += FrmModificarRegistroActas_KeyPress;
            txtPersonasVotaron.KeyUp += Evento_KeyUp;
            txtPersonasVotaron.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtPersonasVotaron.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtPersonasVotaron.Leave += new System.EventHandler(tbxValue_Leave);

            txtRepresentantes.KeyPress += FrmModificarRegistroActas_KeyPress;
            txtRepresentantes.KeyUp += Evento_KeyUp;
            txtRepresentantes.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtRepresentantes.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtRepresentantes.Leave += new System.EventHandler(tbxValue_Leave);

            txtVotosSacados.KeyPress += FrmModificarRegistroActas_KeyPress;
            txtVotosSacados.KeyUp += Evento_KeyUp;
            txtVotosSacados.GotFocus += new System.EventHandler(tbxValue_GotFocus);
            txtVotosSacados.MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
            txtVotosSacados.Leave += new System.EventHandler(tbxValue_Leave);

            txtTotalCapturado.KeyPress += TxtPreventCaptura_KeyPress;
            txtBoletasR.KeyPress += TxtPreventCaptura_KeyPress;
        }



        private void guardarRegistroVotos(bool nolegible = false)
        {
            try
            {
                int boletasSobrantes = Convert.ToInt32(txtSobrantes.Text);
                int personas_votaron = Convert.ToInt32(txtPersonasVotaron.Text);
                int votos_sacados = Convert.ToInt32(txtVotosSacados.Text);
                int con_cinta = chkCinta.Checked ? 1 : 0;
                int con_etiqueta = chkEtiqueta.Checked ? 1 : 0;

                List<sice_ar_votos_cotejo> lista_votos = new List<sice_ar_votos_cotejo>();
                int id_casilla = Convert.ToInt32(cmbCasilla.SelectedValue);
                if (id_casilla == 0)
                    throw new Exception("Error al guardar los datos");

                int estatus_acta = 0;
                int selectedSupuesto = 0;
                int estatus_paquete = Convert.ToInt32(cmbEstatusPaquete.SelectedValue);
                int condiciones_paquete = Convert.ToInt32(cmbEstadoPaquete.SelectedValue);
                if (estatus_paquete == 1)
                {
                    estatus_acta = 7;
                    selectedSupuesto = 0;
                }
                else if (estatus_paquete == 2)
                {
                    estatus_acta = 6;
                    selectedSupuesto = 0;
                }
                else if(estatus_paquete == 3)
                {
                    selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                    if (selectedSupuesto == 0)
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
                    

                    selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                    if (this.flagSelectSupuesto > 0)
                    {
                        estatus_acta = 5;
                        selectedSupuesto = this.flagSelectSupuesto;
                    }
                    else if (selectedSupuesto > 0)
                    {
                        estatus_acta = 5;
                    }
                    else
                    {
                        estatus_acta = 1;
                    }
                }
                else
                {
                    estatus_acta = 9;
                    selectedSupuesto = 0;
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
                        lista_votos.Add(new sice_ar_votos_cotejo()
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
                    regActas = new RegistroLocalGenerales();



                    int res = regActas.guardarDatosVotos(lista_votos, Convert.ToInt32(cmbCasilla.SelectedValue), selectedSupuesto, Convert.ToInt32(txtSobrantes.Text),
                        0, Convert.ToInt32(txtPersonasVotaron.Text), Convert.ToInt32(txtRepresentantes.Text), Convert.ToInt32(txtVotosSacados.Text),
                        0, estatus_acta, estatus_paquete, condiciones_paquete,con_etiqueta,con_cinta,true);
                    if (res == 1)
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
                //this.tableLayoutPanel2.Enabled = true;
                throw ex;
            }
        }

        private void guardarRegistroVotosRP(bool nolegible = false)
        {
            try
            {
                int boletasSobrantes = Convert.ToInt32(txtSobrantes.Text);
                int personas_votaron = Convert.ToInt32(txtPersonasVotaron.Text);
                int votos_sacados = Convert.ToInt32(txtVotosSacados.Text);
                int boletasRecibidas = Configuracion.BoletasEspecial;
                int con_cinta = chkCinta.Checked ? 1 : 0;
                int con_etiqueta = chkEtiqueta.Checked ? 1 : 0;

                List<sice_ar_votos_cotejo_rp> lista_votos = new List<sice_ar_votos_cotejo_rp>();
                int id_casilla = Convert.ToInt32(cmbCasilla.SelectedValue);
                if (id_casilla == 0)
                    throw new Exception("Error al guardar los datos");

                int estatus_acta = 0;
                int selectedSupuesto = 0;
                int estatus_paquete = Convert.ToInt32(cmbEstatusPaquete.SelectedValue);
                int condiciones_paquete = Convert.ToInt32(cmbEstadoPaquete.SelectedValue);
                if (estatus_paquete == 1)
                {
                    estatus_acta = 7;
                    selectedSupuesto = 0;
                }
                else if (estatus_paquete == 2)
                {
                    estatus_acta = 6;
                    selectedSupuesto = 0;
                }
                else if(estatus_paquete == 3)
                {
                    selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                    if (selectedSupuesto == 0)
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
                    

                    selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                    if (this.flagSelectSupuesto > 0)
                    {
                        estatus_acta = 5;
                        selectedSupuesto = this.flagSelectSupuesto;
                    }
                    else if (selectedSupuesto > 0)
                    {
                        estatus_acta = 5;
                    }
                    else
                    {
                        estatus_acta = 1;
                    }
                }
                else
                {
                    estatus_acta = 9;
                    selectedSupuesto = 0;
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
                        lista_votos.Add(new sice_ar_votos_cotejo_rp()
                        {
                            id_partido = id_partido,
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
                    regActas = new RegistroLocalGenerales();



                    int res = regActas.guardarDatosVotosRP(lista_votos, Convert.ToInt32(cmbCasilla.SelectedValue), selectedSupuesto, Convert.ToInt32(txtSobrantes.Text),
                        0, Convert.ToInt32(txtPersonasVotaron.Text), Convert.ToInt32(txtRepresentantes.Text), Convert.ToInt32(txtVotosSacados.Text),
                        0, estatus_acta, estatus_paquete, condiciones_paquete,con_etiqueta,con_cinta,true);
                    if (res == 1)
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
                //this.tableLayoutPanel2.Enabled = true;
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
                    mensaje = "El total Capturado es Diferente de los Votos Sacados de la Urna";
                }
                else if (sumaSobrantes != totalBoletasRecibidas)
                {
                    mensaje = "La Sumatoria de Personas y Representantes que votaron mas Boletas Sobrantes es Diferente de el Numero de boletas Recibidas";
                }

                if (mensaje != "")
                {
                    this.cmbSupuesto.SelectedValue = 4;
                    msgBox = new MsgBox(this.MdiParent, mensaje + "¿Enviar Acta a Recuento?", "Atención", MessageBoxButtons.YesNo, "Advertencia");
                    DialogResult result = msgBox.ShowDialog(this);
                    if (result == DialogResult.Yes)
                    {

                        return true;
                    }
                    else
                    {
                        cmbSupuesto.SelectedValue = 0;
                        return false;
                    }
                }
                return true;
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
                regActas = new RegistroLocalGenerales();
                if (this.sc == null)
                {
                    this.sc = regActas.ListaSescciones();
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
                    List<SeccionCasillaConsecutivo> caGp = (from p in this.sc where p.seccion == Convert.ToInt32(cmbSeccion.SelectedValue) select p).ToList();
                    if (caGp.Count > 0)
                        this.distritoActual = caGp[0].distrito;
                    caGp.Insert(0, new SeccionCasillaConsecutivo() { id = 0, casilla = "Seleccionar Casilla" });
                    cmbCasilla.DataSource = caGp;
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
                cmbSupuesto.DataSource = null;
                cmbSupuesto.DisplayMember = "Supuesto";
                cmbSupuesto.ValueMember = "id";
                if (this.supuestos == null)
                {
                    this.supuestos = regActas.ListaSupuestos();
                    this.supuestos.Insert(0, new sice_ar_supuestos() { id = 0, supuesto = "SIN MOTIVO DE RECUENTO" });
                }

                cmbSupuesto.DataSource = this.supuestos;

                regActas = new RegistroLocalGenerales();
                

                cmbEstatusPaquete.DataSource = null;
                cmbEstatusPaquete.DisplayMember = "estatus";
                cmbEstatusPaquete.ValueMember = "id";
                cmbEstatusPaquete.DataSource = regActas.ListaEstatusPaquete();
                cmbEstatusPaquete.SelectedValue = 3;
                //cmbCasilla.SelectedIndex = 1;

                cmbEstadoPaquete.DataSource = null;
                cmbEstadoPaquete.DisplayMember = "estado";
                cmbEstadoPaquete.ValueMember = "id";
                List<sice_ar_estado_paquete> list = regActas.ListaCondicionesPaquete();
                if (list.Count > 0)
                    list.Insert(0, new sice_ar_estado_paquete() { id = 0, estado = "SELECCIONE CONDICIONES DEL PAQUETE" });
                cmbEstadoPaquete.DataSource = list;



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
                regActas = new RegistroLocalGenerales();
                int res = regActas.verificarCasillaRegistrada(Convert.ToInt32(cmbCasilla.SelectedValue),"MR");
                if (res == 0)
                {
                    msgBox = new MsgBox(this, "Casilla NO Registrada", "Atención", MessageBoxButtons.OK, "Advertencia");
                    msgBox.ShowDialog(this);
                    cmbCasilla.SelectedIndex = 0;
                    this.BloquearControles();
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

        private void cargarResultadosVotos()
        {
            try
            {
                SeccionCasillaConsecutivo SelectedCasilla = (SeccionCasillaConsecutivo)cmbCasilla.SelectedItem;
                if (SelectedCasilla.tipo == "RP")
                {
                    this.flagRP = true;
                    this.cargarResultadosVotosRP();
                }
                else
                {
                    this.flagRP = false;
                    regActas = new RegistroLocalGenerales();
                    if (this.distritoActual == 0)
                        throw new Exception("No se pudo cargar lista de Resultados");
                    int totalVotos = 0;
                    List<CandidatosVotos> lsCandidatosVotos = regActas.ListaResultadosCasilla(Convert.ToInt32(cmbCasilla.SelectedValue), "sice_ar_votos_cotejo");
                    sice_ar_reserva detallesActa = regActas.DetallesActa(Convert.ToInt32(cmbCasilla.SelectedValue),"MR");
                    sice_ar_supuestos supuesto = regActas.getSupuesto(Convert.ToInt32(cmbCasilla.SelectedValue));
                    this.listaIdCandidatoPTMORENA = new List<int>();
                    if (supuesto != null)
                        cmbSupuesto.SelectedValue = supuesto.id;
                    else
                        cmbSupuesto.SelectedValue = 0;
                    if (lsCandidatosVotos != null && detallesActa != null)
                    {
                        int TotalRepresentantes = 1;
                        foreach (CandidatosVotos cnd in lsCandidatosVotos)
                        {
                            //si morena pt o pt-morena
                            if (cnd.id_partido == 5 || cnd.id_partido == 9 || cnd.id_partido == 15)
                                listaIdCandidatoPTMORENA.Add((int)cnd.id_candidato);
                            if (cnd.coalicion != "" && cnd.coalicion != null && cnd.tipo_partido != "COALICION")
                            {
                                TotalRepresentantes += regActas.RepresentantesCComun(cnd.coalicion);
                            }
                            else if(cnd.tipo_partido != "COALICION")
                            {
                                if (cnd.partido_local == 1)
                                    TotalRepresentantes += 1;
                                else if (cnd.partido_local == 0)
                                    TotalRepresentantes += 2;
                            }
                        }
                        //if (SelectedCasilla.casilla == "S1")
                        //    TotalRepresentantes = 0;
                        //var groupTotalNacional = lsCandidatosVotos.GroupBy(x => x.partido_local).Select(grp => new {
                        //    local = grp.Key,
                        //    total = grp.Count(),
                        //}).ToArray();
                        //int TotalRepresentantes = 0;
                        //foreach (var numInfo in groupTotalNacional)
                        //{
                        //    if (numInfo.local == 1)
                        //        TotalRepresentantes += numInfo.total;
                        //    else if(numInfo.local == 0)
                        //        TotalRepresentantes += numInfo.total * 2;
                        //}
                        this.totalCandidatos = lsCandidatosVotos.Count();


                        this.pictureBoxes = new PictureBox[lsCandidatosVotos.Count];
                        this.textBoxes = new TextBox[lsCandidatosVotos.Count];
                        this.panels = new Panel[lsCandidatosVotos.Count];
                        this.labelsName = new Label[lsCandidatosVotos.Count];
                        this.btnGuardar.Enabled = true;

                        if (SelectedCasilla.casilla == "S1")
                            txtRepresentantes.Enabled = false;
                        SeccionCasillaConsecutivo tempSec = (from p in this.sc where p.id == Convert.ToInt32(cmbCasilla.SelectedValue) select p).FirstOrDefault();
                        this.lblListaNominal.Text = tempSec.listaNominal.ToString();
                        if (SelectedCasilla.casilla == "S1")
                            this.lblListaNominal.Text = "0";
                        if (tempSec.distrito == 13)
                        {
                            TotalRepresentantes--;
                        }
                        this.lblDistrito.Text = tempSec.distrito.ToString();
                        this.Lnominal = tempSec.listaNominal;
                        this.boletasRecibidas = tempSec.listaNominal + TotalRepresentantes; //Lista nominal + 2 veces el numero de representantes de casillas
                        this.txtBoletasR.Text = this.boletasRecibidas.ToString();
                        this.txtPersonasVotaron.Text = detallesActa.personas_votaron.ToString();
                        this.txtRepresentantes.Text = detallesActa.num_representantes_votaron.ToString();
                        this.txtVotosSacados.Text = detallesActa.votos_sacados.ToString();
                        this.chkCinta.Checked = detallesActa.con_cinta == 1 ? true : false;
                        this.chkEtiqueta.Checked = detallesActa.con_etiqueta == 1 ? true : false;


                        cmbEstadoPaquete.SelectedValue = detallesActa.id_condiciones_paquete != null ? (int)detallesActa.id_condiciones_paquete : 0;
                        cmbEstatusPaquete.SelectedValue = detallesActa.id_estatus_paquete != null ? (int)detallesActa.id_estatus_paquete : 4;
                        //cmbSupuesto.SelectedValue = detallesActa.id_estatus_paquete != null ? (int)detallesActa.id_estatus_paquete : 4;
                        //cmbIncidencias.SelectedValue = detallesActa.id_incidencias != null ? (int)detallesActa.id_incidencias : 0;



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
                        for (int i = 0; i < totalCandidatos; i++)
                        {

                            pictureBoxes[i] = new PictureBox();
                            textBoxes[i] = new TextBox();
                            labelsName[i] = new Label();
                            panels[i] = new Panel();

                            //Imagen
                            pictureBoxes[i].Anchor = System.Windows.Forms.AnchorStyles.None;
                            pictureBoxes[i].Image = (lsCandidatosVotos[i].tipo == "NULO") ? (System.Drawing.Image)(Resources.nulos) : (lsCandidatosVotos[i].tipo == "NO REGISTRADO") ? (System.Drawing.Image)(Resources.no_regis) : (System.Drawing.Image)(resources.GetObject(lsCandidatosVotos[i].imagen));
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
                            textBoxes[i].KeyPress += FrmModificarRegistroActas_KeyPress;
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
                        this.txtSobrantes.Focus();
                        //textBoxes[0].Focus();
                        //ShowScrollBar(this.tableLayoutPanel2.Handle, SB_HORZ, false);     
                        this.txtSobrantes.Text = (detallesActa.boletas_sobrantes != null) ? detallesActa.boletas_sobrantes.ToString() : "0";
                        this.tablePanelPartidos.ResumeLayout();
                        this.tablePanelPartidos.Visible = true;
                        panelCaptura.Visible = true;
                        this.VerificarTotal();
                        this.txtSobrantes.Focus();

                    }
                }
                

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }

        }

        private void cargarResultadosVotosRP()
        {
            try
            {
                regActas = new RegistroLocalGenerales();
                int totalVotos = 0;
                List<PartidosVotosRP> lsPartidosVotos = regActas.ListaResultadosCasillaRP(Convert.ToInt32(cmbCasilla.SelectedValue), "sice_ar_votos_cotejo_rp");
                sice_ar_reserva detallesActa = regActas.DetallesActa(Convert.ToInt32(cmbCasilla.SelectedValue),"RP");
                sice_ar_supuestos supuesto = regActas.getSupuesto(Convert.ToInt32(cmbCasilla.SelectedValue));
                if (supuesto != null)
                    cmbSupuesto.SelectedValue = supuesto.id;
                else
                    cmbSupuesto.SelectedValue = 0;
                if (lsPartidosVotos != null && detallesActa != null)
                {                    
                    this.totalCandidatos = lsPartidosVotos.Count();
                    int TotalRepresentantes = 1;
                    foreach (PartidosVotosRP cnd in lsPartidosVotos)
                    {
                        if (cnd.coalicion != "" && cnd.coalicion != null && cnd.tipo != "COALICION")
                        {
                            TotalRepresentantes += regActas.RepresentantesCComun(cnd.coalicion);
                        }
                        else if (cnd.tipo != "COALICION")
                        {
                            if (cnd.partido_local == 1)
                                TotalRepresentantes += 1;
                            else if (cnd.partido_local == 0)
                                TotalRepresentantes += 2;
                        }
                    }

                    this.pictureBoxes = new PictureBox[lsPartidosVotos.Count];
                    this.textBoxes = new TextBox[lsPartidosVotos.Count];
                    this.panels = new Panel[lsPartidosVotos.Count];
                    this.labelsName = new Label[lsPartidosVotos.Count];
                    this.btnGuardar.Enabled = true;
                    txtRepresentantes.Enabled = false;

                    SeccionCasillaConsecutivo tempSec = (from p in this.sc where p.id == Convert.ToInt32(cmbCasilla.SelectedValue) select p).FirstOrDefault();
                    if (tempSec.distrito == 13)
                    {
                        TotalRepresentantes--;
                    }
                    this.lblListaNominal.Text = "0";
                    this.lblDistrito.Text = tempSec.distrito.ToString();
                    this.Lnominal = tempSec.listaNominal;
                    this.boletasRecibidas = this.Lnominal + TotalRepresentantes; //Lista nominal + 2 veces el numero de representantes de casillas
                    this.txtBoletasR.Text = this.boletasRecibidas.ToString();
                    this.txtPersonasVotaron.Text = detallesActa.personas_votaron.ToString();
                    this.txtRepresentantes.Text = detallesActa.num_representantes_votaron.ToString();
                    this.txtVotosSacados.Text = detallesActa.votos_sacados.ToString();
                    this.chkCinta.Checked = detallesActa.con_cinta == 1 ? true : false;
                    this.chkEtiqueta.Checked = detallesActa.con_etiqueta == 1 ? true : false;


                    cmbEstadoPaquete.SelectedValue = detallesActa.id_condiciones_paquete != null ? (int)detallesActa.id_condiciones_paquete : 0;
                    cmbEstatusPaquete.SelectedValue = detallesActa.id_estatus_paquete != null ? (int)detallesActa.id_estatus_paquete : 4;
                    //cmbIncidencias.SelectedValue = detallesActa.id_incidencias != null ? (int)detallesActa.id_incidencias : 0;



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
                    for (int i = 0; i < totalCandidatos; i++)
                    {

                        pictureBoxes[i] = new PictureBox();
                        textBoxes[i] = new TextBox();
                        labelsName[i] = new Label();
                        panels[i] = new Panel();

                        //Imagen
                        pictureBoxes[i].Anchor = System.Windows.Forms.AnchorStyles.None;
                        pictureBoxes[i].Image = (lsPartidosVotos[i].tipo == "NULO") ? (System.Drawing.Image)(Resources.nulos) : (lsPartidosVotos[i].tipo == "NO REGISTRADO") ? (System.Drawing.Image)(Resources.no_regis) : (System.Drawing.Image)(resources.GetObject(lsPartidosVotos[i].imagen));
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
                        labelsName[i].Text = lsPartidosVotos[i].tipo == "NULO" ? "Votos Nulos" : lsPartidosVotos[i].tipo == "NO REGISTRADO" ? "Candidato No Registrado" : lsPartidosVotos[i].partido;

                        //TextBox
                        textBoxes[i].Dock = System.Windows.Forms.DockStyle.Top;
                        textBoxes[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        textBoxes[i].Location = new System.Drawing.Point(46, 157);
                        textBoxes[i].Name = "textBox" + i;
                        textBoxes[i].Size = new System.Drawing.Size(63, 29);
                        textBoxes[i].TabIndex = 1 + i;
                        textBoxes[i].Tag = lsPartidosVotos[i].id_partido.ToString();
                        textBoxes[i].KeyPress += FrmModificarRegistroActas_KeyPress;
                        textBoxes[i].KeyUp += Evento_KeyUp;
                        textBoxes[i].GotFocus += new System.EventHandler(tbxValue_GotFocus);
                        textBoxes[i].MouseUp += new System.Windows.Forms.MouseEventHandler(tbxValue_MouseUp);
                        textBoxes[i].Leave += new System.EventHandler(tbxValue_Leave);
                        textBoxes[i].MaxLength = 3;
                        textBoxes[i].Text = lsPartidosVotos[i].votos.ToString();
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
                    //textBoxes[0].Focus();
                    //ShowScrollBar(this.tableLayoutPanel2.Handle, SB_HORZ, false);     
                    this.txtSobrantes.Text = (detallesActa.boletas_sobrantes != null) ? detallesActa.boletas_sobrantes.ToString() : "0";
                    this.tablePanelPartidos.ResumeLayout();
                    this.tablePanelPartidos.Visible = true;
                    panelCaptura.Visible = true;
                    this.VerificarTotal();
                    this.txtSobrantes.Focus();

                }
            }
            catch(Exception ex)
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
                this.chkCinta.Checked = false;
                this.chkEtiqueta.Checked = false;
                this.boletasRecibidas = 0;
                this.txtTotalCapturado.Text = "0";
                this.lblListaNominal.Text = "No.";
                this.lblDistrito.Text = "No.";

                if (!soloBloq)
                {
                    this.cargarResultadosVotos();
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
            //this.tableLayoutPanel2.Enabled = true;
            this.ClearDataTable(true);
            this.btnGuardar.Enabled = false;
            //this.cargarComboSeccion();
            this.lblListaNominal.Text = "No.";
            this.txtTotalCapturado.Text = "0";
            this.lblDistrito.Text = "No.";
            this.txtBoletasR.Text = "0";
            this.txtSobrantes.Text = "0";
            this.boletasRecibidas = 0;
            this.chkCinta.Checked = false;
            this.chkEtiqueta.Checked = false;

            this.txtPersonasVotaron.Text = "0";
            this.txtRepresentantes.Text = "0";
            this.txtVotosSacados.Text = "0";

            this.cmbEstatusPaquete.SelectedValue = 3;
            this.cmbEstadoPaquete.SelectedValue = 0;
            this.cmbSupuesto.SelectedValue = 0;

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
                List<TempSumaVotos> listaVotos = new List<TempSumaVotos>();
                double tempVotosPT = 0;
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
                        if (this.flagRP)
                            listaVotos.Add(new TempSumaVotos { id_candidatos = tempIdCandidato, votos = num });
                        else if (this.listaIdCandidatoPTMORENA.IndexOf(tempIdCandidato) != -1)
                            tempVotosPT += num;
                        else
                            listaVotos.Add(new TempSumaVotos { id_candidatos = tempIdCandidato, votos = num });

                        if (tempIdCandidato == -2)
                            votosNulos = num;
                    }
                    else
                    {
                        datos.Text = "0";
                        if (this.flagRP)
                            listaVotos.Add(new TempSumaVotos { id_candidatos = tempIdCandidato, votos = 0 });
                        else if (this.listaIdCandidatoPTMORENA.IndexOf(tempIdCandidato) != -1)
                            tempVotosPT += num;
                        else
                            listaVotos.Add(new TempSumaVotos { id_candidatos = tempIdCandidato, votos = 0 });
                        if (tempIdCandidato == 0)
                            votosNulos = 0;
                    }

                    //DONDE 100 morena pt pt morena
                    

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
                listaVotos.Add(new TempSumaVotos { id_candidatos = 100, votos = tempVotosPT });
                this.totalVotos = Convert.ToInt32(totalVotos + boletasSobrantes);
                if (flagError == 1)
                {
                    this.flagSelectSupuesto = 4;
                    this.cmbSupuesto.SelectedValue = 4;
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
                    //this.cmbSupuesto.Enabled = false;
                    //this.DesactivarTextBoxes();
                    msgBox = new MsgBox(this, "TODOS LOS VOTOS A FAVOR DE UN PARTIDO", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                    return;
                }

                listaVotos = listaVotos.Where(x => x.id_candidatos > 0).OrderBy(x => x.votos).ToList();
                double primero = listaVotos[listaVotos.Count - 1].votos;
                double segundo = listaVotos[listaVotos.Count - 2].votos;
                if(primero > 0 && segundo > 0)
                {
                    double diferencia = primero - segundo;
                    if (votosNulos > diferencia)
                    {
                        this.flagSelectSupuesto = 5;
                        this.cmbSupuesto.SelectedValue = 5;
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
                            if (sender != null)
                            {
                                this.cmbSupuesto.SelectedValue = 0;
                            }

                        }
                    }
                }
                else
                {
                    //this.cmbSupuesto.Enabled = true;
                    int selectedSupuesto = Convert.ToInt32(cmbSupuesto.SelectedValue);
                    if (selectedSupuesto == 5 || selectedSupuesto == 4 || selectedSupuesto == 6)
                    {
                        if (sender != null)
                        {
                            this.cmbSupuesto.SelectedValue = 0;
                        }

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

        private void FrmModificarRegistroActas_KeyPress(object sender, KeyPressEventArgs e)
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
            try
            {
                int? selected = Convert.ToInt32(cmbCasilla.SelectedValue);
                txtRepresentantes.Enabled = true;
                this.cmbEstatusPaquete.SelectedValue = 3;
                this.cmbSupuesto.SelectedValue = 0;
                this.cmbEstadoPaquete.SelectedValue = 0;
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.BloquearControles();
            this.Close();
        }

        private void ModificarRegistroActas_Shown(object sender, EventArgs e)
        {
            this.MdiParent.WindowState = FormWindowState.Maximized;
            this.cargarComboSeccion();
        }

        private void btnNoConta_Click(object sender, EventArgs e)
        {

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

        private void btnNoLegible_Click(object sender, EventArgs e)
        {
            try
            {
                msgBox = new MsgBox(this.MdiParent, "¿Marcar la Casilla como NO LEGIBLE?", "Atención", MessageBoxButtons.YesNo, "Advertencia");
                DialogResult result = msgBox.ShowDialog(this);
                if (result == DialogResult.Yes)
                {
                    this.guardarRegistroVotos(true);
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
                //int sel = Convert.ToInt32(cmbEstatusActa.SelectedValue);
                ////Habilitar cmbSupuesto solo si
                //if(sel == 3 || sel== 5 || sel == 4)
                //{
                //    cmbSupuesto.Enabled = true;
                //    cmbIncidencias.Enabled = true;
                //}
                ////No se debe capturar
                //else if (sel == 6 || sel==7 || sel== 9 || sel == 11 || sel == 10)
                //{
                //    cmbSupuesto.Enabled = false;
                //    cmbIncidencias.Enabled = true;
                //    cmbIncidencias.SelectedValue = 0;

                //    //if (sel == 6 || sel == 7)
                //    //{
                //    //    cmbEstatusPaquete.SelectedValueChanged -= cmbEstatusPaquete_SelectedValueChanged;

                //    //    cmbEstatusPaquete.SelectedValue = 1;

                //    //    cmbEstatusPaquete.SelectedValueChanged += cmbEstatusPaquete_SelectedValueChanged;
                //    //}
                //}
                //else if(sel==1 || sel ==2 || sel == 8)
                //{
                //    cmbSupuesto.Enabled = false;
                //    cmbSupuesto.SelectedValue = 0;
                //    cmbIncidencias.Enabled = false;
                //    cmbIncidencias.SelectedValue = 0;
                //}

                
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void cmbEstatusPaquete_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                int sel = Convert.ToInt32(cmbEstatusPaquete.SelectedValue);
                if (sel != 3)
                {
                    cmbEstadoPaquete.Enabled = false;
                    cmbSupuesto.SelectedValue = 0;
                    cmbSupuesto.Enabled = false;

                    tblPanelBoletas.Enabled = false;
                    tablePanelPartidos.Enabled = false;
                }
                else
                {
                    cmbEstadoPaquete.Enabled = true;
                    cmbSupuesto.Enabled = true;

                    tblPanelBoletas.Enabled = true;
                    tablePanelPartidos.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void cmbSupuesto_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                int sel = Convert.ToInt32(cmbSupuesto.SelectedValue);
                if (sel > 0 && this.flagSelectSupuesto == 0)
                {
                    tblPanelBoletas.Enabled = false;
                    tablePanelPartidos.Enabled = false;
                }
                else
                {
                    tblPanelBoletas.Enabled = true;
                    tablePanelPartidos.Enabled = true;
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



