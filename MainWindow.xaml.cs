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
            set 
            { 
                Console.WriteLine("EmployeeID 1 step: " + value);
                employeeID = value; 
            }
        }

        public MainWindow()
        {
            InitializeComponent();
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
                Console.WriteLine("Employee 2 step: " + EmployeeID);
                newWindow.EmployeeID = EmployeeID;
                newWindow.EmployeeName = EmployeeName;
                Console.WriteLine("Employee ID 3 step: " + EmployeeID);
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
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem selectedItem)
            {
                if (selectedItem == productTreeViewItem)
                {
                    LoadProducts();
                }
                else if (selectedItem == categoryTreeViewItem)
                {
                    LoadCategories();
                }
                else if (selectedItem == manufacturerTreeViewItem)
                {
                    LoadManufacturers();
                }
                else if (selectedItem == clientsTreeViewItem)
                {
                    LoadClients();
                }
                else if (selectedItem == clientTypeTreeViewItem)
                {
                    LoadClientTypes();
                }
                else if (selectedItem == saleTreeViewItem)
                {
                    LoadSales();
                }
                else if (selectedItem == deliveryTypeTreeViewItem)
                {
                    LoadDeliveryTypes();
                }
                else if (selectedItem == employeesTreeViewItem)
                {
                    LoadEmployees();
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
                
                if (row.Row.Table.Columns.Contains("ID_продажи"))
                {
                    selectedSaleId = Convert.ToInt32(row["ID_продажи"]);
                }
                
                if (row.Row.Table.Columns.Contains("ID_клиента"))
                {
                    selectedClientId = Convert.ToInt32(row["ID_клиента"]);
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
                        "SELECT \nТовар.ID_товара,    Товар.Название,\nПроизводитель.Название AS Название_производителя,\n    Категория.Название AS Название_категории,    Товар.Цена,\n    Товар.Количество_на_складе,\n    Товар.Описание FROM \n    (Товар\n    INNER JOIN Производитель ON Товар.ID_производителя = Производитель.ID_производителя)\n    INNER JOIN Категория ON Товар.ID_категории = Категория.ID_категории;\n";
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
        }
        
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = listTreeView.SelectedItem as TreeViewItem;
            if (selectedItem == productTreeViewItem)
            {
                if (selectedProductId == -1)
                {
                    MessageBox.Show("Выберите товар для редактирования.");
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
                    MessageBox.Show("Выберите клиента для редактирования.");
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
                    MessageBox.Show("Выберите продажу для редактирования.");
                    return;
                }
                
                SaleWindow saleWindow = new SaleWindow();
                saleWindow.LoadSaleById(selectedSaleId);
                saleWindow.ShowDialog();
            }
            if (selectedItem == manufacturerTreeViewItem)
            {
                if (selectedManufacturerId == -1)
                {
                    MessageBox.Show("Выберите товар для редактирования.");
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
                    MessageBox.Show("Выберите товар для редактирования.");
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
                if (id != -1)
                {
                    row.Delete();
                }
                else
                {
                    MessageBox.Show("Ошибка: не удается найти идентификатор для удаления.");
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления.");
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
                        MessageBox.Show("Запись успешно удалена.");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти запись для удаления.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении данных: " + ex.Message);
                }
            }
        }
    }
}