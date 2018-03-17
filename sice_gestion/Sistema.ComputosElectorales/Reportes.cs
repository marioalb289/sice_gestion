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

namespace Sistema.ComputosElectorales
{
    public partial class Reportes : Form
    {
        private ComputosElectoralesGenerales CompElec;
        private MsgBox msgBox;
        private int pageNumber = 1;
        int totalPages = 0;
        public Reportes()
        {
            InitializeComponent();
            this.cargarComboDistrito();
        }

        private void cargarComboDistrito()
        {
            try
            {
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

        private void InicializarPaginador(int? distrito,int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                int totalCandidatos = CompElec.ListaCandidatos((int)distrito).Count+2;
                totalPages = CompElec.ResultadosSeccion(0,0,(int)distrito).Count / totalCandidatos;
                if(totalPages > 0)
                {
                    double res = (double)totalPages / (double)pageSize;
                    double flor = Math.Ceiling(res);
                    totalPages = Convert.ToInt32(flor);
                }
                int y = 0;
                int pz = pageSize * totalCandidatos;
                int pn = pz * (pageNumber - 1);
                cargarResultados(distrito,pn, pz);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void cargarResultados(int? distrito, int pageNumber, int pageSize)
        {
            try
            {
                CompElec = new ComputosElectoralesGenerales();
                List<VotosSeccion> vSeccion = CompElec.ResultadosSeccion(pageNumber, pageSize,(int)distrito);
                List<Candidatos> candidatos = CompElec.ListaCandidatos((int)distrito);
                dgvResultados.Columns.Clear();
                dgvResultados.DataSource = null;

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
                    columnaCandidato.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    columnaCandidato.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvResultados.Columns.Add(columnaCandidato);
                }
                //Agregar columnas adicionales
                DataGridViewTextBoxColumn NoRegColumna = new DataGridViewTextBoxColumn();
                NoRegColumna.Name = "no_registrados";
                NoRegColumna.HeaderText = "No Registrados";
                NoRegColumna.ValueType = typeof(System.Int32);
                NoRegColumna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                NoRegColumna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                NoRegColumna.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvResultados.Columns.Add(NoRegColumna);

                DataGridViewTextBoxColumn NulosColumna = new DataGridViewTextBoxColumn();
                NulosColumna.Name = "nulos";
                NulosColumna.HeaderText = "Nulos";
                NulosColumna.ValueType = typeof(System.Int32);
                NulosColumna.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                NulosColumna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
                int contCand = 4;
                DataGridViewRow row = (DataGridViewRow)dgvResultados.Rows[fila].Clone();
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
                            row.Cells[0].Value = v.id_casilla;
                            row.Cells[1].Value = v.seccion;
                            row.Cells[2].Value = v.casilla;

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
                            decimal Porcentaje1 = Math.Round((Convert.ToDecimal(Primero) * 100) / totalVotacionEmitida, 2);
                            decimal Porcentaje2 = Math.Round((Convert.ToDecimal(Seegundo) * 100) / totalVotacionEmitida, 2);
                            diferencia = Porcentaje1 - Porcentaje2;
                        }
                        row.Cells[3].Value = diferencia + "%";

                        //Votacion Emitida
                        row.Cells[contCand].Value = totalVotacionEmitida;

                        //Lista Nominal
                        row.Cells[contCand + 1].Value = Lnominal;

                        //Porcentaje de Participacion
                        row.Cells[contCand + 2].Value = Math.Round((Convert.ToDecimal(totalVotacionEmitida) * 100) / Lnominal, 2) + "%";

                        //Agregar fila
                        dgvResultados.Rows.Add(row);
                        fila++;
                        row = (DataGridViewRow)dgvResultados.Rows[fila].Clone();
                        contCand = 4;
                        vLst = new List<int>();
                        Noregynulo = 0;
                        //Inrementar filla
                    }

                    //Agregar Columnas
                    row.Cells[0].Value = v.id_casilla;
                    row.Cells[1].Value = v.seccion;
                    row.Cells[2].Value = v.casilla;
                    Lnominal = v.lista_nominal;

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
                    this.InicializarPaginador(selected);
                    btnPrimero.Enabled = !this.IsFirstPage();
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
    }
}
