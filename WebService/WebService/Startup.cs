using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Data.Mock;
using WebService.Services.Data.Mongo;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

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
            services
                .AddSingleton(typeof(ILogger), new LoggerCollection {new ConsoleLogger(), new FileLogger()})
                .AddSingleton<IThrow, Throw>()
                //.AddSingleton<IDataService<Resident>, MockResidentsService>()
                //.AddSingleton<IDataService<ReceiverModule>, ReceiverModulesService>();
                .AddSingleton<IDataService<Resident>, ResidentsService>()
                .AddSingleton<IDataService<ReceiverModule>, ReceiverModulesService>();

            services
                .AddMvc()
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

            app.UseCors((option) => { option.AllowAnyOrigin().AllowAnyMethod(); });

            app.UseExceptionMiddleware()
                .UseStaticFiles()
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