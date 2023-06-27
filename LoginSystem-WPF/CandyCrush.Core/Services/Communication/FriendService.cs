using CandyCrush.Core.Data;
using CandyCrush.Core.Models;
using CandyCrush.Core.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CandyCrush.Core.Services.Communication
{
    public class FriendService : IFriendService
    {
        private readonly CandyDbContext dbContext;
        private static Player CurrentPlayer => SessionManager.CurrentSession?.Player ?? throw new ArgumentNullException();

        public FriendService(CandyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddFriendAsync(string friendUsername)
        {
            var friend = await dbContext.Players.SingleAsync(player => player.Username == friendUsername);
            if (friendUsername == CurrentPlayer.Username)
            {
                throw new ArgumentException("You can't add yourself as a friend!");
            }

            var friendshipExists = await dbContext.Friendships
                .AnyAsync(friendship => friendship.Players.Contains(CurrentPlayer) && friendship.Players.Contains(friend));

            if (friendshipExists)
            {
                throw new ArgumentException("Friendship already exists.");
            }

            var friendship = new Friendship(new List<Player> { CurrentPlayer, friend });
            await dbContext.Friendships.AddAsync(friendship);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveFriendshipAsync(string friendUsername)
        {
            var friend = await dbContext.Players.SingleAsync(player => player.Username == friendUsername);

            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(friendship => friendship.Players.Contains(CurrentPlayer) &&
                                                   friendship.Players.Contains(friend))
                ?? throw new ArgumentException("Friendship doesnt exist.");

            dbContext.Friendships.Remove(friendship);
            await dbContext.SaveChangesAsync();
            
        }

        public async Task<List<Player>> GetFriendsAsync(int skipCount, int takeCount, string searchQuery = "")
        {
            IQueryable<Friendship> query = dbContext.Friendships.Where(friendship => friendship.Players.Contains(CurrentPlayer));

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(friendship => friendship.Players.Any(player => player.Username.Contains(searchQuery)));
            }

            List<Player> friends = await query.Skip(skipCount)
                                              .Take(takeCount)
                                              .SelectMany(friendship => friendship.Players)
                                              .Where(player => player.Id != CurrentPlayer.Id)
                                              .ToListAsync();

            return friends;
        }
    }
}
