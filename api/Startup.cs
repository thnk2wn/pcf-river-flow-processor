using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverFlowApi.Data.Entities;
using RiverFlowApi.Data.Services;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
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
            services.AddDbContext<RiverDbContext>(options => options.UseMySql(Configuration));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IFlowRecordingService, FlowRecordingService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "RiverFlow API", Version = "v1" });
            });
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "/riverflow");
            });

            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RiverDbContext>();
                // context.Database.GetDbConnection().ConnectionString
                context.Database.SetCommandTimeout(90);
                context.Database.EnsureCreated();
            }
        }
    }
}
