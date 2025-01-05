using System;
using System.Windows;
using System.Windows.Input;

namespace HCI_PROJEKA_1
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _username;
        private string _password;
        private string _confirmPassword;
        private ICommand _registerCommand;

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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand
        {
            get
            {
                return _registerCommand ??= new RelayCommand(Register, CanRegister);
            }
        }

        private bool CanRegister(object parameter)
        {
            return !string.IsNullOrEmpty(Username) &&
                   !string.IsNullOrEmpty(Password) &&
                   Password == ConfirmPassword;
        }

        private void Register(object parameter)
        {
            var userModel = new UserModel();

            if (userModel.RegisterUser(Username, Password))
            {
                MessageBox.Show("Registration successful.");
            }
            else
            {
                MessageBox.Show("Username already exists. Please choose a different username.");
            }
        }

    }
}
