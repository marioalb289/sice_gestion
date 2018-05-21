using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema.DataModel;
using System.Transactions;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

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

        public sice_configuracion_recuento Configuracion_Recuento(string sistema)
        {
            using (DatabaseContext contexto = new DatabaseContext(con))
            {
                return (from p in contexto.sice_configuracion_recuento where p.sistema == sistema select p).FirstOrDefault();
            }
        }

        public List<sice_ar_supuestos> ListaSupuestos()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_ar_supuestos select p).ToList();
                }
                    
            }
            catch(Exception E)
            {
                throw E;
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
                        "WHERE C.id_cabecera_local = " + LoginInfo.id_municipio;
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

        public List<sice_ar_estatus_acta> ListaEstatusActa()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_ar_estatus_acta select p).ToList();
                }
            }
            catch(Exception E)
            {
                throw E;
            }
        }
        public List<sice_ar_estatus_paquete> ListaEstatusPaquete()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_ar_estatus_paquete select p).ToList();
                }
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public List<sice_ar_incidencias> ListaIncidencias()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_ar_incidencias select p).ToList();
                }
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public List<sice_distritos_locales> ListaDistritos()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string condicion = " "; 
                    if(LoginInfo.privilegios == 5)
                    {
                        condicion = "WHERE C.id_cabecera_local = " + LoginInfo.id_municipio + " ";
                    }

                    string consulta =
                        "SELECT D.* FROM sice_casillas C " +
                        "JOIN sice_distritos_locales D on D.id = C.id_distrito_local " +
                        condicion+
                        "GROUP BY C.id_distrito_local ";
                    List<sice_distritos_locales> lsCasilla = contexto.Database.SqlQuery<sice_distritos_locales>(consulta).ToList();
                    return lsCasilla;
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
        public sice_ar_supuestos getSupuesto(int id_casilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_ar_reserva join sup in contexto.sice_ar_supuestos on p.id_supuesto equals sup.id where p.id_casilla == id_casilla select sup).FirstOrDefault();
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
                        "JOIN sice_partidos_politicos P ON P.id = C.fk_partido " +
                        "ORDER BY P.prelacion ASC";
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
                            "CASE WHEN RV.tipo = 'VOTO' THEN P.prelacion WHEN RV.tipo = 'NULO' THEN 200 WHEN RV.tipo = 'NO REGISTRADO' THEN  100 END AS prelacion, " +
                            "RV.votos," +
                            "RV.tipo," +
                            "RES.tipo_reserva as estatus, " +
                            "EA.estatus AS estatus_acta, " +
                            "EA.id AS id_estatus_acta, " +
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
                        "LEFT JOIN sice_ar_reserva RES ON RES.id_casilla = RV.id_casilla " +
                        "LEFT JOIN sice_ar_estatus_acta EA ON RES.id_estatus_acta = EA.id "+
                        "ORDER BY C.seccion ASC, RV.id_casilla ASC, prelacion ASC " +
                        limit;

                    return contexto.Database.SqlQuery<VotosSeccion>(consulta).ToList();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<sice_ar_votos_cotejo> ResultadosVotosCotejo(int pageNumber = 0, int pageSize = 0, int id_distrito_local = 0)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string result = string.Join(",",LoginInfo.lista_distritos);
                    string condicion = " AND C.id_distrito_local IN ("+result+") ";
                    string consulta =
                        "SELECT " +
                            "RV.* " +
                        "FROM sice_ar_votos_cotejo RV " +
                        "LEFT JOIN sice_candidatos CND ON CND.id = RV.id_candidato " +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = CND.fk_partido " +
                        "JOIN sice_casillas C ON C.id = RV.id_casilla " + condicion +
                        "JOIN sice_municipios M ON M.id = C.id_municipio " +
                        "JOIN sice_municipios M2 ON M2.id = C.id_cabecera_local " +
                        "LEFT JOIN sice_ar_reserva RES ON RES.id_casilla = RV.id_casilla " +
                        "LEFT JOIN sice_ar_estatus_acta EA ON RES.id_estatus_acta = EA.id " +
                        "ORDER BY C.seccion ASC, RV.id_casilla ASC, prelacion ASC ";

                    return contexto.Database.SqlQuery<sice_ar_votos_cotejo>(consulta).ToList();
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

        public sice_ar_reserva DetallesActa(int id_casilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from r in contexto.sice_ar_reserva where r.id_casilla == id_casilla select r).FirstOrDefault();
                }
            }
            catch (Exception E)
            {
                throw E;
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
                        "CASE WHEN V.tipo = 'VOTO' THEN P.prelacion WHEN V.tipo = 'NULO' THEN 200 WHEN V.tipo = 'NO REGISTRADO' THEN	100 END AS prelacion, " +
                        "CONCAT(C.nombre,' ',C.apellido_paterno,' ',C.apellido_materno)as candidato, " +
                        "CD.nombre_candidatura, " +
                        "P.siglas_par as partido, " +
                        "P.img_par as imagen " +
                        "FROM " + tabla + " V " +
                        "LEFT JOIN sice_candidatos C ON C.id = V.id_candidato " +
                        "LEFT JOIN sice_candidaturas CD ON CD.id = C.fk_cargo AND CD.titular = 1 " + //"AND CD.id_distrito =" + distrito +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = C.fk_partido " +
                        "WHERE V.id_casilla = " + casilla + " " + " AND V.tipo <> 'NO VALIDO' " +
                        "ORDER BY prelacion ASC";
                    return contexto.Database.SqlQuery<CandidatosVotos>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public List<CasillasRecuento> ListaCasillasRecuentos(int distrito,bool completo = false)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string join = "JOIN sice_casillas C ON C.id = R.id_casilla AND C.id_distrito_local = " + distrito + " ";
                    if (completo)
                        join = "JOIN sice_casillas C ON C.id = R.id_casilla ";
                    string consulta =
                        "SELECT " +
                            "C.id as id_casilla, " +
                            " C.seccion, " +
                            "C.tipo_casilla as casilla, " +
                            "S.supuesto " +
                        "FROM sice_ar_reserva R " +
                        join +
                        "JOIN sice_ar_supuestos S ON S.id = R.id_supuesto " +
                        "WHERE R.id_supuesto IS NOT NULL ";
                    return contexto.Database.SqlQuery<CasillasRecuento>(consulta).ToList();
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
                            tmp.estatus = "OCUPADO";
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

        public int ActaNoLegible(int idDocumento)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        int res = 0;
                        sice_ar_documentos doc = (from td in contexto.sice_ar_documentos where td.id == idDocumento select td).FirstOrDefault();
                        if (doc != null)
                        {
                            doc.estatus = "CANCELADO";
                            contexto.SaveChanges();
                            res = 1;
                        }
                        TransactionContexto.Complete();
                        return res;
                    }

                }

            }
            catch (Exception E)
            {
                throw E;
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
                            tmpDoc.importado_dato = null;
                            contexto.SaveChanges();
                        }

                        sice_ar_documentos doc = (from d in contexto.sice_ar_documentos where d.id == idDocumento select d).FirstOrDefault();
                        List<sice_ar_asignacion> asg = (from a in contexto.sice_ar_asignacion where a.id_documento == idDocumento select a).ToList();
                        if (doc != null)
                        {
                            doc.id_casilla = idCasilla;
                            doc.filtro = null;
                            doc.identificado = DateTime.Now;
                            doc.importado_dato = 0;
                            doc.estatus = "VALIDO";
                            //doc.id_estatus_acta = estatus_acta;
                            //doc.id_estatus_paquete = estatus_paquete;
                            //if (incidencias == 0)
                            //    doc.id_incidencias = null;
                            //else
                            //    doc.id_incidencias = incidencias;
                            //doc.casilla_instalada = casilla_instalada;
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



        public int guardarDatosVotos(List<sice_ar_votos_cotejo> listaVotos, int id_casilla, int supuesto,int boletasSobrantes,int numEscritos,int personas_votaron,
            int representantes,int votos_sacados,int incidencias,int estatus_acta,int estatus_paquete,bool modificar = false)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        bool ceros = true;
                        if (estatus_acta == 1 || estatus_acta == 2 || estatus_acta == 8)
                        {
                            ceros = false;
                        }
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
                                v1.votos = ceros ? 0 : voto.votos;
                                v1.importado = 0;
                                v1.estatus = 1;                                
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
                            rc.tipo_reserva = "ATENDIDO";
                            rc.num_escritos = ceros ? 0 : numEscritos;
                            if (supuesto == 0)
                                rc.id_supuesto = null;
                            else
                                rc.id_supuesto = supuesto;
                            rc.boletas_sobrantes = ceros ? 0 :boletasSobrantes;
                            rc.personas_votaron = ceros ? 0 : personas_votaron;
                            rc.num_representantes_votaron = ceros ? 0  : representantes;
                            rc.votos_sacados = ceros ? 0 : votos_sacados;
                            rc.id_estatus_acta = estatus_acta;
                            rc.id_estatus_paquete = estatus_paquete;
                            if (incidencias == 0)
                                rc.id_incidencias = null;
                            else
                                rc.id_incidencias = incidencias;
                            rc.importado = 0;
                            rc.inicializada = 0;
                            rc.updated_at = DateTime.Now;
                        }
                        else
                        {
                            rc = new sice_ar_reserva();
                            rc.id_casilla = id_casilla;
                            rc.tipo_reserva = "ATENDIDO";
                            rc.create_at = DateTime.Now;
                            rc.updated_at = DateTime.Now;
                            rc.num_escritos = ceros ? 0  : numEscritos;
                            rc.importado = 0;
                            if (supuesto == 0)
                                rc.id_supuesto = null;
                            else
                                rc.id_supuesto = supuesto;
                            rc.boletas_sobrantes = ceros ? 0 : boletasSobrantes;
                            rc.personas_votaron = ceros ? 0 : personas_votaron;
                            rc.num_representantes_votaron = ceros ? 0 : representantes;
                            rc.votos_sacados = ceros ? 0  : votos_sacados;
                            rc.id_estatus_acta = estatus_acta;
                            rc.id_estatus_paquete = estatus_paquete;
                            rc.inicializada = 0;
                            if (incidencias == 0)
                                rc.id_incidencias = null;
                            else
                                rc.id_incidencias = incidencias;
                            contexto.sice_ar_reserva.Add(rc);
                        }
                        if (modificar)
                        {
                            sice_ar_historico hs = new sice_ar_historico();
                            hs.id_casilla = id_casilla;
                            if (supuesto == 0)
                                hs.id_supuesto = null;
                            else
                                hs.id_supuesto = supuesto;
                            hs.fecha = DateTime.Now;
                            contexto.sice_ar_historico.Add(hs);
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

        public int GuardarConfiguracionRecuento(double horas,int propietarios,int suplentes)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        int res = 0;

                        sice_configuracion_recuento conf = (from c in contexto.sice_configuracion_recuento where c.sistema == "RA" select c).FirstOrDefault();
                        if(conf != null)
                        {
                            conf.no_consejeros = propietarios;
                            conf.no_suplentes = suplentes;
                            conf.horas_disponibles = Convert.ToSingle(horas);
                            contexto.SaveChanges();
                            res = 1;
                        }
                        else
                        {
                            sice_configuracion_recuento newConf = new sice_configuracion_recuento();
                            newConf.sistema = "RA";
                            newConf.no_consejeros = propietarios;
                            newConf.no_suplentes = suplentes;
                            newConf.horas_disponibles = Convert.ToSingle(horas);
                            contexto.sice_configuracion_recuento.Add(newConf);
                            contexto.SaveChanges();
                            res = 1;
                        }
                        
                        TransactionContexto.Complete();
                        return res;
                    }

                }

            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public List<int> ListaCasillaCapturadasRegActas()
        {
            try
            {
                List<int> listaCasilla = new List<int>();
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    listaCasilla = (from v in contexto.sice_ar_votos_cotejo where v.estatus == 1 select (int)v.id_casilla).Distinct().ToList();

                    return listaCasilla;
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
                //Obtener lista de casillas ya registrdas
                List<int> listaCasilla = ListaCasillaCapturadasRegActas();
                List<sice_ar_votos_cotejo> listaVotosImportar = new List<sice_ar_votos_cotejo>();
                string condicion = "";
                string condicion2 = "";
                string casilla;
                if (listaCasilla.Count > 0)
                {
                    casilla = string.Join(",", listaCasilla);
                    condicion = " AND RV.id_casilla NOT IN( " + casilla + " ) ";
                    condicion2 = " AND R.id_casilla NOT IN( " + casilla + " ) ";
                }

                //Buscar votos en la bd del servidor excluyendo casilla ya registradas o descargadas
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    string consulta =
                        "SELECT " +
                            "RV.* " +
                        "FROM " +
                        "sice_ar_votos_cotejo RV " +                        
                        "JOIN sice_casillas C ON C.id = RV.id_casilla AND C.id_distrito_local = " + distrito + " "+
                        "JOIN sice_ar_reserva RES ON RES.id_casilla = RV.id_casilla AND RES.tipo_reserva = 'ATENDIDO' " +
                        "WHERE RV.estatus = 1 " + condicion;
                    listaVotosImportar = contexto.Database.SqlQuery<sice_ar_votos_cotejo>(consulta).ToList();


                }
                if (listaVotosImportar.Count > 0 )
                {
                    //Guardar Datos
                    guardarVotosImportados(listaVotosImportar);
                }
                else
                {
                    return 2;
                }

                return 1;
            }
            catch(Exception E)
            {
                return 0;
            }
        }

        public void guardarVotosImportados(List<sice_ar_votos_cotejo> listaVotos)
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
                                v1.importado = 1;
                                v1.estatus = 1;
                                
                            }

                            sice_ar_reserva rc = (from p in contexto.sice_ar_reserva where p.id_casilla == voto.id_casilla select p).FirstOrDefault();
                            if (rc != null)
                            {
                                rc.tipo_reserva = "ATENDIDO";
                                rc.importado = 1;
                            }
                            else
                            {
                                rc = new sice_ar_reserva();
                                rc.id_casilla = voto.id_casilla;
                                rc.tipo_reserva = "ATENDIDO";
                                rc.importado = 1;
                                contexto.sice_ar_reserva.Add(rc);
                            }
                            contexto.SaveChanges();
                        }
                        TransactionContexto.Complete();
                    }
                }
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public void validarPuntosRecuento()
        {
            try
            {
                List<sice_distritos_locales> distritos = this.ListaDistritos();
                int totalRecuento = this.ListaCasillasRecuentos(0, true).Count();
                sice_configuracion_recuento conf = this.Configuracion_Recuento("RA");
                if (conf == null)
                    throw new Exception("No se establecio la Configuración para Puntos de Recuento");
                int propietarios = (int)conf.no_consejeros;
                int suplentes = (int)conf.no_suplentes;
                int grupos_tabajo = (propietarios - 3) + suplentes;
                if (grupos_tabajo > 5)
                    grupos_tabajo = 5;
                int segmentos = Convert.ToInt32(conf.horas_disponibles * 2);

                double parcialPuntoRecuento = (((double)totalRecuento / (double)grupos_tabajo) / (double)segmentos);
                int puntos_recuento = this.Round(parcialPuntoRecuento);


                if (puntos_recuento > 8)
                {
                    throw new Exception("La Configuración Actual arroja mas de 8 puntos de Recuento. No se puede tener mas de 8 puntos de Recuento.\nModifique la configuración para Recuento");
                }

            }
            catch(Exception E)
            {
                throw E;
            }
        }

        public int Round(double numero)
        {
            if (numero < 1.0)
                return 1;
            double decimalpoints = Math.Abs(numero - Math.Floor(numero));
            if (decimalpoints > 0.30)
                return (int)Math.Floor(numero) + 1;
            else
                return (int)Math.Floor(numero);
        }

        public int generarExcelRespaldo(SaveFileDialog fichero, int distrito, bool completo = false)
        {
            try
            {

                Excel.Application excel = new Excel.Application();
                Excel._Workbook libro = null;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                libro = (Excel._Workbook)excel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);

                List<string> entidades = new List<string>(new string[] { "sice_ar_documentos","sice_ar_historico","sice_ar_reserva","sice_ar_votos_cotejo"});
                
                foreach(string entidad in entidades)
                {
                    this.generaHojaRespaldo(entidad, libro);
                }
                
                

                ((Excel.Worksheet)excel.ActiveWorkbook.Sheets["Hoja1"]).Delete();   //Borramos la hoja que crea en el libro por defecto


                libro.Saved = true;
                //libro.SaveAs(Environment.CurrentDirectory + @"\Ejemplo2.xlsx");  // Si es un libro nuevo
                //libro.SaveAs(fichero.FileName,
                //Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                object misValue = System.Reflection.Missing.Value;
                libro.SaveAs(fichero.FileName, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue,
                misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                Excel.XlSaveConflictResolution.xlUserResolution, true,
                misValue, misValue, misValue);

                libro.Close(true, misValue, misValue);

                excel.UserControl = false;
                excel.Quit();

                Marshal.ReleaseComObject(libro);
                //Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(excel);
                return 1;



            }
            catch (Exception E)
            {
                return 0;
            }
        }
        public void generaHojaRespaldo(string entidad, Excel._Workbook libro)
        {
            try
            {
                Excel._Worksheet hoja = null;
                Excel.Range rango = null;
                int filaInicialTabla = 11;


                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                hoja = (Excel._Worksheet)libro.Worksheets.Add();
                hoja.Name = entidad;  //Aqui debe ir el nombre de la tabla a respaldar


                //Montamos las cabeceras 
                CrearEncabezadosRespaldo(ref hoja, entidad);


                //return;
                //Agregar Datos
                int fila = 2; int columna = 1;

                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    switch (entidad)
                    {
                        case "sice_ar_documentos":
                            List<sice_ar_documentos> data = (from d in contexto.sice_ar_documentos select d).ToList();
                            foreach(sice_ar_documentos d in data)
                            {
                                hoja.Cells[fila, 1] = d.id;
                                hoja.Cells[fila, 2] = d.nombre;
                                hoja.Cells[fila, 3] = d.ruta;
                                hoja.Cells[fila, 4] = d.estatus;
                                hoja.Cells[fila, 5] = d.filtro;
                                hoja.Cells[fila, 6] = d.estatus_filtro1;
                                hoja.Cells[fila, 7] = d.estatus_filtro2;
                                hoja.Cells[fila, 8] = d.estatus_filtro3;
                                hoja.Cells[fila, 9] = d.estatus_revisor;
                                hoja.Cells[fila, 10] = d.estatus_cotejador;
                                hoja.Cells[fila, 11] = d.id_casilla;
                                hoja.Cells[fila, 12] = d.identificado;
                                hoja.Cells[fila, 13] = d.create_at;
                                hoja.Cells[fila, 14] = d.updated_at;
                                hoja.Cells[fila, 15] = d.importado;
                                hoja.Cells[fila, 16] = d.importado_dato;

                                fila++;
                            }
                            
                        break;
                        case "sice_ar_historico":
                            List<sice_ar_historico> data2 = (from d in contexto.sice_ar_historico select d).ToList();
                            foreach (sice_ar_historico d in data2)
                            {
                                hoja.Cells[fila, 1] = d.id;
                                hoja.Cells[fila, 2] = d.id_supuesto;
                                hoja.Cells[fila, 3] = d.fecha;
                                hoja.Cells[fila, 4] = d.id_casilla;
                                hoja.Cells[fila, 5] = d.importado;
                                fila++;
                            }
                        break;
                        case "sice_ar_reserva":
                            List<sice_ar_reserva> data3 = (from d in contexto.sice_ar_reserva select d).ToList();
                            foreach (sice_ar_reserva d in data3)
                            {
                                hoja.Cells[fila, 1] = d.id;
                                hoja.Cells[fila, 2] = d.id_casilla;
                                hoja.Cells[fila, 3] = d.tipo_reserva;
                                hoja.Cells[fila, 4] = d.id_documento;
                                hoja.Cells[fila, 5] = d.importado;
                                hoja.Cells[fila, 6] = d.id_supuesto;
                                hoja.Cells[fila, 7] = d.create_at;
                                hoja.Cells[fila, 8] = d.updated_at;
                                hoja.Cells[fila, 9] = d.num_escritos;
                                hoja.Cells[fila, 10] = d.boletas_sobrantes;
                                hoja.Cells[fila, 11] = d.personas_votaron;
                                hoja.Cells[fila, 12] = d.num_representantes_votaron;
                                hoja.Cells[fila, 13] = d.votos_sacados;
                                hoja.Cells[fila, 14] = d.casilla_instalada;
                                hoja.Cells[fila, 15] = d.id_estatus_acta;
                                hoja.Cells[fila, 16] = d.id_estatus_paquete;
                                hoja.Cells[fila, 17] = d.id_incidencias;
                                hoja.Cells[fila, 18] = d.inicializada;
                                fila++;
                            }
                        break;
                        case "sice_ar_votos_cotejo":
                            List<sice_ar_votos_cotejo> data4 = this.ResultadosVotosCotejo();
                            foreach (sice_ar_votos_cotejo d in data4)
                            {
                                hoja.Cells[fila, 1] = d.id;
                                hoja.Cells[fila, 2] = d.id_candidato;
                                hoja.Cells[fila, 3] = d.id_casilla;
                                hoja.Cells[fila, 4] = d.votos;
                                hoja.Cells[fila, 5] = d.tipo;
                                hoja.Cells[fila, 6] = d.importado;
                                hoja.Cells[fila, 7] = d.estatus;
                                fila++;
                            }
                        break;
                    }
                }

            }
            catch (Exception E)
            {
                throw E;
            }
        }
        private void CrearEncabezadosRespaldo(ref Excel._Worksheet hoja, string entidad)
        {
            try
            {
                Excel.Range rango;
                List<string> nombres = new List<string>();
                switch (entidad)
                {
                    case "sice_ar_documentos":
                        nombres = typeof(sice_ar_documentos).GetProperties()
                       .Select(property => property.Name)
                       .ToList();
                        break;
                    case "sice_ar_historico":
                        nombres = typeof(sice_ar_historico).GetProperties()
                       .Select(property => property.Name)
                       .ToList();
                        break;
                    case "sice_ar_reserva":
                        nombres = typeof(sice_ar_reserva).GetProperties()
                       .Select(property => property.Name)
                       .ToList();
                        break;
                    case "sice_ar_votos_cotejo":
                        nombres = typeof(sice_ar_votos_cotejo).GetProperties()
                       .Select(property => property.Name)
                       .ToList();
                        break;
                }
               
                int cont = 1;
                foreach(string nombre in nombres)
                {
                    hoja.Cells[1, cont] = nombre;
                    cont++;
                }                
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public int generarExcelRecuento(SaveFileDialog fichero, int distrito, bool completo = false)
        {
            try
            {

                Excel.Application excel = new Excel.Application();
                Excel._Workbook libro = null;
                completo = true;//Se cambio aqui para generar reporte de todo el recuento

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                libro = (Excel._Workbook)excel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);

                if (completo)
                {
                    List<sice_distritos_locales> distritos = this.ListaDistritos();
                    int totalRecuento = this.ListaCasillasRecuentos(distrito, true).Count();
                    sice_configuracion_recuento conf = this.Configuracion_Recuento("RA");
                    int grupos_tabajo = 0;
                    int puntos_recuento = 0;
                    if (conf != null)
                    {
                        int propietarios = (int)conf.no_consejeros;
                        int suplentes = (int)conf.no_suplentes;
                        grupos_tabajo = (propietarios - 3) + suplentes;
                        if (grupos_tabajo > 5)
                            grupos_tabajo = 5;

                        DateTime fecha1 = new DateTime(2018, 7, 8, 8, 0, 0);
                        DateTime fecha2 = new DateTime(2018, 7, 11, 0, 0, 0);
                        double horasRestantes = Math.Floor((fecha2 - fecha1).TotalHours);

                        int segmentos = (Convert.ToInt32(horasRestantes) - Convert.ToInt32(conf.horas_disponibles)) * 2;

                        double parcialPuntoRecuento = (((double)totalRecuento / (double)grupos_tabajo) / (double)segmentos);
                        puntos_recuento = this.Round(parcialPuntoRecuento);
                    }
                        
                    




                    foreach (sice_distritos_locales ds in distritos.OrderByDescending(x => x.id))
                    {
                        Console.WriteLine("Insetando Libro: " + ds.distrito);


                        List<Candidatos> listaCandidatos = this.ListaCandidatos(ds.id);
                        int tc = listaCandidatos.Count;
                        List<VotosSeccion> vSeccionTotales = this.ResultadosSeccionCaptura(0, 0, ds.id);
                        List<VotosSeccion> totalAgrupado = vSeccionTotales.GroupBy(x => x.id_casilla).Select(data => new VotosSeccion { id_candidato = data.First().id_candidato, casilla = data.First().casilla, lista_nominal = data.First().lista_nominal + tc * 2, votos = data.First().votos }).ToList();
                        int TotalVotosDistrito = vSeccionTotales.Sum(x => (int)x.votos);
                        decimal diferencia = 0;
                        List<VotosSeccion> listaSumaCandidatos = vSeccionTotales.Where(x => x.estatus == "ATENDIDO" && x.id_candidato != null).GroupBy(y => y.id_candidato).Select(data => new VotosSeccion { id_candidato = data.First().id_candidato, votos = data.Sum(d => d.votos) }).OrderBy(x => x.votos).ToList();
                        if (listaSumaCandidatos.Count > 0)
                        {
                            int PrimeroTotal = (int)listaSumaCandidatos[listaSumaCandidatos.Count - 1].votos;
                            int SeegundoTotal = (int)listaSumaCandidatos[listaSumaCandidatos.Count - 2].votos;
                            int diferenciaTotal = PrimeroTotal - SeegundoTotal;
                            if (TotalVotosDistrito > 0)
                            {
                                diferencia = Math.Round((Convert.ToDecimal(diferenciaTotal) * 100) / TotalVotosDistrito, 2);
                            }
                        }

                        this.generaHojaRecuento(ds.id, libro,diferencia, totalRecuento,grupos_tabajo,puntos_recuento);

                    }
                }
                else
                {
                    this.generaHojaRecuento(distrito, libro);
                }

                ((Excel.Worksheet)excel.ActiveWorkbook.Sheets["Hoja1"]).Delete();   //Borramos la hoja que crea en el libro por defecto


                libro.Saved = true;
                //libro.SaveAs(Environment.CurrentDirectory + @"\Ejemplo2.xlsx");  // Si es un libro nuevo
                //libro.SaveAs(fichero.FileName,
                //Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                object misValue = System.Reflection.Missing.Value;
                libro.SaveAs(fichero.FileName, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue,
                misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                Excel.XlSaveConflictResolution.xlUserResolution, true,
                misValue, misValue, misValue);

                libro.Close(true, misValue, misValue);

                excel.UserControl = false;
                excel.Quit();

                Marshal.ReleaseComObject(libro);
                //Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(excel);
                return 1;



            }
            catch (Exception E)
            {
                return 0;
            }
        }

        public void generaHojaRecuento(int distrito, Excel._Workbook libro,decimal diferencia = 0,int totalRecuento = 0,int grupos_trabajo = 0,int puntos_recuento = 0)
        {
            try
            {
                Excel._Worksheet hoja = null;
                Excel.Range rango = null;
                int filaInicialTabla = 11;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                hoja = (Excel._Worksheet)libro.Worksheets.Add();
                hoja.Name = "DISTRITO " + distrito;  //Aqui debe ir el nombre del distrito

                List<CasillasRecuento> listaRecuento = this.ListaCasillasRecuentos(distrito);
                
                //Montamos las cabeceras 
                char letraFinal = CrearEncabezadosRecuento(filaInicialTabla, ref hoja, distrito,listaRecuento.Count,totalRecuento,diferencia,grupos_trabajo,puntos_recuento, 1);

                //return;
                //Agregar Datos
                int fila = filaInicialTabla + 1;
                int idCasillaActual = 0;
                int cont = 1;
                int contCand = 6;
                //row.Cells[0].Value = 1;
                //dgvResultados.Rows.Add(row);
                List<int> vLst = new List<int>();
                int Noregynulo = 0;
                int Lnominal = 0;

               
                if(listaRecuento.Count > 0)
                {
                    foreach(CasillasRecuento casillla in listaRecuento)
                    {
                        //Agregar Columnas
                        hoja.Cells[fila, 1] = casillla.id_casilla;
                        hoja.Cells[fila, 2] = casillla.seccion;
                        hoja.Cells[fila, 3] = casillla.casilla;
                        hoja.Cells[fila, 4] = casillla.supuesto;

                        //Agregar fila
                        string x = "A" + (fila).ToString();
                        string y = letraFinal.ToString() + (fila).ToString();
                        rango = hoja.Range[x, y];
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        //Console.WriteLine("Ins")
                        fila++;
                    }
                }
                
            }
            catch (Exception E)
            {
                throw E;
            }
        }       

        private char CrearEncabezadosRecuento(int fila, ref Excel._Worksheet hoja, int distrito,int totalDistritoCasillasRecuento,int totalRecuento,decimal diferencia = 0, int grupos_trabajo = 0, int puntos_recuento = 0, int columnaInicial = 1)
        {
            try
            {
                Excel.Range rango;
                string rutaImagen = System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\";

                sice_casillas casilla = null;
                sice_distritos_locales dlocal = null;
                sice_municipios mun = null;

                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    casilla = (from c in contexto.sice_casillas where c.id_distrito_local == distrito select c).FirstOrDefault();
                    mun = (from m in contexto.sice_municipios where m.id == casilla.id_cabecera_local select m).FirstOrDefault();
                    dlocal = (from d in contexto.sice_distritos_locales where d.id == distrito select d).FirstOrDefault();
                }
                //Configuracon Hoja
                hoja.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
                hoja.PageSetup.Zoom = 60;
                hoja.PageSetup.PrintTitleRows = "$10:$11";

                //** Montamos el título en la línea 1 **
                hoja.Cells[1, 3] = "SISTEMA DE REGISTRO DE ACTAS DEL PROCESO ELECTORAL LÓCAL 2017-2018";
                hoja.Range[hoja.Cells[1, 3], hoja.Cells[1, 4]].Merge();
                hoja.Cells[2, 3] = "ELECCIÓN DE DIPUTADOS DE MAYORÍA RELATIVA POR CASILLA, SECCIÓN Y DISTRITO LOCAL";
                hoja.Range[hoja.Cells[2, 3], hoja.Cells[2, 4]].Merge();
                hoja.Cells[3, 3] = "LISTA DE CASILLAS A RECUENTO";
                hoja.Range[hoja.Cells[3, 3], hoja.Cells[3, 4]].Merge();
                char columnaLetra = 'A';
                hoja.Shapes.AddPicture(rutaImagen + "iepc.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 125, 52);
                //hoja.Shapes.

                List<double> widths = new List<double>();

                //Agregar encabezados
                hoja.Cells[fila - 7, columnaInicial] = "DIFERENCIA ENTRE 1° Y 2° LUGAR";
                hoja.Cells[fila - 7, columnaInicial].RowHeight = 35;
                hoja.Range[hoja.Cells[fila - 7, columnaInicial], hoja.Cells[fila - 7, columnaInicial + 1]].Merge();
                hoja.Cells[fila - 7, columnaInicial].WrapText = true;
                hoja.Cells[fila - 7, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                hoja.Cells[fila - 7, columnaInicial].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                hoja.Cells[fila - 7, columnaInicial + 2] = diferencia + "%"; //Aqui debe sacar calculo
                hoja.Range[hoja.Cells[fila - 7, columnaInicial + 2], hoja.Cells[fila - 7, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 7, columnaInicial + 2].WrapText = true;
                hoja.Cells[fila - 7, columnaInicial + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                hoja.Cells[fila - 7, columnaInicial + 2].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                hoja.Cells[fila - 7, columnaInicial + 2].Font.Bold = true;

                hoja.Cells[fila - 6, columnaInicial] = "TIPO DE RECUENTO";
                hoja.Range[hoja.Cells[fila - 6, columnaInicial], hoja.Cells[fila - 6, columnaInicial + 1]].Merge();
                hoja.Cells[fila - 6, columnaInicial].WrapText = true;
                hoja.Cells[fila - 6, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                hoja.Cells[fila - 6, columnaInicial].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                if(diferencia == 0)
                {
                    hoja.Cells[fila - 6, columnaInicial + 2] = "NO APLICA"; //Si diferencia menos a 1% recuento Total, sino Parcial
                }
                else
                {
                    hoja.Cells[fila - 6, columnaInicial + 2] = (diferencia < 1) ? "TOTAL" : "PARCIAL"; //Si diferencia menos a 1% recuento Total, sino Parcial
                }
                hoja.Range[hoja.Cells[fila - 6, columnaInicial + 2], hoja.Cells[fila - 6, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 6, columnaInicial + 2].WrapText = true;
                hoja.Cells[fila - 6, columnaInicial + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                hoja.Cells[fila - 6, columnaInicial + 2].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                hoja.Cells[fila - 6, columnaInicial + 2].Font.Bold = true;

                hoja.Cells[fila - 5, columnaInicial] = "TOTAL CASILLAS A RECUENTO ";
                hoja.Cells[fila - 5, columnaInicial].RowHeight = 35;
                hoja.Range[hoja.Cells[fila - 5, columnaInicial], hoja.Cells[fila - 5, columnaInicial + 1]].Merge();
                hoja.Cells[fila - 5, columnaInicial].WrapText = true;
                hoja.Cells[fila - 5, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                hoja.Cells[fila - 5, columnaInicial].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                hoja.Cells[fila - 5, columnaInicial + 2] = totalRecuento; //TOTAL DE CASILLAS A RECUENTO
                hoja.Range[hoja.Cells[fila - 5, columnaInicial + 2], hoja.Cells[fila - 5, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 5, columnaInicial + 2].WrapText = true;
                hoja.Cells[fila - 5, columnaInicial + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                hoja.Cells[fila - 5, columnaInicial + 2].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                hoja.Cells[fila - 5, columnaInicial + 2].Font.Bold = true;

                hoja.Cells[fila - 4, columnaInicial] = "TOTAL CASILLAS A RECUENTO " + dlocal.distrito;
                hoja.Cells[fila - 4, columnaInicial].RowHeight = 35;
                hoja.Range[hoja.Cells[fila - 4, columnaInicial], hoja.Cells[fila - 4, columnaInicial + 1]].Merge();
                hoja.Cells[fila - 4, columnaInicial].WrapText = true;
                hoja.Cells[fila - 4, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                hoja.Cells[fila - 4, columnaInicial].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                hoja.Cells[fila - 4, columnaInicial + 2] = totalDistritoCasillasRecuento; //TOTAL DE CASILLAS A RECUENTO
                hoja.Range[hoja.Cells[fila - 4, columnaInicial + 2], hoja.Cells[fila - 4, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 4, columnaInicial + 2].WrapText = true;
                hoja.Cells[fila - 4, columnaInicial + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                hoja.Cells[fila - 4, columnaInicial + 2].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                hoja.Cells[fila - 4, columnaInicial + 2].Font.Bold = true;

                hoja.Cells[fila - 3, columnaInicial] = "GRUPOS DE TRABAJO";
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 3, columnaInicial + 1]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                hoja.Cells[fila - 3, columnaInicial].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                hoja.Cells[fila - 3, columnaInicial + 2] = totalRecuento == 0 || totalRecuento < 20 || grupos_trabajo == 0 ? "NO APLICA" : grupos_trabajo.ToString(); //CALCULAR NUMERO DE GRUPOS DE TRABAJO
                hoja.Range[hoja.Cells[fila - 3, columnaInicial + 2], hoja.Cells[fila - 3, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 3, columnaInicial + 2].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                hoja.Cells[fila - 3, columnaInicial + 2].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                hoja.Cells[fila - 3, columnaInicial + 2].Font.Bold = true;

                hoja.Cells[fila - 2, columnaInicial] = "PUNTOS DE RECUENTO";
                hoja.Range[hoja.Cells[fila - 2, columnaInicial], hoja.Cells[fila - 2, columnaInicial + 1]].Merge();
                hoja.Cells[fila - 2, columnaInicial].WrapText = true;
                hoja.Cells[fila - 2, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                hoja.Cells[fila - 2, columnaInicial].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                hoja.Cells[fila - 2, columnaInicial + 2] = totalRecuento == 0 || totalRecuento < 20 || puntos_recuento == 0 ? "NO APLICA" : puntos_recuento.ToString(); //PUNTOS DE RECUENTO
                hoja.Range[hoja.Cells[fila - 2, columnaInicial + 2], hoja.Cells[fila - 2, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 2, columnaInicial + 2].WrapText = true;
                hoja.Cells[fila - 2, columnaInicial + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                hoja.Cells[fila - 2, columnaInicial + 2].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                hoja.Cells[fila - 2, columnaInicial + 2].Font.Bold = true;


                hoja.Cells[fila - 1, columnaInicial] = dlocal.distrito+" CABECERA "+mun.municipio;
                hoja.Range[hoja.Cells[fila - 1, columnaInicial], hoja.Cells[fila - 1, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 1, columnaInicial].WrapText = true;
                hoja.Cells[fila - 1, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                hoja.Cells[fila, columnaInicial] = "No."; columnaInicial++; columnaLetra++; widths.Add(8.57);
                hoja.Cells[fila, columnaInicial] = "Sección"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "Casilla"; columnaInicial++; columnaLetra++; widths.Add(25.29);
                hoja.Cells[fila, columnaInicial] = "Motivo Recuento"; columnaInicial++;widths.Add(100);              

                //Colores de Fondo
                rango = hoja.Range["A" + fila, "D" + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                              

                //Ponemos borde a las celdas
                string letra = columnaLetra.ToString() + fila;
                rango = hoja.Range["A" + (fila - 7), letra];
                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                //Centramos los textos
                rango = hoja.Rows[fila];
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                //Colores titulo1
                rango = hoja.Range["C1", "C1"];
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(38)))), ((int)(((byte)(36))))));
                rango.Font.Size = 16;
                rango.Font.Bold = true;
                //Colores Titulo2
                rango = hoja.Range["C2", "C2"];
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(70)))), ((int)(((byte)(47))))));
                rango.Font.Size = 12;
                rango.Font.Bold = true;
                //Colores Titulo3
                rango = hoja.Range["C3", "C3"];
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(70)))), ((int)(((byte)(47))))));
                rango.Font.Size = 12;
                rango.Font.Bold = true;

                //Modificamos los anchos de las columnas
                int cont = 1;
                foreach (int widh in widths)
                {
                    rango = hoja.Columns[cont];
                    rango.ColumnWidth = widh;
                    cont++;
                }
                return columnaLetra++;
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public int generarExcel(SaveFileDialog fichero,int distrito, bool completo = false)
        {
            try
            {
                
                Excel.Application excel = new Excel.Application();
                Excel._Workbook libro = null;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                libro = (Excel._Workbook)excel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);

                if (completo)
                {
                    List<sice_distritos_locales> distritos = this.ListaDistritos();
                    foreach (sice_distritos_locales ds in distritos.OrderByDescending(x => x.id))
                    {
                        Console.WriteLine("Insetando Libro: " + ds.distrito);
                        this.generaHoja(ds.id, libro);
                    }
                }
                else
                {
                    this.generaHoja(distrito, libro);
                }

                ((Excel.Worksheet)excel.ActiveWorkbook.Sheets["Hoja1"]).Delete();   //Borramos la hoja que crea en el libro por defecto


                libro.Saved = true;
                //libro.SaveAs(Environment.CurrentDirectory + @"\Ejemplo2.xlsx");  // Si es un libro nuevo
                //libro.SaveAs(fichero.FileName,
                //Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                object misValue = System.Reflection.Missing.Value;
                libro.SaveAs(fichero.FileName, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue,
                misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                Excel.XlSaveConflictResolution.xlUserResolution, true,
                misValue, misValue, misValue);

                libro.Close(true,misValue,misValue);

                excel.UserControl = false;
                excel.Quit();

                Marshal.ReleaseComObject(libro);
                //Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(excel);
                return 1;
                   

                
            }
            catch (Exception E)
            {
                return 0;
            }
        }

        public void generaHoja(int distrito, Excel._Workbook libro)
        {
            try
            {
                Excel._Worksheet hoja = null;
                Excel.Range rango = null;
                int filaInicialTabla = 7;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                hoja = (Excel._Worksheet)libro.Worksheets.Add();
                hoja.Name = "DISTRITO " + distrito;  //Aqui debe ir el nombre del distrito
                List<VotosSeccion> vSeccion = this.ResultadosSeccionCaptura(0, 0, (int)distrito);
                List<Candidatos> candidatos = this.ListaCandidatos((int)distrito);
                int tempC = candidatos.Count;

                //Montamos las cabeceras 
                char letraFinal = CrearEncabezados(filaInicialTabla, ref hoja, vSeccion, candidatos, 1);


                //Agregar Datos
                int fila = filaInicialTabla + 1;
                int idCasillaActual = 0;
                int cont = 1;
                int contCand = 6;
                //row.Cells[0].Value = 1;
                //dgvResultados.Rows.Add(row);
                List<int> vLst = new List<int>();
                int Noregynulo = 0;
                int Lnominal = 0;

                //return;
                foreach (VotosSeccion v in vSeccion)
                {
                    //idCasillaActual = (int)v.id_casilla;
                    //Agregar Columnas

                    if ((idCasillaActual != (int)v.id_casilla && idCasillaActual > 0) || cont == vSeccion.Count)
                    {
                        //Agregar Ultima columna
                        if (cont == vSeccion.Count)
                        {
                            //Agregar Columnas
                            hoja.Cells[fila, 1] = v.id_casilla;
                            hoja.Cells[fila, 2] = v.seccion;
                            hoja.Cells[fila, 3] = v.casilla;
                            hoja.Cells[fila, 4] = (v.estatus != null) ? (v.estatus == "ATENDIDO") ? "CAPTURADA": v.estatus : "NO CAPTURADA";

                            hoja.Cells[fila, contCand].Value = v.votos;
                            vLst.Add((int)v.votos);
                            contCand++;
                        }

                        //Diferencia entre el primero y segundo
                        vLst.Sort();
                        int Primero = vLst[vLst.Count - 1];
                        int Seegundo = vLst[vLst.Count - 2];
                        int totalVotacionEmitida = vLst.Sum() + Noregynulo;
                        decimal diferencia = 0;
                        if (totalVotacionEmitida > 0)
                        {
                            int diferenciaTotal = Primero - Seegundo;
                            diferencia = Math.Round((Convert.ToDecimal(diferenciaTotal) * 100) / totalVotacionEmitida, 2);
                        }
                        hoja.Cells[fila, 5] = diferencia + "%";

                        //Votacion Emitida
                        hoja.Cells[fila, contCand] = totalVotacionEmitida;

                        //Lista Nominal
                        hoja.Cells[fila, contCand + 1] = Lnominal;

                        //Porcentaje de Participacion
                        if (totalVotacionEmitida == 0)
                            hoja.Cells[fila, contCand + 2] = 0 + "%";
                        else
                            hoja.Cells[fila, contCand + 2] = Math.Round((Convert.ToDecimal(totalVotacionEmitida) * 100) / Lnominal, 2) + "%";

                        //Agregar fila
                        string x = "A" + (fila).ToString();
                        string y = letraFinal.ToString() + (fila).ToString();
                        rango = hoja.Range[x, y];
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        //Console.WriteLine("Ins")
                        fila++;
                        contCand = 6;
                        vLst = new List<int>();
                        Noregynulo = 0;
                        //Inrementar filla
                    }

                    //Agregar Columnas
                    hoja.Cells[fila, 1] = v.id_casilla;
                    hoja.Cells[fila, 2] = v.seccion;
                    hoja.Cells[fila, 3] = v.casilla;
                    hoja.Cells[fila, 4] = (v.estatus != null) ? (v.estatus == "ATENDIDO") ? "CAPTURADA" : v.estatus : "NO CAPTURADA";
                    Lnominal = v.lista_nominal + tempC * 2;

                    hoja.Cells[fila, contCand] = v.votos;
                    if (v.tipo == "VOTO")
                        vLst.Add((int)v.votos);
                    else
                        Noregynulo += (int)v.votos;

                    idCasillaActual = (int)v.id_casilla;
                    cont++;
                    contCand++;

                    //if(cont == vSeccion.Count){
                    //    dgvResultados.Rows.Add(row);
                    //}


                }
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        private char CrearEncabezados(int fila, ref Excel._Worksheet hoja, List<VotosSeccion> vSeccion, List<Candidatos> candidatos, int columnaInicial = 1)
        {
            try
            {
                Excel.Range rango;
                Excel.Range rangoTitutlo;
                float Left = 0;
                float Top = 0;
                const float ImageSize = 42; //Tamaño Imagen Partidos
                string rutaImagen = System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\";

                //** Montamos el título en la línea 1 **
                hoja.Cells[1, 3] = "SISTEMA DE REGISTRO DE ACTAS DEL PROCESO ELECTORAL LÓCAL 2017-2018";
                hoja.Cells[2, 3] = "RESULTADOS ELECTORALES POR PARTIDOS POLÍTICOS O CANDIDATURA INDEPENDIENTE";
                hoja.Cells[3, 3] = "ELECCIÓN DE DIPUTADOS DE MAYORÍA RELATIVA POR CASILLA, SECCIÓN Y DISTRITO LOCAL";
                char columnaLetra = 'A';
                hoja.Shapes.AddPicture(rutaImagen+"iepc.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 125, 45);
                //hoja.Shapes.

                List<double> widths = new List<double>();

                //Agregar encabezados
                hoja.Cells[fila-3, columnaInicial] = "DISTRITO 1 CABECERA VICTORIA DE DURANGO";
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial+3]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                hoja.Cells[fila, columnaInicial] = "No."; columnaInicial++; columnaLetra++; widths.Add(8.57);
                hoja.Cells[fila, columnaInicial] = "Sección"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "Casilla"; columnaInicial++; columnaLetra++; widths.Add(25.29);
                hoja.Cells[fila, columnaInicial] = "Estatus"; columnaInicial++; columnaLetra++; widths.Add(12.29);

                hoja.Cells[fila, columnaInicial] = "Diferencia entre 1° y 2° Lugar"; columnaInicial++; columnaLetra++; widths.Add(12.29);
                hoja.Cells[fila, columnaInicial - 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila, columnaInicial - 1].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila, columnaInicial-1], hoja.Cells[fila - 3, columnaInicial-1]].Merge();
                hoja.Cells[fila, columnaInicial-1].WrapText = true;
                hoja.Cells[fila, columnaInicial-1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                


                //Agregar Columnas Caniddatos y Partidos
                foreach (Candidatos c in candidatos)
                {
                    //Agregar Imagen del Partido
                    rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila-3, columnaInicial];
                    hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                    Left = 3 + (float)((double)rango.Left);
                    Top = (float)((double)rango.Top);
                    
                    hoja.Shapes.AddPicture(rutaImagen + "pri.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                    hoja.Cells[fila, columnaInicial] = c.partido; columnaInicial++; columnaLetra++; widths.Add(8.57);
                }
                //Agregar columnas adicionales

                //Imagen no registrados
                rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                hoja.Range[hoja.Cells[fila-3, columnaInicial], hoja.Cells[fila-1, columnaInicial]].Merge();
                Left = 3 + (float)((double)rango.Left);
                Top = (float)((double)rango.Top);

                hoja.Shapes.AddPicture(rutaImagen + "no-regis.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                hoja.Cells[fila, columnaInicial] = "NOREG"; columnaInicial++; columnaLetra++; widths.Add(8.57);

                //Imagen Nulos
                rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                Left = 3 + (float)((double)rango.Left);
                Top = (float)((double)rango.Top);
                hoja.Shapes.AddPicture(rutaImagen + "nulos.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                hoja.Cells[fila, columnaInicial] = "NULOS"; columnaInicial++; columnaLetra++; widths.Add(8.57);

                hoja.Cells[fila-3, columnaInicial] = "Votación Total Emitida";
                hoja.Cells[fila - 3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila - 3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                hoja.Cells[fila, columnaInicial] = "TOTAL"; columnaInicial++; columnaLetra++; widths.Add(8.57);

                hoja.Cells[fila, columnaInicial] = "L. Nominal"; columnaInicial++; columnaLetra++; widths.Add(10);
                hoja.Cells[fila, columnaInicial] = "%"; widths.Add(10);                
                hoja.Cells[fila - 3, columnaInicial] = "Lista Nominal y Porcentaje de Participación Ciudadana";
                hoja.Cells[fila-3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila-3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial-1], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                //Colores de Fondo
                rango = hoja.Range["A"+fila, "D"+fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                //Colores de Fondo Partido
                rango = hoja.Range["F" + fila, columnaLetra.ToString() + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(38)))), ((int)(((byte)(36))))));
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                //Ponemos borde a las celdas
                string letra = columnaLetra.ToString() + fila;
                rango = hoja.Range["A" + (fila-3), letra];
                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                //Centramos los textos
                rango = hoja.Rows[fila];
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                //Colores titulo1
                rango = hoja.Range["C1", "C1"];
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(38)))), ((int)(((byte)(36))))));
                rango.Font.Size = 16;
                rango.Font.Bold = true;
                //Colores Titulo2
                rango = hoja.Range["C2", "C2"];
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(70)))), ((int)(((byte)(47))))));
                rango.Font.Size = 12;
                rango.Font.Bold = true;
                //Colores Titulo3
                rango = hoja.Range["C3", "C3"];
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(70)))), ((int)(((byte)(47))))));
                rango.Font.Size = 12;
                rango.Font.Bold = true;

                //Modificamos los anchos de las columnas
                int cont = 1;
                foreach (int widh in widths)
                {
                    rango = hoja.Columns[cont];
                    rango.ColumnWidth = widh;
                    cont++;
                }
                return columnaLetra++;
            }
            catch (Exception E)
            {
                throw E;
            }
        }
    }

    public class CasillasRecuento
    {
        public int id_casilla { get; set; }
        public int seccion { get; set; }
        public string casilla { get; set; }
        public string supuesto { get; set; }
    }
}
