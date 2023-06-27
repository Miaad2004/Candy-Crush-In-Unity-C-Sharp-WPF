using CandyCrush.Core.Models;
using CandyCrush.Core.Services.Authentication;
using CandyCrush.Core.Services.Communication;
using CandyCrush.Core.Services.Game;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    /// Interaction logic for Panel_FriendsSubview.xaml
    /// </summary>

    public partial class Panel_FriendsSubview : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private const int BatchSize = 20;
        private readonly IFriendService friendService;
        private readonly IGameService gameService;
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
                PerformSearch();
            }
        }

        public Panel_FriendsSubview()
        {
            InitializeComponent();

            // Dependency Injection
            var serviceProvider = (Application.Current as App ?? throw new ArgumentNullException(nameof(Application))).ServiceProvider;
            friendService = serviceProvider.GetRequiredService<IFriendService>();
            gameService = serviceProvider.GetRequiredService<IGameService>();

            DataContext = this;
            isLoadingData = false;
            skipCount = 0;
            LoadInitialPlayers();
        }

        private void PerformSearch()
        {
            skipCount = 0;
            Players.Clear();
            LoadInitialPlayers();
        }

        private async void LoadInitialPlayers()
        {
            var initialPlayers = await friendService.GetFriendsAsync(skipCount, BatchSize, SearchQuery);
            foreach (var player in initialPlayers)
            {
                Players.Add(player);
            }
            skipCount += initialPlayers.Count;
        }

        private async Task LoadMorePlayers()
        {
            isLoadingData = true;
            var additionalPlayers = await friendService.GetFriendsAsync(skipCount, BatchSize, SearchQuery);
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

        private async void FriendCard_UnfriendButtonClicked(object sender, Controls.UserEventArgs e)
        {
            try
            {
                await friendService.RemoveFriendshipAsync(e.Username);
                MessageBox.Show($"{e.Username} removed from your friends.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            catch (Exception ex)
            {
                if (ex is ArgumentException)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                else
                    throw;
            }

            RefreshFriendsList();
        }

        private async void FriendCard_CreateContestButtonClicked(object sender, Controls.UserEventArgs e)
        {
            try
            {
                await gameService.AddMatchAsync(e.Username);
                MessageBox.Show($"Contest added successfully. Check out the Matches tab.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            catch (Exception ex)
            {
                if (ex is ArgumentException)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                else
                    throw;
            }
        }

        public void RefreshFriendsList()
        {
            skipCount = 0;
            Players.Clear();
            LoadInitialPlayers();
        }
    }
}
