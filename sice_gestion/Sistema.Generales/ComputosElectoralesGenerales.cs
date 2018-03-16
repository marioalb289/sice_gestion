using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema.DataModel;
using System.Transactions;

namespace Sistema.Generales
{
    public class ComputosElectoralesGenerales
    {
        public List<sice_distritos_locales> ListaDistritos()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLOCAL"))
                {
                    return (from d in contexto.sice_distritos_locales select d).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }
        public List<VotosSeccion> ResultadosSeccion(int id_distrito_local = 0)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLOCAL"))
                {
                    string condicion = "";
                    if (id_distrito_local != 0)
                        condicion = " AND C.id_distrito_local = " + id_distrito_local + " ";
                    string consulta =
                        "SELECT " +
                            "C.seccion," +
                            "RV.id_casilla," +
                            "C.tipo_casilla as casilla," +
                            "C.lista_nominal," +
                            "RV.id_candidato," +
                            "RV.votos," +
                            "RV.tipo," +
                            "CONCAT(CND.nombre, ' ', CND.apellido_paterno, ' ', CND.apellido_materno) as candidato," +
                            "P.siglas_par as partido," +
                            "P.img_par as imagen," +
                            "C.id_distrito_local as distrito_local," +
                            "M.municipio," +
                            "M2.municipio AS cabecera_local " +
                        "FROM sice_votos RV " +
                        "LEFT JOIN sice_candidatos CND ON CND.id = RV.id_candidato " +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = CND.fk_partido " +
                        "JOIN sice_casillas C ON C.id = RV.id_casilla " + condicion +
                        "JOIN sice_municipios M ON M.id = C.id_municipio " +
                        "JOIN sice_municipios M2 ON M2.id = C.id_cabecera_local " +
                        "ORDER BY C.seccion ASC, RV.id_casilla ASC, RV.id_candidato DESC ";

                    return contexto.Database.SqlQuery<VotosSeccion>(consulta).ToList();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SeccionCasillaConsecutivo> ListaSescciones()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLOCAL"))
                {
                    string consulta =
                        "SELECT C.* FROM sice_casillas C " +
                        "LEFT JOIN sice_reserva_captura RC ON RC.id_casilla = C.id " +
                        "WHERE RC.id IS NULL"+ " AND C.id_cabecera_local = " +LoginInfo.id_municipio;
                    List<sice_casillas> lsCasilla = contexto.Database.SqlQuery<sice_casillas>(consulta).ToList();
                    return (from p in lsCasilla
                            select new SeccionCasillaConsecutivo
                            {
                                id = p.id,
                                seccion = (int)p.seccion,
                                casilla = p.tipo_casilla,
                                consecutivo = (int)p.consecutivo_total,
                                listaNominal = (int)p.lista_nominal,
                                distrito = (int)p.id_distrito_local,
                                municipio = (int)p.id_municipio
                            }).ToList();
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public List<SeccionCasillaConsecutivo> ListaSesccionesReserva()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLOCAL"))
                {
                    string consulta =
                        "SELECT C.* FROM sice_casillas C " +
                        "JOIN sice_reserva_captura RC ON RC.id_casilla = C.id " +
                        "WHERE RC.tipo_reserva = 'RESERVA' AND C.id_cabecera_local = " + LoginInfo.id_municipio;
                    List<sice_casillas> lsCasilla = contexto.Database.SqlQuery<sice_casillas>(consulta).ToList();
                    return (from p in lsCasilla
                            select new SeccionCasillaConsecutivo
                            {
                                id = p.id,
                                seccion = (int)p.seccion,
                                casilla = p.tipo_casilla,
                                consecutivo = (int)p.consecutivo_total,
                                listaNominal = (int)p.lista_nominal,
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

        public int CasillaReserva(int id_casilla,string motivo)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLOCAL"))
                {
                    

                    sice_reserva_captura rc = (from p in contexto.sice_reserva_captura where p.id_casilla == id_casilla select p).FirstOrDefault();
                    if (rc != null)
                    {
                        rc.tipo_reserva = motivo;
                    }
                    else
                    {
                        rc = new sice_reserva_captura();
                        rc.id_casilla = id_casilla;
                        rc.tipo_reserva = motivo;
                        contexto.sice_reserva_captura.Add(rc);
                    }
                    contexto.SaveChanges();
                    return 1;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public int guardarDatosVotos(List<sice_votos> listaVotos, int id_casilla, int totalCandidatos)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLOCAL"))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        sice_votos v1 = new sice_votos();
                        foreach (sice_votos voto in listaVotos)
                        {
                            v1.id_candidato = voto.id_candidato;
                            v1.id_casilla = voto.id_casilla;
                            v1.tipo = voto.tipo;
                            v1.votos = voto.votos;
                            contexto.sice_votos.Add(v1);
                            contexto.SaveChanges();
                        }

                        sice_reserva_captura rc = (from p in contexto.sice_reserva_captura where p.id_casilla == id_casilla select p).FirstOrDefault();
                        if(rc != null)
                        {
                            rc.tipo_reserva = "CAPTURADA";
                        }
                        else
                        {
                            rc = new sice_reserva_captura();
                            rc.id_casilla = id_casilla;
                            rc.tipo_reserva = "CAPTURADA";
                            contexto.sice_reserva_captura.Add(rc);
                        }
                        contexto.SaveChanges();
                        TransactionContexto.Complete();
                        return 1;
                    }
                }

            }
            catch(Exception E)
            {
                throw E;
            }             
        }
    }

    public class SeccionCasillaConsecutivo
    {
        public int id { get; set; }
        public int consecutivo { get; set; }
        public int seccion { get; set; }
        public string casilla { get; set; }
        public int distrito { get; set; }
        public int municipio { get; set; }
        public int listaNominal { get; set; }
    }
}
