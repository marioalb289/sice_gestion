using MySql.Data.MySqlClient;
using Sistema.DataModel;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema.Generales
{
    public class TestConexion
    {
        public bool IsServerConnected()
        {
            Database.SetInitializer<DatabaseContext>(new CreateDatabaseIfNotExists<DatabaseContext>());
            //return false;
            DatabaseContext contexto = new DatabaseContext("MYSQLSERVER");
            //datacenterEntities contexto = new datacenterEntities();

            var connectionString = contexto.Database.Connection.ConnectionString;
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                connection.Close();
                //SistemaAdministracion.Conexion = true;
                MessageBox.Show("Conexion Establecida");
                return true;
            }
            catch (MySqlException sql)
            {
                //SistemaAdministracion.Conexion = false;
                MessageBox.Show("No hay Conexion. \n Trabajndo en modo SIN CONEXIÓN");
                return false;
            }

        }
    }
}
