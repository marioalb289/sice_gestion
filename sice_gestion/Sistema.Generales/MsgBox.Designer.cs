namespace Sistema.Generales
{
    partial class MsgBox
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
            this.picIcono = new System.Windows.Forms.PictureBox();
            this.bttCancelar = new System.Windows.Forms.Button();
            this.bttAceptar = new System.Windows.Forms.Button();
            this.bttSi = new System.Windows.Forms.Button();
            this.bttNo = new System.Windows.Forms.Button();
            this.bttAbort = new System.Windows.Forms.Button();
            this.bttRetry = new System.Windows.Forms.Button();
            this.bttIgnore = new System.Windows.Forms.Button();
            this.bttOk = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.lblMensaje = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picIcono)).BeginInit();
            this.SuspendLayout();
            // 
            // picIcono
            // 
            this.picIcono.Location = new System.Drawing.Point(9, 34);
            this.picIcono.Name = "picIcono";
            this.picIcono.Size = new System.Drawing.Size(76, 76);
            this.picIcono.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picIcono.TabIndex = 0;
            this.picIcono.TabStop = false;
            // 
            // bttCancelar
            // 
            this.bttCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bttCancelar.Location = new System.Drawing.Point(85, 138);
            this.bttCancelar.Name = "bttCancelar";
            this.bttCancelar.Size = new System.Drawing.Size(75, 28);
            this.bttCancelar.TabIndex = 2;
            this.bttCancelar.Text = "Cancelar";
            this.bttCancelar.Click += new System.EventHandler(this.bttCancelar_Click);
            // 
            // bttAceptar
            // 
            this.bttAceptar.Location = new System.Drawing.Point(85, 168);
            this.bttAceptar.Name = "bttAceptar";
            this.bttAceptar.Size = new System.Drawing.Size(75, 28);
            this.bttAceptar.TabIndex = 3;
            this.bttAceptar.Text = "Aceptar";
            this.bttAceptar.Click += new System.EventHandler(this.bttAceptar_Click);
            // 
            // bttSi
            // 
            this.bttSi.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bttSi.Location = new System.Drawing.Point(160, 138);
            this.bttSi.Name = "bttSi";
            this.bttSi.Size = new System.Drawing.Size(75, 28);
            this.bttSi.TabIndex = 2;
            this.bttSi.Text = "Si";
            this.bttSi.Click += new System.EventHandler(this.bttSi_Click);
            // 
            // bttNo
            // 
            this.bttNo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bttNo.Location = new System.Drawing.Point(160, 168);
            this.bttNo.Name = "bttNo";
            this.bttNo.Size = new System.Drawing.Size(75, 28);
            this.bttNo.TabIndex = 2;
            this.bttNo.Text = "No";
            this.bttNo.Click += new System.EventHandler(this.bttNo_Click);
            // 
            // bttAbort
            // 
            this.bttAbort.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bttAbort.Location = new System.Drawing.Point(244, 138);
            this.bttAbort.Name = "bttAbort";
            this.bttAbort.Size = new System.Drawing.Size(75, 28);
            this.bttAbort.TabIndex = 2;
            this.bttAbort.Text = "Abortar";
            this.bttAbort.Click += new System.EventHandler(this.bttAbort_Click);
            // 
            // bttRetry
            // 
            this.bttRetry.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bttRetry.Location = new System.Drawing.Point(327, 138);
            this.bttRetry.Name = "bttRetry";
            this.bttRetry.Size = new System.Drawing.Size(75, 28);
            this.bttRetry.TabIndex = 2;
            this.bttRetry.Text = "Reintentar";
            this.bttRetry.Click += new System.EventHandler(this.bttRetry_Click);
            // 
            // bttIgnore
            // 
            this.bttIgnore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bttIgnore.Location = new System.Drawing.Point(410, 138);
            this.bttIgnore.Name = "bttIgnore";
            this.bttIgnore.Size = new System.Drawing.Size(75, 28);
            this.bttIgnore.TabIndex = 2;
            this.bttIgnore.Text = "Ignorar";
            this.bttIgnore.Click += new System.EventHandler(this.bttIgnore_Click);
            // 
            // bttOk
            // 
            this.bttOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bttOk.Location = new System.Drawing.Point(244, 168);
            this.bttOk.Name = "bttOk";
            this.bttOk.Size = new System.Drawing.Size(75, 28);
            this.bttOk.TabIndex = 2;
            this.bttOk.Text = "Ok";
            this.bttOk.Click += new System.EventHandler(this.bttOk_Click);
            // 
            // lblMensaje
            // 
            this.lblMensaje.BackColor = System.Drawing.SystemColors.Control;
            this.lblMensaje.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensaje.Location = new System.Drawing.Point(91, 8);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(394, 127);
            this.lblMensaje.TabIndex = 4;
            this.lblMensaje.Text = "Registro de Actas";
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MsgBox
            // 
            this.AcceptButton = this.bttAceptar;
            this.CancelButton = this.bttCancelar;
            this.ClientSize = new System.Drawing.Size(490, 201);
            this.ControlBox = false;
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.bttOk);
            this.Controls.Add(this.bttIgnore);
            this.Controls.Add(this.bttRetry);
            this.Controls.Add(this.bttAbort);
            this.Controls.Add(this.bttNo);
            this.Controls.Add(this.bttSi);
            this.Controls.Add(this.bttAceptar);
            this.Controls.Add(this.bttCancelar);
            this.Controls.Add(this.picIcono);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MsgBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MMessageBox";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MsgBox_FormClosed);
            this.Load += new System.EventHandler(this.MsgBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picIcono)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picIcono;
        private System.Windows.Forms.Button bttCancelar;
        private System.Windows.Forms.Button bttAceptar;
        private System.Windows.Forms.Button bttSi;
        private System.Windows.Forms.Button bttNo;
        private System.Windows.Forms.Button bttAbort;
        private System.Windows.Forms.Button bttRetry;
        private System.Windows.Forms.Button bttIgnore;
        private System.Windows.Forms.Button bttOk;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Label lblMensaje;
    }
}