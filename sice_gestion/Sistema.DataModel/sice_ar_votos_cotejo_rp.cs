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
    
    public partial class sice_ar_votos_cotejo_rp
    {
        public int id { get; set; }
        public Nullable<int> id_partido { get; set; }
        public Nullable<int> id_casilla { get; set; }
        public Nullable<int> votos { get; set; }
        public string tipo { get; set; }
        public Nullable<int> estatus { get; set; }
        public Nullable<int> importado { get; set; }
    }
}
