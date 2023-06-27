using CandyCrush.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Services.Game
{
    public interface IGameService
    {
        Task AddMatchAsync(string friendUsername);
        Task<string> GetMatchResult(Guid matchId);
        Task<List<Match>> GetCurrentUserMatchesAsync(int skipCount, int takeCount, string searchQuery = "");
        Task<UnityExitInfo> LaunchUnityAsync(Guid? matchId = null);
        Task<UnityExitInfo> StartMutilplayerSession(Guid matchId);
    }
}
