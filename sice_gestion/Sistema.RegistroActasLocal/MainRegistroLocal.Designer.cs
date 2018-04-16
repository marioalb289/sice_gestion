﻿namespace Sistema.RegistroActasLocal
{
    partial class MainRegistroLocal
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
            this.btnIdentificar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConsultarActas = new System.Windows.Forms.Button();
            this.btnRegistrarActas = new System.Windows.Forms.Button();
            this.btnReportes = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel1.Controls.Add(this.btnIdentificar, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnConsultarActas, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnRegistrarActas, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnReportes, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.button1, 4, 1);
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
            // btnIdentificar
            // 
            this.btnIdentificar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnIdentificar.BackColor = System.Drawing.Color.Transparent;
            this.btnIdentificar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnIdentificar.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIdentificar.Image = global::Sistema.RegistroActasLocal.Properties.Resources.identificar;
            this.btnIdentificar.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnIdentificar.Location = new System.Drawing.Point(12, 293);
            this.btnIdentificar.Name = "btnIdentificar";
            this.btnIdentificar.Size = new System.Drawing.Size(136, 164);
            this.btnIdentificar.TabIndex = 21;
            this.btnIdentificar.Text = "Identificar Actas";
            this.btnIdentificar.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnIdentificar.UseVisualStyleBackColor = false;
            this.btnIdentificar.Click += new System.EventHandler(this.btnIdentificar_Click);
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
            this.btnConsultarActas.Image = global::Sistema.RegistroActasLocal.Properties.Resources.consultar;
            this.btnConsultarActas.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnConsultarActas.Location = new System.Drawing.Point(332, 293);
            this.btnConsultarActas.Name = "btnConsultarActas";
            this.btnConsultarActas.Size = new System.Drawing.Size(136, 163);
            this.btnConsultarActas.TabIndex = 14;
            this.btnConsultarActas.Text = "Consultar Actas";
            this.btnConsultarActas.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnConsultarActas.UseVisualStyleBackColor = false;
            this.btnConsultarActas.Click += new System.EventHandler(this.btnConsultarActas_Click);
            // 
            // btnRegistrarActas
            // 
            this.btnRegistrarActas.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRegistrarActas.BackColor = System.Drawing.Color.Transparent;
            this.btnRegistrarActas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnRegistrarActas.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegistrarActas.Image = global::Sistema.RegistroActasLocal.Properties.Resources.editar;
            this.btnRegistrarActas.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRegistrarActas.Location = new System.Drawing.Point(172, 293);
            this.btnRegistrarActas.Name = "btnRegistrarActas";
            this.btnRegistrarActas.Size = new System.Drawing.Size(136, 163);
            this.btnRegistrarActas.TabIndex = 13;
            this.btnRegistrarActas.Text = "Registrar Actas";
            this.btnRegistrarActas.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRegistrarActas.UseVisualStyleBackColor = false;
            this.btnRegistrarActas.Click += new System.EventHandler(this.btnRegistrarActas_Click);
            // 
            // btnReportes
            // 
            this.btnReportes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReportes.BackColor = System.Drawing.Color.Transparent;
            this.btnReportes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnReportes.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReportes.Image = global::Sistema.RegistroActasLocal.Properties.Resources.reporte;
            this.btnReportes.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReportes.Location = new System.Drawing.Point(492, 293);
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Size = new System.Drawing.Size(136, 163);
            this.btnReportes.TabIndex = 17;
            this.btnReportes.Text = "Reportes";
            this.btnReportes.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnReportes.UseVisualStyleBackColor = false;
            this.btnReportes.Click += new System.EventHandler(this.btnReportes_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = global::Sistema.RegistroActasLocal.Properties.Resources.salir;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button1.Location = new System.Drawing.Point(652, 293);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 163);
            this.button1.TabIndex = 20;
            this.button1.Text = "Salir";
            this.button1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainRegistroLocal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainRegistroLocal";
            this.Text = "MainRegistroLocal";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnConsultarActas;
        private System.Windows.Forms.Button btnRegistrarActas;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnReportes;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnIdentificar;
    }
}