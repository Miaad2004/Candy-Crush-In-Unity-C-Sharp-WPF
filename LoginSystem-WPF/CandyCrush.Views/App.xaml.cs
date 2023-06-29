using CandyCrush.Core.Data;
using CandyCrush.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace CandyCrush.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool isFirstLoadFlag = true;
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Setup configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Setup dependency injection
            var services = new ServiceCollection();
            ServiceFactory.ConfigureServices(services, configuration);
            ServiceProvider = services.BuildServiceProvider();

            var dbContext = ServiceProvider.GetRequiredService<CandyDbContext>();
            dbContext.Database.EnsureCreated();

            base.OnStartup(e);
        }

        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            if (isFirstLoadFlag)
            {
                PlayBackgroundMusic();
                isFirstLoadFlag = false;
            }

            base.OnLoadCompleted(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        private void PlayBackgroundMusic()
        {
            bool isPlaying = true;

            string startupSoundFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StaticResources/LoginView/bg-music.mp3");
            MediaPlayer player = new();
            player.Open(new Uri(startupSoundFilePath));
            player.Volume = 0.5;

            Activated += (sender, args) =>
            {
                if (isPlaying) player.Play();
            };
            Deactivated += (sender, args) => player.Pause();

            // Loop
            player.MediaEnded += (sender, args) =>
            {
                player.Position = TimeSpan.Zero;
                player.Play();
            };

            Application.Current.MainWindow.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.M && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    if (isPlaying)
                    {
                        player.Stop();
                        isPlaying = false;
                    }
                    else
                    {
                        player.Play();
                        isPlaying = true;
                    }
                }
            };

        }

    }
}
