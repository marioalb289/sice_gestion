//using MySql.Data.MySqlClient;
using Sistema.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Transactions;
using System.Configuration;

namespace Sistema.Generales
{
    public class SincronizarDatos
    {
        private static System.Timers.Timer aTimer;
        public SincronizarDatos()
        {            
            SetTimer();
        }

        public void detener()
        {
            try
            {
                aTimer.Dispose();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void SetTimer()
        {
            try
            {
                // Create a timer with a two second interval.
                aTimer = new System.Timers.Timer(Configuracion.TimerDatosReg);
                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                //Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);
                SincronizarRegistroActas();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void SincronizarRegistroActas()
        {
            try
            {
                aTimer.Stop();
                //DateTime fechaInicio = new DateTime(2018, 5, 28, 8, 0, 0);
                //DateTime fechaActual = DateTime.Now;
                //if (fechaActual >= fechaInicio)
                //{
                    //Iniciar Proceso
                    ThreadStart delegado = new ThreadStart(() => ProcesoSincronizarRegistroActas());
                    delegado += () =>
                    {
                        aTimer.Start();
                    };
                    //Creamos la instancia del hilo 
                    Thread hilo = new Thread(delegado) { IsBackground = true };
                    //Iniciamos el hilo 
                    hilo.Start();
                //}
                //else
                //{
                //    aTimer.Start();
                //}

            }
            catch(Exception ex)
            {
                aTimer.Start();
                Console.WriteLine(ex.Message);
            }
        }

        private void ProcesoSincronizarRegistroActas()
        {
            try
            {
                int res = SubirDatosRegistroActas();
                switch (res)
                {
                    case 0:
                        Console.WriteLine("Error al sincornizar Datos");
                        break;
                    case 1:
                        Console.WriteLine("Proceso Terminado correctamente");
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Backup()
        {
            try
            {
                //string constring = ConfigurationManager.ConnectionStrings["MYSQLOCAL"].ToString();
                //string file = "C:\\respaldos222\\backup.sql";
                //using (MySqlConnection conn = new MySqlConnection(constring))
                //{
                //    using (MySqlCommand cmd = new MySqlCommand())
                //    {
                //        using (MySqlBackup mb = new MySqlBackup(cmd))
                //        {
                //            cmd.Connection = conn;
                //            conn.Open();
                //            mb.ExportToFile(file);
                //            conn.Close();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
        }

        public int SubirDatosRegistroActas()
        {
            try
            {
                // Esto se ejecuta en un hilo combinado
                Console.WriteLine("Iniciando Subida Registro de Actas"); // Escribe "tick..."
                //this.Backup();
                //Thread.Sleep(25000);
                List<sice_ar_votos_cotejo> listaLocalVotos = new List<sice_ar_votos_cotejo>();
                List<sice_ar_votos_cotejo_rp> listaLocalVotosRP = new List<sice_ar_votos_cotejo_rp>();
                List<sice_configuracion_recuento> listaConfiguracionRecuento = new List<sice_configuracion_recuento>();
                List<sice_ar_reserva> listaReserva = new List<sice_ar_reserva>();
                List<sice_ar_documentos> listaDocumentos = new List<sice_ar_documentos>();
                List<sice_ar_historico> listaHistorico = new List<sice_ar_historico>();

                using (DatabaseContext contextoLocal = new DatabaseContext("MYSQLOCAL"))
                {
                    listaLocalVotos = (from i in contextoLocal.sice_ar_votos_cotejo where i.importado == 0 && i.estatus == 1 select i).ToList();
                    listaLocalVotosRP = (from i in contextoLocal.sice_ar_votos_cotejo_rp where i.importado == 0 && i.estatus == 1 select i).ToList();
                    listaReserva = (from i in contextoLocal.sice_ar_reserva where i.importado == 0  select i).ToList();
                    listaDocumentos = (from i in contextoLocal.sice_ar_documentos where i.importado_dato == 0 select i).ToList();
                    listaHistorico = (from i in contextoLocal.sice_ar_historico where i.importado == 0 select i).ToList();
                    listaConfiguracionRecuento = (from i in contextoLocal.sice_configuracion_recuento where i.importado == 0 && i.sistema == "RA" select i).ToList();
                }

                using (DatabaseContext contextoServer = new DatabaseContext("MYSQLSERVER"))
                {
                    //using (var TransactionContexto = new TransactionScope())
                    //{
                        foreach (sice_ar_reserva reserva in listaReserva)
                        {
                            int? id_casilla2 = reserva.id_casilla;
                            sice_ar_reserva rc = (from p in contextoServer.sice_ar_reserva where p.id_casilla == id_casilla2 select p).FirstOrDefault();
                            if (rc != null)
                            {
                                rc.id_casilla = reserva.id_casilla;
                                rc.tipo_reserva = reserva.tipo_reserva;
                                rc.id_documento = reserva.id_documento;
                                rc.importado = reserva.importado;
                                rc.id_supuesto = reserva.id_supuesto;
                                rc.create_at = reserva.create_at;
                                rc.updated_at = reserva.updated_at;
                                rc.num_escritos = reserva.num_escritos;
                                rc.boletas_sobrantes = reserva.boletas_sobrantes;
                                rc.personas_votaron = reserva.personas_votaron;
                                rc.num_representantes_votaron = reserva.num_representantes_votaron;
                                rc.votos_sacados = reserva.votos_sacados;
                                rc.casilla_instalada = reserva.casilla_instalada;
                                rc.id_estatus_acta = reserva.id_estatus_acta;
                                rc.id_estatus_paquete = reserva.id_estatus_paquete;
                                rc.id_incidencias = reserva.id_incidencias;
                                rc.inicializada = reserva.inicializada;
                                rc.id_condiciones_paquete = reserva.id_condiciones_paquete;
                                rc.tipo_votacion = reserva.tipo_votacion;
                                rc.grupo_trabajo = reserva.grupo_trabajo;
                                rc.con_cinta = reserva.con_cinta;
                                rc.con_etiqueta = reserva.con_etiqueta;
                            }
                            else
                            {
                                rc = new sice_ar_reserva();
                                rc.id_casilla = reserva.id_casilla;
                                rc.tipo_reserva = reserva.tipo_reserva;
                                rc.id_documento = reserva.id_documento;
                                rc.importado = reserva.importado;
                                rc.id_supuesto = reserva.id_supuesto;
                                rc.create_at = reserva.create_at;
                                rc.updated_at = reserva.updated_at;
                                rc.num_escritos = reserva.num_escritos;
                                rc.boletas_sobrantes = reserva.boletas_sobrantes;
                                rc.personas_votaron = reserva.personas_votaron;
                                rc.num_representantes_votaron = reserva.num_representantes_votaron;
                                rc.votos_sacados = reserva.votos_sacados;
                                rc.casilla_instalada = reserva.casilla_instalada;
                                rc.id_estatus_acta = reserva.id_estatus_acta;
                                rc.id_estatus_paquete = reserva.id_estatus_paquete;
                                rc.id_incidencias = reserva.id_incidencias;
                                rc.inicializada = reserva.inicializada;
                                rc.id_condiciones_paquete = reserva.id_condiciones_paquete;
                                rc.tipo_votacion = reserva.tipo_votacion;
                                rc.grupo_trabajo = reserva.grupo_trabajo;
                                rc.con_cinta = reserva.con_cinta;
                                rc.con_etiqueta = reserva.con_etiqueta;
                                contextoServer.sice_ar_reserva.Add(rc);
                            }
                            contextoServer.SaveChanges();

                        }

                        sice_ar_votos_cotejo v1 = null;
                        foreach (sice_ar_votos_cotejo voto in listaLocalVotos)
                        {
                            if (voto.id_candidato != null)
                            {
                                v1 = (from d in contextoServer.sice_ar_votos_cotejo where d.id_candidato == voto.id_candidato && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    v1 = (from d in contextoServer.sice_ar_votos_cotejo where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    v1 = (from d in contextoServer.sice_ar_votos_cotejo where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }

                            if (v1 != null)
                            {
                                v1.id_candidato = voto.id_candidato;
                                v1.id_casilla = voto.id_casilla;
                                v1.tipo = voto.tipo;
                                v1.votos = voto.votos;
                                v1.importado = 1;
                                v1.estatus = 1;
                                contextoServer.SaveChanges();
                            }
                        }

                        sice_ar_votos_cotejo_rp vrp1 = null;
                        foreach (sice_ar_votos_cotejo_rp voto in listaLocalVotosRP)
                        {
                            if (voto.id_partido != null)
                            {
                                vrp1 = (from d in contextoServer.sice_ar_votos_cotejo_rp where d.id_partido == voto.id_partido && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    vrp1 = (from d in contextoServer.sice_ar_votos_cotejo_rp where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    vrp1 = (from d in contextoServer.sice_ar_votos_cotejo_rp where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }

                            if (vrp1 != null)
                            {
                                vrp1.id_partido = voto.id_partido;
                                vrp1.id_casilla = voto.id_casilla;
                                vrp1.tipo = voto.tipo;
                                vrp1.votos = voto.votos;
                                vrp1.importado = 0;
                                vrp1.estatus = 1;
                                contextoServer.SaveChanges();
                            }
                        }

                        foreach (sice_ar_documentos doc in listaDocumentos)
                        {
                            sice_ar_documentos tempDoc = (from d in contextoServer.sice_ar_documentos where d.nombre == doc.nombre select d).FirstOrDefault();
                            if (tempDoc != null)
                            {
                                tempDoc.id_casilla = doc.id_casilla;
                                tempDoc.estatus = doc.estatus;
                                tempDoc.updated_at = doc.updated_at;
                                tempDoc.identificado = doc.identificado;
                                contextoServer.SaveChanges();
                            }
                        }

                        foreach(sice_configuracion_recuento conf in listaConfiguracionRecuento)
                        {
                            sice_configuracion_recuento tempConf = (from d in contextoServer.sice_configuracion_recuento where d.id_distrito == conf.id_distrito && d.sistema == "RA" select d).FirstOrDefault();
                            if(tempConf != null)
                            {
                                tempConf.grupos_trabajo = conf.grupos_trabajo;
                                tempConf.horas_disponibles = conf.horas_disponibles;
                                tempConf.id_distrito = conf.id_distrito;
                                tempConf.importado = 0;
                                tempConf.inicializado = conf.inicializado;
                                tempConf.puntos_recuento = conf.puntos_recuento;
                                tempConf.sistema = conf.sistema;
                                tempConf.tipo_recuento = conf.tipo_recuento;
                                contextoServer.SaveChanges();
                            }
                            else
                            {
                                tempConf = new sice_configuracion_recuento();
                                tempConf.grupos_trabajo = conf.grupos_trabajo;
                                tempConf.horas_disponibles = conf.horas_disponibles;
                                tempConf.id_distrito = conf.id_distrito;
                                tempConf.importado = 0;
                                tempConf.inicializado = conf.inicializado;
                                tempConf.puntos_recuento = conf.puntos_recuento;
                                tempConf.sistema = conf.sistema;
                                tempConf.tipo_recuento = conf.tipo_recuento;
                                contextoServer.sice_configuracion_recuento.Add(tempConf);
                                contextoServer.SaveChanges();

                            }
                        }

                        foreach (sice_ar_historico hs in listaHistorico)
                        {
                            sice_ar_historico hs2 = new sice_ar_historico();
                            hs2.id_supuesto = hs.id_supuesto;
                            hs2.fecha = hs.fecha;
                            hs2.id_casilla = hs.id_casilla;
                            hs2.importado = hs.importado;
                            contextoServer.sice_ar_historico.Add(hs2);
                            contextoServer.SaveChanges();
                        }

                        //TransactionContexto.Complete();

                    //}
                }

                using (DatabaseContext contextoLocal = new DatabaseContext("MYSQLOCAL"))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        //foreach (sice_ar_reserva reserva in listaReserva)
                        //{
                        //    sice_ar_reserva rc = (from p in contextoLocal.sice_ar_reserva where p.id_casilla == reserva.id_casilla select p).FirstOrDefault();
                        //    if (rc != null)
                        //    {
                        //        rc.importado = 1;
                        //        contextoLocal.SaveChanges();
                        //    }
                        //}
                        sice_ar_votos_cotejo v1 = null;
                        foreach (sice_ar_votos_cotejo voto in listaLocalVotos)
                        {
                            if (voto.id_candidato != null)
                            {
                                v1 = (from d in contextoLocal.sice_ar_votos_cotejo where d.id_candidato == voto.id_candidato && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    v1 = (from d in contextoLocal.sice_ar_votos_cotejo where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    v1 = (from d in contextoLocal.sice_ar_votos_cotejo where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }

                            if (v1 != null)
                            {
                                v1.importado = 1;
                                contextoLocal.SaveChanges();
                            }
                        }

                        sice_ar_votos_cotejo_rp v1rp = null;
                        foreach (sice_ar_votos_cotejo_rp voto in listaLocalVotosRP)
                        {
                            if (voto.id_partido != null)
                            {
                                v1rp = (from d in contextoLocal.sice_ar_votos_cotejo_rp where d.id_partido == voto.id_partido && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    v1rp = (from d in contextoLocal.sice_ar_votos_cotejo_rp where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    v1rp = (from d in contextoLocal.sice_ar_votos_cotejo_rp where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }

                            if (v1rp != null)
                            {
                                v1rp.importado = 1;
                                contextoLocal.SaveChanges();
                            }
                        }

                        foreach(sice_configuracion_recuento conf in listaConfiguracionRecuento)
                        {
                            sice_configuracion_recuento tempConf = (from d in contextoLocal.sice_configuracion_recuento where d.id == conf.id select d).FirstOrDefault();
                            if(tempConf != null)
                            {
                                tempConf.importado = 1;
                                contextoLocal.SaveChanges();
                            }
                        }

                        foreach (sice_ar_documentos doc in listaDocumentos)
                        {
                            sice_ar_documentos tempDoc = (from d in contextoLocal.sice_ar_documentos where d.nombre == doc.nombre select d).FirstOrDefault();
                            if (tempDoc != null)
                            {
                                tempDoc.importado_dato = 1;
                                contextoLocal.SaveChanges();
                            }
                        }
                        foreach (sice_ar_historico hs in listaHistorico)
                        {
                            sice_ar_historico tempHs = (from d in contextoLocal.sice_ar_historico where d.id == hs.id select d).FirstOrDefault();
                            tempHs.importado = 1;
                            contextoLocal.SaveChanges();
                        }
                        TransactionContexto.Complete();
                    }

                }

                //if (listaLocalVotos.Count > 0 && listaReserva.Count > 0)
                //{
                //    using (DatabaseContext contextoServer = new DatabaseContext("MYSQLSERVER"))
                //    {
                //        using (var TransactionContexto = new TransactionScope())
                //        {
                //            foreach (sice_ar_reserva reserva in listaReserva)
                //            {
                //                sice_ar_reserva rc = (from p in contextoServer.sice_ar_reserva where p.id_casilla == reserva.id_casilla select p).FirstOrDefault();
                //                if (rc != null)
                //                {
                //                    rc.tipo_reserva = reserva.tipo_reserva;
                //                    rc.importado = 1;
                //                }
                //                else
                //                {
                //                    rc = new sice_ar_reserva();
                //                    rc.id_casilla = reserva.id_casilla;
                //                    rc.tipo_reserva = reserva.tipo_reserva;
                //                    rc.importado = 1;
                //                    contextoServer.sice_ar_reserva.Add(rc);
                //                }
                //                contextoServer.SaveChanges();
                //            }
                //            sice_ar_votos_cotejo v1 = null;
                //            foreach (sice_ar_votos_cotejo voto in listaLocalVotos)
                //            {
                //                if (voto.id_candidato != null)
                //                {
                //                    v1 = (from d in contextoServer.sice_ar_votos_cotejo where d.id_candidato == voto.id_candidato && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                //                }
                //                else
                //                {
                //                    if (voto.tipo == "NULO")
                //                        v1 = (from d in contextoServer.sice_ar_votos_cotejo where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                //                    else if (voto.tipo == "NO REGISTRADO")
                //                        v1 = (from d in contextoServer.sice_ar_votos_cotejo where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                //                }

                //                if (v1 != null)
                //                {
                //                    v1.id_candidato = voto.id_candidato;
                //                    v1.id_casilla = voto.id_casilla;
                //                    v1.tipo = voto.tipo;
                //                    v1.votos = voto.votos;
                //                    v1.importado = 1;
                //                    v1.estatus = 1;
                //                    contextoServer.SaveChanges();
                //                }
                //            }
                            
                //            TransactionContexto.Complete();
                //        }
                //    }

                //    using (DatabaseContext contextoLocal = new DatabaseContext("MYSQLOCAL"))
                //    {
                //        using (var TransactionContexto = new TransactionScope())
                //        {
                //            foreach (sice_ar_reserva reserva in listaReserva)
                //            {
                //                sice_ar_reserva rc = (from p in contextoLocal.sice_ar_reserva where p.id_casilla == reserva.id_casilla select p).FirstOrDefault();
                //                if (rc != null)
                //                {
                //                    rc.importado = 1;
                //                    contextoLocal.SaveChanges();
                //                }
                //            }
                //            sice_ar_votos_cotejo v1 = null;
                //            foreach (sice_ar_votos_cotejo voto in listaLocalVotos)
                //            {
                //                if (voto.id_candidato != null)
                //                {
                //                    v1 = (from d in contextoLocal.sice_ar_votos_cotejo where d.id_candidato == voto.id_candidato && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                //                }
                //                else
                //                {
                //                    if (voto.tipo == "NULO")
                //                        v1 = (from d in contextoLocal.sice_ar_votos_cotejo where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                //                    else if (voto.tipo == "NO REGISTRADO")
                //                        v1 = (from d in contextoLocal.sice_ar_votos_cotejo where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                //                }

                //                if (v1 != null)
                //                {
                //                    v1.importado = 1;
                //                    contextoLocal.SaveChanges();
                //                }
                //            }                         

                //            TransactionContexto.Complete();
                //        }

                //    }
                //}

                //if (listaDocumentos.Count > 0)
                //{
                //    using (DatabaseContext contextoServer = new DatabaseContext("MYSQLSERVER"))
                //    {
                //        using (var TransactionContexto = new TransactionScope())
                //        {
                //            foreach (sice_ar_documentos doc in listaDocumentos)
                //            {
                //                sice_ar_documentos tempDoc = (from d in contextoServer.sice_ar_documentos where d.nombre == doc.nombre select d).FirstOrDefault();
                //                if (tempDoc != null)
                //                {
                //                    tempDoc.id_casilla = doc.id_casilla;
                //                    tempDoc.estatus = doc.estatus;
                //                    tempDoc.updated_at = doc.updated_at;
                //                    contextoServer.SaveChanges();
                //                }
                //            }
                //            TransactionContexto.Complete();
                //        }
                //    }
                //    using (DatabaseContext contextoLocal = new DatabaseContext("MYSQLOCAL"))
                //    {
                //        using (var TransactionContexto = new TransactionScope())
                //        {
                //            foreach (sice_ar_documentos doc in listaDocumentos)
                //            {
                //                sice_ar_documentos tempDoc = (from d in contextoLocal.sice_ar_documentos where d.nombre == doc.nombre select d).FirstOrDefault();
                //                if (tempDoc != null)
                //                {
                //                    tempDoc.importado_dato = 1;
                //                    contextoLocal.SaveChanges();
                //                }
                //            }
                //            TransactionContexto.Complete();
                //        }
                //    }

                //}

                Console.WriteLine("Sincronizacion completa Registro de Actas");
                return 1;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }            
        }
    }
}
