using CandyCrush.Core.Models;
using CandyCrush.Core.Services.Communication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace CandyCrush.UI.Popups
{
    /// <summary>
    /// Interaction logic for AddFriendsPopup.xaml
    /// </summary>
    public partial class AddFriendsPopup : Window, IPopup, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private const int BatchSize = 20;
        private readonly IPlayerService playerService;
        private readonly IFriendService friendService;
        private bool isLoadingData;
        private int skipCount;

        private ObservableCollection<Player> players = new();

        public ObservableCollection<Player> Players
        {
            get { return players; }
            set
            {
                players = value;
                OnPropertyChanged(nameof(Players));
            }
        }

        private string searchQuery = string.Empty;

        public string SearchQuery
        {
            get { return searchQuery; }
            set
            {
                searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
                PerformSearchAsync();
            }
        }

        public AddFriendsPopup()
        {
            InitializeComponent();

            // Dependency Injection
            var serviceProvider = (Application.Current as App ?? throw new ArgumentNullException(nameof(Application))).ServiceProvider;
            playerService = serviceProvider.GetRequiredService<IPlayerService>();
            friendService = serviceProvider.GetRequiredService<IFriendService>();

            DataContext = this;
            isLoadingData = false;
            skipCount = 0;
            LoadInitialPlayers();
        }

        public async void PerformSearchAsync() // Add Task return type
        {
            skipCount = 0;
            Players.Clear();
            UsersScrollViewer.ScrollToVerticalOffset(0);
            await LoadInitialPlayers(); 
        }

        private async Task LoadInitialPlayers() 
        {
            var initialPlayers = await playerService.GetPlayers(skipCount, BatchSize, SearchQuery);
            foreach (var player in initialPlayers)
            {
                Players.Add(player);
            }
            skipCount += initialPlayers.Count;
        }

        private async Task LoadMorePlayers()
        {
            isLoadingData = true;
            var additionalPlayers = await playerService.GetPlayers(skipCount, BatchSize, SearchQuery);
            foreach (var player in additionalPlayers)
            {
                Players.Add(player);
            }
            skipCount += additionalPlayers.Count;
            isLoadingData = false;
        }

        private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset == e.ExtentHeight - e.ViewportHeight && !isLoadingData)
            {
                await LoadMorePlayers();
            }
        }
        public void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void UserCard_AddFriendButtonClicked(object sender, Controls.UserEventArgs e)
        {
            try
            {
                await friendService.AddFriendAsync(e.Username);
                MessageBox.Show($"{e.Username} added to your friends.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            catch(Exception ex)
            {
                if (ex is ArgumentException)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                else
                    throw;
            }
        }
    }
}
