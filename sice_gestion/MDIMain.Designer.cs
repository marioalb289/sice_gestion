namespace sice_gestion
{
    partial class MDIMain
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
            this.lblDescarga = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDescarga
            // 
            this.lblDescarga.AutoSize = true;
            this.lblDescarga.Location = new System.Drawing.Point(436, 9);
            this.lblDescarga.Name = "lblDescarga";
            this.lblDescarga.Size = new System.Drawing.Size(40, 13);
            this.lblDescarga.TabIndex = 0;
            this.lblDescarga.Text = "Espera";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblDescarga);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1008, 32);
            this.panel1.TabIndex = 4;
            // 
            // MDIMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.panel1);
            this.IsMdiContainer = true;
            this.Name = "MDIMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MDIMain";
            this.Load += new System.EventHandler(this.MDIMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
        public System.Windows.Forms.Label lblDescarga;
        private System.Windows.Forms.Panel panel1;
    }
}



