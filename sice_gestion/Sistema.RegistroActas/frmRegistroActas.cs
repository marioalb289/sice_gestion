﻿using Cyotek.Windows.Forms;
using Sistema.DataModel;
using Sistema.Generales;
using Sistema.RegistroActas.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Sistema.RegistroActas
{
    public partial class frmRegistroActas : Form
    {
        #region Instance Fields

        private Image _previewImage;
        private RegistroActasGenerales rgActas;
        private List<SeccionCasilla> sc;
        private PictureBox[] pictureBoxes;
        private TextBox[] textBoxes;
        private Panel[] panels;
        private Label[] labelsName;
        private int flagCombo = 0;
        private int idDocumento = 0;
        private int totalCandidatos = 0;
        private MsgBox msgBox;
        private Loading Loadingbox;

        #endregion

        public frmRegistroActas()
        {
            InitializeComponent();
            this.Activated += FrmRegistroActas_Activated;
        }

        private void FrmRegistroActas_Activated(object sender, EventArgs e)
        {
            if(Loadingbox != null)
            {
                Loadingbox.Activate();
                Loadingbox.Focus();
            }
            
        }

        private void frmRegistroActas_Load(object sender, EventArgs e)
        {            
            imageBox.MouseWheel += new MouseEventHandler(DoNothing_MouseWheel);
            
        }

        private void frmRegistroActas_Shown(object sender, EventArgs e)
        {
            this.MdiParent.WindowState = FormWindowState.Maximized;
            this.cargarActaYaAsignada();
        }


        private void DoNothing_MouseWheel(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("Se mueve rueda del mouse");
            HandledMouseEventArgs ee = (HandledMouseEventArgs)e;
            ee.Handled = true;

        }

        private void CargarImagen(sice_ar_documentos documento)
        {
            try
            {
                ftp ftpClient = new ftp(Configuracion.NetworkFtp, Configuracion.User, Configuracion.Pass);
                Image imagen = ftpClient.downloadImage(Configuracion.Repo+"/"+documento.nombre);
                this.OpenImage(imagen);

            }
            catch(Exception ex)
            {
                throw new Exception("Error al Cargar imagen");
            }
        }

        private void cargarActaYaAsignada()
        {
            try
            {
                Loadingbox = new Loading(this,"Cargando");
                Loadingbox.Show(this);
                rgActas = new RegistroActasGenerales();
                sice_ar_documentos doc = rgActas.BuscarActaAsignada();
                if (doc != null)
                {
                    this.idDocumento = doc.id;
                    flagCombo = 0;
                    this.cargarComboSeccion();
                    this.CargarImagen(doc);
                    Loadingbox.Close();
                    flagCombo++;
                    msgBox = new MsgBox(this, "Acta Asginada", "Atención", MessageBoxButtons.OK, "Ok");
                    msgBox.ShowDialog();
                }
                Loadingbox.Close();
                this.tableLayoutPanel2.Enabled = true;
            }
            catch(Exception ex)
            {
                Loadingbox.Close();
                this.tableLayoutPanel2.Enabled = true;
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void cargarComboSeccion()
        {
            try
            {       

                imageBox.SelectionMode = ImageBoxSelectionMode.Zoom;
                imageBox.AllowClickZoom = true;
                imageBox.Enabled = true;

                cmbSeccion.DataSource = null;
                cmbSeccion.DisplayMember = "Seccion";
                cmbSeccion.ValueMember = "Seccion";
                rgActas = new RegistroActasGenerales();
                if(this.sc == null)
                {
                    this.sc = rgActas.ListaSescciones();
                }
                
                var seGp = sc.GroupBy(x => x.seccion, x => x.id, (seccion,idSe) => new { IdSeccion = idSe, Seccion = seccion }).Select(g => g.Seccion).ToList();
                cmbSeccion.DataSource = seGp;
                cmbSeccion.Enabled = true;
                this.ClearDataTable();

                
            
            }
            catch(Exception ex)
            {
                throw ex;
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
                    int distrito = caGp[0].distrito;
                    caGp.Insert(0, new SeccionCasilla() { id = 0, casilla = "Seleccionar Casilla" });
                    cmbCasilla.DataSource = caGp;
                    //cmbCasilla.SelectedIndex = 1;
                    cmbCasilla.Enabled = true;

                    cmbCasilla.Enabled = true;
                    this.cargarCandidatosResultados(distrito);
                }
            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void verificarCasilla()
        {
            try
            {
                rgActas = new RegistroActasGenerales();
                int res = rgActas.verificarCasillaValida(Convert.ToInt32(cmbCasilla.SelectedValue));
                if (res == 0)
                {
                    this.btnGuardar.Enabled = true;
                    this.btnLimpiar.Enabled = true;
                    this.btnLegible.Enabled = true;
                    
                }
                else
                {
                    string estatus = "";
                    if (res == 1)
                        estatus = "COTEJO";
                    else
                        estatus = "VÁLIDO";
                    msgBox = new MsgBox(this, "Casilla ya Registrada y en estatus: "+estatus, "Atención", MessageBoxButtons.OK, "Advertencia");
                    msgBox.ShowDialog(this);
                    cmbCasilla.SelectedIndex = 0;
                    this.btnGuardar.Enabled = false;
                    this.btnLimpiar.Enabled = false;
                    this.btnLegible.Enabled = false;
                }
                


            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void NoLegible()
        {
            try
            {
                if (this.idDocumento == 0)
                    throw new Exception("No se pudo marcar el acata como NO LEGIBLE");
                rgActas = new RegistroActasGenerales();
                if (rgActas.EnviarRevision(this.idDocumento, "NO LEGIBLE"))
                {
                    msgBox = new MsgBox(this, "Acta marcada como NO LEGIBLE", "Atención", MessageBoxButtons.OK, "Ok");
                    msgBox.ShowDialog(this);
                    this.BloquearControles();
                }                    
                else{
                    msgBox = new MsgBox(this,"No se pudo marcar el acata como NO LEGIBLE", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                }
                    


            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void cargarCandidatosResultados(int distrito)
        {
            try
            {
                rgActas = new RegistroActasGenerales();
                List<Candidatos> lsCandidatos = rgActas.ListaCandidatos(distrito);
                this.totalCandidatos = lsCandidatos.Count();
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
                if ( lsCandidatos != null)
                {
                    this.pictureBoxes = new PictureBox[lsCandidatos.Count + 2];
                    this.textBoxes = new TextBox[lsCandidatos.Count + 2];
                    this.panels = new Panel[lsCandidatos.Count + 2];
                    this.labelsName = new Label[lsCandidatos.Count + 2];
                    this.tblPanaelPartidos.RowCount = 1;                  
                    
                    for (int i = 0; i < lsCandidatos.Count +2; i++)
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
                        pictureBoxes[i].Image = (i > lsCandidatos.Count - 1) ? (i == lsCandidatos.Count ? (System.Drawing.Image)(Resources.no_regis) : (System.Drawing.Image)(Resources.nulos)) : ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
                        pictureBoxes[i].Location = new System.Drawing.Point(15, 57);
                        pictureBoxes[i].Name = "pictureBox" + i;
                        pictureBoxes[i].Size = new System.Drawing.Size(75, 44);
                        pictureBoxes[i].SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                        pictureBoxes[i].TabIndex = 20+i;
                        pictureBoxes[i].TabStop = false;

                        //ETIQUETA DEL NOMBRE DEL CANDIADATO
                        labelsName[i].Dock = System.Windows.Forms.DockStyle.Top;
                        labelsName[i].Location = new System.Drawing.Point(0, 28);
                        labelsName[i].Name = "labelNameCandidato"+i;
                        labelsName[i].Size = new System.Drawing.Size(75, 13);
                        labelsName[i].TabIndex = 5;
                        labelsName[i].Text = (i > lsCandidatos.Count - 1) ? i == lsCandidatos.Count ? "Candidato No Registrado" : "Votos Nulos" : lsCandidatos[i].candidato;
                        labelsName[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                        //PANEL DONDE IRAN LA IMAGEN Y LA ETIQUETA
                        panels[i].Controls.Add(labelsName[i]);
                        panels[i].Controls.Add(pictureBoxes[i]);
                        panels[i].Dock = System.Windows.Forms.DockStyle.Fill;
                        panels[i].Location = new System.Drawing.Point(15, 57);
                        panels[i].Name = "panelImagenPartido"+i;
                        panels[i].Size = new System.Drawing.Size(75, 44);
                        panels[i].TabIndex = 200 + i;

                        

                        this.tblPanaelPartidos.Controls.Add(panels[i], 0, i + 1);

                        //Texbox para captura de votos
                        textBoxes[i].Anchor = System.Windows.Forms.AnchorStyles.None;
                        textBoxes[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        textBoxes[i].Location = new System.Drawing.Point(106, 64);
                        textBoxes[i].Name = "textBox" + i;
                        textBoxes[i].Size = new System.Drawing.Size(100, 29);
                        textBoxes[i].TabIndex = 100 + i;
                        //Votos nulos 0 Candidato no registrado -1
                        textBoxes[i].Tag =   (i > lsCandidatos.Count - 1) ? i == lsCandidatos.Count ? "-1" : "0" : lsCandidatos[i].id_candidato.ToString();
                        textBoxes[i].KeyPress += FrmRegistroActas_KeyPress;
                        textBoxes[i].KeyDown += FrmRegistroActas_KeyDown;
                        textBoxes[i].MaxLength = 3;
                        textBoxes[i].Text = "0";
                        textBoxes[i].TextAlign = HorizontalAlignment.Center;

                        this.tblPanaelPartidos.Controls.Add(textBoxes[i], 1, i + 1);



                    }
                    this.btnGuardar.Enabled = true;
                    this.btnLimpiar.Enabled = true;
                    this.btnLegible.Enabled = true;
                    this.tblPanaelPartidos.ResumeLayout();
                    this.tblPanaelPartidos.Visible = true;
                }

            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }

        }        

        private void guardarRegistroVotos()
        {
            try
            {
                Loadingbox = new Loading(this, "Guardando");
                Loadingbox.Show(this);
                this.tableLayoutPanel2.Enabled = false;
                List<sice_ar_votos> lista_votos = new List<sice_ar_votos>();
                int id_casilla = Convert.ToInt32(cmbCasilla.SelectedValue);
                if (id_casilla == 0)
                    throw new Exception("Selecciona una Casilla");
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
                        else if(tempIdCandidato == 0)
                        {
                            tipo_voto = "NULO";
                        }
                        else
                        {
                            tipo_voto = "NO REGISTRADO";
                        }
                        lista_votos.Add(new sice_ar_votos() {
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
                if(lista_votos.Count > 0)
                {
                    rgActas = new RegistroActasGenerales();
                    int res = rgActas.guardarDatosVotos(lista_votos, this.idDocumento, Convert.ToInt32(cmbCasilla.SelectedValue),this.totalCandidatos);
                    Loadingbox.Close();
                    switch (res)
                    {
                        case 1:
                            msgBox = new MsgBox(this,"Datos Guardados correctamente", "Atención", MessageBoxButtons.OK, "Ok");
                            msgBox.ShowDialog(this);
                            break;
                        case 2:
                            msgBox = new MsgBox(this,"El acta no ha pasado el proceso de validación \nActa enviada a Revision", "Atención", MessageBoxButtons.OK, "Advertencia");
                            msgBox.ShowDialog(this);
                            break;
                        case 3:
                            msgBox = new MsgBox(this, "Acta validada correctamente", "Atención", MessageBoxButtons.OK, "Ok");
                            msgBox.ShowDialog(this);
                            break;
                        case 4:
                            msgBox = new MsgBox(this, "Ya existe un documento Asingado a esta casilla y en proceso de validación. \nEl documento actual será enviado a Revisión para su evaluacion", "Atención", MessageBoxButtons.OK, "Advertencia");
                            msgBox.ShowDialog(this);
                            break;
                            
                    }
                    this.BloquearControles();

                }

            }
            catch(Exception ex)
            {
                Loadingbox.Close();
                this.tableLayoutPanel2.Enabled = true;
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
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

        private void FrmRegistroActas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void BloquearControles()
        {
            this.tableLayoutPanel2.Enabled = true;
            this.ClearDataTable(true);
            this.btnGuardar.Enabled = false;
            this.btnLimpiar.Enabled = false;
            this.btnLegible.Enabled = false;
            this.btnTomarActa.Enabled = true;
            this.cmbCasilla.Enabled = false;
            this.cmbSeccion.Enabled = false;

            imageBox.Image = null;

        }

        private void ReiniciarCeros()
        {
            try
            {
                foreach (TextBox datos in this.textBoxes)
                {
                    datos.Text = "0";
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
                    this.cargarComboCasilla();
                }
                    
                else
                {
                    this.tblPanaelPartidos.ResumeLayout();
                    this.tblPanaelPartidos.Visible = true;
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
            this.Close();
        }

        #region Overridden Methods

        //protected override void OnLoad(EventArgs e)
        //{
        //    //base.OnLoad(e);

        //    //this.FillZoomLevels();
            
        //    //this.OpenImage(Resources.iepc);

        //    //imageBox.SelectionMode = ImageBoxSelectionMode.Zoom;
        //    //imageBox.AllowClickZoom = true;
        //}

        #endregion

        #region Private Members

        private void DrawBox(Graphics graphics, Color color, RectangleF rectangle, double scale)
        {
            float penWidth;

            penWidth = 2 * (float)scale;

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(64, color)))
            {
                graphics.FillRectangle(brush, rectangle);
            }

            using (Pen pen = new Pen(color, penWidth)
            {
                DashStyle = DashStyle.Dot,
                DashCap = DashCap.Round
            })
            {
                graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }
        

        private void OpenImage(Image image)
        {
            imageBox.Image = image;
            imageBox.ZoomToFit();

            this.UpdateStatusBar();
            this.UpdatePreviewImage();
        }
        
        private void UpdatePreviewImage()
        {
            if (_previewImage != null)
            {
                _previewImage.Dispose();
            }

            _previewImage = imageBox.GetSelectedImage();

            //previewImageBox.Image = _previewImage;
        }

        private void UpdateStatusBar()
        {
            //zoomLevelsToolStripComboBox.Text = string.Format("{0}%", imageBox.Zoom);
            //autoScrollPositionToolStripStatusLabel.Text = this.FormatPoint(imageBox.AutoScrollPosition);
            //imageSizeToolStripStatusLabel.Text = this.FormatRectangle(imageBox.GetImageViewPort());
            //zoomToolStripStatusLabel.Text = string.Format("{0}%", imageBox.Zoom);
        }

        #endregion

        #region Event Handlers

        private void actualSizeToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ActualSize();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetImage(imageBox.GetSelectedImage() ?? imageBox.Image);
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void imageBox_MouseLeave(object sender, EventArgs e)
        {
            //cursorToolStripStatusLabel.Text = string.Empty;
        }

        private void imageBox_MouseMove(object sender, MouseEventArgs e)
        {
            //this.UpdateCursorPosition(e.Location);
        }

        private void imageBox_Paint(object sender, PaintEventArgs e)
        {
            // highlight the image
            //if (showImageRegionToolStripButton.Checked)
            //{
            //    this.DrawBox(e.Graphics, Color.CornflowerBlue, imageBox.GetImageViewPort(), 1);
            //}

            //// show the region that will be drawn from the source image
            //if (showSourceImageRegionToolStripButton.Checked)
            //{
            //    this.DrawBox(e.Graphics, Color.Firebrick, new RectangleF(imageBox.GetImageViewPort().Location, imageBox.GetSourceImageRegion().Size), 1);
            //}
        }

        private void imageBox_Resize(object sender, EventArgs e)
        {
            this.UpdateStatusBar();
        }

        private void imageBox_Scroll(object sender, ScrollEventArgs e)
        {
            this.UpdateStatusBar();
        }

        private void imageBox_Selected(object sender, EventArgs e)
        {
            this.UpdatePreviewImage();
        }

        private void imageBox_SelectionRegionChanged(object sender, EventArgs e)
        {
            //selectionToolStripStatusLabel.Text = this.FormatRectangle(imageBox.SelectionRegion);
        }

        private void imageBox_ZoomChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Haciendo Zom");
            //this.HorizontalScroll.Maximum = 0;
            //this.AutoScroll = false;
            //this.VerticalScroll.Visible = false;
            //this.AutoScroll = true;
            this.UpdateStatusBar();
        }

        private void imageBox_ZoomLevelsChanged(object sender, EventArgs e)
        {
            //this.FillZoomLevels();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "All Supported Images (*.bmp;*.dib;*.rle;*.gif;*.jpg;*.png)|*.bmp;*.dib;*.rle;*.gif;*.jpg;*.png|Bitmaps (*.bmp;*.dib;*.rle)|*.bmp;*.dib;*.rle|Graphics Interchange Format (*.gif)|*.gif|Joint Photographic Experts (*.jpg)|*.jpg|Portable Network Graphics (*.png)|*.png|All Files (*.*)|*.*";
                dialog.DefaultExt = "png";

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        this.OpenImage(Image.FromFile(dialog.FileName));
                    }
                    catch (Exception ex)
                    {
                        msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                        msgBox.ShowDialog(this);
                    }
                }
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageBox.SelectAll();
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageBox.SelectNone();
        }

        private void showImageRegionToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.Invalidate();
        }

        private void zoomInToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ZoomIn();
        }

        

        private void zoomOutToolStripButton_Click(object sender, EventArgs e)
        {
            imageBox.ZoomOut();
        }
        private void cmbCasilla_SelectedValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Se cambio combo");
        }

        

        private void cmbSeccion_SelectedValueChanged(object sender, EventArgs e)
        {
            //this.cargarComboCasilla();
            if(flagCombo > 0)
            {
                this.ClearDataTable();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int id_casilla = Convert.ToInt32(cmbCasilla.SelectedValue);
                if (id_casilla == 0)
                    throw new Exception("Selecciona una Casilla");
                msgBox = new MsgBox(this.MdiParent, "¿Guardar datos del Acta?", "Atención", MessageBoxButtons.YesNo, "Question");
                DialogResult result = msgBox.ShowDialog(this);
                if (result == DialogResult.Yes)
                {
                    this.guardarRegistroVotos();
                }

            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        #endregion

        private void btnTomarActa_Click(object sender, EventArgs e)
        {
            try
            {
                Loadingbox = new Loading(this, "Cargando");
                Loadingbox.Show(this);
                rgActas = new RegistroActasGenerales();
                sice_ar_documentos res = rgActas.TomarCasilla();
                this.btnTomarActa.Enabled = false;
                if (res != null)
                {
                    //this.ClearDataTable();
                    this.idDocumento = res.id;
                    flagCombo = 0;
                    this.cargarComboSeccion();
                    this.CargarImagen(res);
                    Loadingbox.Close();
                    flagCombo++;
                    msgBox = new MsgBox(this, "Acta Asignada", "Atención", MessageBoxButtons.OK, "Ok");
                    msgBox.ShowDialog(this);
                }
                else
                {
                    btnTomarActa.Enabled = true;
                    Loadingbox.Close();
                    //throw new Exception("No hay Actas disponibles");                   
                    msgBox = new MsgBox(this, "No hay actas disponibles", "Atención", MessageBoxButtons.OK, "Advertencia");
                    msgBox.ShowDialog(this);

                }



            }
            catch (Exception ex)
            {
                Loadingbox.Close();
                this.BloquearControles();
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
            


        }

        private void cmbCasilla_SelectedValueChanged_1(object sender, EventArgs e)
        {
            try
            {
                int? selected = Convert.ToInt32(cmbCasilla.SelectedValue);
                if (selected != null && selected != 0)
                    this.verificarCasilla();
            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLegible_Click(object sender, EventArgs e)
        {
            try
            {
                msgBox = new MsgBox(this.MdiParent, "¿Marcar acta como no Legible?\nSera enviada a revisión", "Atención", MessageBoxButtons.YesNo, "Question");
                DialogResult result = msgBox.ShowDialog(this);
                if (result == DialogResult.Yes)
                {
                    this.NoLegible();
                }
            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
            
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                this.ReiniciarCeros();
            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void Loadingbox_Activated(object sender, EventArgs e)
        {
            Loadingbox.Activate();
        }
    }

}
