using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompuSale
{
    public partial class MainWindow : Window
    {
        public string EmployeeName
        {
            get { return UserNameTextBlock.Text; }
            set { UserNameTextBlock.Text = value; }
        }

        public int employeeID;
        public int EmployeeID
        {
            get {return employeeID;}
            set { employeeID = value; }
        }
        
        public string userRole;
        public string UserRole {
            get { return userRole; }
            set
            {
                userRole = value;
                ApplyRoleSettings();
            } 
        }

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void ApplyRoleSettings()
        {
            if (userRole == "Администратор")
            {
                Console.WriteLine("Администратор авторизован");
            }
            else if (userRole == "Сотрудник")
            {
                Console.WriteLine("Сотрудник авторизован");
                
                employeesTreeViewItem.Visibility = Visibility.Collapsed;
                ReportButton.Visibility = Visibility.Collapsed;
            }
        }

        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=../../DataBase/information_system.accdb;";

        private int selectedProductId = -1;
        private int selectedClientId = -1;
        private int selectedManufacturerId = -1;
        private int selectedCategoryId = -1;
        private int selectedSaleId = -1;

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = listTreeView.SelectedItem as TreeViewItem;
            if (selectedItem != null)
            {
                if (selectedItem == productTreeViewItem)
                {
                    ProductWindow newWindow = new ProductWindow();
                    newWindow.Show();
                }
                if (selectedItem == clientsTreeViewItem)
                {
                    ClientWindow newWindow = new ClientWindow();
                    newWindow.Show();
                }
                if (selectedItem == saleTreeViewItem)
                {
                    SaleWindow newWindow = new SaleWindow();
                    newWindow.EmployeeID = EmployeeID;
                    newWindow.EmployeeName = EmployeeName;
                    newWindow.Show();
                }
                if (selectedItem == manufacturerTreeViewItem)
                {
                    ListsWindow newWindow = new ListsWindow
                    {
                        SelectedTreeViewItem = "Производитель"  // Устанавливаем значение для проверки
                    };
                    newWindow.Show();
                }
                if (selectedItem == categoryTreeViewItem)
                {
                    ListsWindow newWindow = new ListsWindow
                    {
                        SelectedTreeViewItem = "Категория"  // Устанавливаем значение для проверки
                    };
                    newWindow.Show();
                }
            }
            else
            {
                MessageBox.Show("Выберите раздел для добавления записи");
            }
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem selectedItem)
            {
                if (selectedItem == productTreeViewItem)
                {
                    LoadProducts();
                    if (userRole == "Сотрудник")
                    {
                        EditButton.Visibility = Visibility.Collapsed;
                        AddButton.Visibility = Visibility.Collapsed;
                        DeleteButton.Visibility = Visibility.Collapsed;
                    }
                    else if (userRole == "Администратор")
                    {
                        EditButton.Visibility = Visibility.Visible;
                        AddButton.Visibility = Visibility.Visible;
                        DeleteButton.Visibility = Visibility.Visible;
                    }
                }
                else if (selectedItem == categoryTreeViewItem)
                {
                    LoadCategories();
                    if (userRole == "Сотрудник")
                    {
                        EditButton.Visibility = Visibility.Collapsed;
                        AddButton.Visibility = Visibility.Collapsed;
                        DeleteButton.Visibility = Visibility.Collapsed;
                    }
                    else if (userRole == "Администратор")
                    {
                        EditButton.Visibility = Visibility.Visible;
                        AddButton.Visibility = Visibility.Visible;
                        DeleteButton.Visibility = Visibility.Visible;
                    }
                }
                else if (selectedItem == manufacturerTreeViewItem)
                {
                    LoadManufacturers();
                    if (userRole == "Сотрудник")
                    {
                        EditButton.Visibility = Visibility.Collapsed;
                        AddButton.Visibility = Visibility.Collapsed;
                        DeleteButton.Visibility = Visibility.Collapsed;
                    }
                    else if (userRole == "Администратор")
                    {
                        EditButton.Visibility = Visibility.Visible;
                        AddButton.Visibility = Visibility.Visible;
                        DeleteButton.Visibility = Visibility.Visible;
                    }
                }
                else if (selectedItem == clientsTreeViewItem)
                {
                    LoadClients();
                    if (userRole == "Сотрудник")
                    {
                        EditButton.Visibility = Visibility.Visible;
                        AddButton.Visibility = Visibility.Visible;
                        DeleteButton.Visibility = Visibility.Visible;
                    }
                    else if (userRole == "Администратор")
                    {
                        EditButton.Visibility = Visibility.Collapsed;
                        AddButton.Visibility = Visibility.Collapsed;
                        DeleteButton.Visibility = Visibility.Collapsed;
                    }
                }
                else if (selectedItem == clientTypeTreeViewItem)
                {
                    LoadClientTypes();
                    if (userRole == "Сотрудник")
                    {
                        EditButton.Visibility = Visibility.Collapsed;
                        AddButton.Visibility = Visibility.Collapsed;
                        DeleteButton.Visibility = Visibility.Collapsed;
                    }
                    else if (userRole == "Администратор")
                    {
                        EditButton.Visibility = Visibility.Visible;
                        AddButton.Visibility = Visibility.Visible;
                        DeleteButton.Visibility = Visibility.Visible;
                    }
                }
                else if (selectedItem == saleTreeViewItem)
                {
                    LoadSales();
                    if (userRole == "Сотрудник")
                    {
                        EditButton.Visibility = Visibility.Visible;
                        AddButton.Visibility = Visibility.Visible;
                        DeleteButton.Visibility = Visibility.Visible;
                    }
                    else if (userRole == "Администратор")
                    {
                        EditButton.Visibility = Visibility.Collapsed;
                        AddButton.Visibility = Visibility.Collapsed;
                        DeleteButton.Visibility = Visibility.Collapsed;
                    }
                }
                else if (selectedItem == deliveryTypeTreeViewItem)
                {
                    LoadDeliveryTypes();
                    if (userRole == "Сотрудник")
                    {
                        EditButton.Visibility = Visibility.Collapsed;
                        AddButton.Visibility = Visibility.Collapsed;
                        DeleteButton.Visibility = Visibility.Collapsed;
                    }
                    else if (userRole == "Администратор")
                    {
                        EditButton.Visibility = Visibility.Visible;
                        AddButton.Visibility = Visibility.Visible;
                        DeleteButton.Visibility = Visibility.Visible;
                    }
                }
                else if (selectedItem == employeesTreeViewItem)
                {
                    LoadEmployees();
                    if (userRole == "Сотрудник")
                    {
                        EditButton.Visibility = Visibility.Collapsed;
                        AddButton.Visibility = Visibility.Collapsed;
                        DeleteButton.Visibility = Visibility.Collapsed;
                    }
                    else if (userRole == "Администратор")
                    {
                        EditButton.Visibility = Visibility.Visible;
                        AddButton.Visibility = Visibility.Visible;
                        DeleteButton.Visibility = Visibility.Visible;
                    }
                }
                // string header = selectedItem.Header.ToString();
                // switch (header)
                // {
                //     case "Товар":
                //         LoadProducts();
                //         break;
                //
                //     case "Производители":
                //         LoadManufacturers();
                //         break;
                //     
                //     default:
                //         break;
                // }
            }
        }
        private void EmployeeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is DataRowView row)
            {
                if (row.Row.Table.Columns.Contains("ID_товара"))
                {
                    selectedProductId = Convert.ToInt32(row["ID_товара"]);
                }

                if (row.Row.Table.Columns.Contains("ID_производителя"))
                {
                    selectedManufacturerId = Convert.ToInt32(row["ID_производителя"]);
                }
                
                if (row.Row.Table.Columns.Contains("ID_категории"))
                {
                    selectedCategoryId = Convert.ToInt32(row["ID_категории"]);
                }

                if (row.Row.Table.Columns.Contains("ID_клиента"))
                {
                    selectedCategoryId = Convert.ToInt32(row["ID_клиента"]);
                }

                if (row.Row.Table.Columns.Contains("ID_продажи"))
                {
                    selectedSaleId = Convert.ToInt32(row["ID_продажи"]);
                }

                if (row.Row.Table.Columns.Contains("ID_сотрудника"))
                {
                    selectedSaleId = Convert.ToInt32(row["ID_сотрудника"]);
                }
            }
        }

        private void RenameColumns()
        {
            foreach (var column in EmployeeDataGrid.Columns)
            {
                switch (column.Header.ToString())
                {
                    case "ID_категории":
                        column.Header = "№ категории";
                        break;
                    case "ID_производителя":
                        column.Header = "№ производителя";
                        break;
                    case "ID_клиента":
                        column.Header = "№ клиента";
                        break;
                    case "ID_типа_клиента":
                        column.Header = "№ типа клиента";
                        break;
                    case "ID_продажи":
                        column.Header = "№ продажи";
                        break;
                    case "ID_сотрудника":
                        column.Header = "№ сотрудника";
                        break;
                    case "ID_способа_доставки":
                        column.Header = "№ способа доставки";
                        break;
                    case "Номер_телефона":
                        column.Header = "Телефон";
                        break;
                    case "Дата_продажи":
                        column.Header = "Дата";
                        break;
                    case "Общая_стоимость":
                        column.Header = "Общая стоимость";
                        break;
                    case "Адрес_доставки":
                        column.Header = "Адрес доставки";
                        break;
                    case "Количество_на_складе":
                        column.Header = "Количество на складе";
                        break;
                    case "Тип_клиента":
                        column.Header = "Тип клиента";
                        break;
                    case "Способ_доставки":
                        column.Header = "Способ доставки";
                        break;
                }
            }
        }

        private void LoadProducts()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query =
                        "SELECT \nТовар.ID_товара,    Товар.Название,\nПроизводитель.Название AS Производитель,\n    Категория.Название AS Категория,    Товар.Цена,\n    Товар.Количество_на_складе,\n    Товар.Описание FROM \n    (Товар\n    INNER JOIN Производитель ON Товар.ID_производителя = Производитель.ID_производителя)\n    INNER JOIN Категория ON Товар.ID_категории = Категория.ID_категории;\n";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    EmployeeDataGrid.ItemsSource = dataTable.DefaultView;
                    EmployeeDataGrid.Columns[0].Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
            RenameColumns();
        }
        private void LoadManufacturers()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ID_производителя, Название FROM Производитель";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    EmployeeDataGrid.ItemsSource = dataTable.DefaultView;
                    EmployeeDataGrid.Columns[0].Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
            RenameColumns();
        }
        private void LoadClients()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query =
                        "SELECT Клиент.ID_клиента, Клиент.Название, Клиент.Номер_телефона, Тип_клиента.Название AS Тип_клиента FROM Клиент INNER JOIN Тип_клиента ON Клиент.ID_типа_клиента = Тип_клиента.ID_типа_клиента";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    EmployeeDataGrid.ItemsSource = dataTable.DefaultView;
                    EmployeeDataGrid.Columns[0].Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
            RenameColumns();
        }
        private void LoadCategories()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ID_категории, Название FROM Категория";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    EmployeeDataGrid.ItemsSource = dataTable.DefaultView;
                    EmployeeDataGrid.Columns[0].Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
            RenameColumns();
        }
        private void LoadClientTypes()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Название FROM Тип_клиента";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    EmployeeDataGrid.ItemsSource = dataTable.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
            RenameColumns();
        }
        private void LoadSales()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Продажа.ID_продажи,\nПродажа.Дата_продажи,\nСотрудник.ФИО AS Сотрудник,\nПродажа.Статус,\nПродажа.Общая_стоимость,\nКлиент.Название AS Клиент,\nСпособ_доставки.Название AS Способ_доставки,\nПродажа.Адрес_доставки\nFROM ((Продажа\nINNER JOIN Сотрудник ON Продажа.ID_сотрудника = Сотрудник.ID_сотрудника)\nINNER JOIN Клиент ON Продажа.ID_клиента = Клиент.ID_клиента)\nINNER JOIN Способ_доставки ON Продажа.ID_способа_доставки = Способ_доставки.ID_способа_доставки";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    EmployeeDataGrid.ItemsSource = dataTable.DefaultView;
                    EmployeeDataGrid.Columns[0].Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
            RenameColumns();
        }
        private void LoadDeliveryTypes()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Название FROM Способ_доставки";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    EmployeeDataGrid.ItemsSource = dataTable.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
            RenameColumns();
        }
        private void LoadEmployees()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ID_сотрудника, ФИО, Логин, Пароль, Роль FROM Сотрудник";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    EmployeeDataGrid.ItemsSource = dataTable.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
            RenameColumns();
        }
        
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = listTreeView.SelectedItem as TreeViewItem;
            if (selectedItem != null)
            {
                if (selectedItem == productTreeViewItem)
                {
                    if (selectedProductId == -1)
                    {
                        MessageBox.Show("Выберите товар для редактирования");
                        return;
                    }

                    ProductWindow productWindow = new ProductWindow();
                    productWindow.LoadProductDataById(selectedProductId);
                    productWindow.ShowDialog();
                }
                if (selectedItem == clientsTreeViewItem)
                {
                    if (selectedClientId == -1)
                    {
                        MessageBox.Show("Выберите клиента для редактирования");
                        return;
                    }

                    ClientWindow clientWindow = new ClientWindow();
                    clientWindow.LoadClientDataById(selectedClientId);
                    clientWindow.ShowDialog();
                }
                if (selectedItem == saleTreeViewItem)
                {
                    if (selectedSaleId == -1)
                    {
                        MessageBox.Show("Выберите продажу для редактирования");
                        return;
                    }

                    SaleWindow saleWindow = new SaleWindow();
                    saleWindow.LoadSaleById(selectedSaleId);
                    saleWindow.EditSale(selectedSaleId);
                    saleWindow.ShowDialog();
                }
                if (selectedItem == manufacturerTreeViewItem)
                {
                    if (selectedManufacturerId == -1)
                    {
                        MessageBox.Show("Выберите производителя для редактирования");
                        return;
                    }

                    ListsWindow listsWindow = new ListsWindow
                    {
                        SelectedTreeViewItem = "Производитель"  // Устанавливаем значение для проверки
                    };
                    listsWindow.LoadManufacturerDataById(selectedManufacturerId);
                    listsWindow.ShowDialog();
                }
                if (selectedItem == categoryTreeViewItem)
                {
                    if (selectedCategoryId == -1)
                    {
                        MessageBox.Show("Выберите категорию для редактирования");
                        return;
                    }

                    ListsWindow listsWindow = new ListsWindow
                    {
                        SelectedTreeViewItem = "Категория"  // Устанавливаем значение для проверки
                    };
                    listsWindow.LoadCategoryDataById(selectedCategoryId);
                    listsWindow.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования");
            }
        }
        
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is DataRowView row)
            {
                int id = -1;
                
                if (row.Row.Table.Columns.Contains("ID_товара"))
                {
                    id = Convert.ToInt32(row["ID_товара"]);
                    DeleteRecordById("Товар", "ID_товара", id);
                }
                else if (row.Row.Table.Columns.Contains("ID_производителя"))
                {
                    id = Convert.ToInt32(row["ID_производителя"]);
                    DeleteRecordById("Производитель", "ID_производителя", id);
                }
                else if (row.Row.Table.Columns.Contains("ID_категории"))
                {
                    id = Convert.ToInt32(row["ID_категории"]);
                    DeleteRecordById("Категория", "ID_категории", id);
                }
                else if (row.Row.Table.Columns.Contains("ID_продажи"))
                {
                    id = Convert.ToInt32(row["ID_продажи"]);
                    DeleteRecordById("Продажа", "ID_продажи", id);
                }
                else if (row.Row.Table.Columns.Contains("ID_сотрудника"))
                {
                    id = Convert.ToInt32(row["ID_сотрудника"]);
                    DeleteRecordById("Сотрудник", "ID_продажи", id);
                }
                if (id != -1)
                {
                    row.Delete();
                }
                else
                {
                    MessageBox.Show("Ошибка: не удается найти идентификатор для удаления");
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления");
            }
        }
        
        private void DeleteRecordById(string tableName, string idColumn, int id)
        {
            string query = $"DELETE FROM {tableName} WHERE {idColumn} = @ID";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Запись успешно удалена");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти запись для удаления");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении данных: " + ex.Message);
                }
            }
        }

        private void SearchInDataGrid(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ShowAllRows();
                return;
            }

            bool found = false;

            foreach (var item in EmployeeDataGrid.Items)
            {
                var row = item as DataRowView;
                if (row != null)
                {
                    bool matchFound = false;

                    foreach (var column in row.Row.ItemArray)
                    {
                        if (column.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            matchFound = true;
                            break;
                        }
                    }

                    DataGridRow dataGridRow = EmployeeDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (dataGridRow != null)
                    {
                        if (matchFound)
                        {
                            dataGridRow.Visibility = Visibility.Visible;
                            found = true;
                        }
                        else
                        {
                            dataGridRow.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }

            if (!found)
            {
                ShowAllRows();
            }
        }

        private void ShowAllRows()
        {
            foreach (var item in EmployeeDataGrid.Items)
            {
                DataGridRow dataGridRow = EmployeeDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (dataGridRow != null)
                {
                    dataGridRow.Visibility = Visibility.Visible;
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginwindow = new LoginWindow();
            loginwindow.Show();
            this.Close();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchWatermark.Visibility = string.IsNullOrEmpty(searchTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            string searchText = searchTextBox.Text;
            SearchInDataGrid(searchText);
        }
    }
}