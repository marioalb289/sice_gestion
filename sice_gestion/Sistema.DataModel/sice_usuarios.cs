//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sistema.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class sice_usuarios
    {
        public int id { get; set; }
        public string nombre_formal { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public int titular { get; set; }
        public string contrasena { get; set; }
        public string correo { get; set; }
        public int id_municipio { get; set; }
        public int privilegios { get; set; }
        public string rango { get; set; }
        public string puesto { get; set; }
        public int estado { get; set; }
        public int importado { get; set; }
        public Nullable<int> created_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<int> updated_by { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }
}
