# Sistema de Alquiler de Vehículos (WindowsFormsApp5)

## 1. Nombre y matrícula
- **Nombre:** Yariel Israel Nova Moreta 
- **Matrícula:** [2025-0306]

## 2. Descripción del programa
Aplicación de escritorio desarrollada en **C# (.NET Windows Forms)** para gestionar alquileres de vehículos[cite: 2, 5]. Permite registrar datos del cliente y fecha de salida, agregar uno o varios vehículos con su cantidad de días, seguro, GPS y tipo de entrega, y calcular el subtotal, descuento corporativo, domicilio, ITBIS y total a pagar con formato en pesos dominicanos (`RD$ 0.00`)[cite: 1, 3, 4].

## 3. Datos de entrada
- **Cliente:** Nombre del cliente (`txtCliente` / `textBox1`)[cite: 1].
- **Cliente corporativo:** Casilla de verificación (`chkCorporativo` / `checkBox1`) para aplicar un **15 %** de descuento sobre el subtotal[cite: 1].
- **Fecha de salida:** Selector de fecha (`dtpSalida` / `dateTimePicker1`), inicializado en la fecha actual[cite: 1].
- **Vehículo:** Selección o nombre del vehículo (`cmbVehiculo` / `comboBox1` o `txtVehiculo`) y tarifa diaria base (`txtTarifa` / `textBox2`)[cite: 1, 3].
- **Días:** Cantidad de días de alquiler (`nudDias` / `numericUpDown1`), en un rango de **1 a 30** días[cite: 1].
- **Seguro:** Opción **Básico** (`rbtBasico` / `radioButton1`, por defecto) o **Completo** (`rbtCompleto` / `radioButton2`, **RD$ 300.00** por día adicional)[cite: 1].
- **GPS:** Casilla (`chkGPS` / `checkBox2`) que suma **RD$ 100.00** por día[cite: 1].
- **Entrega:** Opción **En sucursal** (`rbtSucursal` / `radioButton3`, por defecto) o **Domicilio** (`rbtDomicilio` / `radioButton4`, cargo único de **RD$ 500.00**)[cite: 1].

## 4. Proceso
- **Al cargar (`Load`):** Configura los valores iniciales, establece la fuente `Consolas` en el encabezado y la lista de alquileres, muestra los títulos de columnas (`Días`, `Vehículo`, `Tarifa`, `Total`) y limpia los campos y totales[cite: 1, 3].
- **Agregar al alquiler:** Valida la selección/ingreso del vehículo y la tarifa, calcula la tarifa diaria efectiva (tarifa base + seguro completo si aplica + GPS si aplica) y el total de la línea (`tarifaDiaria * dias`), formatea la fila con anchos de columna **6, 24, 10 y 12** (`PadRight`/`PadLeft`) incluyendo los extras entre paréntesis junto al vehículo, acumula el `subtotal` en una variable `decimal` y reinicia los controles del vehículo[cite: 1, 3].
- **Calcular total:** Verifica que exista al menos un vehículo agregado (si no, muestra un aviso y detiene el cálculo), calcula:
  - `descuento` = `subtotal * 0.15` (si `Cliente corporativo` está marcado; si no, `0`)[cite: 1].
  - `domicilio` = `500` (si `Domicilio` está seleccionado; si no, `0`)[cite: 1].
  - `baseImponible` = `subtotal - descuento + domicilio`[cite: 1].
  - `itbis` = `baseImponible * 0.18` (18 % sobre la base imponible)[cite: 1].
  - `total` = `baseImponible + itbis`[cite: 1].
- **Nuevo alquiler / Limpiar:** Restablece todos los campos, limpia la lista y el `subtotal` a `0`, deja los resultados en `RD$ 0.00` y devuelve el foco al campo del cliente[cite: 1, 3].
- **Salir:** Cierra la aplicación[cite: 1].

## 5. Salida
- **Listado de alquileres (`lstAlquileres` / `listBox1`):** Muestra cada vehículo agregado en columnas alineadas con `Días`, `Vehículo (extras)`, `Tarifa` y `Total`[cite: 1, 3].
- **Resumen de costos (`RD$ 0.00`):** Muestra **Subtotal**, **Descuento**, **Domicilio**, **ITBIS (18%)** y **Total a pagar**[cite: 1, 3, 4].
- **Mensajes de validación/aviso:** Alertas si falta el vehículo, la tarifa no es válida o se intenta calcular sin vehículos agregados[cite: 1, 3].

## 6. Capturas de pantalla del programa en ejecución

pantalla de inicio

<img width="754" height="410" alt="image" src="https://github.com/user-attachments/assets/4da33e84-eb84-4abe-8b0d-567d044584fc" />

salida

<img width="746" height="460" alt="image" src="https://github.com/user-attachments/assets/7289fe5f-7aa8-48df-bb72-d5b8f3498dd1" />
