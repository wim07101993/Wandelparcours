using Microsoft.Extensions.DependencyInjection;
using VideoConverter;
using WebService.Services.Authorization;
using WebService.Services.Data;
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
                .AddSingleton<ITokensService, TokensService>()
                .AddSingleton<ILocationsService, LocationsService>()
                .AddSingleton<IDatabaseManager, DatabaseManager>()
                .AddSingleton<IVideoConverter, VideoConverter.VideoConverter>();
    }
}