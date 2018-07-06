using Sistema.Generales;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sistema.DataModel;
using System.Threading;
using System.Globalization;
using Sistema.ComputosElectorales.Properties;

namespace Sistema.ComputosElectorales
{
    public partial class Reportes : Form
    {
        private ComputosElectoralesGenerales CompElec;
        private MsgBox msgBox;
        private int pageNumber = 1;
        private int totalPages = 0;
        private List<Candidatos> listaCandidatos;
        private System.ComponentModel.ComponentResourceManager resources;

        private PictureBox[] pictureBoxes;
        private TextBox[] textBoxes;
        private Panel[] panels;
        private Label[] labelsName;
        private Panel[] panelRes;
        private Label[] labelsRes;
        private Label[] labelsPor;

        private int TotalVotosDistrito = 0;

        public Reportes()
        {
            InitializeComponent();
            this.cargarComboDistrito();
            resources = new System.ComponentModel.ComponentResourceManager(typeof(Properties.Resources));
        }

        private void BuscarDistritos(int distrito)
        {
            try
            {
                if (LoginInfo.lista_distritos.Count > 0)
                {
                    int? result = LoginInfo.lista_distritos.Find(x => x == distrito);
                    if (result != 0)
                        btnDescargar.Enabled = false;
                    else
                        btnDescargar.Enabled = true;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void cargarComboDistrito()
        {
            try
            {
                if(LoginInfo.privilegios == 6)
                {
                    btnGenerarExcel.Visible = true;
                    btnGenerarExcelTodo.Visible = false;
                    btnExcelRecuento.Visible = false;
                }
                else
                {
                    btnGenerarExcel.Visible = true;
                    btnGenerarExcelTodo.Visible = true;
                    btnExcelRecuento.Visible = true;
                }
                //if (LoginInfo.privilegios == 6 || LoginInfo.privilegios == 1)
                //    btnGenerarExcelTodo.Visible = true;
                //else
                //    btnGenerarExcelTodo.Visible = false;
                CompElec = new ComputosElectoralesGenerales();
                List<sice_distritos_locales> ds = CompElec.ListaDistritos();
                ds.Insert(0, new sice_distritos_locales() { id = 0, distrito = "Seleccionar Distrito" });
                //ds.Insert(1, new sice_distritos_locales() { id = 0, distrito = "TODOS" });
                cmbDistrito.DataSource = null;
                cmbDistrito.DisplayMember = "distrito";
                cmbDistrito.ValueMember = "id";
                cmbDistrito.DataSource = ds;
                cmbDistrito.Enabled = true;

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void InicializarPaginador(int? distrito, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                int totalCandidatos = CompElec.ListaCandidatos((int)distrito).Count + 2;
                totalPages = CompElec.ResultadosSeccion(0, 0, (int)distrito).Count / totalCandidatos;
                if (totalPages > 0)
                {
                    double res = (double)totalPages / (double)pageSize;
                    double flor = Math.Ceiling(res);
                    totalPages = Convert.ToInt32(flor);
                }
                int y = 0;
                int pz = pageSize * totalCandidatos;
                int pn = pz * (pageNumber - 1);
                cargarResultados(distrito, pn, pz);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cargarResultadosTotales(int distrito)
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                List<Candidatos> listaCandidatos = CompElec.ListaCandidatos(distrito);
                int TotalRepresentantes = 0;
                //foreach (Candidatos cnd in listaCandidatos)
                //{
                //    if (cnd.tipo_partido != "COALICION")
                //    {
                //        if (cnd.coalicion != "")
                //        {
                //            TotalRepresentantes += CompElec.RepresentantesCComun(cnd.coalicion);
                //        }
                //        else
                //        {
                //            if (cnd.partido_local == 1)
                //                TotalRepresentantes += 1;
                //            else
                //                TotalRepresentantes += 2;
                //        }
                //    }

                //}

                List<VotosSeccion> vSeccionTotales = CompElec.ResultadosSeccion(0, 0, (int)distrito);
                List<VotosSeccion> vSeccionTotales2 = CompElec.ResultadosSeccionRP(0, 0, (int)distrito);
                List<AvanceCaptura> listaAvances = CompElec.ListaResultadosAvances(distrito);
                List<VotosSeccion> totalAgrupado = vSeccionTotales.Where(x => x.casilla != "S1").GroupBy(x => x.id_casilla).
                    Select(data => new VotosSeccion
                    {
                        id_candidato = data.First().id_candidato,
                        id_estatus_acta = data.First().id_estatus_acta,
                        seccion = data.First().seccion,
                        casilla = data.First().casilla,
                        lista_nominal = data.First().lista_nominal + TotalRepresentantes,
                        votos = data.First().votos
                    }).ToList();

                int totalCapturado = listaAvances.Where(x => x.id_estatus_acta == 1 || x.id_estatus_acta == 2 || x.id_estatus_acta == 8).Count();
                int totalRecuento = listaAvances.Where(x => x.id_estatus_acta == 5).Count();
                int totalNoregis = listaAvances.Where(x => x.id_estatus_acta == null).Count();
                int totalAReserva = listaAvances.Where(x => x.id_estatus_acta == 4).Count();
                int totalNoConta = listaAvances.Where(x => x.id_estatus_acta == 9).Count();
                int totalActas = listaAvances.Count;

                int totalSecciones = vSeccionTotales.GroupBy(x => x.seccion).Select(data => new VotosSeccion { seccion = data.First().seccion }).Count();
                int LnominalDistrito = totalAgrupado.Sum(x => x.lista_nominal);
                this.TotalVotosDistrito = vSeccionTotales.Where(x=> x.id_estatus_acta == 1).Sum(x => (int)x.votos);
                
                int actasCapturadas = vSeccionTotales.Where(x => x.id_estatus_acta == 1 || x.id_estatus_acta == 2 || x.id_estatus_acta == 8).GroupBy(y => y.id_casilla).Count();

                this.lblListaNominal.Text = String.Format(CultureInfo.InvariantCulture, "{0:#,#}", LnominalDistrito);
                this.lblTotalVotos.Text = TotalVotosDistrito > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", TotalVotosDistrito) : "0";

                decimal PorcentajeParDistrito = 0;
                if (TotalVotosDistrito > 0)
                {
                    PorcentajeParDistrito = Math.Round((Convert.ToDecimal(TotalVotosDistrito) * 100) / LnominalDistrito, 2);
                }
                this.lblParticipacion.Text = PorcentajeParDistrito + "%";
                this.lblDistrito.Text = distrito.ToString();
                this.lblActasCapturadas.Text = totalCapturado > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", totalCapturado) : "0";
                this.lblActasRecuento.Text = totalRecuento > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", totalRecuento) : "0";
                this.lblNoCap.Text = totalNoregis > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", totalNoregis) : "0";
                this.lblTotalActas.Text = totalActas > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", totalActas) : "0";
                //var x = vSeccion.Select(x=> new VotosSeccion { id_candidato = x.id_candidato, votos = x.s })


                //List<VotosSeccion> listaSumaCandidatos = vSeccionTotales.Where(x => x.estatus == "CAPTURADA" && x.id_candidato != null).GroupBy(y => y.id_candidato).Select(data => new VotosSeccion { id_candidato = data.First().id_candidato, votos = data.Sum(d => d.votos) }).OrderBy(x => x.votos).ToList();
                List<CandidatosResultados> lsCandidatos = CompElec.ListaResultadosCandidatos(distrito, true);
                List<CandidatosResultados> lsCandidatos2 = new List<CandidatosResultados>();
                //List<string> lsCoalicion = lsCandidatos.Where(x => x.coalicion != "" && x.coalicion != null).Select(data => data.coalicion).ToList();
                //string[] stringSeparators = new string[] { "," };
                //string[] result;
                //List<int> coaliciones = new List<int>();
                //foreach (string p in lsCoalicion)
                //{
                //    result = p.Split(stringSeparators, StringSplitOptions.None);
                //    foreach(string r in result)
                //    {
                //        coaliciones.Add(Convert.ToInt32(r));
                //    }
                //}
                //int yy = 0;
                //yy = coaliciones.Find(x => x == 2);
                lsCandidatos = lsCandidatos.Select(data => new CandidatosResultados {
                    id_candidato = data.id_candidato,
                    partido = data.partido,
                    candidato = data.candidato,
                    votos = data.votos,
                    tipo = data.tipo,
                    id_partido = data.id_partido
                }).Where(x=> x.tipo == "VOTO").OrderByDescending(x=> x.votos).ToList();

                int tempVotosPT = 0;
                if (lsCandidatos.Count > 0)
                {
                    foreach (CandidatosResultados ls in lsCandidatos)
                    {
                        if (ls.id_partido == 5 || ls.id_partido == 9 || ls.id_partido == 15)
                        {
                            tempVotosPT += (int)ls.votos;
                        }
                        else
                        {
                            lsCandidatos2.Add(ls);
                        }
                    }
                    lsCandidatos2.Add(new CandidatosResultados
                    {
                        id_candidato = 100,
                        votos = tempVotosPT,
                        id_partido = 9
                    });
                    lsCandidatos = lsCandidatos2.OrderByDescending(x => x.votos).ToList();
                    int PrimeroTotal = (int)lsCandidatos[0].votos;
                    int SeegundoTotal = (int)lsCandidatos[1].votos;
                    int diferenciaTotal = PrimeroTotal - SeegundoTotal;
                    decimal diferenciaPorcentajeTotal = 0;
                    if (TotalVotosDistrito > 0 && diferenciaTotal > 0 )
                    {
                        diferenciaPorcentajeTotal = Math.Round((Convert.ToDecimal(diferenciaTotal) * 100) / TotalVotosDistrito, 2);
                    }
                    this.lblDiferencia.Text = diferenciaPorcentajeTotal + "%";
                }
                else
                {
                    this.lblDiferencia.Text = 0 + "%";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cargarResultados(int? distrito, int pageNumber, int pageSize)
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                List<VotosSeccion> vSeccion = CompElec.ResultadosSeccion(pageNumber, pageSize, (int)distrito);
                List<Candidatos> candidatos = CompElec.ListaCandidatos((int)distrito);
                dgvResultados.Columns.Clear();
                dgvResultados.DataSource = null;
                dgvResultados.ColumnHeadersHeight = 85;  // or maybe a little more..


                //Agregar encabezados

                DataGridViewTextBoxColumn noColumna = new DataGridViewTextBoxColumn();
                noColumna.Name = "no";
                noColumna.HeaderText = "No.";
                noColumna.ValueType = typeof(System.Int32);
                noColumna.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvResultados.Columns.Add(noColumna);

                DataGridViewTextBoxColumn seccionColumna = new DataGridViewTextBoxColumn();
                seccionColumna.Name = "seccion";
                seccionColumna.HeaderText = "Sección";
                seccionColumna.ValueType = typeof(System.Int32);
                seccionColumna.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvResultados.Columns.Add(seccionColumna);

                DataGridViewTextBoxColumn casillaColumna = new DataGridViewTextBoxColumn();
                casillaColumna.Name = "casilla";
                casillaColumna.HeaderText = "Casilla";
                casillaColumna.ValueType = typeof(string);
                casillaColumna.Width = 100;
                dgvResultados.Columns.Add(casillaColumna);

                DataGridViewTextBoxColumn estatusColumna = new DataGridViewTextBoxColumn();
                estatusColumna.Name = "estatus";
                estatusColumna.HeaderText = "Estatus";
                estatusColumna.ValueType = typeof(string);
                estatusColumna.Width = 135;
                dgvResultados.Columns.Add(estatusColumna);

                DataGridViewTextBoxColumn DferenciaColumna = new DataGridViewTextBoxColumn();
                DferenciaColumna.Name = "diferencia";
                DferenciaColumna.HeaderText = "Diferencia entre 1° y 2° Lugar";
                DferenciaColumna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DferenciaColumna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                DferenciaColumna.ValueType = typeof(System.Int32);
                DferenciaColumna.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvResultados.Columns.Add(DferenciaColumna);


                //Agregar Columnas Caniddatos y Partidos
                foreach (Candidatos c in candidatos)
                {
                    DataGridViewTextBoxColumn columnaCandidato = new DataGridViewTextBoxColumn();
                    columnaCandidato.Name = c.partido;
                    columnaCandidato.HeaderText = c.partido;
                    columnaCandidato.ValueType = typeof(System.Int32);
                    columnaCandidato.Width = 80;
                    columnaCandidato.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                    dgvResultados.Columns.Add(columnaCandidato);
                }
                //Agregar columnas adicionales
                DataGridViewTextBoxColumn NoRegColumna = new DataGridViewTextBoxColumn();
                NoRegColumna.Name = "no_registrados";
                NoRegColumna.HeaderText = "No Registrados";
                NoRegColumna.ValueType = typeof(System.Int32);
                NoRegColumna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                NoRegColumna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
                NoRegColumna.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvResultados.Columns.Add(NoRegColumna);

                DataGridViewTextBoxColumn NulosColumna = new DataGridViewTextBoxColumn();
                NulosColumna.Name = "nulos";
                NulosColumna.HeaderText = "Nulos";
                NulosColumna.ValueType = typeof(System.Int32);
                NulosColumna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.BottomCenter;
                NulosColumna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
                NulosColumna.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvResultados.Columns.Add(NulosColumna);

                DataGridViewTextBoxColumn VtotalColumna = new DataGridViewTextBoxColumn();
                VtotalColumna.Name = "vtotal";
                VtotalColumna.HeaderText = "Votación total Emitida";
                VtotalColumna.ValueType = typeof(System.Int32);
                VtotalColumna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                VtotalColumna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                VtotalColumna.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvResultados.Columns.Add(VtotalColumna);

                DataGridViewTextBoxColumn LnominalColumna = new DataGridViewTextBoxColumn();
                LnominalColumna.Name = "lnominal";
                LnominalColumna.HeaderText = "L. Nominal";
                LnominalColumna.ValueType = typeof(System.Int32);
                LnominalColumna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                LnominalColumna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                LnominalColumna.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvResultados.Columns.Add(LnominalColumna);

                DataGridViewTextBoxColumn PparticipacionColumna = new DataGridViewTextBoxColumn();
                PparticipacionColumna.Name = "porcentaje";
                PparticipacionColumna.HeaderText = "Porcentaje Participación";
                PparticipacionColumna.ValueType = typeof(System.Decimal);
                PparticipacionColumna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                PparticipacionColumna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                PparticipacionColumna.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvResultados.Columns.Add(PparticipacionColumna);

                //this.dgvResultados.Columns.Count
                int fila = 0;
                int idCasillaActual = 0;
                int cont = 1;
                int contCand = 5;
                DataGridViewRow row = (DataGridViewRow)dgvResultados.Rows[fila].Clone();
                //row.Cells[0].Value = 1;
                //dgvResultados.Rows.Add(row);
                List<int> vLst = new List<int>();
                int Noregynulo = 0;
                int Lnominal = 0;

                this.listaCandidatos = CompElec.ListaCandidatos((int)distrito);
                //int tempC = listaCandidatos.Count;

                int TotalRepresentantes = 0;
                //foreach (Candidatos cnd in listaCandidatos)
                //{
                //    if (cnd.tipo_partido != "COALICION")
                //    {
                //        if (cnd.coalicion != "")
                //        {
                //            TotalRepresentantes += CompElec.RepresentantesCComun(cnd.coalicion);
                //        }
                //        else
                //        {
                //            if (cnd.partido_local == 1)
                //                TotalRepresentantes += 1;
                //            else
                //                TotalRepresentantes += 2;
                //        }
                //    }

                //}
                int votos = 0;
                int tempVotosPT = 0;
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
                            row.Cells[0].Value = v.id_casilla;
                            row.Cells[1].Value = v.seccion;
                            row.Cells[2].Value = v.casilla;
                            row.Cells[3].Value = (v.estatus_acta != null) ? v.estatus_acta : "NO CAPTURADA";

                            votos = v.estatus != "CAPTURADA" ? 0 : (int)v.votos;
                            row.Cells[contCand].Value = votos;
                            vLst.Add(votos);
                            contCand++;
                        }

                        //Diferencia entre el primero y segundo
                        vLst.Add(tempVotosPT);
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
                        row.Cells[4].Value = diferencia + "%";

                        //Votacion Emitida
                        row.Cells[contCand].Value = totalVotacionEmitida;

                        //Lista Nominal
                        row.Cells[contCand + 1].Value = Lnominal;

                        //Porcentaje de Participacion
                        if (totalVotacionEmitida == 0)
                            row.Cells[contCand + 2].Value = 0 + "%";
                        else
                            row.Cells[contCand + 2].Value = Math.Round((Convert.ToDecimal(totalVotacionEmitida) * 100) / Lnominal, 2) + "%";

                        //Agregar fila
                        dgvResultados.Rows.Add(row);
                        fila++;
                        row = (DataGridViewRow)dgvResultados.Rows[fila].Clone();
                        contCand = 5;
                        vLst = new List<int>();
                        Noregynulo = 0;
                        tempVotosPT = 0;
                        //Inrementar filla
                    }

                    //Agregar Columnas
                    row.Cells[0].Value = v.id_casilla;
                    row.Cells[1].Value = v.seccion;
                    row.Cells[2].Value = v.casilla;
                    row.Cells[3].Value = (v.estatus_acta != null) ? v.estatus : "NO CAPTURADA";
                    Lnominal = v.casilla == "S1" ? Configuracion.BoletasEspecial : v.lista_nominal + TotalRepresentantes;

                    votos = v.estatus != "CAPTURADA" ? 0 : (int)v.votos;
                    row.Cells[contCand].Value =votos;
                   if (v.tipo == "VOTO")
                    {
                        if (v.id_partido == 5 || v.id_partido == 9 || v.id_partido == 15)
                        {
                            tempVotosPT += v.estatus != "CAPTURADA" ? 0 : (int)v.votos;
                        }
                        else
                        {
                            vLst.Add(v.estatus != "CAPTURADA" ? 0 :(int)v.votos);
                        }
                        
                    }
                    else
                    {
                        Noregynulo += v.estatus != "CAPTURADA" ? 0 :(int)v.votos;
                    }

                    idCasillaActual = (int)v.id_casilla;
                    cont++;
                    contCand++;

                    //if(cont == vSeccion.Count){
                    //    dgvResultados.Rows.Add(row);
                    //}


                }
                foreach (DataGridViewColumn column in dgvResultados.Columns)
                {
                    column.HeaderCell.Style.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                    column.Frozen = false;
                    //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                dgvResultados.ScrollBars = ScrollBars.Both;

                this.cargarCandidatos((int)distrito);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cargarCandidatos(int distrito)
        {
            try
            {
                this.tablePanelPartidos.Visible = false;
                this.tablePanelPartidos.Controls.Clear();
                this.tablePanelPartidos.RowStyles.Clear();
                this.tablePanelPartidos.ColumnStyles.Clear();
                this.tablePanelPartidos.RowCount = 0;
                this.tablePanelPartidos.ColumnCount = 0;
                this.tablePanelPartidos.SuspendLayout();

                CompElec = new ComputosElectoralesGenerales();
                List<CandidatosResultados> lsCandidatos = CompElec.ListaResultadosCandidatos(distrito,true);
                if(lsCandidatos.Count == 0)
                    lsCandidatos = CompElec.ListaResultadosCandidatos(distrito);
                int totalCandidatos = lsCandidatos.Count();
                if (lsCandidatos != null)
                {
                    
                    totalCandidatos = lsCandidatos.Count();


                    this.pictureBoxes = new PictureBox[lsCandidatos.Count];
                    this.textBoxes = new TextBox[lsCandidatos.Count];
                    this.panels = new Panel[lsCandidatos.Count];
                    this.labelsName = new Label[lsCandidatos.Count];

                    this.panelRes = new Panel[lsCandidatos.Count];
                    this.labelsPor = new Label[lsCandidatos.Count];
                    this.labelsRes = new Label[lsCandidatos.Count];

                    this.tablePanelPartidos.RowCount = 1;

                    //Agregar Columnas
                    this.tablePanelPartidos.AutoScroll = true;
                    this.tablePanelPartidos.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                    this.tablePanelPartidos.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
                    this.tablePanelPartidos.ColumnCount = totalCandidatos;
                    decimal anchoColumnas = Math.Round(100 / (Convert.ToDecimal(totalCandidatos)), 6);
                    for (int i = 0; i < totalCandidatos; i++)
                    {
                        this.tablePanelPartidos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float)anchoColumnas));
                        //this.tablePanelPartidos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
                    }

                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Properties.Resources));
                    //Agregar Imagen, Etiqueta, TextBox por fila

                    for (int i = 0; i < lsCandidatos.Count; i++)
                    {

                        pictureBoxes[i] = new PictureBox();
                        textBoxes[i] = new TextBox();
                        labelsName[i] = new Label();
                        panels[i] = new Panel();

                        panelRes[i] = new Panel();
                        labelsPor[i] = new Label();
                        labelsRes[i] = new Label();

                        //Imagen
                        pictureBoxes[i].Anchor = System.Windows.Forms.AnchorStyles.None;
                        pictureBoxes[i].Image = lsCandidatos[i].tipo == "NO REGISTRADO" ? (System.Drawing.Image)(Properties.Resources.no_regis) : lsCandidatos[i].tipo == "NULO" ? (System.Drawing.Image)(Properties.Resources.nulos1) : ((System.Drawing.Image)(resources.GetObject(lsCandidatos[i].imagen)));
                        pictureBoxes[i].Location = new System.Drawing.Point(125, 8);
                        pictureBoxes[i].Margin = new System.Windows.Forms.Padding(10, 5, 10, 5);
                        pictureBoxes[i].Name = "pictureBox" + i;
                        pictureBoxes[i].Size = new System.Drawing.Size(49, 70);
                        pictureBoxes[i].SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                        pictureBoxes[i].TabIndex = 20 + i;
                        pictureBoxes[i].TabStop = false;

                        //Etiqueta
                        labelsName[i].Dock = System.Windows.Forms.DockStyle.Fill;
                        labelsName[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        labelsName[i].Location = new System.Drawing.Point(910, 86);
                        labelsName[i].Name = "labelNameCandidato" + i;
                        labelsName[i].Size = new System.Drawing.Size(68, 65);
                        labelsName[i].TabIndex = 51;
                        labelsName[i].Text = lsCandidatos[i].tipo == "NO REGISTRADO" ? "Candidato No Registrado" : lsCandidatos[i].tipo == "NULO" ? "Votos Nulos" : lsCandidatos[i].candidato;
                        labelsName[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                        // 
                        // labelRes
                        // 
                        labelsRes[i].Dock = System.Windows.Forms.DockStyle.Top;
                        labelsRes[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        labelsRes[i].Location = new System.Drawing.Point(0, 0);
                        labelsRes[i].Name = "label1";
                        labelsRes[i].Size = new System.Drawing.Size(150, 26);
                        labelsRes[i].TabIndex = 0;
                        labelsRes[i].Text = lsCandidatos[i].votos > 0 ? String.Format(CultureInfo.InvariantCulture, "{0:#,#}", lsCandidatos[i].votos) : "0";
                        labelsRes[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                        // 
                        // labelPor
                        // 
                        decimal porcentaje = 0;
                        if (this.TotalVotosDistrito > 0 && lsCandidatos[i].votos > 0)
                        {
                            porcentaje = Math.Round((Convert.ToDecimal(lsCandidatos[i].votos) * 100) / Convert.ToDecimal(TotalVotosDistrito), 2);
                        }


                        labelsPor[i].Dock = System.Windows.Forms.DockStyle.Top;
                        labelsPor[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        labelsPor[i].Location = new System.Drawing.Point(0, 26);
                        labelsPor[i].Name = "label2";
                        labelsPor[i].Size = new System.Drawing.Size(150, 30);
                        labelsPor[i].TabIndex = 1;
                        labelsPor[i].Text = porcentaje.ToString() + "%";
                        labelsPor[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                        panelRes[i].Controls.Add(labelsPor[i]);
                        panelRes[i].Controls.Add(labelsRes[i]);
                        panelRes[i].Dock = System.Windows.Forms.DockStyle.Fill;
                        panelRes[i].Location = new System.Drawing.Point(3, 64);
                        panelRes[i].Name = "panelRes" + i;
                        panelRes[i].Size = new System.Drawing.Size(150, 56);
                        panelRes[i].TabIndex = 0;

                        //Agregar Imagen
                        this.tablePanelPartidos.Controls.Add(pictureBoxes[i], i, 0);
                        //Agregar Etiqueta
                        this.tablePanelPartidos.Controls.Add(labelsName[i], i, 1);
                        //Agregar Textbox
                        this.tablePanelPartidos.Controls.Add(panelRes[i], i, 2);


                    }

                    //Agregar Filas
                    this.tablePanelPartidos.RowCount = 3;
                    this.tablePanelPartidos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
                    this.tablePanelPartidos.RowStyles.Add(new System.Windows.Forms.RowStyle());
                    this.tablePanelPartidos.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                    //this.tablePanelPartidos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.692307F));
                    //this.tblPanaelPartidos.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
                    this.tablePanelPartidos.ResumeLayout(false);
                    this.tablePanelPartidos.Visible = true;
                    //textBoxes[0].Focus();
                    //ShowScrollBar(this.tableLayoutPanel2.Handle, SB_HORZ, false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void bloquearControles()
        {
            this.btnAnterior.Enabled = false;
            this.btnSiguiente.Enabled = false;
            this.btnPrimero.Enabled = false;
            this.btnUltimo.Enabled = false;
            this.lblTotalPag.Text = "Páginas";
        }

        private bool IsFirstPage()
        {
            if (pageNumber == 1)
                return true;
            else
                return false;
        }

        private bool IsLastPage()
        {
            if (pageNumber == totalPages)
                return true;
            else
                return false;
        }

        private bool HasPreviousPage()
        {
            if (pageNumber - 1 > 0 && pageNumber <= totalPages)
                return true;
            else
                return false;
        }

        private bool HasNextPage()
        {
            if (pageNumber + 1 <= totalPages)
                return true;
            else
                return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.panel2.Visible = false;
            this.Close();
        }

        private void cmbDistrito_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.TotalVotosDistrito = 0;
                this.pageNumber = 1;
                int? selected = Convert.ToInt32(cmbDistrito.SelectedValue);
                if (selected > 0 && selected != null)
                {
                    this.cargarResultadosTotales((int)selected);
                    this.BuscarDistritos((int)selected);
                    this.InicializarPaginador(selected);
                    if (this.totalPages > 0)
                    {
                        btnPrimero.Enabled = !this.IsFirstPage();
                        btnUltimo.Enabled = !this.IsLastPage();
                        btnAnterior.Enabled = this.HasPreviousPage();
                        btnSiguiente.Enabled = this.HasNextPage();

                        lblTotalPag.Text = string.Format("Pág {0}/{1}", pageNumber, totalPages);
                    }
                    else
                    {
                        bloquearControles();
                    }

                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.HasPreviousPage())
                {
                    this.InicializarPaginador(Convert.ToInt32(cmbDistrito.SelectedValue), --pageNumber);
                    btnPrimero.Enabled = !this.IsFirstPage();
                    btnUltimo.Enabled = true;
                    btnAnterior.Enabled = this.HasPreviousPage();
                    btnSiguiente.Enabled = this.HasNextPage();

                    lblTotalPag.Text = string.Format("Pág {0}/{1}", pageNumber, totalPages);
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.HasNextPage())
                {
                    this.InicializarPaginador(Convert.ToInt32(cmbDistrito.SelectedValue), ++pageNumber);
                    btnPrimero.Enabled = true;
                    btnUltimo.Enabled = !this.IsLastPage();
                    btnAnterior.Enabled = this.HasPreviousPage();
                    btnSiguiente.Enabled = this.HasNextPage();

                    lblTotalPag.Text = string.Format("Pág {0}/{1}", pageNumber, totalPages);
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnPrimero_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrimero.Enabled = false;
                btnUltimo.Enabled = true;
                pageNumber = 1;

                this.InicializarPaginador(Convert.ToInt32(cmbDistrito.SelectedValue), pageNumber);
                btnAnterior.Enabled = this.HasPreviousPage();
                btnSiguiente.Enabled = this.HasNextPage();

                lblTotalPag.Text = string.Format("Pág {0}/{1}", pageNumber, totalPages);

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            try
            {
                btnUltimo.Enabled = false;
                btnPrimero.Enabled = true;
                pageNumber = totalPages;

                this.InicializarPaginador(Convert.ToInt32(cmbDistrito.SelectedValue), pageNumber);
                btnAnterior.Enabled = this.HasPreviousPage();
                btnSiguiente.Enabled = this.HasNextPage();

                lblTotalPag.Text = string.Format("Pág {0}/{1}", pageNumber, totalPages);

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }

        }

        private void btnGenerarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                int? selected = Convert.ToInt32(cmbDistrito.SelectedValue);
                if (selected > 0 && selected != null)
                {
                    btnGenerarExcel.Enabled = false;
                    ((MDIMainComputosElectorales)this.MdiParent).GenerarExcel((int)selected, false);
                }

            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnGenerarExcelTodo_Click(object sender, EventArgs e)
        {
            try
            {
                btnGenerarExcel.Enabled = false;
                ((MDIMainComputosElectorales)this.MdiParent).GenerarExcel(0, true);
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnActualizarGrid_Click(object sender, EventArgs e)
        {
            try
            {
                this.pageNumber = 1;
                int? selected = Convert.ToInt32(cmbDistrito.SelectedValue);
                if (selected > 0 && selected != null)
                {
                    //Buscar Distritos no validos
                    this.BuscarDistritos((int)selected);
                    this.InicializarPaginador(selected);
                    if (this.totalPages > 0)
                    {
                        btnPrimero.Enabled = !this.IsFirstPage();
                        btnUltimo.Enabled = !this.IsLastPage();
                        btnAnterior.Enabled = this.HasPreviousPage();
                        btnSiguiente.Enabled = this.HasNextPage();

                        lblTotalPag.Text = string.Format("Pág {0}/{1}", pageNumber, totalPages);
                    }
                    else
                    {
                        bloquearControles();
                    }

                }
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnDescargar_Click(object sender, EventArgs e)
        {
            try
            {
                int? selected = Convert.ToInt32(cmbDistrito.SelectedValue);
                if (selected > 0 && selected != null)
                {
                    btnDescargar.Enabled = false;
                    ((MDIMainComputosElectorales)this.MdiParent).DescargarDatosLocal((int)selected);

                    //Thread thread = new Thread(() =>
                    //{
                    //    res_descarga = EjecutarProceso(1);
                    //    // Action delegate points to SetLabelTextProperty method
                    //    // Signature of SetLabelTextProperty() method should match
                    //    // with the signature of Action delegate
                    //    Action action = new Action(showMesage);
                    //    this.BeginInvoke(action);
                    //});
                    //thread.Start();

                }
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void Reportes_Shown(object sender, EventArgs e)
        {
            this.MdiParent.WindowState = FormWindowState.Maximized;
        }
        private void Reportes_Load(object sender, EventArgs e)
        {
        }

        private void dgvResultados_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                int inicio = 5;
                int fin = this.listaCandidatos.Count + 4;
                if (this.listaCandidatos.Count > 0)
                {
                    if (e.RowIndex < 0 && (e.ColumnIndex >= inicio && e.ColumnIndex <= fin))

                    {

                        //e.Graphics.DrawImage((System.Drawing.Image)(Resources.pri), e.CellBounds);

                        Image img = (System.Drawing.Image)(resources.GetObject(listaCandidatos[e.ColumnIndex - inicio].imagen));
                        //Image img = (System.Drawing.Image)(Resources.pri);
                        Rectangle r32 = new Rectangle(e.CellBounds.Left + e.CellBounds.Width - 65, 5, 50, 50);
                        Rectangle r96 = new Rectangle(0, 0, 135, 120);
                        string header = dgvResultados.Columns[e.ColumnIndex].HeaderText;
                        e.PaintBackground(e.CellBounds, true);  // or maybe false ie no selection?
                        e.PaintContent(e.CellBounds);

                        e.Graphics.DrawImage(img, r32, r96, GraphicsUnit.Pixel);


                        e.Handled = true;

                    }
                    else if (e.RowIndex < 0 && e.ColumnIndex == fin + 1)
                    {
                        Image img = (System.Drawing.Image)(Resources.no_regis);
                        Rectangle r32 = new Rectangle(e.CellBounds.Left + e.CellBounds.Width - 83, 5, 50, 50);
                        Rectangle r96 = new Rectangle(0, 0, 60, 60);
                        string header = dgvResultados.Columns[e.ColumnIndex].HeaderText;
                        e.PaintBackground(e.CellBounds, true);  // or maybe false ie no selection?
                        e.PaintContent(e.CellBounds);

                        e.Graphics.DrawImage(img, r32, r96, GraphicsUnit.Pixel);


                        e.Handled = true;
                    }
                    else if (e.RowIndex < 0 && e.ColumnIndex == fin + 2)
                    {
                        Image img = (System.Drawing.Image)(Resources.nulos1);
                        Rectangle r32 = new Rectangle(e.CellBounds.Left + e.CellBounds.Width - 56, 5, 50, 50);
                        Rectangle r96 = new Rectangle(0, 0, 60, 60);
                        string header = dgvResultados.Columns[e.ColumnIndex].HeaderText;
                        e.PaintBackground(e.CellBounds, true);  // or maybe false ie no selection?
                        e.PaintContent(e.CellBounds);

                        e.Graphics.DrawImage(img, r32, r96, GraphicsUnit.Pixel);


                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }


        }

        private void btnExcelRecuento_Click(object sender, EventArgs e)
        {
            try
            {
                btnExcelRecuento.Enabled = false;
                ((MDIMainComputosElectorales)this.MdiParent).GenerarExcel(0, true, "RECUENTO");


            }
            catch (Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void Reportes_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.panel2.Visible = false;
        }
    }
}
