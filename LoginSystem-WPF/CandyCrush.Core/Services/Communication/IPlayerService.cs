using CandyCrush.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Services.Communication
{
    public interface IPlayerService
    {
        Task<List<Player>> GetPlayers(int skipCount, int takeCount, string searchQuery="");
    }
}
