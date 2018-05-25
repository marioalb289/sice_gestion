﻿using Sistema.Generales;
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
using Sistema.RegistroActasLocal.Properties;

namespace Sistema.RegistroActasLocal
{
    public partial class Reportes : Form
    {
        private RegistroLocalGenerales rgActas;
        private MsgBox msgBox;
        private int pageNumber = 1;
        private int totalPages = 0;
        private System.ComponentModel.ComponentResourceManager resources;
        static Control.ControlCollection testC;
        List<Candidatos> listaCandidatos;

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
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        private void ValidarRecuento()
        {
            try
            {
                RegistroLocalGenerales reg = new RegistroLocalGenerales();
                reg.validarPuntosRecuento();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void cargarComboDistrito()
        {
            try
            {
                if (LoginInfo.privilegios == 6 || LoginInfo.privilegios == 1)
                    btnGenerarExcelTodo.Visible = true;
                else
                    btnGenerarExcelTodo.Visible = false;
                rgActas = new RegistroLocalGenerales();
                List<sice_distritos_locales> ds = rgActas.ListaDistritos();
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

        private void InicializarPaginador(int? distrito, int pageNumber = 1, int pageSize = 25)
        {
            try
            {
                rgActas = new RegistroLocalGenerales();
                int totalCandidatos = rgActas.ListaCandidatos((int)distrito).Count + 2;
                totalPages = rgActas.ResultadosSeccionCapturaTotal(0, 0, (int)distrito) / totalCandidatos;
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
                rgActas = new RegistroLocalGenerales();
                this.listaCandidatos = rgActas.ListaCandidatos(distrito);
                var groupTotalNacional = this.listaCandidatos.GroupBy(x => x.partido_local).Select(grp => new {
                    local = grp.Key,
                    total = grp.Count(),
                }).ToArray();
                int TotalRepresentantes = 0;
                foreach (var numInfo in groupTotalNacional)
                {
                    if (numInfo.local == 1)
                        TotalRepresentantes += numInfo.total;
                    else
                        TotalRepresentantes += numInfo.total * 2;
                }
                List<VotosSeccion> vSeccionTotales = rgActas.ResultadosSeccionCaptura(0, 0, (int)distrito);
                List<VotosSeccion> totalAgrupado =vSeccionTotales.GroupBy(x => x.id_casilla).Select(data => new VotosSeccion { id_candidato = data.First().id_candidato, casilla = data.First().casilla,lista_nominal = data.First().lista_nominal + TotalRepresentantes, votos = data.First().votos }).ToList();
                int LnominalDistrito = totalAgrupado.Sum(x => x.lista_nominal);
                int TotalVotosDistrito = vSeccionTotales.Sum(x => (int)x.votos);

                this.lblListaNominal.Text = String.Format(CultureInfo.InvariantCulture, "{0:#,#}", LnominalDistrito);
                this.lblTotalVotos.Text = String.Format(CultureInfo.InvariantCulture, "{0:#,#}", TotalVotosDistrito);



                decimal PorcentajeParDistrito = 0;
                if (TotalVotosDistrito > 0)
                {
                    PorcentajeParDistrito = Math.Round((Convert.ToDecimal(TotalVotosDistrito) * 100) / LnominalDistrito, 2);
                }
                this.lblParticipacion.Text = PorcentajeParDistrito + "%";
                this.lblDistrito.Text = distrito.ToString();
                this.lblActasCapturadas.Text = String.Format(CultureInfo.InvariantCulture, "{0:#,#}", vSeccionTotales.Where(x => x.estatus == "ATENDIDO").GroupBy(y => y.casilla).Count());
                //var x = vSeccion.Select(x=> new VotosSeccion { id_candidato = x.id_candidato, votos = x.s })

                List<VotosSeccion> listaSumaCandidatos = vSeccionTotales.Where(x => x.estatus == "ATENDIDO" && x.id_candidato != null).GroupBy(y => y.id_candidato).Select(data => new VotosSeccion { id_candidato = data.First().id_candidato, votos = data.Sum(d => d.votos) }).OrderBy(x=> x.votos).ToList();
                //listaSumaCandidatos.OrderBy(x => x.votos);
                if (listaSumaCandidatos.Count > 0)
                {
                    int PrimeroTotal = (int)listaSumaCandidatos[listaSumaCandidatos.Count - 1].votos;
                    int SeegundoTotal = (int)listaSumaCandidatos[listaSumaCandidatos.Count - 2].votos;
                    int diferenciaTotal = PrimeroTotal - SeegundoTotal;
                    decimal diferenciaPorcentajeTotal = 0;
                    if (TotalVotosDistrito > 0)
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
                List<VotosSeccion> vSeccion = rgActas.ResultadosSeccionCaptura(pageNumber, pageSize, (int)distrito);
                List<Candidatos> candidatos = rgActas.ListaCandidatos((int)distrito);
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
                estatusColumna.Width = 235;
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
                    //columnaCandidato.DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
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
                PparticipacionColumna.Name = "porcentajep";
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

                List<Candidatos> listaCandidatos = rgActas.ListaCandidatos((int)distrito);
                //int tempC = listaCandidatos.Count;

                var groupTotalNacional = listaCandidatos.GroupBy(x => x.partido_local).Select(grp => new {
                    local = grp.Key,
                    total = grp.Count(),
                }).ToArray();
                int TotalRepresentantes = 0;
                foreach (var numInfo in groupTotalNacional)
                {
                    if (numInfo.local == 1)
                        TotalRepresentantes += numInfo.total;
                    else
                        TotalRepresentantes += numInfo.total * 2;
                }

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
                            row.Cells[3].Value = v.estatus_acta != null ? v.estatus_acta : "NO CAPTURADA";

                            row.Cells[contCand].Value = v.votos;
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
                        row.Cells[4].Value = diferencia + "%";

                        //Votacion Emitida
                        row.Cells[contCand].Value = totalVotacionEmitida;

                        //Lista Nominal
                        row.Cells[contCand + 1].Value = Lnominal;

                        //Porcentaje de Participacion
                        if (totalVotacionEmitida == 0)
                        {
                            row.Cells[contCand + 2].Value = 0 + "%";
                        }

                        else
                        {
                            decimal tempRes = Math.Round((Convert.ToDecimal(totalVotacionEmitida) * 100) / Lnominal, 2);
                            row.Cells[contCand + 2].Value = tempRes + "%";
                        }
                            

                        //Agregar fila
                        dgvResultados.Rows.Add(row);
                        fila++;
                        row = (DataGridViewRow)dgvResultados.Rows[fila].Clone();
                        contCand = 5;
                        vLst = new List<int>();
                        Noregynulo = 0;
                        //Inrementar filla
                    }

                    //Agregar Columnas
                    row.Cells[0].Value = v.id_casilla;
                    row.Cells[1].Value = v.seccion;
                    row.Cells[2].Value = v.casilla;
                    row.Cells[3].Value = v.estatus_acta != null ? v.estatus_acta : "NO CAPTURADA";
                    Lnominal =  v.lista_nominal + TotalRepresentantes;

                    row.Cells[contCand].Value = v.votos;
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
                foreach (DataGridViewColumn column in dgvResultados.Columns)
                {
                    column.HeaderCell.Style.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Pixel);
                    column.Frozen = false;
                    //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                dgvResultados.ScrollBars = ScrollBars.Both;


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
            this.Close();
        }

        private void cmbDistrito_SelectedValueChanged(object sender, EventArgs e)
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
                    this.cargarResultadosTotales((int)selected);
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
        
        private void btnDescargar_Click(object sender, EventArgs e)
        {
            try
            {
                int? selected = Convert.ToInt32(cmbDistrito.SelectedValue);
                if (selected > 0 && selected != null)
                {
                    btnDescargar.Enabled = false;
                    ((MDIMainRegistroActas)this.MdiParent).DescargarDatosLocal((int)selected);
                    
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
            catch(Exception ex)
            {
                msgBox = new MsgBox(this, ex.Message, "Atención", MessageBoxButtons.OK, "Error");
                msgBox.ShowDialog(this);
            }
        }

        private void btnGenerarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                //int? selected = Convert.ToInt32(cmbDistrito.SelectedValue);
                //if (selected > 0 && selected != null)
                //{
                //this.ValidarRecuento();
                    btnGenerarExcel.Enabled = false;
                    ((MDIMainRegistroActas)this.MdiParent).GenerarExcel(0,false);

                //}

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
                ((MDIMainRegistroActas)this.MdiParent).GenerarExcel(0,true);                

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


                        //Image img = (System.Drawing.Image)(Resources.pri);
                        Image img = (System.Drawing.Image)(resources.GetObject(listaCandidatos[e.ColumnIndex - inicio].imagen));
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
                        Image img = (System.Drawing.Image)(Resources.nulos);
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
    }
}
