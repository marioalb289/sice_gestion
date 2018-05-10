using Cyotek.Windows.Forms;
using Sistema.DataModel;
using Sistema.Generales;
using Sistema.RegistroActasLocal.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Sistema.RegistroActasLocal
{
    public partial class ModificarActas : Form
    {
        #region Instance Fields

        private Image _previewImage;
        private RegistroLocalGenerales rgActas;
        private List<SeccionCasillaConsecutivo> sc;
        private PictureBox[] pictureBoxes;
        private TextBox[] textBoxes;
        private Panel[] panels;
        private Label[] labelsName;
        private int flagCombo = 0;
        private int idDocumento = 0;
        private int totalCandidatos = 0;
        private MsgBox msgBox;
        private Loading Loadingbox;
        private sice_ar_documentos documento;
        Image imageLoad;

        #endregion

        public ModificarActas()
        {
            InitializeComponent();
            this.Activated += ModificarActas_Activated;
            this.cargarComboSeccion();
        }

        private void ModificarActas_Activated(object sender, EventArgs e)
        {
            if (Loadingbox != null)
            {
                Loadingbox.Activate();
                Loadingbox.Focus();
            }

        }

        private void ModificarActas_Load(object sender, EventArgs e)
        {
            imageBox.MouseWheel += new MouseEventHandler(DoNothing_MouseWheel);

        }

        private void ModificarActas_Shown(object sender, EventArgs e)
        {
            this.MdiParent.WindowState = FormWindowState.Maximized;
        }


        private void DoNothing_MouseWheel(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("Se mueve rueda del mouse");
            HandledMouseEventArgs ee = (HandledMouseEventArgs)e;
            ee.Handled = true;

        }

        private void cargarImagen()
        {
            try
            {
                //iepcdgo.org\mario.canales
                //var credentials = new NetworkCredential("mario.canales@IEPCDGO.org", "Iepc2018");
                Loadingbox = new Loading(this, "Cargando");
                Loadingbox.Show(this);
                rgActas = new RegistroLocalGenerales();
                this.documento = rgActas.getDocumentoCasilla(Convert.ToInt32(cmbCasilla.SelectedValue));
                if (documento != null)
                {
                    //ftp ftpClient = new ftp(Configuracion.NetworkFtp, Configuracion.User, Configuracion.Pass);
                    imageLoad = null;
                    this.idDocumento = documento.id;
                    string curFile = @documento.ruta + documento.nombre;
                    if (File.Exists(curFile))
                    {
                        imageLoad = new Bitmap(@documento.ruta + documento.nombre);
                    }
                    else
                    {
                        ftp ftpClient = new ftp(Configuracion.NetworkFtp, Configuracion.User, Configuracion.Pass);
                        imageLoad = ftpClient.downloadImage(Configuracion.Repo + "/" + documento.nombre);
                    }
                    this.btnGirar.Enabled = true;
                    this.OpenImage(imageLoad);
                    imageBox.Enabled = true;
                    //Cargar imagenes de los filtros
                    Loadingbox.Close();
                    this.btnGuardar.Enabled = true;
                    this.cargarComboSeccionNuevo();
                }
                else
                {
                    Loadingbox.Close();
                    msgBox = new MsgBox(this, "Acta No Registrada", "Atención", MessageBoxButtons.OK, "Advertencia");
                    msgBox.ShowDialog(this);
                }



            }
            catch (Exception ex)
            {
                Loadingbox.Close();
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
                rgActas = new RegistroLocalGenerales();
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

        private void cargarComboSeccionNuevo()
        {
            try
            {
                cmbSeccionNuevo.DataSource = null;
                cmbSeccionNuevo.DisplayMember = "Seccion";
                cmbSeccionNuevo.ValueMember = "Seccion";

                var seGp = sc.GroupBy(x => x.seccion, x => x.id, (seccion, idSe) => new { IdSeccion = idSe, Seccion = seccion }).Select(g => g.Seccion).ToList();
                cmbSeccionNuevo.DataSource = seGp;
                cmbSeccionNuevo.Enabled = true;

                this.cargarComboCasillaNuevo();

            }
            catch (Exception ex)
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
                    caGp.Insert(0, new SeccionCasillaConsecutivo() { id = 0, casilla = "Seleccionar Casilla" });
                    cmbCasilla.DataSource = caGp;
                    //cmbCasilla.SelectedIndex = 1;
                    cmbCasilla.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cargarComboCasillaNuevo()
        {
            try
            {
                if (sc != null)
                {
                    cmbCasillaNuevo.DataSource = null;
                    cmbCasillaNuevo.DisplayMember = "casilla";
                    cmbCasillaNuevo.ValueMember = "id";
                    var caGp = (from p in this.sc where p.seccion == Convert.ToInt32(cmbSeccionNuevo.SelectedValue) select p).ToList();
                    if(caGp.Count > 0)
                    {
                        int distrito = caGp[0].distrito;
                        caGp.Insert(0, new SeccionCasillaConsecutivo() { id = 0, casilla = "Seleccionar Casilla" });
                        cmbCasillaNuevo.DataSource = caGp;
                        //cmbCasilla.SelectedIndex = 1;
                        cmbCasillaNuevo.Enabled = true;
                    }
                    
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
                int? casillaNuevoSel = Convert.ToInt32(cmbCasillaNuevo.SelectedValue);
                int casillaActual = Convert.ToInt32(cmbCasilla.SelectedValue);
                if (Convert.ToInt32(cmbCasillaNuevo.SelectedValue) == casillaActual)
                {
                    cmbCasillaNuevo.SelectedIndex = 0;
                    throw new Exception("Debe seleccionar una casilla diferente a la original del Acta");
                }
                    
                rgActas = new RegistroLocalGenerales();
                if(casillaNuevoSel != 0)
                {
                    int res = rgActas.verificarCasillaValida((int)casillaNuevoSel);
                    if (res != 0)
                    {
                        string estatus = "";
                        if (res == 1)
                            estatus = "COTEJO";
                        else
                            estatus = "VALIDO";
                        msgBox = new MsgBox(this.MdiParent, "Casilla ya Registrada y en estatus: " + estatus + "\n¿Asginar esta casilla al documento Actual?", "Atención", MessageBoxButtons.YesNo, "Advertencia");
                        DialogResult result = msgBox.ShowDialog(this);
                        if (result == DialogResult.No)
                        {
                            cmbCasillaNuevo.SelectedIndex = 0;
                            //this.btnGuardar.Enabled = false;
                        }
                        else
                        {
                            //this.btnGuardar.Enabled = true;
                        }
                    }
                    else
                    {
                        //this.btnGuardar.Enabled = true;
                    }
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
                //if (this.idDocumento == 0)
                //    throw new Exception("No se pudo marcar el acata como NO LEGIBLE");
                //rgActas = new RegistroLocalGenerales();
                //if (rgActas.EnviarRevision(this.idDocumento, "NO LEGIBLE"))
                //{
                //    msgBox = new MsgBox(this, "Acta marcada como NO LEGIBLE", "Atención", MessageBoxButtons.OK, "Ok");
                //    msgBox.ShowDialog(this);
                //    this.BloquearControles();
                //}
                //else
                //{
                //    msgBox = new MsgBox(this, "No se pudo marcar el acata como NO LEGIBLE", "Atención", MessageBoxButtons.OK, "Error");
                //    msgBox.ShowDialog(this);
                //}



            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }



        private void guardarActaIdentificada()
        {
            try
            {
                int id_casilla = Convert.ToInt32(cmbCasillaNuevo.SelectedValue);
                if(id_casilla == 0)
                {
                    id_casilla = Convert.ToInt32(cmbCasilla.SelectedValue);
                }
                //if (id_casilla == 0)
                //    throw new Exception("Selecciona una Casilla");

                rgActas = new RegistroLocalGenerales();
                int res = rgActas.IdentificarActa(this.idDocumento, id_casilla);
                switch (res)
                {
                    case 1:
                        msgBox = new MsgBox(this, "Datos Guardados correctamente", "Atención", MessageBoxButtons.OK, "Ok");
                        msgBox.ShowDialog(this);
                        break;
                    case 0:
                        throw new Exception("Error al Identificar acta");

                }
                this.BloquearControles();



            }
            catch (Exception ex)
            {
                Loadingbox.Close();
                this.tableLayoutPanel2.Enabled = true;
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void BloquearControles()
        {
            cmbSeccion.SelectedIndex = 0;
            this.btnGuardar.Enabled = false;
            this.cmbCasillaNuevo.Enabled = false;
            this.cmbSeccionNuevo.Enabled = false;
            imageBox.Image = null;
            this.btnGirar.Enabled = false;
            this.cmbCasilla.SelectedIndex = 0;

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
            cmbSeccionNuevo.Enabled = false;
            cmbCasillaNuevo.Enabled = false;
            this.btnGuardar.Enabled = false;
            this.btnGirar.Enabled = false;
            var data = cmbSeccionNuevo.DataSource;
            if(data != null)
                cmbSeccionNuevo.SelectedIndex = 0;
            this.cargarComboCasilla();

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                //int id_casilla = Convert.ToInt32(cmbCasillaNuevo.SelectedValue);
                //if (id_casilla == 0)
                //    throw new Exception("Selecciona una Casilla");
                msgBox = new MsgBox(this.MdiParent, "¿Guardar datos del Acta?", "Atención", MessageBoxButtons.YesNo, "Question");
                DialogResult result = msgBox.ShowDialog(this);
                if (result == DialogResult.Yes)
                {
                    this.guardarActaIdentificada();
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        #endregion
        

        private void cmbCasilla_SelectedValueChanged_1(object sender, EventArgs e)
        {
            try
            {
                //int? selected = Convert.ToInt32(cmbCasilla.SelectedValue);
                //if (selected != null && selected != 0)
                //    this.verificarCasilla();
                imageBox.Image = null;
                imageBox.Enabled = false;
                int? selected = Convert.ToInt32(cmbCasilla.SelectedValue);
                if (selected != null && selected != 0)
                    this.cargarImagen();
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Loadingbox_Activated(object sender, EventArgs e)
        {
            Loadingbox.Activate();
        }

        private void cmbSeccionNuevo_SelectedValueChanged(object sender, EventArgs e)
        {
            this.cargarComboCasillaNuevo();
        }

        private void cmbSeccionNueva_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                //this.btnGuardar.Enabled = false;
                int? selected = Convert.ToInt32(cmbCasilla.SelectedValue);
                if (selected != null && selected != 0)
                    this.verificarCasilla();
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnGirar_Click(object sender, EventArgs e)
        {
            try
            {
                imageLoad.RotateFlip(RotateFlipType.Rotate90FlipXY);


                if (imageLoad != null)
                    this.OpenImage(imageLoad);
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }
    }

}
