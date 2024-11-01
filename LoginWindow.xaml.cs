using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.OleDb;

namespace CompuSale
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private string _password = "";
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=../../DataBase/information_system.accdb;";
        
        private bool AuthenticateUser(string username, string password)
        {
            bool isAuthenticated = false;
            
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Запрос на проверку логина и пароля в таблице Сотрудник
                    string query = "SELECT COUNT(*) FROM Сотрудник WHERE Логин = @username AND Пароль = @password";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        // Параметры для защиты от SQL-инъекций
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        int result = (int)command.ExecuteScalar();

                        // Если результат больше 0, пользователь найден
                        isAuthenticated = result > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }

            return isAuthenticated;
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // позиция курсора до изменения
            int caretPosition = passwordTextBox.CaretIndex;

            // новый ввод пользователя
            string newText = passwordTextBox.Text;
            
            if (newText.Length > _password.Length)
            {
                string addedText = newText.Substring(_password.Length);
                _password += addedText;
            }
            else if (newText.Length < _password.Length)
            {
                _password = _password.Substring(0, newText.Length);
            }
            
            passwordTextBox.Text = new string('*', _password.Length);
            
            passwordTextBox.CaretIndex = caretPosition;
            
            
            if (sender == loginTextBox)
            {
                loginWatermark.Visibility = string.IsNullOrEmpty(loginTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            }
            else if (sender == passwordTextBox)
            {
                passwordWatermark.Visibility = string.IsNullOrEmpty(passwordTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = loginTextBox.Text;
            string password = _password;

            if (AuthenticateUser(username, password))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.");
            }
        }
    }
}