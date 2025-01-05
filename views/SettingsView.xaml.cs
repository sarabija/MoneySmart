using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HCI_PROJEKA_1
{
    public partial class SettingsView : UserControl
    {

        public delegate void ExpertModeUnlockedEventHandler(object sender, EventArgs e);
        public event ExpertModeUnlockedEventHandler ExpertModeUnlocked;

        public SettingsView()
        {
            InitializeComponent();
           if(LoginViewModel.Expert == true)
            {
                this.CodeTextBox.Visibility = Visibility.Collapsed;
                this.expert.Visibility = Visibility.Collapsed;
            }
        }

        private void ApplyLanguageButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedLanguage = LanguageComboBox.SelectedItem as ComboBoxItem;
            string language = selectedLanguage?.Content.ToString();

            if (language == "English" || language == "Engleski")
            {
                LoginViewModel loginViewModel = new LoginViewModel();
                loginViewModel.ApplyLanguage("english.xaml");
            }
            else if (language == "Serbian" || language == "Srpski")
            {
                LoginViewModel loginViewModel = new LoginViewModel();
                loginViewModel.ApplyLanguage("serbian.xaml");
            }
        }

        private void ApplyThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ThemeComboBox.SelectedItem as ComboBoxItem;
            string theme = selectedTheme?.Content.ToString();

            if (theme == "Light" || theme == "Svijetlo")
            {
                LoginViewModel loginViewModel = new LoginViewModel();
                loginViewModel.ApplyTheme("light.xaml");
            }
            else if (theme == "Dark" || theme == "Tamno")
            {
                LoginViewModel loginViewModel = new LoginViewModel();
                loginViewModel.ApplyTheme("dark.xaml");
            }
            else if (theme == "Default" || theme == "Podrazumijevano")
            {
                LoginViewModel loginViewModel = new LoginViewModel();
                loginViewModel.ApplyTheme("custom.xaml");
            }
        }

        private void UnlockExpertButton_Click(object sender, RoutedEventArgs e)
        {
            if (CodeTextBox.Text.Trim() == "12")
            {
                ExpertModeUnlocked?.Invoke(this, EventArgs.Empty);
                this.CodeTextBox.Visibility = Visibility.Collapsed;
                this.expert.Visibility = Visibility.Collapsed;
                LoginViewModel loginViewModel = new LoginViewModel();
                loginViewModel.UpdateExpertMode(true);
            }
            else
            {
                MessageBox.Show("Invalid code. Please try again.");
            }
        }



        private void CodeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (CodeTextBox.Text == "Enter code")
            {
                CodeTextBox.Text = "";
                CodeTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void CodeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CodeTextBox.Text))
            {
                CodeTextBox.Text = "Enter code";
                CodeTextBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }




    }
}
