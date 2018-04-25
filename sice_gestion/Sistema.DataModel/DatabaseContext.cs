using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Sistema.DataModel
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(string name)
            : base("name=" + name)
        {
        }
        public DbSet<sice_ar_asignacion> sice_ar_asignacion { get; set; }
        public DbSet<sice_ar_documentos> sice_ar_documentos { get; set; }
        public DbSet<sice_ar_documentos_local> sice_ar_documentos_local { get; set; }
        public DbSet<sice_ar_reserva> sice_ar_reserva { get; set; }
        public DbSet<sice_ar_votos_cotejo> sice_ar_votos_cotejo { get; set; }
        public DbSet<sice_ar_votos> sice_ar_votos { get; set; }
        public DbSet<sice_ar_votos_valida1> sice_ar_votos_valida1 { get; set; }
        public DbSet<sice_ar_votos_valida2> sice_ar_votos_valida2 { get; set; }
        public DbSet<sice_ar_votos_valida3> sice_ar_votos_valida3 { get; set; }
        public DbSet<sice_candidatos> sice_candidatos { get; set; }
        public DbSet<sice_candidaturas> sice_candidaturas { get; set; }
        public DbSet<sice_casillas> sice_casillas { get; set; }
        public DbSet<sice_distritos_locales> sice_distritos_locales { get; set; }
        public DbSet<sice_municipios> sice_municipios { get; set; }
        public DbSet<sice_partidos_politicos> sice_partidos_politicos { get; set; }
        public DbSet<sice_reserva_captura> sice_reserva_captura { get; set; }
        public DbSet<sice_usuarios> sice_usuarios { get; set; }        
        public DbSet<sice_votos> sice_votos { get; set; }
        public DbSet<sice_votos_test> sice_votos_test { get; set; }
        public DbSet<sice_ar_supuestos> sice_ar_supuestos { get; set; }
        public DbSet<sice_ar_historico> sice_ar_historico { get; set; }
        public DbSet<sice_historico> sice_historico { get; set; }
        public DbSet<sice_ar_estatus_acta> sice_ar_estatus_acta { get; set; }
        public DbSet<sice_ar_estatus_paquete> sice_ar_estatus_paquete { get; set; }
        public DbSet<sice_ar_incidencias> sice_ar_incidencias { get; set; }


    }
}
