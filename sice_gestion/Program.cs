using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sice_gestion
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //DateTime x = DateTime.ParseExact("17/05/2018 17:05", "dd/MM/yy h:mm", CultureInfo.InvariantCulture);
            //
            //string dateString = "";
            //DateTime date1 = DateTime.Parse(dateString,System.Globalization.CultureInfo.InvariantCulture);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }
    }
}
