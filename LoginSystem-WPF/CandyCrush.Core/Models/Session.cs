using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Models
{
    public sealed class Session
    {
        private static readonly TimeSpan EXPIRE_TIME_SPAN = TimeSpan.FromMinutes(120);
        private readonly DateTime _creationDate;

        [Key]
        public readonly int Id;
        public Player Player { get; private set; }
        public DateTime ExpireDate => _creationDate + EXPIRE_TIME_SPAN;

        private bool _hasBeenRevoked = false;
        public bool HasExpired => DateTime.UtcNow >= ExpireDate || _hasBeenRevoked;

        public Session(Player player)
            : base()
        {
            Player = player;
            _creationDate = DateTime.UtcNow;
            Id = GetHashCode();
            HeartBeat();
        }

        public void Revoke()
        {
            _hasBeenRevoked = true;
            HeartBeat();
        }

        public void HeartBeat()
        {
            this.Player.LastSeen = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Session - Username: {Player.Username} - Access Level: {Player.AccessLevel} - Session Expire date: {ExpireDate}";
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(_creationDate, Player, ExpireDate, _hasBeenRevoked, HasExpired);
        }
    }
}
