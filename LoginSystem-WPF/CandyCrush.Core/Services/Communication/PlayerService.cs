using CandyCrush.Core.Data;
using CandyCrush.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Services.Communication
{
    public class PlayerService: IPlayerService
    {
        private readonly CandyDbContext dbContext;

        public PlayerService(CandyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Player>> GetPlayers(int skipCount, int takeCount, string searchQuery = "")
        {
            IQueryable<Player> query = dbContext.Players;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.Username.Contains(searchQuery));
            }

            List<Player> players = await query
                .Skip(skipCount)
                .Take(takeCount)
                .ToListAsync();

            return players;
        }

    }
}
