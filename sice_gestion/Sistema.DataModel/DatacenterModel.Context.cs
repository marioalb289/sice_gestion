﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class datacenterEntities : DbContext
    {
        public datacenterEntities()
            : base("name=datacenterEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<sice_ar_asignacion> sice_ar_asignacion { get; set; }
        public DbSet<sice_ar_reserva> sice_ar_reserva { get; set; }
        public DbSet<sice_ar_votos> sice_ar_votos { get; set; }
        public DbSet<sice_candidatos> sice_candidatos { get; set; }
        public DbSet<sice_candidaturas> sice_candidaturas { get; set; }
        public DbSet<sice_distritos_locales> sice_distritos_locales { get; set; }
        public DbSet<sice_municipios> sice_municipios { get; set; }
        public DbSet<sice_partidos_politicos> sice_partidos_politicos { get; set; }
        public DbSet<sice_usuarios> sice_usuarios { get; set; }
        public DbSet<sice_ar_votos_valida1> sice_ar_votos_valida1 { get; set; }
        public DbSet<sice_ar_votos_valida2> sice_ar_votos_valida2 { get; set; }
        public DbSet<sice_ar_votos_valida3> sice_ar_votos_valida3 { get; set; }
        public DbSet<sice_ar_documentos_local> sice_ar_documentos_local { get; set; }
        public DbSet<sice_ar_documentos> sice_ar_documentos { get; set; }
        public DbSet<sice_ar_votos_cotejo> sice_ar_votos_cotejo { get; set; }
        public DbSet<sice_reserva_captura> sice_reserva_captura { get; set; }
        public DbSet<sice_votos> sice_votos { get; set; }
        public DbSet<sice_casillas> sice_casillas { get; set; }
    }
}
