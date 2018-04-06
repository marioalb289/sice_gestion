using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema.DataModel;
using System.Transactions;

namespace Sistema.Generales
{
    public class RegistroLocalGenerales
    {
        private string con = "MYSQLOCAL";

        public RegistroLocalGenerales()
        {
            if (LoginInfo.privilegios == 6)
            {
                con = "MYSQLSERVER";
            }
            else
            {
                con = "MYSQLOCAL";
            }
        }
        public List<SeccionCasillaConsecutivo> ListaSescciones()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string consulta =
                        "SELECT C.* FROM sice_casillas C " +
                        "LEFT JOIN sice_reserva_captura RC ON RC.id_casilla = C.id " +
                        "WHERE RC.id IS NULL" + " AND C.id_cabecera_local = " + LoginInfo.id_municipio;
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

        public List<sice_distritos_locales> ListaDistritos()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from d in contexto.sice_distritos_locales select d).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public sice_ar_documentos BuscarActaAsignada()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    sice_ar_documentos documento = (from doc in contexto.sice_ar_documentos join asig in contexto.sice_ar_asignacion on doc.id equals asig.id_documento where doc.estatus == "OCUPADO" && asig.id_usuario == LoginInfo.id_usuario select doc).FirstOrDefault();
                    return documento;
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public int verificarCasillaValida(int id_casilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    sice_ar_documentos casilla = (from doc in contexto.sice_ar_documentos where doc.id_casilla == id_casilla && (doc.estatus == "VALIDO" || doc.estatus == "COTEJO") select doc).FirstOrDefault();
                    if (casilla != null)
                    {
                        if (casilla.estatus == "COTEJO")
                            return 1;
                        if (casilla.estatus == "VALIDO")
                            return 2;

                    }
                    return 0;
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }

            }
            catch (Exception E)
            { throw E; }

        }

        public int verificarCasillaRegistrada(int id_casilla)
        {
            try
            {                
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    sice_ar_reserva reserva = (from r in contexto.sice_ar_reserva where (r.tipo_reserva == "ATENDIDO" || r.tipo_reserva == "NO LEGIBLE") && r.id_casilla == id_casilla select r).FirstOrDefault();
                    if (reserva != null)
                    {
                        return 1;

                    }
                    return 0;
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }

            }
            catch (Exception E)
            { throw E; }

        }

        public sice_ar_documentos getDocumentoCasilla(int id_casilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_ar_documentos where p.id_casilla == id_casilla select p).FirstOrDefault();
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
                using (DatabaseContext contexto = new DatabaseContext(con))
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

        public List<VotosSeccion> ResultadosSeccionCaptura(int pageNumber = 0, int pageSize = 0, int id_distrito_local = 0)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string condicion = "";
                    string limit = "";
                    if (pageSize != 0)
                        limit = "LIMIT " + pageNumber + "," + pageSize;
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
                        "FROM sice_ar_votos_cotejo RV " +
                        "LEFT JOIN sice_candidatos CND ON CND.id = RV.id_candidato " +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = CND.fk_partido " +
                        "JOIN sice_casillas C ON C.id = RV.id_casilla " + condicion +
                        "JOIN sice_municipios M ON M.id = C.id_municipio " +
                        "JOIN sice_municipios M2 ON M2.id = C.id_cabecera_local " +
                        "ORDER BY C.seccion ASC, RV.id_casilla ASC, RV.id_candidato DESC " +
                        limit;

                    return contexto.Database.SqlQuery<VotosSeccion>(consulta).ToList();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ResultadosSeccionCapturaTotal(int pageNumber = 0, int pageSize = 0, int id_distrito_local = 0)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string condicion = "";
                    if (id_distrito_local != 0)
                        condicion = " AND C.id_distrito_local = " + id_distrito_local + " ";
                    string consulta =
                        "SELECT " +
                            "COUNT(RV.id) as total " +
                        "FROM sice_ar_votos_cotejo RV " +
                        "LEFT JOIN sice_candidatos CND ON CND.id = RV.id_candidato " +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = CND.fk_partido " +
                        "JOIN sice_casillas C ON C.id = RV.id_casilla " + condicion +
                        "JOIN sice_municipios M ON M.id = C.id_municipio " +
                        "JOIN sice_municipios M2 ON M2.id = C.id_cabecera_local " +
                        "ORDER BY C.seccion ASC, RV.id_casilla ASC, RV.id_candidato DESC ";

                    return contexto.Database.SqlQuery<int>(consulta).FirstOrDefault();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CandidatosVotos> ListaResultadosCasilla(int casilla, string tabla = "")
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    if (tabla == "")
                        tabla = "sice_ar_votos";
                    string consulta =
                        "SELECT " +
                        "V.id,	" +
                        "V.id_casilla as id_casilla, " +
                        "V.tipo as tipo, " +
                        "V.votos as votos, " +
                        "CASE WHEN V.tipo = 'VOTO' THEN V.id_candidato WHEN V.tipo = 'NULO' THEN -2 WHEN V.tipo = 'NO REGISTRADO' THEN -1 END as id_candidato, " +
                        "CONCAT(C.nombre,' ',C.apellido_paterno,' ',C.apellido_materno)as candidato, " +
                        "CD.nombre_candidatura, " +
                        "P.siglas_par as partido, " +
                        "P.img_par as imagen " +
                        "FROM " + tabla + " V " +
                        "LEFT JOIN sice_candidatos C ON C.id = V.id_candidato " +
                        "LEFT JOIN sice_candidaturas CD ON CD.id = C.fk_cargo AND CD.titular = 1 " + //"AND CD.id_distrito =" + distrito +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = C.fk_partido " +
                        "WHERE V.id_casilla = " + casilla + " " + " AND V.tipo <> 'NO VALIDO' " +
                        "ORDER BY id_candidato DESC";
                    return contexto.Database.SqlQuery<CandidatosVotos>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public sice_ar_documentos TomarActa()
        {
            try
            {
                //Buscar que el arcivo no se encuentre ya registrado
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        DateTime localDate = DateTime.Now;
                        sice_ar_documentos doc = (from d in contexto.sice_ar_documentos where d.estatus == "LIBRE" select d).FirstOrDefault();
                        if (doc != null)
                        {
                            sice_ar_documentos tmp = (from d in contexto.sice_ar_documentos where d.id == doc.id select d).FirstOrDefault();
                            //Asignar
                            tmp.estatus = "OCUPADO"; ;
                            tmp.updated_at = localDate;
                            contexto.SaveChanges();

                            sice_ar_asignacion newAsig2 = new sice_ar_asignacion();
                            newAsig2.id_documento = doc.id;
                            newAsig2.id_usuario = LoginInfo.id_usuario;
                            newAsig2.filtro = 1;
                            contexto.sice_ar_asignacion.Add(newAsig2);
                            contexto.SaveChanges();
                            TransactionContexto.Complete();
                            return doc;

                        }
                        return doc;

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int IdentificarActa(int idDocumento, int idCasilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        int res = 0;
                        sice_ar_documentos tmpDoc = (from td in contexto.sice_ar_documentos where td.id_casilla == idCasilla select td).FirstOrDefault();
                        if (tmpDoc != null)
                        {
                            tmpDoc.estatus = "LIBRE";
                            tmpDoc.filtro = null;
                            tmpDoc.id_casilla = null;
                            contexto.SaveChanges();
                        }

                        sice_ar_documentos doc = (from d in contexto.sice_ar_documentos where d.id == idDocumento select d).FirstOrDefault();
                        List<sice_ar_asignacion> asg = (from a in contexto.sice_ar_asignacion where a.id_documento == idDocumento select a).ToList();
                        if (doc != null)
                        {
                            doc.id_casilla = idCasilla;
                            doc.filtro = null;
                            doc.estatus = "VALIDO";
                            contexto.SaveChanges();
                            if(asg.Count > 0)
                            {
                                asg.ForEach(x => x.filtro = null);
                                contexto.SaveChanges();
                            }

                            res = 1;
                        }
                        else
                        {
                            res = 0;
                        }
                        TransactionContexto.Complete();
                        return res;
                    }
                        
                }
                    
            }
            catch(Exception E)
            {
                throw E;
            }
        }

        public int guardarDatosVotos(List<sice_ar_votos_cotejo> listaVotos, int id_casilla,bool nolegible = false)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        sice_ar_votos_cotejo v1 = null;
                        foreach (sice_ar_votos_cotejo voto in listaVotos)
                        {
                            if (voto.id_candidato != null)
                            {
                                v1 = (from d in contexto.sice_ar_votos_cotejo where d.id_candidato == voto.id_candidato && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    v1 = (from d in contexto.sice_ar_votos_cotejo where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    v1 = (from d in contexto.sice_ar_votos_cotejo where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }

                            if (v1 != null)
                            {
                                v1.id_candidato = voto.id_candidato;
                                v1.id_casilla = voto.id_casilla;
                                v1.tipo = voto.tipo;
                                v1.votos = voto.votos;
                                v1.importado = 0;
                                contexto.SaveChanges();
                            }
                            else
                            {
                                return 0;
                            }
                        }

                        sice_ar_reserva rc = (from p in contexto.sice_ar_reserva where p.id_casilla == id_casilla select p).FirstOrDefault();
                        if (rc != null)
                        {
                            rc.tipo_reserva = (nolegible) ? "NO LEGIBLE":"ATENDIDO";                           
                            rc.importado = 0;
                        }
                        else
                        {
                            rc = new sice_ar_reserva();
                            rc.id_casilla = id_casilla;
                            rc.tipo_reserva = (nolegible) ? "NO LEGIBLE" : "ATENDIDO";
                            rc.importado = 0;
                            contexto.sice_ar_reserva.Add(rc);
                        }
                        contexto.SaveChanges();
                        TransactionContexto.Complete();
                        return 1;
                    }
                }

            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public int DescargarDatos(int distrito)
        {
            try
            {
                return 1;
            }
            catch(Exception E)
            {
                throw E;
            }
        }
    }
}
