﻿using System;
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

namespace sice_gestion
{
    public partial class Login : Form
    {

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
            //btnAcceso.BackColor = Color.FromArgb(1, 154, 0, 0);


            //this.FormBorderStyle = FormBorderStyle.None;
            //Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
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
            this.Hide();
            MDIMain mod = new MDIMain();
            mod.FormClosed += FormClosedEventHandler;
            mod.Show();
        }

        private void FormClosedEventHandler(object sender, FormClosedEventArgs e)
        {
            this.Show();
        }
    }
}
