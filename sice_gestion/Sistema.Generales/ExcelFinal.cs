using Sistema.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Sistema.Generales
{
    public class ExcelFinal
    {
        private string con = "MYSQLSERVER";

        public List<VotosSeccion> ResultadosSeccion()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
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
                            "RC.boletas_sobrantes, "+
                            "CONCAT(CND.nombre, ' ', CND.apellido_paterno, ' ', CND.apellido_materno) as candidato," +
                            "P.id as id_partido," +
                            "P.siglas_par as partido," +
                            "P.img_par as imagen," +
                            "C.id_distrito_local as distrito_local," +
                            "M.id AS id_municipio,"+
                            "M.municipio," +
                            "M2.municipio AS cabecera_local, " +
                            "RC.tipo_reserva as estatus, " +
                            "RC.grupo_trabajo, " +
                            "EA.estatus AS estatus_acta, " +
                            "EA.id AS id_estatus_acta " +
                        "FROM sice_votos RV " +
                        "LEFT JOIN sice_reserva_captura RC ON RC.id_casilla = RV.id_casilla " +
                        "LEFT JOIN sice_estado_acta EA ON RC.id_estatus_acta = EA.id " +
                        "LEFT JOIN sice_candidatos CND ON CND.id = RV.id_candidato " +
                        "LEFT JOIN sice_partidos_politicos P ON P.id = CND.fk_partido " +
                        "JOIN sice_casillas C ON C.id = RV.id_casilla " +
                        "JOIN sice_municipios M ON M.id = C.id_municipio " +
                        "JOIN sice_municipios M2 ON M2.id = C.id_cabecera_local " +
                        "ORDER BY C.seccion ASC, RV.id_casilla ASC, prelacion ASC ";

                    return contexto.Database.SqlQuery<VotosSeccion>(consulta).ToList();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<sice_partidos_politicos> ListaPartidos()
        {
            try
            {
                using (DatabaseContext contexto = new DatabaseContext(con))
                {
                    return (from p in contexto.sice_partidos_politicos where p.id != 2 && p.id != 3 && p.id != 6 select p).OrderBy(x => x.prelacion).ToList();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public int getColumna(string partido)
        {
            try
            {
                int columna = 1;
                switch (partido)
                {
                    case "CC_PAN_PRD_PD":
                        columna = 23;
                        break;
                    case "PRI":
                        columna = 24;
                        break;
                    case "PVEM":
                        columna = 25;
                        break;
                    case "PT":
                        columna = 26;
                        break;
                    case "MC":
                        columna = 27;
                        break;
                    case "PANAL":
                        columna = 28;
                        break;
                    case "MORENA":
                        columna = 29;
                        break;
                    case "PES":
                        columna = 30;
                        break;
                    case "C_PT_MORENA":
                        columna = 31;
                        break;
                    case "IND_DTTO_II":
                        columna = 32;
                        break;
                    case "IND_DTTO_IV":
                        columna = 33;
                        break;
                    case "IND_DTTO_XIII":
                        columna = 34;
                        break;
                }
                return columna;
            }
            catch(Exception E)
            {
                throw E;
            }
        }

        public int getNumRepresentantes(int distrito)
        {
            try
            {
                int representantes = 20;
                if (distrito == 2 || distrito == 4)
                    representantes = 21;
                else
                    representantes = 20;

                
                return representantes;
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public int generarExcel(SaveFileDialog fichero)
        {
            //this.generarExcelFinal(fichero);
            //return 1;
            try
            {
                Microsoft.Office.Interop.Excel.Application excel = new Excel.Application();
                Microsoft.Office.Interop.Excel._Workbook libro = null;

                //completo = true;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                libro = (Excel._Workbook)excel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);

                
                this.generaHoja(libro);
                

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

        public void generaHoja(Excel._Workbook libro)
        {
            try
            {
                Excel._Worksheet hoja = null;
                Excel.Range rango = null;
                int filaInicialTabla = 4;

                //creamos un libro nuevo y la hoja con la que vamos a trabajar
                hoja = (Excel._Worksheet)libro.Worksheets.Add();
                hoja.Name = "18.2 Dip Local ";  //Aqui debe ir el nombre del distrito
                List<VotosSeccion> vSeccion = this.ResultadosSeccion();

                
                ////Montamos las cabeceras 
                char letraFinal = CrearEncabezados(filaInicialTabla, ref hoja, 1);


                //Agregar Datos
                int fila = filaInicialTabla + 1;
                int idCasillaActual = 0;
                int cont = 1;
                int contCand = 6;
                //row.Cells[0].Value = 1;
                //dgvResultados.Rows.Add(row);
                List<int> vLst = new List<int>();
                List<int> listaVotosValidos = new List<int>();
                int Noregynulo = 0;
                int Lnominal = 0;
                bool flagInsert = true;

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
                            //hoja.Cells[fila, 1] = v.id_casilla;
                            //hoja.Cells[fila, 2] = v.seccion; hoja.Cells[fila, 2].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            //hoja.Cells[fila, 3] = v.casilla;
                            //hoja.Cells[fila, 4] = (v.estatus != null) ? v.estatus : "NO CAPTURADA";

                            //votos = v.estatus != "CAPTURADA" ? 0 : (int)v.votos;
                            //hoja.Cells[fila, contCand].Value = votos;
                            //vLst.Add(votos);
                            if (v.tipo == "VOTO")
                            {
                                hoja.Cells[fila, getColumna(v.partido)] = v.votos;
                                vLst.Add((int)v.votos);
                            }
                            else if (v.tipo == "NULO")
                            {
                                hoja.Cells[fila, 18] = v.votos;
                                Noregynulo += (int)v.votos;
                            }
                            else
                            {
                                hoja.Cells[fila, 19] = v.votos;
                                Noregynulo += (int)v.votos;
                            }
                            contCand++;
                        }

                        //Diferencia entre el primero y segundo
                        int totalVotacionEmitida = vLst.Sum() + Noregynulo;
                        int totalVotacionValida = vLst.Sum();

                        hoja.Cells[fila, 20] = totalVotacionValida; //Votos Validos
                        hoja.Cells[fila, 21] = totalVotacionEmitida; //Total Votacion

                        //Agregar Estilo fila
                        //string x = "A" + (fila).ToString();
                        //string y = letraFinal.ToString() + (fila).ToString();
                        //rango = hoja.Range[x, y];
                        //rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

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
                        string id_casilla = "";
                        string tipo_casilla = "";
                        string ext_contigua = "0";
                        if(v.casilla.Length >= 5)
                        {
                            tipo_casilla = v.casilla.Substring(0, 1);
                            id_casilla = "1";
                            ext_contigua = v.casilla.Substring(4);
                        }
                        else
                        {
                            tipo_casilla = v.casilla.Substring(0, 1);
                            id_casilla = v.casilla.Substring(1);
                        }

                        //Agregar Columnas
                        hoja.Cells[fila, 1] = 1;
                        hoja.Cells[fila, 2] = "DURANGO";
                        hoja.Cells[fila, 3] = v.distrito_local;
                        hoja.Cells[fila, 4] = v.cabecera_local;
                        hoja.Cells[fila, 5] = v.id_municipio;
                        hoja.Cells[fila, 6] = v.municipio;
                        hoja.Cells[fila, 7] = v.seccion; hoja.Cells[fila, 7].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        hoja.Cells[fila, 8] = id_casilla;
                        hoja.Cells[fila, 9] = tipo_casilla;
                        hoja.Cells[fila, 10] = ext_contigua; //EXT CONTIGUA
                        hoja.Cells[fila, 11] = 12;
                        hoja.Cells[fila, 12] = v.id_estatus_acta;
                        hoja.Cells[fila, 13] = "SI";
                        hoja.Cells[fila, 14] = 2;
                        hoja.Cells[fila, 15] = "---"; //ID_INCIDENTE
                        hoja.Cells[fila, 16] = "---"; //NUM_ACTA_IMPRESO
                        hoja.Cells[fila, 17] = v.lista_nominal;
                        hoja.Cells[fila, 22] = v.lista_nominal + getNumRepresentantes(v.distrito_local);//NUM BOLETAS RECIBIDAS
                        hoja.Cells[fila, 35] = v.boletas_sobrantes;
                        hoja.Cells[fila, 36] = "---"; //NUM ESCRITOS
                        hoja.Cells[fila, 37] = "---"; //BOLETAS OTRA ELECCION
                    }
                    
                    if(v.tipo == "VOTO")
                    {
                        hoja.Cells[fila, getColumna(v.partido)] = v.votos;
                        vLst.Add((int)v.votos);
                    }
                    else if(v.tipo == "NULO")
                    {
                        hoja.Cells[fila, 18] = v.votos;
                        Noregynulo += (int)v.votos;
                    }
                    else
                    {
                        hoja.Cells[fila, 19] = v.votos;
                        Noregynulo += (int)v.votos;
                    }

                   

                    idCasillaActual = (int)v.id_casilla;
                    cont++;
                    contCand++;

                    flagInsert = false;

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

        private char CrearEncabezados(int fila, ref Excel._Worksheet hoja,int columnaInicial = 1)
        {
            try
            {
                Excel.Range rango;
                Excel.Range rangoTitutlo;
                List<sice_partidos_politicos> listaPartidos = ListaPartidos();

                

                //Configuracon Hoja
                hoja.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
                hoja.PageSetup.Zoom = 63;
                hoja.PageSetup.PrintTitleRows = "$1:$7";

                hoja.PageSetup.TopMargin = 37.79;
                hoja.PageSetup.BottomMargin = 37.79;
                hoja.PageSetup.LeftMargin = 22.67;
                hoja.PageSetup.RightMargin = 22.67;

                char columnaLetra = 'A';


                List<double> widths = new List<double>();
                
                hoja.Cells[fila, columnaInicial] = "ID_ESTADO"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "NOMBRE_ESTADO"; columnaInicial++; columnaLetra++; widths.Add(18.43);
                hoja.Cells[fila, columnaInicial] = "ID_DISTRITO_LOCAL"; columnaInicial++; columnaLetra++; widths.Add(20.57);
                hoja.Cells[fila, columnaInicial] = "CABECERA_DISTRITAL_LOCAL"; columnaInicial++; columnaLetra++; widths.Add(25.29);
                hoja.Cells[fila, columnaInicial] = "ID_MUNICIPIO_LOCAL"; columnaInicial++; columnaLetra++; widths.Add(20.57);
                hoja.Cells[fila, columnaInicial] = "MUNICIPIO_LOCAL"; columnaInicial++; columnaLetra++; widths.Add(20.29);
                hoja.Cells[fila, columnaInicial] = "SECCION"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "ID_CASILLA"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "TIPO_CASILLA"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "EXT_CONTIGUA"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "ID_TIPO_CANDIDATURA"; columnaInicial++; columnaLetra++; widths.Add(25.29);
                hoja.Cells[fila, columnaInicial] = "ESTATUS_ACTA"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "CASILLA_INSTALADA"; columnaInicial++; columnaLetra++; widths.Add(20.29);
                hoja.Cells[fila, columnaInicial] = "ESTATUS_PAQUETE"; columnaInicial++; columnaLetra++; widths.Add(20.29);
                hoja.Cells[fila, columnaInicial] = "ID_INCIDENTE"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "NUM_ACTA_IMPRESO"; columnaInicial++; columnaLetra++; widths.Add(20.20);
                hoja.Cells[fila, columnaInicial] = "LISTA_NOMINAL_CASILLA"; columnaInicial++; columnaLetra++; widths.Add(25.29);
                hoja.Cells[fila, columnaInicial] = "NUM_VOTOS_NULOS"; columnaInicial++; columnaLetra++; widths.Add(20.29);
                hoja.Cells[fila, columnaInicial] = "NO_REGISTRADOS"; columnaInicial++; columnaLetra++; widths.Add(20.29);
                hoja.Cells[fila, columnaInicial] = "NUMERO_VOTOS_VALIDOS"; columnaInicial++; columnaLetra++; widths.Add(25.29);
                hoja.Cells[fila, columnaInicial] = "TOTAL_VOTOS"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "NUMERO_BOLETAS_RECIBIDAS"; columnaInicial++; columnaLetra++; widths.Add(29.29);

                //Agregar Columnas Caniddatos y Partidos
                foreach (sice_partidos_politicos p in listaPartidos)
                {
                    hoja.Cells[fila, columnaInicial] = p.siglas_par;
                    columnaInicial++; columnaLetra++; widths.Add(20.29);
                }
                //Agregar columnas adicionales
                hoja.Cells[fila, columnaInicial] = "NUMERO_BOLETAS_SOBRANTES"; columnaInicial++; columnaLetra++; widths.Add(29.29);
                hoja.Cells[fila, columnaInicial] = "NUM_ESCRITOS"; columnaInicial++; columnaLetra++; widths.Add(14.43);
                hoja.Cells[fila, columnaInicial] = "BOLETAS_OTRA_ELECCION"; columnaInicial++; columnaLetra++; widths.Add(25.29);
                
                //Ponemos borde a las celdas
                string letra = columnaLetra.ToString() + fila;
                rango = hoja.Range["A4:AL4"];
                rango.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                //Centramos los textos
                rango = hoja.Rows[fila];
                rango.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

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
}
