using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SwaggerOData
{
    internal class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateOpenApiInfo(description));
            }
        }

        public static OpenApiInfo CreateOpenApiInfo(ApiVersionDescription description = null)
        {
            var info = new OpenApiInfo()
            {
                Title = "Swagger OData API",
                Version = description?.ApiVersion?.ToString() ?? "1.0"
            };

            if (description?.IsDeprecated == true)
                info.Description += " This API version has been deprecated.";

            return info;
        }
    }
}
