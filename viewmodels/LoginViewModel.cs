using HCI_PROJEKA_1;
using System.Windows.Input;
using System.Windows;
using System;

public class LoginViewModel : BaseViewModel
{
    public static string CurrentLanguage { get; private set; }  
    public static string LoggedInUsername;
    public static bool Expert = false;

    private string _username;
    private string _password;
    private ICommand _loginCommand;
    private readonly UserModel _userModel;

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoginCommand
    {
        get
        {
            return _loginCommand ??= new RelayCommand(Login, CanLogin);
        }
    }

    public LoginViewModel()
    {
        _userModel = new UserModel();
    }

    private bool CanLogin(object parameter)
    {
        return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
    }

    private void Login(object parameter)
    {
        try
        {
            if (_userModel.CheckUserCredentials(Username, Password))
            {
                LoggedInUsername = Username;
          
                ApplyTheme(_userModel.GetTheme(LoggedInUsername));
                ApplyLanguage(_userModel.GetLanguage(LoggedInUsername));

                var mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow;
                if (_userModel.IsExpertUser(LoggedInUsername))
                { 
                    mainWindow.SettingsView_ExpertModeUnlocked();
                    Expert = true;
                    
                }
                else
                {
                    Expert = false;
                }

                LoadCurrentLanguage();
                mainWindow.Show();

                if (parameter is Window loginWindow)
                {
                    loginWindow.Close();
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred while logging in: {ex.Message}");
        }
        
    }

    private void LoadCurrentLanguage()
    {
        try
        {
            string language = _userModel.GetLanguage(LoggedInUsername);  
            if (language.Equals("serbian", StringComparison.OrdinalIgnoreCase) || language.Equals("srpski", StringComparison.OrdinalIgnoreCase))
            {
                CurrentLanguage = "Serbian";
            }
            else
            {
                CurrentLanguage = "English";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load language: {ex.Message}");
        }
    }

    public void ApplyTheme(string themeFile)
    {
        if (!themeFile.Contains("xaml"))
        {
            themeFile = themeFile + ".xaml";
        }

        var newDictionary = new ResourceDictionary { Source = new Uri($"Themes/{themeFile}", UriKind.Relative) };
        Application.Current.Resources.MergedDictionaries.Add(newDictionary);

        string theme = themeFile.Replace(".xaml", "");
        UpdateTheme(theme);
    }

    public void ApplyLanguage(string languageFile)
    {
        if (!languageFile.Contains("xaml"))
        {
            languageFile = languageFile + ".xaml";
        }

        var newDictionary = new ResourceDictionary { Source = new Uri($"Resources/{languageFile}", UriKind.Relative) };
        Application.Current.Resources.MergedDictionaries.Add(newDictionary);

        string language = languageFile.Replace(".xaml", "");
        UpdateLanguage(language);

        CurrentLanguage = language.Equals("serbian", StringComparison.OrdinalIgnoreCase) ? "Serbian" : "English";
    }

    public bool UpdateTheme(string theme)
    {
        try
        {
            return _userModel.UpdateTheme(LoggedInUsername, theme);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update theme: {ex.Message}");
            return false;
        }
    }

    public bool UpdateLanguage(string language)
    {
        try
        {
            return _userModel.UpdateLanguage(LoggedInUsername, language);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update language: {ex.Message}");
            return false;
        }
    }

    public bool UpdateExpertMode(bool isExpertUser)
    {
        Expert = true;
        try
        {
            return _userModel.UpdateExpertUser(LoggedInUsername, isExpertUser);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update expert mode: {ex.Message}");
            return false;
        }
    }
}
