using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Generales
{
    public static class LoginInfo
    {
        public static int id_usuario;
        public static string nombre;
        public static string apellido;
        public static string nombre_formal;
        public static int privilegios;
        public static int estado;
        public static int id_municipio;
        public static int id_cabecera_local;
    }

    public static class Configuracion
    {
        public const string NetWorkPath = @"\\192.168.1.67\sice_archivos";
        public const string NetworkFtp = @"ftp://192.168.1.67/";
        public const string User = "iepc_ftp_centos";
        public const string Pass = "$i$t3m4s2018utc";
        public const string Repo = "sice_archivos";
    }
}
