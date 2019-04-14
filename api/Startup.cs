using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Query;
using RiverFlowApi.Data.Services;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery.Client;
using Swashbuckle.AspNetCore.Swagger;

namespace RiverFlowApi
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
                .AddDbContext<RiverDbContext>(options => options.UseMySql(Configuration))
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "RiverFlow API", Version = "v1" });
                })
                .AddDiscoveryClient(Configuration)
                .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddScoped<IFlowRecordingService, FlowRecordingService>();
            services.AddScoped<IStateFlowSummaryQuery, StateFlowSummaryQuery>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RiverFlow API V1");
            });

#if DEBUG
            Console.WriteLine("Adding Stackify tracking middleware");
            app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
#endif

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "/riverflow");
            });

            var serviceProvider = app.ApplicationServices;
            var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();

            logger.LogInformation("Using service registry via discovery client");
            app.UseDiscoveryClient();

            logger.LogInformation("Ensuring DB is setup");
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RiverDbContext>();
                context.Database.SetCommandTimeout(90);
                context.Database.EnsureCreated();
            }

            logger.LogInformation("Startup complete");
        }
    }
}
