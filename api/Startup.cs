using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RiverflowApi.Data.Services;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Mapping;
using RiverFlowApi.Data.Query;
using RiverFlowApi.Data.Query.Gauge;
using RiverFlowApi.Data.Query.River;
using RiverFlowApi.Data.Query.State;
using RiverFlowApi.Data.Services;
using RiverFlowApi.Http;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using Steeltoe.Discovery.Client;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;

namespace RiverFlowApi
{
    public class Startup
    {
        private readonly bool useSwagger;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.useSwagger = Convert.ToBoolean(Configuration["Meta:SwaggerEnabled"]);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<RiverDbContext>(options => options.UseMySql(Configuration));

            if (this.useSwagger)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new Info
                        {
                            Title = "RiverFlow API",
                            Version = "v1",
                            Description = "Endpoints for recording and retrieving USGS river gauge readings and related data"
                        });

                    c.ExampleFilters();

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });

                services.AddSwaggerExamplesFromAssemblyOf<Startup>();
            }

            services
                .AddDiscoveryClient(Configuration)
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddScoped<IFlowRecordingService, FlowRecordingService>();
            services.AddScoped<IStateFlowSummaryQuery, StateFlowSummaryQuery>();
            services.AddScoped<IStateFlowSummaryMapper, StateFlowSummaryMapper>();
            services.AddScoped<IRiverQuery, RiverQuery>();
            services.AddScoped<IStateQuery, StateQuery>();
            services.AddScoped<IGaugeQuery, GaugeQuery>();
            services.AddScoped<IHypermediaService, HttpHypermediaService>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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

            if (this.useSwagger)
            {
                logger.LogInformation("Setting up Swagger");
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RiverFlow API V1");
                    c.RoutePrefix = string.Empty;
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
