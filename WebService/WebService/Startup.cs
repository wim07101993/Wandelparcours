using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using WebService.Helpers.Extensions;
using WebService.Middleware;
using WebService.Services.Authorization;
using WebService.Services.Data;
using WebService.Services.Data.Mongo;
using WebService.Services.Logging;
using WebService.Services.Randomizer;

namespace WebService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services
                .AddSingleton(typeof(ILogger), new LoggerCollection {new ConsoleLogger(), new FileLogger()})
                .AddSingleton<IRandomizer, Randomizer>()
                //.AddSingleton<IMediaService, MockMediaService>()
                //.AddSingleton<IResidentsService, MockResidentsService>()
                //.AddSingleton<IReceiverModulesService, ReceiverModulesService>()
                //.AddSingleton<IUsersService, MockUsersService>()
                .AddSingleton<IMediaService, MediaService>()
                .AddSingleton<IResidentsService, ResidentsService>()
                .AddSingleton<IReceiverModulesService, ReceiverModulesService>()
                .AddSingleton<IUsersService, UsersService>()
                .AddSingleton<ITokenService, TokenService>();

            services
                .AddMvc(options =>
                {
                    options.Filters.Add<ExceptionPipeline>();
                    //options.Filters.Add<AuthPipelineFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod()
            );

            app.UseCors((option) => { option.AllowAnyOrigin().AllowAnyMethod(); });

            app.UseStaticFiles()
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");

                    routes.MapSpaFallbackRoute(
                        name: "spa-fallback",
                        defaults: new {controller = "Home", action = "Index"});
                });
        }
    }
}