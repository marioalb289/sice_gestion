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
    
    public partial class sice_ar_reserva
    {
        public int id { get; set; }
        public Nullable<int> id_casilla { get; set; }
        public string tipo_reserva { get; set; }
        public Nullable<int> id_documento { get; set; }
        public Nullable<int> importado { get; set; }
        public Nullable<int> id_supuesto { get; set; }
        public Nullable<System.DateTime> create_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<int> num_escritos { get; set; }
        public Nullable<int> boletas_sobrantes { get; set; }
        public Nullable<int> personas_votaron { get; set; }
        public Nullable<int> num_representantes_votaron { get; set; }
        public Nullable<int> votos_sacados { get; set; }
        public Nullable<int> casilla_instalada { get; set; }
        public Nullable<int> id_estatus_acta { get; set; }
        public Nullable<int> id_estatus_paquete { get; set; }
        public Nullable<int> id_incidencias { get; set; }
    }
}
