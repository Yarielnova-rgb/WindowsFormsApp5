using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        // Mapeo de controles del segundo código
        private Control txtCliente => textBox1;
        private DateTimePicker dtpSalida => dateTimePicker1;
        private CheckBox chkCorporativo => checkBox1;
        private ComboBox cmbVehiculo => comboBox1;
        private TextBox txtTarifa => textBox2;
        private NumericUpDown nudDias => numericUpDown1;

        // Seguro y tipo de entrega
        private RadioButton rbtBasico => radioButton1;
        private RadioButton rbtCompleto => radioButton2;
        private RadioButton rbtSucursal => radioButton3;
        private RadioButton rbtDomicilio => radioButton4;
        private CheckBox chkGPS => checkBox2;

        private ListBox lstAlquileres => listBox1;

        // Recuadros de resultados (los de la derecha)
        private Control lblSubtotal => textBox3;
        private Control lblDescuento => textBox4;
        private Control lblDomicilio => textBox5;
        private Control lblITBIS => textBox6;
        private Control lblTotal => textBox7;

        // Botones y encabezado buscados automáticamente
        private Button btnAgregar;
        private Button btnCalcular;
        private Button btnNuevo;
        private Button btnSalir;
        private Label lblEncabezado;

        // Tarifas base por día de cada vehículo
        private readonly Dictionary<string, decimal> tarifas = new Dictionary<string, decimal>
        {
            { "Económico", 1500m },
            { "Sedán", 2200m },
            { "Jeepeta", 3500m }
        };

        // Valores fijos del enunciado
        private const decimal SEGURO_COMPLETO = 300m;   // por día
        private const decimal GPS_DIA = 100m;           // por día
        private const decimal DOMICILIO = 500m;         // una sola vez
        private const decimal DESC_CORPORATIVO = 0.15m; // 15 % del subtotal
        private const decimal ITBIS = 0.18m;            // 18 % de la base

        // Aquí se va acumulando el subtotal de todos los vehículos agregados
        private decimal subtotal = 0m;

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

        // Arma una línea de la tabla con los anchos pedidos: 6, 24, 10 y 12
        private string ArmarLinea(string dias, string vehiculo, string tarifa, string total)
        {
            return dias.PadRight(6) + vehiculo.PadRight(24) + tarifa.PadLeft(10) + total.PadLeft(12);
        }

        // Formato de dinero: RD$ 0.00
        private string Dinero(decimal valor)
        {
            return "RD$ " + valor.ToString("N2", CultureInfo.InvariantCulture);
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
                lblEncabezado.Text = ArmarLinea("Días", "Vehículo", "Tarifa", "Total");
            }

            LimpiarTodo();
        }

        private void cmbVehiculo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbVehiculo.SelectedItem != null)
                txtTarifa.Text = tarifas[cmbVehiculo.SelectedItem.ToString()].ToString("N2", CultureInfo.InvariantCulture);
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

            decimal tarifaBase;
            if (!decimal.TryParse(txtTarifa.Text.Trim(), NumberStyles.Number,
                                  CultureInfo.InvariantCulture, out tarifaBase) || tarifaBase <= 0)
            {
                MessageBox.Show("Tarifa por día no válida.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal tarifaDiaria = tarifaBase;
            if (rbtCompleto.Checked) tarifaDiaria += SEGURO_COMPLETO;
            if (chkGPS.Checked) tarifaDiaria += GPS_DIA;

            int dias = (int)nudDias.Value;
            decimal totalLinea = tarifaDiaria * dias;

            subtotal += totalLinea;

            string extras = "";
            if (rbtCompleto.Checked && chkGPS.Checked) extras = " (Completo + GPS)";
            else if (rbtCompleto.Checked) extras = " (Completo)";
            else if (chkGPS.Checked) extras = " (GPS)";

            lstAlquileres.Items.Add(ArmarLinea(
                dias.ToString(),
                vehiculo + extras,
                tarifaDiaria.ToString("N2", CultureInfo.InvariantCulture),
                totalLinea.ToString("N2", CultureInfo.InvariantCulture)));

            // Dejo los controles del vehículo listos para el siguiente
            cmbVehiculo.SelectedIndex = -1;
            txtTarifa.Clear();
            nudDias.Value = 1;
            rbtBasico.Checked = true;
            chkGPS.Checked = false;
            cmbVehiculo.Focus();
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            if (lstAlquileres.Items.Count == 0)
            {
                MessageBox.Show("Agregue al menos un vehículo al alquiler.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            decimal descuento = chkCorporativo.Checked ? subtotal * DESC_CORPORATIVO : 0m;
            decimal domicilio = rbtDomicilio.Checked ? DOMICILIO : 0m;
            decimal baseImponible = subtotal - descuento + domicilio;
            decimal itbis = baseImponible * ITBIS;
            decimal total = baseImponible + itbis;

            lblSubtotal.Text = Dinero(subtotal);
            lblDescuento.Text = Dinero(descuento);
            lblDomicilio.Text = Dinero(domicilio);
            lblITBIS.Text = Dinero(itbis);
            lblTotal.Text = Dinero(total);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarTodo();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LimpiarTodo()
        {
            subtotal = 0m;
            lstAlquileres.Items.Clear();

            txtCliente.Text = "";
            dtpSalida.Value = DateTime.Today;
            chkCorporativo.Checked = false;

            cmbVehiculo.SelectedIndex = -1;
            txtTarifa.Clear();
            nudDias.Value = 1;
            rbtBasico.Checked = true;
            chkGPS.Checked = false;
            rbtSucursal.Checked = true;

            lblSubtotal.Text = Dinero(0m);
            lblDescuento.Text = Dinero(0m);
            lblDomicilio.Text = Dinero(0m);
            lblITBIS.Text = Dinero(0m);
            lblTotal.Text = Dinero(0m);

            txtCliente.Focus();
        }

        // Métodos vacíos que quedaron conectados desde el diseñador.
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void checkBox2_CheckedChanged(object sender, EventArgs e) { }
        private void label11_Click(object sender, EventArgs e) { }
        private void label14_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}