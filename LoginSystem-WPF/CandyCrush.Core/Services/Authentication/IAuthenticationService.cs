using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task SignUpPlayerAsync(string username, string password, string profileImagePath);
        Task LoginAsync(string username, string password);
        void Logout();
    }
}
