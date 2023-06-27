using CandyCrush.Core.Models;
using CandyCrush.Core.Services.Authentication;
using CandyCrush.Core.Services.Communication;
using CandyCrush.Core.Services.Game;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CandyCrush.UI.Views
{
    /// <summary>
    /// Interaction logic for Panel_MatchesSubview.xaml
    /// </summary>
    public partial class Panel_MatchesSubview : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private const int BatchSize = 20;
        private readonly IGameService gameService;
        private bool isLoadingData;
        private int skipCount;

        private ObservableCollection<Match> matches = new();

        public ObservableCollection<Match> Matches
        {
            get { return matches; }
            set
            {
                matches = value;
                OnPropertyChanged(nameof(Matches));
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

        public Panel_MatchesSubview()
        {
            InitializeComponent();

            // Dependency Injection
            var serviceProvider = (Application.Current as App ?? throw new ArgumentNullException(nameof(Application))).ServiceProvider;
            gameService = serviceProvider.GetRequiredService<IGameService>();

            DataContext = this;
            isLoadingData = false;
            skipCount = 0;
            LoadInitialPlayers();
        }

        private void PerformSearch()
        {
            skipCount = 0;
            Matches.Clear();
            LoadInitialPlayers();
        }

        private async void LoadInitialPlayers()
        {
            var initialMatches = await gameService.GetCurrentUserMatchesAsync(skipCount, BatchSize, SearchQuery);
            foreach (var match in initialMatches)
            {
                Matches.Add(match);
            }
            skipCount += initialMatches.Count;
        }

        private async Task LoadMorePlayers()
        {
            isLoadingData = true;
            var additionalPlayers = await gameService.GetCurrentUserMatchesAsync(skipCount, BatchSize, SearchQuery);
            foreach (var player in additionalPlayers)
            {
                Matches.Add(player);
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

        public void RefreshMatchList()
        {
            skipCount = 0;
            Matches.Clear();
            LoadInitialPlayers();
        }

        private async void MatchCard_ViewMatchResultButtonClicked(object sender, Controls.MatchEventArgs e)
        {
            string matchResult = await gameService.GetMatchResult(e.MatchId);
            MessageBox.Show(matchResult, "Match Result", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void MatchCard_PlayButtonClicked(object sender, Controls.MatchEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to start a multiplayer session?",
                                                      "Logout Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No) return;

            try
            {
                UnityExitInfo unityExitInfo =  await gameService.StartMutilplayerSession(e.MatchId);
                UnityExitStatus unityExitStatus = unityExitInfo.ExitStatus; 

                switch (unityExitStatus)
                {
                    case UnityExitStatus.Success:
                        var finalScore = unityExitInfo.FinalUserScore;
                        MessageBox.Show($"Game session ended with status {unityExitStatus}. Final Score: {finalScore}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;

                    case UnityExitStatus.MidGameExit:
                        MessageBox.Show($"Game session ended with status {unityExitStatus}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;

                    default:
                        MessageBox.Show($"Game session ended with status {unityExitStatus}", "Unity Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            catch(Exception ex)
            {
                if (ex is ArgumentException)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                else
                {
                    throw;
                }
            }

            RefreshMatchList();
        }
    }
}
