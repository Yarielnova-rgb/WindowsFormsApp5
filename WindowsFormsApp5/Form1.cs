using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
       
        private Control txtCliente => textBox1;
        private DateTimePicker dtpSalida => dateTimePicker1;
        private CheckBox chkCorporativo => checkBox1;
        private ComboBox cmbVehiculo => comboBox1;
        private TextBox txtTarifa => textBox2;
        private NumericUpDown nudDias => numericUpDown1;

        private RadioButton rbtBasico => radioButton1;
        private RadioButton rbtCompleto => radioButton2;
        private RadioButton rbtSucursal => radioButton3;
        private RadioButton rbtDomicilio => radioButton4;
        private CheckBox chkGPS => checkBox2;

        private ListBox lstAlquileres => listBox1;

        private Control lblSubtotal => textBox3;
        private Control lblDescuento => textBox4;
        private Control lblDomicilio => textBox5;
        private Control lblITBIS => textBox6;
        private Control lblTotal => textBox7;

        
        private Button btnAgregar;
        private Button btnCalcular;
        private Button btnNuevo;
        private Button btnSalir;
        private Label lblEncabezado;
        
        private class Linea
        {
            public string Vehiculo { get; set; }
            public decimal TarifaEfectiva { get; set; }
            public int Dias { get; set; }
            public decimal Total => TarifaEfectiva * Dias;
        }

        private readonly List<Linea> lineas = new List<Linea>();

        private readonly Dictionary<string, decimal> tarifas = new Dictionary<string, decimal>
        {
            { "Económico", 1500m },
            { "Sedán", 2200m },
            { "Jeepeta", 3500m }
        };

        private const decimal SEGURO_COMPLETO = 300m;
        private const decimal GPS_DIA = 100m;
        private const decimal DOMICILIO = 500m;
        private const decimal DESC_CORPORATIVO = 0.15m;
        private const decimal ITBIS = 0.18m;
        private const string FORMATO_FILA = "{0,-6}{1,-24}{2,10}{3,12}";

        public Form1()
        {
            InitializeComponent();

            btnAgregar = BuscarBoton(this, "Agregar al alquiler");
            btnCalcular = BuscarBoton(this, "Calcular total");
            btnNuevo = BuscarBoton(this, "Nuevo alquiler");
            btnSalir = BuscarBoton(this, "Salir");
            lblEncabezado = BuscarEncabezado();

            if (btnAgregar == null || btnCalcular == null || btnNuevo == null || btnSalir == null)
            {
                MessageBox.Show("No se encontró algún botón. Revise que los textos sean:\n" +
                                "Agregar al alquiler, Calcular total, Nuevo alquiler y Salir.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnAgregar.Click += btnAgregar_Click;
            btnCalcular.Click += btnCalcular_Click;
            btnNuevo.Click += btnNuevo_Click;
            btnSalir.Click += btnSalir_Click;
            cmbVehiculo.SelectedIndexChanged += cmbVehiculo_SelectedIndexChanged;
        }

        private Button BuscarBoton(Control padre, string texto)
        {
            foreach (Control c in padre.Controls)
            {
                if (c is Button b && b.Text.Trim() == texto) return b;
                Button r = BuscarBoton(c, texto);
                if (r != null) return r;
            }
            return null;
        }

        private Label BuscarEncabezado()
        {
            Label mejor = null;
            int mejorDist = int.MaxValue;

            foreach (Control c in lstAlquileres.Parent.Controls)
            {
                if (c is Label l)
                {
                    int dist = lstAlquileres.Top - l.Bottom;
                    bool encima = dist >= -5 && dist < 60;
                    bool alineado = l.Right > lstAlquileres.Left && l.Left < lstAlquileres.Right;
                    if (encima && alineado && dist < mejorDist)
                    {
                        mejor = l;
                        mejorDist = dist;
                    }
                }
            }
            return mejor;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbVehiculo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVehiculo.Items.Clear();
            foreach (var v in tarifas.Keys) cmbVehiculo.Items.Add(v);

            txtTarifa.ReadOnly = true;
            nudDias.Minimum = 1;
            nudDias.Maximum = 30;

            lstAlquileres.Font = new Font("Consolas", 9);
            if (lblEncabezado != null)
            {
                lblEncabezado.AutoSize = true;
                lblEncabezado.Font = new Font("Consolas", 9);
                lblEncabezado.Text = string.Format(FORMATO_FILA, "Días", "Vehículo", "Tarifa", "Total");
            }

            Reiniciar();
        }

        private void cmbVehiculo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbVehiculo.SelectedItem != null)
                txtTarifa.Text = tarifas[cmbVehiculo.SelectedItem.ToString()].ToString("N2");
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cmbVehiculo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un vehículo.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbVehiculo.Focus();
                return;
            }

            string vehiculo = cmbVehiculo.SelectedItem.ToString();
            decimal tarifa = tarifas[vehiculo];
            if (rbtCompleto.Checked) tarifa += SEGURO_COMPLETO;
            if (chkGPS.Checked) tarifa += GPS_DIA;

            var linea = new Linea
            {
                Vehiculo = vehiculo,
                TarifaEfectiva = tarifa,
                Dias = (int)nudDias.Value
            };
            lineas.Add(linea);

            string extras = "";
            if (rbtCompleto.Checked && chkGPS.Checked) extras = " (Completo + GPS)";
            else if (rbtCompleto.Checked) extras = " (Completo)";
            else if (chkGPS.Checked) extras = " (GPS)";

            lstAlquileres.Items.Add(string.Format(FORMATO_FILA,
                linea.Dias,
                linea.Vehiculo + extras,
                linea.TarifaEfectiva.ToString("N2"),
                linea.Total.ToString("N2")));
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCliente.Text))
            {
                MessageBox.Show("Ingrese el nombre del cliente.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCliente.Focus();
                return;
            }
            if (lineas.Count == 0)
            {
                MessageBox.Show("Agregue al menos un vehículo al alquiler.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal subtotal = lineas.Sum(l => l.Total);
            decimal descuento = chkCorporativo.Checked ? subtotal * DESC_CORPORATIVO : 0m;
            decimal domicilio = rbtDomicilio.Checked ? DOMICILIO : 0m;
            decimal baseImponible = subtotal - descuento + domicilio;
            decimal itbis = baseImponible * ITBIS;
            decimal total = baseImponible + itbis;

            lblSubtotal.Text = "RD$ " + subtotal.ToString("N2");
            lblDescuento.Text = "RD$ " + descuento.ToString("N2");
            lblDomicilio.Text = "RD$ " + domicilio.ToString("N2");
            lblITBIS.Text = "RD$ " + itbis.ToString("N2");
            lblTotal.Text = "RD$ " + total.ToString("N2");
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Reiniciar();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Reiniciar()
        {
            lineas.Clear();
            lstAlquileres.Items.Clear();

            txtCliente.Text = "";
            dtpSalida.Value = DateTime.Today;
            chkCorporativo.Checked = false;
            cmbVehiculo.SelectedIndex = -1;
            txtTarifa.Clear();
            nudDias.Value = 1;
            rbtBasico.Checked = true;
            rbtSucursal.Checked = true;
            chkGPS.Checked = false;

            string cero = "RD$ 0.00";
            lblSubtotal.Text = lblDescuento.Text = lblDomicilio.Text =
            lblITBIS.Text = lblTotal.Text = cero;

            txtCliente.Focus();
        }

        // Métodos vacíos que el diseñador puede tener conectados (no los borres)
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void checkBox2_CheckedChanged(object sender, EventArgs e) { }
        private void label11_Click(object sender, EventArgs e) { }
        private void label14_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
    }
}