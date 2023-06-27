using CandyCrush.Core.Data;
using CandyCrush.Core.Exceptions;
using CandyCrush.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CandyCrush.Core.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly CandyDbContext _dbContext;
        private readonly IPasswordService _passwordService;

        public AuthenticationService(CandyDbContext dbContext,
                                     IPasswordService passwordService)
        {
            _dbContext = dbContext;
            _passwordService = passwordService;
        }

        private async Task<bool> IsUsernameTakenAsync(string username)
        {
            return await _dbContext.Players.AnyAsync(u => u.Username == username);
        }

        public async Task SignUpPlayerAsync(string username, string password, string profileImagePath)
        {
            SessionManager.AuthorizeMethodAccess(AccessLevels.Unauthorized);

            if (await IsUsernameTakenAsync(username))
            {
                throw new UsernameAlreadyExistsException(username);
            }

            if (!_passwordService.IsStrong(password))
            {
                throw new ArgumentException("Password must be at least 8 characters and contain upper and lower letters.");
            }

            string passwordHash = _passwordService.GetHash(password);

            Player player = new(username, passwordHash, profileImagePath);

            await _dbContext.Players.AddAsync(player);
            await _dbContext.SaveChangesAsync();
        }

        public async Task LoginAsync(string username, string password)
        {
            SessionManager.AuthorizeMethodAccess(AccessLevels.Unauthorized);

            string passwordHash = _passwordService.GetHash(password);

            Session? session = SessionManager.CurrentSession;
            if (session != null && !session.HasExpired)
            {
                throw new InvalidOperationException("There is an active session. Please logout first.");
            }

            Player player = await _dbContext.Players.SingleOrDefaultAsync(u => u.Username == username && u.PasswordHash == passwordHash)
                            ?? throw new InvalidCredentialException("Wrong username or password!");

            SessionManager.CurrentSession = new Session(player);
        }

        public void Logout()
        {
            SessionManager.CurrentSession?.Revoke();
        }

    }
}
