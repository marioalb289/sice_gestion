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
        public DbSet<sice_ar_reserva> sice_ar_reserva { get; set; }
        public DbSet<sice_ar_votos> sice_ar_votos { get; set; }
        public DbSet<sice_ar_votos_valida1> sice_ar_votos_valida1 { get; set; }
        public DbSet<sice_ar_votos_valida2> sice_ar_votos_valida2 { get; set; }
        public DbSet<sice_ar_votos_valida3> sice_ar_votos_valida3 { get; set; }
        public DbSet<sice_candidatos> usice_candidatossuarios { get; set; }
        public DbSet<sice_candidaturas> sice_candidaturas { get; set; }
        public DbSet<sice_casillas> sice_casillas { get; set; }
        public DbSet<sice_distritos_locales> sice_distritos_locales { get; set; }
        public DbSet<sice_municipios> sice_municipios { get; set; }
        public DbSet<sice_partidos_politicos> sice_partidos_politicos { get; set; }
        public DbSet<sice_reserva> sice_reserva { get; set; }
        public DbSet<sice_usuarios> sice_usuarios { get; set; }
    }
}
