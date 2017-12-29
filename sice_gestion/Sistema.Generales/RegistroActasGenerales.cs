using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema.DataModel;

namespace Sistema.Generales
{
    public class RegistroActasGenerales
    {
        public List<SeccionCasilla> ListaSescciones()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLOCAL"))
                {
                    return (from p in contexto.sice_casillas select new SeccionCasilla {
                        id = p.id,
                        seccion = (int)p.seccion,
                        casilla = p.tipo_casilla,
                        distrito = (int)p.id_distrito_local,
                        municipio = (int)p.id_municipio
                    }).ToList();
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }
               
            }
            catch (Exception E)
            { throw E; }
        }

        public List<Candidatos> ListaCandidatos(int distrito)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLOCAL"))
                {
                    string consulta =
                        "SELECT " +
                        "C.id as id_candidato, " +
                        "CONCAT(C.nombre,' ',C.apellido_paterno,' ',C.apellido_materno)as candidato, " +
                        "CD.nombre_candidatura, " +
                        "P.siglas_par as partido, " +
                        "P.img_par as imagen " +
                        "FROM sice_candidatos C " +
                        "JOIN sice_candidaturas CD ON CD.id = C.fk_cargo AND CD.titular = 1 " + //"AND CD.id_distrito =" + distrito +
                        "JOIN sice_partidos_politicos P ON P.id = C.fk_partido";
                    return contexto.Database.SqlQuery<Candidatos>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }
    }

    public class SeccionCasilla
    {
        public int id { get; set; }
        public int seccion { get; set; }
        public string casilla { get; set; }
        public int distrito { get; set; }
        public int municipio { get; set; }
    }

    public class Candidatos
    {
        public int id_candidato { get; set; }
        public string candidato { get; set; }
        public string nombre_candidatura { get; set; }
        public string partido { get; set; }
        public string imagen { get; set; }

    }
}
