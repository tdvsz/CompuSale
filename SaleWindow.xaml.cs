using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        
        public class DeliveryMethod
        {
            public int ID { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Name; // Мы будем показывать только название в ComboBox
            }
        }
        
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=../../DataBase/information_system.accdb;";
        private List<string> clientSuggestions = new List<string>();
        private int currentSaleId = -1;
        private int currentClientId = -1;// ID текущей продажи, должен задаваться при создании новой продажи
        private bool isEditMode = false;

        public SaleWindow()
        {
            InitializeComponent();
            LoadClientSuggestions();
            LoadDataIntoComboBox();
        }
        
        private void LoadDataIntoComboBox()
        {
            string query = "SELECT ID_способа_доставки, Название FROM Способ_доставки";

            DeliveryComboBox.Items.Clear();
    
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);

                try
                {
                    connection.Open();
            
                    OleDbDataReader reader = command.ExecuteReader();

                    // Чтение данных и добавление в ComboBox
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["ID_способа_доставки"]);
                        string name = reader["Название"].ToString();

                        // Создаем новый объект DeliveryMethod и добавляем в ComboBox
                        DeliveryMethod deliveryMethod = new DeliveryMethod
                        {
                            ID = id,
                            Name = name
                        };

                        DeliveryComboBox.Items.Add(deliveryMethod);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
                }
            }
        }

        
        private void LoadClientSuggestions()
        {
            string query = "SELECT Название FROM Клиент";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    clientSuggestions.Add(reader["Название"].ToString());
                }
            }
        }
        
        private void ClientTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            clientWatermark.Visibility = string.IsNullOrEmpty(clientTextBox.Text) ? Visibility.Visible : Visibility.Hidden;

            string input = clientTextBox.Text.ToLower();
            if (string.IsNullOrEmpty(input))
            {
                clientSuggestionsListBox.Visibility = Visibility.Collapsed;
                return;
            }

            var filteredSuggestions = clientSuggestions.FindAll(s => s.ToLower().Contains(input));
            clientSuggestionsListBox.ItemsSource = filteredSuggestions;
            clientSuggestionsListBox.Visibility = filteredSuggestions.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        
        private void ClientSuggestionsListBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (clientSuggestionsListBox.SelectedItem != null)
            {
                clientTextBox.Text = clientSuggestionsListBox.SelectedItem.ToString();
                clientSuggestionsListBox.Visibility = Visibility.Collapsed;
                // suggestionsListBox.SelectedItem = null;
            }
        }
        
        private void EnterPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
                e.Handled = true;
            }
        }
        
        private void clientTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                clientSuggestionsListBox.Visibility = Visibility.Collapsed;

                clientTextBox.Clear();
                clientTextBox.Focus();
            }
        }
        
        private int GetClientId(string clientName)
        {
            string query = "SELECT ID_клиента FROM Клиент WHERE Название = @Название";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@Название", clientName);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
                catch
                {
                    return -1;
                }
            }
        }
        
        private string GetClientNameById(int clientId)
        {
            string query = "SELECT Название FROM Клиент WHERE ID_клиента = @ID_клиента";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_клиента", clientId);

                connection.Open();
                object result = command.ExecuteScalar();
                return result != null ? result.ToString() : string.Empty;
            }
        }
        
        public void LoadClientDataById(int clientId)
        {
            string query = "SELECT Название FROM Клиент WHERE ID_клиента = @ID_клиента";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_клиента", clientId);

                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    clientTextBox.Text = reader["Название"].ToString();

                    // Установите флаг редактирования
                    isEditMode = true;
                    currentClientId = clientId;
                }
            }
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
            
            UpdateTotalPrice();
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
            UpdateTotalPrice();
        }
        
        private void CountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CountWatermark.Visibility = string.IsNullOrEmpty(CountTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }
        
        private void UpdateProductQuantityInSalesItem(int productId, int newQuantity)
        {
            string query = "UPDATE Элемент_продажи SET Количество = @Количество WHERE ID_продажи = @ID_продажи AND ID_товара = @ID_товара";
    
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@Количество", newQuantity);
                command.Parameters.AddWithValue("@ID_продажи", currentSaleId);
                command.Parameters.AddWithValue("@ID_товара", productId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    ReloadDataGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении количества товара: " + ex.Message);
                }
            }
        }
        
        private decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;
            string query = "SELECT Элемент_продажи.Количество, Товар.Цена " +
                           "FROM Элемент_продажи " +
                           "INNER JOIN Товар ON Элемент_продажи.ID_товара = Товар.ID_товара " +
                           "WHERE Элемент_продажи.ID_продажи = @ID_продажи";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_продажи", currentSaleId);

                try
                {
                    connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int quantity = Convert.ToInt32(reader["Количество"]);
                        decimal price = Convert.ToDecimal(reader["Цена"]);
                
                        totalPrice += quantity * price; // Добавляем цену товара с учетом количества
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при вычислении общей стоимости: " + ex.Message);
                }
            }
            return totalPrice;
        }
        
        private void UpdateTotalPrice()
        {
            decimal totalPrice = CalculateTotalPrice();
            totalPriceTextBox.Text = $"Итого: {totalPrice.ToString()} BYN";
            
            // Проверяем, установлен ли текущий ID продажи
            if (currentSaleId <= 0)
            {
                MessageBox.Show("Не удалось сохранить общую стоимость. Неверный ID продажи.");
                return;
            }

            string query = "UPDATE Продажа SET Общая_стоимость = @Общая_стоимость WHERE ID_продажи = @ID_продажи";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);

                // Добавляем параметры для SQL-запроса
                command.Parameters.AddWithValue("@Общая_стоимость", totalPrice);
                command.Parameters.AddWithValue("@ID_продажи", currentSaleId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении общей стоимости: " + ex.Message);
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
        
        private void SaveCountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProductsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                // Получаем ID_товара из выделенной строки
                int productId = Convert.ToInt32(selectedRow["ID_товара"]);
        
                // Получаем новое количество из TextBox
                if (int.TryParse(CountTextBox.Text, out int newQuantity))
                {
                    // Обновляем количество в базе данных
                    UpdateProductQuantityInSalesItem(productId, newQuantity);
                    
                    ReloadDataGrid();
                }
                else
                {
                    MessageBox.Show("Введите корректное количество.");
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для обновления.");
            }
        }
        
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = saleDatePicker.SelectedDate ?? DateTime.Now;
            
            DeliveryMethod selectedDeliveryMethod = (DeliveryMethod)DeliveryComboBox.SelectedItem;
            
            if (selectedDeliveryMethod == null)
            {
                MessageBox.Show("Пожалуйста, выберите способ доставки.");
                return;
            }

            int deliveryMethodId = selectedDeliveryMethod.ID;
            
            string adress = AdressTextBox.Text;
            
            // Получаем имя клиента из TextBox
            string clientName = clientTextBox.Text.Trim();

            // Проверяем, существует ли клиент
            currentClientId = GetClientId(clientName);
            
            string selectedStatus = (StatusComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            
            // SQL-запрос для добавления новой продажи в базу данных
            string query = "INSERT INTO Продажа (Дата_продажи, Статус, Общая_стоимость, адрес_доставки, ID_сотрудника, ID_клиента, ID_способа_доставки) " +
                           "VALUES (@Дата_продажи, @Статус, @Общая_стоимость, @адрес_доставки, @ID_сотрудника, @ID_клиента, @ID_способа_доставки)";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);

                // Устанавливаем параметры для SQL-запроса
                command.Parameters.AddWithValue("@Дата_продажи", selectedDate);  // Текущая дата
                command.Parameters.AddWithValue("@Статус", selectedStatus);
                command.Parameters.AddWithValue("@Общая_стоимость", 0);          // Начальная общая стоимость
                command.Parameters.AddWithValue("@адрес_доставки", adress); // Примерный адрес доставки
                command.Parameters.AddWithValue("@ID_сотрудника", EmployeeID);
                command.Parameters.AddWithValue("@ID_клиента", currentClientId);               // Примерный ID клиента
                command.Parameters.AddWithValue("@ID_способа_доставки", deliveryMethodId);      // Примерный способ доставки

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

        public void EditSale(int saleId)
        {
            this.Width = 450;
            this.Height = 300;
            DeliveryComboBox.Visibility = Visibility.Collapsed;
            clientTextBox.Visibility = Visibility.Collapsed;
            clientWatermark.Visibility = Visibility.Collapsed;
            CountTextBox.Visibility = Visibility.Collapsed;
            CountWatermark.Visibility = Visibility.Collapsed;
            employeeNameTextBox.Visibility = Visibility.Collapsed;
            totalPriceTextBox.Visibility = Visibility.Collapsed;
            searchTextBox.Visibility = Visibility.Collapsed;
            searchWatermark.Visibility = Visibility.Collapsed;
            searchBtn.Visibility = Visibility.Collapsed;
            SaveCountBtn.Visibility = Visibility.Collapsed;
            saleDatePicker.Visibility = Visibility.Collapsed;
            saveBtn.Visibility = Visibility.Collapsed;
            searchResultsListBox.Visibility = Visibility.Collapsed;
            selectedProductsDataGrid.Margin = new Thickness(22, 0, 601, 318);
            StatusComboBox.Margin = new Thickness(22, 210, 696, 278);

            StatusComboBox.Items.Clear();
            StatusComboBox.Items.Add("Новый");
            StatusComboBox.Items.Add("В процессе");
            StatusComboBox.Items.Add("Завершен");
            StatusComboBox.Items.Add("Отменен");

            // Установка текущего статуса продажи из базы данных
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                string saleQuery = "SELECT Статус FROM Продажа WHERE ID_продажи = @ID_продажи";
                OleDbCommand saleCommand = new OleDbCommand(saleQuery, connection);
                saleCommand.Parameters.AddWithValue("@ID_продажи", saleId);

                connection.Open();
                OleDbDataReader saleReader = saleCommand.ExecuteReader();
                if (saleReader.Read())
                {
                    string currentStatus = saleReader["Статус"].ToString();
                    StatusComboBox.SelectedItem = currentStatus;  // Устанавливаем текущий статус
                }
                saleReader.Close();
            }

            // Обработчик изменения статуса с сохранением в БД
            StatusComboBox.SelectionChanged += (s, e) =>
            {
                SaveStatus(saleId);
            };
        }

        private void SaveStatus(int saleId)
        {
            string selectedStatus = StatusComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedStatus))
            {
                MessageBox.Show("Пожалуйста, выберите статус.");
                return;
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                string updateQuery = "UPDATE Продажа SET Статус = @Статус WHERE ID_продажи = @ID_продажи";
                OleDbCommand updateCommand = new OleDbCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@Статус", selectedStatus);
                updateCommand.Parameters.AddWithValue("@ID_продажи", saleId);

                connection.Open();
                updateCommand.ExecuteNonQuery();

                MessageBox.Show("Статус продажи обновлен.");
            }
        }

        private void AdressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdressWatermark.Visibility = string.IsNullOrEmpty(AdressWatermark.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void comboBoxDeliveryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeliveryComboBox.SelectedItem != null &&
                DeliveryComboBox.SelectedItem.ToString() == "Доставка по адресу")
            {
                AdressTextBox.Visibility = Visibility.Visible;
                AdressWatermark.Visibility = Visibility.Visible;
            }
            else
            {
                AdressTextBox.Visibility = Visibility.Collapsed;
                AdressWatermark.Visibility = Visibility.Collapsed;
            }
        }
    }
}