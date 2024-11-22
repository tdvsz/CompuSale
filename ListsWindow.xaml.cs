using System;
using System.Collections.Generic;
using System.Data.OleDb;
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
using System.Windows.Shapes;

namespace CompuSale
{
    /// <summary>
    /// Логика взаимодействия для ListsWindow.xaml
    /// </summary>
    public partial class ListsWindow : Window
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=../../DataBase/information_system.accdb;";
        private bool isEditMode = false;
        private int selectedManufacturerId = -1;
        private int selectedCategorytId = -1;
        
        public string SelectedTreeViewItem { get; set; }
        public ListsWindow()
        {
            InitializeComponent();
        }
        
        public void LoadManufacturerDataById(int ManufacturerId)
        {
            string query = "SELECT Название FROM Производитель WHERE ID_производителя = @ID_производителя";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_производителя", ManufacturerId);

                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    nameTextBox.Text = reader["Название"].ToString();
                    
                    isEditMode = true;
                    selectedManufacturerId = ManufacturerId;
                }
            }
        }
        
        public void LoadCategoryDataById(int CategoryId)
        {
            string query = "SELECT Название FROM Категория WHERE ID_категории = @ID_категории";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_категории", CategoryId);

                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    nameTextBox.Text = reader["Название"].ToString();

                    isEditMode = true; 
                    selectedCategorytId = CategoryId;
                }
            }
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            nameWatermark.Visibility = string.IsNullOrEmpty(nameTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text.Trim();
            string query;

            if (nameTextBox.Text != String.Empty) {
                if (SelectedTreeViewItem == "Производитель")
                {
                    if (isEditMode)
                    {
                        query = "UPDATE Производитель SET Название = @Название WHERE ID_производителя = @ID_производителя";
                    }
                    else
                    {
                        query = "INSERT INTO Производитель (Название) VALUES (@Название)";
                    }
                }
                else if (SelectedTreeViewItem == "Категория")
                {
                    if (isEditMode)
                    {
                        query = "UPDATE Категория SET Название = @Название WHERE ID_категории = @ID_категории";
                    }
                    else
                    {
                        query = "INSERT INTO Категория (Название) VALUES (@Название)";
                    }
                }
                else
                {
                    MessageBox.Show("Не выбран элемент для сохранения");
                    return;
                }

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);

                    command.Parameters.Add("@Название", OleDbType.VarChar).Value = name;

                    if (isEditMode)
                    {
                        // Добавляем параметр ID только если это режим редактирования
                        if (SelectedTreeViewItem == "Производитель")
                        {
                            command.Parameters.Add("@ID_производителя", OleDbType.Integer).Value = selectedManufacturerId;
                        }
                        else if (SelectedTreeViewItem == "Категория")
                        {
                            command.Parameters.Add("@ID_категории", OleDbType.Integer).Value = selectedCategorytId;
                        }
                    }

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show(isEditMode ? "Данные обновлены." : "Данные сохранены в базу данных");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
                    }
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Заполните поле");
                return;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}