using System;
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
    public class ComputosElectoralesGenerales
    {
        private string con = "MYSQLOCAL";

        public ComputosElectoralesGenerales()
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
                            "RV.votos," +
                            "RV.tipo," +
                            "CONCAT(CND.nombre, ' ', CND.apellido_paterno, ' ', CND.apellido_materno) as candidato," +
                            "P.siglas_par as partido," +
                            "P.img_par as imagen," +
                            "C.id_distrito_local as distrito_local," +
                            "M.municipio," +
                            "M2.municipio AS cabecera_local, " +
                            "RC.tipo_reserva as estatus " +
                        "FROM sice_votos RV " +
                        "LEFT JOIN sice_reserva_captura RC ON RC.id_casilla = RV.id_casilla " +
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

        public List<SeccionCasillaConsecutivo> ListaSescciones()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
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
                using (DatabaseContext contexto = new DatabaseContext(con))
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

        public int CasillaReserva(int id_casilla,string motivo)
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
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
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    using (var TransactionContexto = new TransactionScope())
                    {
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
                                v1.votos = voto.votos;
                                v1.estatus = 1;
                                v1.importado = 0;
                                contexto.SaveChanges();
                            }
                        }

                        sice_reserva_captura rc = (from p in contexto.sice_reserva_captura where p.id_casilla == id_casilla select p).FirstOrDefault();
                        if(rc != null)
                        {
                            rc.tipo_reserva = "CAPTURADA";
                            rc.importado = 0;
                        }
                        else
                        {
                            rc = new sice_reserva_captura();
                            rc.id_casilla = id_casilla;
                            rc.tipo_reserva = "CAPTURADA";
                            rc.importado = 0;
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

        public int generarExcel(SaveFileDialog fichero, int distrito, bool completo = false)
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
                    foreach(sice_distritos_locales ds in distritos.OrderByDescending(x => x.id))
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
                libro.SaveAs(fichero.FileName,
                Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);

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
                int filaInicialTabla = 5;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                hoja = (Excel._Worksheet)libro.Worksheets.Add();
                hoja.Name = "DISTRITO "+distrito;  //Aqui debe ir el nombre del distrito
                List<VotosSeccion> vSeccion = this.ResultadosSeccion(0, 0, (int)distrito);
                List<Candidatos> candidatos = this.ListaCandidatos((int)distrito);

                //Montamos las cabeceras 
                char letraFinal = CrearEncabezados(filaInicialTabla, ref hoja,vSeccion,candidatos,1);


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
                            hoja.Cells[fila, 4] = (v.estatus != null) ? v.estatus : "NO CAPTURADA";

                            hoja.Cells[fila,contCand].Value = v.votos;
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
                    hoja.Cells[fila,1] = v.id_casilla;
                    hoja.Cells[fila,2] = v.seccion;
                    hoja.Cells[fila,3] = v.casilla;
                    hoja.Cells[fila,4] = (v.estatus != null) ? v.estatus : "NO CAPTURADA";
                    Lnominal = v.lista_nominal;

                    hoja.Cells[fila,contCand] = v.votos;
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
            catch(Exception E)
            {
                throw E;
            }
        }

        private char CrearEncabezados(int fila, ref Excel._Worksheet hoja, List<VotosSeccion> vSeccion, List<Candidatos> candidatos, int columnaInicial = 1)
        {
            try
            {
                Excel.Range rango;

                //** Montamos el título en la línea 1 **
                hoja.Cells[1, 2] = "LISTA DE RESULTADOS";
                char columnaLetra = 'A';

                List<int> widths = new List<int>();

                //Agregar encabezados
                hoja.Cells[fila, columnaInicial] = "No."; columnaInicial++; columnaLetra++; widths.Add(10);
                hoja.Cells[fila, columnaInicial] = "Sección"; columnaInicial++; columnaLetra++; widths.Add(20);
                hoja.Cells[fila, columnaInicial] = "Casilla"; columnaInicial++; columnaLetra++; widths.Add(30);
                hoja.Cells[fila, columnaInicial] = "Estatus"; columnaInicial++; columnaLetra++; widths.Add(20);
                hoja.Cells[fila, columnaInicial] = "Diferencia entre 1° y 2° Lugar"; columnaInicial++; columnaLetra++; widths.Add(30);


                //Agregar Columnas Caniddatos y Partidos
                foreach (Candidatos c in candidatos)
                {
                    hoja.Cells[fila, columnaInicial] = c.partido; columnaInicial++; columnaLetra++; widths.Add(20);
                }
                //Agregar columnas adicionales
                hoja.Cells[fila, columnaInicial] = "No Registrados"; columnaInicial++; columnaLetra++; widths.Add(20);
                hoja.Cells[fila, columnaInicial] = "Nulos"; columnaInicial++; columnaLetra++; widths.Add(20);
                hoja.Cells[fila, columnaInicial] = "Votación total Emitida"; columnaInicial++; columnaLetra++; widths.Add(30);
                hoja.Cells[fila, columnaInicial] = "L. Nominal"; columnaInicial++; columnaLetra++; widths.Add(20);
                hoja.Cells[fila, columnaInicial] = "Porcentaje Participación"; widths.Add(20);

                //Ponemos borde a las celdas
                string letra = columnaLetra.ToString()+fila;
                rango = hoja.Range["A"+fila, letra];
                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                //Centramos los textos
                rango = hoja.Rows[fila];
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                //Modificamos los anchos de las columnas
                int cont = 1;
                foreach(int widh in widths)
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
}
