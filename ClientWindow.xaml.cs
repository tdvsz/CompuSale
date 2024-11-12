using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=../../DataBase/information_system.accdb;";
        private List<string> clientTypeSuggestions = new List<string>();
        private bool isEditMode = false;
        private int currentClientId = -1;
        
        public ClientWindow()
        {
            InitializeComponent();
            LoadClientTypeSuggestions();
        }
        
        private void LoadClientTypeSuggestions()
        {
            string query = "SELECT Название FROM Тип_клиента";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    clientTypeSuggestions.Add(reader["Название"].ToString());
                }
            }
        }
        
        private void ClientTypeSuggestionsListBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (clientTypeSuggestionsListBox.SelectedItem != null)
            {
                clientTypeTextBox.Text = clientTypeSuggestionsListBox.SelectedItem.ToString();
                clientTypeSuggestionsListBox.Visibility = Visibility.Collapsed;
                // suggestionsListBox.SelectedItem = null;
            }
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            nameWatermark.Visibility = string.IsNullOrEmpty(nameTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }
        
        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            phoneWatermark.Visibility = string.IsNullOrEmpty(phoneTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }
        
        private void ClientTypeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            clientTypeWatermark.Visibility = string.IsNullOrEmpty(clientTypeTextBox.Text) ? Visibility.Visible : Visibility.Hidden;

            string input = clientTypeTextBox.Text.ToLower();
            if (string.IsNullOrEmpty(input))
            {
                clientTypeSuggestionsListBox.Visibility = Visibility.Collapsed;
                return;
            }

            var filteredSuggestions = clientTypeSuggestions.FindAll(s => s.ToLower().Contains(input));
            clientTypeSuggestionsListBox.ItemsSource = filteredSuggestions;
            clientTypeSuggestionsListBox.Visibility = filteredSuggestions.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void phoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(phoneTextBox.Text, e.Text);
        }
        
        private bool IsTextAllowed(string currentText, string newText)
        {
            Regex regex = new Regex("^[0-9+]+$");

            if (!regex.IsMatch(newText))
                return false;

            // запрет на добавление второй запятой
            if (newText == "," && currentText.Contains(","))
                return false;

            // запрет на установку запятой в начале числа
            if (newText == "," && string.IsNullOrEmpty(currentText))
                return false;

            return true;
        }

        private void phoneTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
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
        
        private void clientTypeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                clientTypeSuggestionsListBox.Visibility = Visibility.Collapsed;

                clientTypeTextBox.Clear();
                clientTypeTextBox.Focus();
            }
        }
        
        private int GetClientTypeId(string clientTypeName)
        {
            string query = "SELECT ID_типа_клиента FROM Тип_клиента WHERE Название = @Название";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@Название", clientTypeName);

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
        
        private string GetClientTypeNameById(int clientTypeId)
        {
            string query = "SELECT Название FROM Тип_клиента WHERE ID_типа_клиента = @ID_типа_клиента";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_типа_клиента", clientTypeId);

                connection.Open();
                object result = command.ExecuteScalar();
                return result != null ? result.ToString() : string.Empty;
            }
        }
        
        public void LoadClientDataById(int clientId)
        {
            string query = "SELECT Название, Номер_телефона, ID_типа_клиента FROM Клиент WHERE ID_клиента = @ID_клиента";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_клиента", clientId);

                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    nameTextBox.Text = reader["Название"].ToString();
                    phoneTextBox.Text = reader["Номер_телефона"].ToString();
                    
                    clientTypeTextBox.Text = GetClientTypeNameById(Convert.ToInt32(reader["ID_типа_клиента"]));

                    // Установите флаг редактирования
                    isEditMode = true;
                    currentClientId = clientId;
                }
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text.Trim();
            string phone = phoneTextBox.Text.Trim();
            int clientTypeId = GetClientTypeId(clientTypeTextBox.Text.Trim());

            if (clientTypeId == -1)
            {
                MessageBox.Show("Тип производителя не найден");
                return;
            }

            string query;
            if (isEditMode)
            {
                query = "UPDATE Клиент SET ID_типа_клиента = @ID_типа_клиента, Название = @Название, Номер_телефона = @Номер_телефона WHERE ID_клиента = @ID_клиента";
            }
            else
            {
                query = "INSERT INTO Клиент (ID_типа_клиента, Название, Номер_телефона) " +
                        "VALUES (@ID_типа_клиента, @Название, @Номер_телефона)";
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);

                command.Parameters.Add("@ID_типа_клиента", OleDbType.Integer).Value = clientTypeId;
                command.Parameters.Add("@Название", OleDbType.VarChar).Value = name;
                command.Parameters.Add("@Номер_телефона", OleDbType.VarChar).Value = phone;

                if (isEditMode)
                {
                    command.Parameters.Add("@ID_клиента", OleDbType.Integer).Value = currentClientId;
                }

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show(isEditMode ? "Данные о клиенте обновлены." : "Данные о клиенте сохранены в базу данных.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
                }
            }
            this.Close();
        }
    }
}