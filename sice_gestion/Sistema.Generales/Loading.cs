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
    public partial class Loading : Form
    {
        Form mdi;
        public Loading(Form mdiParent, string mensaje ="Guardando")
        {
            InitializeComponent();
            this.lblMensaje.Text = mensaje;
            //this.Parent = this.MdiParent;
            //this.TransparencyKey = Color.FromArgb(255, 171, 171, 171);
            mdi = mdiParent;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(mdiParent.Location.X + (mdiParent.Width - this.Width) / 2, mdiParent.Location.Y + (mdiParent.Height - this.Height) / 2);
            
        }
        
        private void Loading_Load(object sender, EventArgs e)
        {
            //this.BackColor = Color.FromArgb(255, 171, 171, 171);
            //this.TransparencyKey = Color.FromArgb(255, 171, 171, 171);
        }
    }
}
