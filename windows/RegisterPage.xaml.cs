using System;
using System.Collections.Generic;
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

using System.Windows;

namespace HCI_PROJEKA_1
{
    public partial class RegisterPage : Window
    {
        public RegisterPage()
        {
            InitializeComponent();
            this.DataContext = new RegisterViewModel();
        }

        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UsernamePlaceholder.Visibility = Visibility.Collapsed;
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                UsernamePlaceholder.Visibility = Visibility.Visible;
            }
        }


        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

 
        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

    
        private void ConfirmPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

       
        private void ConfirmPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password))
            {
                ConfirmPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                (this.DataContext as RegisterViewModel).Password = passwordBox.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                (this.DataContext as RegisterViewModel).ConfirmPassword = passwordBox.Password;
            }
        }

        private void LoginTextBlock_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            var loginPage = new LoginPage();
            loginPage.Show();
            this.Close();
        }
    }
}
