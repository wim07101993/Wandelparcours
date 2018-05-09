using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using WebService.Helpers.Extensions;
using WebService.Helpers.JsonConverters;
using WebService.Middleware;
using WebService.Services.Data;

namespace WebService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCors(options =>options.AddPolicy("AllowAllMethods",
                    builder =>
                    {
                        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                    })
                )
                .UseServices()
                .AddMvc(
                    options =>
                    {
                        options.Filters.Add<ReturnCreatedIfPostSucceedsPipeline>();
                        options.Filters.Add<AuthPipelineFilter>();
                    })
                .AddJsonOptions(
                    options =>
                    {
                        options.SerializerSettings.Converters.Add(new ObjectIdConverter());
                        options.SerializerSettings.Converters.Add(new ObjectIdListConverter());
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDatabaseManager databaseManager)
        {
            databaseManager.ConfigureDB();
            databaseManager.ScheduleCleanup(TimeSpan.FromDays(1));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(option => option.AllowAnyOrigin().AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader())
                .UseStaticFiles()
                .UseExceptionMiddelware()
                .UseMvc();
        }
    }
}