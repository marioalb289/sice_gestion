using Sistema.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Transactions;

namespace Sistema.Generales
{
    public class SincronizarDatosComputos
    {
        private static System.Timers.Timer aTimer;
        public SincronizarDatosComputos()
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
                aTimer = new System.Timers.Timer(Configuracion.TimerDatosComp);
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
                SincronizarComputosElectorales();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void SincronizarComputosElectorales()
        {
            try
            {
                aTimer.Stop();
                //DateTime fechaInicio = new DateTime(2018, 5, 28, 8, 0, 0);
                //DateTime fechaActual = DateTime.Now;
                //if(fechaActual >= fechaInicio)
                //{
                    //Iniciar Proceso
                    ThreadStart delegado = new ThreadStart(() => ProcesoSincronizarComputos());
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

        private void ProcesoSincronizarComputos()
        {
            try
            {
                int res = SubirDatosComputos();
                switch (res)
                {
                    case 0:
                        Console.WriteLine("Error al sincornizar Datos Computos");
                        break;
                    case 1:
                        Console.WriteLine("Proceso Terminado correctamente Computos");
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public int SubirDatosComputos()
        {
            try
            {
                // Esto se ejecuta en un hilo combinado
                Console.WriteLine("Iniciando Subida Computos Electorales"); // Escribe "tick..."
                //Thread.Sleep(25000);
                List<sice_votos> listaLocalVotos = new List<sice_votos>();
                List<sice_votos_rp> listaLocalVotosRP = new List<sice_votos_rp>();
                List<sice_configuracion_recuento> listaConfiguracionRecuento = new List<sice_configuracion_recuento>();
                List<sice_reserva_captura> listaReserva = new List<sice_reserva_captura>();
                List<sice_historico> listaHistorico = new List<sice_historico>();

                using (DatabaseContext contextoLocal = new DatabaseContext("MYSQLOCAL"))
                {
                    listaLocalVotos = (from i in contextoLocal.sice_votos where i.importado == 0 && i.estatus == 1 select i).ToList();
                    listaLocalVotosRP = (from i in contextoLocal.sice_votos_rp where i.importado == 0 && i.estatus == 1 select i).ToList();
                    listaReserva = (from i in contextoLocal.sice_reserva_captura where i.importado == 0 select i).ToList();
                    listaHistorico = (from i in contextoLocal.sice_historico where i.importado == 0 select i).ToList();
                    listaConfiguracionRecuento = (from i in contextoLocal.sice_configuracion_recuento where i.importado == 0 && i.sistema == "SICE" select i).ToList();
                }

                using (DatabaseContext contextoServer = new DatabaseContext("MYSQLSERVER"))
                {
                    //using (var TransactionContexto = new TransactionScope())
                    //{
                        foreach (sice_reserva_captura reserva in listaReserva)
                        {
                            int? id_casilla2 = reserva.id_casilla;
                            sice_reserva_captura rc = (from p in contextoServer.sice_reserva_captura where p.id_casilla == id_casilla2 select p).FirstOrDefault();
                            if (rc != null)
                            {
                                rc.id_casilla = reserva.id_casilla;
                                rc.tipo_reserva = reserva.tipo_reserva;
                                rc.tipo_votacion = reserva.tipo_votacion;
                                rc.id_supuesto = reserva.id_supuesto;
                                rc.personas_votaron = reserva.personas_votaron;
                                rc.num_representantes_votaron = reserva.num_representantes_votaron;
                                rc.num_escritos = reserva.num_escritos;
                                rc.votos_sacados = reserva.votos_sacados;
                                rc.boletas_sobrantes = reserva.boletas_sobrantes;
                                rc.casilla_instalada = reserva.casilla_instalada;
                                rc.id_estatus_acta = reserva.id_estatus_acta;
                                rc.id_estatus_paquete = reserva.id_estatus_paquete;
                                rc.id_condiciones_paquete = reserva.id_condiciones_paquete;
                                rc.id_incidencias = reserva.id_incidencias;
                                rc.inicializada = reserva.inicializada;
                                rc.importado = reserva.importado;
                                rc.create_at = reserva.create_at;
                                rc.updated_at = reserva.updated_at;
                                rc.grupo_trabajo = reserva.grupo_trabajo;
                                rc.votos_reservados = reserva.votos_reservados;
                                rc.con_cinta = reserva.con_cinta;
                                rc.con_etiqueta = reserva.con_etiqueta;
                            }
                            else
                            {
                                rc = new sice_reserva_captura();
                                rc.id_casilla = reserva.id_casilla;
                                rc.tipo_reserva = reserva.tipo_reserva;
                                rc.tipo_votacion = reserva.tipo_votacion;
                                rc.id_supuesto = reserva.id_supuesto;
                                rc.personas_votaron = reserva.personas_votaron;
                                rc.num_representantes_votaron = reserva.num_representantes_votaron;
                                rc.num_escritos = reserva.num_escritos;
                                rc.votos_sacados = reserva.votos_sacados;
                                rc.boletas_sobrantes = reserva.boletas_sobrantes;
                                rc.casilla_instalada = reserva.casilla_instalada;
                                rc.id_estatus_acta = reserva.id_estatus_acta;
                                rc.id_estatus_paquete = reserva.id_estatus_paquete;
                                rc.id_condiciones_paquete = reserva.id_condiciones_paquete;
                                rc.id_incidencias = reserva.id_incidencias;
                                rc.inicializada = reserva.inicializada;
                                rc.importado = reserva.importado;
                                rc.create_at = reserva.create_at;
                                rc.updated_at = reserva.updated_at;
                                rc.grupo_trabajo = reserva.grupo_trabajo;
                                rc.votos_reservados = reserva.votos_reservados;
                                rc.con_cinta = reserva.con_cinta;
                                rc.con_etiqueta = reserva.con_etiqueta;
                                contextoServer.sice_reserva_captura.Add(rc);
                            }
                            contextoServer.SaveChanges();

                        }

                        sice_votos v1 = null;
                        foreach (sice_votos voto in listaLocalVotos)
                        {
                            if (voto.id_candidato != null)
                            {
                                v1 = (from d in contextoServer.sice_votos where d.id_candidato == voto.id_candidato && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    v1 = (from d in contextoServer.sice_votos where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    v1 = (from d in contextoServer.sice_votos where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
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

                        sice_votos_rp vrp1 = null;
                        foreach (sice_votos_rp voto in listaLocalVotosRP)
                        {
                            if (voto.id_partido != null)
                            {
                                vrp1 = (from d in contextoServer.sice_votos_rp where d.id_partido == voto.id_partido && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    vrp1 = (from d in contextoServer.sice_votos_rp where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    vrp1 = (from d in contextoServer.sice_votos_rp where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
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

                        foreach (sice_configuracion_recuento conf in listaConfiguracionRecuento)
                        {
                            sice_configuracion_recuento tempConf = (from d in contextoServer.sice_configuracion_recuento where d.id_distrito == conf.id_distrito && d.sistema == "SICE" select d).FirstOrDefault();
                            if (tempConf != null)
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

                        foreach (sice_historico hs in listaHistorico)
                        {
                            sice_ar_historico hs2 = new sice_ar_historico();
                            hs2.id_supuesto = hs.id_supuesto;
                            hs2.fecha = hs.fecha;
                            hs2.id_casilla = hs.id_casilla;
                            hs2.importado = hs.importado;
                            contextoServer.sice_ar_historico.Add(hs2);
                            contextoServer.SaveChanges();
                        }

                     //   TransactionContexto.Complete();

                   // }
                }

                using (DatabaseContext contextoLocal = new DatabaseContext("MYSQLOCAL"))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
                        //foreach (sice_reserva_captura reserva in listaReserva)
                        //{
                        //    sice_reserva_captura rc = (from p in contextoLocal.sice_reserva_captura where p.id_casilla == reserva.id_casilla select p).FirstOrDefault();
                        //    if (rc != null)
                        //    {
                        //        rc.importado = 1;
                        //        contextoLocal.SaveChanges();
                        //    }
                        //}
                        sice_votos v1 = null;
                        foreach (sice_votos voto in listaLocalVotos)
                        {
                            if (voto.id_candidato != null)
                            {
                                v1 = (from d in contextoLocal.sice_votos where d.id_candidato == voto.id_candidato && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    v1 = (from d in contextoLocal.sice_votos where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    v1 = (from d in contextoLocal.sice_votos where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }

                            if (v1 != null)
                            {
                                v1.importado = 1;
                                contextoLocal.SaveChanges();
                            }
                        }
                        sice_votos_rp v1rp = null;
                        foreach (sice_votos_rp voto in listaLocalVotosRP)
                        {
                            if (voto.id_partido != null)
                            {
                                v1rp = (from d in contextoLocal.sice_votos_rp where d.id_partido == voto.id_partido && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }
                            else
                            {
                                if (voto.tipo == "NULO")
                                    v1rp = (from d in contextoLocal.sice_votos_rp where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                                else if (voto.tipo == "NO REGISTRADO")
                                    v1rp = (from d in contextoLocal.sice_votos_rp where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                            }

                            if (v1rp != null)
                            {
                                v1rp.importado = 1;
                                contextoLocal.SaveChanges();
                            }
                        }

                        foreach (sice_configuracion_recuento conf in listaConfiguracionRecuento)
                        {
                            sice_configuracion_recuento tempConf = (from d in contextoLocal.sice_configuracion_recuento where d.id == conf.id select d).FirstOrDefault();
                            if (tempConf != null)
                            {
                                tempConf.importado = 1;
                                contextoLocal.SaveChanges();
                            }
                        }
                        foreach (sice_historico hs in listaHistorico)
                        {
                            sice_historico tempHs = (from d in contextoLocal.sice_historico where d.id == hs.id select d).FirstOrDefault();
                            tempHs.importado = 1;
                            contextoLocal.SaveChanges();
                        }
                        TransactionContexto.Complete();
                    }

                }
                //if(listaLocalVotos.Count > 0 || listaReserva.Count > 0)
                //{
                //    using (DatabaseContext contextoServer = new DatabaseContext("MYSQLSERVER"))
                //    {
                //        using (var TransactionContexto = new TransactionScope())
                //        {
                //            foreach (sice_reserva_captura reserva in listaReserva)
                //            {
                //                sice_reserva_captura rc = (from p in contextoServer.sice_reserva_captura where p.id_casilla == reserva.id_casilla select p).FirstOrDefault();
                //                if (rc != null)
                //                {
                //                    rc.tipo_reserva = reserva.tipo_reserva;
                //                    rc.importado = 1;
                //                }
                //                else
                //                {
                //                    rc = new sice_reserva_captura();
                //                    rc.id_casilla = reserva.id_casilla;
                //                    rc.tipo_reserva = reserva.tipo_reserva;
                //                    rc.importado = 1;
                //                    contextoServer.sice_reserva_captura.Add(rc);
                //                }
                //                contextoServer.SaveChanges();
                //            }
                //            sice_votos v1 = null;
                //            foreach (sice_votos voto in listaLocalVotos)
                //            {
                //                if (voto.id_candidato != null)
                //                {
                //                    v1 = (from d in contextoServer.sice_votos where d.id_candidato == voto.id_candidato && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                //                }
                //                else
                //                {
                //                    if (voto.tipo == "NULO")
                //                        v1 = (from d in contextoServer.sice_votos where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                //                    else if (voto.tipo == "NO REGISTRADO")
                //                        v1 = (from d in contextoServer.sice_votos where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
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
                //            foreach (sice_reserva_captura reserva in listaReserva)
                //            {
                //                sice_reserva_captura rc = (from p in contextoLocal.sice_reserva_captura where p.id_casilla == reserva.id_casilla select p).FirstOrDefault();
                //                if (rc != null)
                //                {
                //                    rc.importado = 1;
                //                    contextoLocal.SaveChanges();
                //                }
                //            }
                //            sice_votos v1 = null;
                //            foreach (sice_votos voto in listaLocalVotos)
                //            {
                //                if (voto.id_candidato != null)
                //                {
                //                    v1 = (from d in contextoLocal.sice_votos where d.id_candidato == voto.id_candidato && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                //                }
                //                else
                //                {
                //                    if (voto.tipo == "NULO")
                //                        v1 = (from d in contextoLocal.sice_votos where d.tipo == "NULO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
                //                    else if (voto.tipo == "NO REGISTRADO")
                //                        v1 = (from d in contextoLocal.sice_votos where d.tipo == "NO REGISTRADO" && d.id_casilla == voto.id_casilla select d).FirstOrDefault();
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
                Console.WriteLine("Sincronizacion completa Computos");
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
