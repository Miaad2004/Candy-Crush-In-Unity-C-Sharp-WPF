using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.UI.Controls
{
    public class MatchEventArgs: EventArgs
    {
        public Guid MatchId { get; }

        public MatchEventArgs(Guid matchId)
        {
            MatchId = matchId;
        }
    }
}
