namespace sice_gestion
{
    partial class Configuracion
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConsultarActas = new System.Windows.Forms.Button();
            this.btnInicializarTablas = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCargarRespaldo = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btnCargarRespaldo, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnConsultarActas, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnInicializarTablas, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.button1, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 600);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.SetColumnSpan(this.label2, 3);
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(190, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(420, 47);
            this.label2.TabIndex = 16;
            this.label2.Text = "Configuración Inicial";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnConsultarActas
            // 
            this.btnConsultarActas.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnConsultarActas.BackColor = System.Drawing.Color.Transparent;
            this.btnConsultarActas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnConsultarActas.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConsultarActas.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnConsultarActas.Location = new System.Drawing.Point(172, 307);
            this.btnConsultarActas.Name = "btnConsultarActas";
            this.btnConsultarActas.Size = new System.Drawing.Size(136, 135);
            this.btnConsultarActas.TabIndex = 14;
            this.btnConsultarActas.Text = "Crear Usuarios";
            this.btnConsultarActas.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnConsultarActas.UseVisualStyleBackColor = false;
            // 
            // btnInicializarTablas
            // 
            this.btnInicializarTablas.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnInicializarTablas.BackColor = System.Drawing.Color.Transparent;
            this.btnInicializarTablas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnInicializarTablas.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInicializarTablas.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnInicializarTablas.Location = new System.Drawing.Point(12, 307);
            this.btnInicializarTablas.Name = "btnInicializarTablas";
            this.btnInicializarTablas.Size = new System.Drawing.Size(136, 135);
            this.btnInicializarTablas.TabIndex = 13;
            this.btnInicializarTablas.Text = "Inicializar Tablas";
            this.btnInicializarTablas.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnInicializarTablas.UseVisualStyleBackColor = false;
            this.btnInicializarTablas.Click += new System.EventHandler(this.btnInicializarTablas_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button1.Location = new System.Drawing.Point(492, 307);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 135);
            this.button1.TabIndex = 20;
            this.button1.Text = "Salir";
            this.button1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // btnCargarRespaldo
            // 
            this.btnCargarRespaldo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCargarRespaldo.BackColor = System.Drawing.Color.Transparent;
            this.btnCargarRespaldo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCargarRespaldo.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCargarRespaldo.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCargarRespaldo.Location = new System.Drawing.Point(332, 307);
            this.btnCargarRespaldo.Name = "btnCargarRespaldo";
            this.btnCargarRespaldo.Size = new System.Drawing.Size(136, 135);
            this.btnCargarRespaldo.TabIndex = 21;
            this.btnCargarRespaldo.Text = "Cargar Respaldo";
            this.btnCargarRespaldo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCargarRespaldo.UseVisualStyleBackColor = false;
            this.btnCargarRespaldo.Click += new System.EventHandler(this.btnCargarRespaldo_Click);
            // 
            // Configuracion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Configuracion";
            this.Text = "Configuracion";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnConsultarActas;
        private System.Windows.Forms.Button btnInicializarTablas;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnCargarRespaldo;
    }
}