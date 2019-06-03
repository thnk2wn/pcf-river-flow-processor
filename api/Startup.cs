using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Mapping;
using RiverFlowApi.Data.Query;
using RiverFlowApi.Data.Services;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
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
            services.AddScoped<IStateFlowSummaryMapper, StateFlowSummaryMapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var serviceProvider = app.ApplicationServices;
            var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();

            if (env.IsDevelopment() || env.IsEnvironment("Local"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var useSwagger = Convert.ToBoolean(Configuration["Meta:SwaggerEnabled"]);

            if (useSwagger)
            {
                logger.LogInformation("Setting up Swagger");
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RiverFlow API V1");
                });
            }
            else
            {
                logger.LogInformation("Swagger is disabled; skipping setup");
            }

            var addStackifyRequestTracer = Convert.ToBoolean(
                Configuration["Middleware:AddStackifyRequestTracer"]);

            if (addStackifyRequestTracer)
            {
                logger.LogInformation("Adding Stackify tracking middleware");
                app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "/riverflow");
            });

            logger.LogInformation("Using service registry via discovery client");
            app.UseDiscoveryClient();

            var dbCreateTimeout = Convert.ToInt32(Configuration["Database:CreateTimeoutSeconds"]);
            logger.LogInformation("Ensuring DB is setup. Create timeout: {timeout}", dbCreateTimeout);

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RiverDbContext>();
                context.Database.SetCommandTimeout(dbCreateTimeout);
                context.Database.EnsureCreated();
            }

            logger.LogInformation("Startup complete");
        }
    }
}
