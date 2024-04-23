using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Globalization;
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        private int id = 1;
        string connectionString = "Data Source=SoftClarita.db;Version=3;";
        public Form1()
        {
            InitializeComponent();           

            dataGridView1.Columns.Add("ID", "ID");
            dataGridView1.Columns.Add("Descripcion", "DESC");
            dataGridView1.Columns.Add("Cantidad", "CANT");
            dataGridView1.Columns.Add("PrecioDolares", "Precio$");
            dataGridView1.Columns.Add("PrecioBolivares", "PrecioBs");           
            dataGridView1.Columns.Add("IVA", "IVA");
            dataGridView1.Columns.Add("Subtotal", "SUBTTL");

            // Agregar columna de botón de eliminación
            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
            deleteButtonColumn.HeaderText = "Acciones";
            deleteButtonColumn.Name = "Eliminar";
            deleteButtonColumn.Text = "Eliminar";
            deleteButtonColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(deleteButtonColumn);

            // Evento para manejar el clic en el botón de eliminación
            dataGridView1.CellContentClick += DataGridView1_CellContentClick;

            // Cambiar el color de fondo de las celdas
            dataGridView1.DefaultCellStyle.BackColor = Color.LightGray;

            // Cambiar el color de fondo de las filas alternas
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

            // Cambiar el color de fondo de las celdas seleccionadas
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.SteelBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            // Cambiar el estilo de borde de las celdas
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;

            // Cambiar el estilo de borde de las filas
            dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Cambiar el estilo de fuente de las celdas
            dataGridView1.DefaultCellStyle.Font = new Font("Arial", 10);

            // Cambiar el estilo de fuente de los encabezados de columna
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);

            // Cambiar el color de fondo de los encabezados de columna
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkGray;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            // Alinear el texto en las celdas
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Cambiar el tamaño de las columnas
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Deshabilitar la selección de múltiples celdas
            dataGridView1.MultiSelect = false;


        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar si se hizo clic en un botón de eliminación
            if (e.ColumnIndex == dataGridView1.Columns["Eliminar"].Index && e.RowIndex >= 0)
            {
                // Mostrar un mensaje de confirmación
                DialogResult result = MessageBox.Show("¿Estás seguro de que quieres eliminar esta fila?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Eliminar la fila seleccionada
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                    CalcularTotal();
                }
            }
        }



        private void btnAgregarItem_Click(object sender, EventArgs e)
        {
                    


            // Obtener los valores ingresados en los TextBox
            string descripcion = txt_Description.Text;
            decimal precio = Convert.ToDecimal(txt_price.Text);
            int cantidad = Convert.ToInt32(txt_Cantidad.Text);
            decimal tasa = Convert.ToDecimal(txt_Tasa.Text);
            decimal iva = 16.00M; // Cambiado a decimal


            // Calcular importe en dólares y en bolívares
            decimal importeDolar = precio * cantidad;


            decimal precioxTasa = Math.Round(tasa * precio, 2);
            string precioxTasaFormateado = precioxTasa.ToString("N2");
      

            decimal importeBs = precioxTasa * cantidad;
            string importeBsFormateado = importeBs.ToString("N2");

            // Calcular el IVA
            decimal impuesto = Math.Round(importeBs * (iva / 100), 2);
            string impuestoFormateado = impuesto.ToString("N2");

            // Calcular el subtotal
            decimal subtotal = Math.Round(importeBs + impuesto, 2);
            string subtotalFormateado = subtotal.ToString("N2");

            // Agregar una nueva fila al DataGridView y asignar los valores calculados
            dataGridView1.Rows.Add(id, descripcion, cantidad, precio, importeBsFormateado, impuestoFormateado, subtotalFormateado);

            // Incrementar el ID para el próximo ítem
            id++;

            CalcularTotal();

            //Borramos las casillas de los textbox
            txt_Cantidad.Clear();
            txt_Description.Clear();
            txt_price.Clear();        
        }

        private void CalcularTotal()
        {
            decimal total = 0;
            decimal iva = 0;
            decimal subtotalbs = 0;

            // Recorrer todas las filas del DataGridView y sumar el subtotal de cada una
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                total += Convert.ToDecimal(row.Cells["Subtotal"].Value);
                iva += Convert.ToDecimal(row.Cells["IVA"].Value);
                subtotalbs += Convert.ToDecimal(row.Cells["PrecioBolivares"].Value);
            }

            // Mostrar el total en un TextBox u otro control
            lblTotal.Text = total.ToString("N2");
            lbl_iva.Text = iva.ToString("N2");
            lblSubtotal.Text = subtotalbs.ToString("N2");
        }

        private void txt_Tasa_KeyPress(object sender, KeyPressEventArgs e)
        {
          
                // Obtener el separador decimal de la configuración regional actual
                char separadorDecimal = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                // Permitir solo dígitos, la tecla de retroceso y el separador decimal
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != separadorDecimal)
                {
                    // Si el usuario intenta ingresar un punto decimal, reemplazarlo por el separador decimal correspondiente
                    if (e.KeyChar == '.')
                    {
                        e.KeyChar = separadorDecimal;
                    }
                    else
                    {
                        // Si se presiona una tecla no válida, ignorarla
                        e.Handled = true;
                    }
                }
            
        }


        public void createbd()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Comando para crear la tabla "invoice"
                string createInvoiceTableQuery = @"CREATE TABLE IF NOT EXISTS invoice (
                                                id INTEGER PRIMARY KEY AUTO_INCREMENT,
                                                client_id INTEGER,
                                                product_id INTEGER,
                                                quantity INTEGER,
                                                price REAL,
                                                total REAL,
                                                date TEXT);";
                SqlCommand createInvoiceTableCommand = new SqlCommand(createInvoiceTableQuery, connection);
                createInvoiceTableCommand.ExecuteNonQuery();

                // Comando para crear la tabla "clients"
                string createClientsTableQuery = @"CREATE TABLE IF NOT EXISTS clients (
                                                id INTEGER PRIMARY KEY AUTO_INCREMENT,
                                                rif TEXT,
                                                razonsocial TEXT);";
                SqlCommand createClientsTableCommand = new SqlCommand(createClientsTableQuery, connection);
                createClientsTableCommand.ExecuteNonQuery();

                // Comando para crear la tabla "product"
                string createProductTableQuery = @"CREATE TABLE IF NOT EXISTS product (
                                                id INTEGER PRIMARY KEY AUTO_INCREMENT,
                                                descripcion TEXT,
                                                price REAL);";
                SqlCommand createProductTableCommand = new SqlCommand(createProductTableQuery, connection);
                createProductTableCommand.ExecuteNonQuery();

                Console.WriteLine("Base de datos y tablas creadas con éxito (si no existían).");
            }
        }

        public void conexion()
        {
            string connectionString = "Data Source=mydatabase.db;Version=3;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Comando para crear la tabla
                string createTableQuery = "CREATE TABLE IF NOT EXISTS mytable (id INTEGER PRIMARY KEY, name TEXT, age INTEGER);";
                SqlCommand createTableCommand = new SqlCommand(createTableQuery, connection);
                createTableCommand.ExecuteNonQuery();
            }
        }
                     
        private void GuardarFactura()
        {
            // Obtener los valores ingresados en los TextBox
            string descripcion = txt_Description.Text;
            decimal precio = Convert.ToDecimal(txt_price.Text);
            int cantidad = Convert.ToInt32(txt_Cantidad.Text);

        }



        /*
        public void conexionbd()
        {
            // Conexión a la base de datos SQLite
            string connectionString = "Data Source=mi_base_de_datos.db;Version=3;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Crear una tabla
                string createTableQuery = "CREATE TABLE IF NOT EXISTS Usuarios (ID INTEGER PRIMARY KEY AUTOINCREMENT, Nombre TEXT, Edad INTEGER)";
                using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Insertar datos en la tabla
                string insertDataQuery = "INSERT INTO Usuarios (Nombre, Edad) VALUES ('Juan', 30)";
                using (SqlCommand command = new SqlCommand(insertDataQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                insertDataQuery = "INSERT INTO Usuarios (Nombre, Edad) VALUES ('María', 25)";
                using (SqlCommand command = new SqlCommand(insertDataQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Consulta para leer los datos insertados
                string selectQuery = "SELECT * FROM Usuarios";
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["ID"]}, Nombre: {reader["Nombre"]}, Edad: {reader["Edad"]}");
                        }
                    }
                }

                connection.Close();
            }
        }//Conexionbd
        */

    }

        
    }
