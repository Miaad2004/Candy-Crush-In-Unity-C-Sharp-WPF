using CandyCrush.Core.Services.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
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

namespace CandyCrush.UI.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Page
    {
        // Dependencies
        private readonly IAuthenticationService authenticationService;

        public LoginView()
        {
            InitializeComponent();
            SessionManager.AuthorizeMethodAccess(AccessLevels.Unauthorized);

            // Dependency Injection
            var serviceProvider = (Application.Current as App ?? throw new ArgumentNullException(nameof(Application))).ServiceProvider;
            authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
        }

        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(passwordBox.Password) && passwordBox.Password.Length > 0)
                textPassword.Visibility = Visibility.Collapsed;
            else
                textPassword.Visibility = Visibility.Visible;
        }

        private void TextPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            passwordBox.Focus();
        }

        private void UsernameBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(usernameBox.Text) && usernameBox.Text.Length > 0)
                textUsername.Visibility = Visibility.Collapsed;
            else
                textUsername.Visibility = Visibility.Visible;
        }

        private void TextUsername_MouseDown(object sender, MouseButtonEventArgs e)
        {
            usernameBox.Focus();
        }

        private async void Signin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await authenticationService.LoginAsync(usernameBox.Text, passwordBox.Password);
                ClearTextBoxes();
                MessageBox.Show("Logged in Successfully",
                                "Login Successful",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

               NavigationService.Navigate(new Uri("/Views/PanelView.xaml", UriKind.Relative));
            }

            catch (Exception ex)
            {
                if (ex is InvalidCredentialException ||
                    ex is InvalidOperationException ||
                    ex is ArgumentException)
                {
                    MessageBox.Show(ex.Message, "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                else throw;
            }
        }

        private async void Signup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await authenticationService.SignUpPlayerAsync(usernameBox.Text, passwordBox.Password, GetProfileImagePath());
                ClearTextBoxes();
                MessageBox.Show("Signed up successfully, please login.",
                                "Login Successful",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }

            catch (ArgumentException ex)
            {
                _ = MessageBox.Show(ex.Message, "Login error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearTextBoxes()
        {
            usernameBox.Text = string.Empty;
            passwordBox.Password = string.Empty;
        }

        private static string GetProfileImagePath()
        {
            MessageBox.Show("Please select your profile picture.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return OpenFilePicker();
        }

        private static string OpenFilePicker()
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Filter = "Image Files(*.JPG; *.JPEG; *.PNG; *.GIF)| *.JPG; *.JPEG; *.PNG; *.GIF",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };
            _ = openFileDialog.ShowDialog();

            return openFileDialog.FileName;
        }
    }
}
