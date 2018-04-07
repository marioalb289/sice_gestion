using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sistema.Generales;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using sice_gestion.Properties;

namespace sice_gestion
{
    public partial class Login : Form
    {
        private MsgBox msgBox;

        public Login()
        {
            InitializeComponent();
            txtUsuario.Text = "Correo Electronico";
            txtUsuario.ForeColor = Color.FromArgb(1, 162, 162, 162);
            txtContrasena.PasswordChar = '\0';
            txtContrasena.Text = "Contraseña";
            txtContrasena.ForeColor = Color.FromArgb(1, 162, 162, 162);
            pctFondo.Width = this.Width;
            pctFondo.Height = this.Height;
            label3.Parent = pctFondo;
            panel1.Parent = pctFondo;
            panel1.BackColor = Color.FromArgb(100, 255, 255, 255);
            txtUsuario.KeyPress += KeyPress;
            txtContrasena.KeyPress += KeyPress;
            this.Icon = Resources.logo;
            //btnAcceso.BackColor = Color.FromArgb(1, 154, 0, 0);


            //this.FormBorderStyle = FormBorderStyle.None;
            //Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                this.Validar();
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
           

        }

        private void Login_Resize(object sender, EventArgs e)
        {
            pctFondo.Width = this.Width;
            pctFondo.Height = this.Height;

        }        

        private void txtUsuario_Enter(object sender, EventArgs e)
        {
            if(txtUsuario.Text.Equals("Correo Electronico"))
            {
                txtUsuario.Text = "";
                txtUsuario.ForeColor = Color.FromArgb(1, 0, 0, 0);
            }
            
        }

        private void txtUsuario_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                txtUsuario.Text = "Correo Electronico";
                txtUsuario.ForeColor = Color.FromArgb(1, 162, 162, 162);

            }
                
        }

        private void txtContrasena_Enter(object sender, EventArgs e)
        {
            if (txtContrasena.Text.Equals("Contraseña"))
            {
                txtContrasena.Text = "";
                txtContrasena.ForeColor = Color.FromArgb(1, 0, 0, 0);
                txtContrasena.PasswordChar = '*';
            }            

        }

        private void txtContrasena_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                txtContrasena.PasswordChar = '\0';
                txtContrasena.ForeColor = Color.FromArgb(1, 162, 162, 162);
                txtContrasena.Text = "Contraseña";
                
            }
                
        }

        private void btnAcceso_Click(object sender, EventArgs e)
        {
            this.Validar();
        }

        private void Validar()
        {
            try
            {
                if(txtUsuario.Text == "" || txtUsuario.Text == "Correo Electronico")
                {
                    msgBox = new MsgBox(this, "Introduce Usuario", "Atención", MessageBoxButtons.OK, "Advertencia");
                    msgBox.ShowDialog(this);
                    return;
                }
                if (txtContrasena.Text == "" || txtContrasena.Text == "Contraseña")
                {
                    msgBox = new MsgBox(this, "Introduce Contraseña", "Atención", MessageBoxButtons.OK, "Advertencia");
                    msgBox.ShowDialog(this);
                    return;
                }

                CheckLogin chk = new CheckLogin();
                string usuario = "";
                string pass = "";
                using (MD5 md5Hash = MD5.Create())
                {
                    usuario = CheckLogin.GetMd5Hash(md5Hash, txtUsuario.Text);
                    pass = CheckLogin.GetMd5Hash(md5Hash, txtContrasena.Text);
                }

                int res = chk.checkLocal(usuario, pass);
                if (res == 4)
                    res = chk.checkServer(usuario,pass);
                if (res == 1)
                {
                    this.Hide();

                    MDIMain mod = new MDIMain();
                    mod.FormClosed += FormClosedEventHandler;
                    mod.Show();
                }
                else
                {
                    messageRes(res);
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void messageRes(int res)
        {
            switch (res)
            {
                case 0:
                    msgBox = new MsgBox(this, "Usuario o contraseña Incorrectos \n Intentalo de Nuevo", "Atención", MessageBoxButtons.OK, "Advertencia");
                    msgBox.ShowDialog(this);
                    break;
                case 2:
                    msgBox = new MsgBox(this, "Error al conectar con la Base de Datos", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                    //MessageBox.Show("No hay Conexion. \n Trabajndo en modo SIN CONEXIÓN");
                    //this.recargar();
                    break;
                case 3:
                    msgBox = new MsgBox(this, "Error al Accesar. \n Informar al Administrador.", "Atención", MessageBoxButtons.OK, "Error");
                    msgBox.ShowDialog(this);
                    //MessageBox.Show("Error al Guardar los Datos.");
                    break;
            }

        }

        private void FormClosedEventHandler(object sender, FormClosedEventArgs e)
        {
            this.Show();
        }
    }
}
