namespace sice_gestion
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.pctLogo1 = new System.Windows.Forms.PictureBox();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAcceso = new System.Windows.Forms.Button();
            this.pctFondo = new System.Windows.Forms.PictureBox();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pctLogo1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctFondo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtContrasena
            // 
            this.txtContrasena.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtContrasena.Location = new System.Drawing.Point(47, 186);
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.PasswordChar = '*';
            this.txtContrasena.Size = new System.Drawing.Size(200, 26);
            this.txtContrasena.TabIndex = 11;
            this.txtContrasena.Enter += new System.EventHandler(this.txtContrasena_Enter);
            this.txtContrasena.Leave += new System.EventHandler(this.txtContrasena_Leave);
            // 
            // pctLogo1
            // 
            this.pctLogo1.Image = ((System.Drawing.Image)(resources.GetObject("pctLogo1.Image")));
            this.pctLogo1.Location = new System.Drawing.Point(50, 15);
            this.pctLogo1.Name = "pctLogo1";
            this.pctLogo1.Size = new System.Drawing.Size(200, 100);
            this.pctLogo1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctLogo1.TabIndex = 5;
            this.pctLogo1.TabStop = false;
            // 
            // txtUsuario
            // 
            this.txtUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsuario.Location = new System.Drawing.Point(47, 141);
            this.txtUsuario.MaxLength = 50;
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(200, 26);
            this.txtUsuario.TabIndex = 10;
            this.txtUsuario.Tag = "";
            this.txtUsuario.Enter += new System.EventHandler(this.txtUsuario_Enter);
            this.txtUsuario.Leave += new System.EventHandler(this.txtUsuario_Leave);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnAcceso);
            this.panel1.Controls.Add(this.txtContrasena);
            this.panel1.Controls.Add(this.pctLogo1);
            this.panel1.Controls.Add(this.txtUsuario);
            this.panel1.Location = new System.Drawing.Point(233, 150);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 300);
            this.panel1.TabIndex = 8;
            // 
            // btnAcceso
            // 
            this.btnAcceso.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnAcceso.FlatAppearance.BorderColor = System.Drawing.Color.DarkRed;
            this.btnAcceso.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAcceso.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAcceso.ForeColor = System.Drawing.Color.White;
            this.btnAcceso.Location = new System.Drawing.Point(47, 237);
            this.btnAcceso.Name = "btnAcceso";
            this.btnAcceso.Size = new System.Drawing.Size(200, 33);
            this.btnAcceso.TabIndex = 6;
            this.btnAcceso.Text = "Acceso";
            this.btnAcceso.UseVisualStyleBackColor = false;
            this.btnAcceso.Click += new System.EventHandler(this.btnAcceso_Click);
            // 
            // pctFondo
            // 
            this.pctFondo.Image = ((System.Drawing.Image)(resources.GetObject("pctFondo.Image")));
            this.pctFondo.Location = new System.Drawing.Point(1, 0);
            this.pctFondo.Name = "pctFondo";
            this.pctFondo.Size = new System.Drawing.Size(188, 169);
            this.pctFondo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctFondo.TabIndex = 7;
            this.pctFondo.TabStop = false;
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(212, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(380, 88);
            this.label3.TabIndex = 4;
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pctFondo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Login_Load);
            this.Resize += new System.EventHandler(this.Login_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pctLogo1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctFondo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox txtContrasena;
        private System.Windows.Forms.PictureBox pctLogo1;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pctFondo;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.Button btnAcceso;
        private System.Windows.Forms.Label label3;
    }
}