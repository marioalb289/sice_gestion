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
        public DbSet<sice_candidatos> usuarios { get; set; }
        public DbSet<sice_candidaturas> areas { get; set; }
        public DbSet<sice_casillas> cosas_requi { get; set; }
        public DbSet<sice_distritos_locales> lugares { get; set; }
        public DbSet<sice_municipios> requerimientos { get; set; }
        public DbSet<sice_partidos_politicos> sigi_contador_folios { get; set; }
        public DbSet<sice_registro_votos> sigi_documentos { get; set; }
        public DbSet<sice_usuarios> sigi_oficios { get; set; }
    }
}
