using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Models
{
    public class Match: Entity
    {
        public string Title { get; protected set; }
        public DateTime CreationDate { get; protected set; } = DateTime.Now;
        public string ProfileImage1Path { get; private set; }
        public string ProfileImage2Path { get; private set; }
        public ICollection<PlayerMatch> PlayerMatches { get; set; }

        public Match()
        {

        }

        public Match(string title, string profileImage1Path, string profileImage2Path)
        {
            Title = title;
            ProfileImage1Path = profileImage1Path;
            ProfileImage2Path = profileImage2Path;
        }
    }
}
