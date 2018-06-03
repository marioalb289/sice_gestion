using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema.DataModel;
using System.Transactions;

namespace Sistema.Generales
{
    public class RegistroActasGenerales
    {
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
        public List<SeccionCasilla> ListaSescciones()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
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

        public sice_ar_documentos BuscarActaAsignada()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    sice_ar_documentos documento = (from doc in contexto.sice_ar_documentos join asig in contexto.sice_ar_asignacion on doc.id equals asig.id_documento where doc.estatus == "OCUPADO" && asig.id_usuario == LoginInfo.id_usuario && doc.filtro <= 2 select doc).FirstOrDefault();                    
                    return documento;
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }
        public DocumentoReserva BuscarActaAsignadaRevision()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    DocumentoReserva docReserva = null;
                    sice_ar_documentos d = (from doc in contexto.sice_ar_documentos join asig in contexto.sice_ar_asignacion on doc.id equals asig.id_documento where doc.estatus == "OCUPADO" && asig.id_usuario == LoginInfo.id_usuario && doc.filtro == 3 select doc).FirstOrDefault();
                    if (d != null)
                    {
                        sice_ar_reserva reserva = (from r in contexto.sice_ar_reserva where r.id_documento == d.id && r.tipo_reserva != "ATENDIDO" select r).FirstOrDefault();
                        if (reserva != null)
                        {
                            docReserva = new DocumentoReserva
                            {
                                id = d.id,
                                nombre = d.nombre,
                                ruta = d.ruta,
                                estatus = d.estatus,
                                filtro = d.filtro,
                                estatus_filtro1 = d.estatus_filtro1,
                                estatus_filtro2 = d.estatus_filtro2,
                                estatus_filtro3 = d.estatus_filtro3,
                                estatus_revisor = d.estatus_revisor,
                                estatus_cotejador = d.estatus_cotejador,
                                id_casilla = d.id_casilla,
                                create_at = d.create_at,
                                updated_at = d.updated_at,
                                tipo_reserva = reserva.tipo_reserva

                            };
                        }
                        
                    }
                    return docReserva;
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }
        public sice_ar_documentos BuscarActaAsignadaCotejo()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    sice_ar_documentos d = null;
                    d = (from doc in contexto.sice_ar_documentos join asig in contexto.sice_ar_asignacion on doc.id equals asig.id_documento where doc.estatus == "OCUPADO" && asig.id_usuario == LoginInfo.id_usuario && doc.filtro == 4 select doc).FirstOrDefault();
                    return d;
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public CasillaInfo BuscarCasilla(int id_casilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {

                    CasillaInfo casilla = null;
                    casilla = (from c in contexto.sice_casillas
                               join m in contexto.sice_municipios on c.id_municipio equals m.id
                               join d in contexto.sice_distritos_locales on c.id_distrito_local equals d.id
                               where c.id == id_casilla
                               select new CasillaInfo {
                                   id = c.id,
                                   seccion = (int)c.seccion,
                                   casilla = c.tipo_casilla,
                                    id_distrito = (int)c.id_distrito_local,
                                    id_municipio = (int)c.id_municipio,
                                    distrito = d.distrito,
                                    municipio = m.municipio
                     }).FirstOrDefault();
                    return casilla;


                }

            }
            catch (Exception E)
            { throw E; }
        }

        public int verificarCasillaValida(int id_casilla)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    sice_ar_documentos casilla = (from doc in contexto.sice_ar_documentos where doc.id_casilla == id_casilla && (doc.estatus == "VALIDO" || doc.estatus == "COTEJO") select doc).FirstOrDefault();
                    if(casilla != null)
                    {
                        if(casilla.estatus=="COTEJO")
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

        public sice_ar_documentos getDocumentos(int id_documento)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    return (from p in contexto.sice_ar_documentos where p.id == id_documento select p).FirstOrDefault();
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
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    return (from p in contexto.sice_ar_documentos where p.id_casilla == id_casilla select p).FirstOrDefault();
                    //return contexto.sice_casillas.Select(x => new SeccionCasilla { id = x.id, seccion = (int)x.seccion, casilla = (string)x.tipo_casilla }).ToList();
                }

            }
            catch (Exception E)
            { throw E; }

        }

        public List<sice_ar_documentos> ListaDocumentos()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    return (from p in contexto.sice_ar_documentos select p).ToList();
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
                        "JOIN sice_candidaturas CD ON CD.id = C.fk_cargo AND CD.titular = 1 " + //"AND CD.id_distrito =" + distrito +
                        "JOIN sice_partidos_politicos P ON P.id = C.fk_partido";
                    return contexto.Database.SqlQuery<Candidatos>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }
        public List<CandidatosVotos> ListaResultadosCasilla(int casilla,string tabla = "")
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
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
                        "FROM " +tabla +" V " +
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

        public List<CandidatosVotos> ListaResultadosCasillaRevision(int id_documento)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
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
                        "FROM sice_ar_votos_valida1 V " +
                        "LEFT JOIN sice_candidatos C ON C.id = V.id_candidato " +
                        "LEFT JOIN sice_candidaturas CD ON CD.id = C.fk_cargo AND CD.titular = 1 " + //"AND CD.id_distrito =" + distrito +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = C.fk_partido " +
                        "WHERE V.id_documento = " + id_documento + " " +
                        "ORDER BY id_candidato DESC";
                    return contexto.Database.SqlQuery<CandidatosVotos>(consulta).ToList();
                }

            }
            catch (Exception E)
            { throw E; }
        }

        public bool EnviarRevision(int id_documento, string motivo, int? id_casilla = null, int cotejo_failo = 0)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        //Enviar a Revision
                        sice_ar_documentos doc = (from d in contexto.sice_ar_documentos where d.id == id_documento select d).FirstOrDefault();
                        if (doc != null)
                        {
                            doc.estatus = "REVISION";
                            if (cotejo_failo != 0)
                                doc.estatus_cotejador = 1;

                            sice_ar_reserva revision = new sice_ar_reserva();
                            revision.id_casilla = id_casilla;
                            revision.id_documento = id_documento;
                            revision.tipo_reserva = motivo;
                            contexto.sice_ar_reserva.Add(revision);

                            contexto.SaveChanges();

                            TransactionContexto.Complete();

                            return true;
                        }
                        return false;
                    }                                            
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public bool RechazarDocumento(int id_documento)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    //Enviar a Revision
                    sice_ar_documentos doc = (from d in contexto.sice_ar_documentos where d.id == id_documento select d).FirstOrDefault();
                    List<sice_ar_reserva> reserva = (from r in contexto.sice_ar_reserva where r.id_documento == id_documento select r).ToList();
                    if (doc != null)
                    {
                        doc.estatus = "CANCELADO";

                        contexto.SaveChanges();

                        return true;
                    }
                    if(reserva.Count > 0)
                    {
                        reserva.ForEach(a => a.tipo_reserva = "ATENDIDO");
                        contexto.SaveChanges();
                    }
                    return false;

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public sice_ar_documentos TomarCasilla()
        {
            try
            {
                //Buscar que el arcivo no se encuentre ya registrado
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        DateTime localDate = DateTime.Now;

                        string consulta =
                            "SELECT " +
	                            "doc.* " +
                            "FROM sice_ar_documentos doc " +
                            "LEFT JOIN sice_ar_asignacion asg ON asg.id_documento = doc.id " +
                            "WHERE " +
	                            "(doc.estatus = 'LIBRE' " +
		                            "AND asg.id_documento NOT IN ( " +
			                            "SELECT tempAsg.id_documento " +
			                            "FROM sice_ar_asignacion tempAsg " +
			                            "WHERE tempAsg.id_usuario =  " + LoginInfo.id_usuario + " ) " +
	                            ") " +
                            "OR ( " +
	                            "doc.estatus = 'LIBRE' " +
	                            "AND asg.id_documento IS NULL ) ";
                        sice_ar_documentos doc = contexto.Database.SqlQuery<sice_ar_documentos>(consulta).FirstOrDefault();
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
                            newAsig2.filtro = doc.filtro;
                            contexto.sice_ar_asignacion.Add(newAsig2);
                            contexto.SaveChanges();
                            TransactionContexto.Complete();
                            return doc;
                            
                        }
                        return doc;

                    }
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        public DocumentoReserva TomarActaRevision()
        {
            try
            {
                //Buscar que el arcivo no se encuentre ya registrado
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        DocumentoReserva doc = null;
                        DateTime localDate = DateTime.Now;
                        doc = (from d in contexto.sice_ar_documentos join r in contexto.sice_ar_reserva on d.id equals r.id_documento where d.estatus == "REVISION" && r.tipo_reserva != "ATENDIDO" select new DocumentoReserva {
                            id = d.id,
                            nombre = d.nombre,
                            ruta = d.ruta,
                            estatus = d.estatus,
                            filtro = d.filtro,
                            estatus_filtro1 = d.estatus_filtro1,
                            estatus_filtro2 = d.estatus_filtro2,
                            estatus_filtro3 = d.estatus_filtro3,
                            estatus_revisor = d.estatus_revisor,
                            estatus_cotejador = d.estatus_cotejador,
                            id_casilla = d.id_casilla,
                            create_at = d.create_at,
                            updated_at = d.updated_at,
                            tipo_reserva = r.tipo_reserva

                        }).FirstOrDefault();
                        if (doc != null)
                        {
                            sice_ar_documentos tmp = (from d in contexto.sice_ar_documentos where d.id == doc.id select d).FirstOrDefault();
                            //Asignar
                            tmp.estatus = "OCUPADO"; ;
                            tmp.updated_at = localDate;
                            tmp.filtro = 3; //filtro 3 revision
                            contexto.SaveChanges();


                            sice_ar_asignacion asig = (from asg in contexto.sice_ar_asignacion where asg.id_documento == doc.id && asg.filtro == 3 select asg).FirstOrDefault();
                            if(asig == null)
                            {
                                sice_ar_asignacion newAsig2 = new sice_ar_asignacion();
                                newAsig2.id_documento = doc.id;
                                newAsig2.id_usuario = LoginInfo.id_usuario;
                                newAsig2.filtro = 3; //filtro 3 revision
                                contexto.sice_ar_asignacion.Add(newAsig2);
                                contexto.SaveChanges();
                            }
                            else
                            {
                                asig.id_usuario = LoginInfo.id_usuario;
                                contexto.SaveChanges();

                            }
                            
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
        public sice_ar_documentos TomarActaCotejo()
        {
            try
            {
                //Buscar que el arcivo no se encuentre ya registrado
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        sice_ar_documentos doc = null;
                        DateTime localDate = DateTime.Now;
                        doc = (from d in contexto.sice_ar_documentos where d.estatus == "COTEJO" && d.id_casilla != null select d).FirstOrDefault();
                        if (doc != null)
                        {
                            //Asignar
                            doc.estatus = "OCUPADO"; ;
                            doc.updated_at = localDate;
                            doc.filtro = 4; //filtro 4 cotejo
                            contexto.SaveChanges();


                            sice_ar_asignacion asig = (from asg in contexto.sice_ar_asignacion where asg.id_documento == doc.id && asg.filtro == 4 select asg).FirstOrDefault();
                            if (asig == null)
                            {
                                sice_ar_asignacion newAsig2 = new sice_ar_asignacion();
                                newAsig2.id_documento = doc.id;
                                newAsig2.id_usuario = LoginInfo.id_usuario;
                                newAsig2.filtro = 4; //filtro 3 revision
                                contexto.sice_ar_asignacion.Add(newAsig2);
                                contexto.SaveChanges();
                            }
                            else
                            {
                                asig.id_usuario = LoginInfo.id_usuario;
                                contexto.SaveChanges();

                            }

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

        public int guardarDatosVotos(List<sice_ar_votos> listaVotos,int id_documento,int id_casilla, int totalCandidatos)
        {
            
            using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
            {
                using (var TransactionContexto = new TransactionScope())
                {
                    try
                    {
                        int res = 1; //1 datos guardados correctamente 2 casilla enviada a revision 3 casilla validad correctamente
                        sice_ar_documentos doc = (from p in contexto.sice_ar_documentos where p.id == id_documento select p).FirstOrDefault();                       
                        string consulta = "SELECT * "+
                            "FROM "+
                            "sice_ar_votos_valida1 v1 "+
                            "JOIN sice_ar_documentos doc ON doc.id = v1.id_documento "+
                            "WHERE "+
                            "doc.estatus NOT IN('VALIDO', 'REVISION','COTEJO','CANCELADO') " +
                            "AND v1.id_casilla = " +id_casilla;
                        sice_ar_votos_valida1 v1Temp = contexto.Database.SqlQuery<sice_ar_votos_valida1>(consulta).FirstOrDefault();
                        if (v1Temp != null && doc.id != v1Temp.id_documento)
                        {
                            //Marcar oficio como duplicado y enviar a revision
                            //Enviar a Revision
                            sice_ar_reserva revision = new sice_ar_reserva();
                            revision.id_casilla = id_casilla;
                            revision.id_documento = doc.id;
                            revision.tipo_reserva = "DUPLICADO";
                            contexto.sice_ar_reserva.Add(revision);

                            contexto.SaveChanges();

                            doc.estatus = "REVISION";

                            contexto.SaveChanges();

                            TransactionContexto.Complete();
                            return 4;
                            //throw new Exception("Ya existe un documento Asingado a esta casilla y en proceso de validación. El documento actual será enviado a Revisión para su evaluacion");
                        }
                        doc.filtro = doc.filtro + 1;
                        switch (doc.filtro)
                        {
                            case 1:
                                sice_ar_votos_valida1 v1 = new sice_ar_votos_valida1();
                                foreach (sice_ar_votos voto in listaVotos)
                                {
                                    v1.id_candidato = voto.id_candidato;
                                    v1.id_casilla = voto.id_casilla;
                                    v1.tipo = voto.tipo;
                                    v1.votos = voto.votos;
                                    v1.id_documento = doc.id;
                                    contexto.sice_ar_votos_valida1.Add(v1);
                                    contexto.SaveChanges();
                                }
                                break;
                            case 2:
                                sice_ar_votos_valida2 v2 = new sice_ar_votos_valida2();
                                foreach (sice_ar_votos voto in listaVotos)
                                {
                                    v2.id_candidato = voto.id_candidato;
                                    v2.id_casilla = voto.id_casilla;
                                    v2.tipo = voto.tipo;
                                    v2.votos = voto.votos;
                                    v2.id_documento = doc.id;
                                    contexto.sice_ar_votos_valida2.Add(v2);
                                    contexto.SaveChanges();
                                }
                                break;
                            case 3:
                                sice_ar_votos_valida3 v3 = new sice_ar_votos_valida3();
                                foreach (sice_ar_votos voto in listaVotos)
                                {
                                    v3.id_candidato = voto.id_candidato;
                                    v3.id_casilla = voto.id_casilla;
                                    v3.tipo = voto.tipo;
                                    v3.votos = voto.votos;
                                    v3.id_documento = doc.id;
                                    contexto.sice_ar_votos_valida3.Add(v3);
                                    contexto.SaveChanges();
                                }
                                break;
                        }
                        //this.LiberarActa(contexto, doc);

                        if((int)doc.filtro == 2)
                        {
                            if( this.ValidarCaptura1(contexto, doc, id_casilla, totalCandidatos) == 1)
                            {
                                res = 3; //Datos validados
                            }
                            else
                            {
                                res = 1;//No pasa validacion pasar al filtro 3
                                this.LiberarActa(contexto, doc);
                            }
                        }
                        else if((int)doc.filtro == 3)
                        {
                            res = this.ValidarCaptura2(contexto, doc, id_casilla, totalCandidatos);
                        }
                        else
                        {
                            res = 1;
                            this.LiberarActa(contexto, doc);
                        }
                        TransactionContexto.Complete();
                        return res;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
            }
        }

        public int ValidarCaptura1(DatabaseContext contexto, sice_ar_documentos documento, int id_casilla, int totalCandidatos)
        {
            try
            {
                int resp = 0;
                List<TempArVotos> v1 = (from p in contexto.sice_ar_votos_valida1
                                        where p.id_documento == documento.id
                                        orderby p.id_candidato ascending, p.tipo ascending
                                        select new TempArVotos()
                                        {
                                            id_candidato = p.id_candidato,
                                            id_casilla = p.id_casilla,
                                            votos = p.votos,
                                            tipo = p.tipo
                                        }).ToList();

                List<TempArVotos> v2 = (from p in contexto.sice_ar_votos_valida2
                                        where p.id_documento == documento.id
                                        orderby p.id_candidato ascending, p.tipo ascending
                                        select new TempArVotos
                                        {
                                            id_candidato = p.id_candidato,
                                            id_casilla = p.id_casilla,
                                            votos = p.votos,
                                            tipo = p.tipo
                                        }).ToList();
                int errs = 0;

                //Validar las tres tablas donde se guardan los datos
                for (int x = 0; x < totalCandidatos + 2; x++)
                {
                    if ((v1.Count == 0 || v2.Count == 0) || (v1.Count != v2.Count))
                    {
                        errs = 1;
                    }
                    else
                    {
                        if (v1[x].id_candidato != v2[x].id_candidato ||
                            v1[x].id_casilla != v2[x].id_casilla ||
                            v1[x].votos != v2[x].votos)
                        {
                            errs = 1;
                        }
                    }                    
                }
                if(errs == 0)
                {
                    //Vaciar Datos a tablas de conteo oficial
                    sice_ar_votos votosNew = new sice_ar_votos();
                    foreach (TempArVotos votos in v1)
                    {
                        votosNew.id_candidato = votos.id_candidato;
                        votosNew.id_casilla = votos.id_casilla;
                        votosNew.tipo = votos.tipo;
                        votosNew.votos = votos.votos;
                        contexto.sice_ar_votos.Add(votosNew);
                        contexto.SaveChanges();
                    }
                    //Modificar el Documento para establecer a que casilla pertence
                    documento.id_casilla = id_casilla;
                    //Marcar que no fue necesario el filtro 3 ni el filtro revisor
                    documento.estatus_filtro3 = 2;
                    documento.estatus_revisor = 2;
                    documento.estatus_cotejador = 2;
                    documento.estatus = "COTEJO";
                    contexto.SaveChanges();

                    resp = 1;

                }

                return resp;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public int ValidarCaptura2(DatabaseContext contexto, sice_ar_documentos documento,int id_casilla, int totalCandidatos)
        {
            try
            {
                int resp = 0;
                List<TempArVotos> v1 = (from p in contexto.sice_ar_votos_valida1
                                        where p.id_documento == documento.id
                                        orderby p.id_candidato ascending, p.tipo ascending
                                        select new TempArVotos()
                                        {
                                            id_candidato = p.id_candidato,
                                            id_casilla = p.id_casilla,
                                            votos = p.votos,
                                            tipo = p.tipo
                                        }).ToList();

                List<TempArVotos> v2 = (from p in contexto.sice_ar_votos_valida2
                                        where p.id_documento == documento.id
                                        orderby p.id_candidato ascending, p.tipo ascending
                                        select new TempArVotos
                                        {
                                            id_candidato = p.id_candidato,
                                            id_casilla = p.id_casilla,
                                            votos = p.votos,
                                            tipo = p.tipo
                                        }).ToList();

                List<TempArVotos> v3 = (from p in contexto.sice_ar_votos_valida3
                                        where p.id_documento == documento.id
                                        orderby p.id_candidato ascending, p.tipo ascending
                                        select new TempArVotos
                                        {
                                            id_candidato = p.id_candidato,
                                            id_casilla = p.id_casilla,
                                            votos = p.votos,
                                            tipo = p.tipo
                                        }).ToList();
                List<int> errs = new List<Int32> { 0, 0, 0 };
                List<int> listSave = new List<Int32>();
                
                //Validar las tres tablas donde se guardan los datos
                for (int x = 0; x < totalCandidatos + 2; x++)
                {
                    if ((v1.Count == 0 || v2.Count == 0)||(v1.Count != v2.Count))                         
                    {
                        errs[0] = 1;
                    }
                    else
                    {
                        if (v1[x].id_candidato != v2[x].id_candidato ||
                            v1[x].id_casilla != v2[x].id_casilla ||
                            v1[x].votos != v2[x].votos)
                        {
                            errs[0] = 1;
                        }
                        else
                        {
                            listSave.Add(1);
                        }
                            
                    }
                    if ((v1.Count == 0 || v3.Count == 0) || (v1.Count != v3.Count))
                    {
                        errs[1] = 1;
                    }
                    else
                    {
                        if (v1[x].id_candidato != v3[x].id_candidato ||
                            v1[x].id_casilla != v3[x].id_casilla ||
                            v1[x].votos != v3[x].votos)
                        {
                            errs[1] = 1;
                        }
                        else
                        {
                            listSave.Add(3);
                        }

                    }
                    if ((v2.Count == 0 || v3.Count == 0) || (v2.Count != v3.Count))
                    {
                        errs[2] = 1;
                    }
                    else
                    {
                        if (v2[x].id_candidato != v3[x].id_candidato ||
                            v2[x].id_casilla != v3[x].id_casilla ||
                            v2[x].votos != v3[x].votos)
                        {
                            errs[2] = 1;
                        }
                        else
                        {
                            listSave.Add(2);
                        }

                    }
                }

                int res = errs.Sum(s => s);

                

                if(res > 2)
                {
                    //Enviar a Revision
                    sice_ar_reserva revision = new sice_ar_reserva();
                    revision.id_casilla = id_casilla;
                    revision.id_documento = documento.id;
                    revision.tipo_reserva = "REVISION";
                    contexto.sice_ar_reserva.Add(revision);

                    contexto.SaveChanges();

                    //marcar aqui como fallo en filtro 3
                    documento.estatus_filtro3 = 1;
                    documento.estatus_filtro1 = 1;
                    documento.estatus_revisor = 2;
                    documento.estatus_cotejador = 2;
                    documento.estatus = "REVISION";

                    contexto.SaveChanges();

                    resp = 2;
                }
                else
                {
                    //Vaciar Datos a tablas de conteo oficial

                    var listSave2 = listSave.GroupBy(s => s).Select(c => new { Key = c.Key, total = c.Count() }).OrderByDescending(c => c.total).ToList();
                    int keytableSave = listSave2[0].Key;

                    List<TempArVotos> tableSave = new List<TempArVotos>();

                    switch (keytableSave)
                    {
                        case 1:
                            tableSave = v1;
                            break;
                        case 2:
                            tableSave = v2;
                            break;
                        case 3:
                            tableSave = v3;
                            break;
                    }

                    sice_ar_votos votosNew = new sice_ar_votos();
                    foreach (TempArVotos votos in tableSave)
                    {
                        votosNew.id_candidato = votos.id_candidato;
                        votosNew.id_casilla = votos.id_casilla;
                        votosNew.tipo = votos.tipo;
                        votosNew.votos = votos.votos;
                        contexto.sice_ar_votos.Add(votosNew);
                        contexto.SaveChanges();
                    }
                    //Modificar el Documento para establecer a que casilla pertence
                    documento.id_casilla = id_casilla;
                    documento.estatus = "COTEJO";
                    //Marcar que no fue necesario el filtro revisor
                    documento.estatus_revisor = 2;
                    documento.estatus_cotejador = 2;
                    contexto.SaveChanges();

                    resp = 3;

                }

                return resp;
            }
            catch(Exception ex)
            {
                throw ex;

            }
        }

        public void LiberarActa(DatabaseContext contexto, sice_ar_documentos documento)
        {
            try
            {
                int filtro_actual = (int)documento.filtro;

                documento.estatus = "LIBRE";
                //Marcar filtro 1 siempre como valido
                if (filtro_actual == 1)
                    documento.estatus_filtro1 = 0;
                //Marcar filtro 2 como no valido, ya que no paso la validacion
                if (filtro_actual == 2)
                    documento.estatus_filtro2 = 1;

                contexto.SaveChanges();


            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        public List<VotosSeccion> ResultadosSeccion(int pageNumber = 0, int pageSize = 0, int id_distrito_local = 0)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
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
                        "ORDER BY C.seccion ASC, RV.id_casilla ASC, RV.id_candidato DESC "+
                        limit;

                    return contexto.Database.SqlQuery<VotosSeccion>(consulta).ToList();
                }


            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public int guardarDatosRevision(List<sice_ar_votos> listaVotosCaptura, int id_documento, int id_casillaCapturar,  int flagResvotos,int id_casillaActual)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        //Verificar que la casilla que estamos queriendo validar no este ya validada
                        int res = 0;
                        res = verificarCasillaValida(id_casillaCapturar);
                        if (res == 1)
                            throw new Exception("Casilla ya Registrada y en estatus: COTEJO");
                        else if(res == 2)
                            throw new Exception("Casilla ya Registrada y en estatus: VALIDO");
                        //Si viene de cotejo, se deben actualizar los datos ya guardados anteriormente por el proceso de validacion de captura en la tabla sice_ar_votos
                        if(flagResvotos == 1)
                        {
                            List<sice_ar_votos> listVotosActual = (from v in contexto.sice_ar_votos where v.id_casilla == id_casillaActual && v.tipo != "NO VALIDO" select v).ToList();
                            int cont = 0;
                            foreach(sice_ar_votos votos in listVotosActual)
                            {
                                if (cont >= listaVotosCaptura.Count)
                                    break;
                                votos.id_casilla = listaVotosCaptura[cont].id_casilla;
                                votos.id_candidato = listaVotosCaptura[cont].id_candidato;
                                votos.votos = listaVotosCaptura[cont].votos;
                                votos.tipo = listaVotosCaptura[cont].tipo;

                                contexto.SaveChanges();
                                cont++;
                            }

                            if(listaVotosCaptura.Count == listVotosActual.Count)
                            {
                                //Terminar
                            }
                            else if(listaVotosCaptura.Count < listVotosActual.Count)
                            {
                                //Significa que lista de votos a capturar es menor que la lista de votos actual, se deben eliminar esos datos
                                for (int x = cont; x < listVotosActual.Count; x++)
                                {
                                    //sice_ar_votos temp = (from v2 in contexto.sice_ar_votos where v2.id == listVotosActual[x].id select v2).FirstOrDefault();
                                    listVotosActual[x].tipo = "NO VALIDO";
                                    contexto.SaveChanges();
                                }
                            }
                            else
                            {
                                //La lista de votos a capturar es mayor que la lista de votos actual
                                for(int x= cont; x < listaVotosCaptura.Count; x++)
                                {
                                    sice_ar_votos tempV = listaVotosCaptura[x];
                                    contexto.sice_ar_votos.Add(tempV);
                                    contexto.SaveChanges();
                                }
                            }

                            //Liberar Documento
                            sice_ar_documentos doc = (from d in contexto.sice_ar_documentos where d.id == id_documento select d).FirstOrDefault();
                            doc.estatus = "COTEJO";
                            doc.estatus_revisor = 0;
                            doc.id_casilla = id_casillaCapturar;

                            sice_ar_reserva reserva = (from r in contexto.sice_ar_reserva where r.id_documento == id_documento && r.tipo_reserva != "ATENDIDO" select r).FirstOrDefault();
                            reserva.tipo_reserva = "ATENDIDO";

                            contexto.SaveChanges();

                            TransactionContexto.Complete();
                            return 1;

                        }
                        //Los datos vienen de no superar el proceso de validacion de captura, o la acta fue considerada como no legble
                        else
                        {
                            foreach (sice_ar_votos votos in listaVotosCaptura)
                            {
                                contexto.sice_ar_votos.Add(votos);
                                contexto.SaveChanges();
                            }

                            //Liberar Documento
                            sice_ar_documentos doc = (from d in contexto.sice_ar_documentos where d.id == id_documento select d).FirstOrDefault();
                            doc.estatus = "COTEJO";
                            doc.estatus_revisor = 0;
                            doc.id_casilla = id_casillaCapturar;

                            sice_ar_reserva reserva = (from r in contexto.sice_ar_reserva where r.id_documento == id_documento select r).FirstOrDefault();
                            reserva.tipo_reserva = "ATENDIDO";

                            contexto.SaveChanges();

                            TransactionContexto.Complete();
                            return 1;

                        }
                    }
                    
                }
                        
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public int guardarActasCotejo(List<sice_ar_votos> listaVotosCaptura, int id_documento, int id_casillaCapturar)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext("MYSQLSERVER"))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        DateTime localDate = DateTime.Now;
                        sice_ar_votos_cotejo v1 = null;
                        foreach (sice_ar_votos voto in listaVotosCaptura)
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
                                contexto.SaveChanges();
                            }

                        }

                        //Liberar Documento
                        sice_ar_documentos doc = (from d in contexto.sice_ar_documentos where d.id == id_documento select d).FirstOrDefault();
                        doc.estatus = "VALIDO";
                        doc.estatus_cotejador= 0;
                        doc.id_casilla = id_casillaCapturar;
                        doc.updated_at = localDate;
                        contexto.SaveChanges();
                        TransactionContexto.Complete();
                        return 1;
                    }
                }
                        
            }
            catch(Exception ex)
            {
                throw ex;
            }
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

    public class CasillaInfo
    {
        public int id { get; set; }
        public int seccion { get; set; }
        public string casilla { get; set; }
        public int id_distrito { get; set; }
        public int id_municipio { get; set; }
        public string distrito { get; set; }
        public string municipio { get; set; }
    }

    public class CandidatosVotos
    {
        public int id { get; set; }        
        public Nullable<int> id_casilla { get; set; }
        public string tipo { get; set; }
        public Nullable<int> votos { get; set; }        
        public Nullable<int> id_candidato { get; set; }
        public string candidato { get; set; }
        public string nombre_candidatura { get; set; }
        public string partido { get; set; }
        public string imagen { get; set; }
        public Nullable<int> partido_local { get; set; }
        public string coalicion { get; set; }
        public string tipo_partido { get; set; }
        

    }
    public class PartidosVotosRP
    {
        public int id { get; set; }
        public Nullable<int> id_casilla { get; set; }
        public string tipo { get; set; }
        public Nullable<int> votos { get; set; }
        public Nullable<int> id_partido { get; set; }
        public string partido { get; set; }
        public string imagen { get; set; }
        public Nullable<int> partido_local { get; set; }
        public string coalicion { get; set; }

    }

    public class Candidatos
    {
        public int id_candidato { get; set; }
        public string candidato { get; set; }
        public string nombre_candidatura { get; set; }
        public string partido { get; set; }
        public int partido_local { get; set; }
        public string imagen { get; set; }
        public int prelacion { get; set; }
        public string coalicion { get; set; }
        public string tipo_partido { get; set; }

    }
    public class CandidatosResultados
    {
        public Nullable<int> id_candidato { get; set; }
        public string candidato { get; set; }
        public string nombre_candidatura { get; set; }
        public string partido { get; set; }
        public Nullable<int> partido_local { get; set; }
        public string imagen { get; set; }
        public int votos { get; set; }
        public string tipo { get; set; }
        public int prelacion { get; set; }

    }

    public class TempArVotos
    {
        public int id { get; set; }
        public Nullable<int> id_candidato { get; set; }
        public Nullable<int> id_casilla { get; set; }
        public Nullable<int> votos { get; set; }
        public string tipo { get; set; }
    }

    public class VotosSeccion
    {
        public int seccion { get; set; }
        public Nullable<int> id_casilla { get; set; }
        public string casilla { get; set; }
        public int lista_nominal { get; set; }
        public Nullable<int> id_candidato { get; set; }        
        public Nullable<int> votos { get; set; }
        public string tipo { get; set; }
        public string candidato { get; set; }
        public string partido { get; set; }
        public Nullable<int> partido_local { get; set; }
        public string imagen { get; set; }
        public int distrito_local { get; set; }
        public string municipio { get; set; }
        public string cabecera_local { get; set; }
        public string estatus { get; set; }
        public string estatus_acta { get; set; }
        public Nullable<int> id_estatus_acta { get; set; }       

    }

    public class DocumentoReserva
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string ruta { get; set; }
        public string estatus { get; set; }
        public Nullable<int> filtro { get; set; }
        public Nullable<int> estatus_filtro1 { get; set; }
        public Nullable<int> estatus_filtro2 { get; set; }
        public Nullable<int> estatus_filtro3 { get; set; }
        public Nullable<int> estatus_revisor { get; set; }
        public Nullable<int> estatus_cotejador { get; set; }
        public Nullable<int> id_casilla { get; set; }
        public System.DateTime create_at { get; set; }
        public System.DateTime updated_at { get; set; }
        public string tipo_reserva { get; set; }
    }
}
