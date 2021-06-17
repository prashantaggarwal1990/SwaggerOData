using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SwaggerOData
{
	public static class SwaggerExtensions
	{
		public static void AddSwaggerServices(this IServiceCollection services, Action<OpenApiInfo> configureOpenApi, string apiUrl)
		{
			services.AddSwaggerGen(options =>
			{
				options.InitialiseSwaggerDocument(configureOpenApi);
				options.EnableAnnotations();
				options.AddXmlDocumentation();
				options.AddApiHostingServer(apiUrl);
				options.CustomSchemaIds(type => type.FullName);
				options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
			});

			services.AddFormatters();
		}

		private static void AddSwaggerMiddleware(this IApplicationBuilder app, string swaggerBasePath,
			Func<SwaggerUIOptions, (string url, string name)[]> endpoints)
		{
			const char separator = '/';
			swaggerBasePath = GetApiSuffix(swaggerBasePath, separator);
			app.UseSwagger(c =>
			{
				c.RouteTemplate = swaggerBasePath + "/{documentName}/swagger.json";
			});

			app.UseSwaggerUI(options =>
			{
				options.DisplayRequestDuration();
				options.ShowExtensions();

				foreach (var (url, name) in endpoints(options))
					options.SwaggerEndpoint(separator + swaggerBasePath + url, name);

				options.RoutePrefix = $"{swaggerBasePath}";
			});
		}

		public static void AddSwaggerMiddleware(this IApplicationBuilder app, string apiVersion, string name, string swaggerBasePath = "")
		{
			app.AddSwaggerMiddleware(swaggerBasePath, o => new[] { ($"/{apiVersion}/swagger.json", name) });
		}

		public static void AddSwaggerMiddleware(this IApplicationBuilder app, string swaggerBasePath = "")
		{
			app.AddSwaggerMiddleware(swaggerBasePath, options =>
			{
				IApiVersionDescriptionProvider provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
				if (provider == null)
					return new[] { ($"/swagger.json", options.DocumentTitle) };

				return provider.ApiVersionDescriptions.Select(description => ($"/{description.GroupName}/swagger.json",
					description.GroupName)).ToArray();
			});
		}

		private static void InitialiseSwaggerDocument(this SwaggerGenOptions options, Action<OpenApiInfo> configureOpenApi)
		{
			if (options.SwaggerGeneratorOptions.SwaggerDocs.Count == 0)
			{
				var info = ConfigureSwaggerOptions.CreateOpenApiInfo();
				options.SwaggerDoc(info.Version, info);
			}

			if (configureOpenApi != null)
			{
				foreach (var info in options.SwaggerGeneratorOptions.SwaggerDocs.Values)
					configureOpenApi(info);
			}
		}

		private static void AddFormatters(this IServiceCollection services)
		{
			services.AddMvcCore(options =>
			{
				foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
				{
					outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
				}
				foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
				{
					inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
				}
			});
		}

		private static void AddXmlDocumentation(this SwaggerGenOptions options)
		{
			var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName()?.Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
			if (File.Exists(xmlPath))
				options.IncludeXmlComments(xmlPath);
		}

		private static void AddApiHostingServer(this SwaggerGenOptions options, string apiUrl) => options.AddServer(new OpenApiServer() { Url = apiUrl });

		private static string GetApiSuffix(string swaggerBasePath, char separator)
		{
			swaggerBasePath = (swaggerBasePath ?? string.Empty).Trim().Trim(separator);

			if (Uri.TryCreate(swaggerBasePath, UriKind.Absolute, out var uri))
			{
				swaggerBasePath = uri.Segments.Last();

				if (swaggerBasePath.Equals("api", StringComparison.OrdinalIgnoreCase) || swaggerBasePath == separator.ToString())
					swaggerBasePath = string.Empty;
			}

			return string.IsNullOrWhiteSpace(swaggerBasePath) ? "swagger" : swaggerBasePath + "/swagger";
		}
	}
}
