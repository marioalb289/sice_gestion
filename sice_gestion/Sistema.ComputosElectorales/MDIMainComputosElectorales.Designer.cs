namespace Sistema.ComputosElectorales
{
    partial class MDIMainComputosElectorales
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.pnlDescarga = new System.Windows.Forms.Panel();
            this.lblGenerarExcel = new System.Windows.Forms.Label();
            this.lblDescargando = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureExcel = new System.Windows.Forms.PictureBox();
            this.pictureDownload = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlDescarga.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureExcel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDownload)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1008, 75);
            this.panel1.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.pnlDescarga, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 71F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1004, 71);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblUsuario);
            this.panel2.Controls.Add(this.pictureBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(756, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(245, 65);
            this.panel2.TabIndex = 1;
            // 
            // lblUsuario
            // 
            this.lblUsuario.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.Location = new System.Drawing.Point(49, 0);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(196, 65);
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "Nombre Usuario";
            this.lblUsuario.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlDescarga
            // 
            this.pnlDescarga.Controls.Add(this.lblGenerarExcel);
            this.pnlDescarga.Controls.Add(this.pictureExcel);
            this.pnlDescarga.Controls.Add(this.lblDescargando);
            this.pnlDescarga.Controls.Add(this.pictureDownload);
            this.pnlDescarga.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDescarga.Location = new System.Drawing.Point(254, 3);
            this.pnlDescarga.Name = "pnlDescarga";
            this.pnlDescarga.Size = new System.Drawing.Size(496, 65);
            this.pnlDescarga.TabIndex = 2;
            // 
            // lblGenerarExcel
            // 
            this.lblGenerarExcel.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblGenerarExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGenerarExcel.Location = new System.Drawing.Point(-50, 0);
            this.lblGenerarExcel.Name = "lblGenerarExcel";
            this.lblGenerarExcel.Size = new System.Drawing.Size(173, 65);
            this.lblGenerarExcel.TabIndex = 6;
            this.lblGenerarExcel.Text = "Generando Excel";
            this.lblGenerarExcel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblGenerarExcel.Visible = false;
            // 
            // lblDescargando
            // 
            this.lblDescargando.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblDescargando.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescargando.Location = new System.Drawing.Point(223, 0);
            this.lblDescargando.Name = "lblDescargando";
            this.lblDescargando.Size = new System.Drawing.Size(173, 65);
            this.lblDescargando.TabIndex = 4;
            this.lblDescargando.Text = "Descargando Datos";
            this.lblDescargando.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDescargando.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::Sistema.ComputosElectorales.Properties.Resources.iepc1;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(245, 65);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox2.Image = global::Sistema.ComputosElectorales.Properties.Resources.basic;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(49, 65);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // pictureExcel
            // 
            this.pictureExcel.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureExcel.Image = global::Sistema.ComputosElectorales.Properties.Resources.loadingdatacenter1;
            this.pictureExcel.Location = new System.Drawing.Point(123, 0);
            this.pictureExcel.Name = "pictureExcel";
            this.pictureExcel.Size = new System.Drawing.Size(100, 65);
            this.pictureExcel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureExcel.TabIndex = 5;
            this.pictureExcel.TabStop = false;
            this.pictureExcel.Visible = false;
            // 
            // pictureDownload
            // 
            this.pictureDownload.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureDownload.Image = global::Sistema.ComputosElectorales.Properties.Resources.loadingdatacenter1;
            this.pictureDownload.Location = new System.Drawing.Point(396, 0);
            this.pictureDownload.Name = "pictureDownload";
            this.pictureDownload.Size = new System.Drawing.Size(100, 65);
            this.pictureDownload.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureDownload.TabIndex = 3;
            this.pictureDownload.TabStop = false;
            this.pictureDownload.Visible = false;
            // 
            // MDIMainComputosElectorales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.panel1);
            this.IsMdiContainer = true;
            this.Name = "MDIMainComputosElectorales";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sistema de Cómputos Electorales";
            this.Load += new System.EventHandler(this.MDIMain_Load);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnlDescarga.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureExcel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDownload)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.PictureBox pictureBox2;
        public System.Windows.Forms.Panel pnlDescarga;
        public System.Windows.Forms.Label lblDescargando;
        public System.Windows.Forms.PictureBox pictureDownload;
        public System.Windows.Forms.Label lblGenerarExcel;
        public System.Windows.Forms.PictureBox pictureExcel;
    }
}
