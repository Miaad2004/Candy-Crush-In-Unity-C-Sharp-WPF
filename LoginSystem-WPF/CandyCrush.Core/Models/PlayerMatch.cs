using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Models
{
    public class PlayerMatch: Entity
    {
        public Match Match { get; set; }
        public Guid MatchId { get; private set; }
        public Player Player { get; set; }
        public Guid PlayerId { get; private set; }
        public bool HasFinished => Score != null && IsWinner !=null;
        public float? Score { get; set; } = null;
        public bool? IsWinner { get; set; } = null;

        public PlayerMatch()
        {

        }

        public PlayerMatch(Match match, Player player)
        {
            Match = match;
            MatchId = match.Id;
            Player = player;
            PlayerId = player.Id;
        }
    }
}
