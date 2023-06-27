using CandyCrush.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Services.Communication
{
    public interface IFriendService
    {
        Task AddFriendAsync(string friendUsername);
        Task RemoveFriendshipAsync(string friendUsername);
        Task<List<Player>> GetFriendsAsync(int skipCount, int takeCount, string searchQuery = "");
    }
}
