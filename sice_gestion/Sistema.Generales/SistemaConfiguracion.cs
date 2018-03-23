using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema.DataModel;

namespace Sistema.Generales
{
    public class SistemaConfiguracion
    {
        public int Inicializar()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    int result = contexto.Database.ExecuteSqlCommand("TRUNCATE sice_votos_test");
                    if (result != 0)
                        throw new Exception("No se pudo Inicializar bd");
                    List<sice_distritos_locales> listaDistritos = this.ListaDistritos();
                    if(listaDistritos.Count > 0)
                    {
                        foreach(sice_distritos_locales d in listaDistritos)
                        {
                            List<Candidatos> listaCandidatosDistrito = this.ListaCandidatos(d.id);
                            if(listaCandidatosDistrito.Count == 0)
                                throw new Exception("No se pudo Inicializar bd");
                            List<sice_casillas> listaCasillasDistrito = this.ListaCasillasDistrito(d.id);
                            if (listaCasillasDistrito.Count == 0)
                                throw new Exception("No se pudo Inicializar bd");
                            foreach(sice_casillas casilla in listaCasillasDistrito)
                            {
                                Console.WriteLine("Insertando casilla: " + casilla.id);
                                sice_votos_test v1 = new sice_votos_test();
                                for (int x = 0; x < listaCandidatosDistrito.Count +2; x++)
                                {
                                    if (x >= listaCandidatosDistrito.Count)
                                        v1.id_candidato = null;
                                    else
                                        v1.id_candidato = listaCandidatosDistrito[x].id_candidato;
                                    v1.id_casilla = casilla.id;
                                    v1.tipo = (x > listaCandidatosDistrito.Count - 1) ? x == listaCandidatosDistrito.Count ? "NO REGISTRADO" : "NULO" : "VOTO";
                                    v1.votos = 0;
                                    contexto.sice_votos_test.Add(v1);
                                    contexto.SaveChanges();
                                }
                                
                            }
                        }
                    }
                    return 1;
                }

                    
            }
            catch(Exception E)
            {
                throw E;
            }
        }

        public List<sice_casillas> ListaCasillasDistrito(int distrito)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    return (from p in contexto.sice_casillas where p.id_distrito_local == distrito select p).ToList();
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public List<sice_distritos_locales> ListaDistritos()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    return (from d in contexto.sice_distritos_locales select d).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public List<Candidatos> ListaCandidatos(int distrito)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    string consulta =
                        "SELECT " +
                        "C.id as id_candidato, " +
                        "CONCAT(C.nombre,' ',C.apellido_paterno,' ',C.apellido_materno)as candidato, " +
                        "CD.nombre_candidatura, " +
                        "P.siglas_par as partido, " +
                        "P.img_par as imagen " +
                        "FROM sice_candidatos C " +
                        "JOIN sice_candidaturas CD ON CD.id = C.fk_cargo AND CD.titular = 1 " + // "AND CD.id_distrito =" + distrito + " "+
                        "JOIN sice_partidos_politicos P ON P.id = C.fk_partido";
                    return contexto.Database.SqlQuery<Candidatos>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }
    }
}
