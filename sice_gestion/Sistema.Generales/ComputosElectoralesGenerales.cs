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
                    sice_reserva_captura rc = new sice_reserva_captura();
                    rc.id_casilla = id_casilla;
                    rc.tipo_reserva = motivo;
                    contexto.sice_reserva_captura.Add(rc);
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
    }
}
