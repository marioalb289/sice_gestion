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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema.RegistroActas
{
    public partial class frmConsultaActas : Form
    {
        private Image _previewImage;
        private List<SeccionCasilla> sc;
        private RegistroActasGenerales rgActas;
        private int flagCombo = 0;
        Image imageLoad;
        string nameImageLoad = "";
        private MsgBox msgBox;
        private PictureBox[] pictureBoxes;
        private TextBox[] textBoxes;
        private Panel[] panels;
        private Label[] labelsName;

        public frmConsultaActas()
        {

            //this.MdiParent.WindowState = FormWindowState.Maximized;
            InitializeComponent();
            
        }
        private void frmConsultaActas_Load(object sender, EventArgs e)
        {
            
            imageBox.MouseWheel += new MouseEventHandler(DoNothing_MouseWheel);
            this.cargarComboSeccion();
        }

        private void DoNothing_MouseWheel(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("Se mueve rueda del mouse");
            HandledMouseEventArgs ee = (HandledMouseEventArgs)e;
            ee.Handled = true;

        }

        private void cargarComboSeccion()
        {
            try
            {
                cmbSeccion.DataSource = null;
                cmbSeccion.DisplayMember = "Seccion";
                cmbSeccion.ValueMember = "Seccion";
                rgActas = new RegistroActasGenerales();
                if (this.sc == null)
                {
                    this.sc = rgActas.ListaSescciones();
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
                    caGp.Insert(0, new SeccionCasilla() { id = 0, casilla = "Seleccionar Casilla" });
                    cmbCasilla.DataSource = caGp;
                    //cmbCasilla.SelectedIndex = 1;
                    cmbCasilla.Enabled = true;

                    cmbCasilla.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this,ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void ClearImage()
        {
            imageBox.Image = null;
            imageBox.Enabled = false;
            this.btnGuardar.Enabled = false;
        }

        private void cargarImagen()
        {
            try
            {
                rgActas = new RegistroActasGenerales();
                sice_ar_documentos documento = rgActas.getDocumentoCasilla(Convert.ToInt32(cmbCasilla.SelectedValue));
                if(documento != null)
                {
                    ftp ftpClient = new ftp(Configuracion.NetworkFtp, Configuracion.User, Configuracion.Pass);
                    Image imagen = ftpClient.downloadImage(Configuracion.Repo + "/" + documento.nombre);
                    this.OpenImage(imagen);
                    this.nameImageLoad = documento.nombre;
                    imageBox.Enabled = true;
                    btnGuardar.Enabled = true;
                    //Limpiar tablas y cargar datos de votos
                    this.ClearDataTable();
                }
                else
                {
                    msgBox = new MsgBox(this,"Acta No Registrada", "Atención", MessageBoxButtons.OK, "Advertencia");
                    msgBox.ShowDialog(this);
                    //Limpiar tablas
                    this.ClearDataTable(true);
                }

                

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void cargarResultadosVotos()
        {
            try
            {
                rgActas = new RegistroActasGenerales();
                List<CandidatosVotos> lsCandidatosVotos = rgActas.ListaResultadosCasilla(Convert.ToInt32(cmbCasilla.SelectedValue));
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
                if (lsCandidatosVotos != null)
                {
                    this.pictureBoxes = new PictureBox[lsCandidatosVotos.Count];
                    this.textBoxes = new TextBox[lsCandidatosVotos.Count];
                    this.panels = new Panel[lsCandidatosVotos.Count];
                    this.labelsName = new Label[lsCandidatosVotos.Count];
                    this.tblPanaelPartidos.RowCount = 1;

                    

                    for (int i = 0; i < lsCandidatosVotos.Count; i++)
                    {
                        pictureBoxes[i] = new PictureBox();
                        textBoxes[i] = new TextBox();
                        labelsName[i] = new Label();
                        panels[i] = new Panel();

                        this.tblPanaelPartidos.RowCount = this.tblPanaelPartidos.RowCount + 1;

                        this.tblPanaelPartidos.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));

                        //IMAGEN DEL PARTIDO
                        pictureBoxes[i].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                        pictureBoxes[i].Dock = System.Windows.Forms.DockStyle.Top;
                        pictureBoxes[i].Image = (lsCandidatosVotos[i].tipo == "NULO") ? (System.Drawing.Image)(Resources.nulos) : (lsCandidatosVotos[i].tipo == "NO REGISTRADO") ? (System.Drawing.Image)(Resources.no_regis) : (System.Drawing.Image)(resources.GetObject("pictureBox3.Image"));
                        pictureBoxes[i].Location = new System.Drawing.Point(15, 57);
                        pictureBoxes[i].Name = "pictureBox" + i;
                        pictureBoxes[i].Size = new System.Drawing.Size(75, 34);
                        pictureBoxes[i].SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                        pictureBoxes[i].TabIndex = 20 + i;
                        pictureBoxes[i].TabStop = false;

                        //ETIQUETA DEL NOMBRE DEL CANDIADATO
                        labelsName[i].Dock = System.Windows.Forms.DockStyle.Top;
                        labelsName[i].Location = new System.Drawing.Point(0, 28);
                        labelsName[i].Name = "labelNameCandidato" + i;
                        labelsName[i].Size = new System.Drawing.Size(75, 13);
                        labelsName[i].TabIndex = 5;
                        labelsName[i].Text = lsCandidatosVotos[i].tipo == "NULO" ? "Votos Nulos" : lsCandidatosVotos[i].tipo == "NO REGISTRADO" ? "Candidato No Registrado": lsCandidatosVotos[i].candidato; 
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

                        //Texbox para captura de votos
                        textBoxes[i].Anchor = System.Windows.Forms.AnchorStyles.None;
                        textBoxes[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        textBoxes[i].Location = new System.Drawing.Point(106, 64);
                        textBoxes[i].Name = "textBox" + i;
                        textBoxes[i].Size = new System.Drawing.Size(100, 29);
                        textBoxes[i].TabIndex = 100 + i;
                        textBoxes[i].Enabled = false;
                        //Votos nulos 0 Candidato no registrado -1
                        textBoxes[i].Tag = lsCandidatosVotos[i].id.ToString();
                        textBoxes[i].MaxLength = 3;
                        textBoxes[i].Text = lsCandidatosVotos[i].votos.ToString();
                        textBoxes[i].TextAlign = HorizontalAlignment.Center;

                        this.tblPanaelPartidos.Controls.Add(textBoxes[i], 1, i + 1);



                    }


                    this.tblPanaelPartidos.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
                    this.tblPanaelPartidos.ResumeLayout(false);
                    this.tblPanaelPartidos.Visible = true;
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
                    this.cargarResultadosVotos();
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


        private void OpenImage(Image image)
        {
            imageBox.Image = image;
            imageBox.ZoomToFit();

            this.UpdateStatusBar();
            this.UpdatePreviewImage();
        }
        
        private void guardarImagencomo()
        {
            try
            {
                if(this.imageLoad != null)
                {
                    SaveFileDialog fichero = new SaveFileDialog();
                    fichero.Filter = "Images(*.jpg)|*.jpg";
                    fichero.FileName = this.nameImageLoad;
                    ImageFormat format = ImageFormat.Png;

                    if (fichero.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        imageLoad.Save(fichero.FileName);
                    }
                }


            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }


        private void UpdateStatusBar()
        {
            //zoomLevelsToolStripComboBox.Text = string.Format("{0}%", imageBox.Zoom);
            //autoScrollPositionToolStripStatusLabel.Text = this.FormatPoint(imageBox.AutoScrollPosition);
            //imageSizeToolStripStatusLabel.Text = this.FormatRectangle(imageBox.GetImageViewPort());
            //zoomToolStripStatusLabel.Text = string.Format("{0}%", imageBox.Zoom);
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
            try
            {
                this.ClearDataTable(true);
                this.ClearImage();
                this.cargarComboCasilla();
            }
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
            
        }

        private void cmbCasilla_SelectedValueChanged_1(object sender, EventArgs e)
        {
            this.ClearImage();
            int? selected = Convert.ToInt32(cmbCasilla.SelectedValue);
            if (selected != null && selected != 0)
                this.cargarImagen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            this.guardarImagencomo();
        }

        private void frmConsultaActas_Shown(object sender, EventArgs e)
        {
            this.MdiParent.WindowState = FormWindowState.Maximized;
        }
    }
}
