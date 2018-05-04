﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema.DataModel;
using System.Transactions;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

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
                            "RES.tipo_reserva as estatus, " +
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

        public List<CasillasRecuento> ListaCasillasRecuentos(int distrito)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string consulta =
                        "SELECT " +
                            "C.id as id_casilla, " +
                            " C.seccion, " +
                            "C.tipo_casilla as casilla, " +
                            "S.supuesto " +
                        "FROM sice_ar_reserva R " +
                        "JOIN sice_casillas C ON C.id = R.id_casilla AND C.id_distrito_local = " + distrito + " " +
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
                            tmpDoc.id_estatus_acta = null;
                            tmpDoc.id_estatus_paquete = null;
                            tmpDoc.id_incidencias = null;
                            tmpDoc.casilla_instalada = null;
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
                        bool ceros = false;
                        if(supuesto != 0 || estatus_acta == 6 || estatus_acta == 7 || estatus_acta == 9 || estatus_acta ==11)
                        {
                            ceros = true;
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

        public int generarExcelRecuento(SaveFileDialog fichero, int distrito, bool completo = false)
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
                        this.generaHojaRecuento(ds.id, libro);
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

                libro.Close(true);

                excel.UserControl = false;
                excel.Quit();
                return 1;



            }
            catch (Exception E)
            {
                return 0;
            }
        }

        public void generaHojaRecuento(int distrito, Excel._Workbook libro)
        {
            try
            {
                Excel._Worksheet hoja = null;
                Excel.Range rango = null;
                int filaInicialTabla = 7;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                hoja = (Excel._Worksheet)libro.Worksheets.Add();
                hoja.Name = "DISTRITO " + distrito;  //Aqui debe ir el nombre del distrito

                //Montamos las cabeceras 
                char letraFinal = CrearEncabezadosRecuento(filaInicialTabla, ref hoja, 1);

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

                List<CasillasRecuento> listaRecuento = this.ListaCasillasRecuentos(distrito);
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

        private char CrearEncabezadosRecuento(int fila, ref Excel._Worksheet hoja,int columnaInicial = 1)
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
                hoja.Cells[2, 3] = "ELECCIÓN DE DIPUTADOS DE MAYORÍA RELATIVA POR CASILLA, SECCIÓN Y DISTRITO LOCAL";
                hoja.Cells[3, 3] = "LISTA DE CASILLAS A RECUENTO";
                char columnaLetra = 'A';
                hoja.Shapes.AddPicture(rutaImagen + "iepc.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 125, 45);
                //hoja.Shapes.

                List<double> widths = new List<double>();

                //Agregar encabezados
                hoja.Cells[fila - 3, columnaInicial] = "DISTRITO 1 CABECERA VICTORIA DE DURANGO";
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


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
                rango = hoja.Range["A" + (fila - 3), letra];
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

                libro.Close(true);

                excel.UserControl = false;
                excel.Quit();
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
                            decimal Porcentaje1 = Math.Round((Convert.ToDecimal(Primero) * 100) / totalVotacionEmitida, 2);
                            decimal Porcentaje2 = Math.Round((Convert.ToDecimal(Seegundo) * 100) / totalVotacionEmitida, 2);
                            diferencia = Porcentaje1 - Porcentaje2;
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
