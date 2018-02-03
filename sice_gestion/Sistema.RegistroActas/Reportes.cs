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

namespace Sistema.RegistroActas
{
    public partial class Reportes : Form
    {
        private RegistroActasGenerales rgActas;
        public Reportes()
        {
            InitializeComponent();
            this.cargarComboDistrito();
        }

        private void cargarComboDistrito()
        {
            try
            {
                rgActas = new RegistroActasGenerales();
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
                MessageBox.Show(ex.Message);
            }
        }

        private void cargarResultados(int? distrito)
        {
            try
            {
                List<VotosSeccion> vSeccion = rgActas.ResultadosSeccion((int)distrito);
                List<Candidatos> candidatos = rgActas.ListaCandidatos((int)distrito);
                dgvResultados.Columns.Clear();
                dgvResultados.DataSource = null;

                //Agregar Columnas Comunes

                DataGridViewTextBoxColumn seccionColumna = new DataGridViewTextBoxColumn();
                seccionColumna.Name = "seccion";
                seccionColumna.HeaderText = "Sección";
                seccionColumna.ValueType = typeof(System.Int32);
                dgvResultados.Columns.Add(seccionColumna);

                DataGridViewTextBoxColumn casillaColumna = new DataGridViewTextBoxColumn();
                casillaColumna.Name = "casilla";
                casillaColumna.HeaderText = "Casilla";
                casillaColumna.ValueType = typeof(string);
                dgvResultados.Columns.Add(casillaColumna);

                DataGridViewTextBoxColumn DferenciaColumna = new DataGridViewTextBoxColumn();
                DferenciaColumna.Name = "diferencia";
                DferenciaColumna.HeaderText = "Diferencia entre 1° y 2° Lugar";
                DferenciaColumna.ValueType = typeof(System.Int32);
                dgvResultados.Columns.Add(DferenciaColumna);


                //Agregar Columnas Caniddatos y Partidos
                foreach (Candidatos c in candidatos)
                {
                    DataGridViewTextBoxColumn columnaCandidato = new DataGridViewTextBoxColumn();
                    columnaCandidato.Name = c.partido;
                    columnaCandidato.HeaderText = c.partido;
                    columnaCandidato.ValueType = typeof(System.Int32);
                    dgvResultados.Columns.Add(columnaCandidato);
                }
                //Agregar columnas adicionales
                DataGridViewTextBoxColumn NoRegColumna = new DataGridViewTextBoxColumn();
                NoRegColumna.Name = "no_registrados";
                NoRegColumna.HeaderText = "No Registrados";
                NoRegColumna.ValueType = typeof(System.Int32);
                dgvResultados.Columns.Add(NoRegColumna);

                DataGridViewTextBoxColumn NulosColumna = new DataGridViewTextBoxColumn();
                NulosColumna.Name = "nulos";
                NulosColumna.HeaderText = "Nulos";
                NulosColumna.ValueType = typeof(System.Int32);
                dgvResultados.Columns.Add(NulosColumna);

                DataGridViewTextBoxColumn VtotalColumna = new DataGridViewTextBoxColumn();
                VtotalColumna.Name = "vtotal";
                VtotalColumna.HeaderText = "Votación total Emitida";
                VtotalColumna.ValueType = typeof(System.Int32);
                dgvResultados.Columns.Add(VtotalColumna);

                DataGridViewTextBoxColumn LnominalColumna = new DataGridViewTextBoxColumn();
                LnominalColumna.Name = "lnominal";
                LnominalColumna.HeaderText = "L. Nominal";
                LnominalColumna.ValueType = typeof(System.Int32);
                dgvResultados.Columns.Add(LnominalColumna);

                DataGridViewTextBoxColumn PparticipacionColumna = new DataGridViewTextBoxColumn();
                PparticipacionColumna.Name = "porcentajep";
                PparticipacionColumna.HeaderText = "Porcentaje Participación";
                PparticipacionColumna.ValueType = typeof(System.Decimal);
                dgvResultados.Columns.Add(PparticipacionColumna);

                //this.dgvResultados.Columns.Count
                int fila = 0;
                int idCasillaActual = 0;
                int cont = 1;
                int contCand = 3;
                DataGridViewRow row = (DataGridViewRow)dgvResultados.Rows[fila].Clone();
                //row.Cells[0].Value = 1;
                //dgvResultados.Rows.Add(row);
                

                foreach (VotosSeccion v in vSeccion)
                {
                    //idCasillaActual = (int)v.id_casilla;
                    //Agregar Columnas

                    if ((idCasillaActual != (int)v.id_casilla && idCasillaActual > 0) )
                    {   
                        //Diferencia entre el primero y segundo
                        row.Cells[2].Value = 1;
                        //Agregar fila
                        dgvResultados.Rows.Add(row);
                        fila++;
                        row = (DataGridViewRow)dgvResultados.Rows[fila].Clone();
                        contCand = 3;
                        //Inrementar filla
                    }

                    //Agregar Columnas
                    row.Cells[0].Value = v.seccion;
                    row.Cells[1].Value = v.casilla;
                    
                    row.Cells[contCand].Value = v.votos;

                    idCasillaActual = (int)v.id_casilla;
                    cont++;
                    contCand++;

                    if(cont == vSeccion.Count){
                        dgvResultados.Rows.Add(row);
                    }


                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbDistrito_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                int? selected = Convert.ToInt32(cmbDistrito.SelectedValue);
                if (selected > 0 && selected != null)
                    this.cargarResultados(selected);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
