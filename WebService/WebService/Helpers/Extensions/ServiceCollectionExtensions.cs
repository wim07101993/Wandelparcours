using Microsoft.Extensions.DependencyInjection;
using WebService.Services.Authorization;
using WebService.Services.Data;
using WebService.Services.Data.Mock;
using WebService.Services.Data.Mongo;
using WebService.Services.Logging;
using WebService.Services.Randomizer;

namespace WebService.Helpers.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseServices(this IServiceCollection This)
            => This
                .AddSingleton(typeof(ILogger), new LoggerCollection {new ConsoleLogger(), new FileLogger()})
                .AddSingleton<IRandomizer, Randomizer>()
                .AddSingleton<IMediaService, MediaService>()
                .AddSingleton<IResidentsService, ResidentsService>()
                .AddSingleton<IReceiverModulesService, ReceiverModulesService>()
                .AddSingleton<IUsersService, UsersService>()
                .AddSingleton<ITokenService, TokenService>();

        public static IServiceCollection UseMockServices(this IServiceCollection This)
            => This
                .AddSingleton(typeof(ILogger), new LoggerCollection {new ConsoleLogger(), new FileLogger()})
                .AddSingleton<IRandomizer, Randomizer>()
                .AddSingleton<IMediaService, MockMediaService>()
                .AddSingleton<IResidentsService, MockResidentsService>()
                .AddSingleton<IReceiverModulesService, MockReceiverModulesesService>()
                .AddSingleton<IUsersService, MockUsersService>()
                .AddSingleton<ITokenService, TokenService>();
    }
}