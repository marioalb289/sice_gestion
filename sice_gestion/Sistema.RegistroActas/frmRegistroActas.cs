using Cyotek.Windows.Forms;
using Sistema.RegistroActas.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sistema.Generales;
using Sistema.DataModel;

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
        private int flagComboCasilla = 0;
        private int idDocumento = 0;
        private int totalCandidatos = 0;

        #endregion

        public frmRegistroActas()
        {
            InitializeComponent();
            imageBox.MouseWheel += new MouseEventHandler(DoNothing_MouseWheel);
            this.cargarActaYaAsignada();
            //MessageBox.Show(LoginInfo.nombre);
        }
        

        private void DoNothing_MouseWheel(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("Se mueve rueda del mouse");
            HandledMouseEventArgs ee = (HandledMouseEventArgs)e;
            ee.Handled = true;

        }

        private void cargarActaYaAsignada()
        {
            //this.ClearDataTable();
            rgActas = new RegistroActasGenerales();
            sice_ar_documentos doc = rgActas.BuscarActaAsignada();
            if(doc != null)
            {
                this.idDocumento = doc.id;
                flagCombo = 0;
                this.cargarComboSeccion();
                flagCombo++;
                MessageBox.Show("Acta Asginada");
            }
            

        }

        private void cargarComboSeccion()
        {
            try
            {
                //Aqui deberia seleccionar una de las imegenes que vengan del repositorio
                this.OpenImage(Resources.iepc);                

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
                MessageBox.Show(ex.Message);
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
                    caGp.Insert(0, new SeccionCasilla() { id = 0, casilla = "Seleccionar Casilla" });
                    cmbCasilla.DataSource = caGp;
                    //cmbCasilla.SelectedIndex = 1;
                    cmbCasilla.Enabled = true;

                    cmbCasilla.Enabled = true;
                    this.cargarCandidatosResultados(caGp[0].distrito);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void verificarCasilla()
        {
            try
            {
                rgActas = new RegistroActasGenerales();
                if (rgActas.verificarCasillaValida(Convert.ToInt32(cmbCasilla.SelectedValue)))
                {
                    MessageBox.Show("Casilla ya Resgistrada y válida");
                    cmbCasilla.SelectedIndex = 0;
                    this.btnGuardar.Enabled = false;
                    this.btnLimpiar.Enabled = false;
                    this.btnLegible.Enabled = false;
                }
                this.btnGuardar.Enabled = true;
                this.btnLimpiar.Enabled = true;
                this.btnLegible.Enabled = true;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
            }

        }

        private void guardarRegistroVotos()
        {
            try
            {
                List<sice_ar_votos> lista_votos = new List<sice_ar_votos>();
                foreach(TextBox datos in this.textBoxes)
                {
                    double num;
                    if(double.TryParse(datos.Text, out num))
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
                            id_casilla = Convert.ToInt32(cmbCasilla.SelectedValue),
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
                    switch (res)
                    {
                        case 1:
                            MessageBox.Show("Datos Guardados correctamente");
                            break;
                        case 2:
                            MessageBox.Show("Acta enviada a Reivision");
                            break;
                        case 3:
                            MessageBox.Show("Acta validada correctamente");
                            break;
                        case 4:
                            MessageBox.Show("Ya existe un documento Asingado a esta casilla y en proceso de validación. \nEl documento actual será enviado a Revisión para su evaluacion");
                            break;
                            
                    }
                    this.BloquearControles();

                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void BloquearControles()
        {
            this.ClearDataTable(true);
            this.btnGuardar.Enabled = false;
            this.btnLimpiar.Enabled = false;
            this.btnLegible.Enabled = false;
            this.btnTomarActa.Enabled = true;
            this.cmbCasilla.Enabled = false;
            this.cmbSeccion.Enabled = false;

            imageBox.Image = null;

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
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Overridden Methods

        protected override void OnLoad(EventArgs e)
        {
            //base.OnLoad(e);

            //this.FillZoomLevels();
            
            //this.OpenImage(Resources.iepc);

            //imageBox.SelectionMode = ImageBoxSelectionMode.Zoom;
            //imageBox.AllowClickZoom = true;
        }

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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                this.guardarRegistroVotos();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void btnTomarActa_Click(object sender, EventArgs e)
        {
            try
            {
                /*Aqui se debe programar la logica para buscar el archivo
                 * 
                 * 
                 * 
                 * 
                */
                rgActas = new RegistroActasGenerales();
                int res = rgActas.TomarCasilla("abcd16", "ruta/ruta");
                this.btnTomarActa.Enabled = false;
                if (res == 0)
                {
                    throw new Exception("No hay Actas disponibles");
                }                    
                else
                {
                    //this.ClearDataTable();
                    this.idDocumento = res;
                    flagCombo = 0;
                    this.cargarComboSeccion();
                    flagCombo++;
                    MessageBox.Show("Acta Asginada");
                }
                    


            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}
