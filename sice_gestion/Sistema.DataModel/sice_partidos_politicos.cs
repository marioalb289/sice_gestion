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
    
    public partial class sice_partidos_politicos
    {
        public sice_partidos_politicos()
        {
            this.sice_candidatos = new HashSet<sice_candidatos>();
        }
    
        public int id { get; set; }
        public string nombre_par { get; set; }
        public string siglas_par { get; set; }
        public string nombre_comp_presi { get; set; }
        public string correo_insti { get; set; }
        public string telefono_insti { get; set; }
        public string img_par { get; set; }
        public int estado { get; set; }
        public string info_creado { get; set; }
        public Nullable<int> prelacion { get; set; }
    
        public virtual ICollection<sice_candidatos> sice_candidatos { get; set; }
    }
}
