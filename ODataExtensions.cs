using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SwaggerOData
{
    public static class ODataExtensions
	{
		private static readonly Type _modelConfigurationInterfaceType = typeof(IModelConfiguration);

		public static IODataBuilder AddPlatformOData(this IServiceCollection services)
		{

			services.AddODataApiExplorer(options =>
			{
				options.GroupNameFormat = "'v'VVV";
				options.SubstituteApiVersionInUrl = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
			});

			services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
			return services.AddOData();
		}

		public static void UsePlatformOData(this IRouteBuilder routeBuilder, VersionedODataModelBuilder modelBuilder, string routePrefix = "api/v{version:apiVersion}")
		{
			routeBuilder.Select().Count().Expand().OrderBy().Filter().SkipToken().MaxTop(null);

			routeBuilder.ServiceProvider.GetRequiredService<ODataOptions>().UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash;

			// setup the routing
			var versionedModels = modelBuilder.GetEdmModels();
			routeBuilder.MapVersionedODataRoutes("odata-bypath", routePrefix, versionedModels, null);

			routeBuilder.EnableDependencyInjection(x =>
			{
				x.AddService<ODataUriResolver>(Microsoft.OData.ServiceLifetime.Singleton, s => new UnqualifiedCallAndEnumPrefixFreeResolver { EnableCaseInsensitive = true });
			});
		}
	}
}
