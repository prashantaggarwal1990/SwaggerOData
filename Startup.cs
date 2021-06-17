using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using SwaggerOData.DbContexts;
using SwaggerOData.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerOData
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
            services.AddControllers(options => options.EnableEndpointRouting = false);
            services.AddSingleton(Configuration);
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("db"));
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddApiVersioning(options => options.ReportApiVersions = true);
            services.AddODataApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddOData().EnableApiVersioning();
            services.AddSwaggerServices(info =>
            {
                info.Title = info.Version == "1.0" ? "Test API (.NET Core 3.1) v1.0" : "Test API (.NET Core 3.1) another version";
                info.Description = info.Version == "1.0" ? "Our test harness API" : "Another description";
            }, "https://localhost:5001");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, VersionedODataModelBuilder modelBuilder)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var repo = serviceScope.ServiceProvider.GetRequiredService<IEmployeeRepository>();
                ((EmployeeRepository)repo).Initialize();
            }

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.Select().Count().Expand().OrderBy().Filter().SkipToken().MaxTop(null);
                routeBuilder.ServiceProvider.GetRequiredService<ODataOptions>().UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash;
                var versionedModels = modelBuilder.GetEdmModels();
                routeBuilder.MapVersionedODataRoutes("odata-bypath", "api/v{version:apiVersion}", versionedModels, null);

                routeBuilder.EnableDependencyInjection(x =>
                {
                    x.AddService<ODataUriResolver>(Microsoft.OData.ServiceLifetime.Singleton, s => new UnqualifiedCallAndEnumPrefixFreeResolver { EnableCaseInsensitive = true });
                });
            });

            app.AddSwaggerMiddleware();
            app.UseHttpsRedirection();
        }
    }
}
