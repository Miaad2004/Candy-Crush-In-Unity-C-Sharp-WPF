using CandyCrush.Core.Services.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace CandyCrush.Core.Models
{
    public class Player: Entity
    {
        private string _username;
        public string Username
        {
            get => _username;
            protected set
            {
                if (string.IsNullOrEmpty(value) || value.Length < 5)
                {
                    throw new ArgumentException("Username must be at least 5 characters.(Case Sensitive)");
                }

                _username = value;
            }
        }
        public string PasswordHash { get; protected set; }
        public AccessLevels AccessLevel { get; protected set; }
        public ICollection<Friendship> Friendships { get; private set; }
        public ICollection<PlayerMatch> PlayerMatches { get; private set; }
        public string ProfileImagePath { get; private set; }
        public DateTime LastSeen { get; set; } = DateTime.Now;

        public Player()
        {

        }

        public Player(string username, string passwordHash, string profileImagePath)
        {
            Username = username;
            PasswordHash = passwordHash;

            SetProfileImagePath(profileImagePath);
        }

        private void SetProfileImagePath(string? profileImagePath)
        {
            if (profileImagePath is null || !File.Exists(profileImagePath))
            {
                throw new ArgumentException("Invalid profile image path.");
            }

            string subfolderName = "CandyCrush";
            string subfolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                                                subfolderName, "ProfileImages");

            if (!Directory.Exists(subfolderPath))
            {
                Directory.CreateDirectory(subfolderPath);
            }

            string fullImagePath = Path.Combine(subfolderPath, this.Id.ToString() + Path.GetExtension(profileImagePath));

            File.Copy(profileImagePath, fullImagePath, overwrite: true);
            File.SetAttributes(fullImagePath, FileAttributes.ReadOnly);
            this.ProfileImagePath = fullImagePath;
        }
    }
}
