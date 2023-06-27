using CandyCrush.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Services.Authentication
{
    public class SessionManager
    {
        public static Session? CurrentSession { get; set; } = null;
        public static bool IsCurrentSessionActive()
        {
            return CurrentSession != null &&
                   !CurrentSession.HasExpired;
        }

        public static void AuthorizeMethodAccess(AccessLevels requiredAccessLevel)
        {
            if (requiredAccessLevel == AccessLevels.Unauthorized) return;
            if (!IsCurrentSessionActive() || CurrentSession?.Player.AccessLevel < requiredAccessLevel)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this functionality.");
            }
        }
    }
}
