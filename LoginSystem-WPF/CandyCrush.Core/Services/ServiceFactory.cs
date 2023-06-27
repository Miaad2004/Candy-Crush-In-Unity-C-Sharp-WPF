using CandyCrush.Core.Data;
using CandyCrush.Core.Services.Authentication;
using CandyCrush.Core.Services.Communication;
using CandyCrush.Core.Services.Game;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.Core.Services
{
    public static class ServiceFactory
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<CandyDbContext>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IFriendService, FriendService>();
            services.AddScoped<IGameService, GameService>();
            services.AddSingleton(configuration);

        }
    }
}
