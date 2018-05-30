namespace sice_gestion
{
    partial class FrmModulos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmModulos));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.btnComputos = new System.Windows.Forms.Button();
            this.btnConf = new System.Windows.Forms.Button();
            this.btnRegistroActas = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnComputos, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnConf, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnRegistroActas, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.24242F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75.75758F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 561);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 2);
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(199, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(386, 47);
            this.label2.TabIndex = 16;
            this.label2.Text = "Seleccionar Sistema";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnComputos
            // 
            this.btnComputos.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnComputos.BackColor = System.Drawing.Color.Transparent;
            this.btnComputos.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnComputos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnComputos.Image = ((System.Drawing.Image)(resources.GetObject("btnComputos.Image")));
            this.btnComputos.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnComputos.Location = new System.Drawing.Point(420, 280);
            this.btnComputos.Name = "btnComputos";
            this.btnComputos.Size = new System.Drawing.Size(140, 135);
            this.btnComputos.TabIndex = 14;
            this.btnComputos.Text = "Cómputos Electorales";
            this.btnComputos.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnComputos.UseVisualStyleBackColor = false;
            this.btnComputos.Click += new System.EventHandler(this.btnComputos_Click);
            // 
            // btnConf
            // 
            this.btnConf.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnConf.BackColor = System.Drawing.Color.Transparent;
            this.btnConf.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnConf.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConf.Image = ((System.Drawing.Image)(resources.GetObject("btnConf.Image")));
            this.btnConf.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnConf.Location = new System.Drawing.Point(28, 280);
            this.btnConf.Name = "btnConf";
            this.btnConf.Size = new System.Drawing.Size(140, 135);
            this.btnConf.TabIndex = 17;
            this.btnConf.Text = "Configuración";
            this.btnConf.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnConf.UseVisualStyleBackColor = false;
            this.btnConf.Visible = false;
            this.btnConf.Click += new System.EventHandler(this.btnConf_Click);
            // 
            // btnRegistroActas
            // 
            this.btnRegistroActas.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRegistroActas.BackColor = System.Drawing.Color.Transparent;
            this.btnRegistroActas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnRegistroActas.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegistroActas.Image = ((System.Drawing.Image)(resources.GetObject("btnRegistroActas.Image")));
            this.btnRegistroActas.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRegistroActas.Location = new System.Drawing.Point(224, 280);
            this.btnRegistroActas.Name = "btnRegistroActas";
            this.btnRegistroActas.Size = new System.Drawing.Size(140, 135);
            this.btnRegistroActas.TabIndex = 13;
            this.btnRegistroActas.Text = "Registro de Actas";
            this.btnRegistroActas.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRegistroActas.UseVisualStyleBackColor = false;
            this.btnRegistroActas.Click += new System.EventHandler(this.btnRegistroActas_Click);
            // 
            // FrmModulos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmModulos";
            this.Opacity = 0.2D;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRegistroActas;
        private System.Windows.Forms.Button btnComputos;
        private System.Windows.Forms.Button btnConf;
    }
}