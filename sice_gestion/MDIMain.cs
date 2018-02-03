using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sice_gestion
{
    public partial class MDIMain : Form
    {
        private int childFormNumber = 0;
        private int flagWatcher = 0;

        public MDIMain()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Ventana " + childFormNumber++;
            childForm.Show();
        }        

        private void MDIMain_Load(object sender, EventArgs e)
        {
            FrmModulos mod = new FrmModulos();
            mod.MdiParent = this;
            mod.Dock = DockStyle.Fill;
            //mod.ControlBox = false;
            mod.Show();
            //this.RunWatchFile();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void RunWatchFile()
        {            

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "";
            string path = @desktop + "\\sice_archivos";

            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*.jpg";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;            

        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);

            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                this.DisplayNotify();
            }

            if (e.ChangeType == WatcherChangeTypes.Changed )
            {
                //Hubo cambios
                this.DisplayNotify();

            }
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        public void DisplayNotify()
        {
            try
            {
                notifyActas.Text = "Notificación de Actas Nuevas";
                notifyActas.Visible = true;
                notifyActas.BalloonTipIcon = ToolTipIcon.Info;
                notifyActas.BalloonTipTitle = "Se han agregado nuevas actas escaneadas";
                notifyActas.BalloonTipText = "Haga click aqui para capturas datos";
                notifyActas.ShowBalloonTip(100);
                //notifyActas.ShowBalloonTip(3000,"Se han agregado NUEVAS ACTAS","Haga click aqui para capturar datos", ToolTipIcon.Info);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void notifyActas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                Sistema.RegistroActas.frmRegistroActas form = new Sistema.RegistroActas.frmRegistroActas();
                form.MdiParent = this;
                form.Dock = DockStyle.Fill;
                form.Show();
                notifyActas.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void notifyActas_Click_1(object sender, EventArgs e)
        {
            try
            {
                Sistema.RegistroActas.frmRegistroActas form = new Sistema.RegistroActas.frmRegistroActas();
                form.MdiParent = this;
                form.Dock = DockStyle.Fill;
                form.Show();
                notifyActas.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
