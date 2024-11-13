using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CompuSale
{
    public partial class ProductWindow : Window
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=../../DataBase/information_system.accdb;";
        private List<string> manufacturerSuggestions = new List<string>();
        private List<string> categorySuggestions = new List<string>();
        private bool isEditMode = false;
        private int currentProductId = -1;

        public ProductWindow()
        {
            InitializeComponent();
            LoadManufacturerSuggestions();
            LoadCategorySuggestions();
            currencyWatermark.Visibility = Visibility.Hidden;
        }

        private void LoadManufacturerSuggestions()
        {
            string query = "SELECT Название FROM Производитель";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    manufacturerSuggestions.Add(reader["Название"].ToString());
                }
            }
        }

        private void LoadCategorySuggestions()
        {
            string query = "SELECT Название FROM Категория";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    categorySuggestions.Add(reader["Название"].ToString());
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Watermark.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Hidden;

            string input = textBox.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                addManufacturerBtn.Visibility = Visibility.Collapsed;
                return;
            }

            bool manufacturerExists = manufacturerSuggestions.Contains(input, StringComparer.OrdinalIgnoreCase);

            addManufacturerBtn.Visibility = manufacturerExists ? Visibility.Collapsed : Visibility.Visible;

            input = textBox.Text.ToLower();
            if (string.IsNullOrEmpty(input))
            {
                suggestionsListBox.Visibility = Visibility.Collapsed;
                return;
            }

            var filteredSuggestions = manufacturerSuggestions.FindAll(s => s.ToLower().Contains(input));
            suggestionsListBox.ItemsSource = filteredSuggestions;
            suggestionsListBox.Visibility = filteredSuggestions.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void addManufacturerBtn_Click(object sender, RoutedEventArgs e)
        {
            string newManufacturer = textBox.Text.Trim();

            if (!string.IsNullOrEmpty(newManufacturer))
            {
                string insertQuery = "INSERT INTO Производитель (Название) VALUES (@Название)";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(insertQuery, connection);
                    command.Parameters.AddWithValue("@Название", newManufacturer);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                LoadManufacturerSuggestions();
                addManufacturerBtn.Visibility = Visibility.Collapsed;

                MessageBox.Show("Производитель добавлен в базу данных.");
            }
            else
            {
                MessageBox.Show("Введите название производителя.");
            }
        }

        private void CategoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            categoryWatermark.Visibility = string.IsNullOrEmpty(categoryTextBox.Text) ? Visibility.Visible : Visibility.Hidden;

            string input = categoryTextBox.Text.ToLower();
            if (string.IsNullOrEmpty(input))
            {
                categorySuggestionsListBox.Visibility = Visibility.Collapsed;
                return;
            }

            var filteredSuggestions = categorySuggestions.FindAll(s => s.ToLower().Contains(input));
            categorySuggestionsListBox.ItemsSource = filteredSuggestions;
            categorySuggestionsListBox.Visibility = filteredSuggestions.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            nameWatermark.Visibility = string.IsNullOrEmpty(nameTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void PriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            priceWatermark.Visibility = string.IsNullOrEmpty(priceTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            currencyWatermark.Visibility = string.IsNullOrEmpty(priceTextBox.Text) ? Visibility.Hidden : Visibility.Visible;
        }

        private void priceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(priceTextBox.Text, e.Text);
        }

        private bool IsTextAllowed(string currentText, string newText)
        {
            Regex regex = new Regex("^[0-9]+$");

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

        private void PriceTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
            }
        }

        private void CountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            countWatermark.Visibility = string.IsNullOrEmpty(countTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            descriptionWatermark.Visibility = string.IsNullOrEmpty(descriptionTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void SuggestionsListBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (suggestionsListBox.SelectedItem != null)
            {
                textBox.Text = suggestionsListBox.SelectedItem.ToString();
                suggestionsListBox.Visibility = Visibility.Collapsed;
                // suggestionsListBox.SelectedItem = null;
            }
        }

        private void CategorySuggestionsListBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (categorySuggestionsListBox.SelectedItem != null)
            {
                categoryTextBox.Text = categorySuggestionsListBox.SelectedItem.ToString();
                categorySuggestionsListBox.Visibility = Visibility.Collapsed;
                // categorySuggestionsListBox.SelectedItem = null;
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

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                suggestionsListBox.Visibility = Visibility.Collapsed;

                textBox.Clear();
                textBox.Focus();
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text.Trim();
            string description = descriptionTextBox.Text.Trim();
            int count = 0;
            decimal price = 0;

            if (!int.TryParse(countTextBox.Text.Trim(), out count))
            {
                MessageBox.Show("Введите корректное количество.");
                return;
            }

            try
            {
                price = decimal.Parse(priceTextBox.Text.Trim().Replace(",", "."),
                                      System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                MessageBox.Show("Введите корректную цену.");
                return;
            }

            int categoryId = GetCategoryId(categoryTextBox.Text.Trim());
            int manufacturerId = GetManufacturerId(textBox.Text.Trim());

            if (categoryId == -1 || manufacturerId == -1)
            {
                MessageBox.Show("Категория или производитель не найдены.");
                return;
            }

            string query;
            if (isEditMode)
            {
                query = "UPDATE Товар SET Название = @Название, Описание = @Описание, Цена = @Цена, " +
                        "Количество_на_складе = @Количество_на_складе, ID_категории = @ID_категории, " +
                        "ID_производителя = @ID_производителя WHERE ID_товара = @ID_товара";
            }
            else
            {
                query = "INSERT INTO Товар (Название, Описание, Цена, Количество_на_складе, ID_категории, ID_производителя) " +
                        "VALUES (@Название, @Описание, @Цена, @Количество_на_складе, @ID_категории, @ID_производителя)";
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);

                command.Parameters.Add("@Название", OleDbType.VarChar).Value = name;
                command.Parameters.Add("@Описание", OleDbType.VarChar).Value = description;
                command.Parameters.Add("@Цена", OleDbType.Currency).Value = price;
                command.Parameters.Add("@Количество_на_складе", OleDbType.Integer).Value = count;
                command.Parameters.Add("@ID_категории", OleDbType.Integer).Value = categoryId;
                command.Parameters.Add("@ID_производителя", OleDbType.Integer).Value = manufacturerId;

                if (isEditMode)
                {
                    command.Parameters.Add("@ID_товара", OleDbType.Integer).Value = currentProductId;
                }

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show(isEditMode ? "Данные о товаре обновлены." : "Данные о товаре сохранены в базу данных.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
                }
            }
            this.Close();
        }

        private int GetCategoryId(string categoryName)
        {
            string query = "SELECT ID_категории FROM Категория WHERE Название = @Название";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@Название", categoryName);

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

        private int GetManufacturerId(string manufacturerName)
        {
            string query = "SELECT ID_производителя FROM Производитель WHERE Название = @Название";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@Название", manufacturerName);

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
        
        public void LoadProductDataById(int productId)
        {
            string query = "SELECT Название, Описание, Цена, Количество_на_складе, ID_категории, ID_производителя FROM Товар WHERE ID_товара = @ID_товара";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_товара", productId);

                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    nameTextBox.Text = reader["Название"].ToString();
                    descriptionTextBox.Text = reader["Описание"].ToString();
                    priceTextBox.Text = reader["Цена"].ToString();
                    countTextBox.Text = reader["Количество_на_складе"].ToString();

                    // Используйте методы для получения названий производителя и категории по ID
                    textBox.Text = GetManufacturerNameById(Convert.ToInt32(reader["ID_производителя"]));
                    categoryTextBox.Text = GetCategoryNameById(Convert.ToInt32(reader["ID_категории"]));

                    // Установите флаг редактирования
                    isEditMode = true;
                    currentProductId = productId;
                }
            }
        }

        private string GetManufacturerNameById(int manufacturerId)
        {
            string query = "SELECT Название FROM Производитель WHERE ID_производителя = @ID_производителя";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_производителя", manufacturerId);

                connection.Open();
                object result = command.ExecuteScalar();
                return result != null ? result.ToString() : string.Empty;
            }
        }

        private string GetCategoryNameById(int categoryId)
        {
            string query = "SELECT Название FROM Категория WHERE ID_категории = @ID_категории";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@ID_категории", categoryId);

                connection.Open();
                object result = command.ExecuteScalar();
                return result != null ? result.ToString() : string.Empty;
            }
        }
    }
}