using CandyCrush.Core.Data;
using CandyCrush.Core.Models;
using CandyCrush.Core.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CandyCrush.Core.Services.Game
{
    public class GameService : IGameService
    {
        private readonly IConfiguration configuration;
        private readonly CandyDbContext dbContext;
        private static Player CurrentPlayer => SessionManager.CurrentSession?.Player ?? throw new ArgumentNullException();

        public GameService(CandyDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
        }

        public async Task AddMatchAsync(string friendUsername)
        {
            var friend = await dbContext.Players.SingleAsync(player => player.Username == friendUsername);
            if (friendUsername == CurrentPlayer.Username)
            {
                throw new ArgumentException("You can't add yourself to a contest!");
            }

            var matchTitle = $"Multi-{CurrentPlayer.Username} & {friendUsername}";
            var match = new Match(matchTitle,
                                  profileImage1Path: CurrentPlayer.ProfileImagePath,
                                  profileImage2Path: friend.ProfileImagePath);
            match.PlayerMatches = new List<PlayerMatch>
            {
                new PlayerMatch(match, CurrentPlayer),
                new PlayerMatch(match, friend)
            };

            await dbContext.Matches.AddAsync(match);
            await dbContext.SaveChangesAsync();
        }

        public async Task<string> GetMatchResult(Guid matchId)
        {
            var match = await dbContext.Matches
                .Include(m => m.PlayerMatches)
                .ThenInclude(pm => pm.Player)
                .SingleOrDefaultAsync(m => m.Id == matchId)
                ?? throw new ArgumentException("Invalid match ID.");

            if (match.PlayerMatches.Any(pm => pm.Score == null))
            {
                var playersNotPlayed = match.PlayerMatches.Select(pm => pm.Player.Username);
                return $"The following players have not played yet: {string.Join(", ", playersNotPlayed)}";
            }

            var scores = match.PlayerMatches.Select(pm => $"{pm.Player.Username}: {pm.Score}");
            var highestScore = match.PlayerMatches.Max(pm => pm.Score);
            var winners = match.PlayerMatches.Where(pm => pm.Score == highestScore);

            if (winners.Count() > 1)
            {
                var winnerUsernames = winners.Select(w => w.Player.Username);
                return $"Scores: {string.Join(", ", scores)} | Winners: {string.Join(", ", winnerUsernames)} (Draw)";
            }
            else
            {
                var winner = winners.First();
                return $"Scores: {string.Join(", ", scores)} | Winner: {winner.Player.Username}";
            }
        }


        public async Task<List<Match>> GetCurrentUserMatchesAsync(int skipCount, int takeCount, string searchQuery = "")
        {
            IQueryable<Match> query = dbContext.Matches
                                               .Include(m => m.PlayerMatches)
                                               .Where(m => m.PlayerMatches.Any(pm => pm.PlayerId == CurrentPlayer.Id));

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(match => match.PlayerMatches.Any(playerMatch => playerMatch.Player.Username.ToLower().Contains(searchQuery.ToLower())));
            }

            List<Match> matches = await query.Skip(skipCount)
                                             .Take(takeCount)
                                             .ToListAsync();

            return matches;
        }

        public async Task<float> ReceiveScoreFromUnity(string matchResultOutputPath)
        {
            string matchResultJson = await File.ReadAllTextAsync(matchResultOutputPath);
            var matchResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MatchResult>(matchResultJson);
            return matchResult?.Score ?? throw new Exception("Unity Match Result not found.");
        }

        public async Task<UnityExitInfo> LaunchUnityAsync(Guid? matchId=null)
        {
            if (matchId == null)
                matchId = Guid.NewGuid();

            string unityGamePath = configuration.GetSection("Unity:UnityPath")?.Value
                                   ?? throw new ConfigurationErrorsException("Unity:UnityPath not found in config.json");

            if (!File.Exists(unityGamePath))
            {
                throw new ArgumentException("Unity game path does not exist.");
            }

            string matchResultOutputPath = configuration.GetSection("Unity:MatchResultOutputPath")?.Value
                                           ?? throw new ConfigurationErrorsException("Unity:MatchResultOutputPath not found in config.json");
            matchResultOutputPath = Path.Combine(matchResultOutputPath, $"{matchId}.json");

            float gameDurationInSeconds = float.Parse(configuration.GetSection("Unity:GameDurationInSeconds")?.Value
                                                      ?? throw new ConfigurationErrorsException("Unity:GameDuration not found in config.json"));


            string[] args = new string[]
            {
                "-gameDuration",
                gameDurationInSeconds.ToString(),
                "-matchResultOutputPath",
                matchResultOutputPath
            };

            ProcessStartInfo startInfo = new(unityGamePath)
            {
                Arguments = string.Join(" ", args)
            };

            Process unityGameProcess = new()
            {
                StartInfo = startInfo
            };

            unityGameProcess.Start();

            await unityGameProcess.WaitForExitAsync();
            float finalScore = await ReceiveScoreFromUnity(matchResultOutputPath);

            return new UnityExitInfo((UnityExitStatus)unityGameProcess.ExitCode, finalScore);
        }


        public async Task<UnityExitInfo> StartMutilplayerSession(Guid matchId)
        {
            var match = await dbContext.Matches
                .Include(m => m.PlayerMatches)
                .ThenInclude(pm => pm.Player)
                .SingleOrDefaultAsync(m => m.Id == matchId)
                ?? throw new ArgumentException("Invalid match ID.");

            var hasPlayedBefore = match.PlayerMatches.Any(pm => pm.PlayerId == CurrentPlayer.Id && pm.HasFinished);
            if (hasPlayedBefore)
            {
                throw new ArgumentException("You have played before.");
            }

            var unityExitInfo = await LaunchUnityAsync(matchId);
            if (unityExitInfo.ExitStatus == UnityExitStatus.Success)
            {
                var player = match.PlayerMatches.Where(pm => pm.PlayerId == CurrentPlayer.Id).Single();
                player.Score = unityExitInfo.FinalUserScore;
            }

            await dbContext.SaveChangesAsync();

            return unityExitInfo;
        }

    }
}
