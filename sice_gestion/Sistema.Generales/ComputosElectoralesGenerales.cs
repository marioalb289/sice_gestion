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
using OfficeOpenXml;
using System.IO;
using System.Globalization;

namespace Sistema.Generales
{
    public class ComputosElectoralesGenerales
    {
        private string con = "MYSQLOCAL";

        public ComputosElectoralesGenerales()
        {
            if (LoginInfo.privilegios == 5 || LoginInfo.privilegios == 6 || LoginInfo.privilegios == 7)
            {
                con = "MYSQLOCAL";

            }
            else if (LoginInfo.privilegios == 4)
            {
                con = "MYSQLSERVER";
            }
        }

        public void InicializarComputos()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        List<sice_distritos_locales> ds = ListaDistritos();
                        foreach(sice_distritos_locales distrito in ds)
                        {
                            sice_configuracion_recuento conf = (from p in contexto.sice_configuracion_recuento where p.id_distrito == distrito.id && p.sistema == "RA" && p.inicializado == 0 select p).FirstOrDefault();
                            if(conf!= null)
                            {
                                if (conf.tipo_recuento == "PARCIAL")
                                {
                                    //Listar Casilla a recuento
                                    List<sice_ar_reserva> listaCasillas = (from r in contexto.sice_ar_reserva join c in contexto.sice_casillas on r.id_casilla equals c.id where (r.id_estatus_acta == 3 || r.id_estatus_acta == 5 || r.id_estatus_acta == 8) && c.id_distrito_local == distrito.id  select r).ToList();
                                    if (listaCasillas.Count > 0)
                                    {
                                        foreach (sice_ar_reserva casilla in listaCasillas)
                                        {
                                            sice_reserva_captura new_casilla = (from p in contexto.sice_reserva_captura where p.id_casilla == casilla.id select p).FirstOrDefault();
                                            if (new_casilla == null)
                                            {
                                                new_casilla = new sice_reserva_captura();
                                                new_casilla.id_casilla = casilla.id_casilla;
                                                new_casilla.tipo_reserva = "RECUENTO";
                                                new_casilla.id_supuesto = casilla.id_supuesto;
                                                new_casilla.id_estatus_acta = casilla.id_estatus_acta;
                                                new_casilla.id_estatus_paquete = casilla.id_estatus_paquete;
                                                new_casilla.id_incidencias = casilla.id_incidencias;
                                                new_casilla.boletas_sobrantes = 0;
                                                new_casilla.personas_votaron = 0;
                                                new_casilla.num_representantes_votaron = 0;
                                                new_casilla.inicializada = 1;
                                                new_casilla.votos_sacados = 0;
                                                new_casilla.num_escritos = 0;
                                                new_casilla.importado = 0;
                                                new_casilla.grupo_trabajo = casilla.grupo_trabajo;
                                                new_casilla.create_at = DateTime.Now;
                                                new_casilla.updated_at = DateTime.Now;
                                                new_casilla.tipo_votacion = casilla.tipo_votacion;
                                                contexto.sice_reserva_captura.Add(new_casilla);
                                                contexto.SaveChanges();

                                                //casilla.inicializada = 1;
                                                contexto.SaveChanges();
                                            }
                                            else
                                            {
                                                new_casilla.id_casilla = casilla.id_casilla;
                                                new_casilla.tipo_reserva = "RECUENTO";
                                                new_casilla.id_supuesto = casilla.id_supuesto;
                                                new_casilla.id_estatus_acta = casilla.id_estatus_acta;
                                                new_casilla.id_estatus_paquete = casilla.id_estatus_paquete;
                                                new_casilla.id_incidencias = casilla.id_incidencias;
                                                new_casilla.boletas_sobrantes = 0;
                                                new_casilla.personas_votaron = 0;
                                                new_casilla.num_representantes_votaron = 0;
                                                new_casilla.inicializada = 1;
                                                new_casilla.votos_sacados = 0;
                                                new_casilla.num_escritos = 0;
                                                new_casilla.importado = 0;
                                                new_casilla.grupo_trabajo = casilla.grupo_trabajo;
                                                new_casilla.create_at = DateTime.Now;
                                                new_casilla.updated_at = DateTime.Now;
                                                new_casilla.tipo_votacion = casilla.tipo_votacion;
                                                contexto.SaveChanges();
                                            }

                                        }
                                        
                                    }

                                }
                                else
                                {
                                   
                                    List<CasillasRecuento> listaCasillas = this.ListaCasillasRecuentosRA(contexto,distrito.id, true);


                                    decimal cGt = Math.Round(Convert.ToDecimal(listaCasillas.Count) / Convert.ToDecimal(conf.grupos_trabajo), 0);
                                    int limitador_parcial = Convert.ToInt32(cGt);
                                    int limitador_total = limitador_parcial * ((int)conf.grupos_trabajo - 1);
                                    int contador_principal = 1;
                                    int contador_casilla = 1;
                                    int contador_grupo = 1;
                                    foreach (CasillasRecuento casilla in listaCasillas)
                                    {
                                        int grupo_asignado = contador_grupo;
                                        listaCasillas[contador_principal - 1].grupo_trabajo = grupo_asignado;
                                        contador_casilla++;
                                        contador_principal++;
                                        if (contador_casilla > limitador_parcial)
                                        {
                                            if (contador_principal <= limitador_total)
                                            {
                                                contador_casilla = 1;
                                                contador_grupo++;
                                            }
                                            else
                                            {
                                                contador_casilla = 1;
                                                contador_grupo = (int)conf.grupos_trabajo;
                                            }
                                        }
                                    }

                                    foreach (CasillasRecuento casilla in listaCasillas)
                                    {
                                        sice_reserva_captura new_casilla = (from p in contexto.sice_reserva_captura where p.id_casilla == casilla.id_casilla select p).FirstOrDefault();
                                        if (new_casilla == null)
                                        {
                                            new_casilla = new sice_reserva_captura();
                                            new_casilla.id_casilla = casilla.id_casilla;
                                            new_casilla.tipo_reserva = "RECUENTO";
                                            new_casilla.id_supuesto = null;
                                            new_casilla.id_estatus_acta = 5;
                                            new_casilla.id_estatus_paquete = 0;
                                            new_casilla.id_incidencias = 0;
                                            new_casilla.boletas_sobrantes = 0;
                                            new_casilla.personas_votaron = 0;
                                            new_casilla.num_representantes_votaron = 0;
                                            new_casilla.inicializada = 1;
                                            new_casilla.votos_sacados = 0;
                                            new_casilla.num_escritos = 0;
                                            new_casilla.importado = 0;
                                            new_casilla.grupo_trabajo = casilla.grupo_trabajo;
                                            new_casilla.create_at = DateTime.Now;
                                            new_casilla.updated_at = DateTime.Now;
                                            new_casilla.tipo_votacion = "MR";
                                            contexto.sice_reserva_captura.Add(new_casilla);
                                            contexto.SaveChanges();
                                        }
                                        else
                                        {
                                            new_casilla.id_casilla = casilla.id_casilla;
                                            new_casilla.tipo_reserva = "RECUENTO";
                                            new_casilla.id_supuesto = null;
                                            new_casilla.id_estatus_acta = 5;
                                            new_casilla.id_estatus_paquete = 0;
                                            new_casilla.id_incidencias = 0;
                                            new_casilla.boletas_sobrantes = 0;
                                            new_casilla.personas_votaron = 0;
                                            new_casilla.num_representantes_votaron = 0;
                                            new_casilla.inicializada = 1;
                                            new_casilla.votos_sacados = 0;
                                            new_casilla.num_escritos = 0;
                                            new_casilla.importado = 0;
                                            new_casilla.grupo_trabajo = casilla.grupo_trabajo;
                                            new_casilla.create_at = DateTime.Now;
                                            new_casilla.updated_at = DateTime.Now;
                                            new_casilla.tipo_votacion = "MR";
                                            contexto.SaveChanges();
                                        }
                                    }
                                }

                                conf.inicializado = 1;
                                contexto.SaveChanges();
                                
                            }
                        }
                        TransactionContexto.Complete();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<CasillasRecuento> ListaCasillasRecuentosRA(DatabaseContext contexto, int distrito, bool completo = false)
        {
            try
            {
                using (contexto = new DatabaseContext(con))
                {
                    string join = "";
                    string consulta = "";
                    if (completo)
                    {
                        consulta =
                           "SELECT " +
                               "C.id as id_casilla, " +
                               "C.id_distrito_local, " +
                               "C.seccion, " +
                               "C.tipo_casilla as casilla " +
                           "FROM sice_casillas C " +
                           "WHERE C.id_distrito_local = " + distrito + " AND C.tipo_votacion = 'MR' " +
                           "ORDER BY C.id_distrito_local ASC,C.seccion,C.id ASC";
                    }
                    else
                    {
                        join = "JOIN sice_casillas C ON C.id = R.id_casilla AND C.id_distrito_local = " + distrito + " AND C.tipo_votacion = 'MR' ";
                        consulta =
                           "SELECT " +
                               "C.id as id_casilla, " +
                               "C.id_distrito_local, " +
                               "C.seccion, " +
                               "C.tipo_casilla as casilla, " +
                               "R.grupo_trabajo as grupo_trabajo , " +
                               "S.supuesto " +
                           "FROM sice_ar_reserva R " +
                           join +
                           "JOIN sice_ar_supuestos S ON S.id = R.id_supuesto " +
                           "WHERE R.id_supuesto IS NOT NULL " +
                           "ORDER BY C.id_distrito_local ASC,C.seccion,C.id ASC";
                    }

                    return contexto.Database.SqlQuery<CasillasRecuento>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public sice_configuracion_recuento Configuracion_Recuento(string sistema, int id_distrito)
        {
            using (DatabaseContext contexto = new DatabaseContext(con))
            {
                return (from p in contexto.sice_configuracion_recuento where p.sistema == sistema && p.id_distrito == id_distrito select p).FirstOrDefault();
            }
        }

        public sice_reserva_captura DetallesActa(int id_casilla, string tipo_votacion)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from r in contexto.sice_reserva_captura where r.id_casilla == id_casilla && r.tipo_votacion == tipo_votacion select r).FirstOrDefault();
                }
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public sice_ar_reserva DetallesActaRA(int id_casilla, string tipo_votacion)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from r in contexto.sice_ar_reserva where r.id_casilla == id_casilla && r.tipo_votacion == tipo_votacion select r).FirstOrDefault();
                }
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public List<sice_estado_acta> ListaEstatusActa(string tipo = "")
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    if (tipo == "")
                        return (from p in contexto.sice_estado_acta where p.id != 4 select p).ToList();
                    else if (tipo == "RECUENTO")
                        return (from p in contexto.sice_estado_acta select p).ToList();
                    else
                        return (from p in contexto.sice_estado_acta where p.id != 3 && p.id != 5 select p).ToList();
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

