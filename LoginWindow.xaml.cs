using System.Windows;
using System.Windows.Controls;

namespace CompuSale
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private string _password = "";

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
    }
}