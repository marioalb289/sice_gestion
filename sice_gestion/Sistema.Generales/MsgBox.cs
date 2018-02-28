using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema.Generales
{
    public partial class MsgBox : Form
    {
        Form mdi;
        System.Drawing.Point Punto1 = new System.Drawing.Point(229, 138);
        System.Drawing.Point Punto2 = new System.Drawing.Point(312, 138);
        System.Drawing.Point Punto3 = new System.Drawing.Point(395, 138);

        public MsgBox(Form mdiParent, string strText)
        {
            InitializeComponent();
            //  this.MdiParent = mdiParent;
            mdi = mdiParent;
            AdminMensaje(strText, "");
            AdminBotones(MessageBoxButtons.OK);
            AdminIcon("None");
            this.Parent = this.MdiParent;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        public MsgBox(Form mdiParent, string strText, string strCaption)
        {
            InitializeComponent();
            //  this.MdiParent = mdiParent;
            mdi = mdiParent;
            AdminMensaje(strText, strCaption);
            AdminBotones(MessageBoxButtons.OK);
            AdminIcon("None");
            this.Parent = this.MdiParent;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        public MsgBox(Form mdiParent, string strText, string strCaption, MessageBoxButtons enmButtons)
        {
            InitializeComponent();
            //  this.MdiParent = mdiParent;
            mdi = mdiParent;
            AdminMensaje(strText, strCaption);
            AdminBotones(enmButtons);
            AdminIcon("None");
            this.Parent = this.MdiParent;
            this.StartPosition = FormStartPosition.CenterParent;
        }
        public MsgBox(Form mdiParent, string strText, MessageBoxButtons enmButtons, string enmIcon)
        {
            InitializeComponent();
            //  this.MdiParent = mdiParent;
            mdi = mdiParent;
            AdminMensaje(strText, "");
            AdminBotones(enmButtons);
            AdminIcon(enmIcon);
            this.BringToFront();
            this.Parent = this.MdiParent;
            this.StartPosition = FormStartPosition.CenterParent;
        }
        public MsgBox(Form mdiParent, string strText, string strCaption, MessageBoxButtons enmButtons, string enmIcon)
        {
            InitializeComponent();
            //  this.MdiParent = mdiParent;
            mdi = mdiParent;
            AdminMensaje(strText, strCaption);
            AdminBotones(enmButtons);
            AdminIcon(enmIcon);
            this.BringToFront();
            this.Parent = this.MdiParent;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        //public void Paremetros(string strText, string strCaption)
        //{
        //    AdminMensaje(strText, strCaption);
        //    AdminBotones(MessageBoxButtons.OK);
        //    AdminIcon(MessageBoxIcon.None);
        //}

        //public void Paremetros(string strText, string strCaption, MessageBoxButtons enmButtons)
        //{
        //    AdminMensaje(strText, strCaption);
        //    AdminBotones(enmButtons);
        //    AdminIcon(MessageBoxIcon.None);
        //}
        //public void Paremetros(string strText, string strCaption, MessageBoxButtons enmButtons, MessageBoxIcon enmIcon)
        //{
        //    AdminMensaje(strText, strCaption);
        //    AdminBotones(enmButtons);
        //    AdminIcon(enmIcon);
        //}
        private void AdminMensaje(string strText, string strCaption)
        {
            this.lblMensaje.Text = strText;
            this.Text = strCaption;

        }
        private void AdminIcon(string enmIcon)
        {
            try
            {
                if ("Ok" == enmIcon) picIcono.Image = Sistema.Generales.Resources.ok2;// new ImageResourceHandle("asterisk_59.png");
                if ("Error" == enmIcon) picIcono.Image = Sistema.Generales.Resources.Error_60;
                if ("Advertencia" == enmIcon) picIcono.Image = Sistema.Generales.Resources.Warning_64;
                if ("Question" == enmIcon) picIcono.Image = Sistema.Generales.Resources.question64;
                if ("Informacion" == enmIcon) picIcono.Image = Sistema.Generales.Resources.Information_64;
                if ("None" == enmIcon) picIcono.Image = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void AdminBotones(MessageBoxButtons enmButtons)
        {
            bttIgnore.Visible = false;
            bttRetry.Visible = false;
            bttAbort.Visible = false;
            bttNo.Visible = false;
            bttSi.Visible = false;
            bttAceptar.Visible = false;
            bttCancelar.Visible = false;
            bttOk.Visible = false;

            if (MessageBoxButtons.AbortRetryIgnore == enmButtons)
            {
                bttAbort.Visible = true; this.bttAbort.Location = Punto1;
                bttRetry.Visible = true; this.bttRetry.Location = Punto2;
                bttIgnore.Visible = true; this.bttIgnore.Location = Punto3;
            }
            if (MessageBoxButtons.OK == enmButtons)
            {
                bttOk.Visible = true; this.bttOk.Location = Punto3;
            }
            if (MessageBoxButtons.OKCancel == enmButtons)
            {
                bttCancelar.Visible = true; this.bttCancelar.Location = Punto2;
                bttOk.Visible = true; this.bttOk.Location = Punto3;
            }
            if (MessageBoxButtons.RetryCancel == enmButtons)
            {
                bttRetry.Visible = true; this.bttRetry.Location = Punto2;
                bttCancelar.Visible = true; this.bttCancelar.Location = Punto3;
            }
            if (MessageBoxButtons.YesNo == enmButtons)
            {
                bttSi.Visible = true; this.bttSi.Location = Punto2;
                bttNo.Visible = true; this.bttNo.Location = Punto3;
            }
            if (MessageBoxButtons.YesNoCancel == enmButtons)
            {
                bttSi.Visible = true; this.bttSi.Location = Punto1;
                bttNo.Visible = true; this.bttNo.Location = Punto2;
                bttCancelar.Visible = true; this.bttCancelar.Location = Punto3;
            }
        }

        private void bttCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }


        private void bttAceptar_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

        private void bttAbort_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.Close();
        }

        private void bttRetry_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.Close();
        }

        private void bttIgnore_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.Close();
        }

        private void bttSi_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

        private void bttNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void bttOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        private void MsgBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (mdi != null) mdi.Focus();
                this.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void MsgBox_Load(object sender, EventArgs e)
        {
            try
            {
                this.Focus();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