        public List<sice_partidos_politicos> ListaPartidosPoliticos()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_partidos_politicos where p.tipo == "PARTIDO" select p).OrderBy(x => x.prelacion).ToList();
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
                    string condicion = " ";
                    if (LoginInfo.privilegios == 5 || LoginInfo.privilegios == 6)
                    {
                        condicion = "WHERE C.id_cabecera_local = " + LoginInfo.id_municipio + " ";
                    }

                    string consulta =
                        "SELECT D.* FROM sice_casillas C " +
                        "JOIN sice_distritos_locales D on D.id = C.id_distrito_local " +
                        condicion +
                        "GROUP BY C.id_distrito_local ";
                    List<sice_distritos_locales> lsCasilla = contexto.Database.SqlQuery<sice_distritos_locales>(consulta).ToList();
                    return lsCasilla;
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public List<sice_ar_supuestos> ListaSupuestos()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_ar_supuestos where p.SICE == 1 select p).ToList();
                }

            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public int verificarCasillaRegistrada(int id_casilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    sice_reserva_captura reserva = (from r in contexto.sice_reserva_captura where r.id_casilla == id_casilla select r).FirstOrDefault();
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

        public int verificarRecuento(int id_casilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    //Estatus 3 5 y 8 son para recuento
                    sice_ar_reserva res = (from r in contexto.sice_ar_reserva where r.id_casilla == id_casilla && (r.id_estatus_acta == 3 || r.id_estatus_acta == 5 || r.id_estatus_acta == 8) select r).FirstOrDefault();
                    sice_reserva_captura res2 = (from r in contexto.sice_reserva_captura where r.id_casilla == id_casilla && (r.id_estatus_acta == 3 || r.id_estatus_acta == 5 || r.id_estatus_acta == 8) select r).FirstOrDefault();
                    if (res != null || res2 != null)
                        return 1;
                    else
                        return 0;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
        }
        public int verificarReservaConsejo(int id_casilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    //Buscar Solo actas Reservadas para Consejo
                    sice_reserva_captura res = (from r in contexto.sice_reserva_captura where r.id_casilla == id_casilla && r.id_estatus_acta == 4 select r).FirstOrDefault();
                    if (res != null)
                        return 1;
                    else
                        return 0;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public double porcentajeRP(int id_partido, int porcentaje)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_configuracion_rp where p.id_partido == id_partido && p.porcentaje == porcentaje select (double)p.valor).FirstOrDefault();
                }
            }
            catch(Exception E)
            {
                throw E;
            }
        }


        public List<CasillasRecuento> ListaCasillasRecuentos(int distrito, bool completo = false,bool reporte = false)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string join = "";
                    string consulta = "";
                    string condicion2 = "";
                    if (completo)
                    {
                        if (reporte)
                            join = "JOIN sice_reserva_captura R ON R.id_casilla = C.id AND R.tipo_reserva = 'RECUENTO' AND (R.inicializada <> 1) ";
                        else
                            join = "JOIN sice_reserva_captura R ON R.id_casilla = C.id AND ((R.tipo_reserva = 'CAPTURADA' AND R.inicializada = 0) OR (R.tipo_reserva = 'RECUENTO' && R.grupo_trabajo IS NULL)) ";
                        consulta =
                           "SELECT " +
                               "C.id as id_casilla, " +
                               "C.id_distrito_local, " +
                               "C.seccion, " +
                               "C.tipo_casilla as casilla " +
                           "FROM sice_casillas C " +
                           join +
                           "WHERE C.id_distrito_local = " + distrito + " AND C.tipo_votacion = 'MR'" +
                           "ORDER BY C.id_distrito_local ASC,C.seccion,C.id ASC";
                    }
                    else
                    {
                        if (!reporte)
                            condicion2 = " AND R.grupo_trabajo IS NULL ";
                        join = "JOIN sice_casillas C ON C.id = R.id_casilla AND C.id_distrito_local = " + distrito + " ";
                        consulta =
                           "SELECT " +
                               "C.id as id_casilla, " +
                               "C.id_distrito_local, " +
                               "C.seccion, " +
                               "C.tipo_casilla as casilla, " +
                               "R.grupo_trabajo as grupo_trabajo , " +
                               "S.supuesto " +
                           "FROM sice_reserva_captura R " +
                           join +
                           "JOIN sice_ar_supuestos S ON S.id = R.id_supuesto " +
                           "WHERE R.id_estatus_acta = 5 AND R.inicializada = 2  AND C.tipo_votacion = 'MR' " +condicion2+
                           "ORDER BY C.id_distrito_local ASC,C.seccion,C.id ASC";
                    }

                    return contexto.Database.SqlQuery<CasillasRecuento>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
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
                        "P.local as partido_local, " +
                        "P.info_creado as coalicion, " +
                        "P.img_par as imagen, " +
                        "P.tipo as tipo_partido " +
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

        public List<PartidosVotosRP> ListaResultadosCasillaRP(int casilla, string tabla = "")
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    if (tabla == "")
                        tabla = "sice_votos_rp";
                    string consulta =
                        "SELECT " +
                        "V.id, " +
                        "V.id_casilla AS id_casilla, " +
                        "V.tipo AS tipo, " +
                        "V.votos AS votos, " +
                        "CASE WHEN V.tipo = 'VOTO' THEN V.id_partido WHEN V.tipo = 'NULO' THEN - 2 WHEN V.tipo = 'NO REGISTRADO' THEN - 1 END AS id_partido, " +
                        "CASE WHEN V.tipo = 'VOTO' THEN P.prelacion WHEN V.tipo = 'NULO' THEN  200 WHEN V.tipo = 'NO REGISTRADO' THEN 100 END AS prelacion, " +
                        "P.siglas_par AS partido, " +
                        "P.LOCAL AS partido_local, " +
                        "P.info_creado AS coalicion, " +
                        "P.img_par AS imagen " +
                        "FROM " + tabla + " V " +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = V.id_partido " +
                         "WHERE V.id_casilla = " + casilla + " " + " AND V.tipo <> 'NO VALIDO' " +
                        "ORDER BY prelacion ASC ";
                    return contexto.Database.SqlQuery<PartidosVotosRP>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }


        public List<VotosSeccion> ResultadosSeccion(int pageNumber =0, int pageSize=0,int id_distrito_local = 0)
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
                            "CONCAT(CND.nombre, ' ', CND.apellido_paterno, ' ', CND.apellido_materno) as candidato," +
                            "P.siglas_par as partido," +
                            "P.img_par as imagen," +
                            "C.id_distrito_local as distrito_local," +
                            "M.municipio," +
                            "M2.municipio AS cabecera_local, " +
                            "RC.tipo_reserva as estatus, " +
                            "RC.grupo_trabajo, " +
                            "EA.estatus AS estatus_acta, " +
                            "EA.id AS id_estatus_acta "+
                        "FROM sice_votos RV " +
                        "LEFT JOIN sice_reserva_captura RC ON RC.id_casilla = RV.id_casilla " +
                        "LEFT JOIN sice_estado_acta EA ON RC.id_estatus_acta = EA.id " +
                        "LEFT JOIN sice_candidatos CND ON CND.id = RV.id_candidato " +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = CND.fk_partido " +
                        "JOIN sice_casillas C ON C.id = RV.id_casilla " + condicion +
                        "JOIN sice_municipios M ON M.id = C.id_municipio " +
                        "JOIN sice_municipios M2 ON M2.id = C.id_cabecera_local " +
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
        public List<VotosSeccion> ResultadosSeccionRP(int pageNumber = 0, int pageSize = 0, int id_distrito_local = 0)
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
                            "C.seccion, " +
                            "V.id_casilla, " +
                            "C.tipo_casilla AS casilla, " +
                            "C.lista_nominal, " +
                            "CASE WHEN V.tipo = 'VOTO' THEN V.id_partido WHEN V.tipo = 'NULO' THEN - 2 WHEN V.tipo = 'NO REGISTRADO' THEN - 1 END AS id_partido, " +
                            "CASE WHEN V.tipo = 'VOTO' THEN P.prelacion WHEN V.tipo = 'NULO' THEN   200 WHEN V.tipo = 'NO REGISTRADO' THEN 100 END AS prelacion, " +
                             "V.votos, " +
                             "V.tipo, " +
                             "P.siglas_par AS partido, " +
                             "P.img_par AS imagen, " +
                             "C.id_distrito_local AS distrito_local, " +
                             "M.municipio, " +
                             "M2.municipio AS cabecera_local, " +
                             "RC.tipo_reserva AS estatus, " +
                             "RC.grupo_trabajo, " +
                             "EA.estatus AS estatus_acta, " +
                             "EA.id AS id_estatus_acta " +
                        "FROM sice_votos_rp V " +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = V.id_partido " +
                        "LEFT JOIN sice_reserva_captura RC ON RC.id_casilla = V.id_casilla " +
                        "LEFT JOIN sice_estado_acta EA ON RC.id_estatus_acta = EA.id " +
                        "JOIN sice_casillas C ON C.id = V.id_casilla " + condicion +
                        "JOIN sice_municipios M ON M.id = C.id_municipio " +
                        "JOIN sice_municipios M2 ON M2.id = C.id_cabecera_local " +
                        "ORDER BY C.seccion ASC, V.id_casilla ASC, prelacion ASC " +
                        limit;
                        
                    return contexto.Database.SqlQuery<VotosSeccion>(consulta).ToList();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<sice_votos> ResultadosSiceVotos()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string result = string.Join(",", LoginInfo.lista_distritos);
                    string condicion = " AND C.id_distrito_local IN (" + result + ") ";
                    string consulta =
                        "SELECT " +
                             "RV.* " +
                        "FROM sice_votos RV " +
                        "LEFT JOIN sice_reserva_captura RC ON RC.id_casilla = RV.id_casilla " +
                        "LEFT JOIN sice_candidatos CND ON CND.id = RV.id_candidato " +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = CND.fk_partido " +
                        "JOIN sice_casillas C ON C.id = RV.id_casilla " + condicion +
                        "JOIN sice_municipios M ON M.id = C.id_municipio " +
                        "JOIN sice_municipios M2 ON M2.id = C.id_cabecera_local " +
                        "ORDER BY C.seccion ASC, RV.id_casilla ASC, prelacion ASC ";

                    return contexto.Database.SqlQuery<sice_votos>(consulta).ToList();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public sice_ar_supuestos getSupuesto(int id_casilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_reserva_captura join sup in contexto.sice_ar_supuestos on p.id_supuesto equals sup.id where p.id_casilla == id_casilla select sup).FirstOrDefault();
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }

            }
            catch (Exception E)
            { throw E; }

        }

        public List<SeccionCasillaConsecutivo> ListaSescciones(bool capturada = false)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string consulta = "";
                    if (!capturada)
                    {
                        if(LoginInfo.privilegios == 7)
                        {
                            consulta =
                                "SELECT C.* FROM sice_casillas C ";
                        }
                        else
                        {
                            consulta =
                                "SELECT C.* FROM sice_casillas C " +
                                "WHERE C.id_cabecera_local = " + LoginInfo.id_municipio;
                        }
                        
                    }
                    else
                    {
                        consulta =
                       "SELECT C.*, " +
                       "CASE WHEN C.tipo_casilla = 'S1' THEN	100 WHEN C.tipo_casilla = 'S1-RP' THEN 200 WHEN C.tipo_casilla <> 'S1' THEN	1 END AS especial " +
                       "FROM sice_casillas C " +
                       "LEFT JOIN sice_reserva_captura RC ON RC.id_casilla = C.id " +
                       "WHERE RC.id IS NULL" + " AND C.id_cabecera_local = " + LoginInfo.id_municipio + " " +
                       "ORDER BY C.id_distrito_local ASC,especial ASC,C.seccion,C.id ASC";

                    }

                    
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
                                municipio = (int)p.id_municipio,
                                tipo = p.tipo_votacion
                            }).ToList();
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public List<sice_casillas> ListaCasillas(string tipo,int distrito)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_casillas where p.tipo_votacion == tipo && p.id_distrito_local == distrito select p).ToList();
                    
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public List<SeccionCasillaConsecutivo> ListaSesccionesReserva(bool ReservaConsejo)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string condicion = "";
                    string condicion2 = " AND (RC.grupo_trabajo IS NULL OR RC.grupo_trabajo = 0) ";
                    if (LoginInfo.grupo_trabajo > 0)
                        condicion2 = "AND RC.grupo_trabajo = " + LoginInfo.grupo_trabajo + " ";
                    if (ReservaConsejo)
                    {
                        condicion = "RC.tipo_reserva = 'RESERVA' ";
                        condicion2 = "";
                    }
                    else
                    {
                        condicion = "RC.tipo_reserva = 'RECUENTO' ";

                    }
                    string consulta =
                        "SELECT C.* FROM sice_casillas C " +
                        "JOIN sice_reserva_captura RC ON RC.id_casilla = C.id " +
                        "WHERE " + condicion + " AND C.id_cabecera_local = " + LoginInfo.id_municipio + " " +
                        condicion2 +
                        "ORDER BY C.id_distrito_local ASC,C.id";
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
                                municipio = (int)p.id_municipio,
                                tipo = p.tipo_votacion
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
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string consulta =
                        "SELECT " +
                        "C.id as id_candidato, " +
                        "CONCAT(C.nombre,' ',C.apellido_paterno,' ',C.apellido_materno)as candidato, " +
                        "CD.nombre_candidatura, " +
                        "P.siglas_par as partido, " +
                        "P.img_par as imagen, " +
                        "P.local as partido_local, " +
                        "P.info_creado as coalicion, " +
                        "P.tipo as tipo_partido " +
                        "FROM sice_candidatos C " +
                        "JOIN sice_candidaturas CD ON CD.id = C.fk_cargo AND CD.titular = 1 " + "AND CD.id_distrito =" + distrito + " " +
                        "JOIN sice_partidos_politicos P ON P.id = C.fk_partido " +
                        "ORDER BY P.prelacion ASC";
                    return contexto.Database.SqlQuery<Candidatos>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public List<CandidatosResultados> ListaResultadosCandidatos(int distrito, bool flagCondicon = false, bool flagReporte = false)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string condicion = "WHERE RC.id_estatus_acta = 1 OR RC.id_estatus_acta = 2 OR RC.id_estatus_acta = 8 " ;
                    if (!flagCondicon)
                        condicion = "";
                    if(flagReporte)
                        condicion = "WHERE (RC.id_estatus_acta = 1 OR RC.id_estatus_acta = 2 OR RC.id_estatus_acta = 8 )  AND RC.grupo_trabajo = "+LoginInfo.grupo_trabajo+ " ";
                    string consulta =
                        "SELECT " +
                        "CND.id AS id_candidato, " +
                        "CONCAT(CND.nombre, ' ', CND.apellido_paterno, ' ', CND.apellido_materno) AS candidato,CD.nombre_candidatura, " +
                        " P.id AS id_partido, " +
                        " P.siglas_par AS partido, " +
                        "P.LOCAL AS partido_local, " +
                        "P.img_par AS imagen, " +
                        "P.info_creado AS coalicion, " +
                        "SUM(RV.votos) as votos, " +
                        "RV.tipo, " +
                        "CASE WHEN RV.tipo = 'VOTO' THEN P.prelacion WHEN RV.tipo = 'NULO' THEN 200 WHEN RV.tipo = 'NO REGISTRADO' THEN  100 END AS prelacion " +
                        "FROM sice_votos RV " +
                        "LEFT JOIN sice_reserva_captura RC ON RC.id_casilla = RV.id_casilla "+
                        "LEFT JOIN sice_candidatos CND ON CND.id = RV.id_candidato " +
                        "LEFT JOIN sice_candidaturas CD ON CD.id = CND.fk_cargo " +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = CND.fk_partido " +
                        "JOIN sice_casillas C ON C.id = RV.id_casilla "  + "AND C.id_distrito_local =" + distrito + " " +
                        "JOIN sice_municipios M ON M.id = C.id_municipio " +
                        "JOIN sice_municipios M2 ON M2.id = C.id_cabecera_local " +
                        condicion +
                        "GROUP BY C.id_distrito_local,RV.id_candidato,RV.tipo " +
                        "ORDER BY prelacion ASC ";
                    return contexto.Database.SqlQuery<CandidatosResultados>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public List<PartidosVotosRP> ListaResultadosPartidos(int distrito, bool flagCondicon = false, bool flagReporte = false)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    string condicion = "WHERE RC.id_estatus_acta = 1 OR RC.id_estatus_acta = 2 OR RC.id_estatus_acta = 8 ";
                    if (!flagCondicon)
                        condicion = "";
                    if (flagReporte)
                        condicion = "WHERE (RC.id_estatus_acta = 1 OR RC.id_estatus_acta = 2 OR RC.id_estatus_acta = 8 )  AND RC.grupo_trabajo = " + LoginInfo.grupo_trabajo + " ";
                    string consulta =
                        "SELECT " +
                            "V.id, " +
                            "V.id_casilla AS id_casilla, " +
                            "V.tipo AS tipo," +
                            "SUM(V.votos) AS votos, " +
                            "CASE WHEN V.tipo = 'VOTO' THEN V.id_partido WHEN V.tipo = 'NULO' THEN - 2 WHEN V.tipo = 'NO REGISTRADO' THEN - 1 END AS id_partido, " +
                            "CASE WHEN V.tipo = 'VOTO' THEN P.prelacion WHEN V.tipo = 'NULO' THEN 200 WHEN V.tipo = 'NO REGISTRADO' THEN   100 END AS prelacion, " +
                            "P.siglas_par AS partido, " +
                            "P.LOCAL AS partido_local, " +
                            "P.info_creado AS coalicion, " +
                            "P.img_par AS imagen " +
                        "FROM sice_votos_rp V " +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = V.id_partido " +
                        "LEFT JOIN sice_reserva_captura RC ON RC.id_casilla = V.id_casilla " +
                        "LEFT JOIN sice_estado_acta EA ON RC.id_estatus_acta = EA.id " +
                        "JOIN sice_casillas C ON C.id = V.id_casilla " + "AND C.id_distrito_local =" + distrito + " " +
                        "JOIN sice_municipios M ON M.id = C.id_municipio " +
                        "JOIN sice_municipios M2 ON M2.id = C.id_cabecera_local " +
                        condicion +
                        "GROUP BY C.id_distrito_local,V.id_partido,V.tipo " +
                        "ORDER BY prelacion ASC ";
                    return contexto.Database.SqlQuery<PartidosVotosRP>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public int RepresentantesCComun(string coalicion)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    int representantes = 0;
                    string result = string.Join(",", coalicion);
                    string condicion = "WHERE id IN (" + result + ") ";
                    string consulta = "SELECT * FROM sice_partidos_politicos " + condicion;
                    List<sice_partidos_politicos> listaPartidos = contexto.Database.SqlQuery<sice_partidos_politicos>(consulta).ToList();
                    foreach (sice_partidos_politicos p in listaPartidos)
                    {
                        if (p.local == 1)
                            representantes += 1;
                        else
                            representantes += 2;
                    }

                    return representantes;
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public int CasillaReserva(int id_casilla,string motivo,int? supuesto = null)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    

                    sice_reserva_captura rc = (from p in contexto.sice_reserva_captura where p.id_casilla == id_casilla select p).FirstOrDefault();
                    if (rc != null)
                    {
                        rc.tipo_reserva = motivo;
                        rc.id_supuesto = supuesto;

                        rc.boletas_sobrantes = 0;
                        rc.personas_votaron = 0;
                        rc.num_representantes_votaron = 0;
                        rc.num_escritos = 0;
                        rc.votos_sacados = 0;
                    }
                    else
                    {
                        rc = new sice_reserva_captura();
                        rc.id_supuesto = supuesto;
                        rc.id_casilla = id_casilla;
                        rc.tipo_reserva = motivo;

                        rc.boletas_sobrantes = 0;
                        rc.personas_votaron = 0;
                        rc.num_representantes_votaron = 0;
                        rc.num_escritos = 0;
                        rc.votos_sacados = 0;

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

        


        public int guardarDatosVotos(List<sice_votos> listaVotos, int id_casilla, int supuesto, int boletasSobrantes, int numEscritos, int personas_votaron,
            int representantes, int votos_sacados, int incidencias, int estatus_acta, int estatus_paquete, int votos_reserva = 0, bool modificar = false)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        bool ceros = true;
                        bool recuento = false;
                        bool no_conta = false;
                        bool reserva = false;

                        int? ep = 0;//estatus paquete
                        int? cp = 0;//condiicones del paquete
                        int ce = 0; //Con etiqueta
                        int cc = 0; //con cinta

                        sice_ar_reserva detalleRA = DetallesActaRA(id_casilla, "MR");
                        if (detalleRA != null)
                        {
                            ep = detalleRA.id_estatus_paquete;
                            cp = detalleRA.id_condiciones_paquete;
                            ce = detalleRA.con_etiqueta;
                            cc = detalleRA.con_cinta;
                        }


                        if (estatus_acta == 1 || estatus_acta == 2 || estatus_acta == 8)
                        {
                            ceros = false;
                        }
                        if (estatus_acta == 3 || estatus_acta == 5 )
                        {
                            recuento = true;
                        }
                        if(estatus_acta == 4)
                        {
                            reserva = true;
                        }
                        if (estatus_acta == 9)
                        {
                            no_conta = true;
                        }
                        sice_votos v1 = null;
                        foreach (sice_votos voto in listaVotos)
                        {
                            if (voto.id_candidato != null)
                            {
                                v1 = (from d in contexto.sice_votos where d.id_candidato == voto.id_candidato && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    v1 = (from d in contexto.sice_votos where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    v1 = (from d in contexto.sice_votos where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }

                            if (v1 != null)
                            {
                                v1.id_candidato = voto.id_candidato;
                                v1.id_casilla = voto.id_casilla;
                                v1.tipo = voto.tipo;
                                v1.votos = ceros && !reserva ? 0 : voto.votos;
                                v1.importado = 0;
                                v1.estatus = 1;
                                contexto.SaveChanges();
                            }
                            else
                            {
                                return 0;
                            }
                        }

                        sice_reserva_captura rc = (from p in contexto.sice_reserva_captura where p.id_casilla == id_casilla select p).FirstOrDefault();
                        if (rc != null)
                        {
                            rc.tipo_reserva = recuento ? "RECUENTO" : no_conta ? "NO CONTABILIZABLE" : reserva ? "RESERVA": "CAPTURADA";
                            rc.num_escritos = ceros && !reserva ? 0 : numEscritos;
                            if (supuesto == 0)
                                rc.id_supuesto = null;
                            else
                                rc.id_supuesto = supuesto;
                            rc.boletas_sobrantes = ceros && !reserva ? 0 : boletasSobrantes;
                            rc.personas_votaron = ceros && !reserva ? 0 : personas_votaron;
                            rc.num_representantes_votaron = ceros && !reserva ? 0 : representantes;
                            rc.votos_sacados = ceros && !reserva ? 0 : votos_sacados;
                            rc.id_estatus_acta = estatus_acta;
                            rc.id_estatus_paquete = ep;
                            rc.id_condiciones_paquete = cp;
                            rc.con_cinta = cc;
                            rc.con_etiqueta = ce;
                            rc.votos_reservados = votos_reserva;
                            rc.tipo_votacion = "MR";
                            if (incidencias == 0)
                                rc.id_incidencias = null;
                            else
                                rc.id_incidencias = incidencias;
                            rc.importado = 0;
                            rc.updated_at = DateTime.Now;
                        }
                        else
                        {
                            rc = new sice_reserva_captura();
                            rc.id_casilla = id_casilla;
                            rc.tipo_reserva = recuento ? "RECUENTO" : no_conta ? "NO CONTABILIZABLE" : reserva ? "RESERVA" : "CAPTURADA";
                            rc.create_at = DateTime.Now;
                            rc.updated_at = DateTime.Now;
                            rc.num_escritos = ceros && !reserva ? 0 : numEscritos;
                            rc.importado = 0;
                            if (supuesto == 0)
                                rc.id_supuesto = null;
                            else
                                rc.id_supuesto = supuesto;
                            rc.boletas_sobrantes = ceros && !reserva ? 0 : boletasSobrantes;
                            rc.personas_votaron = ceros && !reserva ? 0 : personas_votaron;
                            rc.num_representantes_votaron = ceros && !reserva ? 0 : representantes;
                            rc.votos_sacados = ceros && !reserva ? 0 : votos_sacados;
                            rc.id_estatus_acta = estatus_acta;
                            rc.id_estatus_paquete = ep;
                            rc.id_condiciones_paquete = cp;
                            rc.con_cinta = cc;
                            rc.con_etiqueta = ce;
                            rc.inicializada = recuento ? 2 : 0;
                            rc.votos_reservados = votos_reserva;
                            rc.tipo_votacion = "MR";
                            if (incidencias == 0)
                                rc.id_incidencias = null;
                            else
                                rc.id_incidencias = incidencias;
                            contexto.sice_reserva_captura.Add(rc);
                        }
                        if (modificar)
                        {
                            sice_historico hs = new sice_historico();
                            hs.id_casilla = id_casilla;
                            if (supuesto == 0)
                                hs.id_supuesto = null;
                            else
                                hs.id_supuesto = supuesto;
                            hs.fecha = DateTime.Now;
                            hs.importado = 0;
                            contexto.sice_historico.Add(hs);
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

        public int guardarDatosVotosRP(List<sice_votos_rp> listaVotos, int id_casilla, int supuesto, int boletasSobrantes, int numEscritos, int personas_votaron,
            int representantes, int votos_sacados, int incidencias, int estatus_acta, int estatus_paquete,int votos_reserva = 0, bool modificar = false)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        bool ceros = true;
                        bool recuento = false;
                        bool no_conta = false;
                        bool reserva = false;

                        int? ep = 0;//estatus paquete
                        int? cp = 0;//condiicones del paquete
                        int ce = 0; //Con etiqueta
                        int cc = 0; //con cinta

                        sice_ar_reserva detalleRA = DetallesActaRA(id_casilla, "RP");
                        if (detalleRA != null)
                        {
                            ep = detalleRA.id_estatus_paquete;
                            cp = detalleRA.id_condiciones_paquete;
                            ce = detalleRA.con_etiqueta;
                            cc = detalleRA.con_cinta;
                        }

                        if (estatus_acta == 1 || estatus_acta == 2 || estatus_acta == 8)
                        {
                            ceros = false;
                        }
                        if (estatus_acta == 3 || estatus_acta == 5)
                        {
                            recuento = true;
                        }
                        if (estatus_acta == 4)
                        {
                            reserva = true;
                        }
                        if (estatus_acta == 9)
                        {
                            no_conta = true;
                        }
                        sice_votos_rp v1 = null;
                        foreach (sice_votos_rp voto in listaVotos)
                        {
                            if (voto.id_partido != null)
                            {
                                v1 = (from d in contexto.sice_votos_rp where d.id_partido == voto.id_partido && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    v1 = (from d in contexto.sice_votos_rp where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    v1 = (from d in contexto.sice_votos_rp where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }

                            if (v1 != null)
                            {
                                v1.id_partido = voto.id_partido;
                                v1.id_casilla = voto.id_casilla;
                                v1.tipo = voto.tipo;
                                v1.votos = ceros && !reserva ? 0 : voto.votos;
                                v1.importado = 0;
                                v1.estatus = 1;
                                contexto.SaveChanges();
                            }
                            else
                            {
                                return 0;
                            }
                        }

                        sice_reserva_captura rc = (from p in contexto.sice_reserva_captura where p.id_casilla == id_casilla select p).FirstOrDefault();
                        if (rc != null)
                        {
                            rc.tipo_reserva = recuento ? "RECUENTO" : no_conta ? "NO CONTABILIZABLE" : reserva ? "RESERVA" : "CAPTURADA";
                            rc.num_escritos = ceros ? 0 : numEscritos;
                            if (supuesto == 0)
                                rc.id_supuesto = null;
                            else
                                rc.id_supuesto = supuesto;
                            rc.boletas_sobrantes = ceros ? 0 : boletasSobrantes;
                            rc.personas_votaron = ceros ? 0 : personas_votaron;
                            rc.num_representantes_votaron = ceros ? 0 : representantes;
                            rc.votos_sacados = ceros ? 0 : votos_sacados;
                            rc.id_estatus_acta = estatus_acta;
                            rc.id_estatus_paquete = ep;
                            rc.id_condiciones_paquete = cp;
                            rc.con_cinta = cc;
                            rc.con_etiqueta = ce;
                            rc.votos_reservados = votos_reserva;
                            rc.tipo_votacion = "RP";
                            rc.inicializada = 0;
                            if (incidencias == 0)
                                rc.id_incidencias = null;
                            else
                                rc.id_incidencias = incidencias;
                            rc.importado = 0;
                            rc.updated_at = DateTime.Now;
                        }
                        else
                        {
                            rc = new sice_reserva_captura();
                            rc.id_casilla = id_casilla;
                            rc.tipo_reserva = recuento ? "RECUENTO" : no_conta ? "NO CONTABILIZABLE" : reserva ? "RESERVA" : "CAPTURADA";
                            rc.create_at = DateTime.Now;
                            rc.updated_at = DateTime.Now;
                            rc.num_escritos = ceros ? 0 : numEscritos;
                            rc.importado = 0;
                            if (supuesto == 0)
                                rc.id_supuesto = null;
                            else
                                rc.id_supuesto = supuesto;
                            rc.boletas_sobrantes = ceros ? 0 : boletasSobrantes;
                            rc.personas_votaron = ceros ? 0 : personas_votaron;
                            rc.num_representantes_votaron = ceros ? 0 : representantes;
                            rc.votos_sacados = ceros ? 0 : votos_sacados;
                            rc.id_estatus_acta = estatus_acta;
                            rc.id_estatus_paquete = ep;
                            rc.id_condiciones_paquete = cp;
                            rc.con_cinta = cc;
                            rc.con_etiqueta = ce;
                            rc.votos_reservados = votos_reserva;
                            rc.inicializada = 0;
                            rc.tipo_votacion = "RP";
                            if (incidencias == 0)
                                rc.id_incidencias = null;
                            else
                                rc.id_incidencias = incidencias;
                            contexto.sice_reserva_captura.Add(rc);
                        }
                        if (modificar)
                        {
                            sice_historico hs = new sice_historico();
                            hs.id_casilla = id_casilla;
                            if (supuesto == 0)
                                hs.id_supuesto = null;
                            else
                                hs.id_supuesto = supuesto;
                            hs.fecha = DateTime.Now;
                            hs.importado = 0;
                            contexto.sice_historico.Add(hs);
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

        public int GuardarConfiguracionRecuento(double horas, int id_distrito, int grupos_trabajo, int puntos_recuento, string tipo_recuento)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        int res = 0;

                        sice_configuracion_recuento conf = (from c in contexto.sice_configuracion_recuento where c.sistema == "SICE" && c.id_distrito == id_distrito select c).FirstOrDefault();
                        bool flag_reporte = false;
                        if (conf != null)
                        {
                            conf.grupos_trabajo = grupos_trabajo;
                            conf.horas_disponibles = horas;
                            conf.id_distrito = id_distrito;
                            conf.puntos_recuento = puntos_recuento;
                            conf.tipo_recuento = tipo_recuento;
                            conf.sistema = "SICE";
                            contexto.SaveChanges();
                            flag_reporte = (conf.tipo_recuento == "TOTAL" ) ? true : false;
                            res = 1;
                        }
                        else
                        {
                            sice_configuracion_recuento newConf = new sice_configuracion_recuento();
                            newConf.sistema = "SICE";
                            newConf.grupos_trabajo = grupos_trabajo;
                            newConf.horas_disponibles = horas;
                            newConf.id_distrito = id_distrito;
                            newConf.puntos_recuento = puntos_recuento;
                            newConf.tipo_recuento = tipo_recuento;
                            contexto.sice_configuracion_recuento.Add(newConf);
                            contexto.SaveChanges();
                            res = 1;
                        }

                        //Asignar los Grupos de Trabajo a los usuarios DE RECUENTO PARCIAL
                        //if (tipo_recuento == "PARCIAL")
                        //{
                            List<CasillasRecuento> lsRecuento = this.ListaCasillasRecuentos(id_distrito, tipo_recuento == "PARCIAL" ? false : true, (tipo_recuento == "TOTAL") ? false : true);

                            decimal cGt = Math.Round(Convert.ToDecimal(lsRecuento.Count) / Convert.ToDecimal(grupos_trabajo), 0);
                            int limitador_parcial = Convert.ToInt32(cGt);
                            int limitador_total = limitador_parcial * (grupos_trabajo - 1);
                            int contador_principal = 1;
                            int contador_casilla = 1;
                            int contador_grupo = 1;
                            foreach (CasillasRecuento casilla in lsRecuento)
                            {
                                int grupo_asignado = contador_grupo;
                                lsRecuento[contador_principal - 1].grupo_trabajo = grupo_asignado;
                                contador_casilla++;
                                contador_principal++;
                                if (contador_casilla > limitador_parcial)
                                {
                                    if (contador_principal <= limitador_total)
                                    {
                                        contador_casilla = 1;
                                        contador_grupo++;
                                    }
                                    else
                                    {
                                        contador_casilla = 1;
                                        contador_grupo = grupos_trabajo;
                                    }
                                }
                            }
                            foreach (CasillasRecuento casilla in lsRecuento)
                            {
                                sice_reserva_captura detalleCasilla = (from p in contexto.sice_reserva_captura where p.id_casilla == casilla.id_casilla select p).FirstOrDefault();

                                if(tipo_recuento == "TOTAL")
                                {
                                    detalleCasilla.id_estatus_acta = 5;
                                    detalleCasilla.tipo_reserva = "RECUENTO";
                                }
                                detalleCasilla.grupo_trabajo = casilla.grupo_trabajo;
                                contexto.SaveChanges();
                            }
                        //}



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

        public DetallesComputos DetalleComputos(int distrito)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    DetallesComputos cmp = new DetallesComputos();

                    cmp.total_actas = (from p in contexto.sice_casillas where p.id_distrito_local == distrito && p.tipo_votacion == "MR" select p.id).ToList().Count;
                    List<sice_reserva_captura> lsDetalles = (from p in contexto.sice_reserva_captura join cs in contexto.sice_casillas on p.id_casilla equals cs.id where cs.id_distrito_local == distrito select p).ToList();
                    if(lsDetalles != null)
                    {
                        cmp.total_capturado = lsDetalles.Where(x => x.tipo_reserva == "CAPTURADA").ToList().Count;
                        cmp.total_reserva = lsDetalles.Where(x => x.tipo_reserva == "RESERVA").ToList().Count;
                        cmp.total_no_conta = lsDetalles.Where(x => x.tipo_reserva == "NO CONTABILIZABLE").ToList().Count;
                        cmp.total_recuento = lsDetalles.Where(x => x.tipo_reserva == "RECUENTO").ToList().Count;

                    }
                    else
                    {
                        cmp.total_capturado = 0;
                        cmp.total_reserva = 0;
                        cmp.total_no_conta = 0;
                        cmp.total_recuento = 0;

                    }

                    return cmp;
                }
                    
            }
            catch(Exception E)
            {
                throw E;
            }
        }

        public List<int> ListaCasillaCapturadasComp()
        {
            try
            {
                List<int> listaCasilla = new List<int>();
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    listaCasilla = (from v in contexto.sice_votos where v.estatus == 1 select (int)v.id_casilla).Distinct().ToList();

                    return listaCasilla;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public sice_reserva_captura EstatusActa(int id_casilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from e in contexto.sice_reserva_captura where e.id_casilla == id_casilla select e).FirstOrDefault();
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
                List<int> listaCasilla = ListaCasillaCapturadasComp();
                List<TempVotosEstatus> listaVotosImportar = new List<TempVotosEstatus>();
                string condicion = "";
                string casilla;
                if (listaCasilla.Count > 0)
                {
                    casilla = string.Join(",", listaCasilla);
                    condicion = " AND RV.id_casilla NOT IN( " + casilla + " ) ";
                }

                //Buscar votos en la bd del servidor excluyendo casilla ya registradas o descargadas
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    string consulta =
                        "SELECT " +
                            "RV.*, RES.tipo_reserva as reserva " +
                        "FROM " +
                        "sice_votos RV " +
                        "JOIN sice_casillas C ON C.id = RV.id_casilla AND C.id_distrito_local = " + distrito + " " +
                        "JOIN sice_reserva_captura RES ON RES.id_casilla = RV.id_casilla AND (RES.tipo_reserva = 'CAPTURADA' OR RES.tipo_reserva = 'NO CONTABILIZABLE') " +
                        "WHERE RV.estatus = 1 " + condicion;
                    listaVotosImportar = contexto.Database.SqlQuery<TempVotosEstatus>(consulta).ToList();


                }
                if (listaVotosImportar.Count > 0)
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
            catch (Exception E)
            {
                return 0;
            }
        }

        public void guardarVotosImportados(List<TempVotosEstatus> listaVotos)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        sice_votos v1 = null;
                        foreach (TempVotosEstatus voto in listaVotos)
                        {
                            if (voto.id_candidato != null)
                            {
                                v1 = (from d in contexto.sice_votos where d.id_candidato == voto.id_candidato && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    v1 = (from d in contexto.sice_votos where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    v1 = (from d in contexto.sice_votos where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
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

                            sice_reserva_captura rc = (from p in contexto.sice_reserva_captura where p.id_casilla == voto.id_casilla select p).FirstOrDefault();
                            if (rc != null)
                            {
                                rc.tipo_reserva = voto.reserva;
                                rc.importado = 1;
                            }
                            else
                            {
                                rc = new sice_reserva_captura();
                                rc.id_casilla = voto.id_casilla;
                                rc.tipo_reserva = voto.reserva;
                                rc.importado = 1;
                                contexto.sice_reserva_captura.Add(rc);
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

        public int importarExcel(OpenFileDialog fichero)
        {
            try
            {
                using (ExcelPackage archivoExcel = new ExcelPackage(new FileInfo(fichero.FileName)))
                {
                    ExcelWorkbook libro = archivoExcel.Workbook;
                    List<ExcelWorksheet> listaHojas = libro.Worksheets.ToList(); //select sheet here                    
                    foreach (ExcelWorksheet hojaActual in listaHojas)
                    {
                        this.guardarDatosExcel(hojaActual);
                    }
                }
                return 1;
            }
            catch (Exception E)
            {
                return 0;
            }
        }

        public void guardarDatosExcel(ExcelWorksheet hojaActual)
        {
            try
            {
                int filaInicio = 2;
                int totalRows = hojaActual.Dimension.End.Row;
                for (int rowNum = filaInicio; rowNum <= totalRows; rowNum++) //selet starting row here
                {
                    using (DatabaseContext contexto = new DatabaseContext(con))
                    {
                        using (var TransactionContexto = new TransactionScope())
                        {
                            switch (hojaActual.Name)
                            {
                                case "sice_ar_votos_cotejo":
                                    int tempId = Convert.ToInt32(hojaActual.Cells[rowNum, 2].Value);
                                    int? id_candidato = tempId == 0 ? null : (int?)tempId;
                                    int? id_casilla = Convert.ToInt32(hojaActual.Cells[rowNum, 3].Value);
                                    string tipo = hojaActual.Cells[rowNum, 5].Value.ToString();
                                    sice_ar_votos_cotejo v1 = null;
                                    if (id_candidato != null)
                                    {
                                        v1 = (from d in contexto.sice_ar_votos_cotejo where d.id_candidato == id_candidato && d.id_casilla == id_casilla select d).FirstOrDefault();
                                    }
                                    else
                                    {
                                        if (tipo == "NULO")
                                            v1 = (from d in contexto.sice_ar_votos_cotejo where d.tipo == "NULO" && d.id_casilla == id_casilla select d).FirstOrDefault();
                                        else if (tipo == "NO REGISTRADO")
                                            v1 = (from d in contexto.sice_ar_votos_cotejo where d.tipo == "NO REGISTRADO" && d.id_casilla == id_casilla select d).FirstOrDefault();
                                    }

                                    if (v1 != null)
                                    {
                                        v1.id_candidato = id_candidato;
                                        v1.id_casilla = id_casilla;
                                        v1.tipo = tipo;
                                        v1.votos = Convert.ToInt32(hojaActual.Cells[rowNum, 4].Value);
                                        v1.importado = Convert.ToInt32(hojaActual.Cells[rowNum, 6].Value);
                                        v1.estatus = Convert.ToInt32(hojaActual.Cells[rowNum, 7].Value); ;
                                        contexto.SaveChanges();
                                    }
                                    break;
                                case "sice_ar_reserva":
                                    int? id_casilla2 = Convert.ToInt32(hojaActual.Cells[rowNum, 2].Value);
                                    sice_ar_reserva rc = (from p in contexto.sice_ar_reserva where p.id_casilla == id_casilla2 select p).FirstOrDefault();
                                    if (rc != null)
                                    {
                                        rc.id_casilla = Convert.ToInt32(hojaActual.Cells[rowNum, 2].Value);
                                        rc.tipo_reserva = hojaActual.Cells[rowNum, 3].Value.ToString();
                                        rc.id_documento = Convert.ToInt32(hojaActual.Cells[rowNum, 4].Value);
                                        rc.importado = Convert.ToInt32(hojaActual.Cells[rowNum, 5].Value);
                                        rc.id_supuesto = Convert.ToInt32(hojaActual.Cells[rowNum, 6].Value);
                                        string x = hojaActual.Cells[rowNum, 7].Value.ToString();

                                        rc.create_at = hojaActual.Cells[rowNum, 7].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 7].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                        rc.updated_at = hojaActual.Cells[rowNum, 8].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 8].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                        rc.num_escritos = Convert.ToInt32(hojaActual.Cells[rowNum, 9].Value);
                                        rc.boletas_sobrantes = Convert.ToInt32(hojaActual.Cells[rowNum, 10].Value);
                                        rc.personas_votaron = Convert.ToInt32(hojaActual.Cells[rowNum, 11].Value);
                                        rc.num_representantes_votaron = Convert.ToInt32(hojaActual.Cells[rowNum, 12].Value);
                                        rc.votos_sacados = Convert.ToInt32(hojaActual.Cells[rowNum, 13].Value);
                                        rc.casilla_instalada = Convert.ToInt32(hojaActual.Cells[rowNum, 14].Value);
                                        rc.id_estatus_acta = Convert.ToInt32(hojaActual.Cells[rowNum, 15].Value);
                                        rc.id_estatus_paquete = Convert.ToInt32(hojaActual.Cells[rowNum, 16].Value);
                                        rc.id_incidencias = Convert.ToInt32(hojaActual.Cells[rowNum, 17].Value);
                                        rc.inicializada = Convert.ToInt32(hojaActual.Cells[rowNum, 18].Value);
                                    }
                                    else
                                    {
                                        rc = new sice_ar_reserva();
                                        rc.id_casilla = Convert.ToInt32(hojaActual.Cells[rowNum, 2].Value);
                                        rc.tipo_reserva = hojaActual.Cells[rowNum, 3].Value.ToString();
                                        rc.id_documento = Convert.ToInt32(hojaActual.Cells[rowNum, 4].Value);
                                        rc.importado = Convert.ToInt32(hojaActual.Cells[rowNum, 5].Value);
                                        rc.id_supuesto = Convert.ToInt32(hojaActual.Cells[rowNum, 6].Value);
                                        rc.create_at = hojaActual.Cells[rowNum, 7].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 7].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                        rc.updated_at = hojaActual.Cells[rowNum, 8].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 8].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                        rc.num_escritos = Convert.ToInt32(hojaActual.Cells[rowNum, 9].Value);
                                        rc.boletas_sobrantes = Convert.ToInt32(hojaActual.Cells[rowNum, 10].Value);
                                        rc.personas_votaron = Convert.ToInt32(hojaActual.Cells[rowNum, 11].Value);
                                        rc.num_representantes_votaron = Convert.ToInt32(hojaActual.Cells[rowNum, 12].Value);
                                        rc.votos_sacados = Convert.ToInt32(hojaActual.Cells[rowNum, 13].Value);
                                        rc.casilla_instalada = Convert.ToInt32(hojaActual.Cells[rowNum, 14].Value);
                                        rc.id_estatus_acta = Convert.ToInt32(hojaActual.Cells[rowNum, 15].Value);
                                        rc.id_estatus_paquete = Convert.ToInt32(hojaActual.Cells[rowNum, 16].Value);
                                        rc.id_incidencias = Convert.ToInt32(hojaActual.Cells[rowNum, 17].Value);
                                        rc.inicializada = Convert.ToInt32(hojaActual.Cells[rowNum, 18].Value);
                                        contexto.sice_ar_reserva.Add(rc);
                                    }
                                    contexto.SaveChanges();
                                    break;
                                case "sice_ar_historico":
                                    sice_ar_historico hs = new sice_ar_historico();
                                    hs.id_supuesto = Convert.ToInt32(hojaActual.Cells[rowNum, 2].Value);
                                    hs.fecha = hojaActual.Cells[rowNum, 3].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 3].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                    hs.id_casilla = Convert.ToInt32(hojaActual.Cells[rowNum, 4].Value);
                                    hs.importado = Convert.ToInt32(hojaActual.Cells[rowNum, 5].Value);
                                    contexto.sice_ar_historico.Add(hs);
                                    contexto.SaveChanges();
                                    break;
                                case "sice_ar_documentos":
                                    string documento = hojaActual.Cells[rowNum, 2].Value.ToString();
                                    sice_ar_documentos doc = (from d in contexto.sice_ar_documentos where d.nombre == documento select d).FirstOrDefault();
                                    if (doc != null)
                                    {
                                        doc.nombre = hojaActual.Cells[rowNum, 2].Value.ToString();
                                        doc.ruta = hojaActual.Cells[rowNum, 3].Value.ToString();
                                        doc.estatus = hojaActual.Cells[rowNum, 4].Value.ToString();
                                        doc.filtro = Convert.ToInt32(hojaActual.Cells[rowNum, 5].Value);
                                        doc.estatus_filtro1 = Convert.ToInt32(hojaActual.Cells[rowNum, 6].Value);
                                        doc.estatus_filtro2 = Convert.ToInt32(hojaActual.Cells[rowNum, 7].Value);
                                        doc.estatus_filtro3 = Convert.ToInt32(hojaActual.Cells[rowNum, 8].Value);
                                        doc.estatus_revisor = Convert.ToInt32(hojaActual.Cells[rowNum, 8].Value);
                                        doc.estatus_cotejador = Convert.ToInt32(hojaActual.Cells[rowNum, 10].Value);
                                        doc.id_casilla = Convert.ToInt32(hojaActual.Cells[rowNum, 11].Value);
                                        doc.identificado = hojaActual.Cells[rowNum, 12].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 12].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                        doc.create_at = hojaActual.Cells[rowNum, 13].Value.ToString() != "" ? DateTime.Parse(hojaActual.Cells[rowNum, 13].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
                                        doc.updated_at = hojaActual.Cells[rowNum, 14].Value.ToString() != "" ? DateTime.Parse(hojaActual.Cells[rowNum, 14].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
                                        doc.importado = Convert.ToInt32(hojaActual.Cells[rowNum, 15].Value);
                                        doc.importado_dato = Convert.ToInt32(hojaActual.Cells[rowNum, 16].Value);


                                    }
                                    else
                                    {
                                        doc = new sice_ar_documentos();
                                        doc.nombre = hojaActual.Cells[rowNum, 2].Value.ToString();
                                        doc.ruta = hojaActual.Cells[rowNum, 3].Value.ToString();
                                        doc.estatus = hojaActual.Cells[rowNum, 4].Value.ToString();
                                        doc.filtro = Convert.ToInt32(hojaActual.Cells[rowNum, 5].Value);
                                        doc.estatus_filtro1 = Convert.ToInt32(hojaActual.Cells[rowNum, 6].Value);
                                        doc.estatus_filtro2 = Convert.ToInt32(hojaActual.Cells[rowNum, 7].Value);
                                        doc.estatus_filtro3 = Convert.ToInt32(hojaActual.Cells[rowNum, 8].Value);
                                        doc.estatus_revisor = Convert.ToInt32(hojaActual.Cells[rowNum, 8].Value);
                                        doc.estatus_cotejador = Convert.ToInt32(hojaActual.Cells[rowNum, 10].Value);
                                        doc.id_casilla = Convert.ToInt32(hojaActual.Cells[rowNum, 11].Value);
                                        doc.identificado = hojaActual.Cells[rowNum, 12].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 12].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                        doc.create_at = hojaActual.Cells[rowNum, 13].Value.ToString() != "" ? DateTime.Parse(hojaActual.Cells[rowNum, 13].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
                                        doc.updated_at = hojaActual.Cells[rowNum, 14].Value.ToString() != "" ? DateTime.Parse(hojaActual.Cells[rowNum, 14].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : DateTime.Now;
                                        doc.importado = Convert.ToInt32(hojaActual.Cells[rowNum, 15].Value);
                                        doc.importado_dato = Convert.ToInt32(hojaActual.Cells[rowNum, 16].Value);
                                        contexto.sice_ar_documentos.Add(doc);
                                    }
                                    contexto.SaveChanges();
                                    break;
                                case "sice_votos":
                                    int tempId2 = Convert.ToInt32(hojaActual.Cells[rowNum, 2].Value);
                                    int? id_candidato2 = tempId2 == 0 ? null : (int?)tempId2;
                                    int? id_casilla3 = Convert.ToInt32(hojaActual.Cells[rowNum, 3].Value);
                                    string tipo2 = hojaActual.Cells[rowNum, 5].Value.ToString();
                                    sice_votos v2 = null;
                                    if (id_candidato2 != null)
                                    {
                                        v2 = (from d in contexto.sice_votos where d.id_candidato == id_candidato2 && d.id_casilla == id_casilla3 select d).FirstOrDefault();
                                    }
                                    else
                                    {
                                        if (tipo2 == "NULO")
                                            v2 = (from d in contexto.sice_votos where d.tipo == "NULO" && d.id_casilla == id_casilla3 select d).FirstOrDefault();
                                        else if (tipo2 == "NO REGISTRADO")
                                            v2 = (from d in contexto.sice_votos where d.tipo == "NO REGISTRADO" && d.id_casilla == id_casilla3 select d).FirstOrDefault();
                                    }

                                    if (v2 != null)
                                    {
                                        v2.id_candidato = id_candidato2;
                                        v2.id_casilla = id_casilla3;
                                        v2.votos = Convert.ToInt32(hojaActual.Cells[rowNum, 4].Value);
                                        v2.tipo = tipo2;
                                        v2.importado = Convert.ToInt32(hojaActual.Cells[rowNum, 6].Value);
                                        v2.estatus = Convert.ToInt32(hojaActual.Cells[rowNum, 7].Value); ;

                                    }
                                    contexto.SaveChanges();
                                    break;
                                case "sice_reserva_captura":
                                    int? id_casilla4 = Convert.ToInt32(hojaActual.Cells[rowNum, 2].Value);
                                    sice_reserva_captura rc2 = (from p in contexto.sice_reserva_captura where p.id_casilla == id_casilla4 select p).FirstOrDefault();
                                    if (rc2 != null)
                                    {
                                        rc2.id_casilla = Convert.ToInt32(hojaActual.Cells[rowNum, 2].Value);
                                        rc2.tipo_reserva = hojaActual.Cells[rowNum, 3].Value.ToString();
                                        rc2.importado = Convert.ToInt32(hojaActual.Cells[rowNum, 4].Value);
                                        rc2.id_supuesto = Convert.ToInt32(hojaActual.Cells[rowNum, 5].Value);
                                        rc2.num_escritos = Convert.ToInt32(hojaActual.Cells[rowNum, 6].Value);
                                        rc2.boletas_sobrantes = Convert.ToInt32(hojaActual.Cells[rowNum, 7].Value);
                                        rc2.create_at = hojaActual.Cells[rowNum, 8].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 8].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                        rc2.updated_at = hojaActual.Cells[rowNum, 9].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 9].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                        rc2.personas_votaron = Convert.ToInt32(hojaActual.Cells[rowNum, 10].Value);
                                        rc2.num_representantes_votaron = Convert.ToInt32(hojaActual.Cells[rowNum, 11].Value);
                                        rc2.votos_sacados = Convert.ToInt32(hojaActual.Cells[rowNum, 12].Value);
                                        rc2.casilla_instalada = Convert.ToInt32(hojaActual.Cells[rowNum, 13].Value);
                                        rc2.id_estatus_acta = Convert.ToInt32(hojaActual.Cells[rowNum, 14].Value);
                                        rc2.id_estatus_paquete = Convert.ToInt32(hojaActual.Cells[rowNum, 15].Value);
                                        rc2.id_incidencias = Convert.ToInt32(hojaActual.Cells[rowNum, 16].Value);
                                        rc2.inicializada = Convert.ToInt32(hojaActual.Cells[rowNum, 17].Value);
                                    }
                                    else
                                    {
                                        rc2 = new sice_reserva_captura();

                                        rc2.id_casilla = Convert.ToInt32(hojaActual.Cells[rowNum, 2].Value);
                                        rc2.tipo_reserva = hojaActual.Cells[rowNum, 3].Value.ToString();
                                        rc2.importado = Convert.ToInt32(hojaActual.Cells[rowNum, 4].Value);
                                        rc2.id_supuesto = Convert.ToInt32(hojaActual.Cells[rowNum, 5].Value);
                                        rc2.num_escritos = Convert.ToInt32(hojaActual.Cells[rowNum, 6].Value);
                                        rc2.boletas_sobrantes = Convert.ToInt32(hojaActual.Cells[rowNum, 7].Value);
                                        rc2.create_at = hojaActual.Cells[rowNum, 8].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 8].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                        rc2.updated_at = hojaActual.Cells[rowNum, 9].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 9].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                        rc2.personas_votaron = Convert.ToInt32(hojaActual.Cells[rowNum, 10].Value);
                                        rc2.num_representantes_votaron = Convert.ToInt32(hojaActual.Cells[rowNum, 11].Value);
                                        rc2.votos_sacados = Convert.ToInt32(hojaActual.Cells[rowNum, 12].Value);
                                        rc2.casilla_instalada = Convert.ToInt32(hojaActual.Cells[rowNum, 13].Value);
                                        rc2.id_estatus_acta = Convert.ToInt32(hojaActual.Cells[rowNum, 14].Value);
                                        rc2.id_estatus_paquete = Convert.ToInt32(hojaActual.Cells[rowNum, 15].Value);
                                        rc2.id_incidencias = Convert.ToInt32(hojaActual.Cells[rowNum, 16].Value);
                                        rc2.inicializada = Convert.ToInt32(hojaActual.Cells[rowNum, 17].Value);
                                        contexto.sice_reserva_captura.Add(rc2);
                                    }
                                    contexto.SaveChanges();
                                    break;
                                case "sice_historico":
                                    sice_historico hs2 = new sice_historico();
                                    hs2.id_supuesto = Convert.ToInt32(hojaActual.Cells[rowNum, 2].Value);
                                    hs2.fecha = hojaActual.Cells[rowNum, 3].Value.ToString() != "" ? (DateTime?)DateTime.Parse(hojaActual.Cells[rowNum, 3].Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null;
                                    hs2.id_casilla = Convert.ToInt32(hojaActual.Cells[rowNum, 4].Value);
                                    hs2.importado = Convert.ToInt32(hojaActual.Cells[rowNum, 5].Value);
                                    contexto.sice_historico.Add(hs2);
                                    contexto.SaveChanges();
                                    break;
                            }
                            TransactionContexto.Complete();

                        }

                        //contexto.Database.Connection.Close();
                    }

                    //var row = myWorksheet.Cells[rowNum, 1].Value;
                    //Console.WriteLine("Valor: " + hojaActual.Cells[rowNum, 1].Value);
                }


            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public int generarExcelRespaldo(SaveFileDialog fichero)
        {
            try
            {

                Excel.Application excel = new Excel.Application();
                Excel._Workbook libro = null;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                libro = (Excel._Workbook)excel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);

                List<string> entidades = new List<string>(new string[] { "sice_historico", "sice_reserva_captura", "sice_votos" });

                foreach (string entidad in entidades)
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
                        
                        case "sice_historico":
                            List<sice_historico> data2 = (from d in contexto.sice_historico select d).ToList();
                            foreach (sice_historico d in data2)
                            {
                                hoja.Cells[fila, 1] = d.id;
                                hoja.Cells[fila, 2] = d.id_supuesto;
                                hoja.Cells[fila, 3].NumberFormat = "@";
                                hoja.Cells[fila, 3] = d.fecha != null ? ((DateTime)d.fecha).ToString("yyyy-MM-dd hh:mm:ss") : "";
                                hoja.Cells[fila, 4] = d.id_casilla;
                                hoja.Cells[fila, 5] = d.importado;
                                fila++;
                            }
                            break;
                        case "sice_reserva_captura":
                            List<sice_reserva_captura> data3 = (from d in contexto.sice_reserva_captura select d).ToList();
                            foreach (sice_reserva_captura d in data3)
                            {
                                hoja.Cells[fila, 1] = d.id;
                                hoja.Cells[fila, 2] = d.id_casilla;
                                hoja.Cells[fila, 3] = d.tipo_reserva;
                                hoja.Cells[fila, 4] = d.importado;
                                hoja.Cells[fila, 5] = d.id_supuesto;
                                hoja.Cells[fila, 6] = d.num_escritos;
                                hoja.Cells[fila, 7] = d.boletas_sobrantes;
                                hoja.Cells[fila, 8].NumberFormat = "@";
                                hoja.Cells[fila, 8] = d.create_at != null ? ((DateTime)d.create_at).ToString("yyyy-MM-dd hh:mm:ss"): "";
                                hoja.Cells[fila, 9].NumberFormat = "@";
                                hoja.Cells[fila, 9] = d.updated_at != null ? ((DateTime)d.updated_at).ToString("yyyy-MM-dd hh:mm:ss"): "";
                                hoja.Cells[fila, 10] = d.personas_votaron;
                                hoja.Cells[fila, 11] = d.num_representantes_votaron;
                                hoja.Cells[fila, 12] = d.votos_sacados;
                                hoja.Cells[fila, 13] = d.casilla_instalada;
                                hoja.Cells[fila, 14] = d.id_estatus_acta;
                                hoja.Cells[fila, 15] = d.id_estatus_paquete;
                                hoja.Cells[fila, 16] = d.id_incidencias;
                                hoja.Cells[fila, 17] = d.inicializada;
                                fila++;
                            }
                            break;
                        case "sice_votos":
                            List<sice_votos> data4 = this.ResultadosSiceVotos();
                            foreach (sice_votos d in data4)
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
                    case "sice_historico":
                        nombres = typeof(sice_historico).GetProperties()
                       .Select(property => property.Name)
                       .ToList();
                        break;
                    case "sice_reserva_captura":
                        nombres = typeof(sice_reserva_captura).GetProperties()
                       .Select(property => property.Name)
                       .ToList();
                        break;
                    case "sice_votos":
                        nombres = typeof(sice_votos).GetProperties()
                       .Select(property => property.Name)
                       .ToList();
                        break;
                }

                int cont = 1;
                foreach (string nombre in nombres)
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
                    bool flagRecuentoTotal = false;

                    foreach (sice_distritos_locales ds in distritos.OrderByDescending(x => x.id))
                    {
                        Console.WriteLine("Insetando Libro: " + ds.distrito);

                        List<CasillasRecuento> casillasRecuento = this.ListaCasillasRecuentos(ds.id, false,true);
                        int totalRecuento = casillasRecuento.Count();
                        sice_configuracion_recuento conf = this.Configuracion_Recuento("SICE", ds.id);
                        int grupos_tabajo = 0;
                        int puntos_recuento = 0;
                        if (conf != null)
                        {
                            grupos_tabajo = (int)conf.grupos_trabajo;
                            puntos_recuento = (int)conf.puntos_recuento;

                            if (conf.tipo_recuento == "TOTAL")
                            {
                                flagRecuentoTotal = true;
                                casillasRecuento = this.ListaCasillasRecuentos(ds.id, true,true);
                                totalRecuento = casillasRecuento.Count();
                                decimal cGt = Math.Round(Convert.ToDecimal(casillasRecuento.Count) / Convert.ToDecimal(grupos_tabajo), 0);
                                int limitador_parcial = Convert.ToInt32(cGt);
                                int limitador_total = limitador_parcial * (grupos_tabajo - 1);
                                int contador_principal = 1;
                                int contador_casilla = 1;
                                int contador_grupo = 1;
                                foreach (CasillasRecuento casilla in casillasRecuento)
                                {
                                    int grupo_asignado = contador_grupo;
                                    casillasRecuento[contador_principal - 1].grupo_trabajo = grupo_asignado;
                                    contador_casilla++;
                                    contador_principal++;
                                    if (contador_casilla > limitador_parcial)
                                    {
                                        if (contador_principal <= limitador_total)
                                        {
                                            contador_casilla = 1;
                                            contador_grupo++;
                                        }
                                        else
                                        {
                                            contador_casilla = 1;
                                            contador_grupo = grupos_tabajo;
                                        }
                                    }
                                }
                            }
                        }
                        List<CandidatosResultados> lsCandidatos = ListaResultadosCandidatos(ds.id, true);
                        double diferenciaPorcentajeTotal = 0;
                        //listaSumaCandidatos.OrderBy(x => x.votos);
                        int TotalVotosDistrito = lsCandidatos.Sum(x => (int)x.votos);
                        lsCandidatos = lsCandidatos.Select(data => new CandidatosResultados
                        {
                            id_candidato = data.id_candidato,
                            partido = data.partido,
                            candidato = data.candidato,
                            votos = data.votos,
                            tipo = data.tipo
                        }).Where(x => x.tipo == "VOTO").OrderByDescending(x => x.votos).ToList();
                        if (lsCandidatos.Count > 0)
                        {

                            int PrimeroTotal = (int)lsCandidatos[0].votos;
                            int SeegundoTotal = (int)lsCandidatos[1].votos;
                            int diferenciaTotal = PrimeroTotal - SeegundoTotal;

                            if (TotalVotosDistrito > 0)
                            {
                                diferenciaPorcentajeTotal = Math.Round(((double)diferenciaTotal * 100) / TotalVotosDistrito, 2);
                            }

                        }


                        this.generaHojaRecuento(ds.id, libro, casillasRecuento, diferenciaPorcentajeTotal, totalRecuento, flagRecuentoTotal);

                    }
                }
                else
                {
                    List<CasillasRecuento> casillasRecuento = this.ListaCasillasRecuentos(distrito, true);
                    this.generaHojaRecuento(distrito, libro, casillasRecuento);
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

        public void generaHojaRecuento(int distrito, Excel._Workbook libro, List<CasillasRecuento> listaCasillas, double diferencia = 0, int totalRecuento = 0, bool flagRecuentoTotal = false)
        {
            try
            {
                Excel._Worksheet hoja = null;
                Excel.Range rango = null;
                int filaInicialTabla = 11;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                hoja = (Excel._Worksheet)libro.Worksheets.Add();
                hoja.Name = "DISTRITO " + distrito;  //Aqui debe ir el nombre del distrito

                List<CasillasRecuento> listaRecuento = (from d in listaCasillas where d.id_distrito_local == distrito select d).ToList();

                //Montamos las cabeceras 
                char letraFinal = CrearEncabezadosRecuento(filaInicialTabla, ref hoja, distrito, listaCasillas.Count, totalRecuento, diferencia, 1);

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


                if (listaCasillas.Count > 0)
                {
                    foreach (CasillasRecuento casillla in listaCasillas)
                    {
                        //Agregar Columnas
                        hoja.Cells[fila, 1] = casillla.id_casilla;
                        hoja.Cells[fila, 2] = casillla.seccion;
                        hoja.Cells[fila, 3] = casillla.casilla;
                        hoja.Cells[fila, 4] = flagRecuentoTotal ? "RECUENTO TOTAL" : casillla.supuesto;
                        hoja.Cells[fila, 5] = casillla.grupo_trabajo != null ? "GT-" + casillla.grupo_trabajo : "NO APLICA";

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

        private char CrearEncabezadosRecuento(int fila, ref Excel._Worksheet hoja, int distrito, int totalDistritoCasillasRecuento, int totalRecuento, double diferencia = 0, int columnaInicial = 1)
        {
            try
            {
                Excel.Range rango;
                string rutaImagen = System.AppDomain.CurrentDomain.BaseDirectory + "imagenes\\";

                sice_casillas casilla = null;
                sice_distritos_locales dlocal = null;
                sice_municipios mun = null;

                sice_configuracion_recuento conf = this.Configuracion_Recuento("SICE", distrito);
                int grupos_tabajo = 0;
                int puntos_recuento = 0;
                int horas = 0;
                string tipo_recuento = "PARCIAL";

                if (conf != null)
                {
                    tipo_recuento = conf.tipo_recuento;
                    grupos_tabajo = (int)conf.grupos_trabajo;
                    puntos_recuento = (int)conf.puntos_recuento;
                    horas = Convert.ToInt32(conf.horas_disponibles);
                }

                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    casilla = (from c in contexto.sice_casillas where c.id_distrito_local == distrito select c).FirstOrDefault();
                    mun = (from m in contexto.sice_municipios where m.id == casilla.id_cabecera_local select m).FirstOrDefault();
                    dlocal = (from d in contexto.sice_distritos_locales where d.id == distrito select d).FirstOrDefault();
                }
                //Configuracon Hoja
                hoja.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
                hoja.PageSetup.Zoom = 69;
                hoja.PageSetup.PrintTitleRows = "$10:$11";

                //** Montamos el título en la línea 1 **
                hoja.Cells[1, 3] = "SISTEMA DE REGISTRO DE ACTAS DEL PROCESO ELECTORAL LÓCAL 2017-2018";
                hoja.Range[hoja.Cells[1, 3], hoja.Cells[1, 5]].Merge();
                hoja.Cells[2, 3] = "ELECCIÓN DE DIPUTADOS DE MAYORÍA RELATIVA POR CASILLA, SECCIÓN Y DISTRITO LOCAL";
                hoja.Range[hoja.Cells[2, 3], hoja.Cells[2, 5]].Merge();
                hoja.Cells[3, 3] = "LISTA DE CASILLAS A RECUENTO";
                hoja.Range[hoja.Cells[3, 3], hoja.Cells[3, 5]].Merge();
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


                hoja.Cells[fila - 6, columnaInicial + 2] = tipo_recuento;
                //if(diferencia == 0)
                //{
                //    hoja.Cells[fila - 6, columnaInicial + 2] = "NO APLICA"; //Si diferencia menos a 1% recuento Total, sino Parcial
                //}
                //else
                //{
                //    hoja.Cells[fila - 6, columnaInicial + 2] = (diferencia < 1) ? "TOTAL" : "PARCIAL"; //Si diferencia menos a 1% recuento Total, sino Parcial
                //}
                hoja.Range[hoja.Cells[fila - 6, columnaInicial + 2], hoja.Cells[fila - 6, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 6, columnaInicial + 2].WrapText = true;
                hoja.Cells[fila - 6, columnaInicial + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                hoja.Cells[fila - 6, columnaInicial + 2].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                hoja.Cells[fila - 6, columnaInicial + 2].Font.Bold = true;

                hoja.Cells[fila - 5, columnaInicial] = "TOTAL CASILLAS A RECUENTO " + dlocal.distrito;
                hoja.Cells[fila - 5, columnaInicial].RowHeight = 35;
                hoja.Range[hoja.Cells[fila - 5, columnaInicial], hoja.Cells[fila - 5, columnaInicial + 1]].Merge();
                hoja.Cells[fila - 5, columnaInicial].WrapText = true;
                hoja.Cells[fila - 5, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                hoja.Cells[fila - 5, columnaInicial].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                hoja.Cells[fila - 5, columnaInicial + 2] = totalDistritoCasillasRecuento; //TOTAL DE CASILLAS A RECUENTO
                hoja.Range[hoja.Cells[fila - 5, columnaInicial + 2], hoja.Cells[fila - 5, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 5, columnaInicial + 2].WrapText = true;
                hoja.Cells[fila - 5, columnaInicial + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                hoja.Cells[fila - 5, columnaInicial + 2].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                hoja.Cells[fila - 5, columnaInicial + 2].Font.Bold = true;

                hoja.Cells[fila - 4, columnaInicial] = "HORAS PARA RECONTAR ";
                hoja.Cells[fila - 4, columnaInicial].RowHeight = 35;
                hoja.Range[hoja.Cells[fila - 4, columnaInicial], hoja.Cells[fila - 4, columnaInicial + 1]].Merge();
                hoja.Cells[fila - 4, columnaInicial].WrapText = true;
                hoja.Cells[fila - 4, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                hoja.Cells[fila - 4, columnaInicial].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;

                hoja.Cells[fila - 4, columnaInicial + 2] = horas + " hrs."; //TOTAL DE CASILLAS A RECUENTO
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

                hoja.Cells[fila - 3, columnaInicial + 2] = totalRecuento == 0 || totalRecuento <= 20 || grupos_tabajo == 0 ? "NO APLICA" : grupos_tabajo.ToString(); //CALCULAR NUMERO DE GRUPOS DE TRABAJO
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

                hoja.Cells[fila - 2, columnaInicial + 2] = totalRecuento == 0 || totalRecuento <= 20 || puntos_recuento == 0 ? "NO APLICA" : puntos_recuento.ToString(); //PUNTOS DE RECUENTO
                hoja.Range[hoja.Cells[fila - 2, columnaInicial + 2], hoja.Cells[fila - 2, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 2, columnaInicial + 2].WrapText = true;
                hoja.Cells[fila - 2, columnaInicial + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                hoja.Cells[fila - 2, columnaInicial + 2].VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
                hoja.Cells[fila - 2, columnaInicial + 2].Font.Bold = true;


                hoja.Cells[fila - 1, columnaInicial] = dlocal.distrito + " CABECERA " + mun.municipio;
                hoja.Range[hoja.Cells[fila - 1, columnaInicial], hoja.Cells[fila - 1, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 1, columnaInicial].WrapText = true;
                hoja.Cells[fila - 1, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                hoja.Cells[fila, columnaInicial] = "No."; columnaInicial++; columnaLetra++; widths.Add(8.57);
                hoja.Cells[fila, columnaInicial] = "Sección"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "Casilla"; columnaInicial++; columnaLetra++; widths.Add(25.29);
                hoja.Cells[fila, columnaInicial] = "Motivo Recuento"; columnaInicial++; columnaLetra++; widths.Add(100);
                hoja.Cells[fila, columnaInicial] = "Grupo de Trabajo."; columnaInicial++; widths.Add(25.29);

                //Colores de Fondo
                rango = hoja.Range["A" + fila, "E" + fila];
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

        public int generarExcel(SaveFileDialog fichero, int distrito, bool completo = false)
        {
            try
            {
                Excel.Application excel = new Excel.Application();
                Excel._Workbook libro = null;

                //completo = true;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                libro = (Excel._Workbook)excel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);

                if (completo)
                {
                    List<sice_distritos_locales> distritos = this.ListaDistritos();                    
                    foreach(sice_distritos_locales ds in distritos.OrderByDescending(x => x.id))
                    {
                        Console.WriteLine("Insetando Libro: " + ds.distrito);
                        this.generaHoja(ds.id, libro);
                        //this.generarHojaRP(ds.id, libro);
                    }
                }
                else
                {
                    this.generaHoja(distrito, libro);
                    //this.generarHojaRP(distrito, libro);
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

        public void generaHoja(int distrito, Excel._Workbook libro)
        {
            try
            {
                Excel._Worksheet hoja = null;
                Excel.Range rango = null;
                int filaInicialTabla = 7;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                hoja = (Excel._Worksheet)libro.Worksheets.Add();
                hoja.Name = "DISTRITO "+distrito;  //Aqui debe ir el nombre del distrito
                List<VotosSeccion> vSeccion = this.ResultadosSeccion(0, 0, (int)distrito);
                if (LoginInfo.privilegios == 6)
                    vSeccion = vSeccion.Where(x => x.grupo_trabajo == LoginInfo.grupo_trabajo).ToList();



                //List<VotosSeccion> vSeccion = this.ResultadosSeccion(1, 1, (int)distrito);
                List<Candidatos> candidatos = this.ListaCandidatos((int)distrito);
                //int tempC = candidatos.Count;
                int TotalRepresentantes = 0;
                //foreach (Candidatos cnd in candidatos)
                //{
                //    if (cnd.coalicion != "" && cnd.coalicion != null && cnd.tipo_partido != "COALICION")
                //    {
                //        TotalRepresentantes += this.RepresentantesCComun(cnd.coalicion);
                //    }
                //    else if (cnd.tipo_partido != "COALICION")
                //    {
                //        if (cnd.partido_local == 1)
                //            TotalRepresentantes += 1;
                //        else if (cnd.partido_local == 0)
                //            TotalRepresentantes += 2;
                //    }
                //}

                //Montamos las cabeceras 
                char letraFinal = CrearEncabezados(filaInicialTabla, ref hoja,vSeccion,candidatos,distrito,1);


                //Agregar Datos
                int fila = filaInicialTabla+1;
                int idCasillaActual = 0;
                int cont = 1;
                int contCand = 6;
                //row.Cells[0].Value = 1;
                //dgvResultados.Rows.Add(row);
                List<int> vLst = new List<int>();
                int Noregynulo = 0;
                int Lnominal = 0;
                bool flagInsert = true;

                int votos = 0;
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
                            hoja.Cells[fila, 2] = v.seccion; hoja.Cells[fila, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            hoja.Cells[fila, 3] = v.casilla;
                            hoja.Cells[fila, 4] = (v.estatus != null) ? v.estatus : "NO CAPTURADA";

                            votos = v.estatus_acta != "CAPTURADA" ? 0 : (int)v.votos;
                            hoja.Cells[fila,contCand].Value = votos;
                            vLst.Add(votos);
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
                        hoja.Cells[fila,5] = diferencia + "%";

                        //Votacion Emitida
                        hoja.Cells[fila,contCand] = totalVotacionEmitida;

                        //Lista Nominal
                        hoja.Cells[fila,contCand + 1] = Lnominal;

                        //Porcentaje de Participacion
                        if (totalVotacionEmitida == 0)
                            hoja.Cells[fila,contCand + 2] = 0 + "%";
                        else
                            hoja.Cells[fila,contCand + 2] = Math.Round((Convert.ToDecimal(totalVotacionEmitida) * 100) / Lnominal, 2) + "%";

                        //Agregar Estilo fila
                        string x = "A" + (fila).ToString();
                        string y = letraFinal.ToString() + (fila).ToString();
                        rango = hoja.Range[x, y];
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        //Console.WriteLine("Ins")
                        fila++;
                        contCand = 6;
                        vLst = new List<int>();
                        Noregynulo = 0;
                        flagInsert = true;
                        //Inrementar filla
                    }

                    if (cont >= vSeccion.Count)
                        break;
                    if (flagInsert)
                    {
                        //Agregar Columnas
                        hoja.Cells[fila, 1] = v.id_casilla;
                        hoja.Cells[fila, 2] = v.seccion; hoja.Cells[fila, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        hoja.Cells[fila, 3] = v.casilla; hoja.Cells[fila, 3].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        hoja.Cells[fila, 4] = (v.estatus_acta != null) ? v.estatus : "NO CAPTURADA";
                    }

                    Lnominal = v.lista_nominal + TotalRepresentantes;

                    votos = v.estatus != "CAPTURADA" ? 0 : (int)v.votos;
                    hoja.Cells[fila,contCand] = votos;
                    if (v.tipo == "VOTO")
                        vLst.Add(votos);
                    else
                        Noregynulo += votos;

                    idCasillaActual = (int)v.id_casilla;
                    cont++;
                    contCand++;

                    flagInsert = false;

                    //if(cont == vSeccion.Count){
                    //    dgvResultados.Rows.Add(row);
                    //}


                }

                //**************APARTADO DE RESULTADOS TOTALES
                fila += 4;
                letraFinal = CrearEncabezadosTotalesMR(fila, ref hoja, vSeccion, candidatos, distrito, 1);


                //vSeccion = this.ResultadosSeccion(0, 0, (int)distrito); //<----QUITAR ESTO
                List<VotosSeccion> totalAgrupado = vSeccion.GroupBy(x => x.id_casilla).
                    Select(data => new VotosSeccion
                    {
                        id_candidato = data.First().id_candidato,
                        casilla = data.First().casilla,
                        lista_nominal = data.First().tipo == "S1" || data.First().tipo == "S1-RP" ? data.First().lista_nominal : data.First().lista_nominal + TotalRepresentantes,
                        votos = data.First().votos
                    }).ToList();

                int LnominalDistrito = totalAgrupado.Sum(x => x.lista_nominal);
                int TotalVotosDistrito = vSeccion.Where(x => x.id_estatus_acta == 1).Sum(x => (int)x.votos);
                int totalSecciones = vSeccion.GroupBy(x => x.seccion).Select(data => new VotosSeccion { seccion = data.First().seccion }).Count();
                int totalCasillas = totalAgrupado.Count();

                //this.lblListaNominal.Text = String.Format(CultureInfo.InvariantCulture, "{0:#,#}", LnominalDistrito);
                // this.lblTotalVotos.Text = TotalVotosDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", TotalVotosDistrito) : "0";

                decimal PorcentajeParDistrito = 0;
                if (TotalVotosDistrito > 0)
                {
                    PorcentajeParDistrito = Math.Round((Convert.ToDecimal(TotalVotosDistrito) * 100) / LnominalDistrito, 2);
                }
                //this.lblParticipacion.Text = PorcentajeParDistrito + "%";

                string diferenciaPorcentaje = "0%";
                int diferenciaT = 0;
                List<CandidatosResultados> lsCandidatos = null;
                if (LoginInfo.privilegios == 6)
                    lsCandidatos = ListaResultadosCandidatos(distrito, true,true);
                else
                    lsCandidatos = ListaResultadosCandidatos(distrito,true);
                if(lsCandidatos.Count == 0)
                    lsCandidatos = ListaResultadosCandidatos(distrito, false);
                List<CandidatosResultados> lsCandidatos2 = null;
                lsCandidatos2 = lsCandidatos.Select(data => new CandidatosResultados
                {
                    id_candidato = data.id_candidato,
                    partido = data.partido,
                    candidato = data.candidato,
                    votos = data.votos,
                    tipo = data.tipo
                }).Where(x => x.tipo == "VOTO").OrderByDescending(x => x.votos).ToList();
                if (lsCandidatos2.Count > 0)
                {
                    int PrimeroTotal = (int)lsCandidatos2[0].votos;
                    int SeegundoTotal = (int)lsCandidatos2[1].votos;
                    diferenciaT = PrimeroTotal - SeegundoTotal;
                    decimal diferenciaPorcentajeTotal = 0;
                    if (TotalVotosDistrito > 0 && diferenciaT > 0)
                    {
                        diferenciaPorcentajeTotal = Math.Round((Convert.ToDecimal(diferenciaT) * 100) / TotalVotosDistrito, 2);
                    }
                    diferenciaPorcentaje = diferenciaPorcentajeTotal + "%";
                }

                fila++;
                contCand = 6;
                cont = 1;
                string[] stringSeparators = new string[] { "," };
                string[] result;
                List<PorcentajePartido> lsPorcentajePartido = new List<PorcentajePartido>();
                if (lsCandidatos != null)
                {
                    foreach(CandidatosResultados candidato in lsCandidatos)
                    {                        
                        hoja.Cells[fila, contCand] = candidato.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", candidato.votos) : "0"; 
                        decimal porcentaje = 0;
                        if (TotalVotosDistrito > 0 && candidato.votos > 0)
                        {
                            porcentaje = Math.Round((Convert.ToDecimal(candidato.votos) * 100) / Convert.ToDecimal(TotalVotosDistrito), 2);
                        }
                        candidato.porcentaje = (double)porcentaje;

                        //Calculos Para RP
                        if(candidato.coalicion != null && candidato.coalicion!= "")
                        {
                            if(candidato.id_partido == 14)
                            {
                                //PAN-PRD-PD
                                result = candidato.coalicion.Split(stringSeparators, StringSplitOptions.None);
                                List<PorcentajePartido> lsTemp = new List<PorcentajePartido>();
                                foreach (string r in result)
                                {
                                    int id_partido = Convert.ToInt32(r);
                                    double porcentajeTotal = (double)Math.Floor(porcentaje);
                                    if(porcentajeTotal > 0)
                                    {
                                        double porcentajeCoalicion = porcentajeRP(id_partido, (int)Math.Floor(porcentaje));
                                        int totalVotos = candidato.votos;
                                        double res = (porcentajeCoalicion * totalVotos) / porcentajeTotal;
                                        lsTemp.Add(new PorcentajePartido { id_partido = id_partido, votos = res });
                                    }
                                    else
                                    {
                                        lsTemp.Add(new PorcentajePartido { id_partido = id_partido, votos = 0 });
                                    }
                                    

                                    //float porcentajeCoalicion = porcentajeRP(Convert.ToInt32(r), (int)Math.Floor(porcentaje));
                                    //lsRes.Add( ( porcentaje * candidato.votos) 
                                }
                                lsTemp = lsTemp.OrderBy(x => x.votos).ToList();
                                double tmpTotal = 0;
                                double tempVotoS = 0;
                                double porcentajeTemp = 0;
                                for (int c = 0; c < lsTemp.Count; c++)
                                {
                                    if(lsTemp[c].id_partido == 2 && c == lsTemp.Count -1)
                                    {
                                        tempVotoS = candidato.votos - tmpTotal;
                                        porcentajeTemp = TotalVotosDistrito > 0 ? Math.Round((tempVotoS * 100) / TotalVotosDistrito, 2) : 0;
                                        lsTemp[c].porcentaje = porcentajeTemp;
                                        lsTemp[c].votos = tempVotoS;
                                    }
                                    else
                                    {
                                        tempVotoS = (double)Math.Floor((decimal)lsTemp[c].votos); //Math.Round(lsTemp[c].votos, 0);
                                        porcentajeTemp = TotalVotosDistrito > 0 ? Math.Round((tempVotoS * 100) / TotalVotosDistrito, 2) : 0;
                                        lsTemp[c].votos = tempVotoS;
                                        lsTemp[c].porcentaje = porcentajeTemp;
                                        tmpTotal += tempVotoS;
                                    }
                                }
                                lsPorcentajePartido.AddRange(lsTemp);
                            }
                            else if(candidato.id_partido == 15)
                            {
                                //PT-MORENA
                                List<CandidatosResultados> tempCand = lsCandidatos.Where(x => x.id_partido == 5 || x.id_partido == 9).OrderBy(x=> x.votos).ToList();
                                double res1 = 0;
                                double res2 = 0;
                                int totalVotosCoalicion = candidato.votos;
                                if ((totalVotosCoalicion % 2) == 0)
                                {
                                    res1 = tempCand[0].votos + (double)Math.Floor((decimal)(totalVotosCoalicion / 2));
                                    res2 = tempCand[1].votos + (double)Math.Floor((decimal)(totalVotosCoalicion / 2));
                                }
                                   
                                else
                                {
                                    res1 = tempCand[0].votos + (double)Math.Floor((decimal)(totalVotosCoalicion / 2));
                                    res2 = tempCand[1].votos + (double)Math.Floor((decimal)(totalVotosCoalicion / 2)) + 1;
                                }
                                    
                                
                                lsPorcentajePartido.Add(new PorcentajePartido { id_partido = tempCand[0].id_partido, votos = res1 , porcentaje = TotalVotosDistrito > 0 ? Math.Round((res1 * 100) / TotalVotosDistrito, 2) : 0 });
                                lsPorcentajePartido.Add(new PorcentajePartido { id_partido = tempCand[1].id_partido, votos = res2 , porcentaje = TotalVotosDistrito > 0 ? Math.Round((res2 * 100) / TotalVotosDistrito, 2) : 0 });

                                int yy = 0;
                            }
                            
                        }
                        //else
                        //{
                        //    lsPorcentajePartido.Add(new PorcentajePartido { id_partido = candidato.id_partido, votos = candidato.votos, porcentaje = (double)porcentaje, tipo = candidato.tipo });
                        //}
                        

                        //lsPorcentajePartido.Add(new PorcentajePartido { id_partido = candidato.id_partido, porcentaje = porcentaje,votos = candidato.votos });
                        hoja.Cells[fila+1, contCand] = porcentaje + "%";



                        if (cont == lsCandidatos.Count)
                        {
                            hoja.Cells[fila, 1] = distrito;
                            hoja.Cells[fila, 2] = totalSecciones;
                            hoja.Cells[fila, 3] = totalCasillas;
                            hoja.Cells[fila, 4] = diferenciaT;
                            hoja.Cells[fila+1, 4] = diferenciaPorcentaje;

                            hoja.Range[hoja.Cells[fila, 4], hoja.Cells[fila, 5]].Merge();
                            hoja.Cells[fila, 4].WrapText = true;
                            hoja.Cells[fila, 4].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                            hoja.Range[hoja.Cells[fila + 1, 4], hoja.Cells[fila+1, 5]].Merge();
                            hoja.Cells[fila + 1, 4].WrapText = true;
                            hoja.Cells[fila + 1, 4].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            //Votacion Emitida
                            hoja.Cells[fila, contCand + 1] = TotalVotosDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", TotalVotosDistrito) : "0" ;
                            hoja.Cells[fila+1, contCand + 1] = "100%";

                            //Lista Nominal
                            hoja.Cells[fila, contCand + 2] = LnominalDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", LnominalDistrito) : "0";
                            hoja.Range[hoja.Cells[fila, contCand + 2], hoja.Cells[fila+1, contCand + 2]].Merge();
                            hoja.Cells[fila, contCand + 2].WrapText = true;
                            hoja.Cells[fila, contCand + 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            hoja.Cells[fila, contCand + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                            //Porcentaje de Participacion
                            hoja.Cells[fila, contCand + 3] = PorcentajeParDistrito + "%";
                            hoja.Range[hoja.Cells[fila, contCand + 3], hoja.Cells[fila + 1, contCand + 3]].Merge();
                            hoja.Cells[fila, contCand + 3].WrapText = true;
                            hoja.Cells[fila, contCand + 3].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            hoja.Cells[fila, contCand + 3].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                            //Agregar estilo fila
                            string x = "A" + (fila).ToString();
                            string y = letraFinal.ToString() + (fila).ToString();
                            rango = hoja.Range[x, y];
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                            //Agregar estilo fila
                            x = "D" + (fila+1).ToString();
                            y = letraFinal.ToString() + (fila).ToString();
                            rango = hoja.Range[x, y];
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                            
                        }                       
                        contCand++;
                        cont++;
                    }
                }
                fila += 7; //aqui se incrementa la fila para el siguiente apartado

                if (LoginInfo.privilegios == 6)
                {
                    return;
                }

                ///***************APARTADO DE RP
                bool flagColumna = lsCandidatos.Count > 11 ? true : false;
                letraFinal = CrearEncabezadosTotalesRP(fila, ref hoja,  distrito,flagColumna, 1);

                List<sice_partidos_politicos> lsPartidos = ListaPartidosPoliticos();
                fila++;
                contCand = 5;
                cont = 1;
                if (flagColumna)
                {
                    contCand++;
                }
                List<PorcentajePartido> temPP = new List<PorcentajePartido>();
                foreach (sice_partidos_politicos p in lsPartidos)
                {
                    
                    int tempVotos = 0;
                    PorcentajePartido porcentajePartido = lsPorcentajePartido.Where(x => x.id_partido == p.id).FirstOrDefault();
                    if(porcentajePartido != null)
                    {
                        temPP.Add(new PorcentajePartido { id_partido = porcentajePartido.id_partido, votos = porcentajePartido.votos, porcentaje = porcentajePartido.porcentaje });
                        hoja.Cells[fila, contCand] = porcentajePartido.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", porcentajePartido.votos) : "0";
                        hoja.Cells[fila + 1, contCand] = porcentajePartido.porcentaje + "%";
                    }
                    else
                    {
                        CandidatosResultados candidatosResultados = lsCandidatos.Where(x => x.id_partido == p.id).FirstOrDefault();
                        if (candidatosResultados != null)
                        {
                            temPP.Add(new PorcentajePartido { id_partido = candidatosResultados.id_partido, votos = candidatosResultados.votos, porcentaje = candidatosResultados.porcentaje });
                            hoja.Cells[fila, contCand] = candidatosResultados.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", candidatosResultados.votos) : "0";
                            hoja.Cells[fila + 1, contCand] = candidatosResultados.porcentaje + "%";
                        }
                        else
                        {
                            temPP.Add(new PorcentajePartido { id_partido = p.id, votos = 0, porcentaje = 0 });
                            hoja.Cells[fila, contCand] = "0";
                            hoja.Cells[fila + 1, contCand] =  "0%";
                        }
                    }
                    if (cont == lsPartidos.Count)
                    {
                        contCand++;
                        temPP = temPP.OrderByDescending(xx => xx.votos).ToList();

                        //Diferencia entre 1er y 2do Lugar Votos
                        hoja.Cells[fila, 1] = temPP[0].votos - temPP[1].votos;
                        hoja.Cells[fila, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        //Diferencia entre 1er y 2do Lugar Porcentaje
                        hoja.Cells[fila+1, 1] = (temPP[0].porcentaje - temPP[1].porcentaje) + "%";
                        hoja.Cells[fila+1, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        //NO REGISTRADOS
                        CandidatosResultados cnd1 = lsCandidatos.Where(yy => yy.tipo == "NO REGISTRADO").FirstOrDefault();
                        hoja.Cells[fila, contCand] = cnd1.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", cnd1.votos) : "0"; ;
                        hoja.Cells[fila + 1, contCand] = cnd1.porcentaje > 0 ? cnd1.porcentaje+ "%": "0%";
                        contCand++;

                        //NULOS
                        CandidatosResultados cnd2 = lsCandidatos.Where(z => z.tipo == "NULO").FirstOrDefault();
                        hoja.Cells[fila, contCand] = cnd2.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", cnd2.votos) : "0"; ;
                        hoja.Cells[fila + 1, contCand] = cnd2.porcentaje > 0 ? cnd2.porcentaje+"%" : "0%";

                        //Votacion Emitida
                        hoja.Cells[fila, contCand + 1] = TotalVotosDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", TotalVotosDistrito) : "0";
                        hoja.Cells[fila + 1, contCand + 1] = "100%";

                        //Lista Nominal
                        hoja.Cells[fila, contCand + 2] = LnominalDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", LnominalDistrito) : "0";
                        hoja.Range[hoja.Cells[fila, contCand + 2], hoja.Cells[fila + 1, contCand + 2]].Merge();
                        hoja.Cells[fila, contCand + 2].WrapText = true;
                        hoja.Cells[fila, contCand + 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        hoja.Cells[fila, contCand + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                        //Porcentaje de Participacion
                        hoja.Cells[fila, contCand + 3] = PorcentajeParDistrito + "%";
                        hoja.Range[hoja.Cells[fila, contCand + 3], hoja.Cells[fila + 1, contCand + 3]].Merge();
                        hoja.Cells[fila, contCand + 3].WrapText = true;
                        hoja.Cells[fila, contCand + 3].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        hoja.Cells[fila, contCand + 3].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        //Agregar estilo fila
                        string x = "A" + (fila).ToString();
                        string y = letraFinal.ToString() + (fila).ToString();
                        rango = hoja.Range[x, y];
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        //Agregar estilo fila
                        x = "A" + (fila + 1).ToString();
                        y = letraFinal.ToString() + (fila).ToString();
                        rango = hoja.Range[x, y];
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;


                    }
                    contCand++;
                    cont++;


                }
                this.generarHojaRP(distrito, libro, temPP,lsCandidatos,TotalVotosDistrito,LnominalDistrito,PorcentajeParDistrito);


            }
            catch (Exception E)
            {
                throw E;
            }
        }

        private char CrearEncabezados(int fila, ref Excel._Worksheet hoja, List<VotosSeccion> vSeccion, List<Candidatos> candidatos,int distrito, int columnaInicial = 1)
        {
            try
            {
                Excel.Range rango;
                Excel.Range rangoTitutlo;
                float Left = 0;
                float Top = 0;
                const float ImageSize = 42; //Tamaño Imagen Partidos
                string rutaImagen = System.AppDomain.CurrentDomain.BaseDirectory + "imagenes\\";

                bool flagColumna = candidatos.Count > 11 ? true : false;

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
                hoja.PageSetup.Zoom = flagColumna ? 65:63;
                hoja.PageSetup.PrintTitleRows = "$1:$7";

                hoja.PageSetup.TopMargin = 37.79;
                hoja.PageSetup.BottomMargin = 37.79;
                hoja.PageSetup.LeftMargin = 22.67;
                hoja.PageSetup.RightMargin = 22.67;



                //** Montamos el título en la línea 1 **
                hoja.Cells[1, 3] = "SISTEMA DE REGISTRO DE ACTAS DEL PROCESO ELECTORAL LÓCAL 2017-2018";
                hoja.Cells[2, 3] = "RESULTADOS ELECTORALES POR PARTIDOS POLÍTICOS O CANDIDATURA INDEPENDIENTE";
                hoja.Cells[3, 3] = "ELECCIÓN DE DIPUTADOS DE MAYORÍA RELATIVA POR CASILLA, SECCIÓN Y DISTRITO LOCAL";
                char columnaLetra = 'A';
                hoja.Shapes.AddPicture(rutaImagen + "iepc.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 125, 45);
                //hoja.Shapes.

                List<double> widths = new List<double>();

                //Agregar encabezados
                hoja.Cells[fila - 3, columnaInicial] = dlocal.distrito + " CABECERA " + mun.municipio;
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                hoja.Cells[fila, columnaInicial] = "No."; columnaInicial++; columnaLetra++; widths.Add(8.57);
                hoja.Cells[fila, columnaInicial] = "Sección"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "Casilla"; columnaInicial++; columnaLetra++; widths.Add(10.29);
                hoja.Cells[fila, columnaInicial] = "Estatus"; columnaInicial++; columnaLetra++; widths.Add(15.29);

                hoja.Cells[fila, columnaInicial] = "Diferencia entre 1° y 2° Lugar"; columnaInicial++; columnaLetra++; widths.Add(12.29);
                hoja.Cells[fila, columnaInicial - 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila, columnaInicial - 1].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila, columnaInicial - 1], hoja.Cells[fila - 3, columnaInicial - 1]].Merge();
                hoja.Cells[fila, columnaInicial - 1].WrapText = true;
                hoja.Cells[fila, columnaInicial - 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;



                //Agregar Columnas Caniddatos y Partidos
                foreach (Candidatos c in candidatos)
                {
                    //Agregar Imagen del Partido
                    rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                    hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                    Left = 3 + (float)((double)rango.Left);
                    Top = (float)((double)rango.Top);

                    hoja.Shapes.AddPicture(rutaImagen + c.imagen + ".jpg", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                    hoja.Cells[fila, columnaInicial] = c.partido;
                    hoja.Cells[fila, columnaInicial].Font.Size = 10;
                    columnaInicial++; columnaLetra++; widths.Add(9.57);
                }
                //Agregar columnas adicionales

                //Imagen no registrados
                rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
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

                hoja.Cells[fila - 3, columnaInicial] = "Votación Total Emitida";
                hoja.Cells[fila - 3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila - 3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                hoja.Cells[fila, columnaInicial] = "TOTAL"; columnaInicial++; columnaLetra++; widths.Add(8.57);

                hoja.Cells[fila, columnaInicial] = "L. Nominal"; columnaInicial++; columnaLetra++; widths.Add(10);
                hoja.Cells[fila, columnaInicial] = "%"; widths.Add(10);
                hoja.Cells[fila - 3, columnaInicial] = "Lista Nominal y Porcentaje de Participación Ciudadana";
                hoja.Cells[fila - 3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila - 3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial - 1], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                //Colores de Fondo
                rango = hoja.Range["A" + fila, "D" + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                //Colores de Fondo Partido
                rango = hoja.Range["F" + fila, columnaLetra.ToString() + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(38)))), ((int)(((byte)(36))))));
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

        private char CrearEncabezadosTotalesMR(int fila, ref Excel._Worksheet hoja, List<VotosSeccion> vSeccion, List<Candidatos> candidatos, int distrito, int columnaInicial = 1)
        {
            try
            {
                Excel.Range rango;
                Excel.Range rangoTitutlo;
                float Left = 0;
                float Top = 0;
                const float ImageSize = 42; //Tamaño Imagen Partidos
                string rutaImagen = System.AppDomain.CurrentDomain.BaseDirectory + "imagenes\\";
                List<double> widths = new List<double>();
                char columnaLetra = 'A';

                //Agregar encabezados
                hoja.Cells[fila - 3, columnaInicial] = "RESULTADOS TOTALES";
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial + 2]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                hoja.Cells[fila, columnaInicial] = "DISTRITO"; columnaInicial++; columnaLetra++; 
                hoja.Cells[fila, columnaInicial] = "Secciones"; columnaInicial++; columnaLetra++; 
                hoja.Cells[fila, columnaInicial] = "Casillas"; columnaInicial++; columnaLetra++; 

                hoja.Cells[fila, columnaInicial] = "Diferencia entre 1° y 2° Lugar"; columnaInicial++; columnaLetra++; widths.Add(12.29);
                hoja.Cells[fila, columnaInicial - 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila, columnaInicial - 1].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila, columnaInicial - 1], hoja.Cells[fila - 3, columnaInicial]].Merge();
                hoja.Cells[fila, columnaInicial - 1].WrapText = true;
                hoja.Cells[fila, columnaInicial - 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter; columnaInicial++; columnaLetra++;



                //Agregar Columnas Caniddatos y Partidos
                foreach (Candidatos c in candidatos)
                {
                    //Agregar Imagen del Partido
                    rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                    hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                    Left = 3 + (float)((double)rango.Left);
                    Top = (float)((double)rango.Top);

                    hoja.Shapes.AddPicture(rutaImagen + c.imagen + ".jpg", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                    hoja.Cells[fila, columnaInicial] = c.partido;
                    hoja.Cells[fila, columnaInicial].Font.Size = 10;
                    columnaInicial++; columnaLetra++; widths.Add(9.57);
                }
                //Agregar columnas adicionales

                //Imagen no registrados
                rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
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

                hoja.Cells[fila - 3, columnaInicial] = "Votación Total Emitida";
                hoja.Cells[fila - 3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila - 3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                hoja.Cells[fila, columnaInicial] = "TOTAL"; columnaInicial++; columnaLetra++; widths.Add(8.57);

                hoja.Cells[fila, columnaInicial] = "L. Nominal"; columnaInicial++; columnaLetra++; widths.Add(10);
                hoja.Cells[fila, columnaInicial] = "%"; widths.Add(10);
                hoja.Cells[fila - 3, columnaInicial] = "Lista Nominal y Porcentaje de Participación Ciudadana";
                hoja.Cells[fila - 3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila - 3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial - 1], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                //Colores de Fondo
                rango = hoja.Range["A" + fila, "D" + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                //Colores de Fondo Partido
                rango = hoja.Range["F" + fila, columnaLetra.ToString() + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(38)))), ((int)(((byte)(36))))));
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
                //int cont = 1;
                //foreach (int widh in widths)
                //{
                //    rango = hoja.Columns[cont];
                //    rango.ColumnWidth = widh;
                //    cont++;
                //}
                return columnaLetra++;
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        private char CrearEncabezadosTotalesRP(int fila, ref Excel._Worksheet hoja, int distrito,bool flagColumna, int columnaInicial = 1,bool flagRp = false,string mensaje = "RESULTADOS TOTALES POR PARTIDO")
        {
            try
            {
                Excel.Range rango;
                Excel.Range rangoTitutlo;
                int colExtra = 0;
                if (flagRp)
                    colExtra = 1;
                float Left = 0;
                float Top = 2;
                const float ImageSize = 42; //Tamaño Imagen Partidos
                string rutaImagen = System.AppDomain.CurrentDomain.BaseDirectory + "imagenes\\";
                List<double> widths = new List<double>();
                char columnaLetra = 'A';

                //Agregar encabezados
                hoja.Cells[fila - 3, columnaInicial] = mensaje;
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, flagColumna ? columnaInicial +4 +colExtra: columnaInicial + 3+colExtra]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                //hoja.Cells[fila, columnaInicial] = "DISTRITO"; columnaInicial++; columnaLetra++;
                //hoja.Cells[fila, columnaInicial] = "Secciones"; columnaInicial++; columnaLetra++;
                //hoja.Cells[fila, columnaInicial] = "Casillas"; columnaInicial++; columnaLetra++;

                hoja.Cells[fila, columnaInicial] = "Diferencia entre 1° y 2° Lugar"; columnaInicial++; columnaLetra++; widths.Add(12.29);
                hoja.Cells[fila, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila, columnaInicial-1], hoja.Cells[fila, flagColumna ? columnaInicial+3 +colExtra: columnaInicial+2 +colExtra]].Merge();
                hoja.Range[hoja.Cells[fila+1, columnaInicial - 1], hoja.Cells[fila+1, flagColumna ? columnaInicial + 3 : columnaInicial + 2+colExtra]].Merge();
                hoja.Range[hoja.Cells[fila+2, columnaInicial - 1], hoja.Cells[fila+2, flagColumna ? columnaInicial + 3 : columnaInicial + 2+colExtra]].Merge();

                hoja.Cells[fila, columnaInicial-1].WrapText = true;
                hoja.Cells[fila, columnaInicial-1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter; columnaInicial+=3; columnaLetra++; columnaLetra++; columnaLetra++;
                if (flagColumna)
                {
                    columnaInicial++;
                    columnaLetra++;
                }
                if (flagRp)
                {
                    columnaInicial++;
                    columnaLetra++;
                }
                    
                    


                List<sice_partidos_politicos> lsPartidos = this.ListaPartidosPoliticos();
                int test = 1;
                //Agregar Columnas Caniddatos y Partidos
                foreach (sice_partidos_politicos p in lsPartidos)
                {
                    //Agregar Imagen del Partido
                    rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                    hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                    Left = 3 + (float)((double)rango.Left);
                    Top = (float)((double)rango.Top);
                    if(test == 1 && !flagColumna && !flagRp)
                        hoja.Shapes.AddPicture(rutaImagen + p.img_par + ".jpg", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left+10, Top, ImageSize, ImageSize);
                    else
                        hoja.Shapes.AddPicture(rutaImagen + p.img_par + ".jpg", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                    test++;
                    hoja.Cells[fila, columnaInicial] = p.siglas_par;
                    hoja.Cells[fila, columnaInicial].Font.Size = 10;
                    columnaInicial++; columnaLetra++; widths.Add(9.57);
                }
                //Agregar columnas adicionales

                //Imagen no registrados
                rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
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

                hoja.Cells[fila - 3, columnaInicial] = "Votación Total Emitida";
                hoja.Cells[fila - 3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila - 3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                hoja.Cells[fila, columnaInicial] = "TOTAL"; columnaInicial++; columnaLetra++; widths.Add(8.57);

                hoja.Cells[fila, columnaInicial] = "L. Nominal"; columnaInicial++; columnaLetra++; widths.Add(10);
                hoja.Cells[fila, columnaInicial] = "%"; widths.Add(10);
                hoja.Cells[fila - 3, columnaInicial] = "Lista Nominal y Porcentaje de Participación Ciudadana";
                hoja.Cells[fila - 3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila - 3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial - 1], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                //Colores de Fondo
                rango = hoja.Range["A" + fila, "D" + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                //Colores de Fondo Partido
                rango = hoja.Range["E" + fila, columnaLetra.ToString() + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(38)))), ((int)(((byte)(36))))));
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

                return columnaLetra++;
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public void generarHojaRP(int distrito, Excel._Workbook libro,List<PorcentajePartido> temPP, List<CandidatosResultados> lsCandidatos,int TotalVotosDistritoMR, int LnominalDistritoMR, decimal PorcentajeParDistritoMR)
        {
            try
            {
                List<VotosSeccion> vSeccion = this.ResultadosSeccionRP(0, 0, (int)distrito);
                if (vSeccion.Count == 0)
                    return;

                Excel._Worksheet hoja = null;
                Excel.Range rango = null;
                int filaInicialTabla = 7;
                List<PorcentajePartido> listafinal = new List<PorcentajePartido>();

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                hoja = (Excel._Worksheet)libro.Worksheets.Add();
                hoja.Name = "DISTRITO " + distrito + " RP";  //Aqui debe ir el nombre del distrito
                if (LoginInfo.privilegios == 6)
                    vSeccion = vSeccion.Where(x => x.grupo_trabajo == LoginInfo.grupo_trabajo).ToList();



                //List<VotosSeccion> vSeccion = this.ResultadosSeccion(1, 1, (int)distrito);
                List<sice_partidos_politicos> lsPartidos = ListaPartidosPoliticos();
                //int tempC = candidatos.Count;
                //int TotalRepresentantes = 1;
                //foreach (Candidatos cnd in candidatos)
                //{
                //    if (cnd.coalicion != "" && cnd.coalicion != null && cnd.tipo_partido != "COALICION")
                //    {
                //        TotalRepresentantes += this.RepresentantesCComun(cnd.coalicion);
                //    }
                //    else if (cnd.tipo_partido != "COALICION")
                //    {
                //        if (cnd.partido_local == 1)
                //            TotalRepresentantes += 1;
                //        else if (cnd.partido_local == 0)
                //            TotalRepresentantes += 2;
                //    }
                //}

                //Montamos las cabeceras 
                char letraFinal = CrearEncabezadosRP(filaInicialTabla, ref hoja, vSeccion, lsPartidos, distrito, 1);

                //Agregar Datos
                int fila = filaInicialTabla + 1;
                int idCasillaActual = 0;
                int cont = 1;
                int contCand = 6;
                List<int> vLst = new List<int>();
                int Noregynulo = 0;
                int Lnominal = 0;
                bool flagInsert = true;

                int votos = 0;
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
                            hoja.Cells[fila, 2] = v.seccion; hoja.Cells[fila, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            hoja.Cells[fila, 3] = v.casilla;
                            hoja.Cells[fila, 4] = (v.estatus != null) ? v.estatus : "NO CAPTURADA";

                            votos = v.estatus_acta != "CAPTURADA" ? 0 : (int)v.votos;
                            hoja.Cells[fila, contCand].Value = votos;
                            vLst.Add(votos);
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

                        //Agregar Estilo fila
                        string x = "A" + (fila).ToString();
                        string y = letraFinal.ToString() + (fila).ToString();
                        rango = hoja.Range[x, y];
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        //Console.WriteLine("Ins")
                        fila++;
                        contCand = 6;
                        vLst = new List<int>();
                        Noregynulo = 0;
                        flagInsert = true;
                        //Inrementar filla
                    }

                    if (cont >= vSeccion.Count)
                        break;
                    if (flagInsert)
                    {
                        //Agregar Columnas
                        hoja.Cells[fila, 1] = v.id_casilla;
                        hoja.Cells[fila, 2] = v.seccion; hoja.Cells[fila, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        hoja.Cells[fila, 3] = v.casilla; hoja.Cells[fila, 3].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        hoja.Cells[fila, 4] = (v.estatus_acta != null) ? v.estatus : "NO CAPTURADA";
                    }

                    Lnominal = v.lista_nominal ;

                    votos = v.estatus != "CAPTURADA" ? 0 : (int)v.votos;
                    hoja.Cells[fila, contCand] = votos;
                    if (v.tipo == "VOTO")
                        vLst.Add(votos);
                    else
                        Noregynulo += votos;

                    idCasillaActual = (int)v.id_casilla;
                    cont++;
                    contCand++;

                    flagInsert = false;
                    


                }

                fila += 4;
                letraFinal = CrearEncabezadosTotalesRPSP(fila, ref hoja, vSeccion, lsPartidos, distrito, 1);


                //vSeccion = this.ResultadosSeccion(0, 0, (int)distrito); //<----QUITAR ESTO
                List<VotosSeccion> totalAgrupado = vSeccion.GroupBy(x => x.id_casilla).
                    Select(data => new VotosSeccion
                    {
                        id_partido = data.First().id_partido,
                        casilla = data.First().casilla,
                        lista_nominal = data.First().lista_nominal,
                        votos = data.First().votos
                    }).ToList();

                int LnominalDistrito = totalAgrupado.Sum(x => x.lista_nominal);
                int TotalVotosDistrito = vSeccion.Where(x => x.id_estatus_acta == 1).Sum(x => (int)x.votos);
                int totalSecciones = vSeccion.GroupBy(x => x.seccion).Select(data => new VotosSeccion { seccion = data.First().seccion }).Count();
                int totalCasillas = totalAgrupado.Count();

                //this.lblListaNominal.Text = String.Format(CultureInfo.InvariantCulture, "{0:#,#}", LnominalDistrito);
                // this.lblTotalVotos.Text = TotalVotosDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", TotalVotosDistrito) : "0";

                decimal PorcentajeParDistrito = 0;
                if (TotalVotosDistrito > 0)
                {
                    PorcentajeParDistrito = Math.Round((Convert.ToDecimal(TotalVotosDistrito) * 100) / LnominalDistrito, 2);
                }
                //this.lblParticipacion.Text = PorcentajeParDistrito + "%";

                string diferenciaPorcentaje = "0%";
                int diferenciaT = 0;
                List<PartidosVotosRP> lsPartidosTotales = null;
                if (LoginInfo.privilegios == 6)
                    lsPartidosTotales = ListaResultadosPartidos(distrito, true, true);
                else
                    lsPartidosTotales = ListaResultadosPartidos(distrito, true);
                if (lsPartidosTotales.Count == 0)
                    lsPartidosTotales = ListaResultadosPartidos(distrito, false);
                List<PartidosVotosRP> lsPartidosTotales2 = null;
                lsPartidosTotales2 = lsPartidosTotales.Select(data => new PartidosVotosRP
                {
                    id_partido = data.id_partido,
                    partido = data.partido,
                    votos = data.votos,
                    tipo = data.tipo
                }).Where(x => x.tipo == "VOTO").OrderByDescending(x => x.votos).ToList();
                if (lsPartidosTotales2.Count > 0)
                {
                    int PrimeroTotal = (int)lsPartidosTotales2[0].votos;
                    int SeegundoTotal = (int)lsPartidosTotales2[1].votos;
                    diferenciaT = PrimeroTotal - SeegundoTotal;
                    decimal diferenciaPorcentajeTotal = 0;
                    if (TotalVotosDistrito > 0 && diferenciaT > 0)
                    {
                        diferenciaPorcentajeTotal = Math.Round((Convert.ToDecimal(diferenciaT) * 100) / TotalVotosDistrito, 2);
                    }
                    diferenciaPorcentaje = diferenciaPorcentajeTotal + "%";
                }

                fila++;
                contCand = 6;
                cont = 1;
                string[] stringSeparators = new string[] { "," };
                string[] result;
                if (lsPartidosTotales != null)
                {
                    foreach (PartidosVotosRP partido in lsPartidosTotales)
                    {
                        hoja.Cells[fila, contCand] = partido.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", partido.votos) : "0";
                        decimal porcentaje = 0;
                        if (TotalVotosDistrito > 0 && partido.votos > 0)
                        {
                            porcentaje = Math.Round((Convert.ToDecimal(partido.votos) * 100) / Convert.ToDecimal(TotalVotosDistrito), 2);
                        }

                        listafinal.Add(new PorcentajePartido { id_partido = partido.id_partido, tipo = partido.tipo, votos = (double)partido.votos });


                        //lsPorcentajePartido.Add(new PorcentajePartido { id_partido = candidato.id_partido, porcentaje = porcentaje,votos = candidato.votos });
                        hoja.Cells[fila + 1, contCand] = porcentaje + "%";



                        if (cont == lsPartidosTotales.Count)
                        {
                            hoja.Cells[fila, 1] = distrito;
                            hoja.Cells[fila, 2] = totalSecciones;
                            hoja.Cells[fila, 3] = totalCasillas;
                            hoja.Cells[fila, 4] = diferenciaT;
                            hoja.Cells[fila + 1, 4] = diferenciaPorcentaje;

                            hoja.Range[hoja.Cells[fila, 4], hoja.Cells[fila, 5]].Merge();
                            hoja.Cells[fila, 4].WrapText = true;
                            hoja.Cells[fila, 4].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                            hoja.Range[hoja.Cells[fila + 1, 4], hoja.Cells[fila + 1, 5]].Merge();
                            hoja.Cells[fila + 1, 4].WrapText = true;
                            hoja.Cells[fila + 1, 4].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            //Votacion Emitida
                            hoja.Cells[fila, contCand + 1] = TotalVotosDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", TotalVotosDistrito) : "0";
                            hoja.Cells[fila + 1, contCand + 1] = "100%";

                            //Lista Nominal
                            hoja.Cells[fila, contCand + 2] = LnominalDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", LnominalDistrito) : "0";
                            hoja.Range[hoja.Cells[fila, contCand + 2], hoja.Cells[fila + 1, contCand + 2]].Merge();
                            hoja.Cells[fila, contCand + 2].WrapText = true;
                            hoja.Cells[fila, contCand + 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            hoja.Cells[fila, contCand + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                            //Porcentaje de Participacion
                            hoja.Cells[fila, contCand + 3] = PorcentajeParDistrito + "%";
                            hoja.Range[hoja.Cells[fila, contCand + 3], hoja.Cells[fila + 1, contCand + 3]].Merge();
                            hoja.Cells[fila, contCand + 3].WrapText = true;
                            hoja.Cells[fila, contCand + 3].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            hoja.Cells[fila, contCand + 3].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                            //Agregar estilo fila
                            string x = "A" + (fila).ToString();
                            string y = letraFinal.ToString() + (fila).ToString();
                            rango = hoja.Range[x, y];
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                            //Agregar estilo fila
                            x = "D" + (fila + 1).ToString();
                            y = letraFinal.ToString() + (fila).ToString();
                            rango = hoja.Range[x, y];
                            rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;


                        }
                        contCand++;
                        cont++;
                    }
                }
                fila += 7;
                letraFinal = CrearEncabezadosTotalesRP(fila, ref hoja, distrito,false, 1,true);
                                
                fila++;
                contCand = 6;
                cont = 1;
                //if (flagColumna)
                //{
                //    contCand++;
                //}
                
                foreach (sice_partidos_politicos p in lsPartidos)
                {

                    int tempVotos = 0;
                    PorcentajePartido porcentajePartido = temPP.Where(x => x.id_partido == p.id).FirstOrDefault();
                    hoja.Cells[fila, contCand] = porcentajePartido.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", porcentajePartido.votos) : "0";
                    hoja.Cells[fila + 1, contCand] = porcentajePartido.porcentaje + "%";
                    listafinal[cont - 1].votos += porcentajePartido.votos; 

                    
                    if (cont == lsPartidos.Count)
                    {
                        contCand++;
                        temPP = temPP.OrderByDescending(xx => xx.votos).ToList();

                        //Diferencia entre 1er y 2do Lugar Votos
                        hoja.Cells[fila, 1] = temPP[0].votos - temPP[1].votos;
                        hoja.Cells[fila, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        //Diferencia entre 1er y 2do Lugar Porcentaje
                        hoja.Cells[fila + 1, 1] = (temPP[0].porcentaje - temPP[1].porcentaje) + "%";
                        hoja.Cells[fila + 1, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        //NO REGISTRADOS
                        CandidatosResultados cnd1 = lsCandidatos.Where(yy => yy.tipo == "NO REGISTRADO").FirstOrDefault();
                        //listafinal[listafinal.Count - 2].votos += cnd1.votos;
                        listafinal[cont].votos += cnd1.votos;
                        hoja.Cells[fila, contCand] = cnd1.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", cnd1.votos) : "0"; ;
                        hoja.Cells[fila + 1, contCand] = cnd1.porcentaje > 0 ? cnd1.porcentaje + "%" : "0%";
                        contCand++;

                        //NULOS
                        CandidatosResultados cnd2 = lsCandidatos.Where(z => z.tipo == "NULO").FirstOrDefault();
                        listafinal[cont + 1].votos += cnd2.votos;
                        //listafinal[listafinal.Count - 1].votos += cnd2.votos;
                        hoja.Cells[fila, contCand] = cnd2.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", cnd2.votos) : "0"; ;
                        hoja.Cells[fila + 1, contCand] = cnd2.porcentaje > 0 ? cnd2.porcentaje + "%" : "0%";

                        //Votacion Emitida
                        hoja.Cells[fila, contCand + 1] = TotalVotosDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", TotalVotosDistritoMR) : "0";
                        hoja.Cells[fila + 1, contCand + 1] = "100%";

                        //Lista Nominal
                        hoja.Cells[fila, contCand + 2] = LnominalDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", LnominalDistritoMR) : "0";
                        hoja.Range[hoja.Cells[fila, contCand + 2], hoja.Cells[fila + 1, contCand + 2]].Merge();
                        hoja.Cells[fila, contCand + 2].WrapText = true;
                        hoja.Cells[fila, contCand + 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        hoja.Cells[fila, contCand + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                        //Porcentaje de Participacion
                        hoja.Cells[fila, contCand + 3] = PorcentajeParDistritoMR + "%";
                        hoja.Range[hoja.Cells[fila, contCand + 3], hoja.Cells[fila + 1, contCand + 3]].Merge();
                        hoja.Cells[fila, contCand + 3].WrapText = true;
                        hoja.Cells[fila, contCand + 3].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        hoja.Cells[fila, contCand + 3].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        //Agregar estilo fila
                        string x = "A" + (fila).ToString();
                        string y = letraFinal.ToString() + (fila).ToString();
                        rango = hoja.Range[x, y];
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        //Agregar estilo fila
                        x = "A" + (fila + 1).ToString();
                        y = letraFinal.ToString() + (fila).ToString();
                        rango = hoja.Range[x, y];
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;


                    }
                    contCand++;
                    cont++;


                }

                fila += 7;
                letraFinal = CrearEncabezadosTotalesRP(fila, ref hoja, distrito, false, 1, true,"RESULTADOS TOTALES REPRSENTACIÓN PROPORCIONAL");

                fila++;
                contCand = 6;
                cont = 1;
                int TotalVotosDistritoF = TotalVotosDistrito + TotalVotosDistritoMR;
                int LnominalDistritoF = LnominalDistritoMR + LnominalDistrito;
                decimal PorcentajeParDistritoF = 0;
                if (TotalVotosDistritoF > 0)
                    PorcentajeParDistritoF = Math.Round((Convert.ToDecimal(TotalVotosDistritoF) * 100) / Convert.ToDecimal(LnominalDistritoF), 2);
                //if (flagColumna)
                //{
                //    contCand++;
                //}
                int yyyyyy = listafinal.Count;
                decimal porcentajeFinal = 0;
                foreach (PorcentajePartido p in listafinal)
                {
                    porcentajeFinal = 0;
                    if (TotalVotosDistritoF > 0)
                        porcentajeFinal = Math.Round((Convert.ToDecimal(p.votos) * 100) / Convert.ToDecimal(TotalVotosDistritoF), 2);

                    hoja.Cells[fila, contCand] = p.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", p.votos) : "0";
                    
                    hoja.Cells[fila + 1, contCand] = porcentajeFinal + "%";


                    if (cont == listafinal.Count)
                    {
                        //contCand++;
                        temPP = temPP.OrderByDescending(xx => xx.votos).ToList();

                        //Diferencia entre 1er y 2do Lugar Votos
                        hoja.Cells[fila, 1] = temPP[0].votos - temPP[1].votos;
                        hoja.Cells[fila, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        //Diferencia entre 1er y 2do Lugar Porcentaje
                        hoja.Cells[fila + 1, 1] = (temPP[0].porcentaje - temPP[1].porcentaje) + "%";
                        hoja.Cells[fila + 1, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        ////NO REGISTRADOS
                        //CandidatosResultados cnd1 = lsCandidatos.Where(yy => yy.tipo == "NO REGISTRADO").FirstOrDefault();
                        //hoja.Cells[fila, contCand] = cnd1.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", cnd1.votos) : "0"; ;
                        //hoja.Cells[fila + 1, contCand] = cnd1.porcentaje > 0 ? cnd1.porcentaje + "%" : "0%";
                        //contCand++;

                        ////NULOS
                        //CandidatosResultados cnd2 = lsCandidatos.Where(z => z.tipo == "NULO").FirstOrDefault();
                        //hoja.Cells[fila, contCand] = cnd2.votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", cnd2.votos) : "0"; ;
                        //hoja.Cells[fila + 1, contCand] = cnd2.porcentaje > 0 ? cnd2.porcentaje + "%" : "0%";

                        //Votacion Emitida
                        hoja.Cells[fila, contCand + 1] = TotalVotosDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", TotalVotosDistritoF) : "0";
                        hoja.Cells[fila + 1, contCand + 1] = "100%";

                        //Lista Nominal
                        hoja.Cells[fila, contCand + 2] = LnominalDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", LnominalDistritoF) : "0";
                        hoja.Range[hoja.Cells[fila, contCand + 2], hoja.Cells[fila + 1, contCand + 2]].Merge();
                        hoja.Cells[fila, contCand + 2].WrapText = true;
                        hoja.Cells[fila, contCand + 2].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        hoja.Cells[fila, contCand + 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                        //Porcentaje de Participacion
                        hoja.Cells[fila, contCand + 3] = PorcentajeParDistritoF + "%";
                        hoja.Range[hoja.Cells[fila, contCand + 3], hoja.Cells[fila + 1, contCand + 3]].Merge();
                        hoja.Cells[fila, contCand + 3].WrapText = true;
                        hoja.Cells[fila, contCand + 3].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        hoja.Cells[fila, contCand + 3].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        //Agregar estilo fila
                        string x = "A" + (fila).ToString();
                        string y = letraFinal.ToString() + (fila).ToString();
                        rango = hoja.Range[x, y];
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        //Agregar estilo fila
                        x = "A" + (fila + 1).ToString();
                        y = letraFinal.ToString() + (fila).ToString();
                        rango = hoja.Range[x, y];
                        rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;


                    }
                    contCand++;
                    cont++;


                }
            }
            catch(Exception E)
            {
                throw E;
            }
        }

        private char CrearEncabezadosRP(int fila, ref Excel._Worksheet hoja, List<VotosSeccion> vSeccion, List<sice_partidos_politicos> partidos, int distrito, int columnaInicial = 1)
        {
            try
            {
                Excel.Range rango;
                Excel.Range rangoTitutlo;
                float Left = 0;
                float Top = 0;
                const float ImageSize = 42; //Tamaño Imagen Partidos
                string rutaImagen = System.AppDomain.CurrentDomain.BaseDirectory + "imagenes\\";

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
                hoja.PageSetup.Zoom = 62;
                hoja.PageSetup.PrintTitleRows = "$1:$7";

                hoja.PageSetup.TopMargin = 37.79;
                hoja.PageSetup.BottomMargin = 37.79;
                hoja.PageSetup.LeftMargin = 22.67;
                hoja.PageSetup.RightMargin = 22.67;



                //** Montamos el título en la línea 1 **
                hoja.Cells[1, 3] = "SISTEMA DE REGISTRO DE ACTAS DEL PROCESO ELECTORAL LÓCAL 2017-2018";
                hoja.Cells[2, 3] = "RESULTADOS ELECTORALES POR PARTIDOS POLÍTICOS O CANDIDATURA INDEPENDIENTE";
                hoja.Cells[3, 3] = "ELECCIÓN DE DIPUTADOS DE REPRESENTACIÓN PROPORCIONAL, SECCIÓN Y DISTRITO LOCAL";
                char columnaLetra = 'A';
                hoja.Shapes.AddPicture(rutaImagen + "iepc.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 0, 0, 125, 45);
                //hoja.Shapes.

                List<double> widths = new List<double>();

                //Agregar encabezados
                hoja.Cells[fila - 3, columnaInicial] = dlocal.distrito + " CABECERA " + mun.municipio;
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial + 3]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;


                hoja.Cells[fila, columnaInicial] = "No."; columnaInicial++; columnaLetra++; widths.Add(8.57);
                hoja.Cells[fila, columnaInicial] = "Sección"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "Casilla"; columnaInicial++; columnaLetra++; widths.Add(10.29);
                hoja.Cells[fila, columnaInicial] = "Estatus"; columnaInicial++; columnaLetra++; widths.Add(15.29);

                hoja.Cells[fila, columnaInicial] = "Diferencia entre 1° y 2° Lugar"; columnaInicial++; columnaLetra++; widths.Add(12.29);
                hoja.Cells[fila, columnaInicial - 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila, columnaInicial - 1].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila, columnaInicial - 1], hoja.Cells[fila - 3, columnaInicial - 1]].Merge();
                hoja.Cells[fila, columnaInicial - 1].WrapText = true;
                hoja.Cells[fila, columnaInicial - 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;



                //Agregar Columnas Caniddatos y Partidos
                foreach (sice_partidos_politicos p in partidos)
                {
                    //Agregar Imagen del Partido
                    rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                    hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                    Left = 3 + (float)((double)rango.Left);
                    Top = (float)((double)rango.Top);

                    hoja.Shapes.AddPicture(rutaImagen + p.img_par + ".jpg", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                    hoja.Cells[fila, columnaInicial] = p.siglas_par;
                    hoja.Cells[fila, columnaInicial].Font.Size = 10;
                    columnaInicial++; columnaLetra++; widths.Add(9.57);
                }
                //Agregar columnas adicionales

                //Imagen no registrados
                rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
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

                hoja.Cells[fila - 3, columnaInicial] = "Votación Total Emitida";
                hoja.Cells[fila - 3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila - 3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                hoja.Cells[fila, columnaInicial] = "TOTAL"; columnaInicial++; columnaLetra++; widths.Add(8.57);

                hoja.Cells[fila, columnaInicial] = "L. Nominal"; columnaInicial++; columnaLetra++; widths.Add(10);
                hoja.Cells[fila, columnaInicial] = "%"; widths.Add(10);
                hoja.Cells[fila - 3, columnaInicial] = "Lista Nominal y Porcentaje de Participación Ciudadana";
                hoja.Cells[fila - 3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila - 3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial - 1], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                //Colores de Fondo
                rango = hoja.Range["A" + fila, "D" + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                //Colores de Fondo Partido
                rango = hoja.Range["F" + fila, columnaLetra.ToString() + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(38)))), ((int)(((byte)(36))))));
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
        private char CrearEncabezadosTotalesRPSP(int fila, ref Excel._Worksheet hoja, List<VotosSeccion> vSeccion, List<sice_partidos_politicos> partidos, int distrito, int columnaInicial = 1)
        {
            try
            {
                Excel.Range rango;
                Excel.Range rangoTitutlo;
                float Left = 0;
                float Top = 0;
                const float ImageSize = 42; //Tamaño Imagen Partidos
                string rutaImagen = System.AppDomain.CurrentDomain.BaseDirectory + "imagenes\\";
                List<double> widths = new List<double>();
                char columnaLetra = 'A';

                //Agregar encabezados
                hoja.Cells[fila - 3, columnaInicial] = "RESULTADOS TOTALES";
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial + 2]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                hoja.Cells[fila, columnaInicial] = "DISTRITO"; columnaInicial++; columnaLetra++;
                hoja.Cells[fila, columnaInicial] = "Secciones"; columnaInicial++; columnaLetra++;
                hoja.Cells[fila, columnaInicial] = "Casillas"; columnaInicial++; columnaLetra++;

                hoja.Cells[fila, columnaInicial] = "Diferencia entre 1° y 2° Lugar"; columnaInicial++; columnaLetra++; widths.Add(12.29);
                hoja.Cells[fila, columnaInicial - 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila, columnaInicial - 1].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila, columnaInicial - 1], hoja.Cells[fila - 3, columnaInicial]].Merge();
                hoja.Cells[fila, columnaInicial - 1].WrapText = true;
                hoja.Cells[fila, columnaInicial - 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter; columnaInicial++; columnaLetra++;



                //Agregar Columnas Caniddatos y Partidos
                foreach (sice_partidos_politicos p in partidos)
                {
                    //Agregar Imagen del Partido
                    rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                    hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                    Left = 3 + (float)((double)rango.Left);
                    Top = (float)((double)rango.Top);

                    hoja.Shapes.AddPicture(rutaImagen + p.img_par + ".jpg", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, ImageSize, ImageSize);
                    hoja.Cells[fila, columnaInicial] = p.siglas_par;
                    hoja.Cells[fila, columnaInicial].Font.Size = 10;
                    columnaInicial++; columnaLetra++; widths.Add(9.57);
                }
                //Agregar columnas adicionales

                //Imagen no registrados
                rango = (Microsoft.Office.Interop.Excel.Range)hoja.Cells[fila - 3, columnaInicial];
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
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

                hoja.Cells[fila - 3, columnaInicial] = "Votación Total Emitida";
                hoja.Cells[fila - 3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila - 3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                hoja.Cells[fila, columnaInicial] = "TOTAL"; columnaInicial++; columnaLetra++; widths.Add(8.57);

                hoja.Cells[fila, columnaInicial] = "L. Nominal"; columnaInicial++; columnaLetra++; widths.Add(10);
                hoja.Cells[fila, columnaInicial] = "%"; widths.Add(10);
                hoja.Cells[fila - 3, columnaInicial] = "Lista Nominal y Porcentaje de Participación Ciudadana";
                hoja.Cells[fila - 3, columnaInicial].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                hoja.Cells[fila - 3, columnaInicial].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hoja.Range[hoja.Cells[fila - 3, columnaInicial - 1], hoja.Cells[fila - 1, columnaInicial]].Merge();
                hoja.Cells[fila - 3, columnaInicial].WrapText = true;
                hoja.Cells[fila - 3, columnaInicial].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                //Colores de Fondo
                rango = hoja.Range["A" + fila, "D" + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(149)))), ((int)(((byte)(90))))));
                rango.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);

                //Colores de Fondo Partido
                rango = hoja.Range["F" + fila, columnaLetra.ToString() + fila];
                rango.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(38)))), ((int)(((byte)(36))))));
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
                //int cont = 1;
                //foreach (int widh in widths)
                //{
                //    rango = hoja.Columns[cont];
                //    rango.ColumnWidth = widh;
                //    cont++;
                //}
                return columnaLetra++;
            }
            catch (Exception E)
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
        public string tipo { get; set; }
    }

    public class TempVotosEstatus
    {
        public int id { get; set; }
        public Nullable<int> id_candidato { get; set; }
        public Nullable<int> id_casilla { get; set; }
        public Nullable<int> votos { get; set; }
        public string tipo { get; set; }
        public Nullable<int> importado { get; set; }
        public Nullable<int> estatus { get; set; }
        public string reserva { get; set; }
    }
    public class PorcentajePartido
    {
        public Nullable<int> id_partido { get; set; }
        public double porcentaje { get; set; }
        public double votos { get; set; }
        public string tipo { get; set; }
        public int prelacion { get; set; }

    }

    public class DetallesComputos
    {
        public int id_distrito { get; set; }
        public int total_recuento { get; set; }
        public int total_capturado { get; set; }
        public int total_reserva { get; set; }
        public int total_actas { get; set; }
        public int total_no_conta { get; set; }

    }
}
