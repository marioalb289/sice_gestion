﻿using Cyotek.Windows.Forms;
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
                this.tableLayoutPanel2.Enabled = false;
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
                    int res2 = CompElec.guardarDatosVotos(lista_votos, id_casilla, this.totalCandidatos,true);
                    if (res2 == 1)
                    {
                        this.tableLayoutPanel2.Enabled = true;
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
                this.tableLayoutPanel2.Enabled = true;
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
                    this.pictureBoxes = new PictureBox[lsCandidatosVotos.Count];
                    this.textBoxes = new TextBox[lsCandidatosVotos.Count];
                    this.panels = new Panel[lsCandidatosVotos.Count];
                    this.labelsName = new Label[lsCandidatosVotos.Count];
                    this.tblPanaelPartidos.RowCount = 1;

                    SeccionCasillaConsecutivo tempSec = (from p in this.sc where p.id == Convert.ToInt32(cmbCasilla.SelectedValue) select p).FirstOrDefault();
                    this.lblListaNominal.Text = tempSec.listaNominal.ToString();
                    this.lblDistrito.Text = tempSec.distrito.ToString();
                    this.lblConsecutivo.Text = tempSec.consecutivo.ToString();
                    this.Lnominal = tempSec.listaNominal;

                    bool flagFocus = false;

                    for (int i = 0; i < lsCandidatosVotos.Count; i++)
                    {
                        pictureBoxes[i] = new PictureBox();
                        textBoxes[i] = new TextBox();
                        labelsName[i] = new Label();
                        panels[i] = new Panel();

                        this.tblPanaelPartidos.RowCount = this.tblPanaelPartidos.RowCount + 1;

                        this.tblPanaelPartidos.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));

                        //IMAGEN DEL PARTIDO
                        pictureBoxes[i].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                        pictureBoxes[i].Dock = System.Windows.Forms.DockStyle.Top;
                        pictureBoxes[i].Image = (lsCandidatosVotos[i].tipo == "NULO") ? (System.Drawing.Image)(Properties.Resources.nulos1) : (lsCandidatosVotos[i].tipo == "NO REGISTRADO") ? (System.Drawing.Image)(Properties.Resources.no_regis) : (System.Drawing.Image)(Properties.Resources.pri);
                        pictureBoxes[i].Location = new System.Drawing.Point(15, 57);
                        pictureBoxes[i].Name = "pictureBox" + i;
                        pictureBoxes[i].Size = new System.Drawing.Size(75, 44);
                        pictureBoxes[i].SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                        pictureBoxes[i].TabIndex = 20 + i;
                        pictureBoxes[i].TabStop = false;

                        //ETIQUETA DEL NOMBRE DEL CANDIADATO
                        labelsName[i].Dock = System.Windows.Forms.DockStyle.Top;
                        labelsName[i].Location = new System.Drawing.Point(0, 28);
                        labelsName[i].Name = "labelNameCandidato" + i;
                        labelsName[i].Size = new System.Drawing.Size(75, 13);
                        labelsName[i].TabIndex = 5;
                        labelsName[i].Text = lsCandidatosVotos[i].tipo == "NULO" ? "Votos Nulos" : lsCandidatosVotos[i].tipo == "NO REGISTRADO" ? "Candidato No Registrado" : lsCandidatosVotos[i].candidato;
                        labelsName[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                        //PANEL DONDE IRAN LA IMAGEN Y LA ETIQUETA
                        panels[i].Controls.Add(labelsName[i]);
                        panels[i].Controls.Add(pictureBoxes[i]);
                        panels[i].Dock = System.Windows.Forms.DockStyle.Fill;
                        panels[i].Location = new System.Drawing.Point(15, 57);
                        panels[i].Name = "panelImagenPartido" + i;
                        panels[i].Size = new System.Drawing.Size(75, 44);
                        panels[i].TabIndex = 200 + i;



                        this.tblPanaelPartidos.Controls.Add(panels[i], 0, i + 1);
                        totalVotos += (int)lsCandidatosVotos[i].votos;
                        if (!flagFocus)
                        {
                            textBoxes[i].Focus();
                            flagFocus = true;
                        }
                        //Texbox para captura de votos
                        textBoxes[i].Anchor = System.Windows.Forms.AnchorStyles.None;
                        textBoxes[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        textBoxes[i].Location = new System.Drawing.Point(106, 64);
                        textBoxes[i].Name = "textBox" + i;
                        textBoxes[i].Size = new System.Drawing.Size(100, 29);
                        textBoxes[i].TabIndex = 100 + i;
                        //Votos nulos 0 Candidato no registrado -1
                        textBoxes[i].Tag = lsCandidatosVotos[i].id_candidato.ToString();
                        textBoxes[i].KeyPress += FrmRegistroActas_KeyPress;
                        textBoxes[i].KeyUp += Evento_KeyUp;
                        textBoxes[i].MaxLength = 3;
                        textBoxes[i].Text = lsCandidatosVotos[i].votos.ToString();
                        textBoxes[i].TextAlign = HorizontalAlignment.Center;

                        this.tblPanaelPartidos.Controls.Add(textBoxes[i], 1, i + 1);



                    }

                    this.btnGuardar.Enabled = true;
                    this.btnNoConta.Enabled = true;
                    this.lblTotalCapturado.Text = totalVotos.ToString();
                    this.tblPanaelPartidos.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
                    this.tblPanaelPartidos.ResumeLayout(false);
                    this.tblPanaelPartidos.Visible = true;
                    ShowScrollBar(this.tableLayoutPanel2.Handle, SB_HORZ, false);
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
                this.tblPanaelPartidos.Visible = false;
                this.tblPanaelPartidos.Controls.Clear();
                this.tblPanaelPartidos.RowStyles.Clear();
                this.tblPanaelPartidos.RowCount = 1;
                this.tblPanaelPartidos.SuspendLayout();

                Panel PanelTempTitutlo1 = new Panel();
                Panel PanelTempTitutlo2 = new Panel();

                Label labelTemp1 = new Label();
                Label labelTemp2 = new Label();

                labelTemp1.Dock = System.Windows.Forms.DockStyle.Top;
                labelTemp1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                labelTemp1.ForeColor = System.Drawing.Color.White;
                labelTemp1.Location = new System.Drawing.Point(0, 0);
                labelTemp1.Name = "label4";
                labelTemp1.Size = new System.Drawing.Size(152, 44);
                labelTemp1.TabIndex = 0;
                labelTemp1.Text = "PARTIDO, COALICIÓN O CANDIDATURA";
                labelTemp1.TextAlign = System.Drawing.ContentAlignment.TopCenter;

                labelTemp2.Dock = System.Windows.Forms.DockStyle.Top;
                labelTemp2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                labelTemp2.ForeColor = System.Drawing.Color.White;
                labelTemp2.Location = new System.Drawing.Point(0, 0);
                labelTemp2.Name = "label5";
                labelTemp2.Size = new System.Drawing.Size(152, 32);
                labelTemp2.TabIndex = 0;
                labelTemp2.Text = "RESULTADOS ELECTORALES";
                labelTemp2.TextAlign = System.Drawing.ContentAlignment.TopCenter;

                PanelTempTitutlo1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(158)))), ((int)(((byte)(150)))));
                PanelTempTitutlo1.Controls.Add(labelTemp1);
                PanelTempTitutlo1.Dock = System.Windows.Forms.DockStyle.Fill;
                PanelTempTitutlo1.Location = new System.Drawing.Point(15, 5);
                PanelTempTitutlo1.Name = "pnlTableTitulo";
                PanelTempTitutlo1.Size = new System.Drawing.Size(152, 46);
                PanelTempTitutlo1.TabIndex = 39;

                PanelTempTitutlo2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(158)))), ((int)(((byte)(150)))));
                PanelTempTitutlo2.Controls.Add(labelTemp2);
                PanelTempTitutlo2.Dock = System.Windows.Forms.DockStyle.Fill;
                PanelTempTitutlo2.Location = new System.Drawing.Point(175, 5);
                PanelTempTitutlo2.Name = "pnlTableVotos";
                PanelTempTitutlo2.Size = new System.Drawing.Size(152, 46);
                PanelTempTitutlo2.TabIndex = 41;



                this.tblPanaelPartidos.Controls.Add(PanelTempTitutlo1, 0, 0);
                this.tblPanaelPartidos.Controls.Add(PanelTempTitutlo2, 1, 0);

                if (!soloBloq)
                {
                    this.cargarCandidatosResultados();
                }

                else
                {
                    this.tblPanaelPartidos.ResumeLayout();
                    this.tblPanaelPartidos.Visible = true;
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
            this.btnNoConta.Enabled = false;
            this.cargarComboSeccion();
            //this.btnReserva.Enabled = false;
            //this.btnSiguiente.Enabled = true;

            //this.lblCasilla.Text = "----";
            this.lblTotalCapturado.Text = "No.";
            this.lblConsecutivo.Text = "No.";
            this.lblDistrito.Text = "No.";
            this.lblListaNominal.Text = "No.";


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

                    if (totalVotos > Convert.ToDouble(Lnominal))
                    {
                        flagError = 1;
                        //datos.Text = "0";
                    }
                    lblTotalCapturado.Text = totalVotos.ToString();


                }
                this.totalVotos = Convert.ToInt32(totalVotos);
                if (flagError > 0)
                {
                    this.flagSelectSupuesto = 2;
                    msgBox = new MsgBox(this, "El total de Captura excede la Lista Nominal", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                    return;

                }

                listaVotos.Sort();
                double primero = listaVotos[listaVotos.Count - 1];
                double segundo = listaVotos[listaVotos.Count - 2];
                double diferencia = primero - segundo;
                if (votosNulos > diferencia)
                {

                    if (this.recuento)
                    {
                        this.flagSelectSupuesto = 5;
                        msgBox = new MsgBox(this, "NUMERO DE VOTOS NULOS MAYOR A LA DIFERENCIA ENTRE LOS CANDIDATOS DEL 1ER Y 2DO LUGAR", "Atención", MessageBoxButtons.OK, "Error");
                        msgBox.ShowDialog(this);
                        return;
                    }
                    else
                    {
                        this.flagSelectSupuesto = 5;
                        msgBox = new MsgBox(this.MdiParent, "NUMERO DE VOTOS NULOS MAYOR A LA DIFERENCIA ENTRE LOS CANDIDATOS DEL 1ER Y 2DO LUGAR.\n¿ENVIAR ACTA A RESERVA?", "Atención", MessageBoxButtons.YesNo, "Advertencia");
                        DialogResult result = msgBox.ShowDialog(this);
                        if (result == DialogResult.Yes)
                        {
                            this.ReservarCasilla("RESERVA");
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
            this.VerificarTotal();
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
            this.Close();
        }

        private void ModificarRecuentoVotos_Shown(object sender, EventArgs e)
        {
            //this.MdiParent.WindowState = FormWindowState.Maximized;
            this.cargarComboSeccion();
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