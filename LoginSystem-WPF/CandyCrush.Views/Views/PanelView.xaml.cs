using CandyCrush.Core.Models;
using CandyCrush.Core.Services.Authentication;
using CandyCrush.Core.Services.Game;
using CandyCrush.UI.Popups;
using Microsoft.Extensions.DependencyInjection;
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

namespace CandyCrush.UI.Views
{
    /// <summary>
    /// Interaction logic for Panel.xaml
    /// </summary>
    public partial class PanelView : Page
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IGameService gameService;
        private readonly Player currentPlayer = SessionManager.CurrentSession?.Player ?? throw new ArgumentNullException();

        public PanelView()
        {
            InitializeComponent();
            
            // Dependency Injection
            var serviceProvider = (Application.Current as App 
                                  ?? throw new ArgumentNullException(nameof(Application))).ServiceProvider;
            authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
            gameService = serviceProvider.GetRequiredService<IGameService>();

            // Set Properties
            brushProfileImage.ImageSource = new BitmapImage(new Uri(currentPlayer.ProfileImagePath));
            txtUsername.Text = currentPlayer.Username;

            // Switch to friends tab
            FriendsButton.IsChecked = true;
        }

        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void FriendsButton_Checked(object sender, RoutedEventArgs e)
        {
            SubFrame.NavigationService.Navigate(new Uri("Views/Panel-FriendsSubview.xaml", UriKind.Relative));
        }

        private void MatchesButton_Checked(object sender, RoutedEventArgs e)
        {
            SubFrame.NavigationService.Navigate(new Uri("Views/Panel-MatchesSubview.xaml", UriKind.Relative));
        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            // Deactivate this window
            IsEnabled = false;

            AddFriendsPopup popup = new();
            _ = popup.ShowDialog();

            // Refresh the friends list
            (SubFrame.Content as Panel_FriendsSubview)?.RefreshFriendsList();

            // Reactivate this window
            IsEnabled = true;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                authenticationService.Logout();
                NavigationService.Navigate(new Uri("Views/LoginView.xaml", UriKind.Relative));
            }
        }

        private async void SinglePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            UnityExitInfo unityExitInfo = await gameService.LaunchUnityAsync();
            UnityExitStatus unityExitStatus = unityExitInfo.ExitStatus;

            switch(unityExitStatus)
            {
                case UnityExitStatus.Success:
                    float finalScore = unityExitInfo.FinalUserScore;
                    MessageBox.Show($"Game session ended with status {unityExitStatus}. Final Score: {finalScore}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;

                case UnityExitStatus.MidGameExit:
                    MessageBox.Show($"Game session ended with status {unityExitStatus}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;

                default:
                    MessageBox.Show($"Game session ended with status {unityExitStatus}", "Unity Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
            
            this.IsEnabled = true;
        }
    }
}
