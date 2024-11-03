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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string UserFullName
        {
            get { return UserNameTextBlock.Text; }
            set { UserNameTextBlock.Text = value; }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=../../DataBase/information_system.accdb;";

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

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

        private void LoadProducts()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT \n\tТовар.Название,\n\tПроизводитель.Название AS Производитель_товара,\n\tКатегория.Название AS Категория_товара,\n\tТовар.Цена,\n\tТовар.Количество_на_складе,\n\tТовар.Описание\nFROM \n\tТовар\nINNER JOIN \n\tПроизводитель ON Товар.ID_производителя = Производитель.ID_производителя\nINNER JOIN \n\tКатегория ON Товар.ID_категории = Категория.ID_категории";
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

        private void LoadManufacturers()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Название FROM Производитель";
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
        private void LoadClients()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Клиент.Название, Клиент.Номер_телефона, Тип_клиента.Название AS Тип_клиента FROM Клиент INNER JOIN Тип_клиента ON Клиент.ID_типа_клиента = Тип_клиента.ID_типа_клиента";
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
        private void LoadCategories()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Название FROM Категория";
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
                    string query = "SELECT * FROM Продажа";
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
                    string query = "SELECT ФИО, Логин, Пароль, Роль FROM Сотрудник";
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
    }
}
