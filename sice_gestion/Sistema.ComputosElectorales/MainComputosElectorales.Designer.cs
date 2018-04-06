namespace Sistema.ComputosElectorales
{
    partial class MainComputosElectorales
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
            this.btnRecuentoVotos = new System.Windows.Forms.Button();
            this.btnReportes = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnConsultarActas, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnRecuentoVotos, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnReportes, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 600);
            this.tableLayoutPanel1.TabIndex = 8;
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
            this.label2.Text = "Seleccionar Módulo";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnConsultarActas
            // 
            this.btnConsultarActas.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnConsultarActas.BackColor = System.Drawing.Color.Transparent;
            this.btnConsultarActas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnConsultarActas.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConsultarActas.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnConsultarActas.Location = new System.Drawing.Point(332, 307);
            this.btnConsultarActas.Name = "btnConsultarActas";
            this.btnConsultarActas.Size = new System.Drawing.Size(136, 135);
            this.btnConsultarActas.TabIndex = 14;
            this.btnConsultarActas.Text = "Casillas Reservadas";
            this.btnConsultarActas.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnConsultarActas.UseVisualStyleBackColor = false;
            this.btnConsultarActas.Click += new System.EventHandler(this.btnConsultarActas_Click);
            // 
            // btnRecuentoVotos
            // 
            this.btnRecuentoVotos.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRecuentoVotos.BackColor = System.Drawing.Color.Transparent;
            this.btnRecuentoVotos.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnRecuentoVotos.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecuentoVotos.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRecuentoVotos.Location = new System.Drawing.Point(172, 307);
            this.btnRecuentoVotos.Name = "btnRecuentoVotos";
            this.btnRecuentoVotos.Size = new System.Drawing.Size(136, 135);
            this.btnRecuentoVotos.TabIndex = 13;
            this.btnRecuentoVotos.Text = "Recuento de Votos";
            this.btnRecuentoVotos.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRecuentoVotos.UseVisualStyleBackColor = false;
            this.btnRecuentoVotos.Click += new System.EventHandler(this.btnRecuentoVotos_Click);
            // 
            // btnReportes
            // 
            this.btnReportes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReportes.BackColor = System.Drawing.Color.Transparent;
            this.btnReportes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnReportes.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReportes.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReportes.Location = new System.Drawing.Point(492, 307);
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Size = new System.Drawing.Size(136, 135);
            this.btnReportes.TabIndex = 17;
            this.btnReportes.Text = "Reportes";
            this.btnReportes.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnReportes.UseVisualStyleBackColor = false;
            this.btnReportes.Click += new System.EventHandler(this.btnReportes_Click);
            // 
            // MainComputosElectorales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainComputosElectorales";
            this.Text = "MainComputosElectorales";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConsultarActas;
        private System.Windows.Forms.Button btnRecuentoVotos;
        private System.Windows.Forms.Button btnReportes;
    }
}