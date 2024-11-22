using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.OleDb;
using System.Data;

namespace CompuSale
{
    public partial class LoginWindow : Window
    {
        public static int CurrentEmployeeID { get; set; }  // ID авторизованного сотрудника
        public static string CurrentEmployeeName { get; set; }  // ФИО авторизованного сотрудника

        public LoginWindow()
        {
            InitializeComponent();
        }

        private string _password = "";
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=../../DataBase/information_system.accdb;";

        private string AuthenticateUser(string username, string password)
        {
            string role = null;

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT ID_сотрудника, ФИО, Роль FROM Сотрудник WHERE Логин = @username AND Пароль = @password";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CurrentEmployeeID = reader.GetInt32(0);  // Получаем ID сотрудника
                                CurrentEmployeeName = reader.GetString(1);  // Сохраняем ФИО
                                role = reader.GetString(2);  // Получаем роль сотрудника
                                Console.WriteLine(role);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                }
            }

            return role;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = loginTextBox.Text;
            string password = _password;

            string role = AuthenticateUser(username, password);

            if (role != null)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.EmployeeName = CurrentEmployeeName;  // Передаем ФИО на главную форму
                mainWindow.EmployeeID = CurrentEmployeeID;
                mainWindow.UserRole = role;  // Передаем роль пользователя
                Console.WriteLine(role);
                mainWindow.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.");
            }
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
    }
}