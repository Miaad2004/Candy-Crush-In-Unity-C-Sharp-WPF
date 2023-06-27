using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Models
{
    public sealed class Friendship: Entity
    {
        public ICollection<Player> Players { get; private set; }

        public DateTime FriendshipStartDate { get; private set; }

        public Friendship() { }

        public Friendship(List<Player> players)
        {
            Players = players;
            FriendshipStartDate = DateTime.Now;
        }

    }
}
