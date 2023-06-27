using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Services.Game
{
    public class UnityExitInfo
    {
        public UnityExitStatus ExitStatus { get; private set; }
        public float FinalUserScore { get; private set; }

        public UnityExitInfo(UnityExitStatus exitStatus, float finalUserScore)
        {
            ExitStatus = exitStatus;
            FinalUserScore = finalUserScore;
        }
    }
}
