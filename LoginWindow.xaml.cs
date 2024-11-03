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

        private string AuthenticateUser(string username, string password)
        {
            string fullName = null;

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT ФИО FROM Сотрудник WHERE Логин = @username AND Пароль = @password";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            fullName = result.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }

            return fullName;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int caretPosition = passwordTextBox.CaretIndex;
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

            string fullName = AuthenticateUser(username, password);

            if (fullName != null)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.UserFullName = fullName;
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
