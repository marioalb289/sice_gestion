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
    
    public partial class sice_ar_documentos
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string ruta { get; set; }
        public string estatus { get; set; }
        public Nullable<int> filtro { get; set; }
        public Nullable<int> estatus_filtro1 { get; set; }
        public Nullable<int> estatus_filtro2 { get; set; }
        public Nullable<int> estatus_filtro3 { get; set; }
        public Nullable<int> estatus_revisor { get; set; }
        public Nullable<int> estatus_cotejador { get; set; }
        public Nullable<int> id_casilla { get; set; }
        public System.DateTime create_at { get; set; }
        public System.DateTime updated_at { get; set; }
        public Nullable<int> importado { get; set; }
    }
}
