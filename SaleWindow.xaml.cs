using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;

namespace CompuSale
{
    /// <summary>
    /// Логика взаимодействия для SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window
    {
        public int employeeID;
        public int EmployeeID
        {
            get {return employeeID;}
            set 
            { 
                Console.WriteLine("EmployeeID sale step: " + value);
                employeeID = value; 
            }
        }
        public string EmployeeName
        {
            get { return employeeNameTextBox.Text; }
            set { employeeNameTextBox.Text = value; }
        }
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=../../DataBase/information_system.accdb;";
        private int currentSaleId = -1; // ID текущей продажи, должен задаваться при создании новой продажи
        private bool isEditMode = false;

        public SaleWindow()
        {
            InitializeComponent();
            debug.Text = EmployeeID.ToString();
        }

        // Класс для хранения данных о товаре в DataGrid
        public class ProductForSale
        {
            public string Название { get; set; }
            public decimal Цена { get; set; }
            public int Количество { get; set; }
            public decimal ОбщаяЦена => Цена * Количество;
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchWatermark.Visibility = string.IsNullOrEmpty(searchTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        // Метод для поиска товаров в базе данных
        private void SearchProducts(string searchText)
        {
            string query = "SELECT ID_товара, Название, Цена FROM Товар WHERE Название LIKE @searchText";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();

                    searchResultsListBox.Items.Clear();
                    while (reader.Read())
                    {
                        // Отображаем Название и сохраняем ID в Tag для удобства
                        ListBoxItem item = new ListBoxItem
                        {
                            Content = $"{reader["Название"]} - {reader["Цена"]} BYN",
                            Tag = reader["ID_товара"] // ID сохраняем для дальнейшего использования
                        };
                        searchResultsListBox.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при поиске товаров: " + ex.Message);
                }
            }
        }

        // Обработчик выбора элемента из списка поиска
        private void SearchResultsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchResultsListBox.SelectedItem is ListBoxItem selectedItem)
            {
                int productId = (int)selectedItem.Tag;

                AddProductToDataGrid();
                AddProductToSalesItemTable(productId, 1); // Количество по умолчанию 1

                ReloadDataGrid();
                searchResultsListBox.SelectedItem = null;
            }
        }

        private void ReloadDataGrid()
        {
            string query = "SELECT Элемент_продажи.ID_товара, Товар.Название, Элемент_продажи.Количество FROM Элемент_продажи INNER JOIN Товар ON Элемент_продажи.ID_товара = Товар.ID_товара WHERE Элемент_продажи.ID_продажи = @ID_продажи";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_продажи", currentSaleId);
                OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                selectedProductsDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        // Вывод в datagrid
        private void AddProductToDataGrid()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Добавить условие по текущей продаже
                    string query =
                        "SELECT Элемент_продажи.ID_товара, Товар.Название, Элемент_продажи.Количество " +
                        "FROM Элемент_продажи " +
                        "INNER JOIN Товар ON Элемент_продажи.ID_товара = Товар.ID_товара " +
                        "WHERE Элемент_продажи.ID_продажи = @ID_продажи";  // Фильтруем по ID_продажи
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@ID_продажи", currentSaleId); // Указание ID текущей продажи
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    selectedProductsDataGrid.ItemsSource = dataTable.DefaultView;
                    selectedProductsDataGrid.Columns[0].Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
        }

        // Добавления товара в таблицу Элемент_продажи
        private void AddProductToSalesItemTable(int productId, int quantity)
        {
            string query = "INSERT INTO Элемент_продажи (ID_продажи, ID_товара, Количество) VALUES (@ID_продажи, @ID_товара, @Количество)";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);

                // Параметры для SQL-запроса
                command.Parameters.AddWithValue("@ID_продажи", currentSaleId);  // ID текущей продажи
                command.Parameters.AddWithValue("@ID_товара", productId);       // ID товара
                command.Parameters.AddWithValue("@Количество", quantity);       // Количество товара

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении товара: " + ex.Message);
                }
            }
        }
        
        public void LoadSaleById(int saleId)
        {
            string query = "SELECT * FROM Элемент_продажи WHERE ID_продажи = @ID_продажи";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_продажи", saleId);

                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // Запрос для получения товаров, связанных с конкретной продажей
                    query = "SELECT Элемент_продажи.ID_товара, Товар.Название, Элемент_продажи.Количество " +
                            "FROM Элемент_продажи " +
                            "INNER JOIN Товар ON Элемент_продажи.ID_товара = Товар.ID_товара " +
                            "WHERE Элемент_продажи.ID_продажи = @ID_продажи"; // Фильтрация по ID_продажи

                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@ID_продажи", saleId); // Добавляем параметр для фильтрации
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    selectedProductsDataGrid.ItemsSource = dataTable.DefaultView;

                    isEditMode = true;
                    currentSaleId = saleId;
                }
            }
        }
        
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            string searchText = searchTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                SearchProducts(searchText);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // SQL-запрос для добавления новой продажи в базу данных
            string query = "INSERT INTO Продажа (Дата_продажи, Статус, Общая_стоимость, адрес_доставки, ID_сотрудника, ID_клиента, ID_способа_доставки) " +
                           "VALUES (@Дата_продажи, @Статус, @Общая_стоимость, @адрес_доставки, @ID_сотрудника, @ID_клиента, @ID_способа_доставки)";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);

                // Устанавливаем параметры для SQL-запроса
                command.Parameters.AddWithValue("@Дата_продажи", DateTime.Now.ToString("dd.MM.yyyy"));  // Текущая дата
                command.Parameters.AddWithValue("@Статус", "Ожидание");          // Начальный статус
                command.Parameters.AddWithValue("@Общая_стоимость", 0);          // Начальная общая стоимость
                command.Parameters.AddWithValue("@адрес_доставки", "Минск, улица Примерная, 123"); // Примерный адрес доставки
                command.Parameters.AddWithValue("@ID_сотрудника", EmployeeID);
                command.Parameters.AddWithValue("@ID_клиента", 3);               // Примерный ID клиента
                command.Parameters.AddWithValue("@ID_способа_доставки", 1);      // Примерный способ доставки

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    // Получаем ID новой продажи
                    command.CommandText = "SELECT @@IDENTITY";
                    currentSaleId = (int)command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при создании новой продажи: " + ex.Message);
                }
            }
        }
    }
}