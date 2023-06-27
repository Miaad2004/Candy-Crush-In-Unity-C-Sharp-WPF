using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CandyCrush.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CandyCrush.Core.Data
{
    public class CandyDbContext: DbContext
    {

        public DbSet<Player> Players { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<PlayerMatch> PlayerMatches { get; set; }

        private readonly IConfiguration _configuration;

        public CandyDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new ConfigurationErrorsException("Invalid Configuration");
            return connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseSqlite(GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Player & PlayerMatch => 1-n
            modelBuilder.Entity<Player>()
                .HasMany(p => p.PlayerMatches)
                .WithOne(pm => pm.Player)
                .HasForeignKey(pm => pm.PlayerId);

            // Match & PlayerMatch => 1-n
            modelBuilder.Entity<Match>()
                .HasMany(m => m.PlayerMatches)
                .WithOne(pm => pm.Match)
                .HasForeignKey(pm => pm.MatchId);
        }
    }
}
